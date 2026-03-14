using Cysharp.Threading.Tasks;
using UnityEngine;

namespace MultiplayerARPG
{
    [CreateAssetMenu(fileName = GameDataMenuConsts.INCREASE_GOLD_DIALOG_ACTION_FILE, menuName = GameDataMenuConsts.INCREASE_GOLD_DIALOG_ACTION_MENU, order = GameDataMenuConsts.INCREASE_GOLD_DIALOG_ACTION_ORDER)]
    public class IncreaseGoldDialogAction : BaseNpcDialogAction
    {
        public int gold;

        public override UniTask DoAction(IPlayerCharacterData player)
        {
            player.Gold = player.Gold.Increase(gold);
            return UniTask.CompletedTask;
        }

        public override UniTask<bool> IsPass(IPlayerCharacterData player)
        {
            return new UniTask<bool>(true);
        }
    }
}