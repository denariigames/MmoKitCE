using LiteNetLib.Utils;
using LiteNetLibManager;

namespace MultiplayerARPG
{
    public struct CharacterSummoner : INetSerializable
    {
        public SummonType type;
        public uint objectId;

        public void Serialize(NetDataWriter writer)
        {
            writer.Put((byte)type);
            if (type != SummonType.None)
                writer.PutPackedUInt(objectId);
        }

        public void Deserialize(NetDataReader reader)
        {
            type = (SummonType)reader.GetByte();
            if (type != SummonType.None)
                objectId = reader.GetPackedUInt();
        }
    }

    [System.Serializable]
    public class SyncFieldCharacterSummoner : LiteNetLibSyncField<CharacterSummoner>
    {
        protected override bool IsValueChanged(CharacterSummoner oldValue, CharacterSummoner newValue)
        {
            return oldValue.type != newValue.type ||
                oldValue.objectId != newValue.objectId;

        }
    }
}