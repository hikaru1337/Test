using Discord;
using Discord.Rest;
using System;


namespace DarlingNet.Services.LocalService.DiscordAudit.Data
{
    public class Audits
    {
        public ulong Id { get; set; }
        public IUser Target { get; set; }
        public IUser User { get; set; }
        public string Reason { get; set; }
        public DateTimeOffset Time { get; set; }
    }

    public class AuditsUserAction
    {
        public ulong Id { get; set; }
        public IUser Target { get; set; }
        public MemberInfo TargetBeforeInfo { get; set; }
        public MemberInfo AfterBeforeInfo { get; set; }
        public IUser User { get; set; }
        public string Reason { get; set; }
        public DateTimeOffset Time { get; set; }
    }


}
