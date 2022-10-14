using DarlingDb;
using DarlingDb.Models;
using DarlingNet.Services;
using DarlingNet.Services.LocalService;
using DarlingNet.Services.LocalService.Attribute;
using DarlingNet.Services.LocalService.GetOrCreate;
using DarlingNet.Services.LocalService.VerifiedAction;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using EFCoreSecondLevelCacheInterceptor;
using Microsoft.EntityFrameworkCore;
using Pcg;
using System;
using System.Linq;
using System.Threading.Tasks;
using static DarlingDb.Enums;
using static DarlingNet.Services.CommandHandler;
using static DarlingNet.Services.LocalService.Attribute.CommandLocksAttribute;

namespace DarlingNet.Modules
{
    [Summary("Пользовательские\nкоманды")]
    [RequireBotPermission(ChannelPermission.SendMessages)]
    [RequireBotPermission(ChannelPermission.EmbedLinks)]
    public class User : ModuleBase<ShardedCommandContext>
    {

        [Aliases, Commands, Usage, Descriptions, PermissionBlockCommand]
        public async Task level(SocketGuildUser User = null)
        {
            using (db _db = new())
            {
                if (User == null)
                    User = Context.User as SocketGuildUser;
                var usr = await _db.Users_Guild.GetOrCreate(User.Id, User.Guild.Id);
                uint count = Convert.ToUInt32(usr.Level * 80 * usr.Level);
                uint countNext = Convert.ToUInt32((usr.Level + 1) * 80 * (usr.Level + 1));
                var emb = new EmbedBuilder().WithColor(255, 0, 94)
                                            .WithAuthor($" - Уровень {User}", User.GetAvatarUrl())
                                            .WithDescription($"Уровень: {usr.Level}\nОпыт:{usr.XP - count}/{countNext - count}");

                await Context.Channel.SendMessageAsync("", false, emb.Build());
            }
        }

        [Aliases, Commands, Usage, Descriptions, PermissionBlockCommand]
        public async Task usertop()
        {
            using (db _db = new())
            {
                var UsersTop = _db.Users_Guild.NotCacheable().Where(x => x.GuildsId == Context.Guild.Id && !x.Leaved).OrderByDescending(x => (double)x.XP);
                var emb = new EmbedBuilder().WithColor(255, 0, 94).WithAuthor(" - TOP 10 ПОЛЬЗОВАТЕЛЕЙ", Context.Guild.IconUrl);
                int count = 0;
                foreach (var usr in UsersTop)
                {
                    var DiscordUser = Context.Guild.GetUser(usr.UsersId);
                    if (DiscordUser != null)
                    {
                        count++;
                        Emote emj = null;
                        if (count == 1)
                            emj = Emote.Parse("<a:1st_place:949464689929236510>");
                        else if (count == 2)
                            emj = Emote.Parse("<a:2st_place:949464689937641502>");
                        else if (Context.Guild.Owner == DiscordUser)
                            emj = Emote.Parse("👑");
                        emb.AddField($"{(emj == null ? "" : $"{emj} ")}{DiscordUser} - {(DateTime.Now - DiscordUser.JoinedAt).Value.Days} дней", $"LVL: {usr.Level} Money: {usr.ZeroCoin}");
                    }
                    if (count == 10)
                        break;
                }
                await Context.Channel.SendMessageAsync("", false, emb.Build());
            }
        }

        [Aliases, Commands, Usage, Descriptions, PermissionBlockCommand]
        public async Task zcoin(SocketGuildUser User = null)
        {
            using (db _db = new())
            {
                if (User == null)
                    User = Context.User as SocketGuildUser;
                var UserZeroCoins = await _db.Users_Guild.GetOrCreate(User.Id, User.Guild.Id);
                var emb = new EmbedBuilder().WithColor(255, 0, 94)
                                            .WithAuthor($" - Кошелек {User}", User.GetAvatarUrl())
                                            .WithDescription($"zcoin: {UserZeroCoins.ZeroCoin}");
                await Context.Channel.SendMessageAsync("", false, emb.Build());
            }
        }

