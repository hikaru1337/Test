using DarlingDb;
using DarlingNet.Services;
using DarlingNet.Services.LocalService.Attribute;
using DarlingNet.Services.LocalService.CommandList;
using DarlingNet.Services.LocalService.GetOrCreate;
using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DarlingNet.Modules
{
    [Summary("Управление ботом")]
    [RequireBotPermission(ChannelPermission.SendMessages)]
    [RequireBotPermission(GuildPermission.SendMessages)]
    [RequireBotPermission(GuildPermission.EmbedLinks)]
    public class Help : ModuleBase<ShardedCommandContext>
    {
        private readonly CommandService _service;
        private readonly IServiceProvider _provider;


        public Help(CommandService service, IServiceProvider provider)
        {
            _service = service;
            _provider = provider;
        }


        [Aliases, Commands, Usage, Descriptions]
        public async Task application(byte Number = 0)
        {
            using (db _db = new ())
            {
                var emb = new EmbedBuilder().WithColor(Color.Gold).WithAuthor("Функционал бота");
                var Guild = await _db.Guilds.GetOrCreate(Context.Guild.Id);
                if (Number == 0)
                {
                    for (int i = 1; i < 9; i++)
                    {
                        var x = CounterCommand(Guild.CommandInviseList, i);
                        var Text = x.Item3.Split("|");
                        emb.AddField($"{i}.{Text[0]} {EmojiName(x.Item1, x.Item2.Count)}", Text[1], true);
                    }

                    emb.WithFooter($"Включить/выключить раздел - {Guild.Prefix}application [number]\nВручную включить/выключить команды - {Guild.Prefix}i commandinvise");
                }
                else
                {
                    if (Number <= 8)
                    {
                        var ListCommand = Guild.CommandInviseList;
                        var x = CounterCommand(Guild.CommandInviseList, Number);
                        var ListCommandTwo = x.Item2;
                        string Commands = "";
                        int Counter = ListCommandTwo.Count;
                        int CounterAny = 0;
                        foreach (var item in ListCommandTwo)
                        {
                            if (ListCommand.Any(x => x == item))
                                CounterAny++;
                        }
                        emb.AddField("Тип", x.Item3.Split("|")[0],true);

                        string text = string.Empty;
                        int i = 0;

                        foreach (var item in ListCommandTwo)
                        {
                            i++;
                            if (Counter / 2 >= CounterAny)
                            {
                                if (!ListCommand.Any(x => x == item))
                                    ListCommand.Add(item);
                            }
                            else
                                ListCommand.Remove(item);

                            Commands += $"{item}";
                            if (i == 3)
                            {
                                Commands += "\n";
                                i = 0;
                            }
                            else
                                Commands += ",";
                        }

                        emb.AddField($"Список {(Counter / 2 >= CounterAny ? "отключенных" : "включенных")} команд", Commands, true);
                        Guild.CommandInviseList = ListCommand;
                        _db.Guilds.Update(Guild);
                        await _db.SaveChangesAsync();
                    }
                    else
                        emb.WithDescription("Номер может быть только от 1 до 8!");
                }

                await Context.Channel.SendMessageAsync("", false, emb.Build());
            }
        }

        private static string EmojiName(int i,int MaxCount)
        {
            string Emoji;
            if (i == MaxCount)
                Emoji = BotSettings.EmoteOn;
            else if (i == 0)
                Emoji = BotSettings.EmoteOff;
            else
                Emoji = BotSettings.EmoteWhat;
            return Emoji;
        }

        private (int, List<string>,string) CounterCommand(List<string> CommandInviseList, int Type)
        {
            int i = 0;
            List<string> ListCommand = new();
            string text = "";
            switch (Type)
            {
                case 1:
                    ListCommand = BotSettings.AdminCommands;
                    text = "Админ меню|Бан, кик, мут, временный мут, очистка сообщений, сообщения от бота\n⠀";
                    break;
                case 2:
                    ListCommand = _service.Modules.FirstOrDefault(x => x.Name == "Giveaway").Commands.Select(x => x.Aliases[0]).ToList();
                    text = "Розыгрыши|Меню розыгрышей\n⠀";
                    break;
                case 3:
                    ListCommand = _service.Modules.FirstOrDefault(x => x.Name == "NsfwGif").Commands.Select(x => x.Aliases[0]).Concat(_service.Modules.FirstOrDefault(x => x.Name == "SfwGif").Commands.Select(x => x.Aliases[0])).ToList();
                    text = "Гифки|RolePlay и 18+ gif-анимации\n⠀";
                    break;
                case 4:
                    ListCommand = _service.Modules.FirstOrDefault(x => x.Name == "Pet").Commands.Select(x => x.Aliases[0]).ToList();
                    text = "Игра-тамагочи|Доступ к минигре\n⠀";
                    break;
                case 5:
                    ListCommand = BotSettings.RoleCommands;
                    text = "Роли|Уровневые, временные, покупные роли\n⠀";
                    break;
                case 6:
                    ListCommand = BotSettings.ViolationCommands;
                    text = "Системы нарушений|Варн, репорт системы\n⠀";
                    break;
                case 7:
                    ListCommand = BotSettings.DetectCommands;
                    text = "Отслеживание|Информация о (Входе, выходе, удаленных/измененных сообщениях, голосовых каналах)\n⠀\nРоль при входе, публичное/личное сообщение при входе/выходе\n⠀\nНастройка текстовых каналов\n⠀";
                    break;
                case 8:
                    ListCommand = BotSettings.DetectCommands;
                    text = "Публичные инструменты|Экономика, получение опыта в голосовых/текстовых чатах, свадьбы\n⠀";
                    break;

            }

            foreach (var Command in ListCommand)
            {
                if (CommandInviseList.Any(x => x == Command))
                    i++;
            }
            return (i,ListCommand,text);
        }

        [Aliases, Commands, Usage, Descriptions]
        public async Task modules()
        {
            using (db _db = new ())
            {
                var Prefix = await _db.Guilds.GetPrefix(Context.Guild.Id);
                var emb = new EmbedBuilder().WithColor(255, 0, 94).WithAuthor("📚 Модули бота")
                                            .WithFooter($"Команды модуля - {Prefix}c [Модуль]");
                var mdls = _service.Modules;

                foreach (var mdl in mdls)
                {
                    var Permission = await mdl.GetExecutableCommandsAsync(Context, _provider);
                    if (Permission.Count > 0)
                    {
                        string emoji = string.Empty;
                        switch (mdl.Name)
                        {
                            case "Admins": emoji = "⚠️"; break;
                            case "Bot": emoji = "🌠‍"; break;
                            case "Giveaway": emoji = "🎉"; break;
                            case "Help": emoji = "📓"; break;
                            case "NsfwGif": emoji = "🎀"; break;
                            case "Settings": emoji = "⛓"; break;
                            case "SfwGif": emoji = "🎎"; break;
                            case "User": emoji = "🎃"; break;
                            case "Pet": emoji = "👽"; break;
                            case "Music": emoji = "🎵"; break;
                        }
                        emb.AddField(emoji + " " + mdl.Name, mdl.Summary + "\n⠀", true);
                    }
                }
                if (emb.Fields.Count == 0) 
                    emb.WithDescription("Модули бота отсутствуют!");
                await Context.Channel.SendMessageAsync("",false, emb.Build());
            }
        }

        [Aliases, Commands, Usage, Descriptions]
        public async Task commands(string modules)
        {
            using (db _db = new ())
            {
                var Guild = await _db.Guilds.GetOrCreate(Context.Guild.Id);
                var emb = new EmbedBuilder().WithColor(255, 0, 94).WithDescription("").WithAuthor($"📜 {modules} - Команды [префикс - {Guild.Prefix}]");

                var mdls = _service.Modules.FirstOrDefault(x => x.Name.ToLower() == modules.ToLower());
                if (mdls != null && mdls.GetExecutableCommandsAsync(Context, _provider).Result.Count > 0)
                {
                    var CommandList = Initiliaze.ListCommand.Where(z => mdls.Commands.Any(x => x.Aliases[0] == z.Usage[1] && x.CheckPreconditionsAsync(Context, _provider).Result.IsSuccess)).OrderBy(x=>x.Category).ToList();
                    string TextCommand = string.Empty;
                    foreach (var Command in CommandList)
                    {
                        bool Deleted = false;
                        var AccessBot = Guild.CommandInviseList.FirstOrDefault(x => x == Command.Usage[1]);
                        if (AccessBot != null)
                            Deleted = true;

                        if (!Deleted)
                        {
                            if (!string.IsNullOrWhiteSpace(Command.Category) && CommandList.Count(x => x.Category == Command.Category) > 1)
                            {
                                if (TextCommand == string.Empty || TextCommand != Command.Category)
                                {
                                    TextCommand = Command.Category;
                                    emb.AddField($"**{TextCommand}**\n", "⠀", true);
                                }
                                if(emb.Fields.Last().Value.ToString().Length == 1)
                                    emb.Fields.Last().Value = $"• {Command.Usage[1]}\n";
                                else
                                    emb.Fields.Last().Value += $"• {Command.Usage[1]}\n";
                            }
                            else
                            {
                                TextCommand = string.Empty;
                                emb.Description += $"• {Command.Usage[1]}\n";
                            }
                        }
                    }
                    if(emb.Description.Length > 0 && emb.Fields.Count > 0)
                    {
                        emb.Description = emb.Description.Insert(0, "📚**Остальные команды**\n");
                    }
                    emb.WithFooter($"Подробная информация о команде - {Guild.Prefix}i [Имя команды]");
                }
                else emb.WithDescription($"Модуль {modules} не найден!").WithAuthor($"📜{modules} - ошибка");
                await Context.Channel.SendMessageAsync("",false, emb.Build());
            }
        }

        [Aliases, Commands, Usage, Descriptions]
        public async Task info(string command)
        {
            using (db _db = new ())
            {
                command = command.ToLower();
                var Command = _service.Commands.FirstOrDefault(x => x.Aliases[0].ToLower() == command || x.Aliases.Last().ToLower() == command);
                var emb = new EmbedBuilder().WithAuthor($"📋 Информация о {command}").WithColor(255, 0, 94);

                if (Command != null)
                {
                    var Guild = await _db.Guilds.GetOrCreate(Context.Guild.Id);
                    if (!Guild.CommandInviseList.Contains(Command.Aliases[0]))
                    {
                        string text = string.Empty;
                        foreach (var Parameter in Command.Parameters)
                        {
                            text += $"[{Parameter}{(Parameter.IsOptional ? "/null" : "")}] ";
                        }
                        emb.AddField($"Сокращение: {Command.Remarks.Replace('"', ' ')}",
                                     $"Описание: {Command.Summary}\n" +
                                     $"Пример: {Guild.Prefix}{Command.Name} {text}");
                    }
                    else 
                        emb.WithDescription($"Команда `{command}` отключена создаталем сервера!");
                }
                else 
                    emb.WithDescription($"Команда `{command}` не найдена!");

                await Context.Channel.SendMessageAsync("", false ,emb.Build());
            }
        }

        [Aliases, Commands, Usage, Descriptions]
        public async Task use()
        {
            using (db _db = new ())
            {
                var Prefix = await _db.Guilds.GetPrefix(Context.Guild.Id);
                var bot = Context.Client.CurrentUser;
                var emb = new EmbedBuilder().WithColor(255, 0, 94).WithAuthor($"Информация о боте {bot.Username}🌏", bot.GetAvatarUrl())
                                                                  .WithDescription(string.Format(BotSettings.WelcomeText, Prefix, bot.Id))
                                                                  .WithImageUrl(BotSettings.bannerBoturl);
                await Context.Channel.SendMessageAsync("",false, emb.Build());
            }
        }
    }
}
