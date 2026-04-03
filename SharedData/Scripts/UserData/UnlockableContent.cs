using Cysharp.Text;
using LiteNetLib.Utils;

namespace MultiplayerARPG
{
    [System.Serializable]
    public struct UnlockableContent : INetSerializable
    {
        public UnlockableContentType type;
        public int dataId;
        public int progression;
        public bool unlocked;

        public void Deserialize(NetDataReader reader)
        {
            type = (UnlockableContentType)reader.GetByte();
            dataId = reader.GetPackedInt();
            progression = reader.GetPackedInt();
            unlocked = reader.GetBool();
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put((byte)type);
            writer.PutPackedInt(dataId);
            writer.PutPackedInt(progression);
            writer.Put(unlocked);
        }

        public string GetId()
        {
            return ZString.Concat((byte)type, dataId);
        }
    }
}
