using DarlingDb;
using DarlingDb.Models;
using DarlingDb.Models.ReportSystem;
using DarlingNet.Services;
using DarlingNet.Services.LocalService;
using DarlingNet.Services.LocalService.Attribute;
using DarlingNet.Services.LocalService.GetOrCreate;
using DarlingNet.Services.LocalService.VerifiedAction;
using Discord;
using Discord.Commands;
using Discord.Rest;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static DarlingDb.Enums;
using static DarlingDb.Models.Guilds;
using static DarlingDb.Models.Guilds_Logs;
using static DarlingDb.Models.Guilds_Warns;
using static DarlingNet.Services.LocalService.Attribute.CommandLocksAttribute;

namespace DarlingNet.Modules
{
    [RequireUserPermission(GuildPermission.Administrator)]
    [RequireBotPermission(ChannelPermission.SendMessages)]
    [RequireBotPermission(ChannelPermission.EmbedLinks)]
    [Summary("Настройки бота\nдля сервера")]
    public class Settings : ModuleBase<ShardedCommandContext>
    {
        private readonly CommandService _commands;

        public Settings(CommandService commands)
        {
            _commands = commands;
        } // Подключение компонентов

        //[Aliases, Commands, Usage, Descriptions]
        //[PermissionBlockCommand, DarlingBoostPermission]
        //public async Task Stinger(string accept = null)
        //{
        //    using (DBcontext _db = new ())
        //    {
        //        var emb = new EmbedBuilder().WithAuthor("🔨 Stinger System").WithColor(255, 0, 94);
        //        var Guild = _db.Guilds.GetOrCreate(Context.Guild);
        //        var ThisUser = _db.Users.GetUser(Context.User);
        //        var Permission = (Context.Channel as SocketTextChannel).PermissionOverwrites.FirstOrDefault(x => x.TargetId == Context.Guild.EveryoneRole.Id).Permissions.ViewChannel;
        //        if(Permission == PermValue.Deny)
        //        {
        //            if(!ThisUser.StingerRulesAccept)
        //            {
        //                emb.WithDescription("Данная команда, открывает доступ к системе недоступной обычным пользователям.\n" +
        //                                    "Бот собирает информацию о выданных нарушениях на вашем сервере, и составляет\n" +
        //                                    "на основе их статистику, которую может увидеть каждый создатель сервера, если\n" +
        //                                    "данный пользователь заходит или находится на его сервере.\n\n" +

        //                                    "БЕЗ ВАШЕГО СОГЛАСИЯ, МЫ СОБИРАЕМ ТОЛЬКО НАРУШЕНИЯ ОТ БОТА ИЛИ ОТ СИСТЕМЫ ДИСКОРД(БАН, КИК)\n\n" +

        //                                    $"Прописав комамнду ``{Guild.Prefix}stinger accept``, вы получаете доступ к работе системы,\n" +
        //                                    "и подтверждаете то что мы можем забрать доступ к системе, в случае умышленного\n" +
        //                                    "нарушения каким либо способом работы системы, нарушения статистики, и других\n" +
        //                                    "действий, который могут помешать или искозить работу и выдачу статистики.\n\n" +

        //                                    "В случае нахождения ошибок, или других проблем в работе, \n" +
        //                                    $"вы можете написать команду {Guild.Prefix}bug[описание бага].\n\n" +

        //                                    "Небольшая просьба.Не распространяйте информацию о статистике пользователей!\n" +
        //                                    "Мы вам доверяем, поэтому это остается на вашей совести, вы и другие пользователи\n" +
        //                                    "у которых есть доступ, строите единую систему, помогая вычислять плохих пользователей\n" +
        //                                    "и заранее устранять их в случае, опасности.\n\n" +

        //                                    "Вы можете очень поможете нам, распространив бота по другим серверам, чтобы бот мог\n" +
        //                                    "отслеживать как можно больше пользователей, и собирать более точную статистику!\n");
        //            }
        //            else
        //            {
        //                var StatisticGivePoint = _db.ReportSuspens.Include(x => x.Administrator).Count(x => x.Administrator.GuildsId == Context.Guild.Id);
        //                emb.WithDescription($"Благодаря вам, мы получилии `{StatisticGivePoint}` нарушений в систему.");
        //            }
        //        }
        //        else
        //            emb.WithDescription("Данную команду нельзя использовать в публичных каналах!");
        //        await Context.Channel.SendMessageAsync("",false, emb.Build());
        //    }
        //}

        //[Aliases, Commands, Usage, Descriptions]
        //[PermissionBlockCommand, DarlingBoostPermission, StingerPermission]
        //public async Task Stinger(SocketGuildUser User)
        //{
        //    using (DBcontext _db = new ())
        //    {
        //        var emb = new EmbedBuilder().WithAuthor("🔨 Stinger System").WithColor(255, 0, 94);
        //        var Guild = _db.Guilds.GetOrCreate(Context.Guild);
        //        var Permission = (Context.Channel as SocketTextChannel).PermissionOverwrites.FirstOrDefault(x => x.TargetId == Context.Guild.EveryoneRole.Id).Permissions.ViewChannel;
        //        if (Permission == PermValue.Deny)
        //        {
        //            var UserDb = _db.Suspens.Include(x=>x.Reports).FirstOrDefault(x=>x.UserId == User.Id);
        //            emb.WithTitle($"{User}").WithThumbnailUrl(User.GetAvatarUrl());
        //            if (UserDb == null)
        //            {
        //                emb.WithDescription("О пользователе нету никакой информации!");
        //            }
        //            else
        //            {
        //                emb.WithDescription($"Репутация: {UserDb.UserBall}/10000\n\nПоследние 5 нарушений:\n");
        //                foreach (var Report in UserDb.Reports.OrderBy(x=>x.Time).Take(5))
        //                {
        //                    emb.Description += $"{Report.TypeReport} - {Report.Time}\n";
        //                }
        //                emb.Description += "Обратите внимание, статистику может испортить недавнее нарушение\nпоэтому не все пользователи плохие!";
        //            }
        //        }
        //        else
        //            emb.WithDescription("Данную команду нельзя использовать в публичных каналах!");
        //        await Context.Channel.SendMessageAsync("",false, emb.Build());
        //    }
        //}

        [Aliases, Commands, Usage, Descriptions]
        [PermissionBlockCommand]
        public async Task buttonclick()
        {
            using (db _db = new ())
            {
                var emb = new EmbedBuilder().WithAuthor("🔨 Действие на кнопку").WithColor(255, 0, 94);
                var Buttons = _db.ButtonClick.Include(x => x.Channel).Where(x => x.Channel.GuildsId == Context.Guild.Id);

                string linq = "https://discord.com/channels/{0}/{1}/{2}";
                foreach (var Button in Buttons)
                {
                    string Type = string.Empty;
                    if (!string.IsNullOrWhiteSpace(Button.Value))
                    {
                        var embed = JsonToEmbed.JsonCheck(Button.Value);
                        if (embed.Item1 == null && embed.Item2 == null)
                        {
                            string regex = @"(https?:\/\/|ftps?:\/\/|www\.)((?![.,?!;:()]*(\s|$))[^\s]){2,}";
                            if (Regex.IsMatch(Button.Value, regex, RegexOptions.IgnoreCase))
                                Type = "url-ссылка";
                            else
                                Type = "текстовое сообщение";
                        }
                        else
                            Type = "embed сообщение";

                    }

                    ulong Role = Convert.ToUInt64(Button.RoleId);
                    if (Role != 0)
                    {
                        if (Button.RoleDelOrGet)
                        {
                            if (Type == string.Empty)
                                Type = "Выдача роли ";
                            else
                                Type += " + выдача роли ";
                        }
                        else
                        {
                            if (Type == string.Empty)
                                Type = "Удаление роли ";
                            else
                                Type += " + удаление роли ";
                        }
                        Type += $"[<@{Role}>]";
                    }

                    string text = string.Empty;
                    if (!string.IsNullOrWhiteSpace(Button.Value))
                        text = $"Значение: ||{Button.Value}||";
                    emb.Description += $"{Button.Id}.[Сообщение]({string.Format(linq, Context.Guild.Id, Button.ChannelId, Button.MessageId)}) Тип:{Type} {text}\n";
                }
                if (emb.Description == null)
                    emb.Description += "Кнопок с действиями нету!";
                else
                {
                    var Prefix = await _db.Guilds.GetPrefix(Context.Guild.Id);
                    emb.WithFooter($"Добавить кнопку - {Prefix}i buttonclick.add\nУдалить кнопку - {Prefix}i buttonclick.del");
                }


                await Context.Channel.SendMessageAsync("", false, emb.Build());
            }
        }

        [Aliases, Commands, Usage, Descriptions]
        [PermissionBlockCommand]
        public async Task buttonclickaddrole(ulong buttonid, SocketRole Role = null, string RoleStatus = "выдать")
        {
            using (db _db = new ())
            {
                var emb = new EmbedBuilder().WithAuthor("🔨 Действие на кнопку - добавление").WithColor(255, 0, 94);
                var GetButton = _db.ButtonClick.FirstOrDefault(x => x.Id == buttonid);
                if (GetButton != null)
                {
                    if (Role == null)
                    {
                        emb.WithDescription($"Вы успешно убрали {Role.Mention} с кнопки!");
                        GetButton.RoleId = 0;
                        _db.ButtonClick.Update(GetButton);
                        await _db.SaveChangesAsync();
                    }
                    else
                    {
                        emb.Description = Role.RolePermission();
                        if (emb.Description == null)
                        {
                            bool StatusRole = false;
                            string text = string.Empty;
                            if (RoleStatus == "выдать" || RoleStatus == "убрать")
                            {
                                if (RoleStatus == "выдать")
                                {
                                    text = "выдачу";
                                    StatusRole = true;
                                }
                                else
                                    text = "удаление";

                                await _db.Role.GetOrCreate(Role);
                                GetButton.RoleDelOrGet = StatusRole;
                                GetButton.RoleId = Role.Id;
                                _db.ButtonClick.Update(GetButton);
                                await _db.SaveChangesAsync();
                                emb.WithDescription($"Вы успешно поставили {text} роли на кнопку!");
                            }
                            else
                                emb.WithDescription("Роль можно только `выдать` или `убрать`. Значение RoleStatus - может содержать только эти значения.");
                        }
                    }
                }
                else
                {
                    var Prefix = await _db.Guilds.GetPrefix(Context.Guild.Id);
                    emb.WithDescription("Кнопка с таким номером не найдена!").WithFooter($"Узнать номер - {Prefix}buttonclick");
                }

                await Context.Channel.SendMessageAsync("", false, emb.Build());
            }
        }

