using LidgrenServer.Controllers;
using LidgrenServer.Data;
using LidgrenServer.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using LidgrenServer.services;
using LidgrenServer.controllers;

namespace LidgrenServer
{
    class Program
    {

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((context, services) =>
                {
                    services.AddDbContext<ApplicationDataContext>();
                    services.AddScoped<UserService>();
                    services.AddScoped<UserController>();
                    services.AddScoped<LoginHistoryService>();
                    services.AddScoped<LoginHistoryController>();
                    services.AddScoped<UserRelationshipService>();
                    services.AddScoped<UserRelationshipController>();
                });
       
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            var serviceProvider = host.Services;
            new PacketProcessors(serviceProvider);
        }
    }
}