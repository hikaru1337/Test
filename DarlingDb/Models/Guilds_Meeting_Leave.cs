namespace DarlingDb.Models
{
    public class Guilds_Meeting_Leave
    {
        public uint Id { get; set; }
        public ulong GuildsId { get; set; }
        public Guilds Guilds { get; set; }
        public string LeaveMessage { get; set; }
        public ulong LeaveChannelId { get; set; }
        public Channel LeaveChannel { get; set; }
    }
}
