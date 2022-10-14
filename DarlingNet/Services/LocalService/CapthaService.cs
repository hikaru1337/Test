using DarlingDb;
using DarlingNet.Services.LocalService.GetOrCreate;
using DarlingNet.Services.LocalService.VerifiedAction;
using Discord;
using Discord.WebSocket;
using System;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace DarlingNet.Services.LocalService
{
    internal class CapthaService
    {
        public static async Task CaptcaAuth(SocketUserMessage Message)
        {
            using (db _db = new ())
            {
                var Guild = (Message.Author as SocketGuildUser).Guild;
                var GuildCaptcha = _db.Guilds_Captcha.FirstOrDefault(x => x.GuildId == Guild.Id);
                if (GuildCaptcha != null && Message.Channel.Id == GuildCaptcha.ChannelId)
                {
                    var ThisCaptcha = _db.Captcha.FirstOrDefault(x => x.GuildId == Guild.Id && x.UserId == Message.Author.Id);
                    if (ThisCaptcha != null && Message.Content == ThisCaptcha.Result.ToString())
                    {
                        await Message.Author.RemoveRole(Convert.ToUInt64(GuildCaptcha.RoleId));
                        _db.Captcha.Remove(ThisCaptcha);
                        await _db.SaveChangesAsync();
                    }
                    await Message.DeleteAsync();
                }
            }
        } // Проверка введенного сообщения

        public static async Task<SocketTextChannel> CreateChannel(SocketGuild Guild) // Создание текстового канала
        {
            using (db _db = new ())
            {
                var NewChannel = await Guild.CreateTextChannelAsync("Прохождение Captcha");
                await NewChannel.AddPermissionOverwriteAsync(Guild.EveryoneRole, new OverwritePermissions(viewChannel: PermValue.Deny));
                var ChannelDb = await _db.Channels.GetOrCreate(Guild.Id, NewChannel.Id);
                ChannelDb.UseCommand = false;
                ChannelDb.UseAdminCommand = false;
                ChannelDb.UseRPcommand = false;
                _db.Channels.Update(ChannelDb);
                await _db.SaveChangesAsync();
                var Channel = Guild.GetTextChannel(NewChannel.Id);
                await CreateMessage(Channel);
                return Channel;
            }
        }

        public static async Task<SocketRole> CreateRole(SocketGuild Guild) // Создание роли
        {
            using (db _db = new ())
            {
                var NewRole = await Guild.CreateRoleAsync("Captcha Verify");
                await _db.Role.GetOrCreate(NewRole.Id, Guild.Id);
                return Guild.GetRole(NewRole.Id);
            }
        }

        public static async Task PermissionUpdate(SocketTextChannel Channel, SocketRole Role) // Применение прав в текстовые каналы
        {
            foreach (var TextChannel in Channel.Guild.TextChannels.Where(x => x.Id != Channel.Id))
            {
                var Perm = TextChannel.GetPermissionOverwrite(Role);
                if (Perm == null)
                    await TextChannel.AddPermissionOverwriteAsync(Role, new OverwritePermissions(viewChannel: PermValue.Deny));
            }

            var ChannelPerm = Channel.GetPermissionOverwrite(Role);
            if (ChannelPerm == null || ChannelPerm.Value.ViewChannel != PermValue.Allow)
                await Channel.AddPermissionOverwriteAsync(Role, new OverwritePermissions(viewChannel: PermValue.Allow));
        }

        private static async Task CreateMessage(SocketTextChannel Channel) // Сооздание сообщения!
        {
            var menuBuilder = new SelectMenuBuilder()
                .WithPlaceholder("Нажмите для получения капчи!")
                .WithCustomId("GetCaptcha")
                .WithMinValues(1)
                .WithMaxValues(1)
                .AddOption("Получить капчу!", "GetCaptcha1")
                .AddOption("Получить капчу!", "GetCaptcha2")
                .AddOption("Получить капчу!", "GetCaptcha3");

            var builder = new ComponentBuilder().WithSelectMenu(menuBuilder);

            await Channel.SendMessageAsync("Нажмите для получения капчи!", components: builder.Build());
        }

        public static (Bitmap, int) CreateImage(int Width, int Height)
        {
            Random rnd = new();
            Bitmap result = new(Width, Height);
            int Xpos = rnd.Next(0, Width / 3);
            int Ypos = rnd.Next(15, Height - 20);
            Brush[] colors = { Brushes.Black,
                     Brushes.Red,
                     Brushes.RoyalBlue,
                     Brushes.Green };

            Graphics g = Graphics.FromImage(result);
            g.Clear(System.Drawing.Color.Gray);

            string Text = null;
            int Result = 0;
            int NumberOne = rnd.Next(101, 10000);
            int NumberTwo = rnd.Next(100, NumberOne);

            switch (rnd.Next(0, 2))
            {
                case 0:
                    Text = $"{NumberOne} - {NumberTwo}";
                    Result = NumberOne - NumberTwo;
                    break;
                case 1:
                    Text = $"{NumberOne} + {NumberTwo}";
                    Result = NumberOne + NumberTwo;
                    break;
            }


            g.DrawString(Text,
                         new Font("Arial", 15),
                         colors[rnd.Next(colors.Length)],
                         new PointF(Xpos, Ypos));


            g.DrawLine(Pens.Black,
                       new Point(0, 0),
                       new Point(Width - 1, Height - 1));
            g.DrawLine(Pens.Black,
                       new Point(0, Height - 1),
                       new Point(Width - 1, 0));

            for (int i = 0; i < Width; ++i)
                for (int j = 0; j < Height; ++j)
                    if (rnd.Next() % 20 == 0)
                        result.SetPixel(i, j, System.Drawing.Color.LightGray);

            return (result, Result);
        }
    }
}
