namespace MultiplayerARPG
{
    public partial class UIQuestRewardItemSelection : UICharacterItems
    {
        protected override void OnEnable()
        {
            base.OnEnable();
            CacheSelectionManager.selectionMode = UISelectionMode.Toggle;
        }

        public void UpdateData(int questDataId)
        {
            if (!GameInstance.Quests.ContainsKey(questDataId))
                return;
            UpdateData(GameInstance.Quests[questDataId]);
        }

        public void UpdateData(Quest quest)
        {
            if (quest == null)
                return;
            inventoryType = InventoryType.Unknow;
            UpdateData(GameInstance.PlayingCharacterEntity, quest.selectableRewardItems);
        }

        public void OnClickSelectRewardItem()
        {
            if (CacheSelectionManager.SelectedUI == null)
                return;
            GameInstance.PlayingCharacterEntity.NpcActionComponent.CallCmdSelectQuestRewardItem((byte)CacheSelectionManager.SelectedUI.IndexOfData);
            Hide();
        }
    }
}
