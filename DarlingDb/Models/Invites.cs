namespace DarlingDb.Models
{
    public class Invites
    {
        public uint Id { get; set; }
        public string Key { get; set; }
        public uint Users_GuildId { get; set; }
        public Users_Guild Users_Guild { get; set; }
        public ulong ChannelId { get; set; }
        public Channel Channel { get; set; }
        //public DateTime Outdated { get; set; }
        public int UsesCount { get; set; }
    }
}
