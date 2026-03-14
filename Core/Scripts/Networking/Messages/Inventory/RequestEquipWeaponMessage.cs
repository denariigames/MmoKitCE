using LiteNetLib.Utils;

namespace MultiplayerARPG
{
    public struct RequestEquipWeaponMessage : INetSerializable
    {
        public int nonEquipIndex;
        public byte equipWeaponSet;
        public bool isLeftHand;

        public void Deserialize(NetDataReader reader)
        {
            nonEquipIndex = reader.GetPackedInt();
            equipWeaponSet = reader.GetByte();
            isLeftHand = reader.GetBool();
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.PutPackedInt(nonEquipIndex);
            writer.Put(equipWeaponSet);
            writer.Put(isLeftHand);
        }
    }
}
