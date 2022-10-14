using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DarlingNet.Services.LocalService.SpamCheck
{
    public class PrivateSpam
    {
        private static readonly List<DosStructure> userdos = new();

        public static bool CheckSpamPrivate(SocketGuildUser User)
        {
            _ = userdos.RemoveAll(x => (DateTime.Now - x.Time).TotalSeconds > 10);
            if(userdos.Any(x => x.UsersId == User.Id && x.GuildsId == User.Guild.Id))
                return true;

            return false;
        }

        public static void AddUser(SocketGuildUser User)
            => userdos.Add(new DosStructure() { UsersId = User.Id, GuildsId = User.Guild.Id, Time = DateTime.Now });
    }
}
