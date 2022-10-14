using Discord;
using Discord.WebSocket;
using System.Threading.Tasks;

namespace DarlingNet.Services.LocalService.VerifiedAction
{
    public static class SendUserMessage
    {
        public static Task<string> UserMessage(this SocketGuildUser user, string paintext, EmbedBuilder Build = null, MessageComponent Component = null)
            => SendMessageForUser(user, paintext, Build, Component);
        public static Task<string> UserMessage(this SocketUser user, string paintext, EmbedBuilder Build = null, MessageComponent Component = null)
            => SendMessageForUser(user, paintext, Build, Component);

        private static async Task<string> SendMessageForUser(this SocketUser user, string paintext, EmbedBuilder Build, MessageComponent Component)
        {
            string Message = null;
            try
            {
                await user.SendMessageAsync(paintext, false, Build?.Build(), components: Component);
            }
            catch
            {
                Message = "У пользователя закрыты личные сообщения!";
            }
            return Message;
        }
    }
}
