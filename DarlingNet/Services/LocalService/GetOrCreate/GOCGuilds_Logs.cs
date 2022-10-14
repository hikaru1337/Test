using DarlingDb;
using DarlingDb.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using static DarlingDb.Enums;

namespace DarlingNet.Services.LocalService.GetOrCreate
{
    public static class GOCGuilds_Logs
    {
        public static Guilds_Logs GetLogChannel(this IQueryable<Guilds_Logs> Guilds_Logs,ulong GuildId, ChannelsTypeEnum TypeX)
        {
            return Guilds_Logs.Include(x => x.Channel).FirstOrDefault(x => x.Type == TypeX && x.Channel.GuildsId == GuildId);
        }

        public static IEnumerable<Guilds_Logs> GetLogChannel(this IQueryable<Guilds_Logs> Guilds_Logs, ulong GuildId)
        {
            return Guilds_Logs.Include(x => x.Channel).Where(x => x.Channel.GuildsId == GuildId).AsEnumerable();
        }
    }
}
