using static DarlingDb.Enums;

namespace DarlingDb.Models
{
    public class Guilds_Logs
    {
        public uint Id { get; set; }
        public ulong ChannelId { get; set; }
        public Channel Channel { get;set; }
        public ChannelsTypeEnum Type { get; set; }

    }
}
