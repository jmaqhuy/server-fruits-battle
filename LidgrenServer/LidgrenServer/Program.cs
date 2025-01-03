using LidgrenServer.Controllers;
using LidgrenServer.Data;
using LidgrenServer.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using LidgrenServer.services;
using LidgrenServer.controllers;
using LidgrenServer.repository;

namespace LidgrenServer
{
    class Program
    {
        public static PacketProcessors server;
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((context, services) =>
                {
                    services.AddMemoryCache();
                    services.AddDbContext<ApplicationDataContext>();
                    services.AddScoped<OtpService>();
                    services.AddScoped<EmailService>();
                    services.AddScoped<UserService>();
                    services.AddScoped<UserController>();
                    services.AddScoped<LoginHistoryService>();
                    services.AddScoped<LoginHistoryController>();
                    services.AddScoped<UserRelationshipService>();
                    services.AddScoped<UserRelationshipController>();

                    services.AddScoped<UserCharacterRepository>();
                    services.AddScoped<UserCharacterService>();
                    services.AddScoped<UserCharacterController>();

                    services.AddScoped<CharacterController>();
                    services.AddScoped<CharacterService>();
                    services.AddScoped<CharacterRepository>();

                    // season
                    services.AddScoped<SeasonController>();
                    services.AddScoped<SeasonService>();
                    services.AddScoped<SeasonRepository>();

                    // rank
                    services.AddScoped<RankController>();
                    services.AddScoped<RankService>();
                    services.AddScoped<RankRepository>();

                    // user rank
                    services.AddScoped<UserRankController>();
                    services.AddScoped<UserRankService>();
                    services.AddScoped<UserRankRepository>();

                });

        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            var serviceProvider = host.Services;
            var characterController = serviceProvider.GetRequiredService<CharacterController>();
            if (characterController.CharacterCountAsync().Result == 0)
            {
                Logging.Info("Create sample Character");
                characterController.CreateSampleCharacterAsync();
            };

            // Create sample rank
            var rank = serviceProvider.GetRequiredService<RankController>();
            if (await rank.GetRankModelAsync(1) == null)
            {
                Logging.Info("Start New Season");
                await rank.CreateSampleRanks();
            }


            var season = serviceProvider.GetRequiredService<SeasonController>();
            //Init new Season
            if (await season.GetCurrentSeasonAync() == null)
            {
                Logging.Info("Create sample Rank");
                await season.CreateNewSeasonAsync("First Light", DateTime.Now.Date, DateTime.Now.Date.AddMonths(3));
            }
            server = new PacketProcessors(serviceProvider);
        }
    }
}