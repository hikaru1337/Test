using DarlingDb.Models;
using DarlingDb;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace DarlingNet.Services.LocalService.GetOrCreate
{
    public static class GOCUser
    {
        public static Task<Users> GetOrCreate(this DbSet<Users> Users, ulong UsersId, ulong GuildsId = 0, bool IsBot = false) 
            => GetOrCreateUsers(Users, UsersId, GuildsId, IsBot);

        private static async Task<Users> GetOrCreateUsers(DbSet<Users> Users, ulong UsersId, ulong GuildsId, bool IsBot)
        {
            using (db _db = new ())
            {
                var User = Users.FirstOrDefault(x => x.Id == UsersId);
                if (User == null)
                {
                    try
                    {
                        User = new Users() { Id = UsersId, IsBot = IsBot, LastOnline = DateTime.Now };
                        _db.Users.Add(User);
                        if (GuildsId != 0)
                        {
                            var UsersGuild = new Users_Guild { UsersId = UsersId, GuildsId = GuildsId, ZeroCoin = 1000 };
                            _db.Users_Guild.Add(UsersGuild);
                        }
                        await _db.SaveChangesAsync();
                    }
                    catch (Exception ex)
                    {
                        var userNew = Users.FirstOrDefault(x => x.Id == UsersId);
                        string text = string.Empty;
                        text += $"ОШИБКА GOC_Users -------------------------------------------------------\n\n" +
                                $"UserId - {UsersId}\n" +
                                $"GuildId - {GuildsId}\n" +
                                $"Аккаунт не существует - {userNew == null}\n" +
                                $"\n\n{ex}\n\n" +
                                $"ОШИБКА -------------------------------------------------------";

                        Console.WriteLine(text);
                    }
                }

                return User;
            }
        }

        public static Task<Users_Guild> GetOrCreate(this DbSet<Users_Guild> Users_Guilds, ulong UsersId, ulong GuildsId, bool IsBot = false)
            => GetOrCreateUsersGuilds(Users_Guilds, UsersId, GuildsId, IsBot);

        private static async Task<Users_Guild> GetOrCreateUsersGuilds(DbSet<Users_Guild> Users_Guilds, ulong UsersId, ulong GuildsId, bool IsBot)
        {
            using (db _db = new ())
            {
                Users_Guild User = null;
                Users Users = null;
                try
                {
                    User = Users_Guilds.Include(x => x.Users).FirstOrDefault(x => x.UsersId == UsersId && x.GuildsId == GuildsId);

                    if (User == null)
                    {
                        Users = _db.Users.FirstOrDefault(x => x.Id == UsersId);
                        if (Users == null)
                        {
                            Users = new Users() { Id = UsersId, IsBot = IsBot, LastOnline = DateTime.Now };
                            _db.Users.Add(Users);
                        }
                        User = new Users_Guild { UsersId = UsersId, GuildsId = GuildsId, ZeroCoin = 1000 };
                        _db.Users_Guild.Add(User);
                        await _db.SaveChangesAsync();
                        User.Users = Users;
                    }
                    
                }
                catch (Exception ex)
                {
                    var userDbNew = Users_Guilds.Include(x => x.Users).FirstOrDefault(x => x.UsersId == UsersId && x.GuildsId == GuildsId);
                    var userNew = _db.Users.FirstOrDefault(x => x.Id == UsersId);
                    string text = string.Empty;
                    text += $"ОШИБКА GOC_UserGuilds -------------------------------------------------------\n\n" +
                            $"UserId - {UsersId}\n" +
                            $"GuildId - {GuildsId}\n" +
                            $"Аккаунт сервера не существует - {userNew == null}\n" +
                            $"Аккаунт дискорда не существует - {userDbNew == null}\n" +
                            $"\n\n{ex.Message}\n\n" +
                            $"ОШИБКА -------------------------------------------------------";

                    Console.WriteLine(text);
                    return null;
                }
                return User;
            }
            
        }
    }
}
