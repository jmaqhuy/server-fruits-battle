using System.Diagnostics;
using Lidgren.Network;

namespace LidgrenServer
{
    public enum PacketTypes
    {
        Login,
        PlayerDisconnectsPacket
    }

    public interface IPacket
    {
        void PacketToNetOutGoingMessage(NetOutgoingMessage message);

        void NetIncomingMessageToPacket(NetIncomingMessage message);
    }

    public abstract class Packet : IPacket
    {
        public abstract void PacketToNetOutGoingMessage(NetOutgoingMessage message);

        public abstract void NetIncomingMessageToPacket(NetIncomingMessage message);
    }

    public class Login : Packet
    {
        public string username { get; set; }
        public string password { get; set; }
        public bool isSuccess { get; set; }
        public override void NetIncomingMessageToPacket(NetIncomingMessage message)
        {
            username = message.ReadString();
            password = message.ReadString();
            isSuccess = message.ReadBoolean();
            Logging.Debug($"NetIncomingMessageToPacket: username: {username}, password: {password}, isSuccess: {isSuccess}");
        }

        public override void PacketToNetOutGoingMessage(NetOutgoingMessage message)
        {
            message.Write((byte)PacketTypes.Login);
            message.Write(username);
            message.Write(password);
            message.Write(isSuccess);
            Logging.Debug($"PacketToNetOutGoingMessage: username: {username}, password: {password}, isSuccess: {isSuccess}");
        }
    }
    public class PlayerDisconnectsPacket : Packet
    {
        public string player { get; set; }

        public override void PacketToNetOutGoingMessage(NetOutgoingMessage message)
        {
            message.Write((byte)PacketTypes.PlayerDisconnectsPacket);
            message.Write(player);
        }

        public override void NetIncomingMessageToPacket(NetIncomingMessage message)
        {
            player = message.ReadString();
        }
    }
}
