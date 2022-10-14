namespace DarlingDb.Models
{
    public class Guilds_Captcha
    {
        public uint Id { get; set; }
        public ulong GuildId { get; set; }
        public Guilds Guild { get; set; }
        public ulong? ChannelId { get; set; }
        public Channel Channel { get; set; }
        public ulong? RoleId { get; set; }
        public Role Role { get; set; }
        public bool Run { get; set; }
    }
}
