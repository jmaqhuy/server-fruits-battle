using Lidgren.Network;
using LidgrenServer.models;
using static LidgrenServer.Packets.PacketTypes;

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
            Logout,
            ResetPassword,
            ChangePassword,
            VerifyRegistrationPacket,
            RequireVerifyPacket
        }

        public enum Shop : byte
        {
            LoadShopPacket = 10,
            RequestBuyPacket,
        }

        public enum Room : byte
        {
            CreateRoomPacket = 20,
            JoinRoomPacket,
            JoinRoomPacketToAll,
            ExitRoomPacket,
            InviteFriendPacket,
            ChangeTeamPacket,
            SendChatMessagePacket,
            PlayerReadyPacket,
            RoomListPacket,
            ChangeRoomTypePacket
        }
        public enum GameBattle : byte
        {
            StartGamePacket = 30,
            PlayerOutGamePacket,
            StartTurnPacket,
            EndTurnPacket,
            EndGamePacket,
            PositionPacket,
            HealthPointPacket,
            PlayerDiePacket,
            Shoot,
            SpawnPlayerPacketToAll,
            AlreadyInMatchPacket,
        }

        public enum Friend : byte
        {
            AllFriendPacket = 50,
            FriendRequestPacket,
            SentRequestPacket,
            SuggestFriendPacket,
            SearchFriendPacket,
            BlockFriendPacket,
            AddFriendPacket,
            DeleteFriend,
            AcceptFriendInvite,
            CancelFriendRequest,
            BlockFriend,
            UnBlockFriend
        }

        public enum Character : byte
        {
            GetCurrentCharacterPacket = 70,
            ChangeCharacterPoint
        }
        public enum Rank : byte
        {
            CurrentRankPacket = 80,
            MatchmakingPacket,
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

    /*public class FriendOnlinePacket : Packet
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
    }*/

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
    public class ResetPassword : Packet
    {
        public string username { get; set; }
        public string email { get; set; }
        public bool isSuccess { get; set; }
        public string reason { get; set; }


        public override void PacketToNetOutGoingMessage(NetOutgoingMessage message)
        {
            message.Write((byte)PacketTypes.General.ResetPassword);
            message.Write(isSuccess);
            message.Write(reason);
        }

        public override void NetIncomingMessageToPacket(NetIncomingMessage message)
        {
            username = message.ReadString();
            email = message.ReadString();
        }
    }
    public class ChangePassword : Packet
    {
        public string username { get; set; }
        public string oldPassword { get; set; }
        public string newPass { get; set; }
        public string confirmPass { get; set; }
        public bool isSuccess { get; set; }
        public string reason { get; set; }


        public override void PacketToNetOutGoingMessage(NetOutgoingMessage message)
        {
            message.Write((byte)PacketTypes.General.ChangePassword);
            message.Write(username);
            message.Write(oldPassword);
            message.Write(newPass);
            message.Write(confirmPass);
            message.Write(isSuccess);
            message.Write(reason);
        }

        public override void NetIncomingMessageToPacket(NetIncomingMessage message)
        {
            username = message.ReadString();
            oldPassword = message.ReadString();
            newPass = message.ReadString();
            confirmPass = message.ReadString();
            isSuccess = message.ReadBoolean();
            reason = message.ReadString();
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


    public class VerifyRegistrationPacket : Packet
    {
        public string username { get; set; }
        public string otp { get; set; }
        public bool isSuccess { get; set; }

        public string reason { get; set; }
        public override void PacketToNetOutGoingMessage(NetOutgoingMessage message)
        {
            message.Write((byte)PacketTypes.General.VerifyRegistrationPacket);
            message.Write(isSuccess);
            message.Write(reason);
        }

        public override void NetIncomingMessageToPacket(NetIncomingMessage message)
        {
            username = message.ReadString() ;
            otp = message.ReadString() ;
        }
    }

    public class SignUp : Packet
    {
        public string username { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public bool isSuccess { get; set; }
        public string reason { get; set; }

        public override void NetIncomingMessageToPacket(NetIncomingMessage message)
        {
            username = message.ReadString();
            email = message.ReadString();
            password = message.ReadString();
            Logging.Debug($"NetIncomingMessageToPacket: username: {username}, email: {email}, password: {password}");
        }

        public override void PacketToNetOutGoingMessage(NetOutgoingMessage message)
        {
            message.Write((byte)PacketTypes.General.SignUp);
            message.Write(username);
            message.Write(isSuccess);
            message.Write(reason);
            Logging.Debug($"PacketToNetOutGoingMessage: username: {username}, isSuccess: {isSuccess}, reason: {reason}");
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
            team = (Team)message.ReadByte();
            username = message.ReadString();
            roomId = message.ReadInt16();
            
        }

        public override void PacketToNetOutGoingMessage(NetOutgoingMessage message)
        {
            message.Write((byte)PacketTypes.Room.ChangeTeamPacket);
            message.Write((byte)team);
            message.Write(username);
            message.Write(roomId);
            
            Logging.Info($"Send Change Team Packet {team}");
        }
    }
    public class SuggestFriendPacket : Packet
    {
        public string username { get; set; }
        public List<FriendTabPacket> Friends { get; set; } = new List<FriendTabPacket>();
        public override void PacketToNetOutGoingMessage(NetOutgoingMessage message)
        {
            message.Write((byte)PacketTypes.Friend.SuggestFriendPacket);
            message.Write(Friends.Count);
            foreach (var f in Friends)
            {
                f.Serialize(message);
            }
        }

        public override void NetIncomingMessageToPacket(NetIncomingMessage message)
        {
            username = message.ReadString();
        }
    }
    public class SearchFriendPacket : Packet
    {
        public string username1 { get; set; }
        public string username2 { get; set; }

        public List<FriendTabPacket> Friends { get; set; } = new List<FriendTabPacket>();
        public override void PacketToNetOutGoingMessage(NetOutgoingMessage message)
        {
            message.Write((byte)PacketTypes.Friend.SearchFriendPacket);
            message.Write(Friends.Count);
            foreach (var f in Friends)
            {
                f.Serialize(message);
            }
        }
        public override void NetIncomingMessageToPacket(NetIncomingMessage message)
        {
            username1 = message.ReadString();
            username2 = message.ReadString();
        }
    }
    public class AllFriendPacket : Packet
    {
        public string username { get; set; }
        public List<FriendTabPacket> Friends { get; set; } = new List<FriendTabPacket>();
        public override void PacketToNetOutGoingMessage(NetOutgoingMessage message)
        {
            message.Write((byte)PacketTypes.Friend.AllFriendPacket);
            message.Write(Friends.Count);
            foreach (var f in Friends)
            {
                f.Serialize(message);
            }
        }

        public override void NetIncomingMessageToPacket(NetIncomingMessage message)
        {
            username = message.ReadString();
        }
    }
    public class FriendRequestPacket : Packet
    {
        public string username { get; set; }
        public List<FriendTabPacket> Friends { get; set; } = new List<FriendTabPacket>();
        public override void PacketToNetOutGoingMessage(NetOutgoingMessage message)
        {
            message.Write((byte)PacketTypes.Friend.FriendRequestPacket);
            message.Write(Friends.Count);
            foreach (var f in Friends)
            {
                f.Serialize(message);
            }
        }

        public override void NetIncomingMessageToPacket(NetIncomingMessage message)
        {
            username = message.ReadString();
        }
    }
    public class SentRequestPacket : Packet
    {
        public string username { get; set; }
        public List<FriendTabPacket> Friends { get; set; } = new List<FriendTabPacket>();
        public override void PacketToNetOutGoingMessage(NetOutgoingMessage message)
        {
            message.Write((byte)PacketTypes.Friend.SentRequestPacket);
            message.Write(Friends.Count);
            foreach (var f in Friends)
            {
                f.Serialize(message);
            }
        }

        public override void NetIncomingMessageToPacket(NetIncomingMessage message)
        {
            username = message.ReadString();
        }
    }
    public class BlockFriendPacket : Packet
    {
        public string username { get; set; }
        public List<FriendTabPacket> Friends { get; set; } = new List<FriendTabPacket>();
        public override void PacketToNetOutGoingMessage(NetOutgoingMessage message)
        {
            message.Write((byte)PacketTypes.Friend.BlockFriendPacket);
            message.Write(Friends.Count);
            foreach (var f in Friends)
            {
                f.Serialize(message);
            }
        }

        public override void NetIncomingMessageToPacket(NetIncomingMessage message)
        {
            username = message.ReadString();
        }
    }
    public class AddFriendPacket : Packet
    {
        public string username1 { get; set; }
        public string username2 { get; set; }

        public bool IsSuccess { get; set; }
        public override void PacketToNetOutGoingMessage(NetOutgoingMessage message)
        {
            message.Write((byte)PacketTypes.Friend.AddFriendPacket);
            message.Write(IsSuccess);
        }
        public override void NetIncomingMessageToPacket(NetIncomingMessage message)
        { 
            username1=message.ReadString();
            username2=message.ReadString();
        }
    }
    public class DeleteFriend : Packet
    {
        public string username1 { get; set; }
        public string username2 { get; set; }

        public bool IsSuccess { get; set; }
        public override void PacketToNetOutGoingMessage(NetOutgoingMessage message)
        {
            message.Write((byte)PacketTypes.Friend.DeleteFriend);
            message.Write(IsSuccess);
        }
        public override void NetIncomingMessageToPacket(NetIncomingMessage message)
        {
            username1 = message.ReadString();
            username2 = message.ReadString();
        }
    }
    public class AcceptFriendInvite : Packet
    {
        public string username1 { get; set; }
        public string username2 { get; set; }

        public bool IsSuccess { get; set; }
        public override void PacketToNetOutGoingMessage(NetOutgoingMessage message)
        {
            message.Write((byte)PacketTypes.Friend.AcceptFriendInvite);
            message.Write(IsSuccess);
        }
        public override void NetIncomingMessageToPacket(NetIncomingMessage message)
        {
            username1 = message.ReadString();
            username2 = message.ReadString();
        }
    }
    public class CancelFriendRequest : Packet
    {
        public string username1 { get; set; }
        public string username2 { get; set; }

        public bool IsSuccess { get; set; }
        public override void PacketToNetOutGoingMessage(NetOutgoingMessage message)
        {
            message.Write((byte)PacketTypes.Friend.CancelFriendRequest);
            message.Write(IsSuccess);
        }
        public override void NetIncomingMessageToPacket(NetIncomingMessage message)
        {
            username1 = message.ReadString();
            username2 = message.ReadString();
        }
    }
    public class BlockFriend : Packet
    {
        public string username1 { get; set; }
        public string username2 { get; set; }

        public bool IsSuccess { get; set; }
        public override void PacketToNetOutGoingMessage(NetOutgoingMessage message)
        {
            message.Write((byte)PacketTypes.Friend.BlockFriend);
            message.Write(IsSuccess);
        }
        public override void NetIncomingMessageToPacket(NetIncomingMessage message)
        {
            username1 = message.ReadString();
            username2 = message.ReadString();
        }
    }
    public class UnBlockFriend : Packet
    {
        public string username1 { get; set; }
        public string username2 { get; set; }

        public bool IsSuccess { get; set; }
        public override void PacketToNetOutGoingMessage(NetOutgoingMessage message)
        {
            message.Write((byte)PacketTypes.Friend.UnBlockFriend);
            message.Write(IsSuccess);
        }
        public override void NetIncomingMessageToPacket(NetIncomingMessage message)
        {
            username1 = message.ReadString();
            username2 = message.ReadString();
        }
    }
    public class GetCurrentCharacterPacket : Packet
    {
        public string Username { get; set; }
        public CharacterPacket Character { get; set; }
        public override void PacketToNetOutGoingMessage(NetOutgoingMessage message)
        {
            message.Write((byte)PacketTypes.Character.GetCurrentCharacterPacket);
            Character.Serialize(message);
        }

        public override void NetIncomingMessageToPacket(NetIncomingMessage message)
        {
            Username = message.ReadString();
        }
    }
    public class ChangeCharacterPointPacket : Packet
    {
        public CharacterPacket Character { get; set; }
        public override void PacketToNetOutGoingMessage(NetOutgoingMessage message)
        {
            message.Write((byte)PacketTypes.Character.ChangeCharacterPoint);
            Character.Serialize(message);
        }

        public override void NetIncomingMessageToPacket(NetIncomingMessage message)
        {
            Character = CharacterPacket.Deserialize(message);
        }
    }

    public class PlayerReadyPacket : Packet
    {
        public string Username { get; set; }
        public bool IsReady { get; set; }
        public int RoomId { get; set; }
        public override void PacketToNetOutGoingMessage(NetOutgoingMessage message)
        {
            message.Write((byte)PacketTypes.Room.PlayerReadyPacket);
            message.Write(Username);
            message.Write(IsReady);
            message.Write(RoomId);
        }

        public override void NetIncomingMessageToPacket(NetIncomingMessage message)
        {
            Username = message.ReadString();
            IsReady = message.ReadBoolean();
            RoomId = message.ReadInt32();
        }
    }

    public class StartGamePacket : Packet
    {
        public int roomId { get; set; }

        public override void PacketToNetOutGoingMessage(NetOutgoingMessage message)
        {
            message.Write((byte)PacketTypes.GameBattle.StartGamePacket);
            message.Write(roomId);
        }

        public override void NetIncomingMessageToPacket(NetIncomingMessage message)
        {
            roomId = message.ReadInt32();
        }
    }
    public class PlayerOutGamePacket : Packet
    {
        public string player { get; set; }
        public override void NetIncomingMessageToPacket(NetIncomingMessage message)
        {
            player = message.ReadString();
        }
        public override void PacketToNetOutGoingMessage(NetOutgoingMessage message)
        {
            message.Write((byte)PacketTypes.GameBattle.PlayerOutGamePacket);
            message.Write(player);
        }
    }
    public class EndTurnPacket : Packet
    {
        public string playerName { get; set; }
        public override void NetIncomingMessageToPacket(NetIncomingMessage message)
        {
            playerName = message.ReadString();
        }
        public override void PacketToNetOutGoingMessage(NetOutgoingMessage message)
        {
            message.Write((byte)PacketTypes.GameBattle.EndTurnPacket);
            message.Write(playerName);
        }
    }
    public class EndGamePacket : Packet
    {
        public Team TeamWin {  get; set; }
        public override void NetIncomingMessageToPacket(NetIncomingMessage message)
        {

        }
        public override void PacketToNetOutGoingMessage(NetOutgoingMessage message)
        {
            message.Write((byte)PacketTypes.GameBattle.EndGamePacket);
            message.Write((byte)TeamWin);
        }
    }
    public class PositionPacket : Packet
    {
        public string playerName { get; set; }
        public float X { get; set; }
        public float Y { get; set; }

        public override void NetIncomingMessageToPacket(NetIncomingMessage message)
        {
            playerName = message.ReadString();
            X = message.ReadFloat();
            Y = message.ReadFloat();

        }
        public override void PacketToNetOutGoingMessage(NetOutgoingMessage message)
        {
            message.Write((byte)PacketTypes.GameBattle.PositionPacket);
            message.Write(playerName);
            message.Write(X);
            message.Write(Y);

        }
    }
    public class HealthPointPacket : Packet
    {
        public int HP { get; set; }
        public string PlayerName { get; set; }
        public override void NetIncomingMessageToPacket(NetIncomingMessage message)
        {
            HP = message.ReadInt32();
            PlayerName = message.ReadString();
        }
        public override void PacketToNetOutGoingMessage(NetOutgoingMessage message)
        {
            message.Write((byte)PacketTypes.GameBattle.HealthPointPacket);
            message.Write(HP);
            message.Write(PlayerName);
        }
    }
    public class PlayerDiePacket : Packet
    {
        public string player { get; set; }
        public override void NetIncomingMessageToPacket(NetIncomingMessage message)
        {
            player = message.ReadString();

        }
        public override void PacketToNetOutGoingMessage(NetOutgoingMessage message)
        {
            message.Write((byte)PacketTypes.GameBattle.PlayerDiePacket);
            message.Write(player);
        }
    }
    public class SpawnPlayerPacketToAll : Packet
    {
        public List<SpawnPlayerPacket> SPPacket = new List<SpawnPlayerPacket>();
        public override void PacketToNetOutGoingMessage(NetOutgoingMessage message)
        {
            message.Write((byte)PacketTypes.GameBattle.SpawnPlayerPacketToAll); 
            message.Write(SPPacket.Count);
            foreach (SpawnPlayerPacket sp in SPPacket)
            {
                sp.Serialize(message);
            }
        }

        public override void NetIncomingMessageToPacket(NetIncomingMessage message)
        {
            var cnt = message.ReadInt32();
            SPPacket.Clear();
            for (int i = 0; i < cnt; i++)
            {
                SPPacket.Add(SpawnPlayerPacket.Deserialize(message));
            }
        }
    }
   
    public class StartTurnPacket : Packet
    {
        public string playerName { get; set; }
        public override void PacketToNetOutGoingMessage(NetOutgoingMessage message)
        {
            message.Write((byte)PacketTypes.GameBattle.StartTurnPacket);
            message.Write(playerName);
        }
        public override void NetIncomingMessageToPacket(NetIncomingMessage message)
        {

        }
    }
    public class Shoot : Packet
    {
        public string playerName { get; set; }
        public float force { get; set; }
        public float angle { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public override void PacketToNetOutGoingMessage(NetOutgoingMessage message)
        {
            message.Write((byte)PacketTypes.GameBattle.Shoot);
            message.Write(force);
            message.Write(angle);
            message.Write(X);
            message.Write(Y);
            message.Write(playerName);
        }
        public override void NetIncomingMessageToPacket(NetIncomingMessage message)
        {

            force = message.ReadFloat();
            angle = message.ReadFloat();
            X = message.ReadFloat();
            Y = message.ReadFloat();
            playerName = message.ReadString();
        }

    }

    public class RoomListPacket : Packet
    {
        public List<RoomPacket> rooms { get; set; } = new List<RoomPacket>();

        public override void PacketToNetOutGoingMessage(NetOutgoingMessage message)
        {
            message.Write((byte)PacketTypes.Room.RoomListPacket);
            message.Write(rooms.Count);
            foreach (var r in rooms)
            {
                r.Serialize(message);
            }
        }

        public override void NetIncomingMessageToPacket(NetIncomingMessage message)
        {
            var roomCount = message.ReadInt32();
            rooms.Clear();
            for (int i = 0; i < roomCount; i++)
            {
                rooms.Add(RoomPacket.Deserialize(message));
            }
        }
    }
    public class AlreadyInMatchPacket : Packet
    {
        public int roomId { get; set; }
        public string username { get; set; }

        public override void NetIncomingMessageToPacket(NetIncomingMessage message)
        {
            roomId = message.ReadInt32();
            username = message.ReadString();
        }

        public override void PacketToNetOutGoingMessage(NetOutgoingMessage message)
        {
            message.Write((byte)PacketTypes.GameBattle.AlreadyInMatchPacket);
            message.Write(roomId);
            message.Write(username);
        }
    }
    public class ChangeRoomTypePacket : Packet
    {
        public RoomPacket room { get; set; }

        public override void NetIncomingMessageToPacket(NetIncomingMessage message)
        {
            room = RoomPacket.Deserialize(message);
        }

        public override void PacketToNetOutGoingMessage(NetOutgoingMessage message)
        {
            message.Write((byte)PacketTypes.Room.ChangeRoomTypePacket);
            room.Serialize(message);
        }
    }

    public class CreateRoomPacket : Packet
    {
        public RoomPacket room { get; set; }

        public override void NetIncomingMessageToPacket(NetIncomingMessage message)
        {
            room = RoomPacket.Deserialize(message);
        }

        public override void PacketToNetOutGoingMessage(NetOutgoingMessage message)
        {
            message.Write((byte)PacketTypes.Room.CreateRoomPacket);
            room.Serialize(message);
        }
    }
    public class CurrentRankPacket : Packet
    {
        public string username { get; set; }
        public string rankName { get; set; }
        public string rankAssetName { get; set; }
        public int currentStar { get; set; }
        public int seasonId { get; set; }
        public string seasonName { get; set; }
        public override void NetIncomingMessageToPacket(NetIncomingMessage message)
        {
            username = message.ReadString();
            seasonId = message.ReadInt32();
        }
        public override void PacketToNetOutGoingMessage(NetOutgoingMessage message)
        {
            message.Write((byte)PacketTypes.Rank.CurrentRankPacket);
            message.Write(username);
            message.Write(rankName);
            message.Write(rankAssetName);
            message.Write(currentStar);
            message.Write(seasonId);
            message.Write(seasonName);
        }
    }
    public class MatchmakingPacket : Packet
    {
        public int roomId { get; set; }
        public bool start { get; set; }
        public bool matchFound { get; set; }
        public override void PacketToNetOutGoingMessage(NetOutgoingMessage message)
        {
            message.Write((byte)PacketTypes.Rank.MatchmakingPacket);
            message.Write(roomId);
            message.Write(start);
            message.Write(matchFound);
        }

        public override void NetIncomingMessageToPacket(NetIncomingMessage message)
        {
            roomId = message.ReadInt32();
            start = message.ReadBoolean();
            matchFound = message.ReadBoolean();
        }
    }
}

