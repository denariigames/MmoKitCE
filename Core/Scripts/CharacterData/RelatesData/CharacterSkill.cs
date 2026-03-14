using LiteNetLibManager;

namespace MultiplayerARPG
{
    public partial struct CharacterSkill
    {
        public BaseSkill GetSkill()
        {
            if (GameInstance.Skills.TryGetValue(dataId, out BaseSkill result))
                return result;
            return null;
        }

        public static CharacterSkill Create(BaseSkill skill, int level = 1)
        {
            return Create(skill.DataId, level);
        }
    }

    [System.Serializable]
    public class SyncListCharacterSkill : LiteNetLibSyncList<CharacterSkill>
    {
    }
}