        [Aliases, Commands, Usage, Descriptions]
        [PermissionBlockCommand]
        public async Task buttonclickadd(ulong messageid, [Remainder] string ButtonTextAndMessage)
        {
            using (db _db = new ())
            {
                var emb = new EmbedBuilder().WithAuthor("🔨 Действие на кнопку - добавление").WithColor(255, 0, 94);
                RestUserMessage Message = null;
                foreach (var Channel in Context.Guild.TextChannels)
                {
                    Message = await Channel.GetMessageAsync(messageid) as RestUserMessage;
                    if (Message != null)
                        break;
                }
                if (Message == null)
                    emb.WithDescription($"Сообщение с номером {messageid} не найдено");
                else
                {
                    if (ButtonTextAndMessage.Contains('|'))
                    {
                        var ButtonText = ButtonTextAndMessage.Split("|")[0];
                        var message = ButtonTextAndMessage.Split("|")[1];
                        await _db.Channels.GetOrCreate(Context.Guild.Id, Message.Channel.Id);
                        var NewButton = new ButtonClick() { ChannelId = Message.Channel.Id, MessageId = Message.Id, Value = message };

                        _db.ButtonClick.Add(NewButton);
                        await _db.SaveChangesAsync();
                        NewButton = _db.ButtonClick.FirstOrDefault(x => x == NewButton);
                        var NewComp = new ComponentBuilder();

                        string regex = @"(https?:\/\/|ftps?:\/\/|www\.)((?![.,?!;:()]*(\s|$))[^\s]){2,}";

                        if (Regex.IsMatch(message, regex, RegexOptions.IgnoreCase))
                            NewComp.WithButton("", $"ButtonClickAction_{NewButton.Id}", ButtonStyle.Link, null, message);
                        else
                            NewComp.WithButton(ButtonText, $"ButtonClickAction_{NewButton.Id}");


                        foreach (var Comp in Message.Components)
                        {
                            foreach (var Button in Comp.Components)
                            {
                                NewComp.WithButton(Button as ButtonBuilder);
                            }
                        }

                        await Message.ModifyAsync(x =>
                        {
                            x.Components = NewComp.Build();
                        });

                        var Prefix = await _db.Guilds.GetPrefix(Context.Guild.Id);
                        emb.WithDescription($"Вы успешно создали действие за нажатие кнопки.").WithFooter($"Взаимодействие с ролям, при нажатии - {Prefix}i buttonclick.addrole");
                    }
                    else
                        emb.WithDescription("Для создания кнопки используйте разделитель `|`, который будет разделять название кнопки, и текст выдаваемый при ее нажатии.\nПример: Нажми на меня|Зачем ты нажал?");


                }
                await Context.Channel.SendMessageAsync("", false, emb.Build());
            }
        }

        [Aliases, Commands, Usage, Descriptions]
        [PermissionBlockCommand]
        public async Task buttonclickdel(ulong ButtonId)
        {
            using (db _db = new ())
            {
                var emb = new EmbedBuilder().WithAuthor("🔨 Действие на кнопку - добавление").WithColor(255, 0, 94);
                var GetButton = _db.ButtonClick.FirstOrDefault(x => x.Id == ButtonId);
                if (GetButton == null)
                    emb.WithDescription("Сообщение с этим id, не имеет привязанной кнопки.");
                else
                {
                    var mes = Context.Guild.TextChannels.FirstOrDefault(x => x.GetMessageAsync(GetButton.MessageId).Result != null);
                    var Message = await mes?.GetMessageAsync(GetButton.MessageId) as RestUserMessage;
                    if (mes == null || Message == null)
                        emb.WithDescription($"Сообщение с этой кнопкой не найдено. Возможно сообщение удалено?");
                    else
                    {
                        var NewComp = new ComponentBuilder();

                        foreach (var Comp in Message.Components)
                        {
                            foreach (var Button in Comp.Components.Where(x => x.CustomId != $"ButtonClickAction_{GetButton.Id}"))
                            {
                                NewComp.WithButton(Button as ButtonBuilder);
                            }
                        }

                        await Message.ModifyAsync(x => x.Components = NewComp.Build());

                        emb.WithDescription("Кнопка успешно удалена!");
                        _db.ButtonClick.Remove(GetButton);
                        await _db.SaveChangesAsync();
                    }
                }

                await Context.Channel.SendMessageAsync("", false, emb.Build());
            }
        }

        [Aliases, Commands, Usage, Descriptions]
        [PermissionBlockCommand]
        public async Task emoteclick()
        {
            using (db _db = new ())
            {
                var emb = new EmbedBuilder().WithAuthor("🔨 Действие на эмодзи").WithColor(255, 0, 94);
                var Emotes = _db.EmoteClick.Include(x => x.Channel).Where(x => x.Channel.GuildsId == Context.Guild.Id);
                emb.WithDescription("Инструкция: [КЛИК](https://docs.darlingbot.ru/commands/settings-server/rol-po-nazhatiyu-na-emodzi)\nЗадачи:");
                int i = 0;
                foreach (var Emote in Emotes)
                {
                    i++;
                    emb.Description += $"\n{i}.<#{Emote.ChannelId}> <@&{Emote.RoleId}> {Emote.Emote} {(Emote.Get ? "Выдается" : "Убирается")}";
                }
                if (i == 0)
                    emb.Description += " Ролей за эмодзи нет!";

                await Context.Channel.SendMessageAsync("", false, emb.Build());
            }
        }

        [Aliases, Commands, Usage, Descriptions]
        [PermissionBlockCommand]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        [RequireBotPermission(GuildPermission.AddReactions)]
        public async Task emoteclickadd(ulong messageid, string emote, SocketRole role, string RoleStatus = "выдать")
        {
            using (db _db = new ())
            {
                var emb = new EmbedBuilder().WithAuthor("🔨 Действие на эмодзи - добавление").WithColor(255, 0, 94);
                IMessage Message = null;
                foreach (var Channel in Context.Guild.TextChannels)
                {
                    Message = await Channel.GetMessageAsync(messageid);
                    if (Message != null)
                        break;
                }
                if (Message == null)
                    emb.WithDescription($"Сообщение с номером {messageid} не найдено");
                else
                {
                    var Emoji = Context.Guild.Emotes.FirstOrDefault(x => $"<:{x.Name}:{x.Id}>" == emote);
                    if (Emoji != null)
                    {
                        emb.Description = role.RolePermission();
                        if (emb.Description == null)
                        {
                            if (!_db.EmoteClick.Any(x => x.MessageId == Message.Id && x.Emote == emote))
                            {
                                bool StatusRole = false;
                                string text = string.Empty;
                                if (RoleStatus == "выдать" || RoleStatus == "убрать")
                                {
                                    if (RoleStatus == "выдать")
                                    {
                                        text = "выдачу";
                                        StatusRole = true;
                                    }
                                    else
                                        text = "удаление";

                                    await Message.AddReactionAsync(Emoji);
                                    _db.EmoteClick.Add(new EmoteClick() { MessageId = messageid, Emote = emote, Get = StatusRole, RoleId = role.Id, ChannelId = Message.Id });
                                    await _db.SaveChangesAsync();
                                    emb.WithDescription($"Вы успешно создали {text} {role.Mention} за нажатие {Emoji}");

                                }
                                else
                                    emb.WithDescription("Роль можно только `выдать` или `убрать`. Значение RoleStatus - может содержать только эти значения.");
                            }
                            else
                                emb.WithDescription("Выдача роли на это сообщение с таким эмодзи, уже существует!");
                        }
                    }
                    else emb.WithDescription("Эмодзи не найдено. Возможно вы используете не серверные эмодзи?");
                }
                await Context.Channel.SendMessageAsync("", false, emb.Build());
            }
        }

        [Aliases, Commands, Usage, Descriptions]
        [PermissionBlockCommand]
        public async Task emoteclickdel(uint id)
        {
            using (db _db = new ())
            {
                var emb = new EmbedBuilder().WithAuthor("🔨 Действие на эмодзи - удаление").WithColor(255, 0, 94);
                if (id != 0)
                    id--;
                var Emote = _db.EmoteClick.Include(x=>x.Channel).Where(x => x.Channel.GuildsId == Context.Guild.Id).AsEnumerable().ElementAt((int)id);
                if (Emote == null)
                    emb.WithDescription("Задача с таким id не найдена!");
                else
                {
                    emb.WithDescription($"Задача {(Emote.Get ? "выдачи" : "удаления")} роли <@&{Emote.RoleId}> на эмодзи {Emote.Emote} в канале <#{Emote.ChannelId}> была удалена!");
                    _db.Remove(Emote);
                    await _db.SaveChangesAsync();
                }

                await Context.Channel.SendMessageAsync("", false, emb.Build());
            }
        }

        [Aliases, Commands, Usage, Descriptions]
        [PermissionBlockCommand]
        public async Task task()
        {
            using (db _db = new ())
            {
                var emb = new EmbedBuilder().WithColor(255, 0, 94).WithAuthor("⏰ Запланированные задачи");
                var EmbedBoost = Context.Guild.DarlingBoostGet();
                if (EmbedBoost != null)
                    emb = EmbedBoost;
                else
                {
                    var Tasks = _db.Tasks.Include(x => x.Channel).Where(x => x.Channel.GuildsId == Context.Guild.Id && ((x.Times > DateTime.Now && !x.Repeat) || x.Repeat));
                    emb.WithDescription("Инструкция к команде: [Click](https://docs.darlingbot.ru/commands/settings-server/otlozhennye-zadachi) \nВаши задачи: ");
                    int i = 0;
                    foreach (var Task in Tasks)
                    {
                        string text = string.Empty;
                        i++;
                        var Check = JsonToEmbed.JsonCheck(Task.Message);
                        if (Check.Item2 == null)
                            text = Task.Message;
                        else
                            text = Check.Item1.Description;

                        emb.Description += $"\n\n{i}.<#{Task.ChannelId}> {(Task.Repeat ? $"Повторять в: {Task.Times.ToShortTimeString()}" : $"в {Task.Times}")}  текст: ||{text}||";
                    }
                    if (i == 0)
                        emb.Description += "отсутствуют!";
                    var Prefix = await _db.Guilds.GetPrefix(Context.Guild.Id);
                    emb.AddField("Добавить задачу", $"{Prefix}ts.Add [Channel] [repeat] [Time] [Message]", true);
                    emb.AddField("Удалить задачу", $"{Prefix}ts.Del [id]", true);
                }
                await Context.Channel.SendMessageAsync("", false, emb.Build());
            }
        }

        [Aliases, Commands, Usage, Descriptions]
        [PermissionBlockCommand]

