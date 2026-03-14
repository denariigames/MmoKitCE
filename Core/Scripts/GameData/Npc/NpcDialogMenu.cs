using Cysharp.Threading.Tasks;
using Insthync.AddressableAssetTools;
using Insthync.UnityEditorUtils;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace MultiplayerARPG
{
    [System.Serializable]
    public class NpcDialogMenu
    {
        [Tooltip("Default title")]
        public string title;
        [Tooltip("Titles by language keys")]
        public LanguageData[] titles = new LanguageData[0];

#if UNITY_EDITOR || !UNITY_SERVER
#if UNITY_EDITOR || !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
        [PreviewSprite(50)]
        [SerializeField]
#if !DISABLE_ADDRESSABLES
        [AddressableAssetConversion(nameof(addressableIcon))]
#endif
        protected Sprite icon;
#endif
        public Sprite Icon
        {
            get
            {
#if !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
                return icon;
#else
                return null;
#endif
            }
            set
            {
#if !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
                icon = value;
#endif
            }
        }

#if !DISABLE_ADDRESSABLES
        [SerializeField]
        protected AssetReferenceSprite addressableIcon;
        public AssetReferenceSprite AddressableIcon
        {
            get
            {
                return addressableIcon;
            }
            set
            {
                addressableIcon = value;
            }
        }
#endif

        public UniTask<Sprite> GetIcon()
        {
#if !DISABLE_ADDRESSABLES
            return AddressableIcon.GetOrLoadObjectAsyncOrUseAsset(Icon);
#else
            return UniTask.FromResult(Icon);
#endif
        }
#endif

        public NpcDialogCondition[] showConditions = new NpcDialogCondition[0];
        public bool isCloseMenu;
        [BoolShowConditional(nameof(isCloseMenu), false)]
        public BaseNpcDialog dialog;

        public string Title
        {
            get { return Language.GetText(titles, title); }
        }

        public async UniTask<bool> IsPassConditions(IPlayerCharacterData character)
        {
            if (dialog != null && !await dialog.IsPassMenuCondition(character))
                return false;
            List<UniTask<bool>> tasks = new List<UniTask<bool>>();
            foreach (NpcDialogCondition showCondition in showConditions)
            {
                tasks.Add(showCondition.IsPass(character));
            }
            bool[] isPasses = await UniTask.WhenAll(tasks);
            foreach (bool isPass in isPasses)
            {
                if (!isPass)
                    return false;
            }
            return true;
        }
    }
}