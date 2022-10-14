using DarlingDb;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace DarlingNet.Services
{
    public class StartUpService
    {
        private readonly IServiceProvider _provider;
        private readonly DiscordShardedClient _discord;
        private readonly CommandService _commands;
        private readonly IConfigurationRoot _config;

        public StartUpService(IServiceProvider provider, DiscordShardedClient discord, CommandService commands, IConfigurationRoot config)
        {
            _provider = provider;
            _config = config;
            _discord = discord;
            _commands = commands;
        }

        public async Task StartAsync()
        {
            using (var mContext = new db())
            {
                mContext.Database.Migrate();
                await mContext.SaveChangesAsync();
                mContext.Dispose();

                string discordToken = _config["tokens:discord"];
                if (string.IsNullOrWhiteSpace(discordToken))
                    throw new Exception("нет токена!");
                await _discord.LoginAsync(TokenType.Bot, discordToken);
                await _discord.StartAsync();
                await _discord.SetStatusAsync(UserStatus.DoNotDisturb);
                await _discord.SetGameAsync("Загрузка бота!", null, ActivityType.Playing);
                await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _provider);
            }
        }
    }
}
