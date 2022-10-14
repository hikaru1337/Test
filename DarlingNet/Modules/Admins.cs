using DarlingDb;
using DarlingDb.Models;
using DarlingDb.Models.ReportSystem;
using DarlingNet.Services.LocalService;
using DarlingNet.Services.LocalService.Attribute;
using DarlingNet.Services.LocalService.GetOrCreate;
using DarlingNet.Services.LocalService.VerifiedAction;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using static DarlingDb.Enums;
using static DarlingNet.Services.LocalService.Attribute.CommandLocksAttribute;

namespace DarlingNet.Modules
{
    [RequireBotPermission(ChannelPermission.SendMessages)]
    [Summary("Команды для\nадминистрирования")]
    public class Admins : ModuleBase<ShardedCommandContext>
    {

        [Aliases, Commands, Usage, Descriptions]
        [PermissionBlockCommand]
        [PermissionHierarchy]
        [RequireUserPermission(GuildPermission.BanMembers)]
        [RequireBotPermission(GuildPermission.BanMembers)]
        [RequireBotPermission(ChannelPermission.EmbedLinks)]
        public async Task ban(ulong UserId, uint DeleteMessageDays = 0, [Remainder] string Reason = null)
        {
            var emb = new EmbedBuilder().WithColor(255, 0, 94).WithAuthor($"{UserId} бан");
            if (DeleteMessageDays <= 7)
            {
                emb.WithDescription($"Пользователь <@{UserId}> получил Ban {(Reason != null ? $"\nПричина: {Reason}" : "")}");
                await Context.Guild.AddBan(UserId, (int)DeleteMessageDays, Reason);
                var User = Context.Guild.GetUser(UserId);
                if (User != null)
                {
                    var embb = new EmbedBuilder().WithDescription($"Вы были забанены на сервере {Context.Guild.Name}{(Reason != null ? $"\nПричина: {Reason}" : "")}");
                    var UserLock = await User.UserMessage("", embb);
                    if (UserLock != null)
                        emb.Description += "\nСообщение о бане, не дошло пользователю!";
                }
            }
            else emb.WithDescription("Вы не можете удалить сообщения больше чем за 7 дней").WithAuthor("Бан [ошибка]");
            await Context.Channel.SendMessageAsync("", false, emb.Build());
        }

        [Aliases, Commands, Usage, Descriptions]
        [PermissionBlockCommand]
        [RequireUserPermission(GuildPermission.BanMembers)]
        [RequireBotPermission(GuildPermission.BanMembers)]
        [RequireBotPermission(ChannelPermission.EmbedLinks)]
        public async Task unban(ulong UserId)
        {
            var emb = new EmbedBuilder().WithColor(255, 0, 94).WithAuthor($"{UserId} разбан");
            var text = await Context.Guild.RemoveBan(UserId);
            if (text != null)
                emb.WithDescription(text);
            else
                emb.WithDescription($"Пользователь был разбанен!");
            await Context.Channel.SendMessageAsync("", false, emb.Build());
        }

