using Discord.Commands;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DarlingNet.Services.LocalService.Attribute
{
    public class PermissionHierarchy : PreconditionAttribute
    {
        public async override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
        {
            var PingedUser = context.Message.MentionedUserIds.FirstOrDefault();
            if(PingedUser != 0)
            {
                string error = "";
                var user = await context.Guild.GetUserAsync(PingedUser);
                if (user != null)
                {
                    var bot = await context.Guild.GetCurrentUserAsync();
                    if (user.Id == BotSettings.hikaruid)
                        error = $"Бот не может взаимодействовать с создателем!";
                    else if (user.Guild.OwnerId == user.Id)
                        error = $"Бот не может взаимодействовать с создателем сервера!";
                    else if (bot.Hierarchy < user.Hierarchy)
                        error = $"Бот не может взаимодействовать с {user.Mention},\nтак как роль бота находится ниже роли пользователя.";
                }
                if (error.Length > 0)
                    return await Task.FromResult(PreconditionResult.FromError(error));
            }
            
            return await Task.FromResult(PreconditionResult.FromSuccess());
        }
    }
}