        public async Task taskadd(SocketTextChannel TextChannel, bool repeat, string Time, [Remainder] string message)
        {
            using (db _db = new ())
            {
                var emb = new EmbedBuilder().WithColor(255, 0, 94).WithAuthor("⏰ Запланированные задачи - добавление");
                var EmbedBoost = Context.Guild.DarlingBoostGet();
                if (EmbedBoost != null)
                    emb = EmbedBoost;
                else
                {
                    bool tried = false;
                    Time = Time.Replace('-', ' ');
                    _ = DateTime.TryParse(Time, out DateTime dtfi);
                    if (dtfi.Year == 1)
                    {
                        tried = true;
                        var TimeNow = DateTime.Now.ToString("05.10.yy-HH:mm");
                        emb.WithDescription($"Дата введена некорректно. Пример: {TimeNow}.");
                    }

                    if (!tried)
                    {
                        if (!_db.Tasks.Any(x => x.Times == dtfi && x.Message == message && x.ChannelId == TextChannel.Id))
                        {
                            if (dtfi > DateTime.Now)
                            {
                                if ((dtfi - DateTime.Now).TotalMinutes >= 1 && (dtfi - DateTime.Now).TotalDays <= 31)
                                {
                                    var CountTask = _db.Tasks.Include(x => x.Channel).Count(x => x.Channel.GuildsId == Context.Guild.Id);
                                    if (CountTask <= 15)
                                    {
                                        var Tasks = new Tasks() { ChannelId = TextChannel.Id, Message = message, Times = dtfi, Repeat = repeat };
                                        _db.Tasks.Add(Tasks);
                                        await _db.SaveChangesAsync();
                                        await TaskTimer.StartTaskTimer(Tasks);
                                        emb.WithDescription($"Сообщение отложено в канал {TextChannel.Mention} на {(!repeat ? dtfi.ToString("dd/MM/yyyy HH:mm") : $"повторение в {dtfi.ToShortTimeString()}")}.");
                                    }
                                    else
                                        emb.WithDescription("Создать больше 15 отложенных сообщений нельзя!");
                                }
                                else emb.WithDescription("Время может быть меньше 1 минуты и не больше 31 дня!");
                            }
                            else emb.WithDescription("Время не может быть меньше текущего!");
                        }
                        else
                            emb.WithDescription("У вас уже создана отложенная задача с похожими параметрами!");
                    }
                }


                await Context.Channel.SendMessageAsync("", false, emb.Build());
            }
        }

        [Aliases, Commands, Usage, Descriptions]
        [PermissionBlockCommand]
        public async Task taskdel(uint id)
        {
            using (db _db = new ())
            {
                var emb = new EmbedBuilder().WithColor(255, 0, 94).WithAuthor("⏰ Запланированные задачи - добавление");
                var EmbedBoost = Context.Guild.DarlingBoostGet();
                if (EmbedBoost != null)
                    emb = EmbedBoost;
                else
                {
                    if (id != 0)
                        id--;
                    var Tasks = _db.Tasks.Include(x => x.Channel).Where(x => x.Channel.GuildsId == Context.Guild.Id).AsEnumerable().ElementAt((int)id);

                    if (Tasks == null)
                    {
                        var Prefix = await _db.Guilds.GetPrefix(Context.Guild.Id);
                        emb.WithDescription("Задача с таким id не найдена!").WithFooter($"Узнать id -> {Prefix}Task");
                    }
                    else
                    {
                        emb.WithDescription($"Задача в канал <#{Tasks.ChannelId}> на {(Tasks.Repeat ? $"повторение в {Tasks.Times.ToShortTimeString()}" : Tasks.Times.ToString())} была удалена!");
                        _db.Remove(Tasks);
                        await _db.SaveChangesAsync();
                    }
                }
                await Context.Channel.SendMessageAsync("", false, emb.Build());
            }
        }

        private static string SelectChannelType(string[] ChannelTypes, uint num, bool change)
        {
            string text = string.Empty;
            _ = Enum.TryParse(ChannelTypes[num], out ChannelsTypeEnum myStatus);
            switch (myStatus)
            {
                case ChannelsTypeEnum.Ban:
                    return $"Бан{(change ? "е" : "")} пользователя";
                case ChannelsTypeEnum.UnBan:
                    return $"Разбан{(change ? "е" : "")} пользователя";
                case ChannelsTypeEnum.Kick:
                    return $"Кик{(change ? "е" : "")} пользователя";
                case ChannelsTypeEnum.Left:
                    return $"Выход{(change ? "е" : "")} пользователя";
                case ChannelsTypeEnum.Join:
                    return $"Вход{(change ? "е" : "")} пользователя";
                case ChannelsTypeEnum.MessageEdit:
                    return $"Измененны{(change ? "х" : "е")} сообщения{(change ? "х" : null)}";
                case ChannelsTypeEnum.MessageDelete:
                    return $"Удаленны{(change ? "х" : "е")} сообщения{(change ? "х" : null)}";
                case ChannelsTypeEnum.VoiceAction:
                    return $"Активност{(change ? "и" : "ь")} в голосовых чатах";
                case ChannelsTypeEnum.BirthDay:
                    return $"Поздравлени{(change ? "и" : "е")} пользователей с их днем рождения";
            }
            return text;
        }

        [Aliases, Commands, Usage, Descriptions]
        [PermissionBlockCommand]
        public async Task logsettings(uint selection = 0, SocketTextChannel channel = null)
        {
            using (db _db = new ())
            {
                var Prefix = await _db.Guilds.GetPrefix(Context.Guild.Id);
                var emb = new EmbedBuilder().WithColor(255, 0, 94).WithAuthor(" - Логирование на сервере", Context.Guild.IconUrl);
                var ChannelTypes = Enum.GetNames(typeof(ChannelsTypeEnum));
                if (selection == 0 && channel == null)
                {
                    var ChannelLogs = _db.Guilds_Logs.GetLogChannel(Context.Guild.Id);
                    for (uint i = 1; i < ChannelTypes.Length; i++)
                    {
                        SocketTextChannel Channel = null;
                        string text = SelectChannelType(ChannelTypes, i, false);
                        var This = ChannelLogs.FirstOrDefault(x => x.Type.ToString() == ChannelTypes[i]);
                        if (This != null)
                        {
                            Channel = Context.Guild.GetTextChannel(This.ChannelId);
                            if (Channel == null)
                            {
                                _db.Guilds_Logs.Remove(This);
                                await _db.SaveChangesAsync();
                            }
                        }
                        emb.AddField($"{i}.{text}", Channel == null ? "Канал не указан" : Channel.Mention, true);
                    }
                    emb.WithFooter($"Включить - {Prefix}LogSettings [цифра] [канал]\nОтключить - {Prefix}LogSettings [цифра]");
                }
                else
                {
                    if (!(selection >= 1 && selection <= ChannelTypes.Length))
                        emb.WithDescription($"Выбор может быть только от 1 до {ChannelTypes.Length}").WithFooter($"Подробнее - {Prefix}LogSettings");
                    else
                    {
                        string text = SelectChannelType(ChannelTypes, selection, true);
                        bool success = false;
                        _ = Enum.TryParse(ChannelTypes[selection], out ChannelsTypeEnum myStatus);
                        var ChannelLogs = _db.Guilds_Logs.GetLogChannel(Context.Guild.Id, myStatus);

                        if (channel == null)
                        {
                            if (ChannelLogs == null)
                                emb.WithDescription($"Отправка сообщений о {text} и так отключена");
                            else
                            {
                                success = true;
                                _db.Guilds_Logs.Remove(ChannelLogs);
                                emb.WithDescription($"Отправка сообщений о {text} была отключена");
                            }
                        }
                        else
                        {
                            if (ChannelLogs == null || ChannelLogs.Id != channel.Id)
                            {
                                if (ChannelLogs != null)
                                {
                                    ChannelLogs.ChannelId = channel.Id;
                                    _db.Guilds_Logs.Update(ChannelLogs);
                                }
                                else
                                {
                                    await _db.Channels.GetOrCreate(channel.Guild.Id, channel.Id);
                                    _db.Guilds_Logs.Add(new Guilds_Logs() { ChannelId = channel.Id, Type = myStatus });
                                }

                                success = true;
                                emb.WithDescription($"Отправка сообщений о {text} включена в канал {channel.Mention}");
                            }
                            else
                                emb.WithDescription($"Отправка сообщений о {text} уже включена в канал {channel.Mention}");
                        }
                        if (success)
                            await _db.SaveChangesAsync();

                    }
                }
                await Context.Channel.SendMessageAsync("", false, emb.Build());
            }
        }

        [Aliases, Commands, Usage, Descriptions]
        [PermissionBlockCommand]
        public async Task prefix(string prefix = null)
        {
            using (db _db = new())
            {
                var Guild = await _db.Guilds.GetOrCreate(Context.Guild.Id);
                var emb = new EmbedBuilder().WithColor(255, 0, 94).WithAuthor("💬 Префикс бота");
                if (prefix == null)
                    emb.WithDescription($"Префикс сервера - **{Guild.Prefix}**").WithFooter($"Изменить/Просмотреть - {Guild.Prefix}p [prefix/null]");
                else
                {
                    const int MaxLenght = 4;
                    if (prefix.Length > MaxLenght)
                        emb.WithDescription($"Префикс не может быть длиньше {MaxLenght} символов");
                    else
                    {
                        emb.WithDescription($"Префикс сервера изменен с `{Guild.Prefix}` на `{prefix}`");
                        Guild.Prefix = prefix;
                        _db.Guilds.Update(Guild);
                        await _db.SaveChangesAsync();
                    }
                }
                await Context.Channel.SendMessageAsync("", false, emb.Build());
            }
        }

        [Aliases, Commands, Usage, Descriptions]
        [PermissionBlockCommand, PermissionViolation]
        [RequireBotPermission(GuildPermission.BanMembers)]
        [RequireBotPermission(GuildPermission.KickMembers)]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        public async Task reportrulesadd([Remainder] string Rules)
        {
            using (db _db = new ())
            {
                var emb = new EmbedBuilder().WithColor(255, 0, 94).WithAuthor("Добавление правила");
                var Guild = _db.Guilds.Include(x => x.Reports).FirstOrDefault(x => x.Id == Context.Guild.Id);
                if (Guild.Reports.Count <= 25)
                {
                    emb.WithDescription("Вы успешно добавили правило!").WithFooter($"Время установить наказания за нарушения - {Guild.Prefix}arp");
                    _db.Reports.Add(new Reports { GuildsId = Context.Guild.Id, Rules = Rules });
                    await _db.SaveChangesAsync();
                }
                else emb.WithDescription("Нельзя добавлять больше 25 правил!");

                await Context.Channel.SendMessageAsync("", false, emb.Build());
            }
        }