        [Aliases, Commands, Usage, Descriptions]
        [PermissionBlockCommand, PermissionViolation]
        [RequireUserPermission(GuildPermission.KickMembers)]
        [RequireUserPermission(GuildPermission.BanMembers)]
        [RequireUserPermission(GuildPermission.MuteMembers)]
        [RequireBotPermission(GuildPermission.BanMembers)]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        [RequireBotPermission(GuildPermission.KickMembers)]
        [RequireBotPermission(ChannelPermission.EmbedLinks)]
        [PermissionHierarchy]
        public async Task warn(SocketGuildUser User)
        {
            using (db _db = new())
            {
                var emb = new EmbedBuilder().WithColor(255, 0, 94).WithAuthor("Warn");
                var GuildWarn = _db.Guilds_Warns.Where(x => x.GuildsId == User.Guild.Id);
                if (GuildWarn.Any())
                {
                    var UserDataBase = await _db.Users_Guild.GetOrCreate(User.Id, User.Guild.Id);

                    if (UserDataBase.CountWarns >= GuildWarn.Count())
                        UserDataBase.CountWarns = 1;
                    else
                        UserDataBase.CountWarns++;

                    _db.Users_Guild.Update(UserDataBase);
                    await _db.SaveChangesAsync();

                    emb.WithDescription($"Пользователь {User.Mention} получил {UserDataBase.CountWarns} нарушение");

                    var warn = _db.Guilds_Warns.FirstOrDefault(x => x.GuildsId == User.Guild.Id && x.CountWarn == UserDataBase.CountWarns);
                    if (warn != null)
                        await ReportUser(User, warn.ReportTypes, warn.Time);
                }
                else
                {
                    var Prefix = await _db.Guilds.GetPrefix(Context.Guild.Id);
                    emb.WithDescription("У вас еще нет нарушений для варнов!").WithFooter($"Подробнее `{Prefix}i addwarn`");
                }
                await Context.Channel.SendMessageAsync("", false, emb.Build());
            }
        }

