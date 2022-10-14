using Discord;
using Discord.WebSocket;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DarlingNet.Services.LocalService.VerifiedAction
{
    public static class RoleCheck
    {
        public static string RolePermission(this SocketRole ThisRole)
        {
            string Description = null;
            var rolepos = ThisRole.Guild.CurrentUser.Roles.FirstOrDefault(x => x.Position > ThisRole.Position);
            if (rolepos != null && ThisRole.IsManaged)
                Description = "Роль бота или Boost, нельзя сделать для выдачи!";
            else
                Description = "Роль бота ниже этой роли, из-за чего бот не сможет выдавать ее.\nПоднимите роль бота выше выдаваемой роли.";

            return Description;
        }

        public static Task<string> AddRole(this SocketGuildUser User, ulong RoleId)
            => CheckRole(User, RoleId, true);

        public static Task<string> RemoveRole(this SocketGuildUser User, ulong RoleId)
            => CheckRole(User, RoleId, false);

        public static Task<string> AddRole(this SocketUser User, ulong RoleId)
            => CheckRole(User as SocketGuildUser, RoleId, true);

        public static Task<string> RemoveRole(this SocketUser User, ulong RoleId)
            => CheckRole(User as SocketGuildUser, RoleId, false);

        private static async Task<string> CheckRole(SocketGuildUser User, ulong RoleId, bool Add)
        {
            string Error = "Роль не найдена на сервере!";
            var DiscordRole = User.Guild.GetRole(RoleId);
            if (DiscordRole != null)
            {
                Error = RolePermission(DiscordRole);
                if (Error == null)
                {
                    var bot = User.Guild.CurrentUser;
                    if (bot.GuildPermissions.ManageRoles || bot.GuildPermissions.Administrator)
                    {
                        if (!Add && User.Roles.Contains(DiscordRole))
                            await User.RemoveRoleAsync(DiscordRole);
                        else if (Add && !User.Roles.Contains(DiscordRole))
                            await User.AddRoleAsync(DiscordRole);
                    }
                    else
                        Error = "Бот не имеет прав выдавать роли!";
                }
            }
            return Error;
        }

    }
}