        [Aliases, Commands, Usage, Descriptions]
        [PermissionBlockCommand, PermissionViolation]
        [RequireBotPermission(GuildPermission.BanMembers)]
        [RequireBotPermission(GuildPermission.KickMembers)]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        public async Task reportpunishesadd(byte NumberRules, ReportTypeEnum report, string Time = null)
        {
            using (db _db = new ())
            {
                var emb = new EmbedBuilder().WithColor(255, 0, 94).WithAuthor("Добавить нарушение к правилу");
                if (NumberRules == 0)
                    NumberRules = 1;
                const string error = "Варн должен содержать время, возможно вы ввели большое число?.\nПример: 01:00:00 [ч:м:с]\nПример 2: 10:00:00:00 [д:ч:м:с]";
                bool Success = TimeSpan.TryParse(Time, out TimeSpan result);
                if ((report == ReportTypeEnum.TimeBan || report == ReportTypeEnum.TimeOut) && !Success)
                    emb.WithDescription(error);
                else if (result.TotalSeconds > 604800)
                    emb.WithDescription("Время нарушения не может превышать 7 дней!");
                else
                {
                    var Prefix = await _db.Guilds.GetOrCreate(Context.Guild.Id);
                    var RulesList = _db.Reports.Include(x => x.ReportsList).Where(x => x.GuildsId == Context.Guild.Id).AsEnumerable();
                    if (RulesList.Any())
                    {
                        var thisRules = RulesList.ElementAtOrDefault(NumberRules - 1);
                        if (thisRules == null)
                            emb.WithDescription("Правила с таким номер нету!");
                        else
                        {
                            if (thisRules.ReportsList.Count <= 5)
                            {
                                emb.WithDescription("Нарушение было успешно добавлено.").WithFooter($"Открыть правила - {Prefix}rules");
                                var NewReport = new Reports_Punishes() { ReportId = thisRules.Id, TypeReport = report };
                                if (report == ReportTypeEnum.TimeOut || report == ReportTypeEnum.TimeBan)
                                    NewReport.TimeReport = result;
                                _db.Reports_Punishes.Add(NewReport);
                                await _db.SaveChangesAsync();
                            }
                            else
                                emb.WithDescription("Вы не можете добавлять больше 5 нарушений на 1 правило.");
                        }
                    }
                    else
                    {
                        emb.WithDescription("У вас еще нету правил!").WithFooter($"Добавить - {Prefix}arr [rules text]");
                    }
                }
                await Context.Channel.SendMessageAsync("", false, emb.Build());
            }
        }

        [Aliases, Commands, Usage, Descriptions]
        [PermissionBlockCommand, PermissionViolation]
        [RequireBotPermission(GuildPermission.BanMembers)]
        [RequireBotPermission(GuildPermission.KickMembers)]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        public async Task reportrulesdel(ushort NumberRules)
        {
            using (db _db = new ())
            {
                var emb = new EmbedBuilder().WithColor(255, 0, 94).WithAuthor("Удалить правило");
                if (NumberRules == 0)
                    NumberRules = 1;
                var Rules = _db.Reports.Where(x => x.GuildsId == Context.Guild.Id);
                var Prefix = await _db.Guilds.GetOrCreate(Context.Guild.Id);
                if (!Rules.Any())
                    emb.WithDescription("У вас нету правил!")
                       .WithFooter($"Добавьте правила - {Prefix}arr [rules text]");
                else
                {
                    var ThisRules = Rules.AsEnumerable().ElementAtOrDefault(NumberRules - 1);
                    if (ThisRules == null)
                        emb.WithDescription("Правило с таким номером не найдено!")
                           .WithFooter($"Может вы хотели удалить нарушение? {Prefix}drp [number rules] [number punishes]");
                    else
                    {
                        _db.Reports.Remove(ThisRules);
                        await _db.SaveChangesAsync();
                        emb.WithDescription("Правило и его нарушения успешно удалены!");
                    }

                }
                await Context.Channel.SendMessageAsync("", false, emb.Build());
            }
        }

        [Aliases, Commands, Usage, Descriptions]
        [PermissionBlockCommand, PermissionViolation]
        [RequireBotPermission(GuildPermission.BanMembers)]
        [RequireBotPermission(GuildPermission.KickMembers)]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        public async Task reportpunishesdel(ushort NumberRules, ushort NumberPunishes)
        {
            using (db _db = new ())
            {
                var emb = new EmbedBuilder().WithColor(255, 0, 94).WithAuthor("Удалить нарушение правила");
                if (NumberRules == 0)
                    NumberRules = 1;
                if (NumberPunishes == 0)
                    NumberPunishes = 1;
                var Rules = _db.Reports.Where(x => x.GuildsId == Context.Guild.Id);
                if (!Rules.Any())
                    emb.WithDescription("Правило с таким номером не найдено!");
                else
                {
                    var ThisRules = Rules.AsEnumerable().ElementAtOrDefault(NumberRules - 1);
                    if (ThisRules != null)
                    {
                        var ThisPunishes = ThisRules.ReportsList.ElementAtOrDefault(NumberPunishes - 1);
                        if (ThisPunishes != null)
                        {
                            _db.Remove(ThisPunishes);
                            await _db.SaveChangesAsync();
                            emb.WithDescription("Нарушения успешно удалено!");
                        }
                        else
                            emb.WithDescription("Нарушение с таким номером не найдено!");
                    }
                    else
                        emb.WithDescription("Правило с таким номером не найдено!");


                }
                await Context.Channel.SendMessageAsync("", false, emb.Build());
            }
        }

        [Aliases, Commands, Usage, Descriptions]
        [PermissionBlockCommand, PermissionViolation]
        [RequireBotPermission(GuildPermission.BanMembers)]
        [RequireBotPermission(GuildPermission.KickMembers)]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        public async Task addwarn(byte CountWarn, ReportTypeEnum report, string Time = null)
        {
            using (db _db = new ())
            {
                var emb = new EmbedBuilder().WithColor(255, 0, 94).WithAuthor("⚜️ WarnSystem - Добавить варн");
                const string error = "Варн должен содержать время, возможно вы ввели большое число?\nПример: 01:00:00 [ч:м:с]\nПример 2: 07:00:00:00 [д:ч:м:с]";
                bool Success = TimeSpan.TryParse(Time, out TimeSpan result);
                if ((report == ReportTypeEnum.TimeBan || report == ReportTypeEnum.TimeOut) && !Success)
                    emb.WithDescription(error);
                else if (result.TotalSeconds > 604800)
                    emb.WithDescription("Время нарушения не может превышать 7 дней!");
                else
                {
                    if (CountWarn >= 1 && CountWarn <= 15)
                    {
                        var ThisWarn = _db.Guilds_Warns.FirstOrDefault(x => x.CountWarn == CountWarn);
                        if (ThisWarn != null)
                        {
                            emb.WithDescription($"Варн {CountWarn} был перезаписан с `{ThisWarn.ReportTypes}` на `{report}`.");
                            ThisWarn.ReportTypes = report;
                            ThisWarn.Time = result;
                            _db.Guilds_Warns.Update(ThisWarn);
                        }
                        else
                        {
                            emb.WithDescription($"Варн {CountWarn} был успешно добавлен.");
                            var newwarn = new Guilds_Warns() { GuildsId = Context.Guild.Id, CountWarn = CountWarn, ReportTypes = report, Time = result };
                            _db.Guilds_Warns.Add(newwarn);
                        }
                        var Prefix = await _db.Guilds.GetPrefix(Context.Guild.Id);
                        emb.WithFooter($"Посмотреть все варны {Prefix}ws");
                        await _db.SaveChangesAsync();

                    }
                    else emb.WithDescription($"Количество варнов может быть больше 1 и меньше 15");
                }
                await Context.Channel.SendMessageAsync("", false, emb.Build());
            }
        }

        [Aliases, Commands, Usage, Descriptions]
        [PermissionBlockCommand, PermissionViolation]
        public async Task delwarn(byte CountWarn)
        {
            using (db _db = new ())
            {
                var Prefix = await _db.Guilds.GetPrefix(Context.Guild.Id);
                var warn = _db.Guilds_Warns.FirstOrDefault(x => x.GuildsId == Context.Guild.Id && x.CountWarn == CountWarn);
                var emb = new EmbedBuilder().WithColor(255, 0, 94).WithAuthor("⚜️ WarnSystem - Удалить варн");
                if (warn != null)
                {
                    _db.Guilds_Warns.Remove(warn);
                    await _db.SaveChangesAsync();
                    emb.WithDescription($"Варн с номером {CountWarn} успешно удален.");
                }
                else emb.WithDescription($"Варн с номером {CountWarn} отсутствует.");

                emb.WithFooter($"Посмотреть все варны {Prefix}ws");
                await Context.Channel.SendMessageAsync("", false, emb.Build());
            }
        }

