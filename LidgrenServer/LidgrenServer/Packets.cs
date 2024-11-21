using System.Diagnostics;
using Lidgren.Network;

namespace LidgrenServer
{
    public class PacketTypes
    {
        public enum General
        {
            Login,
            PlayerDisconnectsPacket,
            BasicUserInfoPacket
        }

        public enum Shop
        {
            LoadShopPacket,
            RequestBuyPacket,
        }

        
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
            message.Write((byte)PacketTypes.General.Login);
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
            message.Write((byte)PacketTypes.General.PlayerDisconnectsPacket);
            message.Write(player);
        }

        public override void NetIncomingMessageToPacket(NetIncomingMessage message)
        {
            player = message.ReadString();
        }
    }

    public class BasicUserInfoPacket : Packet
    {
        public string userName { get; set; }
        public string deviceId { get; set; }
        public int coin { get; set; }
        public string displayName { get; set; }


        public override void NetIncomingMessageToPacket(NetIncomingMessage message)
        {
            throw new NotImplementedException();
        }

        public override void PacketToNetOutGoingMessage(NetOutgoingMessage message)
        {
            throw new NotImplementedException();
        }
    }
}
