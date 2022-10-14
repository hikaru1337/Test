using Discord.Commands;
using System;
using System.Threading.Tasks;
using DarlingDb;
using System.Linq;
using Discord.WebSocket;
using DarlingNet.Services.LocalService.VerifiedAction;
using Discord;

namespace DarlingNet.Services.LocalService.Attribute
{
    sealed class PetAttribute : PreconditionAttribute
    {
        public async override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
        {
            using (db _db = new ())
            {
                var Pet = _db.Pets.FirstOrDefault(x => x.UserId == context.User.Id);
                bool Error = false;
                

                if(Pet == null)
                {
                    if(command.Name != "pets")
                        Error = true;
                }
                else
                {
                    if (Pet.Die)
                    {
                        PetService.PetDie(Pet,context.User as SocketGuildUser);
                    }
                    else if((command.Name == "petname" && Pet.Name != Pet.PetType.ToString()) || command.Name == "pets")
                        Error = true;
                }

                if(Error)
                    return await Task.FromResult(PreconditionResult.FromError($"Введенная команда не найдена!"));

                return await Task.FromResult(PreconditionResult.FromSuccess());
            }
        }

    }
}
