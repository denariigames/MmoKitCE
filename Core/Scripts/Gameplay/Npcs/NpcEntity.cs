using Cysharp.Threading.Tasks;
using Insthync.AddressableAssetTools;
using Insthync.UnityEditorUtils;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace MultiplayerARPG
{
    public partial class NpcEntity : BaseGameEntity, IActivatableEntity
    {
        [Category(5, "NPC Settings")]
        [SerializeField]
        [Tooltip("Set it more than `0` to make it uses this value instead of `GameInstance` -> `conversationDistance` as its activatable distance")]
        private float activatableDistance = 0f;

        [SerializeField]
        [Tooltip("It will use `startDialog` if `graph` is empty")]
        private BaseNpcDialog startDialog;
        public BaseNpcDialog StartDialog
        {
            get
            {
                if (graph != null && graph.nodes != null && graph.nodes.Count > 0)
                    return graph.nodes[0] as BaseNpcDialog;
                return startDialog;
            }
            set
            {
                startDialog = value;
            }
        }

        [SerializeField]
        [Tooltip("It will use `graph` start dialog if this is not empty")]
        private NpcDialogGraph graph;
        public NpcDialogGraph Graph
        {
            get
            {
                return graph;
            }
            set
            {
                graph = value;
            }
        }

        [Category("Relative GameObjects/Transforms")]
        [SerializeField]
        [FormerlySerializedAs("uiElementTransform")]
        private Transform characterUiTransform = null;

        [SerializeField]
        [FormerlySerializedAs("miniMapElementContainer")]
        private Transform miniMapUiTransform = null;

        [SerializeField]
        private Transform questIndicatorContainer = null;

        private UINpcEntity _uiNpcEntity;
        private NpcQuestIndicator _questIndicator;

        public Transform CharacterUiTransform
        {
            get
            {
                if (characterUiTransform == null)
                    characterUiTransform = EntityTransform;
                return characterUiTransform;
            }
        }

        public Transform MiniMapUiTransform
        {
            get
            {
                if (miniMapUiTransform == null)
                    miniMapUiTransform = EntityTransform;
                return miniMapUiTransform;
            }
        }

        public Transform QuestIndicatorContainer
        {
            get
            {
                if (questIndicatorContainer == null)
                    questIndicatorContainer = EntityTransform;
                return questIndicatorContainer;
            }
        }

        public override void PrepareRelatesData()
        {
            base.PrepareRelatesData();
            if (startDialog != null)
                GameInstance.AddNpcDialogs(startDialog);
            if (graph != null)
                GameInstance.AddNpcDialogs(graph.GetDialogs());
        }

        protected override void EntityAwake()
        {
            base.EntityAwake();
            gameObject.tag = CurrentGameInstance.npcTag;
            gameObject.layer = CurrentGameInstance.npcLayer;
        }

        protected override void EntityStart()
        {
            base.EntityStart();
            if (startDialog != null)
                GameInstance.AddNpcDialogs(startDialog);
            if (graph != null)
                GameInstance.AddNpcDialogs(graph.GetDialogs());
        }

        public override void OnSetup()
        {
            base.OnSetup();
            InstantiateNpcObjects();
        }

        private async void InstantiateNpcObjects()
        {
            if (!IsClient)
                return;
#if !DISABLE_ADDRESSABLES
            // Instantiates npc objects
            await CurrentGameInstance.AddressableNpcObjects.InstantiateObjectsOrUsePrefabs(CurrentGameInstance.NpcObjects, EntityTransform);
#else
            foreach (var prefab in CurrentGameInstance.NpcObjects)
            {
                if (prefab == null) continue;
                Instantiate(prefab, EntityTransform.position, EntityTransform.rotation, EntityTransform);
            }
#endif
#if !DISABLE_ADDRESSABLES
            // Instantiates npc minimap objects
            await CurrentGameInstance.AddressableNpcMiniMapObjects.InstantiateObjectsOrUsePrefabs(CurrentGameInstance.NpcMiniMapObjects, EntityTransform);
#else
            foreach (var prefab in CurrentGameInstance.NpcMiniMapObjects)
            {
                if (prefab == null) continue;
                Instantiate(prefab, EntityTransform.position, EntityTransform.rotation, EntityTransform);
            }
#endif
            // Instantiates npc UI
            InstantiateUI(await CurrentGameInstance.GetLoadedNpcUIPrefab());
            // Instantiates npc quest indicator
            InstantiateQuestIndicator(await CurrentGameInstance.GetLoadedNpcQuestIndicatorPrefab());
        }

        public void InstantiateUI(UINpcEntity prefab)
        {
            if (prefab == null)
                return;
            if (_uiNpcEntity != null)
                Destroy(_uiNpcEntity.gameObject);
            _uiNpcEntity = Instantiate(prefab, CharacterUiTransform);
            _uiNpcEntity.transform.localPosition = Vector3.zero;
            _uiNpcEntity.Data = this;
        }

        public void InstantiateQuestIndicator(NpcQuestIndicator prefab)
        {
            if (prefab == null)
                return;
            if (_questIndicator != null)
                Destroy(_questIndicator.gameObject);
            _questIndicator = Instantiate(prefab, QuestIndicatorContainer);
            _questIndicator.npcEntity = this;
        }

        private async UniTask FindQuestFromDialog(IPlayerCharacterData playerCharacter, HashSet<int> questIds, BaseNpcDialog baseDialog, List<BaseNpcDialog> foundDialogs = null)
        {
            if (foundDialogs == null)
                foundDialogs = new List<BaseNpcDialog>();

            if (baseDialog == null)
                return;

            NpcDialog dialog = baseDialog as NpcDialog;
            if (dialog == null || foundDialogs.Contains(dialog))
                return;

            foundDialogs.Add(dialog);

            switch (dialog.type)
            {
                case NpcDialogType.Normal:
                    foreach (NpcDialogMenu menu in dialog.menus)
                    {
                        if (menu.isCloseMenu || !await menu.IsPassConditions(playerCharacter)) continue;
                        await FindQuestFromDialog(playerCharacter, questIds, menu.dialog, foundDialogs);
                    }
                    break;
                case NpcDialogType.Quest:
                    if (dialog.quest != null)
                        questIds.Add(dialog.quest.DataId);
                    await FindQuestFromDialog(playerCharacter, questIds, dialog.questAcceptedDialog, foundDialogs);
                    await FindQuestFromDialog(playerCharacter, questIds, dialog.questDeclinedDialog, foundDialogs);
                    await FindQuestFromDialog(playerCharacter, questIds, dialog.questAbandonedDialog, foundDialogs);
                    await FindQuestFromDialog(playerCharacter, questIds, dialog.questCompletedDialog, foundDialogs);
                    break;
                case NpcDialogType.CraftItem:
                    await FindQuestFromDialog(playerCharacter, questIds, dialog.craftNotMeetRequirementsDialog, foundDialogs);
                    await FindQuestFromDialog(playerCharacter, questIds, dialog.craftDoneDialog, foundDialogs);
                    await FindQuestFromDialog(playerCharacter, questIds, dialog.craftCancelDialog, foundDialogs);
                    break;
                case NpcDialogType.SaveRespawnPoint:
                    await FindQuestFromDialog(playerCharacter, questIds, dialog.saveRespawnConfirmDialog, foundDialogs);
                    await FindQuestFromDialog(playerCharacter, questIds, dialog.saveRespawnCancelDialog, foundDialogs);
                    break;
                case NpcDialogType.Warp:
                    await FindQuestFromDialog(playerCharacter, questIds, dialog.warpCancelDialog, foundDialogs);
                    break;
                case NpcDialogType.RefineItem:
                    await FindQuestFromDialog(playerCharacter, questIds, dialog.refineItemCancelDialog, foundDialogs);
                    break;
                case NpcDialogType.PlayerStorage:
                case NpcDialogType.GuildStorage:
                    await FindQuestFromDialog(playerCharacter, questIds, dialog.storageCancelDialog, foundDialogs);
                    break;
                case NpcDialogType.DismantleItem:
                    await FindQuestFromDialog(playerCharacter, questIds, dialog.dismantleItemCancelDialog, foundDialogs);
                    break;
                case NpcDialogType.RepairItem:
                    await FindQuestFromDialog(playerCharacter, questIds, dialog.repairItemCancelDialog, foundDialogs);
                    break;
            }
        }

        public async UniTask<bool> HaveNewQuests(IPlayerCharacterData playerCharacter)
        {
            if (playerCharacter == null)
                return false;
            HashSet<int> questIds = new HashSet<int>();
            await FindQuestFromDialog(playerCharacter, questIds, StartDialog);
            Quest quest;
            List<int> clearedQuests = new List<int>();
            foreach (CharacterQuest characterQuest in playerCharacter.Quests)
            {
                quest = characterQuest.GetQuest();
                if (quest == null || characterQuest.isComplete)
                    continue;
                clearedQuests.Add(quest.DataId);
            }
            foreach (int questId in questIds)
            {
                if (!clearedQuests.Contains(questId) &&
                    GameInstance.Quests.ContainsKey(questId) &&
                    GameInstance.Quests[questId].CanReceiveQuest(playerCharacter))
                    return true;
            }
            return false;
        }

        public async UniTask<bool> HaveInProgressQuests(IPlayerCharacterData playerCharacter)
        {
            if (playerCharacter == null)
                return false;
            HashSet<int> questIds = new HashSet<int>();
            await FindQuestFromDialog(playerCharacter, questIds, StartDialog);
            Quest quest;
            List<int> inProgressQuests = new List<int>();
            foreach (CharacterQuest characterQuest in playerCharacter.Quests)
            {
                quest = characterQuest.GetQuest();
                if (quest == null || characterQuest.isComplete)
                    continue;
                if (quest.HaveToTalkToNpc(playerCharacter, this, characterQuest.randomTasksIndex, out int talkToNpcTaskIndex, out _, out bool completeAfterTalked) && !completeAfterTalked && !characterQuest.completedTasks.Contains(talkToNpcTaskIndex))
                    return true;
                inProgressQuests.Add(quest.DataId);
            }
            foreach (int questId in questIds)
            {
                if (inProgressQuests.Contains(questId))
                    return true;
            }
            return false;
        }

        public async UniTask<bool> HaveTasksDoneQuests(IPlayerCharacterData playerCharacter)
        {
            if (playerCharacter == null)
                return false;
            HashSet<int> questIds = new HashSet<int>();
            await FindQuestFromDialog(playerCharacter, questIds, StartDialog);
            Quest quest;
            List<int> tasksDoneQuests = new List<int>();
            foreach (CharacterQuest characterQuest in playerCharacter.Quests)
            {
                quest = characterQuest.GetQuest();
                if (quest == null || characterQuest.isComplete || !characterQuest.IsAllTasksDoneAndIsCompletingTarget(playerCharacter, this))
                    continue;
                if (quest.HaveToTalkToNpc(playerCharacter, this, characterQuest.randomTasksIndex, out _, out _, out bool completeAfterTalked) && completeAfterTalked)
                    return true;
                tasksDoneQuests.Add(quest.DataId);
            }
            foreach (int questId in questIds)
            {
                if (tasksDoneQuests.Contains(questId))
                    return true;
            }
            return false;
        }

        public virtual float GetActivatableDistance()
        {
            if (activatableDistance > 0f)
                return activatableDistance;
            else
                return GameInstance.Singleton.conversationDistance;
        }

        public virtual bool ShouldClearTargetAfterActivated()
        {
            return false;
        }

        public virtual bool ShouldNotActivateAfterFollowed()
        {
            return false;
        }

        public virtual bool ShouldBeAttackTarget()
        {
            return false;
        }

        public virtual bool CanActivate()
        {
            return true;
        }

        public virtual void OnActivate()
        {
            GameInstance.PlayingCharacterEntity.NpcActionComponent.CallCmdNpcActivate(ObjectId);
        }
    }
}
