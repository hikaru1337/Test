using DarlingDb;
using DarlingDb.Models;
using DarlingNet.Modules;
using DarlingNet.Services.LocalService.GetOrCreate;
using DarlingNet.Services.LocalService.VerifiedAction;
using Discord.Rest;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using static DarlingDb.Enums;

namespace DarlingNet.Services.LocalService
{
    public class DataBaseCheck
    {
        private readonly DiscordShardedClient _shard;
        private readonly Stopwatch sw = new();
        private bool FirstScanning;

        public DataBaseCheck(DiscordShardedClient shard)
        {
            _shard = shard;
        }

        internal Task StartTimerDataCheck()
        {
            for (; ;)
            {
                if(BotSettings._totalShards == BotSettings.ShardReadyCount)
                {
                    FirstScanning = true;
                    Run();
                    Timer TaskTime = new(3600000);
                    TaskTime.Elapsed += (s, e) => Run();
                    TaskTime.Start();
                    return Task.CompletedTask;
                }
            }
        }
        private void Run()
        {
            GuildLeaved = new List<ulong>();
            BotSettings.BotReady = false;
            sw.Start();
            GuildScanning();
            LogSend($"Проверка гильдий - {sw.Elapsed}");
            sw.Restart();
            GuildComponentScanning();
            LogSend($"Проверка компонентов гильдий - {sw.Elapsed}");
            if (!FirstScanning)
            {
                sw.Restart();
                GuildDataDelete();
                LogSend($"Удаление данных гильдий - {sw.Elapsed}");
            }
            sw.Stop();
            BotSettings.BotReady = true;
            FirstScanning = false;
        }

        private static void LogSend(string text) => Console.WriteLine($"{DateTime.Now:dd.MM.yy HH:mm:ss} [DataBase Clear]: {text}");

        private List<ulong> GuildLeaved = new();

        private async void GuildDataDelete()
        {
            using (db _db = new())
            {
                foreach (var GuildId in GuildLeaved)
                {
                    var Guild = _db.Guilds.Include(x => x.Guilds_Warns)
                                          .Include(x => x.Captcha)
                                          .Include(x => x.Guilds_Raid)
                                          .Include(x => x.Channels)
                                          .Include(x => x.Role)
                                          .Include(x => x.Reports)
                                          .Include(x => x.Guilds_Captcha)
                                          .Include(x => x.Users_Guild).ThenInclude(x => x.TempUser)
                                          .Include(x => x.Users_Guild).ThenInclude(x => x.PrivateChannels)
                                          .Include(x => x.Users_Guild).ThenInclude(x => x.Invites)
                                          .FirstOrDefault(x => x.Id == GuildId);

                    _db.PrivateChannels.RemoveRange(Guild.Users_Guild.Where(x => x.PrivateChannels != null).Select(x => x.PrivateChannels));
                    _db.Invites.RemoveRange(Guild.Users_Guild.SelectMany(x => x.Invites));
                    _db.TempUser.RemoveRange(Guild.Users_Guild.SelectMany(x => x.TempUser));

                    _db.Reports.RemoveRange(Guild.Reports);
                    _db.Captcha.RemoveRange(Guild.Captcha);
                    _db.Role.RemoveRange(Guild.Role);
                    _db.Channels.RemoveRange(Guild.Channels);
                    _db.Guilds_Warns.RemoveRange(Guild.Guilds_Warns);

                    if (Guild.Guilds_Raid != null)
                        _db.Guilds_Raid.Remove(Guild.Guilds_Raid);

                    if (Guild.Guilds_Captcha != null)
                        _db.Guilds_Captcha.Remove(Guild.Guilds_Captcha);

                    await _db.SaveChangesAsync();
                }
            }

        }

