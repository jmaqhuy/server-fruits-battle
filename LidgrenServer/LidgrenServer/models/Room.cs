using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lidgren.Network;
using LidgrenServer.Models;

namespace LidgrenServer.models
{
    public enum RoomMode
    {
        Normal,
        BotArea,
        Rank
    }

    public enum RoomStatus
    {
        InMatch,
        InLobby
    }

    public enum RoomType
    {
        OneVsOne = 2,
        TwoVsTwo = 4,
        FourVsFour = 8,
        None
    }

    public class Room
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public RoomMode roomMode { get; set; }
        public RoomStatus roomStatus { get; set; }
        public RoomType RoomType { get; set; }
        public List<Player> playersList { get; set; }
        public int MaxPlayers => (int)RoomType;
        public bool IsRoomFull => playersList.Count >= MaxPlayers;
    }
}
