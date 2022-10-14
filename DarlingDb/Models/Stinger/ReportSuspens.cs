using System;

namespace DarlingDb.Models.Stinger
{
    public class ReportSuspens
    {
        public ulong Id { get; set; }
        public string Comment { get; set; }
        public ulong? AdministratorId { get; set; }
        public Users Administrator { get; set; }
        public ulong SuspensId { get; set; }
        public Suspens Suspens { get; set; }
        public bool Automatic { get; set; }
        public DateTime Time { get; set; }
        public Report TypeReport { get; set; }
        public enum Report : byte
        {
            none,
            Ban,          // :2         // :2 / 30? plus
            Kick,         // 1000 ball  // 1000 / 7 plus
            Mute,         // 100 ball   // 100 / 7  plus
            timeOut,      // 10 ball    // 10 / 7 plus
            TimeBan,      // 100 ball   // 100 / 7
            SpamSystem,   // 10 ball    // 10 / 3
            OtherReport,  // 1 ball     // 1 / 3
            
        }
    }
}
