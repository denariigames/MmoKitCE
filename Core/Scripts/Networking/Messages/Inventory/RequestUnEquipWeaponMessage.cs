using LiteNetLib.Utils;

namespace MultiplayerARPG
{
    public struct RequestUnEquipWeaponMessage : INetSerializable
    {
        public byte equipWeaponSet;
        public bool isLeftHand;
        public int nonEquipIndex;

        public void Deserialize(NetDataReader reader)
        {
            equipWeaponSet = reader.GetByte();
            isLeftHand = reader.GetBool();
            nonEquipIndex = reader.GetPackedInt();
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(equipWeaponSet);
            writer.Put(isLeftHand);
            writer.PutPackedInt(nonEquipIndex);
        }
    }
}