        [Aliases, Commands, Usage, Descriptions]
        [PermissionBlockCommand, PermissionViolation]
        [RequireUserPermission(GuildPermission.KickMembers)]
        [RequireUserPermission(GuildPermission.BanMembers)]
        [RequireUserPermission(GuildPermission.MuteMembers)]
        [RequireBotPermission(GuildPermission.BanMembers)]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        [RequireBotPermission(GuildPermission.KickMembers)]
        [RequireBotPermission(ChannelPermission.EmbedLinks)]
        [PermissionHierarchy]
        public async Task report(SocketGuildUser User, ushort NumberRules)
        {
            using (db _db = new())
            {
                var emb = new EmbedBuilder().WithColor(255, 0, 94).WithAuthor("Репорт");
                if (NumberRules == 0)
                    NumberRules = 1;

                string Error = string.Empty;

                var Report = _db.Reports.Where(x => x.GuildsId == User.Guild.Id).Include(x => x.ReportsList);
                if (Report.Any())
                {
                    var ThisReport = Report.AsEnumerable().ElementAtOrDefault(NumberRules - 1);
                    if (ThisReport != null)
                    {
                        if (ThisReport.ReportsList.Count != 0)
                        {
                            Reports_Punishes ThisPunishes = null; // Обьявляем переменную для дальнейшей выдачи пизды
                            int CountPunishes = 0;
                            await _db.Users_Guild.GetOrCreate(User.Id, User.Guild.Id); // Достаем пользователя если его нету

                            var UserGuild = _db.Users_Guild.Include(x => x.Reports_Punishes).ThenInclude(x=>x.Users_Guild).FirstOrDefault(x => x.UsersId == User.Id && x.GuildsId == User.Guild.Id);
                            // Как долбаебы блять достаем его же еще раз и инклудим

                            var BeforePunishes = UserGuild.Reports_Punishes.FirstOrDefault(x => x.ReportId == ThisReport.Id); // Достаем из него нарушение по данному правилу

                            if (BeforePunishes != null)
                            {
                                BeforePunishes.Users_Guild.Remove(UserGuild); // Удаляем пользователя из прошлого нарушения
                                _db.Reports_Punishes.Update(BeforePunishes); // Обновляем
                                int IndexBeforePunishes = ThisReport.ReportsList.ToList().IndexOf(BeforePunishes); // узнаем индекс прошлого нарушения

                                switch (BeforePunishes.TypeReport)
                                {
                                    case ReportTypeEnum.TimeBan:
                                    case ReportTypeEnum.Ban:
                                        await Context.Guild.RemoveBan(User.Id);
                                        break;
                                    case ReportTypeEnum.TimeOut:
                                    case ReportTypeEnum.Mute:
                                        await User.RemoveTimeOutAsync();
                                        break;
                                }

                                if ((IndexBeforePunishes + 1) < ThisReport.ReportsList.Count)
                                    CountPunishes = IndexBeforePunishes + 1; // счетчик для сообщения + достать новое нарушение

                                ThisPunishes = ThisReport.ReportsList.ElementAtOrDefault(CountPunishes); // Достаем новое нарушение
                            } // Если у него уже было прошлое нарушение.
                            else
                            {
                                ThisPunishes = ThisReport.ReportsList.FirstOrDefault(); // Достаем первое нарушение
                            }// Если у него еще не было нарушения.

                            UserGuild.Reports_Punishes.Add(ThisPunishes); // Добавляем пользователю нарушение
                            _db.Users_Guild.Update(UserGuild);

                            await _db.SaveChangesAsync();

                            

                            emb.WithDescription($"Пользователь {User.Mention} получил {CountPunishes + 1} нарушение по правилу {NumberRules}");
                            await ReportUser(User, ThisPunishes.TypeReport, ThisPunishes.TimeReport);




                            //var UserDataBase = await _db.Users_Guild.GetOrCreate(User.Id, User.Guild.Id);

                            //var ThisUserRules = UserDataBase.ListReports.FirstOrDefault(x => x.RulesNumber == NumberRules - 1);
                            //if (ThisUserRules == null)
                            //{
                            //    var NewList = new List<Reports>();
                            //    NewList.Add(new Reports { RulesNumber = (uint)NumberRules - 1, ReportCount = 0 });
                            //    NewList.AddRange(UserDataBase.ListReports);
                            //    UserDataBase.ListReports = NewList;
                            //    ThisUserRules = UserDataBase.ListReports.FirstOrDefault(x => x.RulesNumber == NumberRules - 1);

                            //}


                            //if (ThisUserRules.ReportCount >= ThisReport.ReportsList.Count)
                            //    ThisUserRules.ReportCount = 1;
                            //else
                            //    ThisUserRules.ReportCount++;

                            //var Base = UserDataBase.ListReports.FirstOrDefault(x => x.RulesNumber == NumberRules - 1);
                            //var NewListBase = UserDataBase.ListReports.Where(x => x.RulesNumber != NumberRules - 1).ToList();
                            //Base.ReportCount = ThisUserRules.ReportCount;
                            //NewListBase.Add(Base);
                            //UserDataBase.ListReports = NewListBase;

                            //emb.WithDescription($"Пользователь {User.Mention} получил {ThisUserRules.ReportCount} нарушение по правилу {NumberRules}");

                            //_db.Users_Guild.Update(UserDataBase);
                            //var ThisReportX = ThisReport.ReportsList.ElementAtOrDefault((int)ThisUserRules.ReportCount - 1);

                            //await ReportUser(User,ThisReportX.TypeReport, ThisReportX.TimeReport);
                        }
                        else
                        {
                            Error = "На сервере еще нет нарушений для правил!";
                            emb.WithFooter($"arp");
                        }
                    }
                    else
                        Error = "Правило с таким номером не найдено!";
                }
                else
                    Error = "На сервере еще нет правил!";


                if (Error.Length > 0)
                {
                    var Prefix = await _db.Guilds.GetPrefix(Context.Guild.Id);
                    emb.WithDescription(Error).WithFooter($"Подробнее - {Prefix}i {(emb.Footer?.Text.Length > 0 ? emb.Footer.Text : "arr")}");
                }

                await Context.Channel.SendMessageAsync("", false, emb.Build());
            }
        }

