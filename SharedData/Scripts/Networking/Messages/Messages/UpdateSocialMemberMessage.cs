using LiteNetLib.Utils;

namespace MultiplayerARPG
{
    [System.Serializable]
    public partial struct UpdateSocialMemberMessage : INetSerializable
    {
        public enum UpdateType : byte
        {
            Add,
            Update,
            Remove,
            Clear,
        }
        public UpdateType type;
        public int socialId;
        public SocialCharacterData character;

        public void Deserialize(NetDataReader reader)
        {
            type = (UpdateType)reader.GetByte();
            socialId = reader.GetPackedInt();
            // Get social member data
            switch (type)
            {
                case UpdateType.Add:
                case UpdateType.Update:
                    character = reader.Get<SocialCharacterData>();
                    break;
                case UpdateType.Remove:
                    character.id = reader.GetString();
                    break;
            }
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.Put((byte)type);
            writer.PutPackedInt(socialId);
            // Put social member data
            switch (type)
            {
                case UpdateType.Add:
                case UpdateType.Update:
                    writer.Put(character);
                    break;
                case UpdateType.Remove:
                    writer.Put(character.id);
                    break;
            }
        }
    }
}
