using System;
using System.Collections.Generic;
using System.Linq;

namespace DarlingDb.Models.Stinger
{
    public class Suspens
    {
        public ulong Id { get; set; }
        public ulong UserId { get; set; }
        public ICollection<Users> Users { get; set; }
        public ICollection<ReportSuspens> Reports { get; set; }
        public double UserBall
        {
            get
            {
                double Ball = 10000;
                ulong CountDays = 0;
                foreach (var Report in Reports.Where(x => (DateTime.Now - x.Time).TotalDays < 30)) // Тут будет ошибка
                {
                    switch (Report.TypeReport)
                    {
                        case ReportSuspens.Report.Ban:
                            Ball /= 2;
                            CountDays = 30;
                            break;
                        case ReportSuspens.Report.Kick:
                            Ball -= 1000;
                            CountDays = 7;
                            break;
                        case ReportSuspens.Report.Mute:
                            Ball -= 100;
                            CountDays = 7;
                            break;
                        case ReportSuspens.Report.timeOut:
                            Ball -= 10;
                            CountDays = 7;
                            break;
                        case ReportSuspens.Report.TimeBan:
                            Ball -= 100;
                            CountDays = 7;
                            break;
                        case ReportSuspens.Report.SpamSystem:
                            Ball -= 10;
                            CountDays = 3;
                            break;
                        case ReportSuspens.Report.OtherReport:
                            Ball -= 1;
                            CountDays = 3;
                            break;
                    }
                    Ball -= (Ball / CountDays) * (DateTime.Now - Report.Time).TotalDays;
                }
                if (Ball < 0)
                    Ball = 0;
                return Ball;
            }
        }
    }
}
