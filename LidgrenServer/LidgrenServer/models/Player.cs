using Lidgren.Network;
using LidgrenServer.Models;

namespace LidgrenServer.models
{
    public enum Team
    {
        Team1, Team2
    }
    public class Player
    {
        public bool IsHost { get; set; } = false;
        public Team team { get; set; }
        public int Position { get; set; }
        public bool isReady { get; set; } = false ;
        
        public UserModel User { get; set; }
        public NetConnection netConnection { get; set; }
    }
}
