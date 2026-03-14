using LiteNetLib.Utils;
using LiteNetLibManager;

namespace MultiplayerARPG
{
    [System.Serializable]
    public struct ItemDropData : INetSerializable
    {
        public bool putOnPlaceholder;
        public CharacterItem characterItem;

        public void Deserialize(NetDataReader reader)
        {
            putOnPlaceholder = reader.GetBool();
            characterItem = reader.Get<CharacterItem>();
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(putOnPlaceholder);
            writer.Put(characterItem);
        }
    }

    [System.Serializable]
    public class SyncFieldItemDropData : LiteNetLibSyncField<ItemDropData>
    {
        protected override bool IsValueChanged(ItemDropData oldValue, ItemDropData newValue)
        {
            return true;
        }
    }
}