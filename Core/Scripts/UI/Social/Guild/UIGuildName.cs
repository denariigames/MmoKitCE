using Cysharp.Text;
using UnityEngine;

namespace MultiplayerARPG
{
    public class UIGuildName : UIBase
    {
        [Tooltip("Format => {0} = {Guild Name}")]
        public UILocaleKeySetting formatKeyGuildName = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_SIMPLE);
        public TextWrapper textGuildName;

        public GuildData Guild { get { return GameInstance.JoinedGuild; } }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            textGuildName = null;
        }

        private void Update()
        {
            if (textGuildName != null)
            {
                if (Guild != null)
                {
                    textGuildName.text = ZString.Format(LanguageManager.GetText(formatKeyGuildName), Guild.guildName);
                    textGuildName.gameObject.SetActive(true);
                }
                else
                {
                    textGuildName.gameObject.SetActive(false);
                }
            }
        }
    }
}
