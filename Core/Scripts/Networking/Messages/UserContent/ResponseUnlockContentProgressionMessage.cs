using LiteNetLib.Utils;

namespace MultiplayerARPG
{
    [System.Serializable]
    public struct ResponseUnlockContentProgressionMessage : INetSerializable
    {
        public UITextKeys message;
        public UnlockableContent unlockableContent;

        public void Deserialize(NetDataReader reader)
        {
            message = (UITextKeys)reader.GetPackedUShort();
            unlockableContent = reader.Get<UnlockableContent>();
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.PutPackedUShort((ushort)message);
            writer.Put(unlockableContent);
        }
    }
}
