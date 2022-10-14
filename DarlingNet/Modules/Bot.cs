using Discord.Commands;
using System;
using System.Linq;
using System.Threading.Tasks;
using DarlingDb;
using System.Net;
using Newtonsoft.Json;
using DarlingDb.Models;
using DarlingNet.Services.LocalService.Attribute;
using Discord;
using DarlingNet.Services;
using DarlingNet.Services.LocalService.GetOrCreate;
using DarlingNet.Services.LocalService;
using Microsoft.EntityFrameworkCore;
using System.IO;
using static DarlingDb.Enums;

namespace DarlingNet.Modules
{
    [Summary("Команды бота")]
    [RequireBotPermission(ChannelPermission.SendMessages)]
    [RequireBotPermission(ChannelPermission.EmbedLinks)]
    public class Bot : ModuleBase<ShardedCommandContext>
    {
        private CommandInfo _command;
        protected override void BeforeExecute(CommandInfo command)
        {
            _command = command;
        }
        public static async Task RefreshPayments()
        {
            using (db _db = new())
            {
                using (WebClient wc = new())
                {
                    try
                    {
                        QiwiTransactions[] json = JsonConvert.DeserializeObject<QiwiTransactions[]>(wc.DownloadString(BotSettings.PayURL));
                        foreach (QiwiTransactions token in json)
                        {
                            var payments = _db.QiwiTransactions.FirstOrDefault(x => x.discord_id == token.discord_id && x.invoice_date_add == token.invoice_date_add && x.invoice_ammount == token.invoice_ammount);
                            if (payments == null)
                            {
                                _db.QiwiTransactions.Add(token);
                                var user = _db.Users.FirstOrDefault(x => x.Id == token.discord_id);
                                user.RealCoin += (uint)token.invoice_ammount;
                                await _db.SaveChangesAsync();
                            }
                        }
                    }
                    catch { }
                }
            }
        }

        [Aliases, Commands, Usage, Descriptions]
        public async Task boost(string buy = "nbuy")
        {
            using (db _db = new())
            {
                buy = buy.ToLower();
                await RefreshPayments();
                var emb = new EmbedBuilder().WithColor(255,0,94);

                var UserBoost = _db.Users.Include(x=>x.Boost).FirstOrDefault(x=>x.Id == Context.User.Id);
                byte DefaultPrice = 150;
                byte UnBanPrice = 50;
                byte price = DefaultPrice;
                string buynow = $"[пополнить баланс]({String.Format(BotSettings.PayUserURL, Context.User.Id, price)})";
                string buyunban = $"[пополните баланс]({String.Format(BotSettings.PayUserURL, Context.User.Id, UnBanPrice)})";
                if (buy == "buy")
                {
                    //if (UserBoost.Streak > 0)
                    //    price -= ((price / 100) * 2 * UserBoost.Streak) / UserBoost.Streak;
                    if (UserBoost.RealCoin >= price)
                    {
                        emb.WithTitle($"DarlingBoost {BotSettings.EmoteBoost}");

                        var Boost = await _db.DarlingBoost.GetOrCreate(Context.User.Id);
                        if ((DateTime.Now - Boost.Ends).TotalHours <= 168)
                        {
                            emb.WithDescription("Вы продлили DarlingBoost на 1 месяц вперед. Спасибо ❤️");
                            //emb.Description += $"\nЗа продление вы получили скидку {DefaultPrice - price} Рублей.";
                            Boost.Streak++;
                        }
                        else
                        {
                            Boost.Streak = 1;
                            emb.WithDescription("Вы купили DarlingBoost на 1 месяц. Спасибо ❤️");
                        }
                        Boost.Ends = DateTime.Now.AddMonths(1);
                        UserBoost.RealCoin -= price;
                        _db.Users.Update(UserBoost);
                        _db.DarlingBoost.Update(Boost);
                        await _db.SaveChangesAsync();
                    }
                    else
                        emb.WithAuthor($"🔰 DarlingBoost").WithDescription($"На вашем счете недостаточно средств для покупки DarlingBoost\nБаланс: {UserBoost.RealCoin} - {buynow}");
                }
                else if(buy == "unban")
                {
                    if (!string.IsNullOrWhiteSpace(UserBoost.BlockedReason))
                    {
                        if (UserBoost.RealCoin >= UnBanPrice)
                        {
                            UserBoost.BlockedReason = string.Empty;
                            _db.Users.Update(UserBoost);
                            await _db.SaveChangesAsync();
                            emb.WithDescription("Блокировка успешно снята!\nБолее не нарушайте работу боту.");
                        }
                        else
                            emb.WithDescription($"У вас недостаточно средств - {buyunban}");
                    }
                    else
                        emb.WithDescription("У вас нету блокировки бота!");
                }
                else
                {
                    var Prefix = await _db.Guilds.GetPrefix(Context.Guild.Id);
                    emb.WithDescription($"Баланс: {UserBoost.RealCoin} - {buynow}\n");
                    if (UserBoost.Boost == null)
                    {
                        emb.WithTitle($"DarlingBoost {BotSettings.EmoteBoostNot}");
                        emb.Description += $"У вас нету буста! [цена {price} руб.]\n" +
                                $"Ты можешь купить буст: {Prefix}boost buy";
                    }
                    else
                    {
                        var TimeNow = DateTime.Now;
                        if (UserBoost.Boost.Ends >= TimeNow)
                        {
                            emb.Description += $"Буст оплачен до {UserBoost.Boost.Ends:dd.MM.yy HH:mm}\n\n" +
                                               $"Благодаря тебе я до сих пор работаю ❤️\n" +
                                               $"Ты можешь продлить буст: {Prefix}boost buy";
                            emb.WithTitle($"DarlingBoost {BotSettings.EmoteBoost}");
                        }
                        else
                        {
                            var Time = TimeNow - UserBoost.Boost.Ends;
                            if (Time.TotalDays < 7)
                                emb.WithTitle($"DarlingBoost {BotSettings.EmoteBoostNo}");
                            else if (Time.TotalDays == 7)
                                emb.WithTitle($"DarlingBoost {BotSettings.EmoteBoostLastDay}");
                            else
                            {
                                emb.WithTitle($"DarlingBoost {BotSettings.EmoteBoostNot}");
                                UserBoost.Boost.Streak = 0;
                                _db.DarlingBoost.Update(UserBoost.Boost);
                                await _db.SaveChangesAsync();
                            }
                            
                            emb.Description += $"Ваш буст закончился {(Time.TotalMinutes == 0 ? $"только что!" : Time.TotalMinutes <= 60 ? $"{(int)Time.TotalMinutes} минут назад" : Time.TotalHours <= 24 ? $"{(int)Time.TotalDays} часа назад" : $"{(int)Time.TotalDays} дня назад") } !\n" +
                                $"Ты можешь продлить буст: {Prefix}boost buy";
                        }
                    }

                    if(!string.IsNullOrWhiteSpace(UserBoost.BlockedReason))
                    {
                        emb.Description += "У вас имеется **блокировка**\n";
                        if (UserBoost.RealCoin >= 50)
                            emb.Description += $"Для снятия блокировки - {buyunban}\nПосле пополнения, пропишите команду еще раз.";
                        else
                            emb.Description += $"Для снятия блокировки - {Prefix}boost unban";
                    }
                    
                }
                await Context.Channel.SendMessageAsync("",false, emb.Build());
            }

        }