        private async void GuildScanning()
        {
            using (db _db = new())
            {
                foreach (var Guild in _db.Guilds.Include(x => x.Users_Guild))
                {
                    var GuildDiscord = _shard.GetGuild(Guild.Id);
                    if (GuildDiscord == null)
                    {
                        if (!Guild.Leaved)
                        {
                            Guild.Leaved = true;
                            GuildLeaved.Add(Guild.Id);
                        }

                        var Users = Guild.Users_Guild.Where(x => !x.Leaved);
                        if (Users.Any())
                        {
                            foreach (var User in Users)
                                User.Leaved = true;

                            _db.Users_Guild.UpdateRange(Users);
                        }

                    }
                    else
                    {
                        if (Guild.Leaved)
                            Guild.Leaved = false;

                        if (Guild.Users_Guild.Count > 0)
                        {
                            foreach (var User in Guild.Users_Guild)
                            {
                                var UserGuild = GuildDiscord.GetUser(User.Id);
                                if (UserGuild != null)
                                    User.Leaved = true;
                                else
                                    User.Leaved = false;

                            }
                            _db.Users_Guild.UpdateRange(Guild.Users_Guild);
                        }
                    }
                    _db.Guilds.Update(Guild);
                }
                await _db.SaveChangesAsync();


                foreach (var Guild in _shard.Guilds)
                {
                    if (!_db.Guilds.Any(x => x.Id == Guild.Id))
                        await _db.Guilds.GetOrCreate(Guild.Id);
                }
            }
        }

