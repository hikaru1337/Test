using DarlingDb;
using DarlingNet.Services.LocalService.GetOrCreate;
using Discord.Commands;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DarlingNet.Services.LocalService.Attribute
{
    public class CommandLocksAttribute
    {
        public class PermissionBlockCommand : PreconditionAttribute
        {
            public async override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
            {
                using (db _db = new ())
                {
                    var Guild = await _db.Guilds.GetOrCreate(context.Guild.Id);

                    if (Guild.CommandInviseList.Any(x => command.Aliases[0] == x))
                        return await Task.FromResult(PreconditionResult.FromError($"Команда отключена создателем сервера."));

                    return await Task.FromResult(PreconditionResult.FromSuccess());
                }
                
            }
        }
    }
}
