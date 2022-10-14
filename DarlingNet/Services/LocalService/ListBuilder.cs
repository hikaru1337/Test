using DarlingDb.Models;
using DarlingNet.Services.LocalService.VerifiedAction;
using Discord;
using Discord.Commands;
using Discord.Rest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static DarlingDb.Enums;
using static DarlingNet.Services.CommandHandler;


namespace DarlingNet.Services.LocalService
{
    public class ListBuilder
    {
        public static async Task<(ulong,int)> ListButtonSliderBuilder(IEnumerable<object> item, EmbedBuilder emb, string CommandName, ShardedCommandContext Context, int countslots = 10,bool Selected = false)
        {
            ulong MessageId = 0;
            if (Selected)
                countslots = 3;

            int page = 1;
            var Skipped = item.Skip((page - 1) * countslots).Take(countslots);
            emb = Lists(Skipped, CommandName, emb);

            var ButtonLeft = new ButtonBuilder {Label = "◀️",CustomId =  nameof(ButtonActionEnum.LeftRight_Left),Style = ButtonStyle.Secondary};
            var ButtonRight = new ButtonBuilder { Label = "▶️", CustomId = nameof(ButtonActionEnum.LeftRight_Right), Style = ButtonStyle.Secondary };

            if (item.Count() > countslots || Selected)
            {
                var MaxPage = Math.Ceiling(Convert.ToDouble(item.Count()) / countslots);
                emb.WithFooter($"Страница {(page)}/{MaxPage}");
                var Comp = new ComponentBuilder();

                for (int i = 1; i <= Skipped.Count(); i++)
                {
                    Comp.WithButton($"{i}", $"Number_{i}");
                    if (i == countslots)
                        break;
                }

                if (page != MaxPage)
                    Comp.WithButton(ButtonRight);

                var mes = await Context.Channel.SendMessageAsync("", false, emb.Build(), components: Comp.Build());
                MessageId = mes.Id;
                var time = DateTime.Now.AddSeconds(60);
                ListData.Add(new ActionData { MessageId = mes.Id,UserId = Context.User.Id, Type = ButtonActionEnum.LeftRight_Wait });
                ListData.Add(new ActionData { MessageId = mes.Id, UserId = Context.User.Id, Type = ButtonActionEnum.Number_Wait });

                while (time > DateTime.Now)
                {
                    if(Selected)
                    {
                        var ress = ListData.FirstOrDefault(x => x.MessageId == mes.Id &&
                                                           (x.Type == ButtonActionEnum.Number_Wait ||
                                                           x.Type == ButtonActionEnum.Number_1 ||
                                                           x.Type == ButtonActionEnum.Number_2 ||
                                                           x.Type == ButtonActionEnum.Number_3 ||
                                                           x.Type == ButtonActionEnum.Number_4 ||
                                                           x.Type == ButtonActionEnum.Number_5));

                        if (ress != null && ress.Type != ButtonActionEnum.Number_Wait)
                            break;
                    }

                    var res = ListData.FirstOrDefault(x => x.MessageId == mes.Id &&
                                                           (x.Type == ButtonActionEnum.LeftRight_Left ||
                                                           x.Type == ButtonActionEnum.LeftRight_Right ||
                                                           x.Type == ButtonActionEnum.LeftRight_Wait));

                    if (res.Type != ButtonActionEnum.LeftRight_Wait)
                    {
                        if (res.Type == ButtonActionEnum.LeftRight_Left)
                            page--;
                        else
                            page++;

                        res.Type = ButtonActionEnum.LeftRight_Wait;

                        Skipped = item.Skip((page - 1) * countslots).Take(countslots);

                        emb = Lists(Skipped, CommandName, emb);
                        emb.WithFooter($"Страница {(page)}/{MaxPage}");
                        var Component = new ComponentBuilder();

                        if (page > 1)
                            Component.WithButton(ButtonLeft);

                        if (Selected)
                        {
                            for (int i = 1; i <= Skipped.Count(); i++)
                            {
                                Component.WithButton($"{i}", $"Number_{i}");
                                if (i == countslots)
                                    break;
                            }
                        }

                        if (page != MaxPage)
                            Component.WithButton(ButtonRight);

                        await mes.ModifyAsync(x =>
                        {
                            x.Embed = emb.Build();
                            x.Components = Component.Build();
                        });

                        time = time.AddSeconds(60);
                    }
                }
                if(mes.Components.Any())
                    await mes.ModifyAsync(x => x.Components = new ComponentBuilder().Build());

                if (!Selected)
                    ListData.RemoveAll(x=>x.MessageId == mes.Id);
            }
            else
                await Context.Channel.Message("", emb);

            return (MessageId, page);
        }


        private static EmbedBuilder Lists(IEnumerable<object> items, string CommandName, EmbedBuilder emb)
        {
            if (CommandName == "myfeedback" || CommandName == "feedbugs")
            {
                emb.Fields.Clear();
                foreach (var Item in items.OfType<Feedback>())
                {
                    string message = Item.Message;
                    if (Item.Message.Length >= 13)
                        message = Item.Message.Substring(0, 13) + "...";

                    emb.AddField($"№{Item.Id} [{Item.Status}]", $"`{message}` [{Item.Time:dd.MM.yy}]", true);
                }
            }
            else if(CommandName == "buyrole" || CommandName == "timerole")
            {
                emb.WithDescription("");
                string Text = string.Empty;
                int i = 0;
                foreach (var Item in items.OfType<Roles>())
                {
                    i++;
                    if (Item.Value.Split(':')?.Length > 1)
                    {
                        var Minute = Convert.ToUInt64(Item.Value.Split(':')[1]);
                        if (Minute > 60)
                            Text = $"[{Minute} час]";
                        else if (Minute <= 60)
                            Text = $"[{Minute} минут]";
                        else if (Minute > 1440)
                            Text = $"[{Minute} дней]";
                    }
                    emb.Description += $"{i}.<@&{Item.RoleId}> - {Item.Value.Split(':')[0]} ZeroCoins {Text}\n";
                }
            }
            return emb;
        }
    }
}
