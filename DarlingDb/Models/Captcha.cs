using System;

namespace DarlingDb.Models
{
    public class Captcha
    {
        public uint Id { get; set; }
        public ulong UserId { get; set; }
        public ulong GuildId { get; set; }
        public Guilds Guild { get; set; }
        public uint Result { get; set; }
        public DateTime TimeToClear { get; set; }
    }
}
