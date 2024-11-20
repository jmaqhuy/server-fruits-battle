using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using Lidgren.Network;
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
            //config.LocalAddress = IPAddress.Parse("192.168.126.50");
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

                    List<NetConnection>all = server.Connections;
                    switch (message.MessageType)
                    {
                        case NetIncomingMessageType.StatusChanged:
                            NetConnectionStatus status = (NetConnectionStatus)message.ReadByte();
                            string reason = message.ReadString();

                            if(status == NetConnectionStatus.Connected)
                            {
                                var player = NetUtility.ToHexString(message.SenderConnection.RemoteUniqueIdentifier);
                                //Add our player to dictionary
                                players.Add(player);

                                //Send Player ID
                                SendLocalPlayerPacket(message.SenderConnection, player);


                                // Send Spawn Info

                                SpawnPlayers(all,message.SenderConnection,player);
                            }
                            break;
                        case NetIncomingMessageType.Data:
                            // Get package type
                            byte type = message.ReadByte();

                            // Create package

                            Packet packet;

                            switch (type)
                            {
                                case (byte)PacketTypes.PositionPacket:
                                    packet = new PositionPacket();
                                    packet.NetIncomingMessageToPacket(message);
                                    SendPositionPacket(all,(PositionPacket)packet);
                                    break;
                                case (byte)PacketTypes.PlayerDisconnectsPacket:
                                    packet = new PLayerDisconnectsPacket();
                                    packet.NetIncomingMessageToPacket(message);
                                    SendPlayerDisconnectPacket(all,(PLayerDisconnectsPacket)packet);
                                    break;
                                default:
                                    Logging.Error("Unhandle Data / Package type");
                                    break;
                            }
                            break ;
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


        public void SpawnPlayers(List<NetConnection>all,NetConnection local,string player)
        {
            //Spawn all client on local player
            all.ForEach(p => 
            {
                string _player = NetUtility.ToHexString(p.RemoteUniqueIdentifier);  
                if (player != _player)
                    SendSpawnPacketToLocal(local, _player, playerPositions[_player].X, playerPositions[_player].Y);
            });

            // Spawn the local player on all client

            Random random = new Random();
            SendSpawnPacketToAll(all,player,random.Next(-3,3),random.Next(-3,3));
        }


        public void SendLocalPlayerPacket(NetConnection local, string player)
        {
            Logging.Info("Sending player their user ID: "+player);

            NetOutgoingMessage message = server.CreateMessage();

            new LocalPlayerPacket() { ID = player}.PacketToNetOutGoingMessage(message);
            server.SendMessage(message, local, NetDeliveryMethod.ReliableOrdered, 0);
        }
        public void SendSpawnPacketToLocal(NetConnection local, string player, float X, float Y) 
        {
            Logging.Info("Sending user spawn info for player: " + player);

            playerPositions[player] = new PlayerPosition() { X = X, Y = Y };
            NetOutgoingMessage message = server.CreateMessage();

            new SpawnPacket() { player = player, X = X, Y = Y }.PacketToNetOutGoingMessage(message);
            server.SendMessage(message, local, NetDeliveryMethod.ReliableOrdered, 0);
        }

        public void SendSpawnPacketToAll(List<NetConnection>all, string player, float X, float Y)
        {
            Logging.Info("Sending user spawn info for player: " + player);

            playerPositions[player] = new PlayerPosition() { X = X, Y = Y };
            NetOutgoingMessage message = server.CreateMessage();

            new SpawnPacket() { player = player, X = X, Y = Y }.PacketToNetOutGoingMessage(message);
            server.SendMessage(message, all, NetDeliveryMethod.ReliableOrdered, 0);
        }
        public void SendPositionPacket(List<NetConnection> all, PositionPacket packet) 
        {
            Logging.Info("Sending position for " + packet.player);

            playerPositions[packet.player] = new PlayerPosition() { X = packet.X, Y = packet.Y };

            NetOutgoingMessage message = server.CreateMessage();
            packet.PacketToNetOutGoingMessage(message);
            server.SendMessage(message,all,NetDeliveryMethod.ReliableOrdered,0);
        }


        public void SendPlayerDisconnectPacket(List<NetConnection> all, PLayerDisconnectsPacket packet) 
        {
            Logging.Info("Player disconnect: " + packet.player);
            
            playerPositions.Remove(packet.player);
            players.Remove(packet.player);
            
            NetOutgoingMessage message = server.CreateMessage();
            packet.PacketToNetOutGoingMessage(message);
            server.SendMessage(message, all, NetDeliveryMethod.ReliableOrdered, 0);
        }
    }
}
