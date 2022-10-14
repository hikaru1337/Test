using System;
using static DarlingDb.Enums;

namespace DarlingDb.Models
{
    public class Guilds_Warns
    {
        public uint Id { get; set; }
        public ulong GuildsId { get; set; }
        public Guilds Guilds { get; set; }
        public byte CountWarn { get; set; }
        public TimeSpan Time { get; set; }
        public ReportTypeEnum ReportTypes { get; set; }


    }
}
