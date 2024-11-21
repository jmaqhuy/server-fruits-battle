using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Lidgren.Network;
using LidgrenServer.controllers;
using LidgrenServer.services;
using LidgrenServer.Data;
using System.Net;

namespace LidgrenServer
{
    public class ServerConnectionManage
    {
        private readonly NetServer server;
        private readonly NetPeerConfiguration config;
        private readonly CancellationTokenSource cts;

        public ServerConnectionManage()
        {
            config = new NetPeerConfiguration("game")
            {
                MaximumConnections = 20,
                LocalAddress = IPAddress.Parse("192.168.0.107"),
                Port = 14242
            };

            server = new NetServer(config);
            cts = new CancellationTokenSource();

            server.Start();
            _ = ListenAsync(cts.Token);
        }

        public async Task ListenAsync(CancellationToken token)
        {
            Logging.Info("Listening for client ...");
            try
            {
                while (!token.IsCancellationRequested)
                {
                    await Task.Delay(10, token);

                    NetIncomingMessage message;

                    while ((message = server.ReadMessage()) != null)
                    {
                        Logging.Info("Message Received");
                        switch (message.MessageType)
                        {
                            case NetIncomingMessageType.StatusChanged:
                                NetConnectionStatus status = (NetConnectionStatus)message.ReadByte();
                                string reason = message.ReadString();

                                if (status == NetConnectionStatus.Connected)
                                {
                                    //Add player to Dic

                                    //Send Player ID

                                    //Send Sqawn Info
                                }
                                break;

                            case NetIncomingMessageType.Data:
                                //get packet type:
                                byte type = message.ReadByte();
                                
                                

                                //create packet
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
                                        Logging.Error("Unhandled Data / Packet Type");
                                        break;
                                }
                                break;

                            

                            case NetIncomingMessageType.DebugMessage:
                            case NetIncomingMessageType.ErrorMessage:
                            case NetIncomingMessageType.WarningMessage:
                            case NetIncomingMessageType.VerboseDebugMessage:
                                Logging.Debug(message.ReadString());
                                break;
                            default:
                                Logging.Error("Unhandled type: " + message.MessageType + "\n"
                                    + "Message LengthBytes: " + message.LengthBytes + "bytes\n"
                                    + message.DeliveryMethod);
                                break;
                        }
                        server.Recycle(message);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                Logging.Info("Server stopped listening.");
            }
            finally
            {
                server.Shutdown("Server shutting down...");
                Logging.Info("Server shut down complete.");
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
                isSuccess = packet.isSuccess, username = packet.username, password = packet.password 
            }.PacketToNetOutGoingMessage(outmsg);
            Logging.Info("Send Login Package to User");
            server.SendMessage(outmsg, user, NetDeliveryMethod.ReliableOrdered, 0);
            server.FlushSendQueue();
        }

        public void Stop()
        {
            cts.Cancel();
        }
    }
}
