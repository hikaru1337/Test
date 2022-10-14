
using DarlingDb.Models;
using System.Threading.Tasks;

namespace DarlingWeb.Data
{
    public interface IGuildService
    {
        Task<Guilds> GetGuild(ulong id);
        Task UpdateGuild(Guilds game, ulong id);
    }
}
