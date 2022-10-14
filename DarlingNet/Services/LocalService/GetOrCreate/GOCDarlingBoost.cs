using DarlingDb.Models;
using DarlingDb;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System;

namespace DarlingNet.Services.LocalService.GetOrCreate
{
    public static class GOCDarlingBoost
    {
        public static Task<DarlingBoost> GetOrCreate(this DbSet<DarlingBoost> Us, ulong UsersId) 
            => DarlingBoostGetOrCreate(UsersId);

        private static async Task<DarlingBoost> DarlingBoostGetOrCreate(ulong UsersId)
        {
            using (db _db = new ())
            {
                DarlingBoost Boost = null;
                var User = _db.Users.Include(x => x.Boost).FirstOrDefault(x => x.Id == UsersId);
                if (User != null)
                {
                    if (User.Boost == null)
                        await BoostCreate();
                    else
                        Boost = User.Boost;
                }
                else
                {
                    await _db.Users.GetOrCreate(UsersId);
                    await BoostCreate();
                }

                async Task BoostCreate()
                {
                    Boost = new DarlingBoost { UsersId = UsersId };
                    if (UsersId == BotSettings.hikaruid)
                        Boost.Ends = DateTime.MaxValue;
                    _db.DarlingBoost.Add(Boost);
                    await _db.SaveChangesAsync();
                }

                return Boost;
            }
        }
    }
}
