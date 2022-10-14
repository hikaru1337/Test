using DarlingDb.Models.ReportSystem;
using System;
using System.Collections.Generic;

namespace DarlingDb.Models
{
    public class Users
    {
        public ulong Id { get; set; }
        public bool IsBot { get; set; }
        public DarlingBoost Boost { get; set; }
        public string BlockedReason { get; set; }
        public ICollection<Feedback> Feedback { get; set; }
        public ICollection<Users_Guild> Users_Guild { get; set; }
        public Pets Pet { get; set; }
        public DateTime BirthDate { get; set; }
        public ulong Reputation { get; set; }
        public DateTime LastReputation { get; set; }
        public ulong LastReputationUserId { get; set; }
        public ushort BirthDateComplete { get; set; }
        public DateTime LastOnline { get; set; }
        public uint RealCoin { get; set; }
        public bool StingerRulesAccept { get; set; }
    }

    public class Users_Guild
    {
        public uint Id { get; set; }
        public ulong UsersId { get; set; }
        public Users Users { get; set; }
        public ulong GuildsId { get; set; }
        public Guilds Guilds { get; set; }
        public TimeSpan VoiceActive { get; set; }
        public ICollection<Reports_Punishes> Reports_Punishes { get; set; }
        public ICollection<Invites> Invites { get; set; }
        public ICollection<Roles_Timer> Roles_Timer { get; set; }
        public ICollection<TempUser> TempUser { get; set; }
        //public Suspens Suspens { get; set; }
        public PrivateChannels PrivateChannels { get; set; }
        //public ICollection<ReportSuspens> ReportSuspens { get; set; }

        public uint? UsersMId { get; set; }
        public Users_Guild UsersM { get; set; }
        public bool Leaved { get; set; }
        public ulong XP { get; set; }
        public ushort Level => (ushort) Math.Sqrt(XP / 80);
        public uint ZeroCoin { get; set; }
        public DateTime Daily { get; set; }
        public ushort Streak { get; set; }
        public uint CountWarns { get; set; }
        public string ReportedRules { get; set; }
        public bool BirthDateInvise { get; set; }
    }
}
