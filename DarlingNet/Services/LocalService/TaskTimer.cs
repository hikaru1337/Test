using DarlingDb.Models;
using DarlingDb;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;
using System.Timers;
using System.Linq;
using DarlingNet.Services.LocalService.VerifiedAction;
using DarlingNet.Services.LocalService.GetOrCreate;
using Discord;
using Microsoft.EntityFrameworkCore;
using static DarlingDb.Enums;

namespace DarlingNet.Services.LocalService
{
    public class TaskTimer
    {
        public static DiscordShardedClient _client;

        public TaskTimer(DiscordShardedClient client)
        {
            _client = client;
        }
        private static int Status = 0;
        public Task StartChangeStatus()
        {
            Timer TaskTime = new(15000);
            TaskTime.Elapsed += (s, e) => ChangeStatus();
            TaskTime.Start();
            return Task.CompletedTask;
        }
        private static async void ChangeStatus()
        {
            Status++;
            string text = string.Empty;
                switch (Status)
                {
                    case 1:
                        text = "docs.darlingbot.ru";
                        break;
                    case 2:
                        text = $"darling!!!";
                        break;
                    case 3:
                        text = $"Хикару сказал ты дурак!";
                        Status = 0;
                        break;
                }
            
            
            await _client.SetGameAsync(text, null, ActivityType.Playing);  
        }

        // Опыт за активность в Голосовых чатах
        internal static Task StartVoiceActivity(SocketGuildUser User)
        {
            Timer TaskTime = new(10000);
            TaskTime.Elapsed += (s, e) => VoiceActivity(s, User);
            TaskTime.Start();
            return Task.CompletedTask;
        }
        private static async void VoiceActivity(object source, SocketGuildUser User)
        {
            using (db _db = new ())
            {
                if (!BotSettings.BotReady)
                {
                    var Guild = await _db.Guilds.GetOrCreate(User.Guild.Id);
                    if (User.VoiceChannel != null && User.VoiceChannel.Id != User.Guild.AFKChannel?.Id &&
                       Guild.VoiceAndCategoryChannelList.Any(x => x == User.VoiceChannel.Id || x == User.VoiceChannel.CategoryId))
                    {
                        if (User.VoiceChannel.Users.Count > 1)
                        {
                            uint CountSpeak = 0;
                            bool ThisUserActive = false;
                            foreach (var UserChannel in User.VoiceChannel.Users)
                            {
                                var UserStatus = UserChannel.VoiceState.Value;
                                if (!UserStatus.IsMuted && !UserStatus.IsDeafened &&
                                    !UserStatus.IsSelfMuted && !UserStatus.IsSelfDeafened)
                                {
                                    CountSpeak++;

                                    if (UserChannel.Id == User.Id)
                                        ThisUserActive = true;
                                }
                            }

                            if (ThisUserActive && CountSpeak > 1)
                            {
                                var user = await _db.Users_Guild.GetOrCreate(User.Id,User.Guild.Id);
                                user.VoiceActive += new TimeSpan(0, 0, 10);
                                user.XP += 10;
                                _db.Users_Guild.Update(user);
                                await _db.SaveChangesAsync();
                            }
                        }
                    }
                    else
                        (source as Timer).Stop();
                }

            }
        }
        // Опыт за активность в Голосовых чатах

        internal static Task StartTimeRole(Roles_Timer Timer)
        {
            Timer TaskTime = new((Timer.ToTime - DateTime.Now).TotalMilliseconds);
            TaskTime.AutoReset = false;
            TaskTime.Elapsed += (s, e) => TimeRole(Timer); /*new ElapsedEventHandler(TimeTempMuted);*/
            TaskTime.Start();
            return Task.CompletedTask;
        }
        internal static async void TimeRole(Roles_Timer Timer)
        {
            using (db _db = new ())
            {
                var TimeNow = _db.Roles_Timer.Include(x=>x.Role).FirstOrDefault(x => x == Timer);
                if (TimeNow != null)
                {
                    var User = _client.GetGuild(TimeNow.Role.GuildsId)?.GetUser(TimeNow.Users_GuildId);
                    if (User != null)
                    {
                        await User.RemoveRole(TimeNow.RoleId);
                        _db.Roles_Timer.Remove(TimeNow);
                        await _db.SaveChangesAsync();
                    }
                }
            }
        }
        // Запуск временных ролей


