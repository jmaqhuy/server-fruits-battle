using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using Lidgren.Network;
using LidgrenServer.Controllers;
using LidgrenServer.Data;
using LidgrenServer.Models;
using LidgrenServer.Services;
using Microsoft.Extensions.DependencyInjection;
using Org.BouncyCastle.Security;
namespace LidgrenServer
{
    public class PlayerPosition
    {
        public float X { get; set; }
        public float Y { get; set; }
    }
    public class PacketProcessors
    {
        private NetServer server;
        private Thread thread;
        private List<string> players;
        private Dictionary<string, PlayerPosition> playerPositions;
        private readonly ServiceProvider _serviceProvider;

        public PacketProcessors()
        {
            players = new List<string>();
            playerPositions = new Dictionary<string, PlayerPosition>();

            NetPeerConfiguration config = new NetPeerConfiguration("game");

            config.MaximumConnections = 20;
            config.LocalAddress = IPAddress.Parse("172.19.201.72");
            config.Port = 14242;
            server = new NetServer(config);
            server.Start();

            _serviceProvider = new ServiceCollection().BuildServiceProvider();

            thread = new Thread(Listen);
            thread.Start();
        }

        public void Listen()
        {
            Logging.Info("Listening for client ...");

            while (!Console.KeyAvailable || Console.ReadKey().Key != ConsoleKey.Escape)
            {
                NetIncomingMessage message;

                while ((message = server.ReadMessage()) != null)
                {
                    Logging.Info("Message received");
                    // Get list of users

                    List<NetConnection> all = server.Connections;
                    switch (message.MessageType)
                    {
                        case NetIncomingMessageType.StatusChanged:
                            NetConnectionStatus status = (NetConnectionStatus)message.ReadByte();
                            string reason = message.ReadString();

                            if (status == NetConnectionStatus.Connected)
                            {
                                var player = NetUtility.ToHexString(message.SenderConnection.RemoteUniqueIdentifier);
                                //Add our player to dictionary
                                players.Add(player);

                                Logging.Info("Player Online now");
                                players.ForEach(player =>
                                {
                                    Logging.Info(player);
                                });

                                ////Send Player ID
                                //SendLocalPlayerPacket(message.SenderConnection, player);


                                //// Send Spawn Info

                                //SpawnPlayers(all, message.SenderConnection, player);
                            }
                            break;
                        case NetIncomingMessageType.Data:
                            // Get package type
                            byte type = message.ReadByte();

                            // Create package
                            if (Enum.IsDefined(typeof(PacketTypes.General), type))
                            {
                                HandleGeneralPacket((PacketTypes.General)type, message);
                            }
                            else if (Enum.IsDefined (typeof(PacketTypes.Shop), type))
                            {
                                HandleShopPacket((PacketTypes.Shop)type, message);
                            }
                            {
                                Logging.Error("Unhandled packet type");
                            }
                            break;

                        case NetIncomingMessageType.DebugMessage:
                        case NetIncomingMessageType.ErrorMessage:
                        case NetIncomingMessageType.WarningMessage:
                        case NetIncomingMessageType.VerboseDebugMessage:
                            string text = message.ReadString();
                            Logging.Debug(text);
                            break;
                        default:
                            Logging.Error("Unhandle type: " + message.MessageType + " " + message.LengthBytes + " bytes" + message.DeliveryMethod);
                            break;
                    }

                    server.Recycle(message);

                }
            }
        }

        

        private void HandleGeneralPacket(PacketTypes.General type, NetIncomingMessage message)
        {
            switch (type)
            {
                case (byte)PacketTypes.General.Login:
                    string deviceId = NetUtility.ToHexString(message.SenderConnection.RemoteUniqueIdentifier);
                    Logging.Info("Type: Login, From: " + deviceId);

                    var loginPacket = new Login();
                   
                    loginPacket.NetIncomingMessageToPacket(message);
                    SendLoginPackage(loginPacket, message.SenderConnection, deviceId);
                    
                    break;
                default:
                    Logging.Error("Unhandle Data / Package type");
                    break;
            }
        }
        private void HandleShopPacket(PacketTypes.Shop type, NetIncomingMessage message)
        {
            throw new NotImplementedException();
        }


        private async void SendLoginPackage(Login packet, NetConnection user, string deviceId)
        {
            var userController = _serviceProvider.GetRequiredService<UserController>();
            

            if (userController.Login(packet.username, packet.password).Result)
            {
                packet.isSuccess = true;
                var currentUser = userController.getUserInfoByUserNameAsync(packet.username).Result;
                var loginHistoryController = _serviceProvider.GetRequiredService<LoginHistoryController>();

                await loginHistoryController.NewUserLoginAsync(currentUser.Id, deviceId);
                await userController.SetUserOnlineAsync(currentUser);
                Logging.Info("UserLogin Successful, Save User Login History");
            }
            else
            {
                packet.isSuccess = false;
            }
            NetOutgoingMessage outmsg = server.CreateMessage();
            new Login()
            {
                isSuccess = packet.isSuccess,
                username = packet.username,
                password = packet.password
            }.PacketToNetOutGoingMessage(outmsg);
            Logging.Info("Send Login Package to User");
            server.SendMessage(outmsg, user, NetDeliveryMethod.ReliableOrdered, 0);

        }

    }
}
