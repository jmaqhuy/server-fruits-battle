using Lidgren.Network;

namespace LidgrenServer.Packets
{
    public class FriendTabPacket
    {
        public string FriendUsername;
        public string FriendDisplayName;
        public bool FriendIsOnline;

        public void Serialize(NetOutgoingMessage message)
        {
            message.Write(FriendUsername);
            message.Write(FriendDisplayName);
            message.Write(FriendIsOnline);
        }

        public static FriendTabPacket Deserialize(NetIncomingMessage message)
        {
            return new FriendTabPacket()
            {
                FriendUsername = message.ReadString(),
                FriendDisplayName = message.ReadString(),
                FriendIsOnline = message.ReadBoolean(),
            };
        }
    }
}
