using Discord.WebSocket;
using System;
using System.Threading.Tasks;

namespace DarlingNet.Services.LocalService.VerifiedAction
{
    internal static class MuteCheck
    {
        public static Task<string> AddMute(this SocketGuildUser User, TimeSpan Time)
            => CheckMute(User, true, Time);

        public static Task<string> DelMute(this SocketGuildUser User)
            => CheckMute(User, false);

        private static async Task<string> CheckMute(SocketGuildUser User, bool Add, TimeSpan Time = new TimeSpan())
        {
            string Error = string.Empty;
            if (User != null)
            {
                var Permission = User.Guild.CurrentUser.GuildPermissions;
                if (Permission.Administrator || Permission.MuteMembers)
                {
                    if (Add)
                    {
                        if (User.Guild.CurrentUser.Hierarchy >= User.Hierarchy)
                            await User.SetTimeOutAsync(Time);
                        else
                            Error = "Роль пользователя, которого вы хотите замутить, выше роли бота!";
                    }
                    else
                    {
                        if (User.TimedOutUntil != null)
                            await User.RemoveTimeOutAsync();
                        else
                            Error = "Пользователь не находится в муте!";
                    }
                }
                else
                    Error = "У бота нет прав мутить пользователя!";
            }
            else
                Error = "Пользователь не найден!";


            return Error;
        }
    }
}
