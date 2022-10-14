using DarlingDb;
using DarlingDb.Models;
using DarlingNet.Services.LocalService;
using DarlingNet.Services.LocalService.DiscordAudit;
using DarlingNet.Services.LocalService.DiscordAudit.Data;
using DarlingNet.Services.LocalService.GetOrCreate;
using DarlingNet.Services.LocalService.SpamCheck;
using DarlingNet.Services.LocalService.VerifiedAction;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static DarlingDb.Enums;

namespace DarlingNet.Services
{
    public class CommandHandler
    {
        private readonly DiscordShardedClient _discord;
        private readonly CommandService _commands;
        private readonly IServiceProvider _provider;

        public CommandHandler(DiscordShardedClient discord, CommandService commands, IServiceProvider provider)
        {
            _discord = discord;
            _commands = commands;
            _provider = provider;

            _commands.CommandExecuted += CommandErrors;
            _discord.ShardReady += ReadyShard;
        }


        private Task ReadyShard(DiscordSocketClient arg)
        {
            arg.ChannelDestroyed += ChannelDelete;
            arg.ChannelCreated += ChannelCreate;

            arg.RoleDeleted += RoleDeleted;
            arg.RoleUpdated += RoleUpdate;

            arg.PresenceUpdated += UserUpdate; // Обновление статуса пользователя
            arg.UserIsTyping += UserIsTyping; // Событие на активность пользователя в чате

            arg.JoinedGuild += GuildJoined; // Приглашение бота на сервер
            arg.LeftGuild += GuildLeft; // Кик бота с сервера

            arg.UserJoined += UserJoined; // Вход пользователя
            arg.UserLeft += UserLeft; // Событие на Выход/Кик пользователя
            arg.UserVoiceStateUpdated += UserVoiceAction; // Активность в голосовом канале
            arg.UserBanned += Banned; // Событие на бан пользователя
            arg.UserUnbanned += UnBanned; // Событие на разбан пользователя

            arg.ReactionAdded += ReactAdd; // Нажатие на эмодзи
            arg.ReactionRemoved += ReactRem; // Удаление нажатой эмодзи

            arg.InviteCreated += InviteCreate; // Создания приглашения
            arg.InviteDeleted += InviteDelete; // Удаления приглашения

            arg.MessagesBulkDeleted += BulkDeteleMessage; // Массовое удаление сообщений
            arg.MessageDeleted += DeleteMessage; // Событие на удаленные сообщения
            arg.MessageUpdated += EditedMessage; // Событие на измененные сообщения
            arg.MessageReceived += SendMessage; // Событие на отправленные сообщения

            arg.ButtonExecuted += ButtonHandler; // Событие нажатие на кнопку
            
            arg.SelectMenuExecuted += MyMenuHandler; // Событие выбора в меню || CAPTCHA

            BotSettings.ShardReadyCount++;
            return Task.CompletedTask;
        }


        private async Task CommandErrors(Optional<CommandInfo> Command, ICommandContext Context, IResult Result)
        {
            using (db _db = new())
            {
                if (!BotSettings.BotReady)
                    return;

                if (!string.IsNullOrWhiteSpace(Result?.ErrorReason))
                {
                    string Prefix = await _db.Guilds.GetPrefix(Context.Guild.Id);
                    var emb = await ErrorMessage.GetError(Result.ErrorReason, Prefix, Command.IsSpecified ? Command.Value : null);
                    await Context.Channel.Message("", emb);
                }
            }
        }

        private async Task ReactAdd(Cacheable<IUserMessage, ulong> mess, Cacheable<IMessageChannel, ulong> chnl, SocketReaction emj)
            => await GetOrRemoveRole(mess, emj, false);
        private async Task ReactRem(Cacheable<IUserMessage, ulong> mess, Cacheable<IMessageChannel, ulong> chnl, SocketReaction emj)
            => await GetOrRemoveRole(mess, emj, true);
        private static async Task GetOrRemoveRole(Cacheable<IUserMessage, ulong> mess, SocketReaction emj, bool getOrRemove)
        {
            using (db _db = new())
            {
                if (emj.User.Value.IsBot || !BotSettings.BotReady)
                    return;

                var message = await mess.GetOrDownloadAsync();
                if (message != null)
                {
                    string EmoteName = emj.Emote.ToString();
                    var mes = _db.EmoteClick.FirstOrDefault(x => x.MessageId == message.Id && EmoteName == x.Emote);
                    if (mes != null)
                    {
                        var usr = emj.User.Value as SocketGuildUser;
                        var role = usr.Guild.GetRole(mes.RoleId);
                        if (role != null)
                        {
                            var rolepos = usr.Guild.CurrentUser.Roles.FirstOrDefault(x => x.Position > role.Position);
                            if (rolepos != null)
                            {
                                if (getOrRemove && !mes.Get || !getOrRemove && mes.Get)
                                {
                                    if (usr.Roles.FirstOrDefault(x => x.Id == role.Id) != null)
                                        await usr.RemoveRole(role.Id);
                                }
                                else if (!getOrRemove && !mes.Get)
                                {
                                    if (usr.Roles.FirstOrDefault(x => x.Id == role.Id) == null)
                                        await usr.AddRole(role.Id);
                                }
                            }
                        }
                    }
                }
            }
        } // Проверка эмодзи в событии ReactRem и ReactAdd

        private async Task GuildLeft(SocketGuild Guild)
        {
            using (db _db = new())
            {
                if (!BotSettings.BotReady)
                    return;
                var GuildDB = await _db.Guilds.GetOrCreate(Guild.Id);
                if (!GuildDB.Leaved)
                {
                    GuildDB.Leaved = true;
                    _db.Guilds.Update(GuildDB);
                    await _db.SaveChangesAsync();
                }
            }
        }

