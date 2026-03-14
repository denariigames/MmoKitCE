using UnityEngine;

namespace MultiplayerARPG
{
    [CreateAssetMenu(fileName = GameDataMenuConsts.PLAYER_TITLE_FILE, menuName = GameDataMenuConsts.PLAYER_TITLE_MENU, order = GameDataMenuConsts.PLAYER_TITLE_ORDER)]
    public partial class PlayerTitle : BaseGameData, IUnlockableGameData
    {
        [SerializeField]
        private UnlockRequirement unlockRequirement;
        public UnlockRequirement UnlockRequirement
        {
            get { return unlockRequirement; }
        }

        [SerializeField]
        protected Buff buff;
        public Buff Buff
        {
            get { return buff; }
            set { buff = value; }
        }

        [System.NonSerialized]
        protected CalculatedBuff _cacheBuff;
        public CalculatedBuff CacheBuff
        {
            get
            {
                if (_cacheBuff == null)
                {
                    _cacheBuff = new CalculatedBuff(Buff, 1);
                    return _cacheBuff;
                }
                return _cacheBuff;
            }
        }
    }
}
