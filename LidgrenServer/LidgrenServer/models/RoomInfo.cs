namespace LidgrenServer.models
{
    public enum RoomMode
    {
        All,
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
        All = 0,
        OneVsOne = 2,
        TwoVsTwo = 4,
        FourVsFour = 8
    }

    public class RoomInfo
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public RoomMode roomMode { get; set; }
        public RoomStatus roomStatus { get; set; }
        public RoomType RoomType { get; set; }
        public List<Player> playersList { get; set; }
        public int MaxPlayers => roomMode != RoomMode.Rank ? (int)RoomType : (int)RoomType / 2;
        public bool IsRoomFull => playersList.Count >= MaxPlayers;

        public RoomInfo Clone()
        {
            return new RoomInfo
            {
                Id = this.Id,
                Name = this.Name,
                roomMode = this.roomMode,
                roomStatus = this.roomStatus,
                RoomType = this.RoomType,
                playersList = new List<Player>(this.playersList)
            };
        }
    }
}
