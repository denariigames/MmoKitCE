using Insthync.UnityEditorUtils;
using UnityEngine;
using UnityEngine.Serialization;

namespace MultiplayerARPG
{
    [System.Serializable]
    public struct QuestTask
    {
        public QuestTaskType taskType;

        [StringShowConditional(nameof(taskType), nameof(QuestTaskType.KillMonster))]
        public MonsterCharacterAmount monsterCharacterAmount;

        [StringShowConditional(nameof(taskType), nameof(QuestTaskType.CollectItem))]
        public ItemAmount itemAmount;
        [StringShowConditional(nameof(taskType), nameof(QuestTaskType.CollectItem))]
        [Tooltip("If this is `TRUE`, it will not decrease task items when quest completed")]
        public bool doNotDecreaseItemsOnQuestComplete;

#if UNITY_EDITOR
        [StringShowConditional(nameof(taskType), nameof(QuestTaskType.TalkToNpc))]
        [NotPatchable]
        [Tooltip("Have to talk to this NPC to complete task")]
        public NpcEntity npcEntity;
#endif
        [StringShowConditional(nameof(taskType), nameof(QuestTaskType.TalkToNpc))]
        [ReadOnlyField]
        public int npcEntityId;
        [StringShowConditional(nameof(taskType), nameof(QuestTaskType.TalkToNpc))]
        [ReadOnlyField]
        public string npcEntityTitle;
        [StringShowConditional(nameof(taskType), nameof(QuestTaskType.TalkToNpc))]
        [ReadOnlyField]
        public LanguageData[] npcEntityTitles;

        [StringShowConditional(nameof(taskType), nameof(QuestTaskType.TalkToNpc))]
        [Tooltip("This dialog will be shown immediately instead of start dialog which set to the NPC")]
        public BaseNpcDialog talkToNpcDialog;
        [StringShowConditional(nameof(taskType), nameof(QuestTaskType.TalkToNpc))]
        [Tooltip("If this is `TRUE` quest will be completed immediately after talked to NPC and all tasks done")]
        public bool completeAfterTalked;

        [StringShowConditional(nameof(taskType), nameof(QuestTaskType.Custom))]
        public BaseCustomQuestTask customQuestTask;

        [Header("Custom Task Description")]
        public bool useCustomDescription;
        [FormerlySerializedAs("defaultDescriptionOverride")]
        public string defaultDescription;
        [FormerlySerializedAs("languageSpecificDescriptionOverrides")]
        public LanguageData[] languageSpecificDescriptions;

        [Header("Custom Task Completed Description")]
        public bool useCustomCompletedDescription;
        [FormerlySerializedAs("defaultCompletedDescriptionOverride")]
        public string defaultCompletedDescription;
        [FormerlySerializedAs("languageSpecificCompletedDescriptionOverrides")]
        public LanguageData[] languageSpecificCompletedDescriptions;

        public string CustomDescription
        {
            get { return Language.GetText(languageSpecificDescriptions, defaultDescription); }
        }

        public string CustomCompletedDescription
        {
            get { return Language.GetText(languageSpecificCompletedDescriptions, defaultCompletedDescription); }
        }

        public string NpcEntityTitle
        {
            get { return Language.GetText(npcEntityTitles, npcEntityTitle); }
        }
    }
}