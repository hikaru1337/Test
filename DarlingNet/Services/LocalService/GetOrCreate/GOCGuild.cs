using DarlingDb.Models;
using DarlingDb;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace DarlingNet.Services.LocalService.GetOrCreate
{
    public static class GOCGuild
    {
        public static async Task<Guilds> GetOrCreate(this DbSet<Guilds> Guilds, ulong GuildsId)
        {
            using (db _db = new())
            {
                var gg = Guilds.FirstOrDefault(u => u.Id == GuildsId);
                if (gg == null)
                {
                    gg = new Guilds() { Id = GuildsId, Prefix = BotSettings.Prefix };
                    _db.Add(gg);
                    await _db.SaveChangesAsync();
                }
                else if (gg.Leaved)
                {
                    gg.Leaved = false;
                    _db.Guilds.Update(gg);
                    await _db.SaveChangesAsync();
                }

                return gg;
            }
        }

        public static async Task<string> GetPrefix(this DbSet<Guilds> Guilds, ulong GuildsId)
        {
            using (db _db = new ())
            {
                var Prefix = Guilds.FromSqlRaw($"SELECT Prefix FROM Guilds WHERE Id = {GuildsId}").Select(x=>x.Prefix).First();
                if (Prefix == null)
                {
                    await GetOrCreate(Guilds,GuildsId);
                    Prefix = BotSettings.Prefix;
                }
                return Prefix;
            }
        }
    }
}
