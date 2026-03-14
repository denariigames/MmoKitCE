using Cysharp.Text;
using UnityEngine;
using UnityEngine.Events;

namespace MultiplayerARPG
{
    public partial class UIChatMessage : UISelectionEntry<ChatMessage>
    {
        [Header("String Formats")]
        [Tooltip("Format {0} = Character Name, {1} = Message")]
        public UILocaleKeySetting formatLocal = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_CHAT_LOCAL);
        [Tooltip("Format {0} = Character Name, {1} = Message")]
        public UILocaleKeySetting formatGlobal = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_CHAT_GLOBAL);
        [Tooltip("Format {0} = Character Name, {1} = Message")]
        public UILocaleKeySetting formatWhisper = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_CHAT_WHISPER);
        [Tooltip("Format {0} = Character Name, {1} = Message")]
        public UILocaleKeySetting formatParty = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_CHAT_PARTY);
        [Tooltip("Format {0} = Character Name, {1} = Message")]
        public UILocaleKeySetting formatGuild = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_CHAT_GUILD);
        [Tooltip("Format {0} = Message")]
        public UILocaleKeySetting formatSystem = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_CHAT_SYSTEM);
        [Tooltip("Format {0} = Character Name, {1} = Message, {2} = Guild Name")]
        public UILocaleKeySetting formatLocalWithGuildName = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_CHAT_LOCAL_WITH_GUILD_NAME);
        [Tooltip("Format {0} = Character Name, {1} = Message, {2} = Guild Name")]
        public UILocaleKeySetting formatGlobalWithGuildName = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_CHAT_GLOBAL_WITH_GUILD_NAME);
        [Tooltip("Format {0} = Character Name, {1} = Message, {2} = Guild Name")]
        public UILocaleKeySetting formatWhisperWithGuildName = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_CHAT_WHISPER_WITH_GUILD_NAME);
        [Tooltip("Format {0} = Character Name, {1} = Message, {2} = Guild Name")]
        public UILocaleKeySetting formatPartyWithGuildName = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_CHAT_PARTY_WITH_GUILD_NAME);

        public TextWrapper uiTextMessage;
        public TextWrapper uiTextSenderOnly;
        public TextWrapper uiTextMessageOnly;
        public TextWrapper uiTextTimestamp;
        public UIChatHandler uiChatHandler;
        public GameObject[] signOnlines = new GameObject[0];
        public GameObject[] signOfflines = new GameObject[0];
        public UnityEvent onIsTypeWriter = new UnityEvent();
        public UnityEvent onNotTypeWriter = new UnityEvent();

        protected override void OnDestroy()
        {
            base.OnDestroy();
            uiTextMessage = null;
            uiTextSenderOnly = null;
            uiTextMessageOnly = null;
            uiTextTimestamp = null;
            uiChatHandler = null;
            onIsTypeWriter?.RemoveAllListeners();
            onIsTypeWriter = null;
            onNotTypeWriter?.RemoveAllListeners();
            onNotTypeWriter = null;
        }

        protected override void UpdateUI()
        {
            bool isOnline = GameInstance.ClientOnlineCharacterHandlers.IsCharacterOnline(Data.senderId);
            GameInstance.ClientOnlineCharacterHandlers.RequestOnlineCharacter(Data.senderId);
            for (int i = 0; i < signOnlines.Length; ++i)
            {
                signOnlines[i].SetActive(isOnline);
            }
            for (int i = 0; i < signOfflines.Length; ++i)
            {
                signOfflines[i].SetActive(!isOnline);
            }
        }

        protected override void UpdateData()
        {
            if (uiTextMessage != null)
            {
                if (Data.channel == ChatChannel.System)
                {
                    uiTextMessage.text = ZString.Format(LanguageManager.GetText(formatSystem), Data.message);
                    onNotTypeWriter.Invoke();
                }
                else
                {
                    string format = string.Empty;
                    switch (Data.channel)
                    {
                        case ChatChannel.Local:
                            if (string.IsNullOrWhiteSpace(Data.guildName))
                                format = LanguageManager.GetText(formatLocal);
                            else
                                format = LanguageManager.GetText(formatLocalWithGuildName);
                            break;
                        case ChatChannel.Global:
                            if (string.IsNullOrWhiteSpace(Data.guildName))
                                format = LanguageManager.GetText(formatGlobal);
                            else
                                format = LanguageManager.GetText(formatGlobalWithGuildName);
                            break;
                        case ChatChannel.Whisper:
                            if (string.IsNullOrWhiteSpace(Data.guildName))
                                format = LanguageManager.GetText(formatWhisper);
                            else
                                format = LanguageManager.GetText(formatWhisperWithGuildName);
                            break;
                        case ChatChannel.Party:
                            if (string.IsNullOrWhiteSpace(Data.guildName))
                                format = LanguageManager.GetText(formatParty);
                            else
                                format = LanguageManager.GetText(formatPartyWithGuildName);
                            break;
                        case ChatChannel.Guild:
                            format = LanguageManager.GetText(formatGuild);
                            break;
                    }
                    uiTextMessage.text = ZString.Format(format, Data.senderName, Data.message, Data.guildName);
                    if (GameInstance.PlayingCharacter != null && GameInstance.PlayingCharacter.CharacterName.Equals(Data.senderName))
                        onIsTypeWriter.Invoke();
                    else
                        onNotTypeWriter.Invoke();
                }
            }
            if (uiTextSenderOnly != null)
                uiTextSenderOnly.text = Data.senderName;
            if (uiTextMessageOnly != null)
                uiTextMessageOnly.text = Data.message;
            InvokeRepeating(nameof(UpdateTimestamp), 0f, 5f);
        }

        private void UpdateTimestamp()
        {
            if (uiTextTimestamp != null)
            {
                System.DateTime dateTime = new System.DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc).AddMilliseconds(Data.timestamp).ToLocalTime();
                uiTextTimestamp.text = (System.DateTime.Now - new System.DateTime(dateTime.Ticks)).GetPrettyDate();
            }
        }

        public void OnClickEntry()
        {
            if (uiChatHandler != null)
                uiChatHandler.OnClickEntry(this);
        }
    }
}
