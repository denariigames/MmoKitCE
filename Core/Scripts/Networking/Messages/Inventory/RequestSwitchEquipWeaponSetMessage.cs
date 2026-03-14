using LiteNetLib.Utils;

namespace MultiplayerARPG
{
    public struct RequestSwitchEquipWeaponSetMessage : INetSerializable
    {
        public byte equipWeaponSet;

        public void Deserialize(NetDataReader reader)
        {
            equipWeaponSet = reader.GetByte();
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(equipWeaponSet);
        }
    }
}