        [Aliases, Commands, Usage, Descriptions]
        [PermissionBlockCommand]
        public async Task channelsettings(SocketTextChannel channel = null, float number = 0)
        {
            using (db _db = new ())
            {
                var emb = new EmbedBuilder().WithColor(255, 0, 94);
                var Prefix = await _db.Guilds.GetPrefix(Context.Guild.Id);
                var chnl = new Channel();
                emb.WithAuthor($"🔨 Настройка каналов {(channel == null ? " " : $"- {channel.Name}")}");
                if (channel != null)
                    chnl = await _db.Channels.GetOrCreate(channel.Guild.Id, channel.Id);
                if (channel != null && number == 0)
                {
                    if (chnl != null)
                    {
                        emb.AddField("1 Получение опыта", chnl.GiveXP ? "Вкл" : "Выкл", true);
                        emb.AddField("2 Удалять ссылки", chnl.DelUrl ? "Вкл" : "Выкл", true);
                        if (chnl.DelUrl) emb.AddField("2.1 Удалять ссылки-изображения?", chnl.DelUrlImage ? "Вкл" : "Выкл", true);
                        if (chnl.DelUrl) emb.AddField("Белый список ссылок:", chnl.csUrlWhiteListList.Count == 0 ? "-" : string.Join(", ", chnl.csUrlWhiteListList), true);
                        emb.AddField("3 Удалять КАПС сообщения", chnl.DelCaps ? "Вкл" : "Выкл", true);
                        emb.AddField("4 Плохие слова", chnl.SendBadWord ? "Вкл" : "Выкл", true);
                        if (chnl.SendBadWord) emb.AddField("Плохие слова:", chnl.BadWordList.Count == 0 ? "-" : string.Join(", ", chnl.BadWordList), true);
                        emb.AddField("5 Использование команд", chnl.UseCommand ? "Вкл" : "Выкл", true);
                        if (!chnl.UseCommand) emb.AddField("5.1 Использование RP команд?", chnl.UseRPcommand ? "Вкл" : "Выкл", true);
                        if (!chnl.UseAdminCommand) emb.AddField("5.2 Использование Admin команд?", chnl.UseAdminCommand ? "Вкл" : "Выкл", true);
                        emb.AddField("6 Спам [BETA]", chnl.Spaming ? "Вкл" : "Выкл", true);
                        emb.AddField("7 Удалять приглашения(кроме тех что сюда)", chnl.InviteMessage ? "Вкл" : "Выкл", true);
                        emb.AddField("Номер Чата", chnl.Id, true);
                        emb.WithFooter($"Вкл/Выкл опции канала - {Prefix}cs [channel] [number]");
                    }
                    else emb.WithDescription("Данный канал не найден!");
                }
                else if (channel != null && number != 0)
                {
                    if (number >= 1 && number <= 7)
                    {
                        switch (number)
                        {
                            case 1:
                                chnl.GiveXP = !chnl.GiveXP;
                                emb.WithDescription($"Получение уровней в {channel.Mention} {(chnl.GiveXP ? "включено" : "выключено")}");
                                break;
                            case 2:
                                chnl.DelUrl = !chnl.DelUrl;
                                emb.WithDescription($"Ссылки в {channel.Mention} {(chnl.DelUrl ? "удаляются" : "не удаляются")}");
                                if (chnl.DelUrl)
                                {
                                    emb.WithFooter("Вы можете настроить удаление ссылок-картинок! Откройте команду еще раз!");
                                    chnl.DelUrlImage = true;
                                }
                                else
                                    chnl.DelUrlImage = false;
                                break;
                            case 2.1f:
                                chnl.DelUrlImage = !chnl.DelUrlImage;
                                emb.WithDescription($"Ссылки-картинки в {channel.Mention} {(chnl.DelUrlImage ? "удаляются" : "не удаляются")}");
                                break;
                            case 3:
                                chnl.DelCaps = !chnl.DelCaps;
                                emb.WithDescription($"КАПС-сообщения {channel.Mention} теперь {(chnl.DelCaps ? "удаляются" : "не удаляются")}");

                                break;
                            case 4:
                                chnl.SendBadWord = !chnl.SendBadWord;
                                emb.WithDescription($"Плохие слова в {channel.Mention} теперь {(chnl.SendBadWord ? "удаляются" : "не удаляются")}");

                                emb.WithFooter($"Добавить/удалить плохие слова - {Prefix}cs.bw [channel] [word]");
                                break;
                            case 5:
                                chnl.UseCommand = !chnl.UseCommand;
                                emb.WithDescription($"Команды в {channel.Mention} теперь {(chnl.UseCommand ? "включены" : "выключены")}");
                                if (!chnl.UseCommand)
                                {
                                    emb.WithFooter("Вы можете настроить RP и Admin команды! Откройте команду еще раз!");
                                    chnl.UseRPcommand = false;
                                    chnl.UseAdminCommand = false;
                                }
                                else
                                {
                                    chnl.UseAdminCommand = true;
                                    chnl.UseRPcommand = true;
                                }
                                break;
                            case 5.1f:
                                chnl.UseRPcommand = !chnl.UseRPcommand;
                                emb.WithDescription($"RP-команды в {channel.Mention} теперь {(chnl.UseRPcommand ? "включены" : "выключены")}");
                                break;
                            case 5.2f:
                                chnl.UseAdminCommand = !chnl.UseAdminCommand;
                                emb.WithDescription($"Admin-команды в {channel.Mention} теперь {(chnl.UseAdminCommand ? "включены" : "выключены")}");

                                break;
                            case 6:
                                chnl.Spaming = !chnl.Spaming;
                                emb.WithDescription($"Проверка на спам в {channel.Mention} {(chnl.Spaming ? "включена" : "выключена")}");
                                emb.WithFooter("Спам Больше 4 похожих сообщений в диапазоне 5 секунд");
                                break;
                            case 7:
                                chnl.InviteMessage = !chnl.InviteMessage;
                                emb.WithDescription($"Приглашения на другие сервера в {channel.Mention} теперь {(chnl.InviteMessage == true ? "удаляются" : "не удаляются")}");
                                break;
                            default:
                                emb.WithDescription($"Команда с таким номером не найдена!");
                                break;
                        }
                        _db.Channels.Update(chnl);
                        await _db.SaveChangesAsync();
                    }
                    else emb.WithDescription("Номер может быть от 1 до 7.").WithFooter($"Подробнее - {Prefix}cs [channel]");
                }
                else emb.WithDescription($"Введите нужный вам канал, пример - {Prefix}cs [channel]");

                await Context.Channel.SendMessageAsync("", false, emb.Build());
            }
        }

        [Aliases, Commands, Usage, Descriptions]
        [PermissionBlockCommand]
        public async Task levelroleadd(SocketRole role, uint level)
        {
            using (db _db = new ())
            {
                var emb = new EmbedBuilder().WithColor(255, 0, 94).WithAuthor("🔨 Добавить уровневую роль");
                var lvlrole = _db.Roles.FirstOrDefault(x => x.RoleId == role.Id && x.Type == Enums.RoleTypeEnum.Level);
                if (lvlrole == null)
                {
                    emb.Description = role.RolePermission();
                    if (emb.Description == null)
                    {
                        var Prefix = await _db.Guilds.GetPrefix(Context.Guild.Id);
                        emb.WithDescription($"Роль {role.Mention} выставлена за {level} уровень").WithFooter($"Посмотреть ваши уровневые роли {Prefix}lr");

                        _db.Roles.Add(new Roles() { RoleId = role.Id, Value = level.ToString(), Type = Enums.RoleTypeEnum.Level });
                        await _db.SaveChangesAsync();
                    }
                }
                else emb.WithDescription($"Роль {role.Mention} уже выдается за {lvlrole.Value} уровень");


                await Context.Channel.SendMessageAsync("", false, emb.Build());
            }
        }

        [Aliases, Commands, Usage, Descriptions]
        [PermissionBlockCommand]
        public async Task levelroledel(SocketRole role)
        {
            using (db _db = new ())
            {
                var emb = new EmbedBuilder().WithColor(255, 0, 94).WithAuthor("🔨 Удалить уровневую роль");
                var lvlrole = _db.Roles.FirstOrDefault(x => x.RoleId == role.Id && x.Type == Enums.RoleTypeEnum.Level);
                emb.WithDescription($"Уровневая роль {role.Mention} {(lvlrole != null ? "удалена" : "не является уровневой")}.");
                if (lvlrole != null)
                {
                    _db.Roles.Remove(lvlrole);
                    await _db.SaveChangesAsync();
                }
                await Context.Channel.SendMessageAsync("", false, emb.Build());
            }
        }

        [Aliases, Commands, Usage, Descriptions]
        [PermissionBlockCommand]
        public async Task violationsystem(ViolationSystemEnum VS = ViolationSystemEnum.none)
        {
            using (db _db = new ())
            {
                var Guild = await _db.Guilds.GetOrCreate(Context.Guild.Id);
                var embed = new EmbedBuilder().WithColor(255, 0, 94).WithAuthor("Выбор системы наказания")
                                              .WithFooter($"Выбор системы - **{Guild.Prefix}VS [WarnSystem/off/null]**\n" +
                                                          $"💤**OFF** - отключить систему");

                if (VS == ViolationSystemEnum.none)
                {
                    switch (Guild.VS)
                    {
                        case ViolationSystemEnum.ReportSystem:
                            embed.WithAuthor("ViolationSystem - ReportSystem ✅")
                                 .WithDescription($"Для настройки системы используйте:\n\n" +
                                                  $"**Добавить правило** - {Guild.Prefix}reportrules.add [текст правила]\n" +
                                                  $"**Удалить правило** - {Guild.Prefix}reportrules.del [номер правила]\n" +
                                                  $"**Добавить нарушение для правила** - {Guild.Prefix}reportpunishes.add [номер правила] [TimeBan,Mute,TimeOut,Kick,Ban] [Время(по возможности)]\n" +
                                                  $"**Удалить нарушение для правила** - {Guild.Prefix}reportpunishes.del [номер правила] [номер нарушения]\n\n" +
                                                  $"**Открыть правила** - {Guild.Prefix}rules\n\n" +
                                                  $"**Выдать одно нарушение** - {Guild.Prefix}report [user] [номер правила]\n" +
                                                  $"**Убрать одно нарушение** - {Guild.Prefix}unreport [user] [номер правила]\n" +
                                                  $"Пример времени - (01:00:0)[ч:м:с] или (01:00:00:00)[д:ч:м:с]");
                            break;
                        case ViolationSystemEnum.WarnSystem:
                            embed.WithAuthor("ViolationSystem - WarnSystem ✅")
                                 .WithDescription($"Для настройки системы используйте:\n\n" +
                                                  $"**Выставить варны** - {Guild.Prefix}addwarn [Номер варна] [TimeBan,Mute,TimeOut,Kick,Ban] [Время(по возможности)]\n" +
                                                  $"**Удалить варн** - {Guild.Prefix}delwarn [номер]\n" +
                                                  $"**Выдать варн** - {Guild.Prefix}warn [USER]\n" +
                                                  $"**Снять варн с пользователя** - {Guild.Prefix}unwarn [USER] [Колво-варнов]\n" +
                                                  $"**Посмотреть варны** - {Guild.Prefix}warns\n" +
                                                  $"Пример времени - (01:00:0)[ч: м:с] или(01:00:00:00)[д: ч:м: с]\n\n" +
                                                  $"**Более подробно тут - [Инструкция](https://docs.darlingbot.ru/commands/settings-server/vybor-sistemy-narushenii)**");
                            break;
                        case ViolationSystemEnum.off:
                        case ViolationSystemEnum.none:
                            embed.WithAuthor("ViolationSystem - ⚠️ У вас не настроена репорт система!")
                                 .WithDescription($"Для настройки выберите тип системы:\n" +
                                                  $"📕**WarnSystem** - выдача наказаний по количеству выданных предупреждений.\n" +
                                                  $"Не нуждается в точной настройке, варн дается за любое нарушенное правило\n\n" +
                                                  $"📒**ReportSystem** - более гипкая система, выдает определенное нарушение\n" +
                                                  "по определенному правилу, что делает ее намного лучше, Warn системы\n" +
                                                  "которая может выдать бан, за простую шалость, которая вовсе не походит на бан.");
                            break;
                    }
                }
                else
                {
                    if (Guild.VS == VS)
                        if (VS == ViolationSystemEnum.off)
                            embed.WithDescription($"Система уже отключена!");
                        else
                            embed.WithDescription($"Система **{VS}** уже выбрана!");
                    else
                    {
                        Guild.VS = VS;
                        _db.Guilds.Update(Guild);
                        await _db.SaveChangesAsync();
                        if (VS == ViolationSystemEnum.off)
                            embed.WithDescription($"Система отключена!");
                        else
                            embed.WithDescription($"Система **{VS}** выбрана!");
                    }
                }

                await Context.Channel.SendMessageAsync("", false, embed.Build());
            }
        }

