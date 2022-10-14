using System;

namespace DarlingDb.Models
{
    public class Tasks
    {
        public uint Id { get; set; }
        public ulong ChannelId { get; set; }
        public Channel Channel { get; set; }
        public string Message { get; set; }
        public DateTime Times { get; set; }
        public bool Repeat { get; set; }
    }
}