        private async Task GuildJoined(SocketGuild Guild)
        {
            using (db _db = new())
            {
                if (!BotSettings.BotReady)
                    return;
                var GuildDB = await _db.Guilds.GetOrCreate(Guild.Id);
                if (GuildDB.Leaved)
                {
                    GuildDB.Leaved = false;
                    _db.Guilds.Update(GuildDB);
                    await _db.SaveChangesAsync();
                }

                var emb = new EmbedBuilder().WithColor(255, 0, 94)
                                            .WithAuthor($"Информация о боте {Guild.CurrentUser.Username}🌏", Guild.CurrentUser.GetAvatarUrl())
                                            .WithDescription(string.Format(BotSettings.WelcomeText, GuildDB.Prefix, Guild.CurrentUser.Id))
                                            .WithImageUrl(BotSettings.bannerBoturl);

                if (Guild.DefaultChannel != null)
                    await Guild.DefaultChannel.Message("", emb);

                await Guild.Owner.UserMessage("", emb);
            }
        }

        public static async Task ChannelCreate(SocketChannel Channel)
        {
            if (Channel is SocketTextChannel TextChannel)
            {
                using (db _db = new())
                {
                    if (!BotSettings.BotReady)
                        return;
                    var Captcha = _db.Guilds_Captcha.FirstOrDefault(x => x.GuildId == TextChannel.Guild.Id);
                    if (Captcha != null && Captcha.Run && Captcha.ChannelId != null && Captcha.RoleId != null)
                    {
                        var Role = TextChannel.Guild.GetRole(Convert.ToUInt64(Captcha.RoleId));
                        await TextChannel.AddPermissionOverwriteAsync(Role, new OverwritePermissions(viewChannel: PermValue.Deny));
                    }
                }
            }
        }


        public async Task MyMenuHandler(SocketMessageComponent arg)
        {
            using (db _db = new())
            {
                if (!BotSettings.BotReady)
                    return;
                switch (arg.Data.CustomId)
                {
                    case "GetCaptcha":
                        {
                            var Struct = CapthaService.CreateImage(200, 125);
                            var Guild = await _db.Guilds.GetOrCreate((arg.Channel as SocketTextChannel).Guild.Id);
                            var UserCaptcha = _db.Captcha.FirstOrDefault(x => x.UserId == arg.User.Id && x.GuildId == Guild.Id);

                            if (UserCaptcha == null)
                            {
                                UserCaptcha = new Captcha { UserId = arg.User.Id, GuildId = Guild.Id, Result = (uint)Struct.Item2, TimeToClear = DateTime.Now.AddDays(1) };
                                _db.Captcha.Add(UserCaptcha);
                            }
                            else
                            {
                                UserCaptcha.Result = (uint)Struct.Item2;
                                UserCaptcha.TimeToClear = DateTime.Now.AddDays(1);
                            }

                            await _db.SaveChangesAsync();

                            var Path = Guid.NewGuid().ToString() + ".jpg";
                            Struct.Item1.Save(Path, System.Drawing.Imaging.ImageFormat.Jpeg);
                            var Channel = _discord.GetChannel(BotSettings.ChannelScreenshots) as SocketTextChannel;
                            var Message = await Channel.SendFileAsync(Path, "Captcha");
                            var Emb = new Embed[1]
                            {
                                new EmbedBuilder().WithAuthor("Captcha").WithDescription("Введите ответ для прохождения проверки!").WithFooter("Если фото неразборчиво, нажмите на кнопку `получить капчу`").WithImageUrl(Message.Attachments.FirstOrDefault().Url).Build()
                            };
                            await arg.RespondAsync($"", Emb, ephemeral: true);
                            File.Delete(Path);

                        }
                        break;

                }
            }
            //await arg.DeferAsync();
        }


        private async Task RoleDeleted(SocketRole Role)
        {
            using (db _db = new())
            {
                if (!BotSettings.BotReady)
                    return;
                var RoleDB = _db.Role.FirstOrDefault(x => x.Id == Role.Id);
                if (RoleDB != null)
                {
                    _db.Role.Remove(RoleDB);
                    await _db.SaveChangesAsync();
                }
            }
        }
        private async Task ChannelDelete(SocketChannel chnl)
        {
            using (db _db = new())
            {
                if (!BotSettings.BotReady)
                    return;
                if (chnl is SocketTextChannel Channel)
                {
                    var ChannelDB = _db.Channels.FirstOrDefault(x => x.Id == Channel.Id);
                    if (ChannelDB != null)
                    {
                        _db.Channels.Remove(ChannelDB);
                        await _db.SaveChangesAsync();
                    }
                }

            }
        }

        private async Task RoleUpdate(SocketRole Before, SocketRole After)
        {
            if (Before != null || After != null || !BotSettings.BotReady)
                return;

            var botRole = Before.Guild.CurrentUser.Roles.OrderByDescending(x => x.Position).FirstOrDefault();
            if (botRole.Id == After.Id)
            {
                var emb = new EmbedBuilder().WithColor(255, 0, 94);
                if (!After.Permissions.Administrator)
                {
                    emb.WithAuthor("Нарушение работы бота.").WithDescription("Привет, мы заметили что вы убрали права администратора у бота.\n\nМы понимаем что вы боитесь за свой сервер, но при отсутствии данного права у бота, его работа может нарушиться, мы уже работаем над этим.\n\nЯ обещаю что бот ничего не сделает,`?*'глупые людишки вашему серверу ко%$#`, и все будет под контролем :)");
                }
                else if (!Before.Permissions.Administrator && After.Permissions.Administrator)
                {
                    emb.WithAuthor("Исправление работы бота.").WithDescription("Предклонись перед моей мощью человек!\nЯ получил права, а значит могу сделать с твоим сервером что хочу!\n\nЭто шутка, он ничего не будет делать :)");
                }

                if (emb.Description.Length > 0)
                    await Before.Guild.Owner.UserMessage("", emb);
            }
        }



