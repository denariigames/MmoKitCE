using Cysharp.Text;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace MultiplayerARPG
{
    public partial class UIPlayerIcon : UISelectionEntry<PlayerIcon>
    {
        [Header("String Formats")]
        [Tooltip("Format => {0} = {Title}")]
        public UILocaleKeySetting formatKeyTitle = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_SIMPLE);
        [Tooltip("Format => {0} = {Description}")]
        public UILocaleKeySetting formatKeyDescription = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_SIMPLE);

        [Header("UI Elements")]
        public TextWrapper uiTextTitle;
        public TextWrapper uiTextDescription;
        public Image imageIcon;
        public GameObject[] lockedObjects = new GameObject[0];
        public GameObject[] unlockedObjects = new GameObject[0];
        public bool IsLocked { get; private set; }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            imageIcon = null;
            lockedObjects.Nullify();
            unlockedObjects.Nullify();
            _data = null;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            UpdateData();
        }

        protected override void UpdateData()
        {
            PlayerIcon icon = Data;
            if (icon == null)
                icon = GameInstance.PlayerIcons.Values.FirstOrDefault();

            if (uiTextTitle != null)
            {
                uiTextTitle.text = ZString.Format(
                    LanguageManager.GetText(formatKeyTitle),
                    icon == null ? LanguageManager.GetUnknowTitle() : icon.Title);
            }

            if (uiTextDescription != null)
            {
                uiTextDescription.text = ZString.Format(
                    LanguageManager.GetText(formatKeyDescription),
                    icon == null ? LanguageManager.GetUnknowDescription() : icon.Description);
            }

            imageIcon.SetImageGameDataIcon(icon);
        }

        public void SetDataByDataId(int dataId)
        {
            PlayerIcon icon;
            if (GameInstance.PlayerIcons.TryGetValue(dataId, out icon))
                Data = icon;
            else
                Data = GameInstance.PlayerIcons.Values.FirstOrDefault();
        }

        public void SetIsLocked(bool isLocked)
        {
            foreach (GameObject lockedObject in lockedObjects)
            {
                lockedObject.SetActive(isLocked);
            }
            foreach (GameObject unlockedObject in unlockedObjects)
            {
                unlockedObject.SetActive(!isLocked);
            }
        }
    }
}
