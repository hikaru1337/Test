using DarlingDb;
using DarlingDb.Models;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using static DarlingDb.Enums;

namespace DarlingNet.Services.LocalService.GetOrCreate
{
    public static class GOCTempMute
    {
        public static TempUser GetValidateMute(this SocketGuildUser User)
        {
            using (db _db = new ())
            {
                return _db.TempUser.Include(x => x.Users_Guild).Where(x => x.Users_Guild.UsersId == User.Id && x.Users_Guild.GuildsId == User.Id).AsEnumerable().FirstOrDefault(x => x.Reason == ReportTypeEnum.Mute || x.Reason == ReportTypeEnum.TimeOut);
            }
        }
    }
}
