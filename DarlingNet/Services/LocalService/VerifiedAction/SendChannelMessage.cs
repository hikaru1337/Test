using Discord;
using Discord.WebSocket;
using System.Threading.Tasks;

namespace DarlingNet.Services.LocalService.VerifiedAction
{
    public static class SendChannelMessage
    {
        public static Task<string> Message(this SocketTextChannel Channel, string paintext, EmbedBuilder Build = null)
            => SendMessageForChannel(Channel, paintext, Build);

        public static Task<string> Message(this ISocketMessageChannel Channel, string paintext, EmbedBuilder Build = null)
            => SendMessageForChannel(Channel as SocketTextChannel, paintext, Build);

        public static Task<string> Message(this IMessageChannel Channel, string paintext, EmbedBuilder Build = null)
            => SendMessageForChannel(Channel as SocketTextChannel, paintext, Build);

        public static Task<string> JsonMessageToEmbed(this ISocketMessageChannel Channel, string text)
        {
            var emb = JsonToEmbed.JsonCheck(text);
            if(emb.Item1 != null)
                return SendMessageForChannel(Channel as SocketTextChannel, emb.Item2, emb.Item1);

            return Task.FromResult("Json сообщение составлено не верно!");
        }


        private static async Task<string> SendMessageForChannel(this SocketTextChannel Channel, string paintext, EmbedBuilder Build, MessageComponent Component = null)
        {
            string Error = string.Empty;
            if ((!string.IsNullOrWhiteSpace(paintext) || Build != null) && Channel != null)
            {
                var Bot = Channel.Guild.CurrentUser;
                var ChannelPermEveryOne = Channel.GetPermissionOverwrite(Channel.Guild.EveryoneRole);
                var ChannelPermBot = Channel.GetPermissionOverwrite(Bot);
                if (Bot.GuildPermissions.Administrator ||
                    (ChannelPermEveryOne != null && ChannelPermEveryOne.Value.SendMessages == PermValue.Allow) ||
                    (ChannelPermBot != null && ChannelPermBot.Value.SendMessages == PermValue.Allow))
                {
                    await Channel.SendMessageAsync(paintext, false, Build?.Build(), components: Component);
                }
                else
                    Error = "У бота недостаточно прав для отправки сообщений в этот канал!";
            }
            return Error;
        }
    }
}
