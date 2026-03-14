using Cysharp.Text;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace MultiplayerARPG
{
    public partial class UIGuildIcon : UISelectionEntry<GuildIcon>
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

        protected override void OnDestroy()
        {
            base.OnDestroy();
            imageIcon = null;
            _data = null;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            UpdateData();
        }

        protected override void UpdateData()
        {
            GuildIcon icon = Data;
            if (icon == null)
                icon = GameInstance.GuildIcons.Values.FirstOrDefault();

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
            GuildIcon guildIcon;
            if (GameInstance.GuildIcons.TryGetValue(dataId, out guildIcon))
                Data = guildIcon;
            else
                Data = GameInstance.GuildIcons.Values.FirstOrDefault();
        }
    }
}
