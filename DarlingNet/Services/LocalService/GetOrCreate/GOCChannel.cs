using DarlingDb.Models;
using DarlingDb;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;

namespace DarlingNet.Services.LocalService.GetOrCreate
{
    public static class GOCChannel
    {
        public static Task<Channel> GetOrCreate(this DbSet<Channel> Channels, ulong GuildsId, ulong Id) 
            => ChannelGetOrCreate(Channels, GuildsId, Id);
        public static Task<Channel> GetOrCreate(this DbSet<Channel> Channels, SocketTextChannel Channel) 
            => ChannelGetOrCreate(Channels, Channel.Guild.Id, Channel.Id);

        public static async Task<Channel> ChannelGetOrCreate(DbSet<Channel> Channels, ulong GuildsId, ulong Id)
        {
            using (db _db = new ())
            {
                var gg = Channels.FirstOrDefault(u => u.Id == Id);
                if (gg == null)
                {
                    gg = new Channel() { GuildsId = GuildsId, Id = Id, UseCommand = true, UseAdminCommand = true,UseRPcommand = true };
                    _db.Add(gg);
                    await _db.SaveChangesAsync();
                }
                return gg;
            }
        }
    }
}
