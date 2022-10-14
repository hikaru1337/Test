using DarlingDb.Models;
using DarlingDb;
using Discord;
using Discord.WebSocket;
using System.Linq;
using System.Threading.Tasks;
using Discord.Rest;
using System;
using DarlingNet.Services.LocalService.SpamCheck;
using DarlingNet.Services.LocalService.GetOrCreate;
using Microsoft.EntityFrameworkCore;

namespace DarlingNet.Services.LocalService
{
    public class PrivateSystem
    {
        public static async Task CheckPrivate(SocketGuild Guild)
        {
            using (db _db = new ())
            {
                var prv = _db.PrivateChannels.Include(x=>x.Users_Guild).Where(x => x.Users_Guild.GuildsId == Guild.Id);
                foreach (var PC in prv) // Проверка Приваток
                {
                    var chnl = Guild.GetVoiceChannel(PC.Id);
                    if(chnl != null)
                        await Privatemethod(chnl,PC);
                    else
                    {
                        _db.PrivateChannels.Remove(PC);
                        await _db.SaveChangesAsync();
                    }
                }
            }
        }

        public static async Task Privatemethod(SocketVoiceChannel VoiceChannel, PrivateChannels ThisPrivateChannel)
        {
            using (db _db = new ())
            {
                var BotPermission = VoiceChannel.Guild.CurrentUser.GuildPermissions;

                if (BotPermission.Administrator || BotPermission.ManageChannels)
                {
                    if (VoiceChannel.Users.Count == 0)
                    {
                        await VoiceChannel.DeleteAsync();
                        _db.PrivateChannels.Remove(ThisPrivateChannel);
                        await _db.SaveChangesAsync();
                    }
                    else if (!VoiceChannel.Users.Any(x => x.Id == ThisPrivateChannel.Users_Guild.UsersId))
                    {
                        var newusr = VoiceChannel.Users.FirstOrDefault();
                        var newusrDb = _db.Users.Include(x => x.Users_Guild).FirstOrDefault(x => x.Id == newusr.Id);

                        var oldusr = VoiceChannel.GetUser(ThisPrivateChannel.Users_Guild.UsersId);
                        ThisPrivateChannel.Users_GuildId = newusrDb.Users_Guild.FirstOrDefault(x=>x.GuildsId == VoiceChannel.Guild.Id).Id;
                        _db.PrivateChannels.Update(ThisPrivateChannel);
                        await _db.SaveChangesAsync();
                        await VoiceChannel.RemovePermissionOverwriteAsync(oldusr);
                        await VoiceChannel.AddPermissionOverwriteAsync(newusr, Permission);
                    }
                    
                }
                
            }
        }

        private static readonly OverwritePermissions Permission = new (connect: PermValue.Allow, muteMembers: PermValue.Allow, deafenMembers: PermValue.Allow,moveMembers: PermValue.Allow,manageChannel: PermValue.Allow);

        public static async Task PrivateCreate(SocketGuildUser user, SocketVoiceChannel PrivateChannel)
        {
            using (db _db = new ())
            {
                if (!PrivateSpam.CheckSpamPrivate(user))
                {
                    var BotPerm = user.Guild.CurrentUser.GuildPermissions;
                    if (BotPerm.Administrator || BotPerm.ManageChannels)
                    {
                        PrivateSpam.AddUser(user);
                        if (PrivateChannel.Category == null)
                        {
                            RestCategoryChannel cat = await user.Guild.CreateCategoryChannelAsync("DARLING PRIVATE");
                            await PrivateChannel.ModifyAsync(x => x.CategoryId = cat.Id);
                        }
                        RestVoiceChannel voicechannel = null;
                        try
                        {
                            voicechannel = await user.Guild.CreateVoiceChannelAsync($"{user}` VOICE", x => x.CategoryId = PrivateChannel.CategoryId);
                            await user.ModifyAsync(x => x.Channel = voicechannel);
                            await voicechannel.AddPermissionOverwriteAsync(user, Permission);
                            var UserDb = await _db.Users_Guild.GetOrCreate(user.Id, user.Guild.Id);
                            _db.PrivateChannels.Add(new PrivateChannels() { Users_GuildId = UserDb.Id, Id = voicechannel.Id });
                            await _db.SaveChangesAsync();
                        }
                        catch (Exception)
                        {
                            await voicechannel?.DeleteAsync();
                        }
                    }
                }
            }
        }
    }
}
