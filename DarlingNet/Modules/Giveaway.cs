using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DarlingDb;
using DarlingDb.Models;
using Discord.Rest;
using Microsoft.EntityFrameworkCore;
using DarlingNet.Services.LocalService.Attribute;
using static DarlingNet.Services.LocalService.Attribute.CommandLocksAttribute;

namespace DarlingNet.Modules
{
    [RequireUserPermission(GuildPermission.Administrator)]
    [RequireBotPermission(ChannelPermission.SendMessages)]
    [RequireBotPermission(ChannelPermission.EmbedLinks)]
    [Summary("Управление\nрозыгрышами")]
    public class Giveaway : ModuleBase<ShardedCommandContext>
    {
        sealed class TimeToEnd
        {
            public ulong Id { get; set; }
            public bool End { get; set; }
        }
        static readonly List<TimeToEnd> EndList = new();


        static string TextFormat(TimeSpan TimeToGo,string Give)
        {
            var text = $"Розыгрыш ***{Give} ***\nНажмите на эмодзи 🎟 чтобы учавствовать!";
            if (TimeToGo.TotalSeconds > 86400)
                text += $"\nОсталось: {TimeToGo.Days} дней и {TimeToGo.Hours} часов";
            else if (TimeToGo.TotalSeconds > 3600)
                text += $"\nОсталось: {TimeToGo.Hours} часов и {TimeToGo.Minutes} минут";
            if (TimeToGo.TotalSeconds > 60)
                text += $"\nОсталось: {TimeToGo.Minutes} минут и {TimeToGo.Seconds} секунд";
            else
                text += $"\nОсталось: {TimeToGo.Seconds} секунд";
            return text;
        }


        public static async Task GiveAwayTimer(GiveAways ThisTask,RestUserMessage message)
        {
            using (db _db = new ())
            {
                var emb = new EmbedBuilder().WithColor(255, 0, 94).WithAuthor($"🎲 **РОЗЫГРЫШ** 🎲");
                var TimeToGo = ThisTask.Times - DateTime.Now;
                string Text = TextFormat(TimeToGo,ThisTask.Surpice);
                var Task = EndList.FirstOrDefault(x => x.Id == ThisTask.Id);
                if (Task == null)
                {
                    Task = new TimeToEnd() { Id = ThisTask.Id };
                    EndList.Add(Task);
                }

                while (ThisTask.Times > DateTime.Now)
                {
                    TimeToGo = ThisTask.Times - DateTime.Now;
         
                    if (TimeToGo.TotalMinutes % 2 <= 0.005 || TimeToGo.TotalSeconds <= 60 && TimeToGo.TotalSeconds % 5 <= 0.1)
                    {
                        Text = TextFormat(TimeToGo, ThisTask.Surpice);
                        emb.WithDescription(Text);
                        await message.ModifyAsync(x => x.Embed = emb.Build());
                    }
                     if (Task.End)
                        break;
                }

                string Winner = string.Empty;
                if (Task.End)
                    emb.WithDescription("Розыгрыш завершен администрацией!");
                else
                {
                    var users = (message as IMessage).GetReactionUsersAsync(new Emoji("🎟"), int.MaxValue);
                    List<IUser> Allusers = new();
                    await foreach (var user in users)
                    {
                        foreach (var use in user.Where(x=>!x.IsBot))
                        {
                            Allusers.Add(use);
                        }
                    }

                    if (Allusers.Count > 0)
                    {
                        if(ThisTask.WinnerCount > 1)
                        {
                            List<IUser> WIN = new();
                            for (int i = 0; i < ThisTask.WinnerCount; i++)
                            {
                                var User = Allusers.ElementAt(new Pcg.PcgRandom().Next(Allusers.Count));
                                if (!WIN.Any(x => x.Id == User.Id))
                                {
                                    WIN.Add(User);
                                    Winner += $"<@{User.Id}>\n";
                                }
                                else if(i > 0 && WIN.Count != Allusers.Count)
                                    i--;
                                    
                            }
                            emb.WithDescription($"***Поздравляю***! Победители:\n {Winner} Выигрыш(и): {ThisTask.Surpice}!");
                            Winner = Winner.Replace('\n',',');
                        }
                        else
                        {
                            IUser WIN = Allusers.ElementAt(new Pcg.PcgRandom().Next(Allusers.Count));
                            Winner = WIN.Mention;
                            emb.WithDescription($"***Поздравляю***! {WIN.Mention} выиграл {ThisTask.Surpice}!");
                        }
                        
                        emb.WithColor(new Color(255, 255, 0));
                        await message.AddReactionAsync(new Emoji("🏆"));
                    }
                    else
                        emb.WithDescription("Недостаточно участников для розыгрыша!");
                }
                _db.GiveAways.Remove(ThisTask);
                await _db.SaveChangesAsync();
                EndList.Remove(Task);

                if(Winner?.Length == 0)
                    await message.ModifyAsync(x => x.Embed = emb.Build());
                else
                {
                    await message.DeleteAsync();
                    await message.Channel.SendMessageAsync(Winner, false, emb.Build());
                }
            }
        }

