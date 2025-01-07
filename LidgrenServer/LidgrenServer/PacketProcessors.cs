using System.Numerics;
using Lidgren.Network;
using LidgrenServer.controllers;
using LidgrenServer.Controllers;
using LidgrenServer.models;
using LidgrenServer.Models;
using LidgrenServer.Packets;
using LidgrenServer.TurnManager;
using Microsoft.Extensions.DependencyInjection;


namespace LidgrenServer
{

    public class PacketProcessors
    {
        private NetServer server;
        private Thread thread;
        


        private List<Player> PlayerOnlineList = new List<Player>();
        private List<RoomInfo> RoomList = new List<RoomInfo> {};
        private List<RoomInfo> MatchmakingRoom = new List<RoomInfo>();
        GameRoomManager roomManager = new GameRoomManager();
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
            RoomList.Add(new RoomInfo
            {
                Id = 1,
                Name = "mode normal 1vs1",
                roomMode = RoomMode.Normal,
                RoomType = RoomType.OneVsOne,
                roomStatus = RoomStatus.InLobby,
                playersList = new List<Player>()
            });
            RoomList.Add(new RoomInfo
            {
                Id = 2,
                Name = "mode normal 2vs2",
                roomMode = RoomMode.Normal,
                RoomType = RoomType.TwoVsTwo,
                roomStatus = RoomStatus.InLobby,
                playersList = new List<Player>()
            });
            RoomList.Add(new RoomInfo
            {
                Id = 3,
                Name = "mode normal 4vs4",
                roomMode = RoomMode.Normal,
                RoomType = RoomType.FourVsFour,
                roomStatus = RoomStatus.InLobby,
                playersList = new List<Player>()
            });
            RoomList.Add(new RoomInfo
            {
                Id = 4,
                Name = "mode rank 2vs2",
                roomMode = RoomMode.Rank,
                RoomType = RoomType.TwoVsTwo,
                roomStatus = RoomStatus.InLobby,
                playersList = new List<Player>()
            });

