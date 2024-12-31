using Lidgren.Network;

namespace LidgrenServer.Packets
{
    public class CharacterPacket
    {
        public int UserCharacterId { get; set; }
        public string CharacterName { get; set; }
        public int CharacterLevel { get; set; }
        public int CharacterXp { get; set; }
        public int CharacterHp { get; set; }
        public int CharacterDamage { get; set; }
        public int CharacterArmor { get; set; }
        public int CharacterLuck { get; set; }
        public bool IsSelected { get; set; }
        public int HpPoint { get; set; }
        public int DamagePoint {  get; set; }
        public int ArmorPoint { get; set; }
        public int LuckPoint { get; set; }

        public void Serialize(NetOutgoingMessage message)
        {
            message.Write(UserCharacterId);
            message.Write(CharacterName);
            message.Write(CharacterLevel);
            message.Write(CharacterXp);
            message.Write(CharacterHp);
            message.Write(CharacterDamage);
            message.Write(CharacterArmor);
            message.Write(CharacterLuck);
            message.Write(IsSelected);
            message.Write(HpPoint);
            message.Write(DamagePoint);
            message.Write(ArmorPoint);
            message.Write(LuckPoint);
        }

        public static CharacterPacket Deserialize(NetIncomingMessage message)
        {
            return new CharacterPacket()
            {
                UserCharacterId = message.ReadInt32(),
                CharacterName = message.ReadString(),
                CharacterLevel = message.ReadInt32(),
                CharacterXp = message.ReadInt32(),
                CharacterHp = message.ReadInt32(),
                CharacterDamage = message.ReadInt32(),
                CharacterArmor = message.ReadInt32(),
                CharacterLuck = message.ReadInt32(),
                IsSelected = message.ReadBoolean(),
                HpPoint = message.ReadInt32(),
                DamagePoint = message.ReadInt32(),
                ArmorPoint = message.ReadInt32(),
                LuckPoint = message.ReadInt32(),
            };
        }
    }
}