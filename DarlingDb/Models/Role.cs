using DarlingDb.Models;
using System.Collections.Generic;

namespace DarlingDb
{
    public class Role
    {
        public ulong Id { get; set; }
        public ulong GuildsId { get; set; }
        public Guilds Guilds { get; set; }
        public ICollection<Roles> Roles { get; set; }
        public ICollection<Roles_Timer> Roles_Timer { get; set; }
        public ICollection<ButtonClick> ButtonClick { get; set; }
        public ICollection<EmoteClick> EmoteClick { get; set; }
        public Guilds_Captcha Guilds_Captcha { get; set; }
        public Guilds_Meeting_Welcome Guilds_Meeting_Welcome { get; set; }
    }
}
