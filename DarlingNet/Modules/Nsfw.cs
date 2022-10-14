using DarlingNet.Services.LocalService.Attribute;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using NekosSharp;
using System.Linq;
using System.Threading.Tasks;
using static DarlingNet.Services.LocalService.Attribute.CommandLocksAttribute;

namespace DarlingNet.Modules
{
    [Name("NsfwGif")]
    [Summary("18+ гифки")]
    [RequireBotPermission(ChannelPermission.SendMessages)]
    [RequireBotPermission(ChannelPermission.EmbedLinks)]
    public class NSFW : ModuleBase<ShardedCommandContext>
    {
        private readonly NekoClient NekoClient = new("DARLING");

        private enum TypeGif
        {
            yuri,
            anal,
            blowjob,
            boobs,
            classic,
            cum,
            feet,
            hentai,
            kuni,
            neko,
            pussy,
            pwank,
            solo,
            spank,
            eroNeko,
            ahegao,
            cosplay,
            eroFeet,
            trap
        }

        private static EmbedBuilder CheckNSFW(ShardedCommandContext Context)
        {
            var emb = new EmbedBuilder();
            if (!(Context.Message.Channel as SocketTextChannel).IsNsfw)
            {
                SocketTextChannel NsfwChannel = null;
                foreach (var Channel in Context.Guild.TextChannels.Where(x => x.IsNsfw))
                {
                    var Permission = Channel.GetPermissionOverwrite(Context.Guild.EveryoneRole);
                    if(Permission.Value.SendMessages != PermValue.Deny && 
                       Permission.Value.ViewChannel  != PermValue.Deny)
                    {
                        NsfwChannel = Channel;
                        break;
                    }
                }

                emb.WithDescription("Данный канал не является NSFW для использования этой команды.\n");
                if (NsfwChannel == null)
                    emb.Description += "Попросите админа создать канал с параметром NSFW.";
                else
                    emb.Description += $"Используйте эту команду в {NsfwChannel.Mention}";
            }
            return emb;
        }

        private async Task GenerateImage(TypeGif Type)
        {
            var emb = CheckNSFW(Context).WithColor(255, 0, 94).WithAuthor($"{Type} GIF 18+");
            if (emb.Description == null)
            {
                while (true)
                {
                    Request Request = null;
                    switch (Type)
                    {
                        case TypeGif.yuri: Request = await NekoClient.Nsfw_v3.YuriGif(); break;
                        case TypeGif.anal: Request = await NekoClient.Nsfw_v3.AnalGif(); break;
                        case TypeGif.blowjob: Request = await NekoClient.Nsfw_v3.BlowjobGif(); break;
                        case TypeGif.boobs: Request = await NekoClient.Nsfw_v3.BoobsGif();break;
                        case TypeGif.classic: Request = await NekoClient.Nsfw_v3.ClassicGif();break;
                        case TypeGif.cum: Request = await NekoClient.Nsfw_v3.CumGif(); break;
                        case TypeGif.feet: Request = await NekoClient.Nsfw_v3.FeetGif();break;
                        case TypeGif.hentai: Request = await NekoClient.Nsfw_v3.HentaiGif(); break;
                        case TypeGif.kuni: Request = await NekoClient.Nsfw_v3.KuniGif(); break;
                        case TypeGif.neko: Request = await NekoClient.Nsfw_v3.NekoGif();break;
                        case TypeGif.pussy: Request = await NekoClient.Nsfw_v3.PussyGif(); break;
                        case TypeGif.pwank: Request = await NekoClient.Nsfw_v3.PwankGif(); break;
                        case TypeGif.solo: Request = await NekoClient.Nsfw_v3.SoloGif(); break;
                        case TypeGif.spank: Request = await NekoClient.Nsfw_v3.SpankGif(); break;
                        case TypeGif.eroNeko: Request = await NekoClient.Nsfw_v3.EroNeko();break;
                        case TypeGif.ahegao: Request = await NekoClient.Nsfw_v3.Ahegao();break;
                        case TypeGif.cosplay: Request = await NekoClient.Nsfw_v3.Cosplay();break;
                        case TypeGif.eroFeet: Request = await NekoClient.Nsfw_v3.EroFeet();break;
                        case TypeGif.trap: Request = await NekoClient.Nsfw_v3.Trap(); break;
                    }

                    if (Request.Success)
                    {
                        emb.WithImageUrl(Request.ImageUrl);
                        break;
                    }
                    else
                        emb.WithDescription("Команда временно не работает!");
                    
                }
            }
            await Context.Channel.SendMessageAsync("", false, emb.Build());
        }

        [Aliases, Commands, Usage, Descriptions, PermissionBlockCommand]
        public async Task yuri() => await GenerateImage(TypeGif.yuri); 

        [Aliases, Commands, Usage, Descriptions, PermissionBlockCommand]
        public async Task anal() => await GenerateImage(TypeGif.anal);

        [Aliases, Commands, Usage, Descriptions, PermissionBlockCommand]
        public async Task blowjob() => await GenerateImage(TypeGif.blowjob);

        [Aliases, Commands, Usage, Descriptions, PermissionBlockCommand]
        public async Task boobs() => await GenerateImage(TypeGif.boobs);

        [Aliases, Commands, Usage, Descriptions, PermissionBlockCommand]
        public async Task classic() => await GenerateImage(TypeGif.classic);

        [Aliases, Commands, Usage, Descriptions, PermissionBlockCommand]
        public async Task cum() => await GenerateImage(TypeGif.cum);

        [Aliases, Commands, Usage, Descriptions, PermissionBlockCommand]
        public async Task feet() => await GenerateImage(TypeGif.feet);

        [Aliases, Commands, Usage, Descriptions, PermissionBlockCommand]
        public async Task hentai() => await GenerateImage(TypeGif.hentai);

        [Aliases, Commands, Usage, Descriptions, PermissionBlockCommand]
        public async Task kuni() => await GenerateImage(TypeGif.kuni);

        [Aliases, Commands, Usage, Descriptions, PermissionBlockCommand]
        public async Task neko() => await GenerateImage(TypeGif.neko);

        [Aliases, Commands, Usage, Descriptions, PermissionBlockCommand]
        public async Task pussy() => await GenerateImage(TypeGif.pussy);

        [Aliases, Commands, Usage, Descriptions, PermissionBlockCommand]
        public async Task pwank() => await GenerateImage(TypeGif.pwank);

        [Aliases, Commands, Usage, Descriptions, PermissionBlockCommand]
        public async Task solo() => await GenerateImage(TypeGif.solo);

        [Aliases, Commands, Usage, Descriptions, PermissionBlockCommand]
        public async Task spank() => await GenerateImage(TypeGif.spank);

        [Aliases, Commands, Usage, Descriptions, PermissionBlockCommand]
        public async Task eroNeko() => await GenerateImage(TypeGif.eroNeko);

        [Aliases, Commands, Usage, Descriptions, PermissionBlockCommand]
        public async Task ahegao() => await GenerateImage(TypeGif.ahegao);

        [Aliases, Commands, Usage, Descriptions, PermissionBlockCommand]
        public async Task cosplay() => await GenerateImage(TypeGif.cosplay);

        [Aliases, Commands, Usage, Descriptions, PermissionBlockCommand]
        public async Task eroFeet() => await GenerateImage(TypeGif.eroFeet);

        [Aliases, Commands, Usage, Descriptions, PermissionBlockCommand]
        public async Task trap() => await GenerateImage(TypeGif.trap);
    }
}