        [Aliases, Commands, Usage, Descriptions, PermissionBlockCommand]
        public async Task daily()
        {
            using (db _db = new())
            {
                var User = await _db.Users_Guild.GetOrCreate(Context.User.Id, Context.Guild.Id);

                var emb = new EmbedBuilder().WithColor(255, 0, 94).WithAuthor(" - daily 🏧", Context.User.GetAvatarUrl());
                var DateNow = DateTime.Now;
                if (DateNow >= User.Daily)
                {
                    if ((DateNow - User.Daily).TotalSeconds >= 86400 /*Math.Abs(DateNow.Day - User.Daily.Day) >= 1*/)
                        User.Streak = 1;
                    else
                        User.Streak++;

                    User.Daily = DateNow.AddDays(1);

                    uint amt = (uint)(500 + ((500 / 35) * User.Streak));
                    emb.WithDescription($"Получено: {amt} ZeroCoin's!\nStreak: {User.Streak}\nСледующий Daily: {User.Daily:dd.MM HH:mm}");
                    User.ZeroCoin += amt;

                    var UserDiscord = _db.DarlingBoost.FirstOrDefault(x => x.Id == Context.User.Id);
                    if (UserDiscord != null && UserDiscord.Active)
                    {
                        User.XP += 100;
                        emb.Description += "\nDarlingBoost: +100 XP";
                    }

                    int rnd = new PcgRandom(1488).Next(0, 1000);
                    if (rnd <= 100)
                    {
                        int moneyrnd = new PcgRandom(1488).Next(300, 3000);
                        User.ZeroCoin += (uint)moneyrnd;
                        emb.Description += $"\n\nСпециальнный бонус в размере {moneyrnd} coin's";
                    }
                    else if (rnd >= 900)
                    {
                        int moneyrnd = new PcgRandom(1488).Next(0, Convert.ToInt32(User.ZeroCoin * 0.5));
                        User.ZeroCoin -= (uint)moneyrnd;
                        emb.Description += $"\n\nМошенники украли у вас {moneyrnd} coin's";
                    }

                    _db.Users_Guild.Update(User);
                    await _db.SaveChangesAsync();
                }
                else
                {
                    var TimeToDaily = User.Daily - DateNow;
                    if (TimeToDaily.TotalSeconds >= 3600)
                        emb.WithDescription($"Дождитесь {TimeToDaily.Hours} часов и {TimeToDaily.Minutes} минут чтобы получить Daily!");
                    else
                        emb.WithDescription($"Дождитесь {(TimeToDaily.TotalSeconds > 60 ? $"{TimeToDaily.Minutes} минут и " : "")} {TimeToDaily.Seconds} секунд чтобы получить Daily!");
                }

                await Context.Channel.SendMessageAsync("", false, emb.Build());
            }
        }

        [Aliases, Commands, Usage, Descriptions, PermissionBlockCommand]
        public async Task reputation(SocketGuildUser user)
        {
            using (db _db = new())
            {
                var emb = new EmbedBuilder().WithColor(255, 0, 94).WithAuthor(" - Reputation 🏧", Context.User.GetAvatarUrl());
                if (user.Id == Context.User.Id)
                    emb.WithDescription("Повысить репутацию самому себе нельзя.");
                else
                {
                    var User = await _db.Users.GetOrCreate(Context.User.Id, Context.Guild.Id);
                    var DateNow = DateTime.Now;
                    if (DateNow >= User.LastReputation)
                    {
                        if (User.Id == User.LastReputationUserId)
                            emb.WithDescription("Вы не можете выдать репутацию одному и тому же пользователю 2 раза подряд.");
                        else
                        {
                            var UserRep = await _db.Users.GetOrCreate(user.Id);
                            User.LastReputation = DateNow.AddDays(1);
                            UserRep.Reputation += 1;
                            User.LastReputationUserId = user.Id;
                            _db.Users.UpdateRange(new[] { UserRep, User });
                            await _db.SaveChangesAsync();
                            emb.WithDescription($"{Context.User.Mention} повысил репутацию {user.Mention}\nРепутация: {UserRep.Reputation}\nСледующая репутация: {User.LastReputation:dd.MM HH:mm}");
                        }  
                    }
                    else
                    {
                        var TimeToDaily = User.LastReputation - DateNow;
                        if (TimeToDaily.TotalSeconds >= 3600)
                            emb.WithDescription($"Дождитесь {TimeToDaily.Hours} часов и {TimeToDaily.Minutes} минут чтобы получить Reputation!");
                        else
                            emb.WithDescription($"Дождитесь {(TimeToDaily.TotalSeconds > 60 ? $"{TimeToDaily.Minutes} минут и " : "")} {TimeToDaily.Seconds} секунд чтобы получить Reputation!");
                    }
                }
                await Context.Channel.SendMessageAsync("", false, emb.Build());
            }
        }

