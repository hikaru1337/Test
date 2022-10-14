

using static DarlingDb.Enums;

namespace DarlingDb.Models
{
    public class Roles
    {
        public uint Id { get; set; }
        public ulong RoleId { get; set; }
        public Role Role { get; set; }
        public string Value { get; set; }
        public RoleTypeEnum Type { get; set; }
    }
}
