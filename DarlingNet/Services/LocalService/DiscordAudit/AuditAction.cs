using DarlingNet.Services.LocalService.DiscordAudit.Data;
using Discord;
using Discord.WebSocket;
using System.Collections.Generic;
using System.Threading.Tasks;
using static DarlingDb.Enums;

namespace DarlingNet.Services.LocalService.DiscordAudit
{
    public static class AuditAction
    {
        public static Task<List<Audits>> BanAudit(this SocketGuild Guild, ulong TargetId, int Count)
        {
            return RunningAudit.Running(Guild, TargetId, Count, ActionType.Ban);
        }

        public static Task<List<Audits>> UnBanAudit(this SocketGuild Guild, ulong TargetId, int Count)
        {
            return RunningAudit.Running(Guild, TargetId, Count, ActionType.Unban);
        }

        public static Task<List<Audits>> KickAudit(this SocketGuild Guild, ulong TargetId, int Count)
        {
            return RunningAudit.Running(Guild, TargetId, Count, ActionType.Kick);
        }

        public static Task<List<AuditsUserAction>> AdminVoiceAudit(this SocketUser User, ulong TargetId, int Count, VoiceAuditActionEnum type)
        {
            return (User as SocketGuildUser).RunningVoiceAction(TargetId, Count, type);
        }
    }
}
