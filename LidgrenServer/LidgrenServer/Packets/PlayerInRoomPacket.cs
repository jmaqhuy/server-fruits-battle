using Lidgren.Network;
using LidgrenServer.models;

namespace LidgrenServer.Packets
{
    public class PlayerInRoomPacket
    {
        public string username { get; set; }
        public string displayname { get; set; }
        public bool isHost { get; set; }
        public Team team { get; set; }
        public int Position { get; set; }
        public bool isReady { get; set; }

        public void Serialize(NetOutgoingMessage message)
        {
            message.Write(username);
            message.Write(displayname);
            message.Write(isHost);
            message.Write((byte)team);
            message.Write(Position);
            message.Write(isReady);
        }

        public static PlayerInRoomPacket Deserialize(NetIncomingMessage message)
        {
            return new PlayerInRoomPacket
            {
                username = message.ReadString(),
                displayname = message.ReadString(),
                isHost = message.ReadBoolean(),
                team = (Team)message.ReadByte(),
                Position = message.ReadInt32(),
                isReady = message.ReadBoolean()
            };
        }
    }
}
