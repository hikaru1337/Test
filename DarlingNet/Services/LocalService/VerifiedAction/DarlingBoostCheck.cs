using Discord;
using Discord.WebSocket;
using DarlingDb;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace DarlingNet.Services.LocalService.VerifiedAction
{
    public static class DarlingBoostCheck
    {
        public static EmbedBuilder DarlingBoostGet(this SocketGuild Guild)
        {
            using (db _db = new ())
            {
                var emb = new EmbedBuilder().WithColor(Color.Red).WithAuthor("DarlingBoost отсутствует");
                var DarlingBoost = _db.Users.Include(x => x.Boost).FirstOrDefault(x => x.Id == Guild.OwnerId);
                if (DarlingBoost != null && DarlingBoost.Boost != null && DarlingBoost.Boost.Active)
                    emb.WithDescription("Для использования системы, владелец сервера должен приобрести [DarlingBoost](https://docs.darlingbot.ru/commands/darling-boost)");
                else
                    emb = null;

                return emb;
            }
        }
    }
}
