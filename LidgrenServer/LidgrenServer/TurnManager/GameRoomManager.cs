using System.Diagnostics;
using Lidgren.Network;
namespace LidgrenServer.TurnManager
{

    public class GameRoomManager
    {
        private readonly Dictionary<int, TurnManager> roomManagers;
        
        public GameRoomManager()
        {
            roomManagers = new Dictionary<int, TurnManager>();
        }

        public void StartTurnManagerForRoom(int roomId, List<NetConnection> players)
        {
            if (!roomManagers.ContainsKey(roomId))
            {
                var turnManager = new TurnManager(roomId, players);
                roomManagers[roomId] = turnManager;

                turnManager.StartTurnManager();
                Logging.Info($"Turn manager started for room {roomId}");
                Logging.Warn($"number of player in player alive {getPLayersAlive(roomId).Count}");
            }
            else
            {
                Logging.Info($"Turn manager for room {roomId} is already running.");
            }
        }

        public void StopTurnManagerForRoom(int roomId)
        {
            if (roomManagers.ContainsKey(roomId))
            {
                roomManagers[roomId].StopTurnManager();
                roomManagers.Remove(roomId);
                Logging.Info(roomManagers.Count+"");
                Logging.Info($"Turn manager stopped for room {roomId} and remove");
            }
            else
            {
                Logging.Info($"No turn manager running for room {roomId}");
            }
        }

        
      
        public void RemovePlayerDead(int roomId, NetConnection player)
        {
            
            if (roomManagers.ContainsKey(roomId))
            {
                roomManagers[roomId].RemovePlayer(player);
                Logging.Debug($"Remove {player} successfully");

            }
            else
            {
                Logging.Error("Error to remove player " + player + " in room "+roomId);
                Logging.Error("Number in roommanager "+roomManagers.Count);
            }
        }
        public string GetPlayerInCurrentTurn(int roomId)
        {
            return roomManagers[roomId].GetPlayerInCurrentTurn();
        }
        public void StartNewTurn(int roomId)
        {
            roomManagers[roomId].StartNewTurn();
        }
       
        
        public void StopTurn(int roomId)
        {
            if (roomManagers.ContainsKey(roomId)) 
            {

                roomManagers[roomId].StopTurnManager();
                Logging.Debug("stop turn manage for room " + roomId);
            }
        }
        public void StartTurn(int roomId) 
        {
            Logging.Debug("start turn manage for room " + roomId);
            roomManagers[roomId].StartTurnManager();
            
        }
        public List<NetConnection>getPLayersAlive(int roomId)
        {
            if (roomManagers.ContainsKey(roomId)) 
            {
                return roomManagers[roomId]?.getPLayersAlive();
            }
            else
            {
                return null;
            }
        }
            
    }
}
