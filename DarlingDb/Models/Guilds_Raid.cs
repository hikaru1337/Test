using System;

namespace DarlingDb.Models
{
    public class Guilds_Raid
    {
        public uint Id { get; set; }
        public ulong GuildsId { get; set; }
        public Guilds Guilds { get; set; }
        public bool RaidRunning { get; set; }
        public byte RaidTime { get; set; }
        public byte RaidUserCount { get; set; }
        public DateTime RaidMuted { get; set; }
    }
}