        [Aliases, Commands, Usage, Descriptions, PermissionBlockCommand]
        public async Task profile(SocketGuildUser User = null)
        {
            using (db _db = new())
            {
                if (User == null)
                    User = Context.User as SocketGuildUser;
                var usr = _db.Users_Guild.Include(x => x.Users.Boost).Include(x => x.Reports_Punishes).ThenInclude(x => x.Report).FirstOrDefault(x => x.UsersId == User.Id && x.GuildsId == Context.Guild.Id);
                var emb = new EmbedBuilder().WithColor(255, 0, 94).WithTitle($"{User}").WithThumbnailUrl(User.GetAvatarUrl());
                var TimeNow = DateTime.Now;


                if (usr.Users.Boost != null && usr.Users.Boost.Active)
                {
                    var Time = TimeNow - usr.Users.Boost.Ends;
                    if (usr.Users.Boost.Ends >= TimeNow)
                        emb.Title += $" {BotSettings.EmoteBoost}";
                    else if (Time.TotalDays < 7)
                        emb.Title += $" {BotSettings.EmoteBoostNo}";
                    else if (Time.TotalDays == 7)
                        emb.Title += $" {BotSettings.EmoteBoostLastDay}";
                    else
                        emb.Title += $" {BotSettings.EmoteBoostNot}";
                } // UserBoost

                var Guild = await _db.Guilds.GetOrCreate(Context.Guild.Id);


                if (usr.UsersMId != null && !Guild.CommandInviseList.Any(x => x == "marry"))
                    emb.Description += $"Женат(а) на <@{usr.UsersMId}>\n";

                string UserRate = string.Empty;
                #region UserRate
                var UserTop = _db.Users_Guild.NotCacheable().Where(x => x.GuildsId == Context.Guild.Id).AsEnumerable().OrderByDescending(x => (double)x.XP).ToList();
                var ThisUserCount = UserTop.FindIndex(x => x.UsersId == User.Id);
                await Context.Guild.DownloadUsersAsync();
                UserRate = $"Рейтинг: {ThisUserCount + 1}/{Context.Guild.Users.Count}";
                #endregion
                emb.Description += UserRate + "\n";

                string DailyRep = "Репутация - доступна сейчас!";
                var TimeToRep = usr.Users.LastReputation - DateTime.Now;
                if (TimeToRep.TotalSeconds > 0)
                    DailyRep = $"До репутации - {TimeToRep.Hours}:{TimeToRep.Minutes}:{TimeToRep.Seconds}";

                emb.Description += $"Кол-во репутации: {usr.Users.Reputation}\n" +
                                   $"{DailyRep}" +
                                   $"ZeroCoin's: {usr.ZeroCoin}\n" +
                                   $"Daily Streak: {usr.Streak}\n" +
                                   $"Level: {usr.Level}\n";

                if (Guild.VoiceAndCategoryChannelList.Count > 0)
                    emb.Description += $"Времени в голосовых чатах: {usr.VoiceActive}\n\n";
                else
                    emb.Description += "\n";

                emb.Description += $"Дата регистрации: {User.CreatedAt:HH:mm dd.MM.yy}\n" +
                                   $"Дата входа: {User.JoinedAt.Value:HH:mm dd.MM.yy}\n";

                string DateBirthDay = string.Empty;
                #region BirthDay
                if (usr.Users.BirthDate.Year == 1)
                    DateBirthDay = $"Дата рождения: {Guild.Prefix}birthdate [date]\n";
                else
                {
                    if (usr.BirthDateInvise)
                        DateBirthDay += $"Включить дату: {Guild.Prefix}birthdate [date] [включить]\n\n";
                    else
                        DateBirthDay = $"Дата рождения: {usr.Users.BirthDate:dd.MM.yy} [Скрыть - {Guild.Prefix}birthdate [date] `отключить`]\n\n";
                }
                #endregion
                emb.Description += DateBirthDay;

                string DateOnline = string.Empty;
                #region Время в сети

                if (usr.Id == Context.User.Id)
                {
                    usr.Users.LastOnline = TimeNow;
                    _db.Users_Guild.Update(usr);
                    await _db.SaveChangesAsync();
                    DateOnline = "В сети: Прямо сейчас!";
                }
                else if (usr.Users.LastOnline.Year == 1)
                    DateOnline = "Был в сети: Неизвестно.";
                else
                {
                    var time = (TimeNow - usr.Users.LastOnline);
                    if (time.TotalMinutes < 60)
                    {
                        if (time.TotalSeconds <= 60)
                            DateOnline = $"{(int)time.TotalSeconds} секунд";
                        else
                            DateOnline = $"{(int)time.TotalMinutes} минут";

                        DateOnline = $"Был в сети {DateOnline} назад.";
                    }
                    else
                    {
                        if (time.TotalHours <= 24)
                            DateOnline = $"Был в сети в {usr.Users.LastOnline:HH:mm}.";
                        else if (time.TotalDays <= 365)
                            DateOnline = $"Был в сети {usr.Users.LastOnline:dd.MM HH:mm}";
                        else
                            DateOnline = $"Был в сети {usr.Users.LastOnline:dd.MM.yyyy HH:mm}";
                    }
                }
                #endregion
                emb.Description += DateOnline + "\n";

                string SystemReport = string.Empty;
                #region SystemReport
                if (Guild.VS == ViolationSystemEnum.WarnSystem)
                {
                    var WarnsCount = _db.Guilds_Warns.Count(x => x.GuildsId == Context.Guild.Id);
                    SystemReport = $"\nWarns: {usr.CountWarns}/{WarnsCount}\n";
                }
                else if (Guild.VS == ViolationSystemEnum.ReportSystem)
                {
                    bool ReportAny = usr.Reports_Punishes.Any();
                    SystemReport += "\n\n**Reports:** ";

                    if (ReportAny)
                        SystemReport += "[правило:нарушение]\n";
                    else
                        SystemReport += "нарушений нет.";


                    if (ReportAny)
                    {
                        var Report = _db.Reports.Where(x => x.GuildsId == User.Guild.Id).Include(x => x.ReportsList).ToList();
                        foreach (var Punishes in usr.Reports_Punishes)
                        {
                            int ReportIndex = Report.IndexOf(Punishes.Report);
                            int PunishesIndex = Report.FirstOrDefault(x => x.Id == Punishes.ReportId).ReportsList.OrderBy(x => x.Id).ToList().IndexOf(Punishes);
                            SystemReport += $"⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀{ReportIndex + 1}:{PunishesIndex + 1}\n";
                        }
                    }
                }
                #endregion
                emb.Description += SystemReport;


                string Daily = string.Empty;
                #region Daily
                var TimeToDaily = usr.Daily - TimeNow;
                if (TimeToDaily.Seconds > 0)
                    Daily = $"До Daily - {TimeToDaily.Hours}:{TimeToDaily.Minutes}:{TimeToDaily.Seconds}";
                else
                    Daily = $"До сброса Daily - {24 + TimeToDaily.Hours}:{60 + TimeToDaily.Minutes}:{60 + TimeToDaily.Seconds}";
                #endregion
                emb.WithFooter(Daily);

                await Context.Channel.SendMessageAsync("", false, emb.Build());

            }
        }

