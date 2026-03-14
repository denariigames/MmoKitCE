using Cysharp.Threading.Tasks;
using Insthync.UnityEditorUtils;
using UnityEngine.Serialization;

namespace MultiplayerARPG
{
    [System.Serializable]
    public struct NpcDialogCondition
    {
        public NpcDialogConditionType conditionType;
        [StringShowConditional(nameof(conditionType), new string[] { nameof(NpcDialogConditionType.PlayerCharacterIs) })]
        public PlayerCharacter playerCharacter;
        [StringShowConditional(nameof(conditionType), new string[] { nameof(NpcDialogConditionType.FactionIs) })]
        public Faction faction;
        [StringShowConditional(nameof(conditionType), new string[] { nameof(NpcDialogConditionType.QuestNotStarted), nameof(NpcDialogConditionType.QuestOngoing), nameof(NpcDialogConditionType.QuestTasksCompleted), nameof(NpcDialogConditionType.QuestCompleted) })]
        public Quest quest;
        [StringShowConditional(nameof(conditionType), new string[] { nameof(NpcDialogConditionType.LevelMoreThanOrEqual), nameof(NpcDialogConditionType.LevelLessThanOrEqual) })]
        [FormerlySerializedAs("conditionalLevel")]
        public int level;
        [StringShowConditional(nameof(conditionType), new string[] { nameof(NpcDialogConditionType.CustomByScriptableObject) })]
        public BaseCustomNpcDialogCondition customConditionScriptableObject;
        [NpcDialogConditionData]
        [FormerlySerializedAs("conditionData")]
        public NpcDialogConditionData customConditionCallback;

        public async UniTask<bool> IsPass(IPlayerCharacterData character)
        {
            int indexOfQuest = -1;
            bool questTasksCompleted = false;
            bool questCompleted = false;
            if (quest != null)
            {
                indexOfQuest = character.IndexOfQuest(quest.DataId);
                if (indexOfQuest >= 0)
                {
                    CharacterQuest characterQuest = character.Quests[indexOfQuest];
                    questTasksCompleted = characterQuest.IsAllTasksDone(character, out _);
                    questCompleted = characterQuest.isComplete;
                }
            }
            switch (conditionType)
            {
                case NpcDialogConditionType.LevelMoreThanOrEqual:
                    return character.Level >= level;
                case NpcDialogConditionType.LevelLessThanOrEqual:
                    return character.Level <= level;
                case NpcDialogConditionType.QuestNotStarted:
                    return indexOfQuest < 0;
                case NpcDialogConditionType.QuestOngoing:
                    return indexOfQuest >= 0 && !questCompleted;
                case NpcDialogConditionType.QuestTasksCompleted:
                    return indexOfQuest >= 0 && questTasksCompleted && !questCompleted;
                case NpcDialogConditionType.QuestCompleted:
                    return indexOfQuest >= 0 && questCompleted;
                case NpcDialogConditionType.FactionIs:
                    return character.FactionId == faction.DataId;
                case NpcDialogConditionType.PlayerCharacterIs:
                    return character.DataId == playerCharacter.DataId;
                case NpcDialogConditionType.CustomByScriptableObject:
                    return await customConditionScriptableObject.IsPass(character);
                case NpcDialogConditionType.CustomByCallback:
                    return customConditionCallback.Invoke(character.Id);
            }
            return true;
        }
    }
}
