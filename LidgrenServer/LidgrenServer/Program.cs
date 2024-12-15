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

                });
       
        public static  void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            var serviceProvider = host.Services;
            var characterController = serviceProvider.GetRequiredService<CharacterController>();
            if (characterController.CharacterCountAsync().Result == 0)
            {
                characterController.CreateSampleCharacterAsync();
            };
            new PacketProcessors(serviceProvider);
        }
    }
}