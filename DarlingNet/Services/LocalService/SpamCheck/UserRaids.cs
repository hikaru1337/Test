using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DarlingNet.Services.LocalService.SpamCheck
{
    public static class UserRaids
    {

        private static readonly List<DosStructure> UserRaidList = new();

        public static IEnumerable<DosStructure> CheckRaid(this SocketGuildUser User,uint RaidTime)
        {
            _ = UserRaidList.RemoveAll(x => (DateTime.Now - x.Time).TotalSeconds >= RaidTime);
            UserRaidList.Add(new DosStructure
            {
                UsersId = User.Id,
                GuildsId = User.Guild.Id,
                Time = DateTime.Now,
            });
            return UserRaidList.Where(x => x.UsersId == User.Id && x.GuildsId == User.Guild.Id);
        }
    }
}