        [Aliases, Commands, Usage, Descriptions, PermissionBlockCommand]
        public async Task birthdate(DateTime date, string accept = null)
        {
            using (db _db = new())
            {
                var TimeNow = DateTime.Now;
                if (!string.IsNullOrWhiteSpace(accept))
                    accept = accept.ToLower();
                var emb = new EmbedBuilder().WithColor(255, 0, 94).WithAuthor("День рождения");
                if (date.Year > TimeNow.Year - 8 || date.Year < TimeNow.Year - 50)
                    emb.WithDescription("Ваш возраст не может быть младше 8 лет, и старше 50 лет.");
                else
                {
                    var usr = await _db.Users_Guild.GetOrCreate(Context.User.Id, Context.Guild.Id);
                    var Prefix = await _db.Guilds.GetPrefix(Context.Guild.Id);
                    if (usr.Users.BirthDate.Year == 1)
                    {
                        if (accept == "принимаю")
                        {
                            emb.WithDescription("Дата рождения применена!");
                            if (date.Hour == 0)
                                date.AddHours(12);
                            usr.Users.BirthDate = date;
                            if (TimeNow.Month > date.Month || TimeNow.Month == date.Month && TimeNow.Day > date.Day)
                                usr.Users.BirthDateComplete = (ushort)(TimeNow.Year + 1);
                            else
                                usr.Users.BirthDateComplete = (ushort)TimeNow.Year;

                            _db.Users.Update(usr.Users);
                            await _db.SaveChangesAsync();
                        }
                        else
                            emb.WithDescription($"Написав свою дату, вы подтверждаете, что она является ВАШЕЙ, и больше не сможет быть изменена.\n\nПрименить дату - {Prefix}birthdate [date] `принимаю`");
                    }
                    else
                    {
                        if (usr.BirthDateInvise)
                        {
                            if (accept == "включить")
                            {
                                emb.WithDescription($"Вы включили поздравления и отображение даты рождения\nОтключить {Prefix}birthdate [date] [отключить]");
                                usr.BirthDateInvise = false;
                                _db.Users_Guild.Update(usr);
                                await _db.SaveChangesAsync();
                            }
                            else if (accept == "отключить")
                                emb.WithDescription($"Отображение даты и поздравление и так отключено!");
                            else
                                emb.WithDescription($"Вы можете включить свою дату рождения: {Prefix}birthdate [date] [включить]");
                        }
                        else
                        {
                            if (accept == "отключить")
                            {
                                emb.WithDescription($"Вы отключили поздравления и скрыли свою дату рождения\nВключить {Prefix}birthdate [date] [включить]");
                                usr.BirthDateInvise = true;
                                _db.Users_Guild.Update(usr);
                                await _db.SaveChangesAsync();
                            }
                            else if (accept == "включить")
                                emb.WithDescription($"Отображение даты и поздравление и так включено!");
                            else
                                emb.WithDescription($"Вы можете отключить свою дату рождения: {Prefix}birthdate [date] [отключить]");
                        }
                    }
                }
                await Context.Channel.SendMessageAsync("", false, emb.Build());
            }
        }

