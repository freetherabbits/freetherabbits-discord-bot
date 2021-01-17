﻿using Discord.Commands;
using Discord.WebSocket;
using Kryptoteket.Bot.Configurations;
using Kryptoteket.Bot.CosmosDB;
using Kryptoteket.Bot.CosmosDB.Repositories;
using Kryptoteket.Bot.InMemoryDB;
using Kryptoteket.Bot.Interfaces;
using Kryptoteket.Bot.Services;
using Kryptoteket.Bot.Services.API;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Exceptions;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;

namespace Kryptoteket.Bot
{
    public class Startup : IDesignTimeDbContextFactory<KryptoteketContext>
    {
        private IConfiguration _configuration { get; set; }

        public Startup()
        {
            var _builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.development.json", optional: true)
                .AddEnvironmentVariables();

            _configuration = _builder.Build();
        }

        public async Task StartAsync()
        {
            Log.Logger = new LoggerConfiguration()
                .Enrich.WithExceptionDetails()
                .WriteTo.Console()
                .CreateLogger();

            Log.Information("Kryptoteket.Bot starting up...");

            var startup = new Startup();
            await startup.RunAsync();
        }

        public async Task RunAsync()
        {
            var services = ConfigureServices();

            var serviceProvider = services.BuildServiceProvider();
            serviceProvider.GetRequiredService<CommandHandlerService>();

            Log.Information("Initiating memoryDB");
            await serviceProvider.GetRequiredService<InitMemoryDB>().InitDB();

            Log.Information("Kryptoteket.Bot started");
            await serviceProvider.GetRequiredService<StartupService>().StartAsync();
            await Task.Delay(-1);
        }

        private IServiceCollection ConfigureServices()
        {
            var services = new ServiceCollection();

            services.AddSingleton(new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = Discord.LogSeverity.Verbose,
                MessageCacheSize = 1000,
                AlwaysDownloadUsers = true
            }));

            services.AddSingleton(new CommandService(new CommandServiceConfig
            {
                DefaultRunMode = RunMode.Async,
                LogLevel = Discord.LogSeverity.Verbose,
                CaseSensitiveCommands = false,
                ThrowOnError = false
            }));

            services.Configure<RequestLocalizationOptions>(options =>
            {
                options.DefaultRequestCulture = new RequestCulture("nb-NO");
            });

            services.AddSingleton<IMiraiexAPIService, MiraiexAPIService>();
            services.AddSingleton<ICovid19APIService, Covid19APIService>();
            services.AddSingleton<ICoinGeckoAPIService, CoinGeckoAPIService>();
            services.AddSingleton<ICoinGeckoRepository, CoinGeckoRepository>();
            services.AddSingleton<IQuickchartAPIService, QuickchartAPIService>();
            services.AddSingleton<INBXAPIService, NBXAPIService>();
            services.AddSingleton<IBitmyntAPIService, BitmyntAPIService>();

            services.AddSingleton<InitMemoryDB>();
            services.AddSingleton<CommandHandlerService>();
            services.AddSingleton<StartupService>();
            services.AddSingleton<LoggingService>();
            services.AddSingleton(_configuration);

            services.AddTransient<HttpResponseService>();
            services.AddTransient<EmbedService>();

            services.AddScoped<IReflinkRepository, ReflinkRepository>();
            services.AddScoped<IRefUserRepository, RefUserRepository>();
            services.AddScoped<IRefExchangeRepository, RefExchangeRepository>();
            services.AddScoped<IBetRepository, BetRepository>();
            services.AddScoped<IPlacedUserBetRepository, PlacedUserBetRepository>();
            services.AddScoped<IBetWinnersRepository, FinishedBetPlacementsRepository>();
            services.AddScoped<IBetUserRepository, BetUserRepository>();

            //services.AddDbContext<KryptoteketContext>(options => options.UseCosmos(
                //_configuration.GetSection("Cosmos-Kryptoteket")["EndpointUri"],
                //_configuration.GetSection("Cosmos-Kryptoteket")["PrimaryKey"],
                //_configuration.GetSection("Cosmos-Kryptoteket")["DatabaseName"]));

            services.AddDbContext<KryptoteketContext>(options => options.UseSqlServer(_configuration.GetConnectionString("Default")));

            services.Configure<ExchangesConfiguration>(options => _configuration.GetSection("Exchanges").Bind(options));
            services.Configure<DiscordConfiguration>(options => _configuration.GetSection("Discord-Kryptoteket").Bind(options));
            services.Configure<CovidAPIConfiguration>(options => _configuration.GetSection("CovidAPI").Bind(options));
            services.Configure<CoinGeckoConfiguration>(options => _configuration.GetSection("CoinGecko").Bind(options));
            services.Configure<QuickchartConfiguration>(options => _configuration.GetSection("Quickchart").Bind(options));
            services.Configure<CosmosDBConfiguration>(options => _configuration.GetSection("Cosmos-Kryptoteket").Bind(options));

            return services;
        }

        public KryptoteketContext CreateDbContext(string[] args)
        {
            var _builder = new ConfigurationBuilder()
               .SetBasePath(Directory.GetCurrentDirectory())
               .AddJsonFile("appsettings.json")
               .AddJsonFile($"appsettings.development.json", optional: true)
               .AddEnvironmentVariables();

            _configuration = _builder.Build();

            DbContextOptionsBuilder<KryptoteketContext> optionsBuilder = new DbContextOptionsBuilder<KryptoteketContext>()
                .UseSqlServer(_configuration.GetConnectionString("Default"));

            return new KryptoteketContext(optionsBuilder.Options);
        }
    }
}