        private async Task ReportUser(SocketGuildUser User, ReportTypeEnum ReportType, TimeSpan Time)
        {
            using (db _db = new())
            {
                var DateTimes = DateTime.Now + Time;
                var UserDb = await _db.Users_Guild.GetOrCreate(User.Id,User.Guild.Id);
                _db.TempUser.RemoveRange(_db.TempUser.Where(x => x.Users_GuildId == UserDb.Id && x.Users_Guild.GuildsId == Context.Guild.Id));
                switch (ReportType)
                {
                    case ReportTypeEnum.Ban:
                    case ReportTypeEnum.TimeBan:
                        if (ReportType == ReportTypeEnum.TimeBan)
                        {
                            var Warn = _db.TempUser.Add(new TempUser() { Users_GuildId = UserDb.Id, ToTime = DateTimes, Reason = ReportType }).Entity;
                            await TaskTimer.StartBanTimer(Warn);
                        }
                        await User.BanAsync();
                        break;
                    case ReportTypeEnum.Mute:
                        await User.AddMute(new TimeSpan(28, 0, 0, 0));
                        break;
                    case ReportTypeEnum.Kick:
                        await User.KickAsync();
                        break;
                    case ReportTypeEnum.TimeOut:
                        await User.AddMute(DateTimes - DateTime.Now);
                        break;
                }
                await _db.SaveChangesAsync();
            }
        }

        [Aliases, Commands, Usage, Descriptions]
        [PermissionBlockCommand, PermissionViolation]
        [RequireUserPermission(GuildPermission.KickMembers)]
        [RequireUserPermission(GuildPermission.MuteMembers)]
        [RequireUserPermission(GuildPermission.BanMembers)]
        [RequireBotPermission(GuildPermission.BanMembers)]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        [RequireBotPermission(ChannelPermission.EmbedLinks)]
        public async Task unreport(SocketGuildUser User, ushort NumberRules)
        {
            using (db _db = new())
            {
                if (NumberRules == 0)
                    NumberRules = 1;
                var emb = new EmbedBuilder().WithColor(255, 0, 94).WithAuthor("Снятие нарушения");
                string Error = string.Empty;
                var Report = _db.Reports.Where(x => x.GuildsId == User.Guild.Id).Include(x => x.ReportsList);
                if (Report.Any())
                {
                    var ThisReport = Report.AsEnumerable().ElementAtOrDefault(NumberRules - 1);
                    if (ThisReport != null)
                    {
                        if (ThisReport.ReportsList.Count != 0) 
                        {
                            var UserDb = await _db.Users_Guild.GetOrCreate(User.Id, User.Guild.Id);
                            var UserGuild = _db.Users_Guild.Include(x => x.Reports_Punishes).FirstOrDefault(x => x.UsersId == User.Id && x.GuildsId == User.Guild.Id);

                            var ThisPunishes = UserGuild.Reports_Punishes.FirstOrDefault(x => x.ReportId == ThisReport.Id);

                            if(ThisPunishes != null)
                            {
                                ThisPunishes.Users_Guild.Remove(UserGuild); // Удаляем пользователя из прошлого нарушения
                                _db.Reports_Punishes.Update(ThisPunishes); // Обновляем

                                int IndexBeforePunishes = ThisReport.ReportsList.ToList().IndexOf(ThisPunishes); // узнаем индекс прошлого нарушения
                                if(IndexBeforePunishes != 0)
                                {
                                    var NextPunishes = ThisReport.ReportsList.ElementAtOrDefault(IndexBeforePunishes - 1); // Достаем новое нарушение
                                    NextPunishes = _db.Reports_Punishes.Include(x=>x.Users_Guild).FirstOrDefault(x=>x.Id == NextPunishes.Id);
                                    NextPunishes.Users_Guild.Add(UserGuild);
                                    _db.Reports_Punishes.Update(NextPunishes);
                                }
                                

                                var ActiveWarn = _db.TempUser.Where(x => x.Users_GuildId == UserDb.Id && x.Users_Guild.GuildsId == User.Guild.Id);
                                if(ActiveWarn != null)
                                    _db.TempUser.RemoveRange(ActiveWarn);

                                await _db.SaveChangesAsync();

                                switch (ThisPunishes.TypeReport)
                                {
                                    case ReportTypeEnum.TimeBan:
                                    case ReportTypeEnum.Ban:
                                        await Context.Guild.RemoveBan(User.Id);
                                        break;
                                    case ReportTypeEnum.TimeOut:
                                    case ReportTypeEnum.Mute:
                                        await User.RemoveTimeOutAsync();
                                        break;
                                }
                                emb.WithDescription($"У пользователя {User.Mention} снято {IndexBeforePunishes + 1} нарушение.");
                            }
                            else
                                emb.WithDescription($"У пользователя {User.Mention} нету нарушений.");
                        }
                        else
                        {
                            Error = "На сервере еще нет нарушений для правил!";
                            emb.WithFooter($"arp");
                        }
                    }
                    else
                        Error = "Правило с таким номером не найдено!";
                }
                else
                    Error = "На сервере еще нет правил!";


                if (Error.Length > 0)
                {
                    var Prefix = await _db.Guilds.GetPrefix(Context.Guild.Id);
                    emb.WithDescription(Error).WithFooter($"Подробнее - {Prefix}i {(emb.Footer?.Text.Length > 0 ? emb.Footer.Text : "arr")}");
                }
                //var usr = await _db.Users_Guild.GetOrCreate(User.Id, User.Guild.Id);

                //var ThisRules = usr.ListReports.FirstOrDefault(x => x.RulesNumber == NumberRules - 1); 
                //if (ThisRules != null && ThisRules.ReportCount > 0)
                //{
                //    emb.WithDescription($"У пользователя {User.Mention} снято {ThisRules.ReportCount} нарушение.");

                //    var Report = _db.Reports.Where(x => x.GuildId == User.Guild.Id).AsEnumerable().ElementAtOrDefault(NumberRules - 1);
                //    if (Report != null)
                //    {
                //        var ActiveWarn = _db.TempUser.Include(x => x.Users_Guild).FirstOrDefault(x => x.Users_GuildId == User.Id && x.Users_Guild.GuildsId == User.Guild.Id);
                //        _db.TempUser.Remove(ActiveWarn);


                //        var ThisReport = Report.ReportsList.ElementAtOrDefault((int)ThisRules.ReportCount);
                //        switch (ThisReport.TypeReport)
                //        {
                //            case ReportTypeEnum.TimeBan:
                //            case ReportTypeEnum.Ban:
                //                await Context.Guild.RemoveBan(User.Id);
                //                break;
                //            case ReportTypeEnum.TimeOut:
                //            case ReportTypeEnum.Mute:
                //                await User.RemoveTimeOutAsync();
                //                break;
                //        }
                //    }

                //    ThisRules.ReportCount--;
                //    _db.Users_Guild.Update(usr);
                //    await _db.SaveChangesAsync();

                //}
                //else
                //    emb.WithDescription($"У пользователя {User.Mention} нету нарушений.");


                await Context.Channel.SendMessageAsync("", false, emb.Build());
            }
        }


