using Discord.WebSocket;
using System.Threading.Tasks;

namespace DarlingNet.Services.LocalService.VerifiedAction
{
    public static class KickCheck
    {
        public static Task<string> AddKick(this SocketGuildUser User, string Reason = null)
            => CheckKick(User, Reason);

        private static async Task<string> CheckKick(SocketGuildUser User, string Reason = null)
        {
            string Error = string.Empty;
            var Permission = User.Guild.CurrentUser.GuildPermissions;
            if (Permission.KickMembers || Permission.Administrator)
            {
                if (User.Guild.CurrentUser.Hierarchy >= User.Hierarchy)
                    await User.KickAsync(Reason);
                else
                    Error = "Роль пользователя, которого вы хотите кикнуть, выше роли бота!";
            }
            else
                Error = "У бота нет прав кикать пользователя!";

            return Error;
        }
    }
}