            while (true) 
            {
                NetIncomingMessage message;

                while ((message = server.ReadMessage()) != null)
                {
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
                            
                            break;
                        case NetIncomingMessageType.Data:
                            // Get package type
                            byte type = message.ReadByte();

                            // Create package
                            if (Enum.IsDefined(typeof(PacketTypes.General), type))
                            {
                                HandleGeneralPacket((PacketTypes.General)type, message);
                            }
                            else if (Enum.IsDefined(typeof(PacketTypes.Shop), type))
                            {
                                HandleShopPacket((PacketTypes.Shop)type, message);
                            }
                            else if (Enum.IsDefined(typeof(PacketTypes.Room), type))
                            {
                                HandleRoomPacket((PacketTypes.Room)type, message);
                            }
                            else if (Enum.IsDefined(typeof(PacketTypes.Friend), type))
                            {
                                HandleFriendPacket((PacketTypes.Friend)type, message);
                            }
                            else if (Enum.IsDefined(typeof(PacketTypes.Character), type))
                            {
                                HandleCharacterPacket((PacketTypes.Character)type, message);
                            }
                            else if (Enum.IsDefined(typeof(PacketTypes.GameBattle), type))
                            {
                                HandleGameBattlePacket((PacketTypes.GameBattle)type, message);
                            }
                            else if (Enum.IsDefined(typeof(PacketTypes.Rank), type))
                            {
                                HandleRankPacket((PacketTypes.Rank)type, message);
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
        private async void HandleRankPacket(PacketTypes.Rank type, NetIncomingMessage message)
        {
            Packet packet;
            switch (type)
            {
                case PacketTypes.Rank.CurrentRankPacket:
                    Logging.Info($"Received CurrentRankPacket from {NetUtility.ToHexString(message.SenderConnection.RemoteUniqueIdentifier)}");
                    packet = new CurrentRankPacket();
                    packet.NetIncomingMessageToPacket(message);
                    Logging.Info($"Get CurrentRankPacket from {((CurrentRankPacket)packet).username}");
                    SendCurrentRankPacket((CurrentRankPacket)packet, message.SenderConnection);
                    break;

                case PacketTypes.Rank.MatchmakingPacket:
                    packet = new MatchmakingPacket();
                    packet.NetIncomingMessageToPacket(message);
                    if (((MatchmakingPacket)packet).start)
                    {
                        StartMatchMaking(((MatchmakingPacket)packet).roomId);
                    }
                    else
                    {
                        StopMatchMaking(((MatchmakingPacket)packet).roomId);
                    }

                    break;
            }
        }

        private void StartMatchMaking(int roomId)
        {
            var currentRoom = RoomList.FirstOrDefault(r => r.Id == roomId);
            if (currentRoom != null)
            {
                List<NetConnection> connections = currentRoom.playersList
                                         .Where(player => player.netConnection != null)
                                         .Select(player => player.netConnection)
                                         .ToList();
                NetOutgoingMessage outmsg = server.CreateMessage();
                new MatchmakingPacket()
                {
                    roomId = roomId,
                    start = true,
                }.PacketToNetOutGoingMessage(outmsg);
                server.SendMessage(outmsg, connections, NetDeliveryMethod.ReliableOrdered, 0);

                // find room
                var copyRoom = currentRoom.Clone();
                MatchmakingRoom.Add(copyRoom);

                foreach (var targetRoom in MatchmakingRoom)
                {
                    if (targetRoom.Id == copyRoom.Id) continue;
                    if (targetRoom.roomStatus == RoomStatus.InMatch) continue;
                    if (targetRoom.RoomType == copyRoom.RoomType)
                    {
                        Team currentTargetRoomTeam = Team.Team1;
                        
                        List<NetConnection> targetRoomConnection = new List<NetConnection>();
                        List<NetConnection> copyRoomConnection = new List<NetConnection>();
                        foreach (var player in targetRoom.playersList)
                        {
                            currentTargetRoomTeam = player.team;
                            player.isReady = true;
                            targetRoomConnection.Add(player.netConnection);
                            Logging.Info($"{player.User.Display_name} in team {player.team} now");
                        }
                        Logging.Info($"Current target room team: {currentTargetRoomTeam}");
                        Team CopyRoomTeam;
                        if (currentTargetRoomTeam == Team.Team1)
                        {
                            CopyRoomTeam = Team.Team2;
                        } else
                        {
                            CopyRoomTeam = Team.Team1;
                        }
                        Logging.Info($"Copy Room Team is {CopyRoomTeam}");
                        Team currentCopyRoomTeam = Team.Team1;
                        for (int i = 0; i < copyRoom.playersList.Count; i++)
                        {
                            currentCopyRoomTeam = copyRoom.playersList[i].team;
                            copyRoom.playersList[i].team = CopyRoomTeam;
                            copyRoom.playersList[i].isReady = true;
                            copyRoomConnection.Add(copyRoom.playersList[i].netConnection);
                            currentRoom.playersList[i].team = CopyRoomTeam;
                            Logging.Info($"{copyRoom.playersList[i].User.Display_name} in team {copyRoom.playersList[i].team} now");
                        }
                        
                        if (CopyRoomTeam != currentCopyRoomTeam)
                        {
                            Logging.Info("Change Team vao sau to Team 2");
                            outmsg = server.CreateMessage();
                            new ChangeTeamPacket()
                            {
                                team = CopyRoomTeam,
                            }.PacketToNetOutGoingMessage(outmsg);
                            server.SendMessage(outmsg, copyRoomConnection, NetDeliveryMethod.ReliableOrdered, 1);
                           
                        }

                        targetRoom.playersList.AddRange(copyRoom.playersList);
                        copyRoom.playersList = targetRoom.playersList;
                        StopMatchMaking(targetRoom.Id, true);
                        
                    }
                }
            }

        }
        private void StopMatchMaking(int roomId, bool matchFound = false)
        {
            RoomInfo? room;
            if (matchFound)
            {
                room = MatchmakingRoom.FirstOrDefault(r => r.Id == roomId);
                if (room != null)
                {
                    var rlist = MatchmakingRoom.Where(r => r.playersList == room.playersList).ToList();
                    foreach (var roomInRlist in rlist)
                    {
                        roomInRlist.roomStatus = RoomStatus.InMatch;
                    }
                }
            }
            else
            {
                room = RoomList.FirstOrDefault(r => r.Id == roomId);
                foreach (var pl in room.playersList)
                {
                    if (pl.IsHost) pl.isReady = false;
                }
                var removeRoom = MatchmakingRoom.FirstOrDefault(r => r.Id == roomId);
                MatchmakingRoom.Remove(removeRoom);
            }
            if (room != null)
            {
                List<NetConnection> connections = room.playersList
                                         .Where(player => player.netConnection != null)
                                         .Select(player => player.netConnection)
                                         .ToList();
                NetOutgoingMessage outmsg = server.CreateMessage();
                new MatchmakingPacket()
                {
                    roomId = roomId,
                    start = false,
                    matchFound = matchFound,
                }.PacketToNetOutGoingMessage(outmsg);
                server.SendMessage(outmsg, connections, NetDeliveryMethod.ReliableOrdered, 0);

            }
        }

        private void SendCurrentRankPacket(CurrentRankPacket packet, NetConnection senderConnection)
        {
            var urC = _serviceProvider.GetService<UserRankController>();
            var ur = urC.GetUserRank(packet.username).Result;
            NetOutgoingMessage outmsg = server.CreateMessage();
            new CurrentRankPacket()
            {
                username = ((CurrentRankPacket)packet).username,
                rankId = ur.RankId,
                rankName = ur.Rank.Name,
                rankAssetName = ur.Rank.AssetName,
                currentStar = ur.CurrentStar,
                seasonId = ur.SeasonId,
                seasonName = ur.Season.Name,

            }.PacketToNetOutGoingMessage(outmsg);
            server.SendMessage(outmsg, senderConnection, NetDeliveryMethod.ReliableOrdered, 0);
        }

        private void HandleGameBattlePacket(PacketTypes.GameBattle type, NetIncomingMessage message)
        {
            string deviceId = NetUtility.ToHexString(message.SenderConnection.RemoteUniqueIdentifier);
            List<NetConnection> players = new List<NetConnection>();
            Packet packet;
            switch (type)
            {
                case PacketTypes.GameBattle.StartGamePacket:
                    Logging.Info("get start game signal from " + NetUtility.ToHexString(message.SenderConnection.RemoteUniqueIdentifier));
                    packet = new StartGamePacket();
                    packet.NetIncomingMessageToPacket(message);
                    StartGame((StartGamePacket)packet, players, message.SenderConnection);
                    break;
                case PacketTypes.GameBattle.PlayerOutGamePacket:
                    break;
                case PacketTypes.GameBattle.EndTurnPacket:

                    Logging.Debug("get end turn signal");
                    packet = new EndTurnPacket();
                    packet.NetIncomingMessageToPacket(message);
                    ResetTurn((EndTurnPacket)packet, message);
                    break;
                case PacketTypes.GameBattle.EndGamePacket:
                    break;
                case PacketTypes.GameBattle.PositionPacket:
                    packet = new PositionPacket();
                    packet.NetIncomingMessageToPacket(message);
                    SendPositionPacket(players, (PositionPacket)packet, message);
                    break;
                case PacketTypes.GameBattle.HealthPointPacket:
                    packet = new HealthPointPacket();
                    packet.NetIncomingMessageToPacket(message);
                    SendHPPacket((HealthPointPacket)packet, message, players);
                    Logging.Debug("Send HP packet");
                    break;
                case PacketTypes.GameBattle.PlayerDiePacket:
                    packet = new PlayerDiePacket();
                    packet.NetIncomingMessageToPacket(message);
                    UpdatePLayerDie((PlayerDiePacket)packet, message);
                    
                    break;
                case PacketTypes.GameBattle.Shoot:
                    packet = new Shoot();
                    packet.NetIncomingMessageToPacket(message);
                    SendShootPacket((Shoot)packet, message, players);
                    Logging.Debug("Send shoot packet");
                    break;


            }

        }
        public void UpdatePLayerDie(PlayerDiePacket packet,NetIncomingMessage message)
        {

            foreach (var rId in getRoomID(packet.player))

            {

                try
                {
                    roomManager.RemovePlayerDead(rId, message.SenderConnection);
                    
                }
                catch (Exception e)
                {
                    continue;
                }
            }
        }
        public List<int> getRoomID(String playerName)
        {
            RoomInfo room = RoomList
                .Where(room => room.playersList.Any(player => player.User.Username == playerName))
                .FirstOrDefault();
            List<int> roomIDs = new List<int>();
            if (room != null)
            {
                if (room.roomMode == RoomMode.Rank)
                {
                    roomIDs.AddRange(MatchmakingRoom
                        .Where(room => room.playersList.Any(player => player.User.Username == playerName))
                        .Select(room => room.Id)
                        .ToList());
                } else
                {
                    roomIDs.Add(room.Id);
                }
            }
           
            return roomIDs;
        }
        public void RemoveRoomFromRankMatch(List<int> roomIDs)
        {
            foreach (var roomID in roomIDs)
            {
                // Find the room object by its ID
                var roomToRemove = MatchmakingRoom.FirstOrDefault(room => room.Id == roomID);

                // Check if the room exists
                if (roomToRemove != null)
                {
                    // Remove the room from the list
                    MatchmakingRoom.Remove(roomToRemove);
                }
            }
        }
        
        public bool CheckWinGame(String playerName, ref int outRoomId)
        {
            int NumberPlayerTeam1 = 0;
            int NumberPlayerTeam2 = 0;
            
            List<int> roomIDs = getRoomID(playerName);
            List<NetConnection> playersAlive = new List<NetConnection>();
            int roomID = roomIDs[0];
            outRoomId = roomID;
            foreach (var rId in roomIDs)
            {
                try
                {
                    playersAlive = roomManager.getPLayersAlive(rId);
                    
                    roomID = rId;
                    outRoomId = rId;
                    if(playersAlive != null)
                    {
                        Logging.Error("get player alive in roomId: " + rId +" number of player: "+playersAlive.Count);
                        roomID = rId;
                        outRoomId = rId;
                        break;
                    }
                }
                catch (Exception e)
                {
                    continue;
                }
            }
            
            

            var targetRoom = RoomList.FirstOrDefault(room => room.Id == roomID);
            if(targetRoom?.roomMode == RoomMode.Rank)
            {
                targetRoom = MatchmakingRoom.FirstOrDefault(room => room.Id == roomID);
            }
            if (targetRoom != null) 
            {
                foreach (var player in targetRoom.playersList)
                {
                    if (playersAlive.Contains(player.netConnection))
                    {
                        
                        if (player.team == Team.Team1)
                        {
                            NumberPlayerTeam1++;
                        }
                        else
                        {
                            NumberPlayerTeam2++;
                        }
                    }
                }

            }
            if(NumberPlayerTeam1 == 0)
            {
                SendEndGame(roomID,Team.Team2);
                RemoveRoomFromRankMatch(roomIDs);

                return true;
                
            }
            if (NumberPlayerTeam2 == 0) 
            {
                SendEndGame(roomID, Team.Team1);
                RemoveRoomFromRankMatch(roomIDs);
                return true;
            }
            return false;

        }

        private async Task SendEndGame(int roomID, Team TeamWin)
        {
            roomManager.StopTurnManagerForRoom(roomID);
            var ucc = _serviceProvider.GetService<UserCharacterController>();
            Logging.Debug("Send End Game for room " + roomID);
            if (TeamWin == Team.Team1)
            {
                Logging.Warn(" Team 1 win");
            }
            else 
            {
                Logging.Warn(" Team 2 win");
            }
            List<NetConnection> players = new List<NetConnection>();
            var targetRoom = RoomList.FirstOrDefault(room => room.Id == roomID);
            UserRankController? urc = null;
            if (targetRoom.roomMode == RoomMode.Rank)
            {
                targetRoom = MatchmakingRoom.FirstOrDefault(room => room.Id == roomID);
                urc = _serviceProvider.GetService<UserRankController>();
            }
            UserCharacterModel ucm;
            Logging.Warn("startupdatexpforplayer");
            if (targetRoom != null)
            {
                foreach (var player in targetRoom.playersList)
                {
                    ucm = ucc.GetCurrentCharacterAsync(player.User.Id).Result;
                    if (TeamWin == player.team)
                    {
                        ucm.Experience += 45;
                        if (urc != null)
                        {
                            urc.ChangeUserRankStar(player.User.Username, 1);
                        }
                    }
                    else 
                    {
                        ucm.Experience += 30;
                        if (urc != null)
                        {
                            urc.ChangeUserRankStar(player.User.Username, -1);
                        }
                    }
                    if (ucm.Experience >= (int)Math.Pow(ucm.Level, 1.5) * 100) ucm.Level += 1;
                    await ucc.UpdateUserCharacter(ucm);
                    players.Add(player.netConnection);
                }
                Logging.Warn("updatesuccessfully");
                // Add all players from the target room to the players list
                
                targetRoom.roomStatus = RoomStatus.InLobby;
            }
            NetOutgoingMessage outgoingMessage = server.CreateMessage();
            new EndGamePacket() {TeamWin = TeamWin}.PacketToNetOutGoingMessage(outgoingMessage);
            if (players.Count != 0)
            {
                server.SendMessage(outgoingMessage, players, NetDeliveryMethod.ReliableOrdered, 0);
            }
            else
            {
                Logging.Error("No player in list players");
            }
            
        }

        public void SendPlayerDie(String playerName)
        {
            
            List<NetConnection> players = new List<NetConnection>();
            // First, find the room ID that corresponds to the sender's connection
            int roomID = getRoomID(playerName)[0];

            // If roomID is found (not 0), proceed to the second part
            if (roomID != 0)
            {
                // Find the target room using the room ID
                var targetRoom = RoomList.FirstOrDefault(room => room.Id == roomID);
                if(targetRoom.roomMode == RoomMode.Rank)
                {
                    targetRoom = RoomList.FirstOrDefault(room => room.Id == roomID);
                }
                if(targetRoom.roomMode == RoomMode.Rank)
                {
                    targetRoom = MatchmakingRoom.FirstOrDefault(room => room.Id == roomID);
                }
                if (targetRoom != null)
                {
                    // Add all players from the target room to the players list
                    players.AddRange(targetRoom.playersList.Select(player => player.netConnection));
                }

            }
            NetOutgoingMessage outgoingMessage = server.CreateMessage();
            new PlayerDiePacket() { player = playerName }.PacketToNetOutGoingMessage(outgoingMessage);
            if (players.Count != 0)
            {
                server.SendMessage(outgoingMessage, players, NetDeliveryMethod.ReliableOrdered, 0);
            }
            else
            {
                Logging.Error("No player in list players");
            }
            Logging.Debug("Send player die for" + playerName);
            NetConnection playerNetconnection = null;

            // Attempt to find the player's NetConnection from the available room IDs.
            foreach (var rid in getRoomID(playerName))
            {
                if (playerNetconnection == null)
                {
                    playerNetconnection = GetNetConnection(playerName, rid);
                    if (playerNetconnection != null)
                    {
                        break; // Found the connection, no need to check other rooms
                    }
                }
            }

            // Check if playerNetconnection was found and proceed to remove the dead status.
            if (playerNetconnection != null)
            {
                foreach (var rid in getRoomID(playerName))
                {
                    roomManager.RemovePlayerDead(rid, playerNetconnection);
                }
            }
            else
            {
                // Handle the case where the player's NetConnection was not found
                Logging.Error("Player's NetConnection was not found.");
            }

        }
        public NetConnection GetNetConnection(string playerName,int roomId)
        {
            var room = RoomList.FirstOrDefault(r => r.Id == roomId);
            if (room == null || room.playersList == null)
            {
                Logging.Warn($"no found room {roomId} in roomlist");

                return null; // Room or players list is not valid
            }

            // Use LINQ to find the player with the given username
            var player = room.playersList.FirstOrDefault(p => p.User.Username == playerName);
            return player?.netConnection; // Return netConnection if found, else return null
        }

        private void SendHPPacket(HealthPointPacket packet, NetIncomingMessage message, List<NetConnection> players)
        {
            NetConnection mess = message.SenderConnection;
            // First, find the room ID that corresponds to the sender's connection
            int roomID = getRoomID(getPlayerName(mess))[0];

            // If roomID is found (not 0), proceed to the second part
            if (roomID != 0)
            {
                // Find the target room using the room ID
                var targetRoom = RoomList.FirstOrDefault(room => room.Id == roomID);
                if(targetRoom.roomMode == RoomMode.Rank)
                {
                    targetRoom = MatchmakingRoom.FirstOrDefault(room => room.Id == roomID);

                }
                if (targetRoom != null)
                {
                    // Add all players from the target room to the players list
                    players.AddRange(targetRoom.playersList.Select(player => player.netConnection));
                }

            }
            NetOutgoingMessage outgoingMessage = server.CreateMessage();
            new HealthPointPacket() { PlayerName = packet.PlayerName, HP = packet.HP }.PacketToNetOutGoingMessage(outgoingMessage);
            if (players.Count != 0)
            {
                server.SendMessage(outgoingMessage, players, NetDeliveryMethod.ReliableOrdered, 0);
            }
            else
            {
                Logging.Error("No player in list players");
            }
            Logging.Debug("Send HP for" + packet.PlayerName + " " + packet.HP);
            if(packet.HP == 0)
            {
                SendPlayerDie(packet.PlayerName);
            }

        }
        private void SendShootPacket(Shoot packet, NetIncomingMessage message, List<NetConnection> players)
        {
            NetConnection mess = message.SenderConnection;
            // First, find the room ID that corresponds to the sender's connection
            int roomID = getRoomID(getPlayerName(mess))[0];
            

            // If roomID is found (not 0), proceed to the second part
            if (roomID != 0)
            {
                // Find the target room using the room ID
                var targetRoom = RoomList.FirstOrDefault(room => room.Id == roomID);
                if(targetRoom?.roomMode == RoomMode.Rank)
                {
                    targetRoom = MatchmakingRoom.FirstOrDefault(room => room.Id == roomID);
                }
                if (targetRoom != null)
                {
                    // Add all players from the target room to the players list
                    players.AddRange(targetRoom.playersList.Select(player => player.netConnection));
                }
                foreach (var rid in getRoomID(getPlayerName(mess)))
                {
                    roomManager.StopTurn(rid);
                }
            }
            NetOutgoingMessage outgoingMessage = server.CreateMessage();
            new Shoot() { playerName = packet.playerName, angle = packet.angle, force = packet.force, X = packet.X, Y = packet.Y }.PacketToNetOutGoingMessage(outgoingMessage);
            if (players.Count != 0)
            {
                server.SendMessage(outgoingMessage, players, NetDeliveryMethod.ReliableOrdered, 0);
            }
            else
            {
                Logging.Error("No player in list players");
            }

        }

        public async void StartGame(StartGamePacket packet, List<NetConnection> players, NetConnection connection)
        {

            /*NetConnection mess = message.SenderConnection;*/
            Dictionary<NetConnection, Team> PlayerTeam = new Dictionary<NetConnection, Team>();
            
            int roomID = packet.roomId;


            // If roomID is found (not 0), proceed to the second part
            if (roomID != 0)
            {
                // Find the target room using the room ID
                var targetRoom = RoomList.FirstOrDefault(room => room.Id == roomID);
                
                if (targetRoom != null)
                {
                    
                    if (targetRoom.roomMode == RoomMode.Rank)
                    {
                        targetRoom = MatchmakingRoom.FirstOrDefault(room => room.Id == roomID);
                        
                        var pl = targetRoom.playersList.FirstOrDefault(pl => pl.netConnection == connection);
                        pl.isReady = false;
                        Logging.Info($"Player {pl.User.Display_name} In Game");
                        foreach (var readyPlayer in targetRoom.playersList)
                        {
                            if (readyPlayer.isReady) return;
                        }
                    }
                    // Add all players from the target room to the players list
                    players.AddRange(targetRoom.playersList.Select(player => player.netConnection));
                    foreach (var Player in targetRoom.playersList)
                    {
                        Player.isReady = false;
                        PlayerTeam.Add(Player.netConnection, Player.team);
                    }

                    targetRoom.roomStatus = RoomStatus.InMatch;

                    SpawnPlayers(targetRoom);


                }
            }
            
            await Task.Delay(1000);
            roomManager.StartTurnManagerForRoom(roomID, players);
        }

        public void SpawnPlayers(RoomInfo room)
        {
            Logging.Debug("Spawn PLayer for room " + room.Id);

            var uccontroller = _serviceProvider.GetService<UserCharacterController>();
            List<SpawnPlayerPacket> packets = new List<SpawnPlayerPacket>();
            List<NetConnection> netConnections = new List<NetConnection>();

            foreach (var player in room.playersList)
            {
                Vector2 spawnPosition = GetRandomVector2();
                netConnections.Add(player.netConnection);
                var ucm = uccontroller.GetCurrentCharacterAsync(player.User.Id).Result;
                packets.Add(
                    new SpawnPlayerPacket()
                    {
                        playerSpawn = player.User.Username,
                        DisplayName = player.User.Display_name,
                        X = spawnPosition.X,
                        Y = spawnPosition.Y,
                        HP = ucm.HpPoint * 100 + ucm.Character.Hp,
                        Attack = ucm.DamagePoint * 10 + ucm.Character.Damage,
                        Amor = ucm.ArmorPoint + ucm.Character.Armor,
                        Lucky = ucm.LuckPoint + ucm.Character.Luck,
                        Team = player.team
                    }
                 );
               
            }
            NetOutgoingMessage outgoingMessage = server.CreateMessage();
            Logging.Info($"There is {packets.Count} Position generated");
            new SpawnPlayerPacketToAll()
            {
                SPPacket = packets
            }.PacketToNetOutGoingMessage( outgoingMessage );
            server.SendMessage(outgoingMessage, netConnections, NetDeliveryMethod.ReliableOrdered, 0);
        }

        private Vector2 GetRandomVector2()
        {
            List<int> possibleYValues = new List<int> { 241, 237,241,241,241 };

           
            int randomYIndex = random.Next(0, possibleYValues.Count);
            int y = possibleYValues[randomYIndex];
            int x = 0;

            
            if (y == 241)
            {
                x = random.Next(420, 430);
            }
            else if (y == 237)
            {
               
                if (random.NextDouble() < 0.5)
                {
                    x = random.Next(417, 420);
                }
                else
                {
                    x = random.Next(430, 432);
                }
            }

            return new Vector2(x, y);
        }


        public void SendPositionPacket(List<NetConnection> players, PositionPacket packet, NetIncomingMessage message)
        {

            int roomID = getRoomID(getPlayerName(message.SenderConnection))[0];

            // If roomID is found (not 0), proceed to the second part
            if (roomID != 0)
            {
                // Find the target room using the room ID
                var targetRoom = RoomList.FirstOrDefault(room => room.Id == roomID);
                if (targetRoom.roomMode == RoomMode.Rank)
                {
                    targetRoom = MatchmakingRoom.FirstOrDefault(room => room.Id == roomID);

                }
                if (targetRoom != null)
                {
                    // Add all players from the target room to the players list
                    players.AddRange(targetRoom.playersList.Select(player => player.netConnection));
                }

            }
            players.Remove(message.SenderConnection);


            NetOutgoingMessage outgoingMessage = server.CreateMessage();
            packet.PacketToNetOutGoingMessage(outgoingMessage);
            if (players.Count != 0)
            {
                server.SendMessage(outgoingMessage, players, NetDeliveryMethod.ReliableOrdered, 0);
            }
        }
        public string getPlayerName(NetConnection netConnection)
        {
            string username = "";
            foreach (var room in RoomList)
            {
                foreach (var player in room.playersList)
                {
                    if (netConnection == player.netConnection)
                    {

                        username = player.User.Username;
                        break;
                    }
                }

                if (username != "")
                {
                    break;
                }
            }
            return username;
        }

        public void SendStartTurn(string playerName, List<NetConnection> all)
        {
            NetOutgoingMessage outgoingMessage = server.CreateMessage();
            new StartTurnPacket() { playerName = playerName }.PacketToNetOutGoingMessage(outgoingMessage);
            server.SendMessage(outgoingMessage, all, NetDeliveryMethod.ReliableOrdered, 0);
        }
        public void SendEndTurn(string playerName, List<NetConnection> all)
        {
            NetOutgoingMessage outgoingMessage = server.CreateMessage();
            new EndTurnPacket() { playerName = playerName }.PacketToNetOutGoingMessage(outgoingMessage);
            server.SendMessage(outgoingMessage, all, NetDeliveryMethod.ReliableOrdered, 0);
        }
        public void ResetTurn(EndTurnPacket packet, NetIncomingMessage message)
        {
            int roomID = 0;
            Logging.Debug("End Turn for player: " + packet.playerName+ " and reset turn");
            if (!CheckWinGame(packet.playerName, ref roomID))
            {
                roomManager.StartTurn(roomID);
            }
            
        }

        private void HandleCharacterPacket(PacketTypes.Character type, NetIncomingMessage message)
        {
            Packet packet;
            UserCharacterController ucc;
            switch (type) 
            {
                case PacketTypes.Character.GetCurrentCharacterPacket:
                    packet = new GetCurrentCharacterPacket();
                    packet.NetIncomingMessageToPacket(message);
                    ucc = _serviceProvider.GetService<UserCharacterController>();
                    Logging.Info($"{((GetCurrentCharacterPacket)packet).Username} get current character packet");
                    var userCharacter = ucc.GetCurrentCharacterAsync(((GetCurrentCharacterPacket)packet).Username).Result;
                    Logging.Info($"Character level: {userCharacter.Level}");

                    NetOutgoingMessage outmsg = server.CreateMessage();
                    new GetCurrentCharacterPacket()
                    {
                        Character = new CharacterPacket()
                        {
                            UserCharacterId = userCharacter.Id,
                            CharacterName = userCharacter.Character.Name,
                            CharacterLevel = userCharacter.Level,
                            CharacterXp = userCharacter.Experience,
                            CharacterHp = userCharacter.Character.Hp,
                            CharacterDamage = userCharacter.Character.Damage,
                            CharacterArmor = userCharacter.Character.Armor,
                            CharacterLuck = userCharacter.Character.Luck,
                            IsSelected = userCharacter.IsSelected,
                            HpPoint = userCharacter.HpPoint,
                            DamagePoint = userCharacter.DamagePoint,
                            ArmorPoint = userCharacter.ArmorPoint,
                            LuckPoint = userCharacter.LuckPoint,
                        }
                    }.PacketToNetOutGoingMessage(outmsg);
                    server.SendMessage(outmsg,message.SenderConnection, NetDeliveryMethod.ReliableOrdered, 0);
                    break;

                case PacketTypes.Character.ChangeCharacterPoint:
                    Logging.Info($"Get Change Character Point Packet");
                    packet = new ChangeCharacterPointPacket();
                    packet.NetIncomingMessageToPacket(message);
                    ucc = _serviceProvider.GetService<UserCharacterController>();
                    ucc.UpdateUserCharacter(((ChangeCharacterPointPacket)packet).Character);
                    break;

                default:
                    Logging.Error("Unhandle Data / Package type, typeof Room");
                    break;

            }
        }

        private void HandleFriendPacket(PacketTypes.Friend type, NetIncomingMessage message)
        {
            Packet packet;
            switch (type)
            {
                case PacketTypes.Friend.AllFriendPacket:
                    Logging.Info("Received Request AllFriendPacket");
                    packet = new AllFriendPacket();
                    packet.NetIncomingMessageToPacket(message);
                    SendAllFriendPacket(((AllFriendPacket)packet).username, message.SenderConnection);
                    break;
                case PacketTypes.Friend.FriendRequestPacket:
                    Logging.Info("Received Request FriendRequestPacket");
                    packet = new FriendRequestPacket();
                    packet.NetIncomingMessageToPacket(message);
                    SendFriendRequestPacket(((FriendRequestPacket)packet).username, message.SenderConnection);
                    break;
                case PacketTypes.Friend.SentRequestPacket:
                    Logging.Info("Received Request SentRequestPacket");
                    packet = new SentRequestPacket();
                    packet.NetIncomingMessageToPacket(message);
                    SendSentFriendPacket(((SentRequestPacket)packet).username, message.SenderConnection);
                    break;
                case PacketTypes.Friend.SuggestFriendPacket:
                    Logging.Info("Received Request SuggestFriendPacket");
                    packet = new SuggestFriendPacket();
                    packet.NetIncomingMessageToPacket(message);
                    SendSuggestFriendPacket(((SuggestFriendPacket)packet).username, message.SenderConnection);
                    break;
                case PacketTypes.Friend.SearchFriendPacket:
                    Logging.Info("Received Request SearchedFriendPacket");
                    packet = new SearchFriendPacket();
                    packet.NetIncomingMessageToPacket(message);
                    SendSearchFriendPacket(((SearchFriendPacket)packet).username1, ((SearchFriendPacket)packet).username2, message.SenderConnection);
                    break;
                case PacketTypes.Friend.BlockFriendPacket:
                    Logging.Info("Received Request BlockFriendPacket");
                    packet = new BlockFriendPacket();
                    packet.NetIncomingMessageToPacket(message);
                    SendBlockFriendPacket(((BlockFriendPacket)packet).username, message.SenderConnection);
                    break;
                case PacketTypes.Friend.AddFriendPacket:
                    Logging.Info("Received Request AddFriendPacket");
                    packet = new AddFriendPacket();
                    packet.NetIncomingMessageToPacket(message);
                    SendAddFriendPacket(((AddFriendPacket)packet).username1,((AddFriendPacket)packet).username2, message.SenderConnection);
                    break;
                case PacketTypes.Friend.DeleteFriend:
                    Logging.Info("Received Request DeleteFriend");
                    packet = new DeleteFriend();
                    packet.NetIncomingMessageToPacket(message);
                    SendDeleteFriend(((DeleteFriend)packet).username1, ((DeleteFriend)packet).username2, message.SenderConnection);
                    break;
                case PacketTypes.Friend.AcceptFriendInvite:
                    Logging.Info("Received Request AcceptFriendInvite");
                    packet = new AcceptFriendInvite();
                    packet.NetIncomingMessageToPacket(message);
                    SendAcceptFriendRequest(((AcceptFriendInvite)packet).username1, ((AcceptFriendInvite)packet).username2, message.SenderConnection);
                    break;
                case PacketTypes.Friend.CancelFriendRequest:
                    Logging.Info("Received Request CancelFriendRequest");
                    packet = new CancelFriendRequest();
                    packet.NetIncomingMessageToPacket(message);
                    SendCancelFriendRequest(((CancelFriendRequest)packet).username1, ((CancelFriendRequest)packet).username2, message.SenderConnection);
                    break;
                case PacketTypes.Friend.BlockFriend:
                    Logging.Info("Received Request BlockFriend");
                    packet = new BlockFriend();
                    packet.NetIncomingMessageToPacket(message);
                    SendBlockFriend(((BlockFriend)packet).username1, ((BlockFriend)packet).username2, message.SenderConnection);
                    break;
                case PacketTypes.Friend.UnBlockFriend:
                    Logging.Info("Received Request UnBlockFriend");
                    packet = new UnBlockFriend();
                    packet.NetIncomingMessageToPacket(message);
                    SendUnBlockFriendRequest(((UnBlockFriend)packet).username1, ((UnBlockFriend)packet).username2, message.SenderConnection);
                    break;
                default:
                    Logging.Error("Unhandle Data / Package type, typeof Room");
                    break;
            }

        }
        private async void SendAddFriendPacket(string username1, string username2, NetConnection netConnection)
        {
            var userController = _serviceProvider.GetService<UserController>();
            int userId1 = await userController.GetUserIdByUsernameAsync(username1);
            int userId2 = await userController.GetUserIdByUsernameAsync(username2);

            var userRelationshipController = _serviceProvider.GetService<UserRelationshipController>();
            bool getSuggestFriend = await userRelationshipController.AddFriend(userId1,userId2);

            NetOutgoingMessage outmsg = server.CreateMessage();
            new AddFriendPacket()
            {
                IsSuccess = getSuggestFriend
            }.PacketToNetOutGoingMessage(outmsg);
            server.SendMessage(outmsg, netConnection, NetDeliveryMethod.ReliableOrdered, 0);
        }
        private async void SendDeleteFriend(string username1, string username2, NetConnection netConnection)
        {
            var userController = _serviceProvider.GetService<UserController>();
            int userId1 = await userController.GetUserIdByUsernameAsync(username1);
            int userId2 = await userController.GetUserIdByUsernameAsync(username2);

            var userRelationshipController = _serviceProvider.GetService<UserRelationshipController>();
            bool getSuggestFriend = await userRelationshipController.DeleteFriend(userId1, userId2);

            NetOutgoingMessage outmsg = server.CreateMessage();
            new DeleteFriend()
            {
                IsSuccess = getSuggestFriend
            }.PacketToNetOutGoingMessage(outmsg);
            server.SendMessage(outmsg, netConnection, NetDeliveryMethod.ReliableOrdered, 0);
        }
        private async void SendAcceptFriendRequest(string username1, string username2, NetConnection netConnection)
        {
            var userController = _serviceProvider.GetService<UserController>();
            int userId1 = await userController.GetUserIdByUsernameAsync(username1);
            int userId2 = await userController.GetUserIdByUsernameAsync(username2);

            var userRelationshipController = _serviceProvider.GetService<UserRelationshipController>();
            bool getSuggestFriend = await userRelationshipController.AcceptFriendInvite(userId1, userId2);

            NetOutgoingMessage outmsg = server.CreateMessage();
            new AcceptFriendInvite()
            {
                IsSuccess = getSuggestFriend
            }.PacketToNetOutGoingMessage(outmsg);
            server.SendMessage(outmsg, netConnection, NetDeliveryMethod.ReliableOrdered, 0);
        }
        private async void SendCancelFriendRequest(string username1, string username2, NetConnection netConnection)
        {
            var userController = _serviceProvider.GetService<UserController>();
            int userId1 = await userController.GetUserIdByUsernameAsync(username1);
            int userId2 = await userController.GetUserIdByUsernameAsync(username2);

            var userRelationshipController = _serviceProvider.GetService<UserRelationshipController>();
            bool getSuggestFriend = await userRelationshipController.CancelFriendRequest(userId1, userId2);

            NetOutgoingMessage outmsg = server.CreateMessage();
            new CancelFriendRequest()
            {
                IsSuccess = getSuggestFriend
            }.PacketToNetOutGoingMessage(outmsg);
            server.SendMessage(outmsg, netConnection, NetDeliveryMethod.ReliableOrdered, 0);
        }
        private async void SendBlockFriend(string username1, string username2, NetConnection netConnection)
        {
            var userController = _serviceProvider.GetService<UserController>();
            int userId1 = await userController.GetUserIdByUsernameAsync(username1);
            int userId2 = await userController.GetUserIdByUsernameAsync(username2);

            var userRelationshipController = _serviceProvider.GetService<UserRelationshipController>();
            bool getSuggestFriend = await userRelationshipController.BlockFriend(userId1, userId2);

            NetOutgoingMessage outmsg = server.CreateMessage();
            new BlockFriend()
            {
                IsSuccess = getSuggestFriend
            }.PacketToNetOutGoingMessage(outmsg);
            server.SendMessage(outmsg, netConnection, NetDeliveryMethod.ReliableOrdered, 0);
        }
        private async void SendUnBlockFriendRequest(string username1, string username2, NetConnection netConnection)
        {
            var userController = _serviceProvider.GetService<UserController>();
            int userId1 = await userController.GetUserIdByUsernameAsync(username1);
            int userId2 = await userController.GetUserIdByUsernameAsync(username2);

            var userRelationshipController = _serviceProvider.GetService<UserRelationshipController>();
            bool getSuggestFriend = await userRelationshipController.UnBlockFriend(userId1, userId2);

            NetOutgoingMessage outmsg = server.CreateMessage();
            new UnBlockFriend()
            {
                IsSuccess = getSuggestFriend
            }.PacketToNetOutGoingMessage(outmsg);
            server.SendMessage(outmsg, netConnection, NetDeliveryMethod.ReliableOrdered, 0);
        }
        private async void SendSuggestFriendPacket(string username, NetConnection netConnection)
        {
            var userController = _serviceProvider.GetService<UserController>();
            int userId = await userController.GetUserIdByUsernameAsync(username);

            var userRelationshipController = _serviceProvider.GetService<UserRelationshipController>();
            List<UserModel> getSuggestFriend = await userRelationshipController.GetSuggestFriendListAsync(userId);

            List<FriendTabPacket> suggestContent = new List<FriendTabPacket>();

            foreach (var item in getSuggestFriend) 
            {
                suggestContent.Add(new FriendTabPacket()
                {
                    FriendUsername = item.Username,
                    FriendDisplayName = item.Display_name
                });
            }

            NetOutgoingMessage outmsg = server.CreateMessage();
            new SuggestFriendPacket() 
            {
                Friends = suggestContent 
            }.PacketToNetOutGoingMessage(outmsg);
            server.SendMessage(outmsg, netConnection, NetDeliveryMethod.ReliableOrdered, 0);
        }
        private async void SendSearchFriendPacket(string username1, string username2, NetConnection netConnection)
        {
            var userRelationshipController = _serviceProvider.GetService<UserRelationshipController>();
            List<UserModel> getSuggestFriend = await userRelationshipController.GetSearchedPlayerAsync(username1, username2);

            List<FriendTabPacket> suggestContent = new List<FriendTabPacket>();

            foreach (var item in getSuggestFriend)
            {
                suggestContent.Add(new FriendTabPacket()
                {
                    FriendUsername = item.Username,
                    FriendDisplayName = item.Display_name
                });
            }

            NetOutgoingMessage outmsg = server.CreateMessage();
            new SearchFriendPacket()
            {
                Friends = suggestContent
            }.PacketToNetOutGoingMessage(outmsg);
            server.SendMessage(outmsg, netConnection, NetDeliveryMethod.ReliableOrdered, 0);
        }
        private async void SendAllFriendPacket(string username, NetConnection netConnection)
        {
            var userController = _serviceProvider.GetService<UserController>();
            int userId = await userController.GetUserIdByUsernameAsync(username);

            var userRelationshipController = _serviceProvider.GetService<UserRelationshipController>();
            List<UserModel> getAllFriend = await userRelationshipController.GetAllFriendsListAsync(userId);

            List<FriendTabPacket> suggestContent = new List<FriendTabPacket>();

            foreach (var item in getAllFriend)
            {
                bool isOnline = false;
                if (PlayerOnlineList.FirstOrDefault(pl=>pl.User.Username == item.Username) != null) { isOnline = true; }

                suggestContent.Add(new FriendTabPacket()
                {
                    FriendUsername = item.Username,
                    FriendDisplayName = item.Display_name,
                    FriendIsOnline = isOnline
                });
            }

            NetOutgoingMessage outmsg = server.CreateMessage();
            new AllFriendPacket()
            {
                Friends = suggestContent
            }.PacketToNetOutGoingMessage(outmsg);
            server.SendMessage(outmsg, netConnection, NetDeliveryMethod.ReliableOrdered, 0);

        }
        private async void SendFriendRequestPacket(string username, NetConnection netConnection)
        {
            var userController = _serviceProvider.GetService<UserController>();
            int userId = await userController.GetUserIdByUsernameAsync(username);

            var userRelationshipController = _serviceProvider.GetService<UserRelationshipController>();
            List<UserModel> getFriendRequest = await userRelationshipController.GetFriendsRequestListAsync(userId);

            List<FriendTabPacket> suggestContent = new List<FriendTabPacket>();

            foreach (var item in getFriendRequest)
            {
                suggestContent.Add(new FriendTabPacket()
                {
                    FriendUsername = item.Username,
                    FriendDisplayName = item.Display_name
                });
            }

            NetOutgoingMessage outmsg = server.CreateMessage();
            new FriendRequestPacket()
            {
                Friends = suggestContent
            }.PacketToNetOutGoingMessage(outmsg);
            server.SendMessage(outmsg, netConnection, NetDeliveryMethod.ReliableOrdered, 0);
        }
        private async void SendSentFriendPacket(string username, NetConnection netConnection)
        {
            var userController = _serviceProvider.GetService<UserController>();
            int userId = await userController.GetUserIdByUsernameAsync(username);

            var userRelationshipController = _serviceProvider.GetService<UserRelationshipController>();
            List<UserModel> getSentFriend = await userRelationshipController.GetSentFriendsListAsync(userId);

            List<FriendTabPacket> suggestContent = new List<FriendTabPacket>();

            foreach (var item in getSentFriend)
            {
                suggestContent.Add(new FriendTabPacket()
                {
                    FriendUsername = item.Username,
                    FriendDisplayName = item.Display_name
                });
            }

            NetOutgoingMessage outmsg = server.CreateMessage();
            new SentRequestPacket()
            {
                Friends = suggestContent
            }.PacketToNetOutGoingMessage(outmsg);
            server.SendMessage(outmsg, netConnection, NetDeliveryMethod.ReliableOrdered, 0);
        }
        private async void SendBlockFriendPacket(string username, NetConnection netConnection)
        {
            var userController = _serviceProvider.GetService<UserController>();
            int userId = await userController.GetUserIdByUsernameAsync(username);

            var userRelationshipController = _serviceProvider.GetService<UserRelationshipController>();
            List<UserModel> getBlockFriend = await userRelationshipController.GetBlockFriendsListAsync(userId);

            List<FriendTabPacket> suggestContent = new List<FriendTabPacket>();

            foreach (var item in getBlockFriend)
            {
                suggestContent.Add(new FriendTabPacket()
                {
                    FriendUsername = item.Username,
                    FriendDisplayName = item.Display_name
                });
            }

            NetOutgoingMessage outmsg = server.CreateMessage();
            new BlockFriendPacket()
            {
                Friends = suggestContent
            }.PacketToNetOutGoingMessage(outmsg);
            server.SendMessage(outmsg, netConnection, NetDeliveryMethod.ReliableOrdered, 0);
        }
        private void HandleRoomPacket(PacketTypes.Room type, NetIncomingMessage message)
        {
            Packet packet;
            RoomInfo room;
            Player player;
            switch (type)
            {
                case PacketTypes.Room.CreateRoomPacket:
                    Logging.Info("Received Create Room Packet");
                    packet = new CreateRoomPacket();
                    packet.NetIncomingMessageToPacket(message);

                    SendJoinRoomPacketToAll(SendCreateNewRoomPacket(((CreateRoomPacket)packet).room, message.SenderConnection));
                    break;

                case PacketTypes.Room.JoinRoomPacket:
                    Logging.Info("Received Join Room Packet");
                    packet = new JoinRoomPacket();
                    packet.NetIncomingMessageToPacket(message);
                    room = SendJoinRoomPacket((JoinRoomPacket)packet, message.SenderConnection);
                    SendJoinRoomPacketToAll(room);
                    break;

                case PacketTypes.Room.ExitRoomPacket:
                    Logging.Info("Received Exit Room Packet");
                    packet = new ExitRoomPacket();
                    packet.NetIncomingMessageToPacket(message);
                    // send to all

                    room = RoomList.FirstOrDefault(r => r.Id == ((ExitRoomPacket)packet).roomId);
                    player = PlayerOnlineList.FirstOrDefault(u => u.User.Username == ((ExitRoomPacket)packet).username);

                    if (room != null && player != null)
                    {
                        RemovePlayerInRoom(player, room);
                        SendJoinRoomPacketToAll(room);
                    }
                    break;

                case PacketTypes.Room.SendChatMessagePacket:
                    Logging.Info("Receive Chat Message");
                    packet = new SendChatMessagePacket();
                    packet.NetIncomingMessageToPacket(message);
                    SendChatMessagePacketToAll((SendChatMessagePacket)packet);
                    break;

                case PacketTypes.Room.PlayerReadyPacket:
                    Logging.Info("Receive PlayerReadyPacket");
                    packet = new PlayerReadyPacket();
                    packet.NetIncomingMessageToPacket(message);
                    room = RoomList.FirstOrDefault(r => r.Id == ((PlayerReadyPacket)packet).RoomId);
                    player = PlayerOnlineList.FirstOrDefault(u => u.User.Username == ((PlayerReadyPacket)packet).Username);

                    if (room != null && player != null)
                    {
                        player.isReady = ((PlayerReadyPacket)packet).IsReady;
                        Logging.Info($"Player {player.User.Username} in Room {room.Id} Ready: {player.isReady}");
                        
                        SendJoinRoomPacketToAll(room);
                    }
                    break;
                

                case PacketTypes.Room.RoomListPacket:
                    List<RoomPacket> roomList = new List<RoomPacket>();
                    foreach (var r in RoomList)
                    {
                        roomList.Add(new RoomPacket
                        {
                            Id = r.Id,
                            Name = r.Name,
                            roomMode = r.roomMode,
                            roomStatus = r.roomStatus,
                            roomType = r.RoomType,
                            PlayerNumber = r.playersList.Count,
                        });
                    }
                    NetOutgoingMessage outmsg = server.CreateMessage();
                    new RoomListPacket()
                    {
                        rooms = roomList,
                    }.PacketToNetOutGoingMessage(outmsg);
                    server.SendMessage(outmsg, message.SenderConnection, NetDeliveryMethod.ReliableOrdered, 0);

                    break;

                case PacketTypes.Room.ChangeRoomTypePacket:
                    packet = new ChangeRoomTypePacket();
                    packet.NetIncomingMessageToPacket(message);
                    room = RoomList.FirstOrDefault(r => r.Id == ((ChangeRoomTypePacket)packet).room.Id);
                    if (room != null)
                    {
                        room.RoomType = ((ChangeRoomTypePacket)packet).room.roomType;
                        Logging.Info($"Room {room.Id} Change Room Type: NEW {room.RoomType.ToString()}");
                        List<NetConnection> connections = room.playersList
                                         .Where(player => player.netConnection != null)
                                         .Select(player => player.netConnection)
                                         .ToList();
                        SendRoomInfo(((ChangeRoomTypePacket)packet).room, connections);
                    }
                    break;
                case PacketTypes.Room.ChangeTeamPacket:
                    packet = new ChangeTeamPacket();
                    packet.NetIncomingMessageToPacket(message);
                    SendChangeTeamPacket((ChangeTeamPacket)packet);
                    break;

                default:
                    Logging.Error("Unhandle Data / Package type, typeof Room");
                    break;
            }

        }

        private void SendChangeTeamPacket(ChangeTeamPacket packet)
        {
            var thisRoom = RoomList.FirstOrDefault(r => r.Id == packet.roomId);
            int maxPlayers = thisRoom.MaxPlayers;
            Player currentPlayer = thisRoom.playersList[0];
            
            if(packet.team == Team.Team1)
            {
                var availablePositions2 = Enumerable.Range(5, 4).ToList();
                foreach (var playerInRoom in thisRoom.playersList)
                {
                    if (playerInRoom.User.Username == packet.username)
                    {
                        currentPlayer = playerInRoom;
                    }
                    if (playerInRoom.team == Team.Team2)
                    {
                        availablePositions2.Remove(playerInRoom.Position);
                    }
                }
                currentPlayer.Position = availablePositions2[random.Next(availablePositions2.Count)];
            }
            else
            {
                var availablePositions1 = Enumerable.Range(1, 4).ToList();
                foreach (var playerInRoom in thisRoom.playersList)
                {
                    if (playerInRoom.team == Team.Team1)
                    {
                        availablePositions1.Remove(playerInRoom.Position);
                    }
                    if (playerInRoom.User.Username == packet.username)
                    {
                        currentPlayer = playerInRoom;
                    }
                }
                currentPlayer.Position = availablePositions1[random.Next(availablePositions1.Count)];
            }
            SendJoinRoomPacketToAll(thisRoom);
        }

        private RoomInfo SendCreateNewRoomPacket(RoomPacket roomPacket, NetConnection senderConnection)
        {
            var player = PlayerOnlineList?.FirstOrDefault(u => u.netConnection == senderConnection);

            int newRoomId;
            do
            {
                newRoomId = random.Next(1000, 10000);
            } while (RoomList.Any(room => room.Id == newRoomId));
            RoomInfo thisRoom = new RoomInfo()
            {
                Id = newRoomId,
                Name = $"Room {newRoomId}",
                roomMode = roomPacket.roomMode,
                roomStatus = RoomStatus.InLobby,
                RoomType = roomPacket.roomType,
                playersList = new List<Player>()
            };
            RoomList.Add(thisRoom);
            Logging.Info($"New Room Created: Id {thisRoom.Id}, PlayerInRoom {thisRoom.playersList.Count}");

            player.IsHost = true;
            player.team = Team.Team1;
            player.Position = 1;
            thisRoom.playersList.Add(player);
            NetOutgoingMessage outmsg = server.CreateMessage();
            new CreateRoomPacket()
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
            server.SendMessage(outmsg, senderConnection, NetDeliveryMethod.ReliableOrdered, 0);
            return thisRoom;
        }

        private void SendChatMessagePacketToAll(SendChatMessagePacket packet)
        {
            RoomInfo? thisroom = RoomList.FirstOrDefault( room => room.Id == packet.RoomID);
            Logging.Info($"Receive Chat Message from Username: {packet.Username}, RoomID: {packet.RoomID}");
            if (thisroom != null)
            {
                foreach (var player in thisroom.playersList)
                {
                    Logging.Info($"Player In this Room: PlayerName {player.User.Display_name}");
                }
                var thisPlayer = thisroom.playersList.FirstOrDefault( player => player.User.Username == packet.Username);

                List<NetConnection> connections = thisroom.playersList
                                         .Where(player => player.netConnection != null)
                                         .Select(player => player.netConnection)
                                         .ToList();
                
                // No One in room now
                if (connections.Count <= 0) return;
                NetOutgoingMessage outmsg = server.CreateMessage();
                packet.DisplayName = thisPlayer.User.Display_name;
                packet.PacketToNetOutGoingMessage(outmsg);
                server.SendMessage(outmsg, connections, NetDeliveryMethod.ReliableOrdered, 0);
            }
            
        }

        private void SendJoinRoomPacketToAll(RoomInfo room)
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
            server.SendMessage(outmsg, connections, NetDeliveryMethod.ReliableOrdered, 1);
            Logging.Info("Send Join Room Packet to all");
        }

        private RoomInfo SendJoinRoomPacket(JoinRoomPacket joinRoomPacket, NetConnection netConnection)
        {
            RoomInfo? thisRoom;

            //Join Random Room
            if (joinRoomPacket.room.Id == 0 && joinRoomPacket.room.roomMode != RoomMode.Rank)
            {
                thisRoom = RoomList?.FirstOrDefault(room =>
                    room.roomMode == joinRoomPacket.room.roomMode &&
                    !room.IsRoomFull &&
                    room.roomStatus == RoomStatus.InLobby &&
                    room.RoomType == joinRoomPacket.room.roomType
                );
            }
            else //Join Room By Id
            {
                thisRoom = RoomList?.FirstOrDefault(room => room.Id == joinRoomPacket.room.Id);
                if (thisRoom != null && thisRoom.IsRoomFull)
                {
                    return null;
                }
            }

            var player = PlayerOnlineList?.FirstOrDefault(u => u.netConnection == netConnection);

            if (thisRoom == null)
            {
                // Room Not Found or Not Suitable, Create New
                return SendCreateNewRoomPacket(joinRoomPacket.room, netConnection);
            }
            else
            {
                if (thisRoom.roomMode != RoomMode.Rank)
                {
                    // Set Position in Waiting Room
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
                        }
                        else
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

                }
                else
                {
                    player.team = Team.Team1;
                }
                player.IsHost = false;
            }


            thisRoom.playersList.Add(player);
            Logging.Info($"User {player.User.Username} Join Room {thisRoom.Id}");


            SendRoomInfo(new RoomPacket()
            {
                Id = thisRoom.Id,
                Name = thisRoom.Name,
                roomMode = thisRoom.roomMode,
                roomStatus = thisRoom.roomStatus,
                roomType = thisRoom.RoomType,
            }, new List<NetConnection> { netConnection });

            Logging.Info("Response Join Room Packet: Room ID: " + thisRoom.Id);
            return thisRoom;
        }
        private void SendRoomInfo(RoomPacket room, List<NetConnection> connections)
        {
            NetOutgoingMessage outmsg = server.CreateMessage();
            new JoinRoomPacket()
            {
                room = room,
            }.PacketToNetOutGoingMessage(outmsg);

            server.SendMessage(outmsg, connections, NetDeliveryMethod.ReliableOrdered, 0);
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

                case PacketTypes.General.Logout:
                    var logout = new Logout();
                    logout.NetIncomingMessageToPacket(message);
                    if (!string.IsNullOrEmpty(logout.username))
                    {
                        RemovePlayerInPlayerOnlineListAndRoom(logout.username);
                        Logging.Info($"{logout.username} logout");
                    }
                    break;

                case PacketTypes.General.ResetPassword:
                    var resetpass = new ResetPassword();
                    resetpass.NetIncomingMessageToPacket(message);
                    SendResetPasswordPacket(resetpass, message.SenderConnection);

                    break;

                case PacketTypes.General.ChangePassword:

                    Logging.Info("Type: ChangePassword, From: " + deviceId);
                    var ChangePass = new ChangePassword();
                    ChangePass.NetIncomingMessageToPacket(message);
                    SendChangePasswordpackage(ChangePass, message.SenderConnection);
                    break;
                case PacketTypes.General.VerifyRegistrationPacket:
                    var verifyRegistrationPacket = new VerifyRegistrationPacket();
                    verifyRegistrationPacket.NetIncomingMessageToPacket(message);
                    SendVerifyRegistrationPacket(verifyRegistrationPacket, message.SenderConnection);
                    break;

                default:
                    Logging.Error("Unhandle Data / Package type");
                    break;
            }
        }