        public class ActionData
        {
            public ulong UserId { get; set; }
            public ulong MessageId { get; set; }
            public ButtonActionEnum Type { get; set; }
        }

        public readonly static List<ActionData> ListData = new();

        private async Task ButtonHandler(SocketMessageComponent Button)
        {
            using (db _db = new())
            {
                if (!BotSettings.BotReady)
                    return;
                if (Button.Data.CustomId.Contains("ButtonClickAction_"))
                {
                    var Id = Convert.ToUInt64(Button.Data.CustomId.Split('_')[1]);
                    var ButtonClick = _db.ButtonClick.FirstOrDefault(x => x.Id == Id);
                    if (!string.IsNullOrWhiteSpace(ButtonClick.Value))
                    {
                        ButtonClick.Value = ButtonClick.Value.Replace("%user%", Button.User.Mention);
                        var embed = JsonToEmbed.JsonCheck(ButtonClick.Value);
                        if (embed.Item1 == null && embed.Item2 == null)
                            await Button.Message.Channel.Message(ButtonClick.Value);
                        else
                            await Button.Message.Channel.Message(embed.Item2, embed.Item1);
                    }

                    ulong Role = Convert.ToUInt64(ButtonClick.RoleId);
                    if (Role != 0)
                    {
                        if (ButtonClick.RoleDelOrGet)
                            await Button.User.AddRole(Role);
                        else
                            await Button.User.RemoveRole(Role);
                    }
                }
                else if (Button.Data.CustomId.Contains("DarlingFind_"))
                {
                    //bool state = Enum.TryParse(Button.Data.CustomId.Split("DarlingFind_")[1], out State myStatus);
                    //if (state)
                    //{
                    //    var Messages = await Button.Channel.GetMessagesAsync(5).FlattenAsync();
                    //    foreach (var Message in Messages.Where(x => x.Components.Count > 0))
                    //        if (Message is SocketUserMessage Mes)
                    //            await Mes.ModifyAsync(x => x.Components = new Optional<MessageComponent>());

                    //    var Darling = _db.Darling.FirstOrDefault(x => x.UsersId == Button.User.Id);
                    //    if (Darling != null)
                    //    {
                    //        Darling.State = myStatus;
                    //        _db.Darling.Update(Darling);
                    //        await _db.SaveChangesAsync();
                    //        await FindDarling.StationGenerate(Button.User);
                    //    }
                    //}
                }
                else
                {
                    _ = Enum.TryParse(Button.Data.CustomId, out ButtonActionEnum myStatus);
                    var result = ListData.FirstOrDefault(x => x.MessageId == Button.Message.Id && x.Type.ToString().Contains(Button.Data.CustomId.Split('_')[0]));
                    if (result != null)
                    {
                        if (result.UserId == Button.User.Id)
                        {
                            switch (result.Type)
                            {
                                case ButtonActionEnum.Marryed_Wait:
                                    result.Type = myStatus;
                                    break;
                                case ButtonActionEnum.Number_Wait:
                                    result.Type = myStatus;
                                    break;
                                case ButtonActionEnum.LeftRight_Wait:
                                    result.Type = myStatus;
                                    break;
                            }
                        }
                    }
                    else
                        await Button.Message.ModifyAsync(x => x.Components = new ComponentBuilder().Build());
                }
                await Button.DeferAsync();
            }
        }