        [Aliases, Commands, Usage, Descriptions, PermissionBlockCommand]
        public async Task transfer(SocketGuildUser User, ushort coin)
        {
            var emb = new EmbedBuilder().WithColor(255, 0, 94).WithAuthor($"{Context.User} 💱 {User}");
            using (db _db = new())
            {
                if (User.Id != Context.User.Id)
                {
                    const int maxSum = 25000;
                    if (coin <= maxSum)
                    {
                        var usr = await _db.Users_Guild.GetOrCreate(Context.User.Id, Context.Guild.Id);
                        if (usr.ZeroCoin >= coin)
                        {
                            var transfuser = await _db.Users_Guild.GetOrCreate(User.Id, Context.Guild.Id);
                            usr.ZeroCoin -= coin;
                            transfuser.ZeroCoin += coin;
                            emb.WithDescription($"Перевод в размере {coin} zerocoin успешно прошел.");
                            _db.Users_Guild.Update(usr);
                            _db.Users_Guild.Update(transfuser);
                            await _db.SaveChangesAsync();
                        }
                        else emb.WithDescription($"У вас недостаточно средств для перевода. Вам нехватает {coin - usr.ZeroCoin} zc");
                    }
                    else emb.WithDescription($"Перевести больше {maxSum} zerocoin нельзя.");
                }
                else emb.WithDescription("Переводить деньги самому себе нельзя!");

                await Context.Channel.SendMessageAsync("", false, emb.Build());
            }
        }

        [Aliases, Commands, Usage, Descriptions, PermissionBlockCommand]
        [RequireBotPermission(GuildPermission.ManageGuild)]
        public async Task myinvite()
        {
            var emb = new EmbedBuilder().WithColor(255, 0, 94).WithAuthor($"Ваши инвайты на сервер {Context.Guild.Name}", Context.Guild.IconUrl);

            foreach (var invite in Context.Guild.GetInvitesAsync()?.Result?.Where(x => x.Inviter.Id == Context.User.Id))
            {
                string text = null;
                if (invite.MaxAge > 0)
                {
                    var Time = (invite.CreatedAt.Value.AddSeconds((double)invite.MaxAge) - DateTime.Now);

                    if (Time.TotalSeconds > 3600)
                        text += $"{Time.Hours}h и {Time.Minutes}m!";
                    else if (Time.TotalSeconds <= 3600)
                        text += $"{(Time.TotalSeconds > 60 ? $"{Time.Minutes}m и" : "")} {Time.Seconds}s!";
                }
                emb.AddField($"ID: {invite.Id}",
                             $"Использований: {invite.Uses}/{(invite.MaxUses == 0 ? "∞" : invite.MaxUses.ToString())}\n" +
                             $"{(invite.MaxAge != 0 ? $"Осталось: {text}" : "")}",
                             true);

            }
            if (emb.Fields.Count == 0)
                emb.WithDescription("Инвайты отсутствуют.");
            await Context.Channel.SendMessageAsync("", false, emb.Build());
        }

        [Aliases, Commands, Usage, Descriptions]
        [PermissionBlockCommand, PermissionViolation]
        public async Task warns()
        {
            var emb = new EmbedBuilder().WithColor(255, 0, 94).WithAuthor("⚜️ WarnSystem - Варны сервера");
            using (db _db = new())
            {
                var Warns = _db.Guilds_Warns.Where(x => x.GuildsId == Context.Guild.Id).OrderBy(x => x.CountWarn);
                foreach (var warn in Warns)
                {
                    string text = string.Empty;
                    switch (warn.ReportTypes)
                    {
                        case ReportTypeEnum.TimeBan:
                            text = $"Бан на {warn.Time}";
                            break;
                        case ReportTypeEnum.Mute:
                            text = $"Мут";
                            break;
                        case ReportTypeEnum.TimeOut:
                            text = $"Мут на {warn.Time}";
                            break;
                        case ReportTypeEnum.Kick:
                            text = $"Кик";
                            break;
                        case ReportTypeEnum.Ban:
                            text = $"Бан";
                            break;
                    }

                    emb.Description += $"{warn.CountWarn}.{text}\n";
                }

                if (!Warns.Any())
                    emb.WithDescription("На сервере еще нет варнов!");
            }
            await Context.Channel.SendMessageAsync("", false, emb.Build());
        }

        [Aliases, Commands, Usage, Descriptions]
        [PermissionBlockCommand, PermissionViolation]
        public async Task rules()
        {
            var emb = new EmbedBuilder().WithColor(255, 0, 94).WithAuthor("⚜️ RulesSystem - Правила сервера");
            using (db _db = new())
            {
                var Reports = _db.Reports.Include(x => x.ReportsList).Where(x => x.GuildsId == Context.Guild.Id);
                int count = 0;
                foreach (var Report in Reports)
                {
                    count++;

                    if (count > 1)
                        emb.Description += "\n";
                    emb.Description += $"➥ **{count}**.{Report.Rules}\n";

                    int countPunishes = 0;
                    string text = string.Empty;
                    foreach (var ReportPunishes in Report.ReportsList)
                    {
                        countPunishes++;
                        if (ReportPunishes.TimeReport.TotalSeconds != 0)
                        {
                            if (ReportPunishes.TypeReport == ReportTypeEnum.TimeBan)
                                text = $"Бан на {ReportPunishes.TimeReport}";
                            else if (ReportPunishes.TypeReport == ReportTypeEnum.TimeOut)
                                text = $"Мут на {ReportPunishes.TimeReport}";
                        }
                        else
                        {
                            switch (ReportPunishes.TypeReport)
                            {
                                case ReportTypeEnum.Mute:
                                    text = "Мут";
                                    break;
                                case ReportTypeEnum.Kick:
                                    text = "Кик";
                                    break;
                                case ReportTypeEnum.Ban:
                                    text = "Бан";
                                    break;
                            }
                        }

                        emb.Description += $"・{countPunishes}){(text?.Length == 0 ? ReportPunishes.TypeReport : text)}\n";
                    }

                    if (Report.ReportsList.Count == 0)
                        emb.Description += "・Администратор еще не добавил нарушений для правила! **БУНТ!!!**";
                }
                if (!Reports.Any())
                    emb.WithDescription("На сервере еще нет правил!");
            }
            await Context.Channel.SendMessageAsync("", false, emb.Build());
        }

