using System.Diagnostics;
using Lidgren.Network;
using LidgrenServer.models;

namespace LidgrenServer
{
    public class PacketTypes
    {
        public enum General : byte
        {
            Login = 0,
            SignUp,
            PlayerDisconnectsPacket,
            BasicUserInfoPacket,
            ChangeDisplayNamePacket
        }

        public enum Shop : byte
        {
            LoadShopPacket = 10,
            RequestBuyPacket,
        }

        public enum Room : byte
        {
            JoinRoomPacket = 20,
            JoinRoomPacketToAll,
            ExitRoomPacket
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

    public class SignUp : Packet
    {
        public string username { get; set; }
        public string password { get; set; }
        public bool isSuccess { get; set; }
        public string reason { get; set; }

        public override void NetIncomingMessageToPacket(NetIncomingMessage message)
        {
            username = message.ReadString();
            password = message.ReadString();
            isSuccess = message.ReadBoolean();
            reason = message.ReadString();
            Logging.Debug($"NetIncomingMessageToPacket: username: {username}, password: {password}, isSuccess: {isSuccess}, reason: {reason}");
        }

        public override void PacketToNetOutGoingMessage(NetOutgoingMessage message)
        {
            message.Write((byte)PacketTypes.General.SignUp);
            message.Write(username);
            message.Write(password);
            message.Write(isSuccess);
            message.Write(reason);
            Logging.Debug($"PacketToNetOutGoingMessage: username: {username}, password: {password}, isSuccess: {isSuccess}, reason: {reason}");
        }
    }
    public class PlayerDisconnectsPacket : Packet
    {
        public string username { get; set; }

        public override void PacketToNetOutGoingMessage(NetOutgoingMessage message)
        {
            message.Write((byte)PacketTypes.General.PlayerDisconnectsPacket);
            message.Write(username);
        }

        public override void NetIncomingMessageToPacket(NetIncomingMessage message)
        {
            username = message.ReadString();
        }
    }

    public class BasicUserInfoPacket : Packet
    {
        public string userName { get; set; }
        public int coin { get; set; }
        public string displayName { get; set; }


        public override void NetIncomingMessageToPacket(NetIncomingMessage message)
        {
            userName = message.ReadString();
            coin = message.ReadInt32();
            displayName = message.ReadString();
            Logging.Debug($"NetIncomingMessageToPacket: username: {userName}, coin: {coin}, displayName: {displayName}");
        }

        public override void PacketToNetOutGoingMessage(NetOutgoingMessage message)
        {
            message.Write((byte)PacketTypes.General.BasicUserInfoPacket);
            message.Write(userName);
            message.Write(coin);
            message.Write(displayName);
            Logging.Debug($"PacketToNetOutGoingMessage: username: {userName}, coin: {coin}, displayName: {displayName}");
        }
    }

    public class ChangeDisplayNamePacket : Packet
    {
        public string username { get; set; }
        public string newDisplayName { get; set; }
        public string error {  get; set; }

        public bool Ok { get; set; }
        public override void NetIncomingMessageToPacket(NetIncomingMessage message)
        {
            username = message.ReadString();
            newDisplayName = message.ReadString();
            error = message.ReadString();
            Ok = message.ReadBoolean();
            Logging.Info($"NetIncomingMessageToPacket: username: {username}, newDisplayName: {newDisplayName}, error: {error}");
        }

        public override void PacketToNetOutGoingMessage(NetOutgoingMessage message)
        {
            message.Write((byte)PacketTypes.General.ChangeDisplayNamePacket);
            message.Write(username);
            message.Write(newDisplayName);
            message.Write(error);
            message.Write(Ok);
        }
    }

    public class JoinRoomPacket : Packet
    {
        public string username { get; set; }
        public string displayName { get; set; }
        public Team team { get; set; }
        public bool isHost { get; set; }
        public int position { get; set; }
        public int roomId { get; set; }
        public string roomName { get; set; }
        public RoomMode roomMode { get; set; }
        public RoomType roomType { get; set; }
        public override void NetIncomingMessageToPacket(NetIncomingMessage message)
        {
            username = message.ReadString();
            displayName = message.ReadString();
            team = (Team)message.ReadByte();
            isHost = message.ReadBoolean();
            position = message.ReadInt32();
            roomId = message.ReadInt32();
            roomName = message.ReadString();
            roomMode = (RoomMode)message.ReadByte();
            roomType = (RoomType)message.ReadByte();
        }

        public override void PacketToNetOutGoingMessage(NetOutgoingMessage message)
        {
            message.Write((byte)PacketTypes.Room.JoinRoomPacket);
            message.Write(username);
            message.Write(displayName);
            message.Write((byte)team);
            message.Write(isHost);
            message.Write(position);
            message.Write(roomId);
            message.Write(roomName);
            message.Write((byte)roomMode);
            message.Write((byte)roomType);
        }
    }
    public class ExitRoomPacket : Packet
    {
        public string username { get; set; }
        public int roomId { get; set; }

        public override void PacketToNetOutGoingMessage(NetOutgoingMessage message)
        {
            message.Write((byte)PacketTypes.Room.ExitRoomPacket);
            message.Write(username);
            message.Write(roomId);
        }

        public override void NetIncomingMessageToPacket(NetIncomingMessage message)
        {
            username = message.ReadString();
            roomId = message.ReadInt32();
        }
    }

    public class JoinRoomPacketToAll : Packet
    {

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
