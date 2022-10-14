using Microsoft.EntityFrameworkCore;
using DarlingDb.Models;
using DarlingDb.Models.Pet;
using DarlingDb.Models.ReportSystem;
using static DarlingDb.Enums;

namespace DarlingDb
{
    public class db : DbContext
    {
        private readonly string ConnectionString = @"Data Source = C:\Users\Lil' KlowN\source\repos\DarlingNet\DarlingNet.db";
        private readonly ulong botid = 663381953181122570;

        public DbSet<Captcha> Captcha { get; set; }
        public DbSet<Pets> Pets { get; set; }
        public DbSet<Items> Items { get; set; }
        public DbSet<Role> Role { get; set; }

        public DbSet<Feedback> Feedback { get; set; }

        public DbSet<Channel> Channels { get; set; }
        public DbSet<DarlingBoost> DarlingBoost { get; set; }
        public DbSet<EmoteClick> EmoteClick { get; set; }
        public DbSet<ButtonClick> ButtonClick { get; set; }
        public DbSet<Guilds> Guilds { get; set; }
        public DbSet<Guilds_Raid> Guilds_Raid { get; set; }
        public DbSet<Guilds_Warns> Guilds_Warns { get; set; }
        public DbSet<Guilds_Logs> Guilds_Logs { get; set; }
        public DbSet<Guilds_Captcha> Guilds_Captcha { get; set; }
        public DbSet<Invites> Invites { get; set; }
        public DbSet<PrivateChannels> PrivateChannels { get; set; }
        public DbSet<Roles> Roles { get; set; }
        public DbSet<Roles_Timer> Roles_Timer { get; set; }
        public DbSet<Tasks> Tasks { get; set; }
        public DbSet<TempUser> TempUser { get; set; }
        public DbSet<Users_Guild> Users_Guild { get; set; }
        public DbSet<Users> Users { get; set; }
        public DbSet<GiveAways> GiveAways { get; set; }
        public DbSet<QiwiTransactions> QiwiTransactions { get; set; }
        public DbSet<Guilds_Meeting_Leave> Guilds_Meeting_Leave { get; set; }
        public DbSet<Guilds_Meeting_Welcome> Guilds_Meeting_Welcome { get; set; }

        public DbSet<Reports> Reports { get; set; }
        public DbSet<Reports_Punishes> Reports_Punishes { get; set; }

        //public DBcontext()
        //{
        //    Database.EnsureDeleted();
        //    Database.EnsureCreated();
        //}
        //public db(DbContextOptions<db> options) : base(options)
        //{ }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.EnableSensitiveDataLogging();
            optionsBuilder.UseSqlite(ConnectionString);
                          /*.UseLoggerFactory(LoggerFactory.Create(x => x
                          .AddConsole().AddDebug().AddFilter(x=>x == LogLevel.None)))*/

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<Darling_LikeDisLike>()
            //    .HasKey(f => new { f.FirstDarlingId, f.SecondDarlingId });

            //modelBuilder.Entity<Darling_LikeDisLike>()
            //    .HasOne(f => f.FirstDarling)
            //    .WithMany(mu => mu.MyLikeDisLike)
            //    .HasForeignKey(f => f.FirstDarlingId).OnDelete(DeleteBehavior.Restrict);

            //modelBuilder.Entity<Darling_LikeDisLike>()
            //    .HasOne(f => f.SecondDarling)
            //    .WithMany(mu => mu.MeLikeDisLikeTwo)
            //    .HasForeignKey(f => f.SecondDarlingId);

            //modelBuilder.Entity<Guilds_Captcha>()
            //.HasOne(p => p.Channel)
            //.WithOne(t => t.Guilds_Captcha)
            //.OnDelete(DeleteBehavior.SetNull);

            //modelBuilder.Entity<Guilds_Captcha>()
            //.HasOne(p => p.Role)
            //.WithOne(t => t.Guilds_Captcha)
            //.OnDelete(DeleteBehavior.SetNull);

            //modelBuilder.Entity<Darling_Hobbies>().HasData(
            //new Darling_Hobbies[]
            //{
            //                new Darling_Hobbies {Id = 1,Name = "Знакомства"},new Darling_Hobbies {Id = 2,Name = "CS"},new Darling_Hobbies {Id = 3,Name = "Minecraft"},new Darling_Hobbies {Id = 4,Name = "Valorant"},
            //                new Darling_Hobbies {Id = 5,Name = "Lol"},new Darling_Hobbies {Id = 6,Name = "Fortnite"},new Darling_Hobbies {Id = 7,Name = "Gta5"},new Darling_Hobbies {Id = 8,Name = "Dota2"},
            //                new Darling_Hobbies {Id = 9,Name = "Cod"},new Darling_Hobbies {Id = 10,Name = "Wow"},new Darling_Hobbies {Id = 11,Name = "Wot"},new Darling_Hobbies {Id = 12,Name = "Overwatch"},
            //                 new Darling_Hobbies {Id = 13,Name = "HeartStone"},new Darling_Hobbies {Id = 14,Name = "BattleField"},new Darling_Hobbies {Id = 15,Name = "OSU"}
            //});

            //modelBuilder.Entity<Users>().HasData(new Users() { Id = botid, IsBot = true });

            //modelBuilder.Entity<Pets>().HasData(new Pets() { UserId = botid, Die = true, Id = 1337 });



            //modelBuilder.Entity<Items>().HasData(new Items[] { new Items {Id = 1,PetsId = 1337, Name = "Вискас", ItemType = PetItemEnum.Еда, Emoji = "🍪", Price = 800, Value = 5 },
            //                                                   new Items {Id = 2,PetsId = 1337, Name = "Мяско", ItemType = PetItemEnum.Еда, Emoji = "🥩", Price = 5000, Value = 30 },
            //                                                   new Items {Id = 3,PetsId = 1337, Name = "Курочка", ItemType = PetItemEnum.Еда, Emoji = "🍗", Price = 20000, Value = 55 },

            //                                                   new Items {Id = 4,PetsId = 1337, Name = "Витаминки", ItemType = PetItemEnum.МедПомощь, Emoji = "💊", Price = 10000, Value = 10 },
            //                                                   new Items {Id = 5,PetsId = 1337, Name = "Антибиотики", ItemType = PetItemEnum.МедПомощь, Emoji = "🦠", Price = 20000, Value = 30 },
            //                                                   new Items {Id = 6,PetsId = 1337, Name = "Волшебный шприц", ItemType = PetItemEnum.МедПомощь, Emoji = "💉", Price = 50000, Value = 65 }});

        }

    }

}
