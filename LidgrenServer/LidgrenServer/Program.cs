using System.Net.Sockets;
using System.Net;
using LidgrenServer.Controllers;
using LidgrenServer.Data;
using LidgrenServer.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Google.Protobuf.WellKnownTypes;

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
       
        public static void Main(string[] args)
        {
            List<string> LocalIpAddresses = new List<string>();
            LocalIpAddresses.Add("127.0.0.1");
            int i = 0;
            string IpSelected;
            while (true) {
                
                var IPAddresses = Dns.GetHostEntry(Dns.GetHostName());
                foreach (var ip in IPAddresses.AddressList)
                {
                    if (ip.AddressFamily == AddressFamily.InterNetwork)
                    {
                        LocalIpAddresses.Add(ip.ToString());
                    }
                }
                //if (LocalIpAddresses.Count == 0)
                //{
                //    i++;
                //    Logging.Warn("There is no network connection: " + i);
                //    if (i == 5)
                //    {
                //        Logging.Error("No NetworkConnection. GoodBye!");
                //        return;
                //    }
                //    Thread.Sleep(3000);
                //} 
                //else
                //{

                    Console.WriteLine("Choose 1 IP Address");
                    for (i = 0; i < LocalIpAddresses.Count; i++)
                    {
                        Console.WriteLine((i + 1) + ": " + LocalIpAddresses[i]);
                    }
                    string userinput = Console.ReadLine();

                    while(int.Parse(userinput) >= LocalIpAddresses.Count || int.Parse(userinput)<=0)
                    {
                        Console.WriteLine("Invalid Input, Please input again!");
                        userinput = Console.ReadLine();
                    }
                    IpSelected = LocalIpAddresses[int.Parse(userinput) - 1];
                    break;
                //}

                
            }
            Logging.Info($"Selected Ip {IpSelected}. Start Server");
            var host = CreateHostBuilder(args).Build();
            var serviceProvider = host.Services;
            new PacketProcessors(serviceProvider, IpSelected);
        }
    }
}