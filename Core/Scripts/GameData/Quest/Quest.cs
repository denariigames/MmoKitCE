using Insthync.UnityEditorUtils;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace MultiplayerARPG
{
    [CreateAssetMenu(fileName = GameDataMenuConsts.QUEST_FILE, menuName = GameDataMenuConsts.QUEST_MENU, order = GameDataMenuConsts.QUEST_ORDER)]
    public partial class Quest : BaseGameData
    {
        public static readonly QuestTask[] EmptyTasks = System.Array.Empty<QuestTask>();

        [Category("Quest Settings")]
        [Tooltip("Requirement to receive quest")]
        public QuestRequirement requirement = new QuestRequirement();
        // TODO: Deprecating, use random tasks
        [HideInInspector]
        [SerializeField]
        private QuestTask[] tasks = new QuestTask[0];
        public QuestTasks[] randomTasks = new QuestTasks[0];
        [Tooltip("Quests which will be abandoned when accept this quest")]
        public Quest[] abandonQuests = new Quest[0];
        [Header("Rewarding")]
        public bool resetAttributes = false;
        public bool resetSkills = false;
        public PlayerCharacter changeCharacterClass = null;
        public Faction changeCharacterFaction = null;
        public int rewardExp = 0;
        public int rewardGold = 0;
        public int rewardStatPoints = 0;
        public int rewardSkillPoints = 0;
        [ArrayElementTitle("currency")]
        public CurrencyAmount[] rewardCurrencies = new CurrencyAmount[0];
        [ArrayElementTitle("item")]
        public ItemAmount[] rewardItems = new ItemAmount[0];
        [ArrayElementTitle("item")]
        public ItemAmount[] selectableRewardItems = new ItemAmount[0];
        [ArrayElementTitle("item")]
        public ItemRandomByWeight[] randomRewardItems = new ItemRandomByWeight[0];
        [FormerlySerializedAs("canRepeat")]
        public QuestRepeatType repeatType = QuestRepeatType.None;
        public bool canAbandon = true;
        [FormerlySerializedAs("nextAssignQuest")]
        public Quest[] nextAssignQuests = new Quest[0];
        [FormerlySerializedAs("autoTrack")]
        public bool autoTrackQuest = false;

        [System.NonSerialized]
        private HashSet<int> _cacheKillMonsterIds;
        public HashSet<int> CacheKillMonsterIds
        {
            get
            {
                if (_cacheKillMonsterIds == null)
                {
                    _cacheKillMonsterIds = new HashSet<int>();
                    if (randomTasks != null && randomTasks.Length > 0)
                    {
                        foreach (QuestTasks tasks in randomTasks)
                        {
                            if (tasks.tasks == null || tasks.tasks.Length == 0)
                                continue;

                            foreach (QuestTask task in tasks.tasks)
                            {
                                if (task.taskType == QuestTaskType.KillMonster &&
                                    task.monsterCharacterAmount.monster != null &&
                                    task.monsterCharacterAmount.amount > 0)
                                {
                                    _cacheKillMonsterIds.Add(task.monsterCharacterAmount.monster.DataId);
                                }
                            }
                        }
                    }
                }
                return _cacheKillMonsterIds;
            }
        }

        [System.NonSerialized]
        private Dictionary<Currency, int> _cacheRewardCurrencies = null;
        public Dictionary<Currency, int> CacheRewardCurrencies
        {
            get
            {
                if (_cacheRewardCurrencies == null)
                {
                    _cacheRewardCurrencies = new Dictionary<Currency, int>();
                    GameDataHelpers.CombineCurrencies(rewardCurrencies, _cacheRewardCurrencies, 1f);
                }
                return _cacheRewardCurrencies;
            }
        }

        public QuestTask[] GetTasks(int index)
        {
            if (randomTasks == null || randomTasks.Length == 0)
                return EmptyTasks;
            QuestTask[] result;
            if (index < 0 || index >= randomTasks.Length)
                result = randomTasks[0].tasks;
            else
                result = randomTasks[index].tasks;
            if (result == null)
                result = EmptyTasks;
            return result;
        }

        public override bool Validate()
        {
            bool hasChanges = false;
            for (int i = 0; i < rewardCurrencies.Length; ++i)
            {
                if (rewardCurrencies[i].amount <= 0)
                {
                    Debug.LogWarning("[Quest] Reward Currencies [" + i + "], amount is " + rewardCurrencies[i].amount + " will be changed to 1 (Minimum Value)");
                    hasChanges = true;
                    CurrencyAmount reward = rewardCurrencies[i];
                    reward.amount = 1;
                    rewardCurrencies[i] = reward;
                }
            }
            for (int i = 0; i < rewardItems.Length; ++i)
            {
                if (rewardItems[i].amount <= 0)
                {
                    Debug.LogWarning("[Quest] Reward Items [" + i + "], amount is " + rewardItems[i].amount + " will be changed to 1 (Minimum Value)");
                    hasChanges = true;
                    ItemAmount reward = rewardItems[i];
                    reward.amount = 1;
                    rewardItems[i] = reward;
                }
            }
            for (int i = 0; i < selectableRewardItems.Length; ++i)
            {
                if (selectableRewardItems[i].amount <= 0)
                {
                    Debug.LogWarning("[Quest] Selectable Reward Items [" + i + "], amount is " + selectableRewardItems[i].amount + " will be changed to 1 (Minimum Value)");
                    hasChanges = true;
                    ItemAmount reward = selectableRewardItems[i];
                    reward.amount = 1;
                    selectableRewardItems[i] = reward;
                }
            }
            for (int i = 0; i < randomRewardItems.Length; ++i)
            {
                if (randomRewardItems[i].maxAmount <= 0)
                {
                    Debug.LogWarning("[Quest] Random Reward Items [" + i + "], max amount is " + randomRewardItems[i].maxAmount + " will be changed to 1 (Minimum Value)");
                    hasChanges = true;
                    ItemRandomByWeight reward = randomRewardItems[i];
                    reward.maxAmount = 1;
                    randomRewardItems[i] = reward;
                }
            }
            if (tasks != null && tasks.Length > 0 && (randomTasks == null || randomTasks.Length <= 0))
            {
                List<QuestTasks> tempQuestTasks = new List<QuestTasks>() {
                    new QuestTasks()
                    {
                        tasks = tasks,
                    }
                };
                randomTasks = tempQuestTasks.ToArray();
                tasks = null;
                hasChanges = true;
            }
#if UNITY_EDITOR
            if (randomTasks != null && randomTasks.Length > 0)
            {
                for (int i = 0; i < randomTasks.Length; ++i)
                {
                    bool hasTaskChanges = false;
                    QuestTasks randomTask = randomTasks[i];
                    if (randomTask.tasks == null || randomTask.tasks.Length <= 0)
                        continue;
                    for (int j = 0; j < randomTask.tasks.Length; ++j)
                    {
                        QuestTask task = randomTask.tasks[j];
                        NpcEntity npcEntity = task.npcEntity;
                        if (npcEntity == null)
                            continue;
                        if (task.npcEntityId != npcEntity.EntityId)
                            task.npcEntityId = npcEntity.EntityId;
                        task.npcEntityTitle = npcEntity.EntityTitleSetting;
                        task.npcEntityTitles = npcEntity.EntityTitlesSetting;
                        hasTaskChanges = true;
                        randomTask.tasks[j] = task;
                    }
                    if (hasTaskChanges)
                    {
                        hasChanges = true;
                        randomTasks[i] = randomTask;
                    }
                }
            }
#endif
            return hasChanges || base.Validate();
        }

        public override void PrepareRelatesData()
        {
            base.PrepareRelatesData();
            if (randomTasks != null && randomTasks.Length > 0)
            {
                foreach (QuestTasks tasks in randomTasks)
                {
                    if (tasks.tasks == null || tasks.tasks.Length == 0)
                        continue;

                    foreach (QuestTask task in tasks.tasks)
                    {
                        GameInstance.AddCharacters(task.monsterCharacterAmount.monster);
                        GameInstance.AddItems(task.itemAmount.item);
                        GameInstance.AddNpcDialogs(task.talkToNpcDialog);
                    }
                }
            }
            GameInstance.AddQuests(abandonQuests);
            GameInstance.AddCharacters(changeCharacterClass);
            GameInstance.AddFactions(changeCharacterFaction);
            GameInstance.AddCurrencies(rewardCurrencies);
            GameInstance.AddItems(rewardItems);
            GameInstance.AddItems(selectableRewardItems);
            GameInstance.AddItems(randomRewardItems);
            GameInstance.AddQuests(requirement.completedQuests);
        }

        public bool HaveToTalkToNpc(IPlayerCharacterData character, NpcEntity npcEntity, int randomTasksIndex, out int taskIndex, out BaseNpcDialog dialog, out bool completeAfterTalked)
        {
            QuestTask[] tasks = GetTasks(randomTasksIndex);
            taskIndex = -1;
            dialog = null;
            completeAfterTalked = false;
            if (tasks == null || tasks.Length == 0)
                return false;
            int indexOfQuest = character.IndexOfQuest(DataId);
            if (indexOfQuest < 0 || character.Quests[indexOfQuest].isComplete)
                return false;
            for (int i = 0; i < tasks.Length; ++i)
            {
                if (tasks[i].taskType != QuestTaskType.TalkToNpc)
                    continue;
                if (tasks[i].npcEntityId == npcEntity.EntityId)
                {
                    taskIndex = i;
                    dialog = tasks[i].talkToNpcDialog;
                    completeAfterTalked = tasks[i].completeAfterTalked;
                    return true;
                }
            }
            return false;
        }

        public bool CanReceiveQuest(IPlayerCharacterData character)
        {
            // Quest is completed, so don't show the menu which navigate to this dialog
            int indexOfQuest = character.IndexOfQuest(DataId);
            if (indexOfQuest >= 0 && character.Quests[indexOfQuest].isComplete)
            {
                long completeTime = character.Quests[indexOfQuest].completeTime;

                System.DateTime completeDateTime = GenericUtils.GetDateTimeBySeconds(completeTime).ToLocalTime();
                System.DateTime todayDateTime = GenericUtils.GetDateTimeBySeconds(System.DateTimeOffset.UtcNow.ToUnixTimeSeconds()).ToLocalTime();

                switch (repeatType)
                {
                    case QuestRepeatType.AnyTime:
                        break;
                    case QuestRepeatType.Daily:
                        if ((todayDateTime.Date - completeDateTime.Date).TotalDays <= 0)
                            return false;
                        break;
                    case QuestRepeatType.Weekly:
                        if ((todayDateTime.StartOfWeek(System.DayOfWeek.Sunday) - completeDateTime.StartOfWeek(System.DayOfWeek.Sunday)).TotalDays <= 0)
                            return false;
                        break;
                    default:
                        return false;
                }
            }
            // Character's level is lower than requirement
            if (character.Level < requirement.level)
                return false;
            // Character's has difference class
            if (requirement.character != null && requirement.character.DataId != character.DataId)
                return false;
            // Character's has difference faction
            if (requirement.faction != null && requirement.faction.DataId != character.FactionId)
                return false;
            // Character's not complete all required quests
            if (requirement.completedQuests != null && requirement.completedQuests.Length > 0)
            {
                foreach (Quest quest in requirement.completedQuests)
                {
                    indexOfQuest = character.IndexOfQuest(quest.DataId);
                    if (indexOfQuest < 0)
                        return false;
                    if (!character.Quests[indexOfQuest].isComplete)
                        return false;
                }
            }
            return true;
        }
    }
}