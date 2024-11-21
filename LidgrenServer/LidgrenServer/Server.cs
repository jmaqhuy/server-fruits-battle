using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using Lidgren.Network;
using LidgrenServer.controllers;
using LidgrenServer.Data;
using LidgrenServer.services;
using Org.BouncyCastle.Security;
namespace LidgrenServer
{
    public class PlayerPosition
    {
        public float X { get; set; }
        public float Y { get; set; }
    }
    public class Server
    {
        private NetServer server;
        private Thread thread;
        private List<string> players;
        private Dictionary<string, PlayerPosition> playerPositions;

        public Server()
        {
            players = new List<string>();
            playerPositions = new Dictionary<string, PlayerPosition>();

            NetPeerConfiguration config = new NetPeerConfiguration("game");

            config.MaximumConnections = 20;
            config.LocalAddress = IPAddress.Parse("192.168.0.107");
            config.Port = 14242;
            server = new NetServer(config);
            server.Start();

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

                            Packet packet;

                            switch (type)
                            {
                                case (byte)PacketTypes.Login:
                                    Logging.Info("Type: Login, From: " + NetUtility.ToHexString(message.SenderConnection.RemoteUniqueIdentifier));
                                    packet = new Login();
                                    packet.NetIncomingMessageToPacket(message);
                                    SendLoginPackage((Login)packet, message.SenderConnection);

                                    break;
                                default:
                                    Logging.Error("Unhandle Data / Package type");
                                    break;
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



        private void SendLoginPackage(Login packet, NetConnection user)
        {
            var userService = new UserService(new ApplicationDataContext());
            var userController = new UserController(userService);

            if (userController.Login(packet.username, packet.password).Result)
            {
                packet.isSuccess = true;
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
