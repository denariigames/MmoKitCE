using LiteNetLibManager;

namespace MultiplayerARPG
{
    public partial struct CharacterAttribute
    {
        public Attribute GetAttribute()
        {
            if (GameInstance.Attributes.TryGetValue(dataId, out Attribute result))
                return result;
            return null;
        }

        public static CharacterAttribute Create(Attribute attribute, int amount = 0)
        {
            return Create(attribute.DataId, amount);
        }
    }

    [System.Serializable]
    public class SyncListCharacterAttribute : LiteNetLibSyncList<CharacterAttribute>
    {
    }
}
