using LiteNetLib.Utils;
using System.Collections.Generic;

namespace MultiplayerARPG.MMO
{
#nullable enable
    public partial struct GetStorageItemsResp : INetSerializable
    {
        public UITextKeys Error { get; set; }
        public List<CharacterItem> StorageItems { get; set; }

        public void Deserialize(NetDataReader reader)
        {
            Error = (UITextKeys)reader.GetByte();
            StorageItems = reader.GetList<CharacterItem>();
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put((byte)Error);
            writer.PutList(StorageItems);
        }
    }
}