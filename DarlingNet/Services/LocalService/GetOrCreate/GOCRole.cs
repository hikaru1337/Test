using DarlingDb.Models;
using DarlingDb;
using System.Linq;
using System.Threading.Tasks;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;

namespace DarlingNet.Services.LocalService.GetOrCreate
{
    public static class GOCRole
    {
        public static Task<Role> GetOrCreate(this DbSet<Role> Roles, ulong RoleId, ulong GuildId) => GetOrCreateRole(Roles, RoleId, GuildId);

        public static Task<Role> GetOrCreate(this DbSet<Role> Roles, SocketRole Role) => GetOrCreateRole(Roles, Role.Id, Role.Guild.Id);

        private static async Task<Role> GetOrCreateRole(DbSet<Role> Roles,ulong RoleId, ulong GuildId)
        {
            using (db _db = new ())
            {
                var gg = Roles.FirstOrDefault(u => u.Id == RoleId && u.GuildsId == GuildId);
                if (gg == null)
                {
                    gg = new Role() { GuildsId = GuildId, Id = RoleId };
                    _db.Role.Add(gg);
                    await _db.SaveChangesAsync();
                }
                return gg;
            }
        }
    }
}
