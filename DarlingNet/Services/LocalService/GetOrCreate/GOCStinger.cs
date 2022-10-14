//using DarlingDb.Models.Stinger;
//using Discord.WebSocket;
//using Microsoft.EntityFrameworkCore;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using DarlingDb;
//using static DarlingDb.Models.Stinger.ReportSuspens;

//namespace DarlingNet.Services.LocalService.GetOrCreate
//{
//    public static class GOCStinger
//    {
//        public static async Task AddReport(this DbSet<Suspens> Us, ulong AdminId, ulong SuspensId, Report Type)
//        {
//            await ReportGetOrCreate(AdminId, SuspensId, Type);
//        }

//        public static async Task ReportGetOrCreate(ulong AdminId, ulong SuspensId, Report Type)
//        {
//            using (DBcontext _db = new())
//            {
//                var Suspens = SuspensGetOrCreate(SuspensId);
//                var NewReport = new ReportSuspens { AdministratorId = AdminId, SuspensId = SuspensId, Time = DateTime.Now, TypeReport = Type, Automatic = AdminId == 0 ? true : false };
//                _db.ReportSuspens.Add(NewReport);
//                await _db.SaveChangesAsync();
//            }
//        }

//        public static Task<Suspens> GetOrCreate(this DbSet<Suspens> Us, ulong Id)
//        {
//            return SuspensGetOrCreate(Id);
//        }
//        public static Task<Suspens> GetOrCreate(this DbSet<Suspens> Us, SocketUser User)
//        {
//            return SuspensGetOrCreate(User.Id);
//        }

//        public static async Task<Suspens> SuspensGetOrCreate(ulong Id)
//        {
//            using (DBcontext _db = new())
//            {
//                var Suspens = _db.Suspens.FirstOrDefault(x => x.UserId == Id);
//                if (Suspens == null)
//                {
//                    Suspens = new Suspens() { UserId = Id };
//                    _db.Suspens.Add(Suspens);
//                    await _db.SaveChangesAsync();
//                }
//                return Suspens;
//            }
//        }
//    }
//}
