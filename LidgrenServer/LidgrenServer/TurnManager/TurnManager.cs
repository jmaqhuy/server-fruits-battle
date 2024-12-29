



namespace LidgrenServer.TurnManager
{


    using System;
    using System.Diagnostics;
    using System.Drawing.Printing;
    using System.Threading;
    using Lidgren.Network;
    

    public class TurnManager
    {
        private Timer turnTimer;
        private readonly int roomId;
        private readonly int interval = 20000; // 20 seconds interval
        private List<NetConnection> playersAlive;
        private int currentPlayerIndex;
        private int previousPlayerIndex;
        
        public TurnManager(int roomId, List<NetConnection> players)
        {
            this.roomId = roomId;
            this.playersAlive = new List<NetConnection>(players);
            this.currentPlayerIndex = 0;
        }

        public void StartTurnManager()
        {
            Logging.Warn("Start turn manager");
            turnTimer = new Timer(OnTimedEvent, null, 0, interval);
        }

        

        private void OnTimedEvent(Object state)
        {
            try
            {
                // Code to execute every 20 seconds for the specific room
                if(currentPlayerIndex == 0)
                {
                    previousPlayerIndex = playersAlive.Count - 1;
                }
                else
                {
                    previousPlayerIndex = currentPlayerIndex-1;
                }
                Program.server.SendEndTurn(Program.server.getPlayerName(playersAlive[previousPlayerIndex]), playersAlive);
                
                Program.server.SendStartTurn(Program.server.getPlayerName(playersAlive[currentPlayerIndex]),playersAlive);
                Logging.Debug("Start turn for player: " + Program.server.getPlayerName(playersAlive[currentPlayerIndex]));
                currentPlayerIndex = (currentPlayerIndex + 1) % playersAlive.Count;
            }
            catch (Exception ex)
            {
                // Handle exceptions to prevent crashing
                Logging.Error($"Error in room {roomId}: {ex.Message}");
            }
        }

        public void StopTurnManager()
        {
            
            
            turnTimer?.Dispose();
        }
        public void RemovePlayer(NetConnection player) 
        {
            if (playersAlive.Contains(player))
            {
                int playerIndex = playersAlive.IndexOf(player);
                if (playerIndex < currentPlayerIndex)
                {
                    playersAlive.RemoveAt(playerIndex);
                    currentPlayerIndex =(currentPlayerIndex - 1) % playersAlive.Count;
                }
                else 
                {
                    playersAlive.RemoveAt(playerIndex);
                    currentPlayerIndex = currentPlayerIndex % playersAlive.Count;
                }
            }
            
           
            
        }
        public List<NetConnection> getPLayersAlive()
        {
            return playersAlive;
        }
       
        
        

    }

}