        private void SendVerifyRegistrationPacket(VerifyRegistrationPacket packet, NetConnection senderConnection)
        {
            var username = packet.username;
            var otp = packet.otp;
            var otpService = _serviceProvider.GetService<OtpService>();
            var verify = false;
            var rs = "";
            
            if (otpService.VerifyOtp(username, otp))
            {
                var userController = _serviceProvider.GetService<UserController>();
                userController.VerifyUserEmail(username);
                verify = true;
            } else
            {
                rs = "Wrong OTP";
            }
            NetOutgoingMessage outmsg = server.CreateMessage();
            new VerifyRegistrationPacket()
            {
                isSuccess = verify,
                reason = rs,
            }.PacketToNetOutGoingMessage(outmsg);
            Logging.Info("Verification " + (verify ? "successful" : "failed"));
            server.SendMessage(outmsg, senderConnection, NetDeliveryMethod.ReliableOrdered, 0);
        }

        private void SendResetPasswordPacket(ResetPassword resetpass, NetConnection connection)
        {
            var userController = _serviceProvider.GetService<UserController>();
            var result = userController.ResetPasswordAsync(resetpass.username, resetpass.email, random);

            NetOutgoingMessage outmsg = server.CreateMessage();
            ResetPassword pack = new ResetPassword();

            if (result.Result)
            {
                pack.isSuccess = true;
            } else
            {
                pack.isSuccess = false;
                pack.reason = "User Not found";
            }
            pack.PacketToNetOutGoingMessage(outmsg);
            server.SendMessage(outmsg, connection, NetDeliveryMethod.ReliableOrdered, 0);
            
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
        private void RemovePlayerInRoom(Player player, RoomInfo room)
        {
            int outRoomId = room.Id;
            if(room.roomStatus == RoomStatus.InMatch)
            {
                Logging.Warn("delete player: " + player.User.Username);
                SendPlayerDie(player.User.Username);
                if (!CheckWinGame(player.User.Username, ref outRoomId))
                {
                    if (player.User.Username == roomManager.GetPlayerInCurrentTurn(outRoomId))
                    {
                        roomManager.StartNewTurn(outRoomId);
                    }
                }
                
                
            }
            room.playersList.Remove(player);
            Logging.Warn($"Player {player.User.Display_name} Exit Room {room.Id}");
            if (room.playersList.Count == 0)
            {
                Logging.Warn($"Room {room.Id} remove because no player in room!");
                RoomInfo selectroom = RoomList.FirstOrDefault(r => r == room);
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
            NetOutgoingMessage outmsg = server.CreateMessage();
            if ( currentUser != null)
            {
                if (!currentUser.isVerify)
                {
                    var otpcode =  _serviceProvider.GetService<OtpService>()
                    .GenerateOtp(currentUser.Username);

                    await _serviceProvider.GetService<EmailService>().
                        SendMailVerifyRegistration(currentUser.Username, currentUser.Email, currentUser.RegisteredAt, otpcode);
                    outmsg.Write((byte)PacketTypes.General.RequireVerifyPacket);
                }
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
            var newUser = userController.SignUp(signUpPacket.username, signUpPacket.password, signUpPacket.email).Result;
            if (newUser != null)
            {
                signUpPacket.isSuccess = true;
                
                var otpcode = _serviceProvider.GetService<OtpService>()
                    .GenerateOtp(newUser.Username);

                _serviceProvider.GetService<EmailService>().
                    SendMailVerifyRegistration(newUser.Username, newUser.Email, newUser.RegisteredAt, otpcode);
                _serviceProvider.GetService<UserCharacterController>().AddCharacterToNewUser(newUser.Id, 1);
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
                reason = signUpPacket.reason
            }.PacketToNetOutGoingMessage(outmsg);
            Logging.Info("Send Sign Up Package to User");
            server.SendMessage(outmsg, user, NetDeliveryMethod.ReliableOrdered, 0);

        }

        private void SendChangePasswordpackage(ChangePassword changePass, NetConnection user)
        {
            var userController = _serviceProvider.GetRequiredService<UserController>();
            var newUser = userController.Login(changePass.username, changePass.oldPassword).Result;
            NetOutgoingMessage outmsg = server.CreateMessage();
            if (newUser != null)
            {
                changePass.isSuccess = true;
                userController.Changepassword(changePass.username, changePass.newPass);
            }
            else
            {
                changePass.isSuccess = false;
                changePass.reason = "The old password is not correct!";
            }
            new ChangePassword()
            {
                isSuccess = changePass.isSuccess,
                username = changePass.username,
                newPass = changePass.reason
            }.PacketToNetOutGoingMessage(outmsg);
            server.SendMessage(outmsg, user, NetDeliveryMethod.ReliableOrdered, 0);
        }
    }
}
