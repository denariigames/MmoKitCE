using LiteNetLib.Utils;

namespace MultiplayerARPG
{
    public struct UpdateTimeOfDayMessage : INetSerializable
    {
        public float timeOfDay;

        public void Deserialize(NetDataReader reader)
        {
            timeOfDay = reader.GetFloat();
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put(timeOfDay);
        }
    }
}