        [Aliases, Commands, Usage, Descriptions]
        public async Task commandinvise(string commandname = null)
        {
            using (db _db = new ())
            {
                var Guild = await _db.Guilds.GetOrCreate(Context.Guild.Id);
                var emb = new EmbedBuilder().WithColor(255, 0, 94).WithAuthor("🔨 Отключенные команды");

                if (commandname == null)
                {
                    var Lists = _commands.Commands.Where(x => Guild.CommandInviseList.Any(z => z == x.Aliases[0])).OrderBy(x => x.Module.Name);
                    string modules = "";
                    int Count = 0;
                    foreach (var Command in Lists)
                    {
                        if (modules != Command.Module.Name || modules == "")
                        {
                            Count = 0;
                            modules = Command.Module.Name;
                            emb.Description += $"\nМодуль - {modules}";
                        }
                        if (Count == 0)
                            emb.Description += $"\n⠀⠀⠀";

                        if (Lists.Count(x => x.Module.Name == Command.Module.Name) > 5)
                        {
                            emb.Description += $"**{Command.Aliases[0]}**,";
                            Count++;
                        }
                        else
                        {
                            emb.Description += $"**{Command.Aliases[0]}** - ";
                            if (Command.Summary.Length > 25)
                                emb.Description += $"{Command.Summary.Substring(0, 25)}...";
                            else
                                emb.Description += Command.Summary;
                        }
                    }

                    if (emb.Description == null)
                        emb.WithDescription("Команды еще не добавлены.").WithFooter("Выключить ");
                    else
                        emb.WithFooter("Включить/выключить ");
                    emb.Footer.Text += $"команду - {Guild.Prefix}CommandInvise [commandname]";
                }
                else
                {
                    commandname = commandname.ToLower();

                    var CommandName = _commands.Commands.FirstOrDefault(x => x.Aliases[0] == commandname || x.Aliases.LastOrDefault() == commandname);
                    //var Dostup = await CommandName.CheckPreconditionsAsync(Context);
                    if (CommandName == null/* || !Dostup.IsSuccess*/ || BotSettings.OwnerCommand.Any(x => x == CommandName.Aliases[0]))
                        emb.WithDescription("Такая команда не существует");
                    else if (BotSettings.CommandNotInvise.Any(x => x == CommandName.Aliases[0]))
                        emb.WithDescription("Эту команду нельзя отключить!");
                    else
                    {
                        List<string> liststring = Guild.CommandInviseList;
                        var Command = Guild.CommandInviseList.FirstOrDefault(x => x == CommandName.Aliases[0]);

                        emb.WithDescription($"Команда `{commandname}` {(Command == null ? "отключена" : "включена")}")
                            .WithFooter($"{(Command == null ? "Включить" : "Отключить")} команду - {Guild.Prefix}ci [commandname]");

                        if (Command == null)
                            liststring.Add(CommandName.Aliases[0]);
                        else
                            liststring.Remove(CommandName.Aliases[0]);

                        Guild.CommandInviseList = liststring;
                        _db.Guilds.Update(Guild);
                        await _db.SaveChangesAsync();
                    }
                }
                await Context.Channel.SendMessageAsync("", false, emb.Build());
            }
        }

        [Aliases, Commands, Usage, Descriptions]
        [PermissionBlockCommand]
        public async Task channelsettingsbadword(SocketTextChannel channel, string word)
        {
            using (db _db = new ())
            {
                var Prefix = await _db.Guilds.GetPrefix(Context.Guild.Id);
                var emb = new EmbedBuilder().WithColor(255, 0, 94).WithAuthor("🔨Настройки канало - добавление плохих слов")
                                            .WithFooter($"Добавить/Удалить - {Prefix}cs.bw {channel.Name} [слово]");
                var chnl = await _db.Channels.GetOrCreate(channel);

                if (chnl.SendBadWord)
                {
                    List<string> badlist = chnl.BadWordList;
                    if (chnl.BadWordList.FirstOrDefault(x => x == word) != null)
                    {
                        emb.WithDescription($"Слово {word} удалено из списока");
                        badlist.Remove(word);
                    }
                    else
                    {
                        emb.WithDescription($"Слово {word} включено в список");
                        badlist.Add(word);
                    }
                    chnl.BadWordList = badlist;
                    _db.Channels.Update(chnl);
                    await _db.SaveChangesAsync();
                }
                else
                    emb.WithDescription($"Вы не включили проверку Плохих слов\n{Prefix}cs {channel.Name} [4]");
                await Context.Channel.SendMessageAsync("", false, emb.Build());
            }
        }

        [Aliases, Commands, Usage, Descriptions]
        [PermissionBlockCommand]
        public async Task channelsettingsurlwhitelist(SocketTextChannel channel, string url)
        {
            using (db _db = new ())
            {
                var Prefix = await _db.Guilds.GetPrefix(Context.Guild.Id);
                var emb = new EmbedBuilder().WithColor(255, 0, 94).WithAuthor("🔨 Настройка каналов - белый список ссылок")
                                            .WithFooter($"Добавить/Удалить - {Prefix}cs.uwl #{channel.Name} [url]");
                var TextChannel = await _db.Channels.GetOrCreate(channel);

                if (TextChannel.DelUrl)
                {
                    List<string> UrlWhiteList = TextChannel.csUrlWhiteListList;
                    if (TextChannel.csUrlWhiteListList.FirstOrDefault(x => x == url) != null)
                    {
                        emb.WithDescription($"Url {url} удалено из White list");
                        UrlWhiteList.Remove(url);
                    }
                    else
                    {
                        emb.WithDescription($"Url {url} включено в White list");
                        UrlWhiteList.Add(url);
                    }

                    TextChannel.csUrlWhiteListList = UrlWhiteList;
                    _db.Channels.Update(TextChannel);
                    await _db.SaveChangesAsync();
                }
                else emb.WithDescription($"Вы не включили Белый лист ссылок!\n{Prefix}cs {channel.Mention} [2]");
                await Context.Channel.SendMessageAsync("", false, emb.Build());
            }
        }

        [Aliases, Commands, Usage, Descriptions]
        [PermissionBlockCommand]
        public async Task messagesettings(byte selection = 0, [Remainder] string text = null)
        {
            using (db _db = new ())
            {
                var Prefix = await _db.Guilds.GetPrefix(Context.Guild.Id);
                var GuildLeave = _db.Guilds_Meeting_Leave.Include(x => x.LeaveChannel).FirstOrDefault(x => x.LeaveChannel.GuildsId == Context.Guild.Id);
                var GuildWelcome = _db.Guilds_Meeting_Welcome.Include(x => x.WelcomeChannel).FirstOrDefault(x => x.WelcomeChannel.GuildsId == Context.Guild.Id);
                if (GuildLeave == null)
                    GuildLeave = new Guilds_Meeting_Leave { GuildsId = Context.Guild.Id };
                if (GuildWelcome == null)
                    GuildWelcome = new Guilds_Meeting_Welcome { GuildsId = Context.Guild.Id };

                var emb = new EmbedBuilder().WithColor(255, 0, 94).WithAuthor("🛎 Настройка оповещения");
                byte point = 1;
                if (selection == 0 && string.IsNullOrWhiteSpace(text))
                {
                    var WelcomeChannel = Context.Guild.GetTextChannel(GuildWelcome.WelcomeChannelId);
                    emb.AddField($"{point}.Канал для Сообщений при входе [channel]", WelcomeChannel != null ? WelcomeChannel.Mention : "Отсутствует", true); // 1 
                    point++;
                    if (WelcomeChannel != null)
                    {
                        emb.AddField($"{point}.Сообщение при входе [json]", GuildWelcome.WelcomeMessage != null ? $"Установлено" : "Отсутствует", true); // 2
                        point++;
                        emb.AddField($"{point}.Личное сообщение при входе [json]", GuildWelcome.WelcomeDMmessage != null ? $"Установлено" : "Отсутствует", true); // 3
                        point++;
                        var WelcomeRole = Context.Guild.GetRole(Convert.ToUInt64(GuildWelcome.WelcomeRoleId));
                        emb.AddField($"{point}.Роль при входе [role]", WelcomeRole != null ? WelcomeRole.Mention : "Отсутствует", true); // 4
                        point++;
                    }

                    var LeaveChannel = Context.Guild.GetTextChannel(GuildLeave.LeaveChannelId);
                    emb.AddField($"{point}.Канал для Сообщений при выходе [channel]", LeaveChannel != null ? LeaveChannel.Mention : "Отсутствует", true); // 5 | 2 
                    point++;
                    if (LeaveChannel != null)
                    {
                        emb.AddField($"{point}.Сообщение при выходе [json]", GuildLeave.LeaveMessage != null ? $"Установлено" : "Отсутствует", true); //6 | 3
                        point++;
                    }
                    emb.WithFooter($"Вкл - {Prefix}ms [цифра] [channel/json/role]\nВыкл - {Prefix}ms [цифра]\n%user% - чтобы упомянуть пользователя");
                }
                else
                {
                    point = 2;
                    if (GuildWelcome.WelcomeChannel != null) point += 3;
                    if (GuildLeave.LeaveChannel != null) point++;
                    if (!(selection >= 1 && selection <= point))
                        emb.WithDescription($"Выбор может быть только от 1 до {point}").WithFooter($"Подробнее - {Prefix}ms");
                    else
                    {
                        if (selection == 1 || selection == 5 && GuildWelcome.WelcomeChannel != null || selection == 2 && GuildWelcome.WelcomeChannel == null)
                        {
                            var Channel = Context.Message.MentionedChannels.FirstOrDefault();
                            if (Channel != null)
                            {
                                emb.WithDescription($"В канал <#{Channel.Id}> буду приходить сообщения о {(selection == 5 || selection == 2 ? "выходе" : "входе")} пользователей.");
                                var CreatedChannel = await _db.Channels.GetOrCreate(Channel.Guild.Id, Channel.Id);
                                if (selection == 1)
                                    GuildWelcome.WelcomeChannelId = CreatedChannel.Id;
                                else
                                    GuildLeave.LeaveChannelId = CreatedChannel.Id;
                            }
                            else
                                emb.WithDescription($"Введенный канал не найден.");
                        }
                        else if ((selection == 2 || selection == 3) && GuildWelcome.WelcomeChannel != null ||
                            selection == 3 && GuildLeave.LeaveChannel == null || selection == 6 && GuildLeave.LeaveChannel != null)
                        {
                            if (text != null)
                            {
                                var embed = JsonToEmbed.JsonCheck(text);
                                if (embed.Item1 == null)
                                    emb.WithDescription($"Сообщение составлено не верно.").WithFooter("Создать сообщение - embed.discord-bot.net");
                                else
                                {
                                    emb.WithDescription($"EmbedVisualizer cообщение будет отправляться ");
                                    if (selection == 3)
                                    {
                                        emb.Description += "пользователю при входе на сервер.";
                                        emb.WithFooter("Если он не отключил возможность отправлять ему сообщения");
                                        GuildWelcome.WelcomeDMmessage = text;
                                    }
                                    else if (selection == 2)
                                    {
                                        emb.Description += $"в канал <#{GuildWelcome.WelcomeChannelId}> при входе на сервер.";
                                        GuildWelcome.WelcomeMessage = text;
                                    }
                                    else
                                    {
                                        emb.Description += $"в канал <#{GuildLeave.LeaveChannelId}> при выходе с сервера.";
                                        GuildLeave.LeaveMessage = text;
                                    }
                                }
                            }
                            else
                            {
                                if (selection == 3)
                                {
                                    if (GuildWelcome.WelcomeDMmessage == null)
                                    {
                                        emb.WithDescription($"Введите текст для включения Личного сообщения при входе").WithFooter($"Подробнее - {Prefix}ms");
                                    }
                                    else
                                    {
                                        emb.WithDescription($"Личное сообщение при входе выключено");
                                        GuildWelcome.WelcomeDMmessage = null;
                                    }
                                }
                                else if (selection == 2)
                                {
                                    if (GuildWelcome.WelcomeMessage == null)
                                        emb.WithDescription($"Введите текст для включения Сообщения при входе").WithFooter($"Подробнее - {Prefix}ms");
                                    else
                                    {
                                        emb.WithDescription($"Сообщение при входе выключено");
                                        GuildWelcome.WelcomeMessage = null;
                                    }
                                }
                                else
                                {
                                    if (GuildLeave.LeaveMessage == null)
                                        emb.WithDescription($"Введите текст для включения Сообщения при выходе").WithFooter($"Подробнее - {Prefix}ms");
                                    else
                                    {
                                        emb.WithDescription($"Сообщение при выходе выключено");
                                        GuildLeave.LeaveMessage = null;
                                    }
                                }

                            }
                        }
                        else if (selection == 4)
                        {
                            if (text != null)
                            {
                                var Role = Context.Message.MentionedRoles.FirstOrDefault();
                                if (Role != null)
                                {
                                    emb.WithDescription($"Новый пользователям будет выдаваться роль {Role.Mention}.");
                                    await _db.Role.GetOrCreate(Role);
                                    GuildWelcome.WelcomeRoleId = Role.Id;
                                }
                                else
                                    emb.WithDescription($"Введенная роль не найдена. Укажите роль в таком формате: {Context.Guild.EveryoneRole.Mention}").WithFooter("Не бойтесь, ни один ваш любимчик не был потревожен.");
                            }
                            else
                            {
                                if (GuildWelcome.WelcomeRoleId == 0)
                                    emb.WithDescription($"Введите роль для выдачи ее пользователям").WithFooter($"Подробнее - {Prefix}ms");
                                else
                                {
                                    emb.WithDescription($"Роль <@{GuildWelcome.WelcomeRole}> не будет выдаваться пользователям");
                                    GuildWelcome.WelcomeRoleId = 0;
                                }
                            }
                        }

                        if (GuildLeave.LeaveChannelId != 0)
                        {
                            if (GuildLeave.Id == 0)
                                _db.Guilds_Meeting_Leave.Add(GuildLeave);
                            else
                                _db.Guilds_Meeting_Leave.Update(GuildLeave);
                        }

                        if (GuildWelcome.WelcomeChannelId != 0)
                        {
                            if (GuildWelcome.Id == 0)
                                _db.Guilds_Meeting_Welcome.Add(GuildWelcome);
                            else
                                _db.Guilds_Meeting_Welcome.Update(GuildWelcome);
                        }

                        await _db.SaveChangesAsync();
                    }
                }
                await Context.Channel.SendMessageAsync("", false, emb.Build());
            }
        }