        [Aliases, Commands, Usage, Descriptions]
        [PermissionBlockCommand]
        public async Task levelrole()
        {
            using (db _db = new())
            {
                var lvl = _db.Roles.Include(x => x.Role).Where(x => x.Role.GuildsId == Context.Guild.Id && x.Type == RoleTypeEnum.Level).AsEnumerable().OrderBy(u => Convert.ToUInt32(u.Value));
                var embed = new EmbedBuilder().WithAuthor($"🔨 Уровневые роли {(lvl.Any() ? "" : "отсутствуют ⚠️")}")
                                              .WithColor(255, 0, 94);
                foreach (var LVL in lvl)
                    embed.Description += $"{LVL.Value} уровень - <@&{LVL.RoleId}>\n";

                if (Context.Guild.Owner == Context.User)
                {
                    var Prefix = await _db.Guilds.GetPrefix(Context.Guild.Id);
                    embed.AddField("Добавить", $"{Prefix}lr.Add [ROLE] [LEVEL]");
                    embed.AddField("Удалить", $"{Prefix}lr.Del [ROLE]");
                }
                await Context.Channel.SendMessageAsync("", false, embed.Build());
            }
        }


        [Aliases, Commands, Usage, Descriptions]
        [PermissionBlockCommand]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        public async Task buyrole() => await RoleTimer(true);

        [Aliases, Commands, Usage, Descriptions]
        [PermissionBlockCommand]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        public async Task timerole() => await RoleTimer(false);

        private async Task RoleTimer(bool BuyOrTime)
        {
            using (db _db = new())
            {
                var embed = new EmbedBuilder().WithColor(255, 0, 94);
                var user = await _db.Users_Guild.GetOrCreate(Context.User.Id, Context.Guild.Id);
                var DBroles = _db.Roles.Include(x => x.Role).Where(x => x.Role.GuildsId == Context.Guild.Id && x.Type == (BuyOrTime ? RoleTypeEnum.Buy : RoleTypeEnum.Time)).AsEnumerable();
                if (DBroles.Any())
                {
                    embed.WithAuthor($"🔨 {(BuyOrTime ? "Покупка" : "Аренда")} ролей");
                    DBroles = DBroles.OrderBy(u => u.Value.Split(':')[0]);
                    int CountSlot = 3;
                    var Id = await ListBuilder.ListButtonSliderBuilder(DBroles, embed, BuyOrTime ? "buyrole" : "timerole", Context, CountSlot, true);
                    var Data = ListData.FirstOrDefault(x => x.MessageId == Id.Item1 && x.Type == ButtonActionEnum.Number_Wait ||
                                                                                 x.Type == ButtonActionEnum.Number_1 ||
                                                                                 x.Type == ButtonActionEnum.Number_2 ||
                                                                                 x.Type == ButtonActionEnum.Number_3 ||
                                                                                 x.Type == ButtonActionEnum.Number_4 ||
                                                                                 x.Type == ButtonActionEnum.Number_5);


                    embed.WithFooter("");
                    if (Data.Type != ButtonActionEnum.Number_Wait)
                    {
                        var Number = (Convert.ToInt32(Data.Type.ToString().Split('_')[1]) - 1);
                        var DBrole = DBroles.ToList()[(Id.Item2 - 1) * CountSlot + Number];
                        var Role = Context.Guild.GetRole(DBrole.RoleId);

                        if (user.ZeroCoin >= Convert.ToUInt64(DBrole.Value.Split(':')[0]))
                        {
                            if (!(Context.User as SocketGuildUser).Roles.Contains(Role))
                            {
                                var role = await Context.User.AddRole(Role.Id);
                                if (!string.IsNullOrWhiteSpace(role))
                                    embed.WithDescription(role);
                                else
                                {
                                    user.ZeroCoin -= Convert.ToUInt32(DBrole.Value.Split(':')[0]);
                                    _db.Users_Guild.Update(user);
                                    await _db.SaveChangesAsync();

                                    if (!BuyOrTime)
                                    {
                                        await _db.Role.GetOrCreate(Role);

                                        var NewTime = new Roles_Timer { RoleId = Role.Id, ToTime = DateTime.Now.AddMinutes(Convert.ToUInt64(DBrole.Value.Split(':')[1])), Users_GuildId = user.Id };
                                        _db.Roles_Timer.Update(NewTime);
                                        await _db.SaveChangesAsync();
                                        await TaskTimer.StartTimeRole(NewTime);
                                    }

                                    embed.WithDescription($"Вы успешно {(BuyOrTime ? "купили" : "арендовали")} {Role.Mention} за {DBrole.Value.Split(':')[0]} ZeroCoins {(BuyOrTime ? "" : $"на {DBrole.Value.Split(':')[1]} минут!")}");

                                }
                            }
                            else embed.WithDescription($"Вы уже {(BuyOrTime ? "купили" : "арендовали")} роль {Role.Mention}");
                        }
                        else embed.WithDescription($"У вас недостаточно средств на счете!\nВаш баланс: {user.ZeroCoin} ZeroCoins");

                        await Context.Channel.SendMessageAsync("", false, embed.Build());
                    }
                    ListData.Remove(Data);
                }
                else
                {
                    if (Context.Guild.Owner == Context.User)
                    {
                        var Prefix = await _db.Guilds.GetPrefix(Context.Guild.Id);
                        embed.AddField("Добавить роль", $"{Prefix}{(BuyOrTime ? "br" : "tr")}.Add [ROLE] [PRICE] {(BuyOrTime ? "" : "[Time minute]")}", true);
                        embed.AddField("Удалить роль", $"{Prefix}{(BuyOrTime ? "br" : "tr")}.Del [ROLE]", true);
                    }
                    else embed.WithDescription($"Попросите создателя сервера выставить роли на {(BuyOrTime ? "продажу" : "аренду")} <3");

                    await Context.Channel.SendMessageAsync("", false, embed.WithAuthor($"🔨{(BuyOrTime ? "Buy" : "Time")}Role - Роли не выставлены на {(BuyOrTime ? "продажу" : "аренду")} ⚠️").Build());
                }
            }
        }


