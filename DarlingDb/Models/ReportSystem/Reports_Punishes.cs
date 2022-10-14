using System;
using System.Collections.Generic;
using static DarlingDb.Enums;

namespace DarlingDb.Models.ReportSystem
{
    public class Reports_Punishes
    {
        public ulong Id { get; set; }
        public ulong ReportId { get; set; }
        public Reports Report { get; set; }
        public ReportTypeEnum TypeReport { get; set; }
        public TimeSpan TimeReport { get; set; }
        public ICollection<Users_Guild> Users_Guild { get; set; }
    }
}