        [Aliases, Commands, Usage, Descriptions]
        public async Task invitebot()
        {
            var application = await Context.Client.GetApplicationInfoAsync();
            var emb = new EmbedBuilder().WithColor(255, 0, 94).WithAuthor($" - Invite {application.Name}", application.IconUrl);
            emb.WithDescription($"Добавить <@{application.Id}> на ваш сервер -> [Клик!](https://discordapp.com/oauth2/authorize?client_id={application.Id}&scope=bot&permissions=8)");
            await Context.Channel.SendMessageAsync("", false, emb.Build());
        }

        [Aliases, Commands, Usage, Descriptions]
        public async Task version()
        {
            var application = await Context.Client.GetApplicationInfoAsync();
            var emb = new EmbedBuilder().WithColor(Color.Gold).WithAuthor($" - Bot {application.Name}", application.IconUrl);
            DateTime creation = File.GetLastWriteTime("DarlingNet.dll");
            emb.WithDescription($"Сборка бота - {creation:ddMMyy.HH:mm}\nVer - {BotSettings.Version}");
            await Context.Channel.SendMessageAsync("", false, emb.Build());
        }

        [Aliases, Commands, Usage, Descriptions]
        public async Task feedback([Remainder] string Message)
        {
            using (db _db = new())
            {
                var emb = new EmbedBuilder().WithColor(255, 0, 94);
                var Count = _db.Feedback.Count() + 1;
                var GuildPrefix = await _db.Guilds.GetOrCreate(Context.Guild.Id);
                foreach (var Attach in Context.Message.Attachments)
                    Message += $"\n{Attach.Url}";
                emb.WithDescription($"Текст:`{Message}`").WithAuthor($"{Context.User} - Спасибо за отправку отчета №{Count}.", Context.User.GetAvatarUrl()).WithFooter($"Посмотреть статус/заявки - {GuildPrefix.Prefix}myfeedback [number/null]");
                var ThisFeedBack = new Feedback {UserId = Context.User.Id,Message = Message,Time = DateTime.Now,Status = StatusTicketEnum.Отправлен };
                _db.Feedback.Add(ThisFeedBack);
                await _db.SaveChangesAsync();
                await Context.Channel.SendMessageAsync("", false, emb.Build());
            }
        }

