using LiteNetLib.Utils;

namespace MultiplayerARPG
{
    [System.Serializable]
    public partial struct EquipWeapons : INetSerializable
    {
        public CharacterItem rightHand;
        public CharacterItem leftHand;
        
        public EquipWeapons Clone(bool generateNewId = false)
        {
            return new EquipWeapons()
            {
                rightHand = rightHand.Clone(generateNewId),
                leftHand = leftHand.Clone(generateNewId),
            };
        }

        public void Serialize(NetDataWriter writer)
        {
            // Right hand
            writer.Put(rightHand);
            // Left hand
            writer.Put(leftHand);
        }

        public void Deserialize(NetDataReader reader)
        {
            // Right hand
            rightHand = reader.Get<CharacterItem>();
            // Left hand
            leftHand = reader.Get<CharacterItem>();
        }
    }
}