        private async Task UserVoiceAction(SocketUser User, SocketVoiceState ActionBefore, SocketVoiceState ActionAfter)
        {
            using (db _db = new())
            {
                if (!BotSettings.BotReady)
                    return;
                await UpdateOnlineDate(User);

                var emb = new EmbedBuilder().WithColor(255, 0, 94).WithTimestamp(DateTimeOffset.Now.ToUniversalTime())
                    .WithFooter(x => x.WithText($"id: {User.Id}")).AddField($"Пользователь", $"{User.Mention}", true);
                var UserGuild = User as SocketGuildUser;
                var Guild = await _db.Guilds.GetOrCreate(UserGuild.Guild.Id);

                if (ActionBefore.VoiceChannel != null)
                {
                    if (UserGuild.Guild.GetVoiceChannel(Guild.PrivateId) != null)
                        await PrivateSystem.CheckPrivate(UserGuild.Guild);

                    if (ActionAfter.VoiceChannel == null)
                    {
                        emb.WithAuthor(" - Выход из Голосового чата", User.GetAvatarUrl())
                           .AddField($"Выход из", $"{ActionBefore.VoiceChannel.Name}", true);
                        // Выход пользователя из голосового чата
                    }
                    else
                    {
                        if (ActionBefore.VoiceChannel != ActionAfter.VoiceChannel)
                        {
                            emb.WithAuthor(" - Переход в другой Голосовой канал", UserGuild.GetAvatarUrl())
                               .AddField($"Переход из", $"{ActionBefore.VoiceChannel.Name}", true)
                               .AddField($"Переход в", $"{ActionAfter.VoiceChannel.Name}", true);

                            if (Guild.VoiceAndCategoryChannelList.Any(x => x == ActionAfter.VoiceChannel.Id || 
                                                                           x == ActionAfter.VoiceChannel.CategoryId))
                                await TaskTimer.StartVoiceActivity(User as SocketGuildUser);
                        }
                        else
                        {
                            var Audit = new AuditsUserAction();
                            string text = string.Empty;

                            emb.AddField($"В канале", $"{ActionBefore.VoiceChannel.Name}", true);
                            var TypeAction = VoiceAuditActionEnum.Defect;
                            if (ActionAfter.IsDeafened)
                            {
                                TypeAction = VoiceAuditActionEnum.AdminDeafened;
                                text = "Администратор отключил звук";
                            }
                            else if (ActionAfter.IsMuted)
                            {
                                TypeAction = VoiceAuditActionEnum.AdminMute;
                                text = "Администратор отключил микрофон";
                            }
                            else if (ActionBefore.IsDeafened)
                            {
                                TypeAction = VoiceAuditActionEnum.AdminUnDeafened;
                                text = "Администратор включил звук";
                            }
                            else if (ActionBefore.IsMuted)
                            {
                                TypeAction = VoiceAuditActionEnum.AdminUnMute;
                                text = "Администратор включил микрофон";
                            }
                            else if (ActionAfter.IsSelfDeafened)
                                text = "Пользователь отключил звук";
                            else if (ActionAfter.IsSelfMuted)
                                text = "Пользователь отключил микрофон";
                            else if (ActionAfter.IsStreaming)
                                text = "Пользователь запустил стрим";
                            else if (ActionBefore.IsSelfDeafened)
                                text = "Пользователь включил звук";
                            else if (ActionBefore.IsSelfMuted)
                                text = "Пользователь включил микрофон";
                            else if (ActionBefore.IsStreaming)
                                text = "Пользователь закончил стрим";

                            if(TypeAction != VoiceAuditActionEnum.Defect)
                                Audit = User.AdminVoiceAudit(User.Id, 1, TypeAction).Result?.FirstOrDefault();

                            if (Audit != null && Audit.User != null)
                                emb.AddField($"Администратор", $"{Audit.User.Mention}");

                            emb.WithAuthor(" - " + text, User.GetAvatarUrl());
                        }
                        // Переход из одного чата в другой
                    }
                }

                if (ActionAfter.VoiceChannel != null)
                {
                    if (Guild.PrivateId == ActionAfter.VoiceChannel.Id && !User.IsBot)
                    {
                        var prv = _db.PrivateChannels.Include(x => x.Users_Guild).Count(x => x.Users_Guild.GuildsId == Guild.Id);
                        var OnlineCount = UserGuild.Guild.Users.Count(x => x.Status == UserStatus.Online || x.Status == UserStatus.DoNotDisturb);
                        var OnlineDel = OnlineCount / 0.6;
                        if (OnlineDel >= prv)
                        {
                            await PrivateSystem.PrivateCreate(UserGuild, ActionAfter.VoiceChannel);
                        }// Проверка приваток
                    }

                    if (ActionBefore.VoiceChannel == null)
                    {
                        emb.WithAuthor(" - Вход в голосовой чат", User.GetAvatarUrl())
                           .AddField($"Вход в ", $"{ActionAfter.VoiceChannel.Name}", true);
                        // Вход пользователя в голосовой чат

                        if (Guild.VoiceAndCategoryChannelList.Any(x => x == ActionAfter.VoiceChannel.Id || 
                                                                       x == ActionAfter.VoiceChannel.CategoryId))
                            await TaskTimer.StartVoiceActivity(User as SocketGuildUser);
                    }
                }
                var Guild_Log = _db.Guilds_Logs.GetLogChannel(Guild.Id, ChannelsTypeEnum.VoiceAction);
                if (Guild_Log != null)
                {
                    var chnl = UserGuild.Guild.GetTextChannel(Guild_Log.ChannelId);
                    if (chnl != null)
                        await chnl.Message("", emb);
                }
            }
        }



        private async Task InviteDelete(SocketGuildChannel Guild, string InviteId)
        {
            using (db _db = new())
            {
                if (BotSettings.BotReady && Guild.Guild.DarlingBoostGet() == null)
                {
                    var ThisInvite = _db.Invites.FirstOrDefault(x => x.Key == InviteId);
                    if (ThisInvite != null)
                    {
                        _db.Invites.Remove(ThisInvite);
                        await _db.SaveChangesAsync();
                    }
                }
            }
        }

        private async Task InviteCreate(SocketInvite Invite)
        {
            using (db _db = new())
            {
                if (BotSettings.BotReady && Invite.Guild.DarlingBoostGet() == null)
                {
                    var User = await _db.Users_Guild.GetOrCreate(Invite.Inviter.Id, Invite.Guild.Id, Invite.Inviter.IsBot);
                    await _db.Channels.GetOrCreate(Invite.Guild.Id, Invite.ChannelId);
                    var NewInvite = new Invites() { Users_GuildId = User.Id, Key = Invite.Code, UsesCount = Invite.Uses, ChannelId = Invite.ChannelId };
                    _db.Invites.Add(NewInvite);
                    await _db.SaveChangesAsync();
                }
            }
        }


