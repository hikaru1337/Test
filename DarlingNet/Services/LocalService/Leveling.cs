using Discord.WebSocket;
using DarlingDb;
using System;
using System.Threading.Tasks;
using System.Linq;
using DarlingNet.Services.LocalService.VerifiedAction;
using Discord;
using Microsoft.EntityFrameworkCore;
using DarlingDb.Models;

namespace DarlingNet.Services.LocalService
{
    public class Leveling
    {
        public static async Task LVL(SocketUserMessage Message,Users_Guild UserDataBase)
        {
            using (db _db = new ())
            {
                var UserDiscord = Message.Author as SocketGuildUser;
                var Roles = _db.Roles.Include(x=>x.Role).Where(x=>x.Role.GuildsId == UserDiscord.Guild.Id).AsEnumerable().Where(x=>x.Type == Enums.RoleTypeEnum.Level).OrderBy(x => x.Value);
                var ThisRole = Roles.LastOrDefault(x=> Convert.ToUInt64(x.Value) <= UserDataBase.Level);

                var NextLevel = (ulong)Math.Sqrt((UserDataBase.XP + 10) / 80);
                if (NextLevel > UserDataBase.Level)
                {
                    var NextRole = Roles.FirstOrDefault(x => Convert.ToUInt64(x.Value) == NextLevel);
                    if (NextRole != null)
                    {
                        if (ThisRole != null)
                            await UserDiscord.RemoveRole(ThisRole.RoleId);

                        await UserDiscord.AddRole(NextRole.RoleId);
                    }

                    uint amt = (uint)(500 + ((500 / 35) * (UserDataBase.Level + 1)));
                    UserDataBase.ZeroCoin += amt;
                    var Fields = new EmbedBuilder().WithAuthor("LEVEL UP", UserDiscord.GetAvatarUrl())
                                      .WithColor(255, 0, 94)
                                      .AddField("LEVEL", $"{UserDataBase.Level + 1}", true)
                                      .AddField("XP", $"{UserDataBase.XP + 10}", true)
                                      .AddField("ZeroCoins", $"+{amt}",true);
                    await Message.Channel.Message("", Fields);

                }
                else if (ThisRole != null)
                    await UserDiscord.AddRole(ThisRole.RoleId);

                UserDataBase.XP += 10;
                _db.Users_Guild.Update(UserDataBase);
                await _db.SaveChangesAsync();
            }
        }
    }
}
