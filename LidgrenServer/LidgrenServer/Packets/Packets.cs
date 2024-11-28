using Lidgren.Network;
using LidgrenServer.models;

namespace LidgrenServer.Packets
{
    public class PacketTypes
    {
        public enum General : byte
        {
            Login = 0,
            SignUp,
            PlayerDisconnectsPacket,
            BasicUserInfoPacket,
            ChangeDisplayNamePacket,
            Logout
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
            ExitRoomPacket,
            ChangeTeamPacket,
            SendChatMessagePacket
        }

        public enum Friend : byte
        {
            FriendOnlinePacket = 30
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

    public class SendChatMessagePacket : Packet
    {
        public string Username { get; set; }
        public string DisplayName { get; set; }
        public int RoomID { get; set; }
        public string Message { get; set; }
        public override void PacketToNetOutGoingMessage(NetOutgoingMessage message)
        {
            message.Write((byte)PacketTypes.Room.SendChatMessagePacket);
            message.Write(Username);
            message.Write(DisplayName);
            message.Write(RoomID);
            message.Write(Message);
        }

        public override void NetIncomingMessageToPacket(NetIncomingMessage message)
        {
            Username = message.ReadString();
            DisplayName = message.ReadString();
            RoomID = message.ReadInt32();
            Message = message.ReadString();
        }
    }

    public class FriendOnlinePacket : Packet
    {
        public string username;
        public string displayName;
        public override void PacketToNetOutGoingMessage(NetOutgoingMessage message)
        {
            message.Write((byte)PacketTypes.Friend.FriendOnlinePacket);
            message.Write(username);
            message.Write(displayName);
        }

        public override void NetIncomingMessageToPacket(NetIncomingMessage message)
        {
            username = message.ReadString();
            displayName = message.ReadString();
        }
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

    public class Logout : Packet
    {
        public string username;

        public override void PacketToNetOutGoingMessage(NetOutgoingMessage message)
        {
            message.Write((byte)PacketTypes.General.Logout);
        }

        public override void NetIncomingMessageToPacket(NetIncomingMessage message)
        {
            username = message.ReadString();
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
        public string error { get; set; }

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
        public RoomPacket room { get; set; }
        public override void NetIncomingMessageToPacket(NetIncomingMessage message)
        {
            room = RoomPacket.Deserialize(message);
        }

        public override void PacketToNetOutGoingMessage(NetOutgoingMessage message)
        {
            message.Write((byte)PacketTypes.Room.JoinRoomPacket);
            room.Serialize(message);
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
        public List<PlayerInRoomPacket> Players { get; set; } = new List<PlayerInRoomPacket>();
        public override void NetIncomingMessageToPacket(NetIncomingMessage message)
        {
            int playerCount = message.ReadInt32();
            Players.Clear();
            for (int i = 0; i < playerCount; i++)
            {
                Players.Add(PlayerInRoomPacket.Deserialize(message));
            }

        }

        public override void PacketToNetOutGoingMessage(NetOutgoingMessage message)
        {
            message.Write((byte)PacketTypes.Room.JoinRoomPacketToAll);
            message.Write(Players.Count);
            foreach (var p in Players)
            {
                p.Serialize(message);
            }   
            
        }
    }

    public class ChangeTeamPacket : Packet
    {
        public string username {  get; set; }
        public int roomId { get; set; }
        public Team team { get; set; }
        public override void NetIncomingMessageToPacket(NetIncomingMessage message)
        {
            username = message.ReadString();
            roomId = message.ReadInt16();
            team = (Team)message.ReadByte();
        }

        public override void PacketToNetOutGoingMessage(NetOutgoingMessage message)
        {
            message.Write(username);
            message.Write(roomId);
            message.Write((byte)team);
        }
    }
}