        private async void GuildComponentScanning()
        {
            using (db _db = new())
            {
                if (FirstScanning)
                {
                    foreach (var Channel in _db.Channels)
                    {
                        var ChannelDiscord = _shard.GetChannel(Channel.Id) as SocketTextChannel;
                        if (ChannelDiscord == null)
                            _db.Channels.Remove(Channel);

                        // ButtonClick
                        // Channels
                        // EmoteClick
                        // GiveAways
                        // Guild_Logs
                        // Guild_Meeting
                        // Invites 50/50
                        // Tasks
                        // TempUser 50/50
                    } // Проверка текстовых каналов

                    foreach (var Role in _db.Role)
                    {
                        var RoleDiscord = _shard.GetGuild(Role.GuildsId)?.GetRole(Role.Id);
                        if (RoleDiscord == null)
                            _db.Role.Remove(Role);

                        // Role
                        // Roles
                        // Roles_Timer
                        // Guild_Meeting_Welcome
                    } // проверка временных/платых/выдающихся ролей

                    _db.SaveChanges();

                    foreach (var Task in _db.Tasks)
                    {
                        if (Task.Times < DateTime.Now && !Task.Repeat)
                            _db.Tasks.Remove(Task);
                        else
                            await TaskTimer.StartTaskTimer(Task);
                    } // Временные задачи

                    foreach (var Time in _db.Roles_Timer) // Временные роли
                    {
                        if (Time.ToTime < DateTime.Now)
                            TaskTimer.TimeRole(Time);
                        else
                            await TaskTimer.StartTimeRole(Time);
                    }

                    foreach (var TempUser in _db.TempUser.Include(x => x.Users_Guild))
                    {
                        bool Remove = false;
                        var Guild = _shard.GetGuild(TempUser.Users_Guild.GuildsId);
                        if (Guild != null)
                        {
                            var User = Guild.GetUser(TempUser.Users_GuildId);
                            if (User != null)
                            {
                                switch (TempUser.Reason)
                                {
                                    case ReportTypeEnum.TimeBan:
                                        {
                                            var Ban = Guild.GetBanAsync(User);
                                            if (DateTime.Now > TempUser.ToTime || Ban == null)
                                            {
                                                if (Ban != null)
                                                    await Guild.RemoveBan(User.Id);
                                                Remove = true;
                                            }
                                            else
                                                await TaskTimer.StartBanTimer(TempUser);
                                        }
                                        break;
                                    case ReportTypeEnum.Mute:
                                        {
                                            await MuteUpdate(TempUser, User);
                                        }
                                        break;
                                    case ReportTypeEnum.TimeOut:
                                        {
                                            if (TempUser.ToTime > DateTime.Now || User.TimedOutUntil == null)
                                                Remove = true;
                                        }
                                        break;
                                }
                            }
                            else
                                Remove = true;
                        }
                        else
                            Remove = true;

                        if (Remove)
                            _db.TempUser.Remove(TempUser);
                    } // Проверка временных мутов

                    #region День рождения
                    int year = DateTime.Now.Year + 1;
                    var BirtDateUsers = _db.Users.Where(x => x.BirthDate.Year != 1 && x.BirthDateComplete < year).AsEnumerable();
                    BirtDateUsers = BirtDateUsers.Where(x => x.BirthDate.Month == DateTime.Now.Month && DateTime.Now.Day >= x.BirthDate.Day);
                    foreach (var User in BirtDateUsers)
                    {
                        await TaskTimer.StartBirthdates(User);
                    }
                    #endregion

                    foreach (var Give in _db.GiveAways)
                    {
                        var Message = await (_shard.GetChannel(Give.ChannelId) as SocketTextChannel)?.GetMessageAsync(Give.Id);
                        if (Message != null)
                            await Giveaway.GiveAwayTimer(Give, Message as RestUserMessage);
                        else
                            _db.GiveAways.Remove(Give);
                    } // проверка розыгрышей

                    foreach (var Guilds in _shard.Guilds)
                    {
                        foreach (var VC in Guilds.VoiceChannels.Where(x => x.Users.Any()))
                        {
                            foreach (var User in VC.Users)
                            {
                                await TaskTimer.StartVoiceActivity(User);
                            }
                        } // Включение опыта в голосовых каналах

                        if (Guilds.DarlingBoostGet() == null)
                        {
                            var BotPermission = Guilds.CurrentUser.GuildPermissions;
                            if (BotPermission.ManageGuild || BotPermission.Administrator)
                            {
                                var Invite = Guilds.GetInvitesAsync();
                                foreach (var Invites in Invite.Result)
                                {
                                    var InviteDB = _db.Invites.FirstOrDefault(x => x.Key == Invites.Code);
                                    if (InviteDB == null)
                                    {
                                        ulong GuildId = Convert.ToUInt64(Invites.GuildId);
                                        var User = await _db.Users_Guild.GetOrCreate(Invites.Inviter.Id, GuildId, Invites.Inviter.IsBot);
                                        var Channel = await _db.Channels.GetOrCreate(GuildId, Invites.ChannelId);
                                        var NewInvite = new Invites() { Users_GuildId = User.Id, Key = Invites.Code, UsesCount = (int)Invites.Uses, ChannelId = Invites.ChannelId };
                                        _db.Invites.Add(NewInvite);
                                    }
                                    else if (InviteDB.UsesCount != Invites.Uses)
                                    {
                                        InviteDB.UsesCount = (int)Invites.Uses;
                                        _db.Invites.Update(InviteDB);
                                    }
                                }
                            }
                        } // ПРОВЕРКА ИНВАЙТОВ
                    }
                }

                foreach (var PC in _db.PrivateChannels.Include(x => x.Users_Guild))
                {
                    var VoiceChannel = _shard.GetChannel(PC.Id) as SocketVoiceChannel;
                    if (VoiceChannel != null)
                        await PrivateSystem.Privatemethod(VoiceChannel, PC);
                    else
                        _db.PrivateChannels.Remove(PC);
                } // Проверка приватных каналов

                var CaptchaNotValid = _db.Captcha.Where(x => x.TimeToClear > DateTime.Now);
                _db.Captcha.RemoveRange(CaptchaNotValid);

                foreach (var Captcha in _db.Guilds_Captcha)
                {
                    if (Captcha.Run && Captcha.RoleId != null && Captcha.ChannelId != null)
                    {
                        var Guild = _shard.GetGuild(Captcha.GuildId);
                        var Channel = Guild?.GetTextChannel(Convert.ToUInt64(Captcha.ChannelId));
                        var Role = Guild?.GetRole(Convert.ToUInt64(Captcha.RoleId));
                        if (Channel != null && Role != null)
                            await CapthaService.PermissionUpdate(Channel, Role);
                    }
                }

                foreach (var TempUser in _db.TempUser.Include(x => x.Users_Guild).Where(x => x.Reason == ReportTypeEnum.Mute))
                {
                    var User = _shard.GetGuild(TempUser.Users_Guild.GuildsId)?.GetUser(TempUser.Users_GuildId);
                    if (User != null)
                        await MuteUpdate(TempUser, User);
                }

                await _db.SaveChangesAsync();
            }
        }

        private static async Task MuteUpdate(TempUser TempUser, SocketGuildUser User)
        {
            using (db _db = new())
            {
                async void DataBaseMethod(bool Update = false)
                {
                    if(Update)
                        _db.TempUser.Update(TempUser);
                    else
                        _db.TempUser.Remove(TempUser);

                    await _db.SaveChangesAsync();
                }


                if (TempUser.ToTime > DateTime.Now)
                {
                    if ((TempUser.ToTime - DateTime.Now).TotalDays < 7)
                    {
                        TempUser.ToTime = TempUser.ToTime.AddDays(20);
                        await User.AddMute(TempUser.ToTime - DateTime.Now);
                        DataBaseMethod(true);
                    }
                }
                else
                    DataBaseMethod(false);
            }
        }
    }
}
