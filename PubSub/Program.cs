using System;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using PubSub.Services;

namespace PubSub
{
    class Program
    {
        private readonly IConfiguration _config;
        private DiscordSocketClient _client;
        private static string _logLevel;

        static void Main(string[] args)
        {
            if (args.Length != 0)
            {
                _logLevel = args[0];
            }

            new Program().MainAsync().GetAwaiter().GetResult();
        }

        public Program()
        {
            _config = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile(path: "config.json").Build();

            Log.Logger = new LoggerConfiguration()
/*                .WriteTo.File("logs/log.log", rollingInterval: RollingInterval.Day)
                .WriteTo.SQLite(_config["Logging"])*/
                .WriteTo.Console()
                .CreateLogger();
        }

        public async Task MainAsync()
        {
            using (var services = ConfigureServices())
            {
                _client = services.GetRequiredService<DiscordSocketClient>();

                // Initialize logging and ready event
                services.GetRequiredService<LoggingService>();
                await _client.LoginAsync(TokenType.Bot, _config["Token"]);
                await _client.StartAsync();
                await services.GetRequiredService<CommandHandler>().InitializeAsync();

                await Task.Delay(-1);
            }
        }

        private Task LogAsync(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }

        private Task ReadyAsync()
        {
            Console.WriteLine($"Connected as -> [{_client.CurrentUser}] :)");
            return Task.CompletedTask;
        }

        private ServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection()
                .AddSingleton(_config)
                .AddSingleton<DiscordSocketClient>()
                .AddSingleton<CommandService>()
                .AddSingleton<CommandHandler>()
                .AddSingleton<LoggingService>()
                //.AddDbContext<WinnersContext>()
                .AddLogging(configure => configure.AddSerilog());

            if (!string.IsNullOrEmpty(_logLevel))
            {
                switch (_logLevel.ToLower())
                {
                    case "info":
                        {
                            services.Configure<LoggerFilterOptions>(options => options.MinLevel = LogLevel.Information);
                            break;
                        }
                    case "error":
                        {
                            services.Configure<LoggerFilterOptions>(options => options.MinLevel = LogLevel.Error);
                            break;
                        }
                    case "debug":
                        {
                            services.Configure<LoggerFilterOptions>(options => options.MinLevel = LogLevel.Debug);
                            break;
                        }
                    default:
                        {
                            services.Configure<LoggerFilterOptions>(options => options.MinLevel = LogLevel.Error);
                            break;
                        }
                }
            }
            else
            {
                services.Configure<LoggerFilterOptions>(options => options.MinLevel = LogLevel.Information);
            }

            var serviceProvider = services.BuildServiceProvider();
            return serviceProvider;
        }
    }
}
