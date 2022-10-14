using System.Collections.Generic;

namespace DarlingDb.Models.ReportSystem
{
    public class Reports
    {
        public ulong Id { get; set; }
        public string Rules { get; set; }
        public ulong GuildsId { get; set; }
        public Guilds Guilds { get; set; }
        public ICollection<Reports_Punishes> ReportsList { get; set; }
    }
}