        private static async Task UpdateOnlineDate(IUser UserNow)
        {
            using (db _db = new())
            {
                var User = _db.Users.FirstOrDefault(x => x.Id == UserNow.Id);
                if (User != null)
                {
                    User.LastOnline = DateTime.Now;
                    _db.Users.Update(User);
                    await _db.SaveChangesAsync();
                }
            }
        }
        private async Task UserUpdate(SocketUser UserNow, SocketPresence Before, SocketPresence After)
        {
            if (Before != null && Before.Status != After.Status && BotSettings.BotReady && UserNow != null)
                await UpdateOnlineDate(UserNow);
        }
        private async Task UserIsTyping(Cacheable<IUser, ulong> arg1, Cacheable<IMessageChannel, ulong> arg2)
        {
            var User = await arg1.GetOrDownloadAsync();
            if (User != null)
                await UpdateOnlineDate(User);
        }



        private async Task UserJoined(SocketGuildUser user)
        {
            using (db _db = new())
            {
                if (!BotSettings.BotReady)
                    return;

                var Guild = _db.Guilds.Include(x => x.Guilds_Raid).Include(x => x.Guilds_Captcha).Include(x => x.Guilds_Meeting_Welcome).ThenInclude(x => x.WelcomeChannel).FirstOrDefault(x => x.Id == user.Guild.Id);

                if (Guild != null)
                {
                    #region Captcha
                    var Captcha = Guild.Guilds_Captcha;
                    if (Captcha != null && Captcha.Run)
                    {
                        var Role = user.Guild.GetRole(Convert.ToUInt64(Captcha.RoleId));
                        if (Role != null)
                            await user.AddRole(Role.Id);
                    }
                    #endregion

                    #region AntiRaid
                    var Raid = Guild.Guilds_Raid;
                    if (Raid != null && Raid.RaidRunning)
                    {
                        async void AddMute(SocketGuildUser UserMute = null)
                            => await (UserMute ?? user).AddMute(new TimeSpan(0, 5, 0));

                        if (Raid.RaidMuted > DateTime.Now)
                            AddMute();
                        else
                        {
                            var CountRaids = user.CheckRaid(Raid.RaidTime);
                            if (CountRaids.Count() >= Raid.RaidUserCount)
                            {
                                Raid.RaidMuted = DateTime.Now.AddSeconds(30);
                                foreach (var userz in CountRaids)
                                {
                                    var UserGuild = user.Guild.GetUser(userz.UsersId);
                                    if (UserGuild != null)
                                        AddMute(UserGuild);
                                }
                            }
                        }
                    }
                    #endregion

                    #region MeetingWelcome
                    var MeetingWelcome = Guild.Guilds_Meeting_Welcome;
                    if (MeetingWelcome != null)
                    {
                        if (!user.IsBot)
                        {
                            await user.AddRole(Convert.ToUInt64(MeetingWelcome.WelcomeRoleId));
                            await MeetingSendMessage(user.Guild, user, 0, MeetingWelcome.WelcomeDMmessage);
                        }
                        await MeetingSendMessage(user.Guild, user, MeetingWelcome.WelcomeChannelId, MeetingWelcome.WelcomeMessage);
                    }
                    #endregion

                    #region Guild Logs
                    var Guild_Log = _db.Guilds_Logs.GetLogChannel(Guild.Id, ChannelsTypeEnum.Join);
                    if (Guild_Log != null)
                    {
                        var JoinedChannelDiscord = user.Guild.GetTextChannel(Guild_Log.ChannelId);
                        if (JoinedChannelDiscord != null)
                        {
                            var builder = new EmbedBuilder().WithColor(255, 0, 94);
                            if (user.Guild.DarlingBoostGet() == null)
                            {
                                foreach (var inviteMetadata in user.Guild.GetInvitesAsync().Result)
                                {
                                    var Invite = _db.Invites.FirstOrDefault(x => x.Key == inviteMetadata.Code);
                                    if (Invite != null && Invite.UsesCount != inviteMetadata.Uses)
                                    {
                                        Invite.UsesCount++;
                                        _db.Invites.Update(Invite);
                                        await _db.SaveChangesAsync();
                                        builder.AddField("Приглашение", $"Code: {inviteMetadata.Code}\nСоздатель: {inviteMetadata.Inviter.Mention}");
                                        break;
                                    }
                                }
                            }
                            builder.WithTimestamp(DateTimeOffset.Now.ToUniversalTime())
                                                            .WithAuthor($"Пользователь присоединился: {user.Username}", user.GetAvatarUrl())
                                                            .WithDescription($"Имя: {user.Mention}\n" +
                                                            $"Участников: {user.Guild.MemberCount}\n" +
                                                            $"Аккаунт создан: {user.CreatedAt.ToUniversalTime():dd.MM.yyyy HH:mm:ss}");
                            await JoinedChannelDiscord.Message("", builder);

                        }
                    }
                    #endregion
                }

            }
        }


        private static async Task MeetingSendMessage(SocketGuild Guild, SocketUser User, ulong ChannelId = 0, string JsonMessage = null,EmbedBuilder emb = null)
        {
            var NewType = (emb, string.Empty);
            if (!string.IsNullOrWhiteSpace(JsonMessage))
            {
                NewType = JsonToEmbed.JsonCheck(JsonMessage);
                if (NewType.Item1 != null)
                {
                    var Description = NewType.Item1.Description;
                    if (Description.Length > 0 && Description.Contains("%user%"))
                        NewType.Item1.Description = Description.Replace("%user%", User.Mention);
                }
            }

            if (ChannelId != 0)
            {
                var WelcomeChannelDiscord = Guild.GetTextChannel(ChannelId);
                if (WelcomeChannelDiscord != null)
                    await WelcomeChannelDiscord.Message(NewType.Item2, NewType.Item1);
            }
            else
                await User.UserMessage(NewType.Item2, NewType.Item1);
        }

