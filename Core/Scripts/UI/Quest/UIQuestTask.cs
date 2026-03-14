using Cysharp.Text;
using UnityEngine;

namespace MultiplayerARPG
{
    public partial class UIQuestTask : UISelectionEntry<UIQuestTaskData>
    {
        public QuestTask QuestTask { get { return Data.questTask; } }
        public int Progress { get { return Data.progress; } }

        [Header("String Formats")]
        [Tooltip("Format => {0} = {Title}, {1} = {Progress}, {2} = {Amount}")]
        public UILocaleKeySetting formatKeyTaskKillMonster = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_QUEST_TASK_KILL_MONSTER);
        [Tooltip("Format => {0} = {Title}, {1} = {Progress}, {2} = {Amount}")]
        public UILocaleKeySetting formatKeyTaskKillMonsterComplete = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_QUEST_TASK_KILL_MONSTER_COMPLETE);
        [Tooltip("Format => {0} = {Title}, {1} = {Progress}, {2} = {Amount}")]
        public UILocaleKeySetting formatKeyTaskCollectItem = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_QUEST_TASK_COLLECT_ITEM);
        [Tooltip("Format => {0} = {Title}, {1} = {Progress}, {2} = {Amount}")]
        public UILocaleKeySetting formatKeyTaskCollectItemComplete = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_QUEST_TASK_COLLECT_ITEM_COMPLETE);
        [Tooltip("Format => {0} = {Title}")]
        public UILocaleKeySetting formatKeyTaskTalkToNpc = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_QUEST_TASK_TALK_TO_NPC);
        [Tooltip("Format => {0} = {Title}")]
        public UILocaleKeySetting formatKeyTaskTalkToNpcComplete = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_QUEST_TASK_TALK_TO_NPC_COMPLETE);

        [Header("UI Elements")]
        public TextWrapper uiTextTaskDescription;

        protected override void OnDestroy()
        {
            base.OnDestroy();
            uiTextTaskDescription = null;
        }

        protected override void UpdateData()
        {
            bool isComplete;
            switch (QuestTask.taskType)
            {
                case QuestTaskType.KillMonster:
                    MonsterCharacterAmount monsterCharacterAmount = QuestTask.monsterCharacterAmount;
                    string monsterTitle = monsterCharacterAmount.monster == null ? LanguageManager.GetUnknowTitle() : monsterCharacterAmount.monster.Title;
                    int monsterKillAmount = monsterCharacterAmount.amount;
                    isComplete = Progress >= monsterKillAmount;
                    if (uiTextTaskDescription != null)
                    {
                        uiTextTaskDescription.text = ZString.Format(
                            isComplete ?
                                LanguageManager.GetText(formatKeyTaskKillMonsterComplete) :
                                LanguageManager.GetText(formatKeyTaskKillMonster), monsterTitle,
                            Progress.ToString("N0"),
                            monsterKillAmount.ToString("N0"));
                        UpdateCustomDescription(isComplete, monsterTitle, Progress, monsterKillAmount);
                    }
                    break;
                case QuestTaskType.CollectItem:
                    ItemAmount itemAmount = QuestTask.itemAmount;
                    string itemTitle = itemAmount.item == null ? LanguageManager.GetUnknowTitle() : itemAmount.item.Title;
                    int itemCollectAmount = itemAmount.amount;
                    isComplete = Progress >= itemCollectAmount;
                    if (uiTextTaskDescription != null)
                    {
                        uiTextTaskDescription.text = ZString.Format(
                            isComplete ?
                                LanguageManager.GetText(formatKeyTaskCollectItemComplete) :
                                LanguageManager.GetText(formatKeyTaskCollectItem), itemTitle,
                            Progress.ToString("N0"),
                            itemCollectAmount.ToString("N0"));
                        UpdateCustomDescription(isComplete, itemTitle, Progress, itemCollectAmount);
                    }
                    break;
                case QuestTaskType.TalkToNpc:
                    string npcTitle = QuestTask.npcEntityTitle;
                    isComplete = Progress > 0;
                    if (QuestTask.completeAfterTalked)
                        isComplete = false;
                    if (uiTextTaskDescription != null)
                    {
                        uiTextTaskDescription.text = ZString.Format(
                            isComplete ?
                                LanguageManager.GetText(formatKeyTaskTalkToNpcComplete) :
                                LanguageManager.GetText(formatKeyTaskTalkToNpc), npcTitle);
                        UpdateCustomDescription(isComplete, npcTitle, Progress, 1);
                    }
                    break;
                case QuestTaskType.Custom:
                    if (uiTextTaskDescription)
                        uiTextTaskDescription.text = QuestTask.customQuestTask.GetTaskDescription(GameInstance.PlayingCharacter, Progress);
                    break;
            }
        }

        public void UpdateCustomDescription(bool isComplete, string objectTitle, int progress, int target)
        {
            if (uiTextTaskDescription == null)
                return;

            if (isComplete)
            {
                if (QuestTask.useCustomCompletedDescription)
                    uiTextTaskDescription.text = ZString.Format(QuestTask.CustomCompletedDescription, objectTitle, progress.ToString("N0"), target.ToString("N0"));
            }
            else
            {
                if (QuestTask.useCustomDescription)
                    uiTextTaskDescription.text = ZString.Format(QuestTask.CustomDescription, objectTitle, progress.ToString("N0"), target.ToString("N0"));
            }
        }
    }
}