        [Aliases, Commands, Usage, Descriptions]
        [PermissionBlockCommand, PermissionViolation]
        [RequireUserPermission(GuildPermission.KickMembers)]
        [RequireUserPermission(GuildPermission.MuteMembers)]
        [RequireUserPermission(GuildPermission.BanMembers)]
        [RequireBotPermission(GuildPermission.BanMembers)]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        [RequireBotPermission(ChannelPermission.EmbedLinks)]
        public async Task unwarn(SocketGuildUser User)
        {
            using (db _db = new())
            {
                var emb = new EmbedBuilder().WithColor(255, 0, 94).WithAuthor("UnWarn");
                var usr = await _db.Users_Guild.GetOrCreate(User.Id, User.Guild.Id);
                if (usr.CountWarns > 0)
                {
                    emb.WithDescription($"У пользователя {User.Mention} снято {usr.CountWarns} нарушение.");

                    var warn = _db.Guilds_Warns.FirstOrDefault(x => x.GuildsId == User.Guild.Id && x.CountWarn == usr.CountWarns);
                    if (warn != null)
                    {
                        var ActiveWarn = _db.TempUser.Include(x => x.Users_Guild).FirstOrDefault(x => x.Users_GuildId == usr.Id && x.Users_Guild.GuildsId == User.Guild.Id);
                        _db.TempUser.Remove(ActiveWarn);
                        switch (warn.ReportTypes)
                        {
                            case ReportTypeEnum.TimeBan:
                            case ReportTypeEnum.Ban:
                                await Context.Guild.RemoveBan(User.Id);
                                break;
                            case ReportTypeEnum.TimeOut:
                            case ReportTypeEnum.Mute:
                                await User.RemoveTimeOutAsync();
                                break;
                        }
                    }

                    usr.CountWarns--;
                    _db.Users_Guild.Update(usr);
                }
                else
                    emb.WithDescription($"У пользователя {User.Mention} нету нарушений.");
                await _db.SaveChangesAsync();
                await Context.Channel.SendMessageAsync("", false, emb.Build());
            }
        }