        [Aliases, Commands, Usage, Descriptions]
        [PermissionBlockCommand]
        [RequireBotPermission(GuildPermission.ManageChannels)]
        public async Task privatechannelcreate()
        {
            using (db _db = new ())
            {
                var Guild = await _db.Guilds.GetOrCreate(Context.Guild.Id);
                var embed = new EmbedBuilder().WithColor(255, 0, 94).WithAuthor("🔨 Создание канала для приваток");
                var PrivateVoiceChannel = Context.Guild.GetVoiceChannel(Guild.PrivateId);

                if (PrivateVoiceChannel == null)
                {
                    embed.WithDescription($"Канал для создания приваток {(Guild.PrivateId == 0 ? "создан" : "пересоздан")}!");
                    var category = await Context.Guild.CreateCategoryChannelAsync("DARLING PRIVATE");
                    var pr = await Context.Guild.CreateVoiceChannelAsync("СОЗДАТЬ 🎉", x => x.CategoryId = category.Id);
                    Guild.PrivateId = pr.Id;
                    _db.Guilds.Update(Guild);
                    await _db.SaveChangesAsync();
                }
                else
                    embed.WithDescription($"У вас уже создан канал для приваток с именем {PrivateVoiceChannel.Name}");
                await Context.Channel.SendMessageAsync("", false, embed.Build());
            }
        }

        [Aliases, Commands, Usage, Descriptions]
        [PermissionBlockCommand]
        public async Task buyroleadd(SocketRole role, ulong price)
        {
            await RoleAdd(role, price, true, new TimeSpan());
        }

        [Aliases, Commands, Usage, Descriptions]
        [PermissionBlockCommand]
        public async Task buyroledel(SocketRole role)
        {
            await RoleDel(role, true);
        }

        [Aliases, Commands, Usage, Descriptions]
        [PermissionBlockCommand]
        public async Task timeroleadd(SocketRole role, ulong price, string time)
        {
            var emb = new EmbedBuilder().WithColor(255, 0, 94).WithAuthor($"🔨 Аренда ролей - добавление");
            bool Success = TimeSpan.TryParse(time, out TimeSpan result);
            if (!Success)
                emb.WithDescription("Время введено неправильно,возможно вы ввели большое число?\nПример: 01:00:00 [ч:м:с]\nПример 2: 07:00:00:00 [д:ч:м:с]");
            else if (result.TotalSeconds > 604800)
                emb.WithDescription("Время временной роли не может превышать 7 дней!");
            else
                await RoleAdd(role, price, false, result);
        }

        [Aliases, Commands, Usage, Descriptions]
        [PermissionBlockCommand]
        public async Task timeroledel(SocketRole role)
        {
            await RoleDel(role, false);
        }

        private async Task RoleAdd(SocketRole role, ulong price, bool BuyOrTime, TimeSpan time)
        {
            using (db _db = new ())
            {
                var emb = new EmbedBuilder().WithColor(255, 0, 94).WithAuthor($"🔨 {(BuyOrTime ? "Продажа" : "Аренда")} ролей - добавление");
                var BuyRoles = _db.Roles.Include(x => x.Role).Where(x => x.Role.GuildsId == Context.Guild.Id && x.Type == (BuyOrTime ? Enums.RoleTypeEnum.Buy : Enums.RoleTypeEnum.Time));
                if (BuyRoles.Count() < 10)
                {
                    var BuyRole = BuyRoles.FirstOrDefault(x => x.Id == role.Id);
                    if (BuyRole == null)
                    {
                        emb.Description = role.RolePermission();
                        if (emb.Description == null)
                        {
                            emb.WithDescription($"Роль {role.Mention} успешно выставлена на {(BuyOrTime ? "продажу" : "аренду")} за {price} ZeroCoin's!");
                            await _db.Role.GetOrCreate(role);
                            var QueryDB = new Roles() { Value = price.ToString() + (BuyOrTime ? "" : $":{time}"), RoleId = role.Id, Type = (BuyOrTime ? Enums.RoleTypeEnum.Buy : Enums.RoleTypeEnum.Time) };
                            _db.Roles.Add(QueryDB);
                            await _db.SaveChangesAsync();
                        }

                    }
                    else emb.WithDescription($"Роль {role.Mention} уже выставлена {(BuyOrTime ? "на продажу" : "в аренду")} за {BuyRole.Value} ZeroCoins!");
                }
                else emb.WithDescription("Выставить больше 10 ролей в магазин нельзя!");

                await Context.Channel.SendMessageAsync("", false, emb.Build());
            }
        }
        private async Task RoleDel(SocketRole role, bool BuyOrTime)
        {
            using (db _db = new ())
            {
                var emb = new EmbedBuilder().WithColor(255, 0, 94).WithAuthor($"🔨 {(BuyOrTime ? "Продажа" : "Аренда")} ролей - удаление");
                var buyrole = _db.Roles.FirstOrDefault(x => x.RoleId == role.Id && x.Type == (BuyOrTime ? Enums.RoleTypeEnum.Buy : Enums.RoleTypeEnum.Time));
                emb.WithDescription($"Роль {role.Mention} {(buyrole != null ? $"удалена с {(BuyOrTime ? "продажи" : "аренды")}" : $"не выставлялась на {(BuyOrTime ? "продажу" : "аренду")}")}.");
                if (buyrole != null)
                {
                    _db.Roles.Remove(buyrole);
                    await _db.SaveChangesAsync();
                }
                await Context.Channel.SendMessageAsync("", false, emb.Build());
            }
        }

