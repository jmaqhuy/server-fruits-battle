using System.Net;
using Lidgren.Network;
using LidgrenServer.Controllers;
using Microsoft.Extensions.DependencyInjection;
namespace LidgrenServer
{

    public class PacketProcessors
    {
        private NetServer server;
        private Thread thread;
        private List<string> players;
        
        private readonly IServiceProvider _serviceProvider;

        public PacketProcessors(IServiceProvider serviceProvider)
        {
            players = new List<string>();
            
            NetPeerConfiguration config = new NetPeerConfiguration("game")
            {
                Port = 14242
            };
            config.EnableMessageType(NetIncomingMessageType.DiscoveryRequest);
            server = new NetServer(config);
            server.Start();

            _serviceProvider = serviceProvider;

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
                        case NetIncomingMessageType.DiscoveryRequest:
                            Logging.Debug("Received DiscoveryRequest");

                            // Respond to discovery requests
                            server.SendDiscoveryResponse(null, message.SenderEndPoint);
                            Logging.Debug("Send Discovery Response");
                            break;

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
                            else
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
            string deviceId = NetUtility.ToHexString(message.SenderConnection.RemoteUniqueIdentifier);
            switch (type)
            {
                case PacketTypes.General.Login:
                    
                    Logging.Info("Type: Login, From: " + deviceId);

                    var loginPacket = new Login();
                   
                    loginPacket.NetIncomingMessageToPacket(message);
                    SendLoginPackage(loginPacket, message.SenderConnection, deviceId);
                    
                    break;

                case PacketTypes.General.SignUp:
                    
                    Logging.Info("Type: SignUp, From: " + deviceId);
                    var signUpPacket = new SignUp();
                    signUpPacket.NetIncomingMessageToPacket(message);
                    SendSignUpPackage(signUpPacket, message.SenderConnection);
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
                
                var currentUser = userController.getUserInfoByUserNameAsync(packet.username).Result;
                var loginHistoryController = _serviceProvider.GetRequiredService<LoginHistoryController>();

                if (loginHistoryController.UserOnlineNow(currentUser.Id).Result)
                {
                    packet.isSuccess = false;
                    Logging.Error("Account Login in another Device");
                }
                else 
                {
                    packet.isSuccess = true;
                    await loginHistoryController.NewUserLoginAsync(currentUser.Id);
                    //await userController.SetUserOnlineAsync(currentUser);
                    Logging.Info("UserLogin Successful, Save User Login History");
                }
                
            }
            else
            {
                packet.isSuccess = false;
                Logging.Error("Incorrect Username or Password");
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
        private void SendSignUpPackage(SignUp signUpPacket, NetConnection user)
        {
            var userController = _serviceProvider.GetRequiredService<UserController>();
            if (userController.SignUp(signUpPacket.username, signUpPacket.password).Result)
            {
                signUpPacket.isSuccess = true;
                Logging.Info("Sign Up Successful, welcome");
            }
            else 
            {
                signUpPacket.isSuccess = false;
                signUpPacket.reason = "User Already Exists!";
            }
            NetOutgoingMessage outmsg = server.CreateMessage();
            new SignUp()
            {
                isSuccess = signUpPacket.isSuccess,
                username = signUpPacket.username,
                password = signUpPacket.password,
                reason = signUpPacket.reason
            }.PacketToNetOutGoingMessage(outmsg);
            Logging.Info("Send Sign Up Package to User");
            server.SendMessage(outmsg, user, NetDeliveryMethod.ReliableOrdered, 0);

        }

    }
}
