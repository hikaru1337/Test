using DarlingDb;
using DarlingDb.Models;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace DarlingWeb.Data
{
    public class GuildService : IGuildService
    {
        //private readonly db _context;
        private readonly NavigationManager _navigationManager;

        public GuildService(/*db context,*/ NavigationManager navigationManager)
        {
        //    _context = context;
            _navigationManager = navigationManager;
        //    _context.Database.EnsureCreated();
        }

        public async Task<Guilds> GetGuild(ulong id)
        {
            using (var _db = new db())
            {
                var Guild = await _db.Guilds.FindAsync(id);
                if (Guild == null)
                {
                    Guild = _db.Guilds.Add(new Guilds { Id = id, Prefix = "h." }).Entity;
                    await _db.SaveChangesAsync();
                }
                return Guild;
            }

        }

        public async Task UpdateGuild(Guilds Guild, ulong id)
        {
            using (var _db = new db())
            {
                var dbGame = await _db.Guilds.FindAsync(id);
                dbGame.Prefix = Guild.Prefix;

                await _db.SaveChangesAsync();
                _navigationManager.NavigateTo("/");
            }
        }
    }
}
