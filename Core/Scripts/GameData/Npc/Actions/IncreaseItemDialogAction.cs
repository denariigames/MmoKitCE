using Cysharp.Threading.Tasks;
using UnityEngine;

namespace MultiplayerARPG
{
    [CreateAssetMenu(fileName = GameDataMenuConsts.INCREASE_ITEM_DIALOG_ACTION_FILE, menuName = GameDataMenuConsts.INCREASE_ITEM_DIALOG_ACTION_MENU, order = GameDataMenuConsts.INCREASE_ITEM_DIALOG_ACTION_ORDER)]
    public class IncreaseItemDialogAction : BaseNpcDialogAction
    {
        public ItemAmount[] itemAmounts = new ItemAmount[0];

        public override UniTask DoAction(IPlayerCharacterData player)
        {
            player.IncreaseItems(itemAmounts);
            return UniTask.CompletedTask;
        }

        public override UniTask<bool> IsPass(IPlayerCharacterData player)
        {
            if (player.IncreasingItemsWillOverwhelming(itemAmounts))
            {
                if (player is PlayerCharacterEntity entity)
                    GameInstance.ServerGameMessageHandlers.SendGameMessage(entity.ConnectionId, UITextKeys.UI_ERROR_WILL_OVERWHELMING);
                return new UniTask<bool>(false);
            }
            return new UniTask<bool>(true);
        }
    }
}