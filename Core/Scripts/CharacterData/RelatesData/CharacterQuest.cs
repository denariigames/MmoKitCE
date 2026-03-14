using LiteNetLibManager;

namespace MultiplayerARPG
{
    public partial struct CharacterQuest
    {
        public Quest GetQuest()
        {
            if (GameInstance.Quests.TryGetValue(dataId, out Quest result))
                return result;
            return null;
        }

        public bool IsAllTasksDone(IPlayerCharacterData character, out bool hasCompleteAfterTalkedTask)
        {
            hasCompleteAfterTalkedTask = false;
            Quest quest = GetQuest();
            if (character == null || quest == null)
                return false;
            QuestTask[] tasks = quest.GetTasks(randomTasksIndex);
            for (int i = 0; i < tasks.Length; ++i)
            {
                GetProgress(character, i, out bool isComplete);
                if (!isComplete)
                    return false;
                if (tasks[i].taskType == QuestTaskType.TalkToNpc && tasks[i].completeAfterTalked)
                    hasCompleteAfterTalkedTask = true;
            }
            return true;
        }

        public bool IsAllTasksDoneAndIsCompletingTarget(IPlayerCharacterData character, NpcEntity npcEntity)
        {
            Quest quest = GetQuest();
            if (character == null || quest == null)
                return false;
            QuestTask[] tasks = quest.GetTasks(randomTasksIndex);
            for (int i = 0; i < tasks.Length; ++i)
            {
                GetProgress(character, i, out bool isComplete);
                if (!isComplete)
                {
                    return false;
                }
                if (tasks[i].taskType == QuestTaskType.TalkToNpc && tasks[i].completeAfterTalked &&
                    (npcEntity == null || tasks[i].npcEntityId != npcEntity.EntityId))
                {
                    return false;
                }
            }
            return true;
        }

        public int GetProgress(IPlayerCharacterData character, int taskIndex, out bool isComplete)
        {
            return GetProgress(character, taskIndex, out _, out _, out isComplete);
        }

        public int GetProgress(IPlayerCharacterData character, int taskIndex, out string targetTitle, out int targetProgress, out bool isComplete)
        {
            Quest quest = GetQuest();
            QuestTask[] tasks = quest.GetTasks(randomTasksIndex);
            if (character == null || quest == null || taskIndex < 0 || taskIndex >= tasks.Length)
            {
                targetTitle = string.Empty;
                targetProgress = 0;
                isComplete = false;
                return 0;
            }
            QuestTask task = tasks[taskIndex];
            int progress;
            switch (task.taskType)
            {
                case QuestTaskType.KillMonster:
                    targetTitle = task.monsterCharacterAmount.monster == null ? string.Empty : task.monsterCharacterAmount.monster.Title;
                    progress = task.monsterCharacterAmount.monster == null ? 0 : CountKillMonster(task.monsterCharacterAmount.monster.DataId);
                    targetProgress = task.monsterCharacterAmount.amount;
                    isComplete = progress >= targetProgress;
                    return progress;
                case QuestTaskType.CollectItem:
                    targetTitle = task.itemAmount.item == null ? string.Empty : task.itemAmount.item.Title;
                    progress = task.itemAmount.item == null ? 0 : character.CountNonEquipItems(task.itemAmount.item.DataId);
                    targetProgress = task.itemAmount.amount;
                    isComplete = progress >= targetProgress;
                    return progress;
                case QuestTaskType.TalkToNpc:
                    targetTitle = task.NpcEntityTitle;
                    if (task.completeAfterTalked)
                        progress = 1;
                    else
                        progress = completedTasks.Contains(taskIndex) ? 1 : 0;
                    targetProgress = 1;
                    isComplete = progress >= targetProgress;
                    return progress;
                case QuestTaskType.Custom:
                    return task.customQuestTask.GetTaskProgress(character, quest, taskIndex, out targetTitle, out targetProgress, out isComplete);
            }
            targetTitle = string.Empty;
            targetProgress = 0;
            isComplete = false;
            return 0;
        }

        public bool AddKillMonster(BaseMonsterCharacterEntity monsterEntity, int killCount)
        {
            return AddKillMonster(monsterEntity.DataId, killCount);
        }

        public bool AddKillMonster(int monsterDataId, int killCount)
        {
            Quest quest = GetQuest();
            if (quest == null || !quest.CacheKillMonsterIds.Contains(monsterDataId))
                return false;
            if (!killedMonsters.ContainsKey(monsterDataId))
                killedMonsters.Add(monsterDataId, 0);
            killedMonsters[monsterDataId] += killCount;
            return true;
        }

        public int CountKillMonster(int monsterDataId)
        {
            if (!killedMonsters.ContainsKey(monsterDataId))
                return 0;
            return killedMonsters[monsterDataId];
        }

        public static CharacterQuest Create(Quest quest)
        {
            return Create(quest.DataId, (byte)GenericUtils.RandomInt(0, quest.randomTasks.Length));
        }
    }

    [System.Serializable]
    public class SyncListCharacterQuest : LiteNetLibSyncList<CharacterQuest>
    {
    }
}
