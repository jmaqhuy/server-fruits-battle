using System;
using LidgrenServer.Data;
using LidgrenServer.services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace LidgrenServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Server server = new Server();
        }
    }
}