using DarlingDb.Models;
using DarlingDb;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using System.Text.RegularExpressions;
using DarlingNet.Services.LocalService.VerifiedAction;
using static DarlingDb.Enums;

namespace DarlingNet.Services.LocalService.SpamCheck
{
    public static class UserMessageScanning
    {
        public class UserMessageForScan
        {
            public bool Detect { get; set; }
            public ulong UserId { get; set; }
            public ulong GuildId { get; set; }
            public List<SocketUserMessage> Messages = new ();
        }

        private const string Pattern = @"(?:https?://)?(?:\w+.)?discord(?:(?:app)?.com/invite|.gg)/([A-Za-z0-9-]+)";
        public static List<UserMessageForScan> MessageUserScan = new ();
        //private static List<SocketUserMessage> MessageList = new List<SocketUserMessage>();

        public static async Task<bool> ChatSystem(ShardedCommandContext Context, Channel Channel, string Prefix, uint UserGuildId)
        {
            using (db _db = new ())
            {
                bool retorn = false;

                if(!string.IsNullOrWhiteSpace(Context.Message.Content))
                {
                    if (Channel.InviteMessage)
                    {
                        if (new Regex(Pattern, RegexOptions.IgnoreCase | RegexOptions.Compiled).Matches(Context.Message.Content.Replace(" ", "").ToLower()).Any())
                        {
                            var Invite = Context.Guild.GetInvitesAsync();
                            if (Invite != null)
                            {
                                var Invitex = Invite.Result.FirstOrDefault(x => Context.Message.Content.Contains(x.Id));
                                if (Invitex == null)
                                {
                                    await Context.Message.DeleteAsync();
                                    retorn = true;
                                }
                            }
                        }
                    } // ИНВАЙТЫ

                    if (Channel.DelCaps)
                    {
                        if (Context.Message.Content.Count(c => char.IsUpper(c)) >= (Context.Message.Content.Length * 0.5))
                        {
                            await Context.Message.DeleteAsync();
                            retorn = true;
                        }
                    } // КАПС СООБЩЕНИЯ

                    if (Channel.SendBadWord && Channel.BadWordList.Count > 0)
                    {
                        int argPos = 0;
                        var x = Regex.Matches(Context.Message.Content, @"\b[\p{L}]+\b").Cast<Match>().Select(match => match.Value.ToLower()).Where(word => Channel.BadWordList.Contains(word)).Any();
                        if (x && !Context.Message.HasStringPrefix(Prefix, ref argPos))
                        {
                            await Context.Message.DeleteAsync();
                            retorn = true;
                        }
                    }// ПЛОХИЕ СЛОВА
                }

                if(!string.IsNullOrWhiteSpace(Context.Message.Content) || Context.Message.Embeds.Count > 0)
                {
                    if (Channel.Spaming)
                    {
                        var ThisUserData = MessageUserScan.FirstOrDefault(x => x.UserId == Context.User.Id && x.GuildId == Context.Guild.Id);
                        if (ThisUserData == null)
                        {
                            ThisUserData = new UserMessageForScan { UserId = Context.User.Id, GuildId = Context.Guild.Id };
                            ThisUserData.Messages.Add(Context.Message);
                            MessageUserScan.Add(ThisUserData);
                        }
                        else
                            ThisUserData.Messages.Add(Context.Message);

                        foreach (var User in MessageUserScan.ToList())
                        {
                            foreach (var Message in User.Messages)
                            {
                                MessageUserScan.RemoveAll(x => (DateTime.Now - Message.CreatedAt).TotalSeconds >= 5);
                            }
                        }
                        //MessageUserScan.RemoveAll(x => (DateTime.Now - x.Messages.FirstOrDefault().CreatedAt).TotalSeconds >= 5);

                        if (!ThisUserData.Detect && ThisUserData.Messages.Count > 3)
                        {
                            int CountSumMessage = 0;
                            foreach (var Messes in ThisUserData.Messages)
                            {
                                string MessageDetected = Messes.Content?.ToLower();
                                string MessageNew = Context.Message.Content?.ToLower();

                                if (!string.IsNullOrWhiteSpace(MessageNew))
                                    MessageNew = Context.Message.Embeds.FirstOrDefault()?.Image.Value.Url;
                                if (!string.IsNullOrWhiteSpace(MessageDetected))
                                    MessageNew = Messes.Embeds.FirstOrDefault()?.Image.Value.Url;

                                if (new MessageSpam().CalculateFuzzyEqualValue(MessageNew, MessageDetected) == 1 || // NULL EXCEPTION - Ошибка
                                    MessageNew.Contains(MessageDetected) ||
                                    MessageDetected.Contains(MessageNew))

                                    CountSumMessage++;
                            }

                            if (CountSumMessage > 3)
                            {
                                ThisUserData.Detect = true;
                                var User = Context.User as SocketGuildUser;
                                await User.AddMute(new TimeSpan(0, 5, 0));

                                var TempedUser = new TempUser() { Users_GuildId = UserGuildId, ToTime = DateTime.Now.AddMinutes(5), Reason = ReportTypeEnum.TimeOut };
                                _db.TempUser.Add(TempedUser);
                                await _db.SaveChangesAsync();

                                var emb = new EmbedBuilder().WithColor(255, 0, 94).WithAuthor("Spam System Detected");
                                emb.WithDescription($"Система, заметила странную активность от {Context.User.Mention},\nи замутила его на **5** минут.\n\nЕсли это мут ошибочный, снять - {Prefix}untimeout {Context.User.Mention}");
                                await Context.Channel.SendMessageAsync("", false, emb.Build());

                                var messa = await Context.Message.Channel.GetMessagesAsync(CountSumMessage).FlattenAsync();
                                var result = messa.Where(x => x.Author.Id == Context.Message.Author.Id);
                                await (Context.Channel as SocketTextChannel).DeleteMessagesAsync(result);
                                MessageUserScan.Remove(ThisUserData);
                                retorn = true;
                            }
                        }
                    } // СПАМ

                    if (Channel.DelUrl)
                    {
                        string message = Context.Message.Content;

                        if (new Regex(@"\b(?:https?://|www\.)\S+\b", RegexOptions.Compiled | RegexOptions.IgnoreCase).Matches(message).Count > 0)
                        {
                            bool success = true;
                            foreach (var word in Channel.csUrlWhiteListList)
                            {
                                if (message.Contains(word))
                                    success = false;
                            }

                            if (success)
                            {
                                success = false;
                                if (Context.Message.Embeds.Count > 0)
                                {
                                    if (!Context.Message.Embeds.Any(x => x.Image != null || x.Type == EmbedType.Gifv || x.Type == EmbedType.Image))
                                        success = true;
                                }
                                else
                                    success = true;
                            }

                            if (success)
                            {
                                var Permission = Context.Guild.CurrentUser.GuildPermissions;
                                if (Permission.Administrator || Permission.ManageMessages)
                                    await Context.Message.DeleteAsync();
                                retorn = true;
                            }

                        }
                    } // Отправка ссылки
                }

                return retorn;
            }

        } // Проверка сообщений
    }
}
