using Discord.Commands;
using System.Threading.Tasks;
using DarlingDb;
using DarlingNet.Services.LocalService.GetOrCreate;
using DarlingDb.Models;
using System;
using DarlingNet.Services.LocalService.CommandList;
using System.Linq;
using static DarlingDb.Enums;

namespace DarlingNet.Services.LocalService.Attribute
{
    public class PermissionViolation : PreconditionAttribute
    {
        public async override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
        {
            using (db _db = new ())
            {
                var Guild = await _db.Guilds.GetOrCreate(context.Guild.Id);
                string text = string.Empty;
                var CommandInfo = Initiliaze.ListCommand.FirstOrDefault(x => x.Usage[1] == command.Aliases[0]);
                switch (Guild.VS)
                {
                    case ViolationSystemEnum.none:
                    case ViolationSystemEnum.off:
                        if (CommandInfo.Category == "ReportSystem" || CommandInfo.Category == "WarnSystem")
                            text = $"У вас не включена одна из систем нарушений.";
                        break;
                    case ViolationSystemEnum.WarnSystem:
                        if (CommandInfo.Category != "WarnSystem")
                            text = $"У вас не включена репорт система!.\n{Guild.Prefix}vs";
                        break;
                    case ViolationSystemEnum.ReportSystem:
                        if (CommandInfo.Category != "ReportSystem")
                            text = $"У вас не включена варн система!.";
                        break;
                }

                if (text.Length > 0)
                {
                    text += $"\n{Guild.Prefix}vs";
                    return await Task.FromResult(PreconditionResult.FromError(text));
                }
                    

                return await Task.FromResult(PreconditionResult.FromSuccess());
            }
        }

    }
}
