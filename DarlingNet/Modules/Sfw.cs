using DarlingNet.Services.LocalService.Attribute;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using NekosSharp;
using System.Threading.Tasks;
using static DarlingNet.Services.LocalService.Attribute.CommandLocksAttribute;

namespace DarlingNet.Modules
{
    [Name("SfwGif")]
    [Summary("RP гифки")]
    [RequireBotPermission(ChannelPermission.SendMessages)]
    [RequireBotPermission(ChannelPermission.EmbedLinks)]
    public class RPgif : ModuleBase<ShardedCommandContext>
    {
        private readonly NekoClient NekoClient = new("DARLING");

        private enum TypeGif
        {
            cuddle,
            feed,
            hug,
            kiss,
            pat,
            poke,
            slap,
            tickle,
            baka,
            nekos,
            smug
        }

        private async Task GenerateGif(TypeGif Type,SocketUser user = null)
        {
            var embed = new EmbedBuilder().WithColor(255, 0, 94).WithAuthor($"{Type} GIF");

            while (true)
            {
                Request Gif = null;
                string Description = $"{Context.User.Mention}";
                switch (Type)
                {
                    case TypeGif.cuddle: 
                        Gif = await NekoClient.Action_v3.CuddleGif();
                        if(user != null)
                            Description += $" Прижался(ась) {(user != null ? $"к {user.Mention}" : "")}";
                        break;
                    case TypeGif.feed: 
                        Gif = await NekoClient.Action_v3.FeedGif();
                        if (user != null)
                            Description += $" {(user != null ? $"кормит {user.Mention}" : "кушает")}";
                        break;
                    case TypeGif.hug: 
                        Gif = await NekoClient.Action_v3.HugGif();
                        if (user != null)
                            Description += $" {(user != null ? $"обнял(а) {user.Mention}" : "обнимается")}";
                        break;
                    case TypeGif.kiss: 
                        Gif = await NekoClient.Action_v3.KissGif();
                        if (user != null)
                            Description += $" {(user != null ? $"поцеловал(а) {user.Mention}" : "целуется")}";
                        break;
                    case TypeGif.pat: 
                        Gif = await NekoClient.Action_v3.PatGif();
                        if (user != null)
                            Description += $" {(user != null ? $"погладил(а) {user.Mention}" : "погладил(а)")}";
                        break;
                    case TypeGif.poke: 
                        Gif = await NekoClient.Action_v3.PokeGif();
                        if (user != null)
                            Description += $" {(user != null ? $"ткнул(а) {user.Mention}" : "тыкает пустоту...")}"; 
                        break;
                    case TypeGif.slap: 
                        Gif = await NekoClient.Action_v3.SlapGif();
                        if (user != null)
                            Description += $" {(user != null ? $"дал(а) пощечину {user.Mention}" : "шлепнул пустоту...")}"; 
                        break;
                    case TypeGif.tickle: 
                        Gif = await NekoClient.Action_v3.TickleGif();
                        if (user != null)
                            Description += $" {(user != null ? $"щекочет {user.Mention}" : "щекочет пустоту...")}"; 
                        break;
                    case TypeGif.baka: 
                        Gif = await NekoClient.Image_v3.BakaGif();
                        if (user != null)
                            Description += $" {(user != null ? $"сказал(а) что {user.Mention} дурак" : $"дурак")}"; 
                        break;
                    case TypeGif.nekos: 
                        Gif = await NekoClient.Image_v3.NekoGif();
                        if (user != null)
                            Description += $" приносит кошечку"; 
                        break;
                    case TypeGif.smug: 
                        Gif = await NekoClient.Image_v3.SmugGif();
                        if (user != null)
                            Description += $" самодовольничает";
                        break;
                }

                if (Gif.Success)
                {
                    embed.WithImageUrl(Gif.ImageUrl).WithDescription(Description);
                    break;
                }
                else
                    embed.WithDescription("Команда временно не работает!");

            }
            await Context.Channel.SendMessageAsync("", false, embed.Build());
        }

        [Aliases, Commands, Usage, Descriptions, PermissionBlockCommand]
        public async Task cuddle(SocketUser user = null) => await GenerateGif(TypeGif.cuddle, user);

        [Aliases, Commands, Usage, Descriptions, PermissionBlockCommand]
        public async Task feed(SocketUser user = null) => await GenerateGif(TypeGif.feed, user);

        [Aliases, Commands, Usage, Descriptions, PermissionBlockCommand]
        public async Task hug(SocketUser user = null) => await GenerateGif(TypeGif.hug, user);

        [Aliases, Commands, Usage, Descriptions, PermissionBlockCommand]
        public async Task kiss(SocketUser user = null) => await GenerateGif(TypeGif.kiss, user);

        [Aliases, Commands, Usage, Descriptions, PermissionBlockCommand]
        public async Task pat(SocketUser user = null) => await GenerateGif(TypeGif.pat, user);

        [Aliases, Commands, Usage, Descriptions, PermissionBlockCommand]
        public async Task poke(SocketUser user = null) => await GenerateGif(TypeGif.poke, user);

        [Aliases, Commands, Usage, Descriptions, PermissionBlockCommand]
        public async Task slap(SocketUser user = null) => await GenerateGif(TypeGif.slap, user);

        [Aliases, Commands, Usage, Descriptions, PermissionBlockCommand]
        public async Task tickle(SocketUser user = null) => await GenerateGif(TypeGif.tickle, user);

        [Aliases, Commands, Usage, Descriptions, PermissionBlockCommand]
        public async Task baka(SocketUser user = null) => await GenerateGif(TypeGif.baka, user);

        [Aliases, Commands, Usage, Descriptions, PermissionBlockCommand]
        public async Task nekos() => await GenerateGif(TypeGif.nekos);

        [Aliases, Commands, Usage, Descriptions, PermissionBlockCommand]
        public async Task smug() => await GenerateGif(TypeGif.smug);
    }
}