        private async Task UserLeft(SocketGuild Guild, SocketUser User)
        {
            using (db _db = new())
            {
                if (!BotSettings.BotReady && User.Id == _discord.CurrentUser.Id)
                    return;

                var LeftChannel = _db.Guilds_Meeting_Leave.FirstOrDefault(x => x.GuildsId == Guild.Id);
                if (LeftChannel != null)
                    await MeetingSendMessage(Guild,User,LeftChannel.LeaveChannelId,LeftChannel.LeaveMessage);

                var LeftLogs = _db.Guilds_Logs.GetLogChannel(Guild.Id, ChannelsTypeEnum.Left);
                if (LeftLogs != null)
                {
                    var LeftChannelDiscord = Guild.GetTextChannel(LeftLogs.ChannelId);
                    if (LeftChannelDiscord != null)
                    {
                        List<Audits> Kicks = await Guild.KickAudit(User.Id, 1);
                        var Kicked = Kicks?.FirstOrDefault();
                        bool es = false;
                        if (Kicked != null && (DateTime.Now - Kicked.Time).TotalSeconds < 1.5)
                            es = true;

                        var builder = new EmbedBuilder().WithColor(255, 0, 94)
                                                                    .WithTimestamp(DateTimeOffset.Now.ToUniversalTime())
                                                                    .WithAuthor($"{User} - Пользователь {(es ? "Кикнут" : "Вышел")}", User.GetAvatarUrl())
                                                                    .AddField($"Пользователь", User.Mention, true);

                        if (es)
                        {
                            builder.AddField($"Кикнул", Kicked.User.Mention, true)
                                   .AddField($"Причина", $"{(string.IsNullOrWhiteSpace(Kicked.Reason) ? "-" : Kicked.Reason)}");

                            //await GOCStinger.ReportGetOrCreate(Kicked.User.Id, User.Id, ReportSuspens.Report.Kick); 
                        }

                        builder.AddField($"Участников", Guild.MemberCount);

                        builder.AddField($"Аккаунт Создан", User.CreatedAt.ToString("dd.MM.yyyy HH:mm:ss"), true)
                               .AddField($"Был на сервере с", (User as SocketGuildUser).JoinedAt.Value.ToString("dd.MM.yyyy HH:mm:ss"), true);
                        await LeftChannelDiscord.Message("", builder);
                    }
                }
            }
        }



        private async Task UnBanned(SocketUser user, SocketGuild guild) => await BanOrUnBan(user, guild, false);
        private async Task Banned(SocketUser user, SocketGuild guild) => await BanOrUnBan(user, guild, true);
        private static async Task BanOrUnBan(SocketUser user, SocketGuild Guild, bool Ban)
        {
            using (db _db = new())
            {
                if (!BotSettings.BotReady)
                    return;
                var Guild_Log = _db.Guilds_Logs.GetLogChannel(Guild.Id, Ban ? ChannelsTypeEnum.Ban : ChannelsTypeEnum.UnBan);
                if (Guild_Log != null)
                {
                    var ChannelForMessage = Guild.GetTextChannel(Guild_Log.ChannelId);
                    if (ChannelForMessage != null)
                    {
                        List<Audits> Bans;
                        if (Ban)
                            Bans = await Guild.BanAudit(user.Id, 1);
                        else
                            Bans = await Guild.UnBanAudit(user.Id, 1);

                        Audits Banned = Bans.FirstOrDefault();
                        //if (Ban)
                        //    await GOCStinger.ReportGetOrCreate(Banned.User.Id, user.Id, ReportSuspens.Report.Ban);
                        var builder = new EmbedBuilder().WithColor(255, 0, 94)
                                                        .WithTimestamp(DateTimeOffset.Now.ToUniversalTime())
                                                        .WithAuthor($"{user} - Пользователь {(Ban ? "Забанен" : "Разбанен")}")
                                                        .AddField($"Пользователь", user.Mention, true)
                                                        .AddField($"{(Ban ? "Забанил" : "Разбанил")}", Banned.User.Mention, true)
                                                        .AddField($"Причина бана", $"{(string.IsNullOrWhiteSpace(Banned.Reason) ? "-" : Banned.Reason)}");
                        await ChannelForMessage.Message("", builder);
                    }
                }
            }
        }