        [Aliases, Commands, Usage, Descriptions]
        [PermissionBlockCommand, PermissionHierarchy]
        [RequireUserPermission(GuildPermission.KickMembers)]
        [RequireBotPermission(GuildPermission.KickMembers)]
        [RequireBotPermission(ChannelPermission.EmbedLinks)]
        public async Task kick(SocketGuildUser User, [Remainder] string Reason = null)
        {
            var Embed = new EmbedBuilder().WithColor(255, 0, 94);
            var LockUser = await User.AddKick(Reason);
            if (LockUser != null)
                Embed.WithAuthor("Ошибка!", Context.Guild.IconUrl).WithDescription(LockUser);
            else
            {
                Embed.WithAuthor("Вас кикнули", Context.Guild.IconUrl)
                     .WithDescription($"Вы были кикнуты с сервера {Context.Guild.Name}{(Reason != null ? $"\nПричина: {Reason}" : "")}");
                var UserLockMes = await User.UserMessage("", Embed);

                Embed.WithAuthor($"{User} Kicked", User.GetAvatarUrl())
                .WithDescription($"Пользователь {User.Mention} был кикнут {(Reason != null ? $"\nПричина: {Reason}" : "")}");
                Embed.Description += $"\n{UserLockMes}";
            }
            await Context.Channel.SendMessageAsync("", false, Embed.Build());
        }


        [Aliases, Commands, Usage, Descriptions]
        [PermissionBlockCommand, PermissionHierarchy]
        [RequireUserPermission(GuildPermission.MuteMembers)]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        [RequireBotPermission(ChannelPermission.EmbedLinks)]
        public async Task mute(SocketGuildUser User)
        {
            using (db _db = new())
            {
                var Mute = User.GetValidateMute();
                var emb = new EmbedBuilder().WithColor(255, 0, 94).WithAuthor($"{User} Muted", User.GetAvatarUrl());
                if (Mute.ToTime > DateTime.Now && User.TimedOutUntil == null)
                {
                    _db.TempUser.Remove(Mute);
                    await _db.SaveChangesAsync();
                }

                if (Mute != null)
                {
                    if (Mute.Reason == ReportTypeEnum.TimeOut)
                        emb.WithDescription($"Пользователь {User.Mention} уже имеет тайм-аут");
                    else if (Mute.Reason == ReportTypeEnum.Mute)
                        emb.WithDescription($"Пользователь {User.Mention} уже имеет бесконечный мут");

                    var Prefix = await _db.Guilds.GetPrefix(User.Guild.Id);
                    emb.WithFooter($"Снять мут - {Prefix}unmute {User}");
                }
                else
                {
                    await User.AddMute(new TimeSpan(28, 0, 0, 0));
                    var UserDb = await _db.Users_Guild.GetOrCreate(User.Id, User.Guild.Id);
                    _db.TempUser.Add(new TempUser() { Reason = ReportTypeEnum.Mute, Users_GuildId = UserDb.Id, ToTime = DateTime.Now.AddDays(28) });
                    await _db.SaveChangesAsync();
                    emb.WithDescription($"Пользователь {User.Mention} получил мут");
                    var embb = emb.WithDescription($"Вы были замучены на сервере {Context.Guild.Name}");
                    await User.UserMessage("", embb);
                }
                await Context.Channel.SendMessageAsync("", false, emb.Build());
            }
        }