        [Aliases, Commands, Usage, Descriptions, PermissionBlockCommand]
        public async Task marry(SocketGuildUser user)
        {
            using (db _db = new())
            {
                var emb = new EmbedBuilder().WithColor(255, 0, 94).WithAuthor($"💞 Женидьба - Ошибка");
                bool checks = false;
                if (Context.User.Id != user.Id)
                {
                    var ContextUser = await _db.Users_Guild.GetOrCreate(Context.User.Id, Context.Guild.Id);
                    var marryuser = await _db.Users_Guild.GetOrCreate(user.Id, Context.Guild.Id);
                    if (ContextUser.UsersMId != marryuser.Id)
                    {
                        var Prefix = await _db.Guilds.GetPrefix(Context.Guild.Id);
                        if (ContextUser.UsersMId == null)
                        {
                            if (marryuser.UsersMId == null)
                            {
                                checks = true;
                                var time = DateTime.Now.AddSeconds(60);
                                emb.WithAuthor($"{Context.User} 💞 {user}").WithDescription($"Заявка на свадьбу, отправлена {user.Mention}").WithFooter("Заявка активна в течении минуты");
                                var builder = new ComponentBuilder().WithButton("Принять", nameof(ButtonActionEnum.Marryed_Yes), ButtonStyle.Success).WithButton("Отклонить", nameof(ButtonActionEnum.Marryed_No), ButtonStyle.Danger);
                                var mes = await Context.Channel.SendMessageAsync("", false, emb.Build(), components: builder.Build());
                                var NewApplication = new ActionData { MessageId = mes.Id, UserId = user.Id, Type = ButtonActionEnum.Marryed_Wait };
                                ListData.Add(NewApplication);

                                while (time > DateTime.Now)
                                {
                                    NewApplication = ListData.FirstOrDefault(x => x.MessageId == mes.Id);
                                    if (NewApplication.Type == ButtonActionEnum.Marryed_Yes)
                                    {
                                        ContextUser.UsersM = marryuser;
                                        _db.Users_Guild.Update(ContextUser);
                                        await _db.SaveChangesAsync();
                                        emb.WithDescription($"{user.Mention} и {Context.User.Mention} поженились!");
                                        break;
                                    }
                                    else if (NewApplication.Type == ButtonActionEnum.Marryed_No)
                                    {
                                        emb.WithDescription($"{user.Mention} отказался(лась) от свадьбы!");
                                        break;
                                    }
                                }

                                if (NewApplication.Type == ButtonActionEnum.Marryed_Wait)
                                    emb.WithDescription($"{user.Mention} не успел(а) принять заявку!");

                                ListData.Remove(NewApplication);
                                emb.Footer.Text = null;
                                await mes.ModifyAsync(x => { x.Components = new ComponentBuilder().Build(); x.Embed = emb.Build(); });
                            }
                            else emb.WithDescription($"{user} женат(а), нужно сначала развестись!").WithFooter($"Развестить - {Prefix}divorce");
                        }
                        else emb.WithDescription("Вы уже женаты, сначала разведитесь!").WithFooter($"Развестить - {Prefix}divorce");
                    }
                    else emb.WithDescription("Вы уже женаты на этом пользователе");
                }
                else emb.WithDescription("Вы не можете жениться на себе!");

                if (!checks)
                    await Context.Channel.SendMessageAsync("", false, emb.Build());
            }
        }

