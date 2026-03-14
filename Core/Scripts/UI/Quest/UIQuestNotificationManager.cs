using System.Collections.Generic;
using Cysharp.Text;
using LiteNetLibManager;
using UnityEngine;

namespace MultiplayerARPG
{
    public class UIQuestNotificationManager : MonoBehaviour
    {
        private class QuestRecord
        {
            public int dataId;
            public bool isComplete;
            public List<QuestTaskRecord> tasks = new List<QuestTaskRecord>();
        }

        private class QuestTaskRecord
        {
            public int progress;
            public bool isComplete;
        }

        [Tooltip("Format => {0} = {Quest Title}")]
        public UILocaleKeySetting formatKeyQuestAccept = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_QUEST_TITLE_ON_GOING);
        [Tooltip("Format => {0} = {Quest Title}, {1} = {Progress}, {2} = {Target}")]
        public UILocaleKeySetting formatKeyQuestTaskUpdateKillMonster = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_QUEST_TASK_KILL_MONSTER);
        [Tooltip("Format => {0} = {Quest Title}, {1} = {Progress}, {2} = {Target}")]
        public UILocaleKeySetting formatKeyQuestTaskUpdateKillMonsterComplete = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_QUEST_TASK_KILL_MONSTER_COMPLETE);
        [Tooltip("Format => {0} = {Quest Title}, {1} = {Progress}, {2} = {Target}")]
        public UILocaleKeySetting formatKeyQuestTaskUpdateCollectItem = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_QUEST_TASK_COLLECT_ITEM);
        [Tooltip("Format => {0} = {Quest Title}, {1} = {Progress}, {2} = {Target}")]
        public UILocaleKeySetting formatKeyQuestTaskUpdateCollectItemComplete = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_QUEST_TASK_COLLECT_ITEM_COMPLETE);
        [Tooltip("Format => {0} = {Quest Title}, {1} = {Progress}, {2} = {Target}")]
        public UILocaleKeySetting formatKeyQuestTaskUpdateTalkToNpc = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_QUEST_TASK_TALK_TO_NPC);
        [Tooltip("Format => {0} = {Quest Title}, {1} = {Progress}, {2} = {Target}")]
        public UILocaleKeySetting formatKeyQuestTaskUpdateTalkToNpcComplete = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_QUEST_TASK_TALK_TO_NPC_COMPLETE);
        [Tooltip("Format => {0} = {Quest Title}")]
        public UILocaleKeySetting formatKeyQuestComplete = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_QUEST_TITLE_COMPLETE);
        public TextWrapper questAcceptMessagePrefab;
        public TextWrapper questTaskUpdateMessagePrefab;
        public TextWrapper questCompleteMessagePrefab;
        public UIGameMessageHandler messageHandler;
        public float delayBeforeShowingMessages = 1f;

        private List<QuestRecord> comparingQuests = new List<QuestRecord>();
        private float awakenTime = 0f;

        private void Awake()
        {
            awakenTime = Time.unscaledTime;
        }

        private void OnEnable()
        {
            Setup();
        }

        public void Setup()
        {
            MakeComparingQuests();
            GameInstance.PlayingCharacterEntity.onQuestsOperation += OnQuestsOperation;
            GameInstance.PlayingCharacterEntity.onNonEquipItemsOperation += OnNonEquipItemsOperation;
        }

        public void Desetup()
        {
            GameInstance.PlayingCharacterEntity.onQuestsOperation -= OnQuestsOperation;
            GameInstance.PlayingCharacterEntity.onNonEquipItemsOperation -= OnNonEquipItemsOperation;
        }

        private QuestRecord MakeRecord(CharacterQuest characterQuest)
        {
            QuestRecord record = new QuestRecord();
            record.dataId = characterQuest.dataId;
            record.isComplete = characterQuest.isComplete;
            Quest questData = characterQuest.GetQuest();
            QuestTask[] tasks = questData.GetTasks(characterQuest.randomTasksIndex);
            for (int i = 0; i < tasks.Length; ++i)
            {
                bool isComplete;
                int progress = characterQuest.GetProgress(GameInstance.PlayingCharacterEntity, i, out _, out _, out isComplete);
                record.tasks.Add(new QuestTaskRecord()
                {
                    progress = progress,
                    isComplete = isComplete,
                });
            }
            return record;
        }

        private void MakeComparingQuests()
        {
            comparingQuests.Clear();
            foreach (CharacterQuest characterQuest in GameInstance.PlayingCharacterEntity.Quests)
            {
                comparingQuests.Add(MakeRecord(characterQuest));
            }
        }

        private void OnNonEquipItemsOperation(LiteNetLibSyncListOp operation, int index, CharacterItem oldItem, CharacterItem newItem)
        {
            float currentTime = Time.unscaledTime;
            TextWrapper newMessage;
            BasePlayerCharacterEntity character = GameInstance.PlayingCharacterEntity;
            CharacterQuest tempCharacterQuest;
            Quest tempQuestData;
            QuestTask[] tempTasks;
            switch (operation)
            {
                case LiteNetLibSyncListOp.Add:
                case LiteNetLibSyncListOp.Insert:
                case LiteNetLibSyncListOp.Set:
                case LiteNetLibSyncListOp.Dirty:
                    for (int i = 0; i < character.Quests.Count; ++i)
                    {
                        tempCharacterQuest = character.Quests[i];
                        if (tempCharacterQuest.isComplete)
                            continue;
                        tempQuestData = tempCharacterQuest.GetQuest();
                        tempTasks = tempQuestData.GetTasks(tempCharacterQuest.randomTasksIndex);
                        for (int j = 0; j < tempTasks.Length; ++j)
                        {
                            if (tempTasks[j].taskType != QuestTaskType.CollectItem || !tempTasks[j].itemAmount.item)
                                continue;
                            string taskTitle;
                            int maxProgress;
                            bool updatingIsComplete;
                            int updatingProgress = tempCharacterQuest.GetProgress(character, j, out taskTitle, out maxProgress, out updatingIsComplete);

                            bool comparingIsComplete = comparingQuests[i].tasks[j].isComplete;
                            int comparingProgress = comparingQuests[i].tasks[j].progress;

                            if (comparingIsComplete != updatingIsComplete || comparingProgress != updatingProgress)
                            {
                                if (currentTime - awakenTime >= delayBeforeShowingMessages)
                                {
                                    newMessage = messageHandler.AddMessage(questCompleteMessagePrefab);
                                    if (updatingProgress >= maxProgress)
                                        newMessage.text = ZString.Format(LanguageManager.GetText(formatKeyQuestTaskUpdateCollectItemComplete.ToString()), taskTitle, updatingProgress, maxProgress);
                                    else
                                        newMessage.text = ZString.Format(LanguageManager.GetText(formatKeyQuestTaskUpdateCollectItem.ToString()), taskTitle, updatingProgress, maxProgress);
                                }
                                comparingQuests[i].tasks[j].progress = updatingProgress;
                                comparingQuests[i].tasks[j].isComplete = updatingIsComplete;
                            } // End if
                        } // End for
                    }
                    break;
                case LiteNetLibSyncListOp.Clear:
                case LiteNetLibSyncListOp.RemoveAt:
                case LiteNetLibSyncListOp.RemoveFirst:
                case LiteNetLibSyncListOp.RemoveLast:
                    for (int i = 0; i < character.Quests.Count; ++i)
                    {
                        tempCharacterQuest = character.Quests[i];
                        if (tempCharacterQuest.isComplete)
                            continue;
                        tempQuestData = tempCharacterQuest.GetQuest();
                        tempTasks = tempQuestData.GetTasks(tempCharacterQuest.randomTasksIndex);
                        for (int j = 0; j < tempTasks.Length; ++j)
                        {
                            if (tempTasks[j].taskType != QuestTaskType.CollectItem || !tempTasks[j].itemAmount.item)
                                continue;
                            int progress = character.CountNonEquipItems(tempTasks[j].itemAmount.item.DataId);
                            bool isComplete = progress >= tempTasks[j].itemAmount.amount;
                            comparingQuests[i].tasks[j].progress = progress;
                            comparingQuests[i].tasks[j].isComplete = isComplete;
                            break;
                        }
                    }
                    break;
            }
        }

        private void OnQuestsOperation(LiteNetLibSyncListOp operation, int index, CharacterQuest oldItem, CharacterQuest newItem)
        {
            float currentTime = Time.unscaledTime;
            TextWrapper newMessage;
            BasePlayerCharacterEntity character = GameInstance.PlayingCharacterEntity;
            CharacterQuest tempCharacterQuest;
            Quest tempQuestData;
            switch (operation)
            {
                case LiteNetLibSyncListOp.Clear:
                    comparingQuests.Clear();
                    break;
                case LiteNetLibSyncListOp.Add:
                case LiteNetLibSyncListOp.Insert:
                    tempCharacterQuest = character.Quests[index];
                    tempQuestData = tempCharacterQuest.GetQuest();
                    if (currentTime - awakenTime >= delayBeforeShowingMessages)
                    {
                        newMessage = messageHandler.AddMessage(questAcceptMessagePrefab);
                        newMessage.text = ZString.Format(LanguageManager.GetText(formatKeyQuestAccept.ToString()), tempQuestData.Title);
                    }
                    comparingQuests.Add(MakeRecord(tempCharacterQuest));
                    break;
                case LiteNetLibSyncListOp.Set:
                case LiteNetLibSyncListOp.Dirty:
                    tempCharacterQuest = character.Quests[index];
                    tempQuestData = tempCharacterQuest.GetQuest();
                    if (comparingQuests[index].isComplete && !tempCharacterQuest.isComplete)
                    {
                        // **Repeatable quests can be accepted again after completed**
                        if (currentTime - awakenTime >= delayBeforeShowingMessages)
                        {
                            newMessage = messageHandler.AddMessage(questAcceptMessagePrefab);
                            newMessage.text = ZString.Format(LanguageManager.GetText(formatKeyQuestAccept.ToString()), tempQuestData.Title);
                        }
                        comparingQuests[index] = MakeRecord(tempCharacterQuest);
                    }
                    else if (!comparingQuests[index].isComplete && tempCharacterQuest.isComplete)
                    {
                        if (currentTime - awakenTime >= delayBeforeShowingMessages)
                        {
                            newMessage = messageHandler.AddMessage(questCompleteMessagePrefab);
                            newMessage.text = ZString.Format(LanguageManager.GetText(formatKeyQuestComplete.ToString()), tempQuestData.Title);
                        }
                        comparingQuests[index] = MakeRecord(tempCharacterQuest);
                    }
                    else if (!comparingQuests[index].isComplete)
                    {
                        QuestTask[] tasks = tempQuestData.GetTasks(tempCharacterQuest.randomTasksIndex);
                        for (int j = 0; j < tasks.Length; ++j)
                        {
                            string taskTitle;
                            int maxProgress;
                            bool updatingIsComplete;
                            int updatingProgress = tempCharacterQuest.GetProgress(character, j, out taskTitle, out maxProgress, out updatingIsComplete);

                            bool comparingIsComplete = comparingQuests[index].tasks[j].isComplete;
                            int comparingProgress = comparingQuests[index].tasks[j].progress;

                            if (comparingIsComplete != updatingIsComplete || comparingProgress != updatingProgress)
                            {
                                if (currentTime - awakenTime >= delayBeforeShowingMessages)
                                {
                                    switch (tasks[j].taskType)
                                    {
                                        case QuestTaskType.KillMonster:
                                            newMessage = messageHandler.AddMessage(questTaskUpdateMessagePrefab);
                                            if (updatingProgress >= maxProgress)
                                                newMessage.text = ZString.Format(LanguageManager.GetText(formatKeyQuestTaskUpdateKillMonsterComplete.ToString()), taskTitle, updatingProgress, maxProgress);
                                            else
                                                newMessage.text = ZString.Format(LanguageManager.GetText(formatKeyQuestTaskUpdateKillMonster.ToString()), taskTitle, updatingProgress, maxProgress);
                                            break;
                                        case QuestTaskType.TalkToNpc:
                                            newMessage = messageHandler.AddMessage(questTaskUpdateMessagePrefab);
                                            if (updatingProgress >= maxProgress)
                                                newMessage.text = ZString.Format(LanguageManager.GetText(formatKeyQuestTaskUpdateTalkToNpcComplete.ToString()), taskTitle, updatingProgress, maxProgress);
                                            else
                                                newMessage.text = ZString.Format(LanguageManager.GetText(formatKeyQuestTaskUpdateTalkToNpc.ToString()), taskTitle, updatingProgress, maxProgress);
                                            break;
                                        default:
                                            break;
                                    } // End switch
                                }
                                comparingQuests[index].tasks[j].progress = updatingProgress;
                                comparingQuests[index].tasks[j].isComplete = updatingIsComplete;
                            } // End if
                        } // End for
                    }
                    break;
                case LiteNetLibSyncListOp.RemoveAt:
                    comparingQuests.RemoveAt(index);
                    break;
                case LiteNetLibSyncListOp.RemoveFirst:
                    comparingQuests.RemoveAt(0);
                    break;
                case LiteNetLibSyncListOp.RemoveLast:
                    comparingQuests.RemoveAt(comparingQuests.Count - 1);
                    break;
            }
        }
    }
}
