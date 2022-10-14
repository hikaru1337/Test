namespace DarlingDb.Models
{
    public class ButtonClick
    {
        public uint Id { get; set; }
        public ulong MessageId { get; set; }
        public string Value { get; set; }
        public ulong ChannelId { get; set; }
        public Channel Channel { get; set; }
        public ulong? RoleId { get; set; }
        public Role Role { get; set; }
        public bool RoleDelOrGet { get; set; }
        public bool Counter { get; set; }
    }
}
