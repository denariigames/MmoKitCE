using LiteNetLib.Utils;

namespace MultiplayerARPG
{
    [System.Serializable]
    public struct ResponseAvailableContentsMessage : INetSerializable
    {
        public UITextKeys message;
        public UnlockableContent[] contents;

        public void Deserialize(NetDataReader reader)
        {
            message = (UITextKeys)reader.GetPackedUShort();
            contents = reader.GetArray<UnlockableContent>();
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.PutPackedUShort((ushort)message);
            writer.PutArray(contents);
        }
    }
}