        [Aliases, Commands, Usage, Descriptions, PermissionBlockCommand]
        [RequireUserPermission(GuildPermission.MuteMembers)]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        [RequireBotPermission(ChannelPermission.EmbedLinks)]
        public async Task unmute(SocketGuildUser User)
        {
            using (db _db = new())
            {
                var emb = new EmbedBuilder().WithColor(255, 0, 94).WithAuthor("Мут");
                if(User.TimedOutUntil == null)
                    emb.WithDescription($"Пользователь {User.Mention} не имеет мутов").WithAuthor($"{User} UnMuted?");
                else
                {
                    var mute = User.GetValidateMute();
                    if (mute != null)
                    {
                        _db.TempUser.Remove(mute);
                        await _db.SaveChangesAsync();
                    }
                    emb.WithDescription($"Пользователь {User.Mention} получил размут").WithAuthor($"{User} UnMuted");

                    await User.RemoveTimeOutAsync();
                    var embb = new EmbedBuilder().WithDescription($"Вы были размучены на сервере {Context.Guild.Name}").WithAuthor($"UnMuted");
                    await User.UserMessage("", embb);
                }
                await Context.Channel.SendMessageAsync("", false, emb.Build());
            }
        }

        [Aliases, Commands, Usage, Descriptions]
        [PermissionBlockCommand, PermissionHierarchy]
        [RequireUserPermission(GuildPermission.MuteMembers)]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        [RequireBotPermission(ChannelPermission.EmbedLinks)]
        public async Task timeout(SocketGuildUser User, string Time)
        {
            using (db _db = new())
            {
                var emb = new EmbedBuilder().WithColor(255, 0, 94).WithAuthor("Временный тайм-аут");
                bool Success = TimeSpan.TryParse(Time, out TimeSpan result);
                if (!Success)
                    emb.WithDescription("Время мута указано неправильно, возможно введено большое число!\nПример: 01:00:00 [ч:м:с]\nПример 2: 10:00:00:00 [д:ч:м:с]");
                else if (result.TotalSeconds < 10)
                    emb.WithDescription("Время мута не должно быть меньше 10 секунд!");
                else if (result.TotalSeconds >= 2419200)
                    emb.WithDescription("Время мута не должно превышать 28 дней!");
                else
                {
                    await User.AddMute(result);
                    emb.WithDescription($"Пользователь {User.Mention} получил мут до {User.TimedOutUntil.Value.UtcDateTime}");
                    var embb = emb.WithDescription($"Вы были замучены на сервере {Context.Guild.Name}");
                    await User.UserMessage("", embb);
                }
                await Context.Channel.SendMessageAsync("", false, emb.Build());
            }
        }

        [Aliases, Commands, Usage, Descriptions, PermissionBlockCommand]
        [RequireUserPermission(ChannelPermission.ManageMessages)]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        [RequireBotPermission(ChannelPermission.EmbedLinks)]
        public async Task clear(uint CountMessage)
        {
            var emb = new EmbedBuilder().WithColor(255, 0, 94).WithAuthor("Чистка сообщений");
            if (CountMessage > 100)
                emb.WithFooter("Удалить больше 100 сообщений нельзя!");
            var messages = await Context.Message.Channel.GetMessagesAsync((int)CountMessage + 1).FlattenAsync();
            await ((SocketTextChannel)Context.Channel).DeleteMessagesAsync(messages);
            emb.WithDescription($"Удалено {messages.Count()} сообщений");
            var x = await Context.Channel.SendMessageAsync("", false, emb.Build());
            await Task.Delay(5000);
            await x.DeleteAsync();
        }

