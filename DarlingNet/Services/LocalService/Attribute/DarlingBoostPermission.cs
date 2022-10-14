using Discord.Commands;
using System.Threading.Tasks;
using DarlingDb;
using DarlingNet.Services.LocalService.GetOrCreate;
using System;

namespace DarlingNet.Services.LocalService.Attribute
{
    sealed class DarlingBoostPermission : PreconditionAttribute
    {
        public async override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
        {
            using (db _db = new ())
            {
                var UserBoost = await _db.DarlingBoost.GetOrCreate(context.User.Id);
                
                if(!UserBoost.Active)
                {
                    var Prefix = await _db.Guilds.GetPrefix(context.Guild.Id);
                    return await Task.FromResult(PreconditionResult.FromError($"У вас нету подписки DarlingBoost\nИнформация о бусте: {Prefix}boost"));
                }
                    

                return await Task.FromResult(PreconditionResult.FromSuccess());
            }
        }

    }
}
