using LiteNetLib.Utils;
using System.Collections.Generic;

namespace MultiplayerARPG
{
    [System.Serializable]
    public partial struct UpdateSocialMembersMessage : INetSerializable
    {
        public int id;
        public List<SocialCharacterData> members;

        public void Deserialize(NetDataReader reader)
        {
            id = reader.GetPackedInt();
            members = reader.GetList<SocialCharacterData>();
        }

        public void Serialize(NetDataWriter writer)
        {
            writer.PutPackedInt(id);
            writer.PutList(members);
        }
    }
}
