namespace DarlingDb.Models
{
    public class PrivateChannels
    {
        public ulong Id { get; set; }
        public uint Users_GuildId { get; set; }
        public Users_Guild Users_Guild { get; set; }
    }
}