        [Aliases, Commands, Usage, Descriptions]
        [PermissionBlockCommand]
        public async Task VoicePoint(ulong Id = 0)
        {
            using (db _db = new ())
            {
                var emb = new EmbedBuilder().WithColor(255, 0, 94).WithAuthor("🔊 Категории/каналы, в которых выдается опыт");
                var Guild = await _db.Guilds.GetOrCreate(Context.Guild.Id);
                string FooterText = $"\nДобавить/Удалить - {Guild.Prefix}voicepoint [Id канала/категории]";
                if (Id == 0)
                {
                    foreach (var Channel in Guild.VoiceAndCategoryChannelList)
                    {
                        var chnl = Context.Guild.GetChannel(Channel);
                        string what = string.Empty;

                        if (chnl as SocketVoiceChannel == null)
                            what = "категория";
                        else
                            what = "голосовой чат";

                        emb.Description += $"\n[**{chnl.Name}**] - {what}";
                    }
                    if (Guild.VoiceAndCategoryChannelList.Count == 0)
                        emb.Description += " **Отсутствуют!**";

                    emb.WithFooter($"Опыт не выдается в Афк-канале или при выключенном микрофоне!{FooterText}");
                }
                else
                {
                    emb.WithAuthor("🔊 Опыт в голосовых каналах");
                    var Channel = Context.Guild.GetChannel(Id);
                    if (Channel as SocketCategoryChannel == null && Channel as SocketVoiceChannel == null)
                        emb.WithDescription("В список можно добавить только ID голосового чата или категории.\n\nДля получения ID включите режим [разработчика](https://clck.ru/P475L)");
                    else
                    {
                        var List = Guild.VoiceAndCategoryChannelList;
                        if (!List.Any(x => x == Id))
                        {
                            List.Add(Id);
                            emb.WithDescription("ID успешно добавлено в список, для получения опыта в нем.");
                            if (Channel as SocketCategoryChannel != null)
                            {
                                foreach (var VoiceChannel in (Channel as SocketCategoryChannel).Channels.Where(x => x as SocketVoiceChannel != null))
                                {
                                    foreach (var User in VoiceChannel.Users)
                                    {
                                        await TaskTimer.StartVoiceActivity(User);
                                    }
                                }
                            }
                            else
                            {
                                foreach (var User in (Channel as SocketVoiceChannel).Users)
                                {
                                    await TaskTimer.StartVoiceActivity(User);
                                }
                            }
                        }
                        else
                        {
                            List.Remove(Id);
                            emb.WithDescription("ID успешно удалено из списка.");
                        }
                        Guild.VoiceAndCategoryChannelList = List;
                        _db.Guilds.Update(Guild);
                        await _db.SaveChangesAsync();
                    }
                    emb.WithFooter(FooterText);
                }
                await Context.Channel.SendMessageAsync("", false, emb.Build());
            }
        }

        [Aliases, Commands, Usage, Descriptions]
        [PermissionBlockCommand]
        public async Task raidsettings(uint select = 0, byte point = 0)
        {
            using (db _db = new ())
            {
                var emb = new EmbedBuilder().WithAuthor("🔨RaidSettings").WithColor(255, 0, 94);

                var EmbedBoost = Context.Guild.DarlingBoostGet();
                if (EmbedBoost != null)
                    emb = EmbedBoost;
                else
                {
                    var Guild = _db.Guilds.Include(x => x.Guilds_Raid).FirstOrDefault(x => x.Id == Context.Guild.Id);
                    Guilds_Raid Raid = Guild.Guilds_Raid;
                    if (Raid == null)
                        Raid = new Guilds_Raid { GuildsId = Context.Guild.Id };

                    string raidText = "Anti-Raid [инструкция](https://www.docs.darlingbot.ru/commands/settings-server/bezopasnost/anti-raid-sistema)";
                    if (select == 0 && point == 0)
                    {
                        emb.WithDescription(raidText);
                        emb.AddField("1.Анти-Raid система", $"{(Raid.RaidRunning ? "Вкл" : "Выкл")}", true);
                        if (Raid.RaidRunning)
                        {
                            emb.AddField("2.Время срабатывания", $"{Raid.RaidTime}", true);
                            emb.AddField("3.Кол-во пользователей", $"{Raid.RaidUserCount}", true);
                            emb.WithFooter($"Настроить - {Guild.Prefix}rds [номер функции] [значение]");
                        }
                        else emb.WithFooter($"Включить систему - {Guild.Prefix}rds 1");
                    }
                    else
                    {
                        if (select == 1)
                        {
                            if (!Raid.RaidRunning)
                            {
                                emb.Description += "\nСамые оптимальные настройки уже были выставлены!";
                                emb.WithFooter("Напишите команду без параметров если хотите настроить систему.");
                                Raid.RaidUserCount = 5;
                                Raid.RaidTime = 3;
                                Raid.RaidRunning = true;
                            }
                            emb.WithDescription($"Anti-Raid система {(Raid.RaidRunning ? "включена" : "выключена")}");
                        }
                        else if ((select == 2 || select == 3) && Raid.RaidRunning)
                        {
                            string Text = "Если в течении {0} секунд, зайдет {1} человек, их и последующих зашедших пользователей в течении 30 секунд, замутит.\nВнимание, бот выдает Тайм-Аут на 5 минут, далее вы можете самостоятельно их проверить.";
                            if (select == 2)
                            {
                                if (point < 1 && point > 20)
                                    emb.WithDescription("Ради безопасности ваших пользователей, нельзя выставить время меньше 1 и больше 20 секунд.");
                                else
                                {
                                    emb.WithDescription(string.Format(Text, point, Raid.RaidUserCount));
                                    Raid.RaidTime = point;
                                }
                            }
                            else
                            {
                                if (point < 2 && point > 100)
                                    emb.WithDescription("Ради безопасности ваших пользователей, нельзя выставить кол-во пользователей меньше 1 и больше 100.");
                                else
                                {
                                    emb.WithDescription(string.Format(Text, Raid.RaidTime, point));
                                    Raid.RaidUserCount = point;
                                }

                                
                            }
                        }
                        else
                        {
                            if (Raid.RaidRunning)
                                emb.WithDescription($"Первый параметр не может быть больше 3.\n{raidText}");
                            else
                                emb.WithDescription($"Для того чтобы пользовать Raid системой, нужно ее включить\n{raidText}").WithFooter($"Включить - {Guild.Prefix}rds 1");
                        }
                    }
                    if (Raid.Id != 0)
                        _db.Guilds_Raid.Update(Raid);
                    else
                        _db.Guilds_Raid.Add(Raid);
                    await _db.SaveChangesAsync();
                }
                await Context.Channel.SendMessageAsync("", false, emb.Build());
            }
        }

        [Aliases, Commands, Usage, Descriptions]
        [PermissionBlockCommand]
        public async Task Captcha(uint select = 0)
        {
            using (db _db = new ())
            {
                var emb = new EmbedBuilder().WithAuthor("🔨CaptchaSettings").WithColor(255, 0, 94);
                var EmbedBoost = Context.Guild.DarlingBoostGet();
                if (EmbedBoost != null)
                    emb = EmbedBoost;
                else
                {
                    var ThisCaptcha = _db.Guilds_Captcha.Include(x => x.Channel).FirstOrDefault(x => x.Channel.GuildsId == Context.Guild.Id);
                    var Prefix = await _db.Guilds.GetPrefix(Context.Guild.Id);
                    string error = string.Empty;
                    if (ThisCaptcha == null)
                        ThisCaptcha = new Guilds_Captcha();

                    if (select == 0)
                    {
                        emb.AddField("1.Captcha система", $"{(ThisCaptcha.Run ? "Вкл" : "Выкл")}", true);
                        if (ThisCaptcha.Run)
                        {
                            ulong RoleId = Convert.ToUInt64(ThisCaptcha.RoleId);
                            ulong ChannelId = Convert.ToUInt64(ThisCaptcha.ChannelId);
                            var Role = Context.Guild.GetRole(RoleId);
                            var Channel = Context.Guild.GetTextChannel(ChannelId);
                            emb.AddField("2.Пересоздать", $"Если вы что то удалили, или что то работает не так!", true);
                            emb.AddField("Роль", $"{(Role == null ? "Отсутствует" : $"<@&{Role.Id}>")}", true);
                            emb.AddField("Канал для капчи", $"{(Channel == null ? "Отсутствует" : $"<#{Channel.Id}>")}", true);
                            emb.WithFooter($"Использовать - {Prefix}captcha [номер]");
                        }
                        else
                            emb.WithFooter($"Включить - {Prefix}captcha 1");
                    }
                    else
                    {
                        int maxValue = 3;
                        if (ThisCaptcha.Id == 0)
                            maxValue = 1;

                        if (select >= 1 && select <= maxValue)
                        {
                            switch (select)
                            {
                                case 1:
                                    ThisCaptcha.Run = !ThisCaptcha.Run;

                                    if (ThisCaptcha.Run)
                                    {
                                        var Permission = Context.Guild.CurrentUser.GuildPermissions;

                                        if (!Permission.ManageChannels)
                                            error = "каналов";
                                        else if (!Permission.ManageRoles)
                                            error = "ролей";
                                        else
                                        {
                                            SocketTextChannel Channel = Context.Guild.GetTextChannel(Convert.ToUInt64(ThisCaptcha.ChannelId));

                                            if (Channel == null)
                                            {
                                                Channel = await CapthaService.CreateChannel(Context.Guild);
                                                ThisCaptcha.ChannelId = Channel.Id;
                                            }

                                            SocketRole Role = Context.Guild.GetRole(Convert.ToUInt64(ThisCaptcha.RoleId));
                                            if (Role == null)
                                            {
                                                Role = await CapthaService.CreateRole(Context.Guild);
                                                ThisCaptcha.RoleId = Role.Id;
                                            }
                                            if (ThisCaptcha.Id == 0)
                                                ThisCaptcha.GuildId = Context.Guild.Id;

                                            await CapthaService.PermissionUpdate(Channel, Role);

                                            emb.WithDescription($"Система captcha успешно запущена.\nКанал для капчи: {Channel.Mention}\nРоль: {Role.Mention}").WithFooter($"Доп.настройки - {Prefix}captcha");
                                        }

                                        if (error != string.Empty)
                                            emb.WithDescription($"У бота отсутствуют права на создание {error}.\nВыдайте права Администратора боту, и повторите попытку.");

                                    }
                                    else
                                        emb.WithDescription("Система captcha успешно выключена.");
                                    break;
                                case 2:
                                    {
                                        await Context.Guild.GetRole(Convert.ToUInt64(ThisCaptcha.RoleId))?.DeleteAsync();
                                        await Context.Guild.GetTextChannel(Convert.ToUInt64(ThisCaptcha.ChannelId))?.DeleteAsync();

                                        var Role = await CapthaService.CreateRole(Context.Guild);
                                        ThisCaptcha.RoleId = Role.Id;
                                        var Channel = await CapthaService.CreateChannel(Context.Guild);
                                        ThisCaptcha.ChannelId = Channel.Id;
                                        await CapthaService.PermissionUpdate(Channel, Role);
                                        emb.WithDescription("Данные успешно пересозданы!");
                                    }
                                    break;
                            }
                            if (error == string.Empty)
                            {
                                if (ThisCaptcha.Id == 0)
                                    _db.Guilds_Captcha.Add(ThisCaptcha);
                                else
                                    _db.Guilds_Captcha.Update(ThisCaptcha);

                                await _db.SaveChangesAsync();
                            }

                        }
                        else
                            emb.WithDescription($"Значение может быть от 1 до {maxValue}");
                    }
                }
                await Context.Channel.SendMessageAsync("", false, emb.Build());
            }
        }
    }
}