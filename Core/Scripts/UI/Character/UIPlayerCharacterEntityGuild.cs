using Cysharp.Text;
using Newtonsoft.Json;
using UnityEngine;

namespace MultiplayerARPG
{
    public class UIPlayerCharacterEntityGuild : MonoBehaviour
    {
        [Header("String Formats")]
        [Tooltip("Format => {0} = {Title}")]
        public UILocaleKeySetting formatKeyTitle = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_SIMPLE);

        [Header("UI Elements")]
        public UICharacterEntity uiCharacterEntity;
        public TextWrapper textGuildName;
        public UIGuildIcon uiGuildIcon;

        private int guildId;

        private void OnEnable()
        {
            InvokeRepeating(nameof(UpdateData), 0f, 5f);
            uiCharacterEntity.onUpdateData += OnUpdateData;
            GuildInfoCacheManager.onSetGuildInfo += OnSetGuildInfo;
        }

        private void OnDisable()
        {
            uiCharacterEntity.onUpdateData -= OnUpdateData;
            GuildInfoCacheManager.onSetGuildInfo -= OnSetGuildInfo;
        }

        public void UpdateData()
        {
            OnUpdateData(uiCharacterEntity.Data);
        }

        private void OnUpdateData(BaseCharacterEntity entity)
        {
            BasePlayerCharacterEntity castedEntity = entity as BasePlayerCharacterEntity;
            if (castedEntity == null || castedEntity.GuildId <= 0)
            {
                guildId = 0;
                if (textGuildName != null)
                    textGuildName.SetGameObjectActive(false);
                if (uiGuildIcon != null)
                    uiGuildIcon.SetDataByDataId(0);
                return;
            }
            guildId = castedEntity.GuildId;

            GuildInfoCacheManager.LoadOrGetGuildInfoFromCache(castedEntity.GuildId, OnSetGuildInfo);
        }

        private void OnSetGuildInfo(GuildListEntry guild)
        {
            if (guildId == 0 || guild.Id != guildId)
                return;

            GuildOptions options = new GuildOptions();
            if (!string.IsNullOrEmpty(guild.Options))
                options = JsonConvert.DeserializeObject<GuildOptions>(guild.Options);

            if (textGuildName != null)
            {
                textGuildName.SetGameObjectActive(true);
                textGuildName.text = ZString.Format(LanguageManager.GetText(formatKeyTitle), guild.GuildName);
            }
            if (uiGuildIcon != null)
                uiGuildIcon.SetDataByDataId(options.iconDataId);
        }
    }
}
