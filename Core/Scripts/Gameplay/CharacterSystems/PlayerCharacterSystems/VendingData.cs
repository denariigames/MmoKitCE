using LiteNetLib.Utils;
using LiteNetLibManager;

namespace MultiplayerARPG
{
    [System.Serializable]
    public struct VendingData : INetSerializable
    {
        public bool isStarted;
        public string title;

        public void Deserialize(NetDataReader reader)
        {
            isStarted = reader.GetBool();
            if (isStarted)
                title = reader.GetString();
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(isStarted);
            if (isStarted)
                writer.Put(title);
        }
    }

    [System.Serializable]
    public class SyncFieldVendingData : LiteNetLibSyncField<VendingData>
    {
        protected override bool IsValueChanged(VendingData oldValue, VendingData newValue)
        {
            return oldValue.isStarted != newValue.isStarted ||
                !string.Equals(oldValue.title, newValue.title);
        }
    }
}