        [Aliases, Commands, Usage, Descriptions]
        public async Task myfeedback(ulong Number = 0)
        {
            using (db _db = new())
            {
                var emb = new EmbedBuilder().WithColor(255, 0, 94).WithAuthor("Ваши тикеты");
                if(Number == 0)
                {
                    var Prefix = await _db.Guilds.GetOrCreate(Context.Guild.Id);

                    var List = _db.Feedback.Where(x=>x.UserId == Context.User.Id).AsEnumerable();
                    if (List.Any())
                    {
                        await ListBuilder.ListButtonSliderBuilder(List, emb, _command.Name,Context, 3);
                        return;
                    }
                    else
                        emb.WithDescription("У вас еще нету отчетов.").WithFooter($"Создать - {Prefix.Prefix}feedback [text]");
                }
                else
                {
                    var ThisFeedBack = _db.Feedback.FirstOrDefault(x=>x.Id == Number && x.UserId == Context.User.Id);
                    if (ThisFeedBack == null)
                        emb.WithDescription("Данная заявка не найдена!").WithAuthor("Feedback [ошибка]");
                    else
                    {
                        emb.WithDescription($"Ошибка: {ThisFeedBack.Message}").AddField("Статус", ThisFeedBack.Status.ToString(), true)
                                                                             .AddField("Дата", ThisFeedBack.Time.ToString("HH:mm dd.MM.yy"), true)
                                                                             .WithAuthor($"Feedback №{ThisFeedBack.Id}");

                        if (!string.IsNullOrWhiteSpace(ThisFeedBack.AdminMessage))
                            emb.AddField("Комментарий от админа", ThisFeedBack.AdminMessage, true);
                    }
                       
                }
                await Context.Channel.SendMessageAsync("", false, emb.Build());
            }
        }

        [Aliases, Commands, Usage, Descriptions]
        [RequireOwner]
        public async Task blockuser(ulong userid, string Reason = null)
        {
            using (db _db = new ())
            {
                var emb = new EmbedBuilder().WithColor(255, 0, 94).WithAuthor("Доступ пользователей");
                if (Reason == null)
                    emb.WithDescription("Напишите причину бана или разбан пользователя!");
                else
                {
                    var User = _db.Users.FirstOrDefault(x=>x.Id == userid);
                    if (User != null)
                    {
                        if (Reason == "разбан")
                            User.BlockedReason = string.Empty;
                        else
                            User.BlockedReason = Reason;
                        _db.Users.UpdateRange(User);
                        await _db.SaveChangesAsync();
                        emb.WithDescription($"Пользователь был {(Reason == null ? "разбанен" : "забанен")}!");
                    }
                    else
                        emb.WithDescription("Пользователя с таким Id, нет в базе!");
                }
            }
        }

        [Aliases, Commands, Usage, Descriptions]
        [RequireOwner]
        public async Task feedbugs(ulong Number = 0, StatusTicketEnum ChangeStatus = StatusTicketEnum.none,string Comment = null)
        {
            using (db _db = new ())
            {
                var emb = new EmbedBuilder().WithColor(255, 0, 94).WithAuthor("Тикеты");
                if (Number == 0)
                {
                    var Prefix = await _db.Guilds.GetOrCreate(Context.Guild.Id);

                    var List = _db.Feedback.AsEnumerable();
                    if (List.Any())
                    {
                        List = List.OrderByDescending(x=>x.Status == StatusTicketEnum.Отправлен);
                        await ListBuilder.ListButtonSliderBuilder(List, emb, _command.Name, Context, 3);
                        return;
                    }
                    else
                        emb.WithDescription("Нету отчетов.").WithFooter($"Создать - {Prefix.Prefix}feedback [text]");
                }
                else
                {
                    var ThisFeedBack = _db.Feedback.FirstOrDefault(x => x.Id == Number);
                    if (ThisFeedBack == null)
                        emb.WithDescription("Данная заявка не найдена!").WithAuthor("Feedback [ошибка]");
                    else if (ChangeStatus == StatusTicketEnum.none)
                    {
                        emb.WithDescription($"Ошибка: {ThisFeedBack.Message}").AddField("Статус", ThisFeedBack.Status.ToString(), true)
                                                                              .AddField("Дата", ThisFeedBack.Time.ToString("HH:mm:ss dd.MM.yy"), true)
                                                                              .AddField("Пользователь", $"<@{ThisFeedBack.UserId}>",true)
                                                                              .WithAuthor($"Feedback №{ThisFeedBack.Id}");
                    }
                    else if (ChangeStatus == StatusTicketEnum.Удалена)
                    {
                        emb.WithDescription($"Заявка удалена.").WithAuthor($"Feedback №{ThisFeedBack.Id}");
                        _db.Feedback.Remove(ThisFeedBack);
                        await _db.SaveChangesAsync();
                    }
                    else
                    {
                        emb.WithDescription($"Статус заявки изменен с {ThisFeedBack.Status} на {ChangeStatus}").WithAuthor($"Feedback №{ThisFeedBack.Id}");
                        ThisFeedBack.Status = ChangeStatus;
                        ThisFeedBack.AdminMessage = Comment;
                        _db.Feedback.Update(ThisFeedBack);
                        await _db.SaveChangesAsync();
                    }

                }
                await Context.Channel.SendMessageAsync("", false, emb.Build());
            }
        }
    }
}