        [Aliases, Commands, Usage, Descriptions, PermissionBlockCommand]
        public async Task divorce()
        {
            var emb = new EmbedBuilder().WithColor(255, 0, 94).WithAuthor(" - Развод", Context.User.GetAvatarUrl());
            using (db _db = new())
            {
                var ContextUser = await _db.Users_Guild.GetOrCreate(Context.User.Id, Context.Guild.Id);
                if (ContextUser.UsersMId == null)
                    emb.WithDescription($"Вы не женаты!");
                else
                {
                    ContextUser.UsersMId = null;
                    _db.Users_Guild.Update(ContextUser);
                    await _db.SaveChangesAsync();
                    emb.WithDescription($"Вы успешно развелись с <@{ContextUser.UsersMId}>!");
                }

                await Context.Channel.SendMessageAsync("", false, emb.Build());
            }
        }



        [Aliases, Commands, Usage, Descriptions, PermissionBlockCommand]
        public async Task kazino(KazinoChipsEnum Fishka, ushort Stavka = 0)
        {
            using (db _db = new())
            {
                var emb = new EmbedBuilder().WithColor(255, 0, 94).WithAuthor(" - Казино", Context.User.GetAvatarUrl());
                var account = await _db.Users_Guild.GetOrCreate(Context.User.Id, Context.Guild.Id);
                if (Fishka == KazinoChipsEnum.allblack || Fishka == KazinoChipsEnum.allred || Fishka == KazinoChipsEnum.allzero)
                {
                    if (account.ZeroCoin >= 30000)
                        Stavka = 30000;
                    else
                        Stavka = (ushort)account.ZeroCoin;
                }
                if (Stavka >= 100 && Stavka <= 30000)
                {
                    if (account.ZeroCoin >= Stavka)
                    {
                        int ches = new PcgRandom().Next(6);
                        emb.WithAuthor(" - Казино - ✔️ Выигрыш", Context.User.GetAvatarUrl());
                        if ((Fishka == KazinoChipsEnum.black || Fishka == KazinoChipsEnum.allblack) && ches % 2 == 1 && ches != 5)
                            account.ZeroCoin += Stavka;
                        else if ((Fishka == KazinoChipsEnum.red || Fishka == KazinoChipsEnum.allred) && ches % 2 == 0)
                            account.ZeroCoin += Stavka;
                        else if (ches == 5 && (Fishka == KazinoChipsEnum.zero || Fishka == KazinoChipsEnum.allzero))
                        {
                            ushort BonusUpper = 5;
                            var Boost = await _db.DarlingBoost.GetOrCreate(account.UsersId);
                            if (Boost.Active)
                                BonusUpper = 10;

                            account.ZeroCoin += (ushort)(Stavka * BonusUpper);
                        }
                        else
                        {
                            account.ZeroCoin -= Stavka;
                            emb.WithAuthor(" - Казино - ❌ Проигрыш", Context.User.GetAvatarUrl());
                        }
                        emb.WithDescription($"Выпало: {(ches != 5 && ches % 2 == 1 ? "black" : (ches % 2 == 0) ? "red" : "zero")}\nZeroCoin: {account.ZeroCoin}");

                        if (emb.Author.Name == " - Казино - ✔️ Выигрыш")
                        {
                            int rnd = new PcgRandom(1488).Next(0, 1000);
                            if (rnd <= 100)
                            {
                                int moneyrnd = new PcgRandom(1488).Next(300, 3000);
                                account.ZeroCoin += (uint)moneyrnd;
                                if (rnd >= 0 && rnd <= 25)
                                    emb.Description += $"\n\nSyst3mm er0r g1ved u {moneyrnd} coin's";
                                else if (rnd > 25 && rnd <= 50)
                                    emb.Description += $"\n\nОш11бка, в2дан7 с2мма {moneyrnd} coin's";
                                else if (rnd > 50 && rnd <= 75)
                                    emb.Description += $"\n\nПолучена сумма {moneyrnd} coin's";
                                else if (rnd > 75 && rnd <= 100)
                                    emb.Description += $"\n\n{moneyrnd} coin's выдано {Context.User.Mention}";
                            }
                        }
                        _db.Users_Guild.Update(account);
                        await _db.SaveChangesAsync();
                    }
                    else
                        emb.WithDescription($"Недостаточно средств для ставки.\nВаш баланс: {account.ZeroCoin} zc");
                }
                else
                    emb.WithDescription($"Ставка может быть только меньше 100 и больше 30000");
                await Context.Channel.SendMessageAsync("", false, emb.Build());
            }
        }
    }
}
