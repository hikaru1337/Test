using System;

namespace DarlingDb.Models
{
    public class Roles_Timer
    {
        public uint Id { get; set; }
        public ulong RoleId { get; set; }
        public Role Role { get; set; }
        public uint Users_GuildId { get; set; }
        public Users_Guild Users_Guild { get; set; }
        public DateTime ToTime { get; set; }

    }
}