        [Aliases, Commands, Usage, Descriptions, PermissionBlockCommand]
        [RequireUserPermission(ChannelPermission.ManageMessages)]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        [RequireBotPermission(ChannelPermission.EmbedLinks)]
        public async Task userclear(SocketGuildUser User, uint CountMessage)
        {
            var messages = await Context.Message.Channel.GetMessagesAsync((int)CountMessage).FlattenAsync();
            var result = messages.Where(x => x.Author.Id == User.Id);
            var emb = new EmbedBuilder().WithColor(255, 0, 94).WithAuthor($"Чиста сообщений {User}")
                                        .WithDescription($"Удалено {result.Count()} сообщений от {User.Mention}");
            if (CountMessage > 100)
                emb.WithFooter("Удалить больше 100 сообщений нельзя!");
            else
                emb.WithFooter("Сообщения которым больше 14 дней не удаляются!");

            if (User == Context.User)
                await Context.Message.DeleteAsync();

            await ((SocketTextChannel)Context.Message.Channel).DeleteMessagesAsync(result);
            var x = await Context.Channel.SendMessageAsync("", false, emb.Build());
            await Task.Delay(5000);
            await x.DeleteAsync();
        }
        [Aliases, Commands, Usage, Descriptions, PermissionBlockCommand]
        [RequireUserPermission(ChannelPermission.ManageChannels)]
        [RequireBotPermission(ChannelPermission.EmbedLinks)]
        public async Task embedsay(SocketTextChannel TextChannel, [Remainder] string JsonText)
        {
            var mes = JsonToEmbed.JsonCheck(JsonText);
            if (mes.Item1 == null)
            {
                mes.Item2 = null;
                mes.Item1.Color = new Discord.Color(255, 0, 94);
                mes.Item1.WithAuthor("Ошибка!");
                mes.Item1.Description = "Неправильная конвертация в Json.\nПрочтите инструкцию! - [Инструкция](https://docs.darlingbot.ru/commands/komandy-adminov/embedsay)";
            }
            await TextChannel.SendMessageAsync(mes.Item2, false, mes.Item1.Build());
        }

        [Aliases, Commands, Usage, Descriptions, PermissionBlockCommand]
        [RequireUserPermission(ChannelPermission.ManageChannels)]
        public async Task say(SocketTextChannel TextChannel, [Remainder] string Text)
        {
            await TextChannel.SendMessageAsync(Text);
        }


        //[Command("spawner")]
        //public async Task spawner()
        //{
        //    //var Region = await Context.Client.GetOptimalVoiceRegionAsync();
        //    //var Guild = await Context.Client.CreateGuildAsync("DarlingServer", Region);
        //    //var User = Context.Guild.GetUser(BotSettings.hikaruid);
        //    //var Channels = Context.Client.GetGuild(Guild.Id).DefaultChannel;
        //    //var Invite = await Channels.CreateInviteAsync();
        //    //await User.SendMessageAsync("https://discord.gg/" + Invite.Code);
        //    //var AdminRole = await Guild.CreateRoleAsync("Admin", new GuildPermissions(administrator: true), null,false,null);
        //    //await User.AddRole(AdminRole.Id);
        //    var menuBuilder = new SelectMenuBuilder()
        //        .WithPlaceholder("Нажмите для получения капчи!")
        //        .WithCustomId("GetCaptcha")
        //        .WithMinValues(1)
        //        .WithMaxValues(1)
        //        .AddOption("Получить капчу!", "GetCaptcha1")
        //        .AddOption("Получить капчу!", "GetCaptcha2")
        //        .AddOption("Получить капчу!", "GetCaptcha3");

        //    var builder = new ComponentBuilder().WithSelectMenu(menuBuilder);

        //    await ReplyAsync("Нажмите для получения капчи!", components: builder.Build());
        //}
    }
}
