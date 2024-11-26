using System.Net.Sockets;
using System.Net;
using LidgrenServer.Controllers;
using LidgrenServer.Data;
using LidgrenServer.Services;
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
                    services.AddDbContext<ApplicationDataContext>();
                    services.AddScoped<UserService>();
                    services.AddScoped<UserController>();
                    services.AddScoped<LoginHistoryService>();
                    services.AddScoped<LoginHistoryController>();
                });
       
        public static void Main(string[] args)
        {
            
            string IpSelected = SelectIpAddress();
            
            Logging.Info($"Selected Ip {IpSelected}. Start Server");
            var host = CreateHostBuilder(args).Build();
            var serviceProvider = host.Services;
            new PacketProcessors(serviceProvider, IpSelected);
        }

        private static string SelectIpAddress()
        {
            List<string> LocalIpAddresses = new List<string>();
            while (true)
            {
                LocalIpAddresses.Clear();
                LocalIpAddresses.Add("0.0.0.0");
                var IPAddresses = Dns.GetHostEntry(Dns.GetHostName());
                foreach (var ip in IPAddresses.AddressList)
                {
                    if (ip.AddressFamily == AddressFamily.InterNetwork)
                    {
                        LocalIpAddresses.Add(ip.ToString());
                    }
                }
                Console.WriteLine("Choose 1 IP Address: ");
                Console.WriteLine("0: Reload");
                for (int i = 0; i < LocalIpAddresses.Count; i++)
                {
                    Console.WriteLine((i + 1) + ": " + LocalIpAddresses[i]);
                }
                string userinput = Console.ReadLine();
                while (true)
                {
                    try
                    {
                        while (int.Parse(userinput) >= LocalIpAddresses.Count || int.Parse(userinput) < 0)
                        {
                            Console.WriteLine("Invalid Input, Please input again!");
                            userinput = Console.ReadLine();
                        }
                        break;
                    }
                    catch
                    {
                        Console.WriteLine("Invalid Input, Please input again!");
                    }
                }
                if (int.Parse(userinput) == 0) continue;
                return LocalIpAddresses[int.Parse(userinput) - 1];
            }
        }
    }
}