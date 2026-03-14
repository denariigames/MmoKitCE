using Cysharp.Text;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace MultiplayerARPG
{
    public partial class UIPlayerFrame : UISelectionEntry<PlayerFrame>
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
            PlayerFrame frame = Data;
            if (frame == null)
                frame = GameInstance.PlayerFrames.Values.FirstOrDefault();

            if (uiTextTitle != null)
            {
                uiTextTitle.text = ZString.Format(
                    LanguageManager.GetText(formatKeyTitle),
                    frame == null ? LanguageManager.GetUnknowTitle() : frame.Title);
            }

            if (uiTextDescription != null)
            {
                uiTextDescription.text = ZString.Format(
                    LanguageManager.GetText(formatKeyDescription),
                    frame == null ? LanguageManager.GetUnknowDescription() : frame.Description);
            }

            imageIcon.SetImageGameDataIcon(frame);
        }

        public void SetDataByDataId(int dataId)
        {
            PlayerFrame frame;
            if (GameInstance.PlayerFrames.TryGetValue(dataId, out frame))
                Data = frame;
            else
                Data = GameInstance.PlayerFrames.Values.FirstOrDefault();
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
