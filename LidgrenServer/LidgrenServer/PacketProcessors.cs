using Lidgren.Network;
using LidgrenServer.Controllers;
using LidgrenServer.models;
using LidgrenServer.Packets;
using Microsoft.Extensions.DependencyInjection;
namespace LidgrenServer
{

    public class PacketProcessors
    {
        private NetServer server;
        private Thread thread;
        


        private List<Player> PlayerOnlineList = new List<Player>();
        private List<Room> RoomList = new List<Room> {};
        private Random random = new Random();
        
        private readonly IServiceProvider _serviceProvider;

        public PacketProcessors(IServiceProvider serviceProvider)
        {
           
            NetPeerConfiguration config = new NetPeerConfiguration("FruitsBattle2DGame")
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
                            Logging.Debug(NetUtility.ToHexString(message.SenderConnection.RemoteUniqueIdentifier) + " " + status + ": " + reason);
                            //if (status == NetConnectionStatus.Connected)
                            //{

                            //} 
                            //if (status == NetConnectionStatus.Disconnected)
                            //{

                            //    bool anonymous = true;
                            //    NetConnection netConnection = message.SenderConnection;
                            //    foreach (var playeronline in PlayerOnlineList)
                            //    {
                            //        if (playeronline.netConnection == netConnection)
                            //        {
                                        
                            //            PlayerOnlineList.Remove(playeronline);
                            //            foreach (var room in RoomList)
                            //            {
                            //                foreach (var playerinroom in room.playersList)
                            //                {
                            //                    if (playerinroom.netConnection == netConnection)
                            //                    {
                            //                        anonymous = false;
                            //                        Logging.Info($"Player {playerinroom.User.Username} quit game");
                            //                        room.playersList.Remove(playerinroom);
                            //                    }
                            //                }
                            //            }

                            //            break;
                            //        }
                            //    }

                            //    if (anonymous)
                            //    {
                            //        Logging.Warn($"Anonymous user {netConnection} disconnected");
                            //    }

                            //}
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
                            else if (Enum.IsDefined (typeof(PacketTypes.Room), type))
                            {
                                HandleRoomPacket((PacketTypes.Room)type, message);
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

        private void HandleRoomPacket(PacketTypes.Room type, NetIncomingMessage message)
        {
            Packet packet;
            switch (type)
            {
                case PacketTypes.Room.JoinRoomPacket:
                    Logging.Info("Received Join Room Packet");
                    packet = new JoinRoomPacket();
                    packet.NetIncomingMessageToPacket(message);
                    Room newRoom = SendJoinRoomPacket((JoinRoomPacket)packet, message.SenderConnection);
                    SendJoinRoomPacketToAll(newRoom);
                    break;

                case PacketTypes.Room.ExitRoomPacket:
                    Logging.Info("Received Exit Room Packet");
                    packet = new ExitRoomPacket();
                    packet.NetIncomingMessageToPacket(message);
                    // send to all

                    Room room = RoomList.FirstOrDefault(r => r.Id == ((ExitRoomPacket)packet).roomId);
                    Player player = PlayerOnlineList.FirstOrDefault(u => u.User.Username == ((ExitRoomPacket)packet).username);

                    if (room != null && player != null)
                    {
                        RemovePlayerInRoom(player, room);
                        SendJoinRoomPacketToAll(room);
                    }
                    break;

                default:
                    Logging.Error("Unhandle Data / Package type, typeof Room");
                    break;
            }

        }

        private void SendJoinRoomPacketToAll(Room room)
        {
            List<NetConnection> connections = room.playersList
                                         .Where(player => player.netConnection != null)
                                         .Select(player => player.netConnection)
                                         .ToList();
            
            // No One in room now
            if (connections.Count <= 0) return;

            var host = room.playersList.FirstOrDefault(p => p.IsHost);
            if (host == null)
            {
                room.playersList.First().IsHost = true;
            }

            List<PlayerInRoomPacket> playerInRoomPackets = new List<PlayerInRoomPacket>();
            foreach (var player in room.playersList)
            {
                playerInRoomPackets.Add(new PlayerInRoomPacket()
                {
                    username = player.User.Username,
                    displayname = player.User.Display_name,
                    isHost = player.IsHost,
                    team = player.team,
                    Position = player.Position,
                    isReady = player.isReady,
                    
                });
            }
            NetOutgoingMessage outmsg = server.CreateMessage();
            new JoinRoomPacketToAll()
            {
                Players = playerInRoomPackets,
            }.PacketToNetOutGoingMessage(outmsg);
            server.SendMessage(outmsg, connections, NetDeliveryMethod.ReliableOrdered, 0);
        }

        private Room SendJoinRoomPacket(JoinRoomPacket joinRoomPacket, NetConnection netConnection)
        {
            
            Room? thisRoom = RoomList?.FirstOrDefault(room => 
                    room.roomMode == joinRoomPacket.room.roomMode &&
                    !room.IsRoomFull &&
                    room.roomStatus == RoomStatus.InLobby &&
                    room.RoomType == joinRoomPacket.room.roomType
            );

            var player = PlayerOnlineList?.FirstOrDefault(u => u.netConnection == netConnection);

            if (thisRoom == null)
            {
                thisRoom = CreateNewRoom(joinRoomPacket);
                player.IsHost = true;
                player.team = Team.Team1;
                player.Position = 1;
            }
            else
            {
                int maxPlayers = thisRoom.MaxPlayers;
                var availablePositions1 = Enumerable.Range(1, 4).ToList();
                var availablePositions2 = Enumerable.Range(5, 4).ToList();

                int numberOfPlayerTeam1 = 0;
                int numberOfPlayerTeam2 = 0;

                foreach (var playerInRoom in thisRoom.playersList)
                {
                    if (playerInRoom.team == Team.Team1) 
                    { 
                        availablePositions1.Remove(playerInRoom.Position);
                        numberOfPlayerTeam1++;
                    } else
                    {
                        availablePositions2.Remove(playerInRoom.Position);
                        numberOfPlayerTeam2++;
                    }
                }

                if (numberOfPlayerTeam1 > numberOfPlayerTeam2)
                {
                    player.team = Team.Team2;
                    player.Position = availablePositions2[random.Next(availablePositions2.Count)];
                }
                else
                {
                    player.team = Team.Team1;
                    player.Position = availablePositions1[random.Next(availablePositions1.Count)];
                }
                player.IsHost = false;

            }
            
            
            thisRoom.playersList.Add(player);
            Logging.Info($"User {player.User.Username} Join Room {thisRoom.Id}");
            RoomList.Add(thisRoom);


            NetOutgoingMessage outmsg = server.CreateMessage();
            new JoinRoomPacket()
            {
                room = new RoomPacket()
                {
                    Id = thisRoom.Id,
                    Name = thisRoom.Name,
                    roomMode = thisRoom.roomMode,
                    roomStatus = thisRoom.roomStatus,
                    roomType = thisRoom.RoomType,
                }
            }.PacketToNetOutGoingMessage(outmsg);

            server.SendMessage(outmsg, netConnection, NetDeliveryMethod.ReliableOrdered, 0);
            Logging.Info("Response Join Room Packet: Room ID: " + thisRoom.Id);
            return thisRoom;
        }


        private Room CreateNewRoom(JoinRoomPacket joinRoomPacket)
        {
            int newRoomId;
            do
            {
                newRoomId = random.Next(1000, 10000); 
            } while (RoomList.Any(room => room.Id == newRoomId)); 

            Room thisRoom = new Room()
            {
                Id = newRoomId,
                Name = $"Room {newRoomId}",
                roomMode = joinRoomPacket.room.roomMode,
                roomStatus = RoomStatus.InLobby,
                RoomType = joinRoomPacket.room.roomType, 
                playersList = new List<Player>()
            };
            Logging.Info($"New Room Created: Id {thisRoom.Id}, PlayerInRoom {thisRoom.playersList.Count}");
            return thisRoom;
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

                case PacketTypes.General.BasicUserInfoPacket:
                    Logging.Info("Type: Request for basic user info");
                    var basicUserInfoPacket = new BasicUserInfoPacket();
                    basicUserInfoPacket.NetIncomingMessageToPacket(message);
                    SendBasicUserInfoPacket(basicUserInfoPacket, message.SenderConnection);

                    break;

                case PacketTypes.General.ChangeDisplayNamePacket:
                    Logging.Info("Type: Request for Change DisplayName");
                    var changeDisplayNamePacket = new ChangeDisplayNamePacket();
                    changeDisplayNamePacket.NetIncomingMessageToPacket(message);
                    SendChangeDisplayNamePacket(changeDisplayNamePacket, message.SenderConnection);
                    break;

                case PacketTypes.General.PlayerDisconnectsPacket:
                    var playerDisconnectPacket = new PlayerDisconnectsPacket();
                    playerDisconnectPacket.NetIncomingMessageToPacket(message);


                    var username = playerDisconnectPacket.username;
                    // Check if user is logged in, if so remove from PlayerOnlineList
                    if (!string.IsNullOrEmpty(username))
                    {
                        RemovePlayerInPlayerOnlineListAndRoom(username);
                    }

                    break;

                default:
                    Logging.Error("Unhandle Data / Package type");
                    break;
            }
        }
        private void RemovePlayerInPlayerOnlineListAndRoom(string username)
        {
            var player = PlayerOnlineList.FirstOrDefault(p => p.User.Username == username);
            if (player != null)
            {
                Logging.Debug($"Player {player.User.Username} offline");
                PlayerOnlineList.Remove(player);
                RemovePlayerInRoom(player);
            }
        }
        private void RemovePlayerInRoom(Player player, Room room)
        {
            room.playersList.Remove(player);
            Logging.Warn($"Player {player.User.Display_name} Exit Room {room.Id}");
            if (room.playersList.Count == 0)
            {
                Logging.Warn($"Room {room.Id} remove because no player in room!");
                Room selectroom = RoomList.FirstOrDefault(r => r == room);
                RoomList.Remove(selectroom);
            }
        }

        private void RemovePlayerInRoom(Player player)
        {
            foreach (var room in RoomList.ToList())
            { 
                var playerInRoom = room.playersList.FirstOrDefault(p => p == player);
                if (playerInRoom != null)
                {
                    RemovePlayerInRoom(playerInRoom, room);
                    SendJoinRoomPacketToAll(room);
                    break;
                }
            }
        }


        private async void SendChangeDisplayNamePacket(ChangeDisplayNamePacket changeDisplayNamePacket, NetConnection senderConnection)
        {
            var userController = _serviceProvider.GetService<UserController>();
            await userController.ChangeDisplayName(changeDisplayNamePacket.username, changeDisplayNamePacket.newDisplayName);

            NetOutgoingMessage outmsg = server.CreateMessage();
            new ChangeDisplayNamePacket()
            {
                username = changeDisplayNamePacket.username,
                newDisplayName = changeDisplayNamePacket.newDisplayName,
                error = "",
                Ok = true,
            }.PacketToNetOutGoingMessage(outmsg);

            Logging.Info("Send ChangeDisplayNamePacket to User");
            server.SendMessage(outmsg, senderConnection, NetDeliveryMethod.ReliableOrdered, 0);
        }

        private void SendBasicUserInfoPacket(BasicUserInfoPacket basicUserInfoPacket, NetConnection senderConnection)
        {
            var userController = _serviceProvider.GetService<UserController>();
            var user = userController.getUserInfoByUserNameAsync(basicUserInfoPacket.userName).Result;
            if ( user != null) 
            {
                Logging.Debug($"All User Info: name {user.Username}, coin: {user.Coin}, Id: {user.Id}, Display_name: {user.Display_name}");
                NetOutgoingMessage outmsg = server.CreateMessage();
                new BasicUserInfoPacket()
                {
                    userName = basicUserInfoPacket.userName,
                    coin = user.Coin,
                    displayName = user.Display_name
                }.PacketToNetOutGoingMessage(outmsg);

                Logging.Info("Send Sign Up Package to User");

                server.SendMessage(outmsg, senderConnection, NetDeliveryMethod.ReliableOrdered, 0);

            }
        }

        private void HandleShopPacket(PacketTypes.Shop type, NetIncomingMessage message)
        {
            throw new NotImplementedException();
        }


        private async void SendLoginPackage(Login packet, NetConnection userNetConnection, string deviceId)
        {
            var userController = _serviceProvider.GetRequiredService<UserController>();
            // Login return a UserModel if username and password are correct
            var currentUser = userController.Login(packet.username, packet.password).Result;
            if ( currentUser != null)
            {
                bool isUserOnline = false;
                //var loginHistoryController = _serviceProvider.GetRequiredService<LoginHistoryController>();


                //check in PlayerOnlineList, if exists , reject, else accept
                foreach (var u in PlayerOnlineList)
                {
                    if (u.User == currentUser)
                    {
                        isUserOnline = true;
                        packet.isSuccess = false;
                        Logging.Error("Account Login in another Device");
                        break;
                    }
                }
                if (!isUserOnline)
                {
                    var player = new Player()
                    {
                        User = currentUser,
                        netConnection = userNetConnection
                    };
                    PlayerOnlineList.Add(player);
                    packet.isSuccess = true;
                    
                    // Save in LoginHistory
                    //await loginHistoryController.NewUserLoginAsync(currentUser.Id);
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
            server.SendMessage(outmsg, userNetConnection, NetDeliveryMethod.ReliableOrdered, 0);

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
