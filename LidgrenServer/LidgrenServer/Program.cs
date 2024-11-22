using System;
using Lidgren.Network;
using LidgrenServer.Controllers;
using LidgrenServer.Data;
using LidgrenServer.Models;
using LidgrenServer.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace LidgrenServer
{
    class Program
    {

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((context, services) =>
                {
                    // Đăng ký ApplicationDataContext, UserService, và UserController
                    services.AddDbContext<ApplicationDataContext>();
                    services.AddScoped<UserService>();
                    services.AddScoped<UserController>();
                    services.AddScoped<LoginHistoryService>();
                    services.AddScoped<LoginHistoryController>();
                });
        public static async Task Main(string[] args)
        {
            var config = new NetPeerConfiguration("game")
            {
                Port = 14242
            };
            config.EnableMessageType(NetIncomingMessageType.DiscoveryRequest);

            NetServer server = new NetServer(config);
            server.Start();
            Logging.Debug("Start DiscoveryRequest");

            while (true)
            {
                NetIncomingMessage message;
                while ((message = server.ReadMessage()) != null)
                {
                    if (message.MessageType == NetIncomingMessageType.DiscoveryRequest)
                    {
                        Logging.Debug("Received DiscoveryRequest");

                        // Respond to discovery requests
                        server.SendDiscoveryResponse(null, message.SenderEndPoint);
                        Logging.Debug("Send Discovery Response");
                    }
                    server.Recycle(message);
                }
            }


            //Server server = new Server();
            //var userService = new UserService(new ApplicationDataContext());
            //var userController = new UserController(userService);



            /*
            var host = CreateHostBuilder(args).Build();
            var serviceProvider = host.Services;
            var packetProcessor = new PacketProcessors(serviceProvider);




            */
            //var userController = serviceProvider.GetRequiredService<UserController>();

            //var loginHistoryController = serviceProvider.GetRequiredService<LoginHistoryController>();
            //userController.Login("", "");

            //await userController.CreateSampleUser();

            //await loginHistoryController.NewUserLoginAsync(1, "ABC");






            ////Logging.Info( userController.CreateSampleUser().Result + "");
            //Logging.Info(userController.Login("testUser", "password123").Result + "");

            //var serverManager = new ServerConnectionManage();

            //Console.WriteLine("Press 'q' to stop the server...");
            //while (Console.ReadKey().Key != ConsoleKey.Q) { }

            //serverManager.Stop();
            //await Task.Delay(1000);
        }
    }
}