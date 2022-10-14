using System;
using static DarlingDb.Enums;

namespace DarlingDb.Models
{
    public class TempUser
    {
        public uint Id { get; set; }
        public uint Users_GuildId { get; set; }
        public Users_Guild Users_Guild { get; set; }
        public DateTime ToTime { get; set; }
        public ReportTypeEnum Reason { get; set; }
    }
}
