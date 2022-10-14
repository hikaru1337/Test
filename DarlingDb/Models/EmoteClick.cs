namespace DarlingDb.Models
{
    public class EmoteClick
    {
        public uint Id { get; set; }
        public string Emote { get; set; }
        public ulong MessageId { get; set; }
        public ulong ChannelId { get; set; }
        public Channel Channel { get; set; }
        public ulong RoleId { get; set; }
        public Role Role { get; set; }
        public bool Get { get; set; }
    }
}
