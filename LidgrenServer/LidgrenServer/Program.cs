using System;
using LidgrenServer.controllers;
using LidgrenServer.Data;
using LidgrenServer.model;
using LidgrenServer.services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace LidgrenServer
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            Server server = new Server();
            //var userService = new UserService(new ApplicationDataContext());
            //var userController = new UserController(userService);


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