        [Aliases, Commands, Usage, Descriptions]
        [PermissionBlockCommand]
        [RequireUserPermission(GuildPermission.Administrator)]
        [RequireBotPermission(GuildPermission.AddReactions)]
        public async Task giveawaystart(string Time, byte WinnersCount, [Remainder] string Given)
        {
            using (db _db = new ())
            {
                bool Error = true;
                var emb = new EmbedBuilder().WithColor(255, 0, 94).WithAuthor($"🎲 **РОЗЫГРЫШ** 🎲");
                bool Success = TimeSpan.TryParse(Time, out TimeSpan result);
                if (!Success)
                    emb.WithDescription("Время введено неверно, возможно вы ввели слишком больше число?\nФормат: 01:00:00 [ч:м:с]\nФормат 2: 07:00:00:00 [д:ч:с:м]");
                else if (result.TotalSeconds < 30 || result.TotalSeconds > 604800)
                    emb.WithDescription("Время розыгрыша не может быть меньше 30 секунд, и больше 7 дней!");
                else
                {
                    int maxWinners = 25;
                    if (WinnersCount <= maxWinners)
                    {
                        int MaxGiveAway = 5;
                        var GiveAwaysCount = _db.GiveAways.Include(x => x.Channel).Count(x => x.Channel.GuildsId == Context.Guild.Id);
                        if (GiveAwaysCount <= MaxGiveAway)
                        {
                            var TimeToIvent = DateTime.Now.Add(result);
                            var TimeToGo = TimeToIvent - DateTime.Now;
                            var ReactionDice = new Emoji("🎟");
                            emb.WithDescription(TextFormat(TimeToGo, Given));
                            var message = await Context.Channel.SendMessageAsync("", false, emb.Build());
                            await message.AddReactionAsync(ReactionDice);
                            var ThisTask = _db.Add(new GiveAways() { ChannelId = Context.Channel.Id, Id = message.Id, Times = TimeToIvent, Surpice = Given, WinnerCount = WinnersCount }).Entity;
                            await _db.SaveChangesAsync();
                            EndList.Add(new TimeToEnd() { Id = ThisTask.Id });
                            await GiveAwayTimer(ThisTask, message);
                            Error = false;
                        }
                        else
                            emb.WithDescription($"Кол-во одновременных розыгрышей, не может превышать {MaxGiveAway}!");
                    }
                    else
                        emb.WithDescription($"Кол-во победителей, не может превышать {maxWinners}!");
                }

                if (Error)
                    await Context.Channel.SendMessageAsync("", false, emb.Build());
            }
        }

        [Aliases, Commands, Usage, Descriptions]
        [PermissionBlockCommand]
        public async Task giveawaystop(ulong MessageId)
        {
            var emb = new EmbedBuilder().WithColor(255, 0, 94).WithAuthor($"🎲 **РОЗЫГРЫШ**  🎲");
            var ThisGive = EndList.FirstOrDefault(x => x.Id == MessageId);
            if(ThisGive == null)
                emb.WithDescription("Розыгрыша с таким сообщением не существует!");
            else
            {
                ThisGive.End = true;
                emb.WithDescription("Розыгрыш принудительно завершен!");
            }
            await Context.Channel.SendMessageAsync("",false, emb.Build());
        }
    }
}