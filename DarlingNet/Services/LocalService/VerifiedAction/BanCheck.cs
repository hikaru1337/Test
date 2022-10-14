using Discord.WebSocket;
using System.Threading.Tasks;

namespace DarlingNet.Services.LocalService.VerifiedAction
{
    public static class BanCheck
    {
        public static Task<string> AddBan(this SocketGuild Guild, ulong UsersId, int MessageDelete, string Reason)
            => CheckBan(Guild, UsersId, true, MessageDelete, Reason);

        public static Task<string> RemoveBan(this SocketGuild Guild, ulong UsersId)
            => CheckBan(Guild, UsersId, false);

        private static async Task<string> CheckBan(SocketGuild Guild, ulong UsersId, bool Add, int MessageDelete = 0, string Reason = null)
        {
            string Error = string.Empty;
            var Permission = Guild.CurrentUser.GuildPermissions;
            if (Permission.Administrator || Permission.BanMembers)
            {
                if (Add)
                {
                    var User = Guild.GetUser(UsersId);
                    if (User != null)
                    {
                        if (Guild.CurrentUser.Hierarchy >= User.Hierarchy)
                            await AddBan();
                        else
                            Error = "Роль пользователя, которого вы хотите забанить, выше роли бота!";
                    }
                    else
                        await AddBan();

                    async Task AddBan()
                        => await Guild.AddBanAsync(UsersId, MessageDelete, Reason);
                }
                else
                {
                    var Bans = await Guild.GetBanAsync(UsersId);
                    if (Bans != null)
                        await Guild.RemoveBanAsync(Bans.User);
                    else
                        Error = "Пользователь не находится в бане!";
                }
            }
            else
                Error = "У бота нет прав банить пользователя!";

            return Error;
        }
    }
}