        private async Task BulkDeteleMessage(IReadOnlyCollection<Cacheable<IMessage, ulong>> Messages, Cacheable<IMessageChannel, ulong> Channel)
        {
            using (db _db = new())
            {
                if (!BotSettings.BotReady)
                    return;
                if (await Channel.GetOrDownloadAsync() is SocketTextChannel GettedChannel)
                {
                    var Guild_Log = _db.Guilds_Logs.GetLogChannel(GettedChannel.Guild.Id, ChannelsTypeEnum.MessageDelete);
                    if (Guild_Log != null)
                    {
                        var MessageChannel = GettedChannel.Guild.GetTextChannel(Guild_Log.ChannelId);
                        if (MessageChannel != null)
                        {
                            var emb = new EmbedBuilder().WithColor(255, 0, 94).WithAuthor("Массовое удаление сообщений");
                            
                            emb.AddField("Канал", $"<#{Channel.Id}>", true);
                            emb.WithTimestamp(DateTime.Now.ToUniversalTime());
                            IUser UserNow = null;
                            var List = Messages.Reverse();
                            var Emotes = _db.EmoteClick.Where(x=>x.ChannelId == Channel.Id);
                            bool EmotesAny = Emotes.Any();
                            var Buttons = _db.ButtonClick.Where(x => x.ChannelId == Channel.Id);
                            bool ButtonsAny = Buttons.Any();
                            foreach (var Message in List)
                            {
                                var GettedMessage = await Message.GetOrDownloadAsync();
                                if (GettedMessage == null || GettedMessage.Author.IsBot || GettedMessage.Content.Length > 1023)
                                    return;

                                if (UserNow != GettedMessage.Author || UserNow == null)
                                {
                                    UserNow = GettedMessage.Author;
                                    emb.Description += $"Отправитель [{GettedMessage.Author.Mention}]\n";
                                }

                                string Text = "⠀⠀Сообщение [`{0}`:{1}]";

                                var messageTime = GettedMessage.Timestamp.ToString("HH:mm dd.MM.yy");
                                if (string.IsNullOrWhiteSpace(GettedMessage.Content))
                                    emb.Description += string.Format(Text, GettedMessage.Attachments.FirstOrDefault().Url, messageTime);
                                else
                                    emb.Description += string.Format(Text, GettedMessage.Content, messageTime);
                                
                                emb.Description += "\n";

                                if(ButtonsAny)
                                {
                                    var Button = Buttons.FirstOrDefault(x => x.MessageId == Message.Id);
                                    _db.ButtonClick.Remove(Button);
                                }
                                if(EmotesAny)
                                {
                                    var Emote = Emotes.FirstOrDefault(x => x.MessageId == Message.Id);
                                    _db.EmoteClick.Remove(Emote);
                                }

                                if (emb.Description.Length > 800)
                                {
                                    UserNow = null;
                                    await MessageChannel.Message("", emb);
                                    emb.WithDescription("");
                                }
                            }
                            await _db.SaveChangesAsync();
                            await MessageChannel.Message("", emb);
                        }
                    }
                }
            }
        }
        private async Task DeleteMessage(Cacheable<IMessage, ulong> arg1, Cacheable<IMessageChannel, ulong> arg2)
        {
            using (db _db = new())
            {
                var Message = await arg1.GetOrDownloadAsync();
                if (Message == null || Message.Author.IsBot || Message.Content.Length > 1023 || !BotSettings.BotReady)
                    return;

                if (Message.Author is SocketGuildUser User)
                {
                    var Guild_Log = _db.Guilds_Logs.GetLogChannel(User.Guild.Id, ChannelsTypeEnum.MessageDelete);
                    if (Guild_Log != null)
                    {
                        var MessageChannel = User.Guild.GetTextChannel(Guild_Log.ChannelId);
                        if (MessageChannel != null)
                        {
                            var emb = new EmbedBuilder().WithColor(255, 0, 94).WithAuthor("Сообщение удалено", Message.Author.GetAvatarUrl());
                            emb.AddField("Канал", $"<#{Message.Channel.Id}>", true);
                            emb.AddField("Сообщение", string.IsNullOrWhiteSpace(Message.Content) ? "-" : Message.Content, true);
                            emb.AddField("Отправитель", Message.Author.Mention);
                            if (Message.Attachments.Count > 0)
                                emb.WithImageUrl(Message.Attachments.FirstOrDefault().Url);

                            emb.WithTimestamp(DateTime.Now.ToUniversalTime());
                            await MessageChannel.Message("", emb);
                        }
                    }

                    var em = _db.EmoteClick.FirstOrDefault(x => x.MessageId == Message.Id && x.ChannelId == Message.Channel.Id);
                    if (em != null)
                        _db.EmoteClick.Remove(em);

                    var bc = _db.ButtonClick.FirstOrDefault(x => x.MessageId == Message.Id && x.ChannelId == Message.Channel.Id);
                    if (bc != null)
                        _db.ButtonClick.Remove(bc);

                    if (bc != null || em != null)
                        await _db.SaveChangesAsync();
                }
            }
        }

        private async Task EditedMessage(Cacheable<IMessage, ulong> CachedMessage, SocketMessage MessageNow, ISocketMessageChannel Channel)
        {
            using (db _db = new())
            {
                var Message = await CachedMessage.GetOrDownloadAsync();
                if (Message == null || Message.Author.IsBot || Message.Content.Length > 1023 || MessageNow.Content.Length > 1023 || !BotSettings.BotReady)
                    return;

                if (Channel is SocketTextChannel GuildTextChannel)
                {
                    var Guild_Log = _db.Guilds_Logs.GetLogChannel(GuildTextChannel.Guild.Id, ChannelsTypeEnum.MessageEdit);
                    if (Guild_Log != null)
                    {
                        var ChannelMessageUpdate = GuildTextChannel.Guild.GetTextChannel(Guild_Log.ChannelId);
                        if (ChannelMessageUpdate != null)
                        {
                            var emb = new EmbedBuilder().WithColor(255, 0, 94).WithAuthor("Сообщение изменено", Message.Author.GetAvatarUrl());
                            emb.AddField("Прошлое", string.IsNullOrWhiteSpace(Message.Content) ? "-" : Message.Content);
                            emb.AddField("Новое", string.IsNullOrWhiteSpace(MessageNow.Content) ? "-" : MessageNow.Content);
                            emb.AddField("Канал", $"<#{Message.Channel.Id}>", true);
                            emb.AddField("Отправитель", MessageNow.Author.Mention, true);
                            if (MessageNow.Attachments.Count > 0)
                                emb.WithImageUrl(MessageNow.Attachments.FirstOrDefault().Url);
                            emb.WithTimestamp(DateTime.Now.ToUniversalTime());

                            await ChannelMessageUpdate.Message("", emb);
                        }
                    }
                }
            }
        }

