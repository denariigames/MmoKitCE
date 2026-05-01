using LiteNetLib.Utils;
using System.Collections.Generic;

namespace MultiplayerARPG.MMO
{
#nullable enable
    public partial struct UpdateStorageAndCharacterItemsReq : INetSerializable
    {
        public StorageType StorageType { get; set; }
        public string StorageOwnerId { get; set; }
        public List<CharacterItem> StorageItems { get; set; }
        public string CharacterId { get; set; }
        public List<EquipWeapons> SelectableWeaponSets { get; set; }
        public List<CharacterItem> EquipItems { get; set; }
        public List<CharacterItem> NonEquipItems { get; set; }
        public bool DeleteStorageReservation { get; set; }

        public void Deserialize(NetDataReader reader)
        {
            StorageType = (StorageType)reader.GetByte();
            StorageOwnerId = reader.GetString();
            StorageItems = reader.GetList<CharacterItem>();
            CharacterId = reader.GetString();
            SelectableWeaponSets = reader.GetList<EquipWeapons>();
            EquipItems = reader.GetList<CharacterItem>();
            NonEquipItems = reader.GetList<CharacterItem>();
            DeleteStorageReservation = reader.GetBool();
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put((byte)StorageType);
            writer.Put(StorageOwnerId);
            writer.PutList(StorageItems);
            writer.Put(CharacterId);
            writer.PutList(SelectableWeaponSets);
            writer.PutList(EquipItems);
            writer.PutList(NonEquipItems);
            writer.Put(DeleteStorageReservation);
        }
    }
}
