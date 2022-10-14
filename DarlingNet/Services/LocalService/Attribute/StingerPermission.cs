using Discord.Commands;
using System.Threading.Tasks;
using DarlingDb;
using DarlingNet.Services.LocalService.GetOrCreate;
using System;
using Discord.WebSocket;

namespace DarlingNet.Services.LocalService.Attribute
{
    sealed class StingerPermission : PreconditionAttribute
    {
        public async override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
        {
            using (db _db = new ())
            {
                var User = await _db.Users.GetOrCreate(context.User.Id,context.Guild.Id);

                if (User.StingerRulesAccept)
                    return await Task.FromResult(PreconditionResult.FromSuccess());

                return await Task.FromResult(PreconditionResult.FromError($"Введенная команда не найдена!"));
            }
        }

    }
}
