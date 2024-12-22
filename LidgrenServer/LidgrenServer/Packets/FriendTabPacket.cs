using Lidgren.Network;

namespace LidgrenServer.Packets
{
    public class FriendTabPacket
    {
        public string FriendUsername;
        public string FriendDisplayName;

        public void Serialize(NetOutgoingMessage message)
        {
            message.Write(FriendUsername);
            message.Write(FriendDisplayName);
        }

        public static FriendTabPacket Deserialize(NetIncomingMessage message)
        {
            return new FriendTabPacket()
            {
                FriendUsername = message.ReadString(),
                FriendDisplayName = message.ReadString(),
            };
        }
    }
}
