namespace MultiplayerARPG
{
    public interface IItemsContainerUIVisibilityManager
    {
        bool IsItemsContainerDialogVisible();
        void ShowItemsContainerDialog(ItemsContainerEntity itemsContainerEntity);
        void HideItemsContainerDialog();
        ItemsContainerEntity ItemsContainerEntity { get; }
    }
}
