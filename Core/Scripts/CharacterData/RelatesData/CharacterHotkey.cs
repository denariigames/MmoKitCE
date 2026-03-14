using LiteNetLibManager;

namespace MultiplayerARPG
{
    public partial struct CharacterHotkey
    {
        public static CharacterHotkey Create(string hotkeyId, CharacterItem characterItem)
        {
            // Usable items will use item data id
            string relateId = characterItem.GetItem().Id;
            // For an equipments, it will use item unique id
            if (characterItem.GetEquipmentItem() != null)
                relateId = characterItem.id;
            return new CharacterHotkey()
            {
                hotkeyId = hotkeyId,
                type = HotkeyType.Item,
                relateId = relateId,
            };
        }

        public static CharacterHotkey Create(string hotkeyId, CharacterSkill characterSkill)
        {
            // Use skil data id
            string relateId = characterSkill.GetSkill().Id;
            return new CharacterHotkey()
            {
                hotkeyId = hotkeyId,
                type = HotkeyType.Skill,
                relateId = relateId,
            };
        }
    }

    [System.Serializable]
    public class SyncListCharacterHotkey : LiteNetLibSyncList<CharacterHotkey>
    {
    }
}
