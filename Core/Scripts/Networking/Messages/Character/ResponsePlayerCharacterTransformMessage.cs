using LiteNetLib.Utils;

namespace MultiplayerARPG
{
    public struct ResponsePlayerCharacterTransformMessage : INetSerializable
    {
        public UITextKeys message;
        public Vec3 position;
        public Vec3 rotation;

        public void Deserialize(NetDataReader reader)
        {
            message = (UITextKeys)reader.GetPackedUShort();
            position = reader.Get<Vec3>();
            rotation = reader.Get<Vec3>();
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.PutPackedUShort((ushort)message);
            writer.Put(position);
            writer.Put(rotation);
        }
    }
}