        private async Task SendMessage(SocketMessage message)
        {
            using (db _db = new())
            {
                if (message is not SocketUserMessage UserMessage || 
                    message.Author.IsBot ||
                    !BotSettings.BotReady)
                    return;

                if (message.Channel is IDMChannel DMChannel)
                {
                    //bool Command = true;
                    //int argPos = 0;
                    //var Darling = _db.Darling.FirstOrDefault(x => x.UsersId == UserMessage.Author.Id);
                    //if(Darling != null)
                    //{
                    //    if (Darling.State == State.CreatePhoto || Darling.State == State.EditPhoto)
                    //    {
                    //            if (string.IsNullOrWhiteSpace(UserMessage.Content))
                    //            {
                    //                if (UserMessage.Embeds.Count > 0 && !UserMessage.Embeds.Any(x => x.Image != null || x.Type == EmbedType.Image))
                    //                    Darling.LastMessage = UserMessage.Embeds.FirstOrDefault(x => x.Type == EmbedType.Image).Url;
                    //            }
                    //            else
                    //                Darling.LastMessage = UserMessage.Content;

                    //        Command = false;
                    //    }
                    //    else if (Darling.State == State.EditName ||
                    //        Darling.State == State.EditDescription ||
                    //        Darling.State == State.EditHobby ||
                    //        Darling.State == State.EditPhoto ||
                    //        Darling.State == State.EditYear ||
                    //        Darling.State == State.CreateDescription ||
                    //        Darling.State == State.CreateHobby ||
                    //        Darling.State == State.CreateName ||
                    //        Darling.State == State.CreateYear ||
                    //        Darling.State == State.CreateSexPartner ||
                    //        Darling.State == State.CreateSex)
                    //    {
                    //        Command = false;
                    //        Darling.LastMessage = UserMessage.Content;
                    //    }
                    //    _db.Darling.Update(Darling);
                    //    await _db.SaveChangesAsync();
                    //}
                

                    //if (Command || UserMessage.Content.ToLower() == "start")
                    //{
                    //    var Context = new ShardedCommandContext(_discord, UserMessage);
                    //    await _commands.ExecuteAsync(Context, argPos, _provider);
                    //}
                    //else
                    //    await FindDarling.StationGenerate(UserMessage.Author);
                }
                else
                {

                    var User = message.Author as SocketGuildUser;
                    var Context = new ShardedCommandContext(_discord, UserMessage);
                    var GuildDB = await _db.Guilds.GetOrCreate(Context.Guild.Id);
                    var UserGuild = await _db.Users_Guild.GetOrCreate(UserMessage.Author.Id, Context.Guild.Id);
                    var ChannelDB = await _db.Channels.GetOrCreate(Context.Guild.Id, Context.Channel.Id);
                    await UpdateOnlineDate(User); // Обновление статуса пользователя
                    await CapthaService.CaptcaAuth(UserMessage); // Проверка капчи

                    if (await UserMessageScanning.ChatSystem(Context, ChannelDB, GuildDB.Prefix, UserGuild.Id))
                        return;

                    var NullInfo = string.IsNullOrWhiteSpace(UserGuild.Users.BlockedReason);
                    if (NullInfo || UserMessage.Content.StartsWith(GuildDB.Prefix + "boost"))
                    {
                        bool NotRight = false;
                        if (ChannelDB.UseCommand || ChannelDB.UseRPcommand || ChannelDB.UseAdminCommand) // Возможность использовать команды в этом канале
                        {
                            if (UserMessage.MentionedUsers.Any(x => x.IsBot)) // проверка на белый список команды, при котором можно упоминать бота.
                            {
                                foreach (var Command in BotSettings.CommandsForBot)
                                {
                                    if (!UserMessage.Content.StartsWith($"{GuildDB.Prefix}{Command}"))
                                        NotRight = true;
                                }
                            }

                            if (!NotRight)
                            {
                                if (!ChannelDB.UseCommand && !string.IsNullOrWhiteSpace(UserMessage.Content))
                                {
                                    if (ChannelDB.UseRPcommand && !CheckCommands(GuildDB.Prefix, UserMessage.Content, "SfwGif")) // Проверка на RP команды
                                        NotRight = true;
                                    else if (ChannelDB.UseAdminCommand && !CheckCommands(GuildDB.Prefix, UserMessage.Content, "Admins")) // Проверка на Админ команды
                                        NotRight = true;
                                }


                                int argPos = 0;
                                if (UserMessage.HasStringPrefix(GuildDB.Prefix, ref argPos) && !NotRight && !CommandSpam.Checking(Context)) // Проверка команды
                                    await _commands.ExecuteAsync(Context, argPos, _provider);
                            }
                        }

                        if (ChannelDB.GiveXP)
                            await Leveling.LVL(UserMessage, UserGuild);
                    }
                    else
                    {
                        var emb = new EmbedBuilder().WithColor(Discord.Color.Red).WithAuthor("Вы заблокированы!")
                                                    .WithDescription($"Вы были заблокированы за препядствие работы бота.\nПричина:{UserGuild.Users.BlockedReason}\n\nСнять блокировку: {GuildDB.Prefix}boost");
                        await Context.Channel.Message("", emb);
                    }
                }
            }
        }
        private bool CheckCommands(string GuildDBPrefix, string UserMessage, string CommandName)
        {
            var commands = _commands.Modules.FirstOrDefault(x => x.Name == CommandName)?.Commands;
            foreach (var command in commands)
            {
                if (UserMessage.StartsWith($"{GuildDBPrefix}{command.Aliases[0]}") ||
                    UserMessage.StartsWith($"{GuildDBPrefix}{command.Aliases[command.Aliases.Count - 1]}"))
                    return true;
            }
            return false;
        }
    }
}
