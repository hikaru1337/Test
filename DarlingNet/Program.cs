using DarlingNet.Services;
using DarlingNet.Services.LocalService;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using EFCoreSecondLevelCacheInterceptor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DarlingNet
{
    sealed class Program
    {
        IConfigurationRoot Configuration { get; }

        Program()
        {
            var builder = new ConfigurationBuilder().SetBasePath(AppContext.BaseDirectory);
            builder.AddYamlFile(BotSettings.config_file);
            Configuration = builder.Build();
        }

        static void Main() => new Program().RunAsync().GetAwaiter().GetResult();

        public async Task RunAsync()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("ru-RU"); // Мы русские - с нами бог.
            using (var services = ConfigureServices())
            {
                services.GetRequiredService<LoggingService>();      // Start the logging service
                services.GetRequiredService<CommandHandler>();      // Start the command handler service
                services.GetRequiredService<DiscordShardedClient>();
                await services.GetRequiredService<StartUpService>().StartAsync();       // Start the startup service
                services.GetRequiredService<SecondLevelCacheInterceptor>();
                await services.GetRequiredService<DataBaseCheck>().StartTimerDataCheck();
                await services.GetRequiredService<TaskTimer>().StartChangeStatus();
                await Task.Delay(Timeout.Infinite);
            }
        }

        ServiceProvider ConfigureServices()
        {
            int[] shardIds = Enumerable.Range(0, BotSettings._totalShards).ToArray();
            var ShardConfig = new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Verbose,
                MessageCacheSize = 128,
                DefaultRetryMode = RetryMode.Retry502,
                AlwaysDownloadUsers = true,
                TotalShards = shardIds.Length,

                GatewayIntents = GatewayIntents.All
            };
            var ServiceConfig = new CommandServiceConfig
            {
                LogLevel = LogSeverity.Verbose,
                DefaultRunMode = RunMode.Async,
            };

            return new ServiceCollection()
                    .AddSingleton(new DiscordShardedClient( shardIds, ShardConfig))
                    .AddSingleton(new CommandService(ServiceConfig))
                    .AddSingleton<CommandHandler>()         
                    .AddSingleton<LoggingService>()         
                    .AddSingleton<StartUpService>()         
                    .AddSingleton<DataBaseCheck>()
                    .AddSingleton<TaskTimer>()
                    .AddSingleton(Configuration)           
                    .AddLogging()
                    .AddEFSecondLevelCache(x =>
                    {
                        x.UseMemoryCacheProvider().DisableLogging(true)
                         .CacheQueriesContainingTableNames(CacheExpirationMode.Absolute, TimeSpan.FromSeconds(30), "Channel", "Users", "Guilds");
                    })
                    .BuildServiceProvider();
        }
    }
}
