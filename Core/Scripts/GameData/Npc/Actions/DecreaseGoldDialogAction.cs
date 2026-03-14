using Cysharp.Threading.Tasks;
using UnityEngine;

namespace MultiplayerARPG
{
    [CreateAssetMenu(fileName = GameDataMenuConsts.DECREASE_GOLD_DIALOG_ACTION_FILE, menuName = GameDataMenuConsts.DECREASE_GOLD_DIALOG_ACTION_MENU, order = GameDataMenuConsts.DECREASE_GOLD_DIALOG_ACTION_ORDER)]
    public class DecreaseGoldDialogAction : BaseNpcDialogAction
    {
        public int gold;

        public override UniTask DoAction(IPlayerCharacterData player)
        {
            player.Gold -= gold;
            return UniTask.CompletedTask;
        }

        public override UniTask<bool> IsPass(IPlayerCharacterData player)
        {
            if (player.Gold < gold)
                return new UniTask<bool>(false);
            return new UniTask<bool>(true);
        }
    }
}