using Insthync.UnityEditorUtils.Editor;
using UnityEditor;

namespace MultiplayerARPG
{
    [CustomEditor(typeof(NpcDialog))]
    [CanEditMultipleObjects]
    public class NpcDialogEditor : BaseCustomEditor
    {
        protected override void SetFieldCondition()
        {
            NpcDialog npcDialog = target as NpcDialog;

            if ((target as NpcDialog).graph == null)
            {
                hiddenFields.Add("graph");
                hiddenFields.Add("position");
                hiddenFields.Add("ports");
            }
            hiddenFields.Add("input");

            // Normal
            ShowOnEnum(nameof(npcDialog.type), nameof(NpcDialogType.Normal), nameof(npcDialog.menus));
            // Quest
            ShowOnEnum(nameof(npcDialog.type), nameof(NpcDialogType.Quest), nameof(npcDialog.quest));
            ShowOnEnum(nameof(npcDialog.type), nameof(NpcDialogType.Quest), nameof(npcDialog.questAcceptedDialog));
            ShowOnEnum(nameof(npcDialog.type), nameof(NpcDialogType.Quest), nameof(npcDialog.questDeclinedDialog));
            ShowOnEnum(nameof(npcDialog.type), nameof(NpcDialogType.Quest), nameof(npcDialog.questAbandonedDialog));
            ShowOnEnum(nameof(npcDialog.type), nameof(NpcDialogType.Quest), nameof(npcDialog.questCompletedDialog));
            // Shop
            ShowOnEnum(nameof(npcDialog.type), nameof(NpcDialogType.Shop), nameof(npcDialog.sellItems));
            // Craft Item
            ShowOnEnum(nameof(npcDialog.type), nameof(NpcDialogType.CraftItem), nameof(npcDialog.itemCraft));
            ShowOnEnum(nameof(npcDialog.type), nameof(NpcDialogType.CraftItem), nameof(npcDialog.craftDoneDialog));
            ShowOnEnum(nameof(npcDialog.type), nameof(NpcDialogType.CraftItem), nameof(npcDialog.craftItemWillOverwhelmingDialog));
            ShowOnEnum(nameof(npcDialog.type), nameof(NpcDialogType.CraftItem), nameof(npcDialog.craftNotMeetRequirementsDialog));
            ShowOnEnum(nameof(npcDialog.type), nameof(NpcDialogType.CraftItem), nameof(npcDialog.craftCancelDialog));
            // Save Spawn Point
            ShowOnEnum(nameof(npcDialog.type), nameof(NpcDialogType.SaveRespawnPoint), nameof(npcDialog.saveRespawnMap));
            ShowOnEnum(nameof(npcDialog.type), nameof(NpcDialogType.SaveRespawnPoint), nameof(npcDialog.saveRespawnPosition));
            ShowOnEnum(nameof(npcDialog.type), nameof(NpcDialogType.SaveRespawnPoint), nameof(npcDialog.saveRespawnConfirmDialog));
            ShowOnEnum(nameof(npcDialog.type), nameof(NpcDialogType.SaveRespawnPoint), nameof(npcDialog.saveRespawnCancelDialog));
            ShowOnEnum(nameof(npcDialog.type), nameof(NpcDialogType.SaveRespawnPoint), nameof(npcDialog.confirmRequirement));
            // Warp
            ShowOnEnum(nameof(npcDialog.type), nameof(NpcDialogType.Warp), nameof(npcDialog.warpPortalType));
            ShowOnEnum(nameof(npcDialog.type), nameof(NpcDialogType.Warp), nameof(npcDialog.warpMap));
            ShowOnEnum(nameof(npcDialog.type), nameof(NpcDialogType.Warp), nameof(npcDialog.warpPosition));
            ShowOnEnum(nameof(npcDialog.type), nameof(NpcDialogType.Warp), nameof(npcDialog.warpOverrideRotation));
            ShowOnEnum(nameof(npcDialog.type), nameof(NpcDialogType.Warp), nameof(npcDialog.warpRotation));
            ShowOnEnum(nameof(npcDialog.type), nameof(NpcDialogType.Warp), nameof(npcDialog.warpCancelDialog));
            ShowOnEnum(nameof(npcDialog.type), nameof(NpcDialogType.Warp), nameof(npcDialog.confirmRequirement));
            // Refine Item
            ShowOnEnum(nameof(npcDialog.type), nameof(NpcDialogType.RefineItem), nameof(npcDialog.refineItemCancelDialog));
            // Dismantle Item
            ShowOnEnum(nameof(npcDialog.type), nameof(NpcDialogType.DismantleItem), nameof(npcDialog.dismantleItemCancelDialog));
            // Repair Item
            ShowOnEnum(nameof(npcDialog.type), nameof(NpcDialogType.RepairItem), nameof(npcDialog.repairItemCancelDialog));
            // Storage
            ShowOnEnum(nameof(npcDialog.type), nameof(NpcDialogType.PlayerStorage), nameof(npcDialog.storageCancelDialog));
            ShowOnEnum(nameof(npcDialog.type), nameof(NpcDialogType.GuildStorage), nameof(npcDialog.storageCancelDialog));
        }
    }
}
