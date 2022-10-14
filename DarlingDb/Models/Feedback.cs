using System;
using static DarlingDb.Enums;

namespace DarlingDb.Models
{
    public class Feedback
    {
        public uint Id { get; set; }
        public ulong UserId { get; set; }
        public Users User { get; set; }
        public string Message { get; set; }
        public string AdminMessage { get; set; }
        public DateTime Time { get; set; }
        public StatusTicketEnum Status { get; set; }
    }
}
