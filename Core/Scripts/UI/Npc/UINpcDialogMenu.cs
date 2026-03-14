using UnityEngine;
using UnityEngine.UI;

namespace MultiplayerARPG
{
    [System.Serializable]
    public struct UINpcDialogMenuAction
    {
        public string title;
        public Sprite icon;
        public int menuIndex;
    }

    public partial class UINpcDialogMenu : UISelectionEntry<UINpcDialogMenuAction>
    {
        [Header("UI Elements")]
        public TextWrapper uiTextTitle;
        public UINpcDialog uiNpcDialog;
        public Image imageIcon;

        protected override void OnDestroy()
        {
            base.OnDestroy();
            uiTextTitle = null;
            uiNpcDialog = null;
            imageIcon = null;
        }

        protected override void UpdateData()
        {
            if (uiTextTitle != null)
                uiTextTitle.text = Data.title;

            if (imageIcon != null)
            {
                Sprite iconSprite = Data.icon;
                imageIcon.gameObject.SetActive(iconSprite != null);
                imageIcon.sprite = iconSprite;
            }
        }

        public void OnClickMenu()
        {
            GameInstance.PlayingCharacterEntity.NpcActionComponent.CallCmdSelectNpcDialogMenu((byte)Data.menuIndex);
        }
    }
}
