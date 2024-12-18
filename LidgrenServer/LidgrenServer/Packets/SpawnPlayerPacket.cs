using Lidgren.Network;
using LidgrenServer.models;

namespace LidgrenServer.Packets
{
    public class SpawnPlayerPacket
    {
        public float X { get; set; }
        public float Y { get; set; }
        public string playerSpawn { get; set; }
        public int HP { get; set; }
        public int Attack { get; set; }
        public int Amor { get; set; }
        public int Lucky { get; set; }
        public Team Team { get; set; }

        public void Serialize(NetOutgoingMessage message)
        {
            message.Write(X);
            message.Write(Y);
            message.Write(playerSpawn);
            message.Write(HP);
            message.Write(Attack);
            message.Write(Amor);
            message.Write(Lucky);
            message.Write((byte)Team);
        }

        public static SpawnPlayerPacket Deserialize(NetIncomingMessage message)
        {
            return new SpawnPlayerPacket
            {
                X = message.ReadFloat(),
                Y = message.ReadFloat(),
                playerSpawn = message.ReadString(),
                HP = message.ReadInt32(),
                Attack = message.ReadInt32(),
                Amor = message.ReadInt32(),
                Lucky = message.ReadInt32(),
                Team = (Team)message.ReadByte()
            };
        }
    }
}
