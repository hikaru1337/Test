using System;

namespace DarlingDb.Models
{
    public class GiveAways
    {
        public ulong Id { get; set; }
        public ulong ChannelId { get; set; }
        public Channel Channel { get; set; }
        public DateTime Times { get; set; }
        public string Surpice { get; set; }
        public uint WinnerCount { get; set; }
    }
}