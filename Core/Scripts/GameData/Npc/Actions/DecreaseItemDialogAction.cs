using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace MultiplayerARPG
{
    [CreateAssetMenu(fileName = GameDataMenuConsts.DECREASE_ITEM_DIALOG_ACTION_FILE, menuName = GameDataMenuConsts.DECREASE_ITEM_DIALOG_ACTION_MENU, order = GameDataMenuConsts.DECREASE_ITEM_DIALOG_ACTION_ORDER)]
    public class DecreaseItemDialogAction : BaseNpcDialogAction
    {
        public ItemAmount[] itemAmounts = new ItemAmount[0];

        public override UniTask DoAction(IPlayerCharacterData player)
        {
            player.DecreaseItems(itemAmounts);
            return UniTask.CompletedTask;
        }

        public override UniTask<bool> IsPass(IPlayerCharacterData player)
        {
            using (CollectionPool<Dictionary<BaseItem, int>, KeyValuePair<BaseItem, int>>.Get(out Dictionary<BaseItem, int> requireItemAmounts))
            {
                GameDataHelpers.CombineItems(itemAmounts, requireItemAmounts);
                if (!player.HasEnoughNonEquipItemAmounts(requireItemAmounts, out UITextKeys gameMessage, out _))
                {
                    if (player is PlayerCharacterEntity entity)
                        GameInstance.ServerGameMessageHandlers.SendGameMessage(entity.ConnectionId, gameMessage);
                    return new UniTask<bool>(false);
                }
            }
            return new UniTask<bool>(true);
        }
    }
}