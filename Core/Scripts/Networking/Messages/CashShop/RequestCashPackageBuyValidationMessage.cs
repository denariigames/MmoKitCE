using LiteNetLib.Utils;
using UnityEngine;

namespace MultiplayerARPG
{
    public struct RequestCashPackageBuyValidationMessage : INetSerializable
    {
        public int dataId;
        public RuntimePlatform platform;
        public string receipt;

        public void Deserialize(NetDataReader reader)
        {
            dataId = reader.GetPackedInt();
            platform = (RuntimePlatform)reader.GetByte();
            receipt = reader.GetString();
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.PutPackedInt(dataId);
            writer.Put((byte)platform);
            writer.Put(receipt);
        }
    }
}
