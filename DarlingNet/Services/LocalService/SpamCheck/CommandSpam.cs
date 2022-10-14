using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DarlingNet.Services.LocalService.SpamCheck
{
    public class CommandSpam
    {
        private static readonly List<DosStructure> UserBlockSpam = new();

        public static bool Checking(ShardedCommandContext Context)
        {
            _ = UserBlockSpam.RemoveAll(x => (DateTime.Now - x.Time).TotalSeconds >= 1.5);
            if (UserBlockSpam.Any(x => x.UsersId == Context.User.Id && x.GuildsId == Context.Guild.Id))
                return true;
            
            UserBlockSpam.Add(new DosStructure() { UsersId = Context.User.Id, GuildsId = Context.Guild.Id, Time = DateTime.Now });
            return false;
        }
    }
}
