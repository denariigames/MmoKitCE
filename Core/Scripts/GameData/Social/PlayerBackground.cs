using UnityEngine;

namespace MultiplayerARPG
{
    [CreateAssetMenu(fileName = GameDataMenuConsts.PLAYER_BACKGROUND_FILE, menuName = GameDataMenuConsts.PLAYER_BACKGROUND_MENU, order = GameDataMenuConsts.PLAYER_BACKGROUND_ORDER)]
    public partial class PlayerBackground : BaseGameData, IUnlockableGameData
    {
        [SerializeField]
        private UnlockRequirement unlockRequirement;
        public UnlockRequirement UnlockRequirement
        {
            get { return unlockRequirement; }
        }
    }
}