        // Запуск временных нарушений
        internal static Task StartBirthdates(Users User)
        {
            var Time = User.BirthDate.AddYears(DateTime.Now.Year - User.BirthDate.Year) - DateTime.Now;
            Timer TaskTime = new (Time.TotalMilliseconds < 0 ? 1000 : Time.TotalMilliseconds);
            TaskTime.AutoReset = false;
            TaskTime.Elapsed += (s, e) => Birthdates(User);
            TaskTime.Start();
            return Task.CompletedTask;
        }
        private static async void Birthdates(Users User)
        {
            using (db _db = new ())
            {
                User = _db.Users.Include(x=>x.Users_Guild).FirstOrDefault(x => x.Id == User.Id);
                var emb = new EmbedBuilder().WithColor(255, 0, 94).WithAuthor("С днем рождения солнышко🎉");
                var UserDiscord = _client.GetUser(User.Id);
                if(UserDiscord != null)
                {
                    DateTime TimeNow = DateTime.Now;
                    if (TimeNow.Day <= User.BirthDate.Day)
                        emb.WithDescription("Солнышко, я слышал у тебя сегодня день рождения?\n\nЯ робот, и не очень хорошо умею говорить умные слова,\nно спасибо что ты есть, не опускай руки, я тебя люблю <3");
                    else
                        emb.WithDescription("Солнышко, я слышал у тебя был день рождения?\nС днем рождения солнце!!!\n\nПрости не смог поздравить тебя сразу, надеюсь за меня это сделали твои друзья :)");
                    await UserDiscord.UserMessage("", emb);

                    User.BirthDateComplete = (ushort)(TimeNow.Year + 1);
                    _db.Users.Update(User);
                    await _db.SaveChangesAsync();
                    foreach (var UserGuild in User.Users_Guild)
                    {
                        if (!UserGuild.BirthDateInvise)
                        {
                            var UserDc = _client.GetGuild(UserGuild.GuildsId)?.GetUser(User.Id);
                            if (UserDc != null)
                            {
                                var GuildLogs = _db.Guilds_Logs.GetLogChannel(UserGuild.GuildsId, ChannelsTypeEnum.BirthDay);
                                if (GuildLogs != null)
                                {
                                    var Channel = UserDc.Guild.GetTextChannel(GuildLogs.ChannelId);
                                    if (Channel != null)
                                    {
                                        var Prefix = _db.Guilds.GetPrefix(UserGuild.GuildsId);
                                        emb.WithDescription($"У пользователя 🎊{UserDc.Mention}🎊 сегодня день рождения\nдавайте вместе поздравим его🎊").WithFooter($"Включить себе - {Prefix}pf");
                                        await Channel.Message($"{UserDc.Mention}", emb);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        // Запуск поздравлений


        // Запуск временных нарушений
        internal static Task StartBanTimer(TempUser TempedUser)
        {
            Timer TaskTime = new((TempedUser.ToTime - DateTime.Now).TotalMilliseconds);
            TaskTime.AutoReset = false;
            TaskTime.Elapsed += (s, e) => BanTimer(TempedUser);
            TaskTime.Start();
            return Task.CompletedTask;
        }
        private static async void BanTimer(TempUser Temped)
        {
            using (db _db = new ())
            {
                var User = _client.GetGuild(Temped.Users_Guild.GuildsId)?.GetUser(Temped.Users_GuildId);
                if (User != null)
                    await User.Guild.RemoveBan(User.Id);

                Temped = _db.TempUser.FirstOrDefault(x => x.Id == Temped.Id);
                if (Temped != null)
                {
                    _db.TempUser.Remove(Temped);
                    await _db.SaveChangesAsync();
                }
            }
        }
        // Запуск временных нарушений



        private static double GetMillisecondsTimer(Tasks Tasked)
        {
            DateTime nowTime = DateTime.Now;
            DateTime oneAmTime = Tasked.Times;
            if (Tasked.Repeat)
            {
                oneAmTime = new DateTime(nowTime.Year, nowTime.Month, nowTime.Day, Tasked.Times.Hour, Tasked.Times.Minute, Tasked.Times.Second);
                if (nowTime > oneAmTime)
                    oneAmTime = oneAmTime.AddDays(1);
            }

            return (oneAmTime - nowTime).TotalMilliseconds;
        }

        public static Task StartTaskTimer(Tasks Tasked)
        {
            Timer TaskTime = new(GetMillisecondsTimer(Tasked));
            TaskTime.AutoReset = Tasked.Repeat;
            TaskTime.Elapsed += (s, e) => TaskTimers(Tasked,s);
            TaskTime.Start();
            return Task.CompletedTask;
        }
        private static async void TaskTimers(Tasks Tasked, object T)
        {
            using (db _db = new ())
            {
                var Task = _db.Tasks.FirstOrDefault(x => x == Tasked);
                if (Task != null)
                {
                    
                    var Channel = _client.GetChannel(Task.ChannelId) as SocketTextChannel;
                    bool DeleteTask = false;
                    if (Channel != null)
                    {
                        string Result = await Channel.Message(Task.Message);

                        if (!string.IsNullOrWhiteSpace(Result))
                            await Channel.JsonMessageToEmbed(Task.Message);
                    }
                    else
                        DeleteTask = true;

                    if (!Task.Repeat)
                        DeleteTask = true;
                    else
                    {
                        var Timer = T as Timer;
                        Timer.Interval += GetMillisecondsTimer(Tasked);
                    }

                    if (DeleteTask)
                    {
                        _db.Remove(Task);
                        await _db.SaveChangesAsync();
                    }
                }
            }
        }
        // Запуск Задач
    }
}
