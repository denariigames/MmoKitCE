using System.Collections.Generic;
using LiteNetLib.Utils;

namespace MultiplayerARPG
{
    public class DealingCharacterItems : List<CharacterItem>, INetSerializable
    {
        public void Serialize(NetDataWriter writer)
        {
            writer.PutPackedInt(Count);
            for (int i = 0; i < Count; ++i)
            {
                writer.Put(this[i]);
            }
        }

        public void Deserialize(NetDataReader reader)
        {
            Clear();
            int count = reader.GetPackedInt();
            for (int i = 0; i < count; ++i)
            {
                Add(reader.Get<CharacterItem>());
            }
        }
    }
}
