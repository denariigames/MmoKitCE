using Insthync.CameraAndInput;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace MultiplayerARPG
{
    public partial class UIChatHandler : UIBase
    {
        public enum EntryClickResponseMode
        {
            SetWhisterAsCommand,
            SetWhisterReceiverToField,
            Custom,
        }
        public readonly List<ChatMessage> ChatMessages = new List<ChatMessage>();
        public readonly Dictionary<ChatChannel, List<ChatMessage>> ChannelBasedChatMessages = new Dictionary<ChatChannel, List<ChatMessage>>();

        [Header("Configs")]
        [Tooltip("Message channel to send without channel commands, if `showingMessagesFromAllChannels` is `FALSE` it will show messages from this channel only")]
        public ChatChannel chatChannel = ChatChannel.Local;
        public bool showingMessagesFromAllChannels = true;
        [Tooltip("When enter this key it will show enter chat message field")]
        public KeyCode enterChatKey = KeyCode.Return;
        [Range(0, UIChatHistory.MAX_CHAT_HISTORY)]
        public int chatEntrySize = 30;
        public EntryClickResponseMode entryClickResponseMode = EntryClickResponseMode.SetWhisterAsCommand;

        [Header("Channel Commands")]
        public string globalCommand = "/a";
        public string whisperCommand = "/w";
        public string partyCommand = "/p";
        public string guildCommand = "/g";
        public string systemCommand = "/s";

        [Header("UIs")]
        [Tooltip("These game objects will be activated while entering chat message")]
        public GameObject[] enterChatActiveObjects;
        public InputFieldWrapper uiReceiverField;
        [FormerlySerializedAs("uiEnterChatField")]
        public InputFieldWrapper uiMessageField;
        [FormerlySerializedAs("uiChatMessagePrefab")]
        public UIChatMessage uiPrefab;
        public UIChatMessage uiPrefabOther;
        public UIChatMessage uiPrefabMine;
        [FormerlySerializedAs("uiChatMessageContainer")]
        public Transform uiContainer;
        public ScrollRect scrollRect;

        public bool EnterChatFieldVisible { get; private set; }

        public string EnterChatReceiver
        {
            get { return uiReceiverField == null ? string.Empty : uiReceiverField.text; }
            set { if (uiReceiverField != null) uiReceiverField.text = value; }
        }

        public string EnterChatMessage
        {
            get { return uiMessageField == null ? string.Empty : uiMessageField.text; }
            set { if (uiMessageField != null) uiMessageField.text = value; }
        }

        private UIList _cacheList;
        public UIList CacheList
        {
            get
            {
                if (_cacheList == null)
                {
                    _cacheList = gameObject.AddComponent<UIList>();
                    _cacheList.uiContainer = uiContainer;
                }
                return _cacheList;
            }
        }

        public event System.Action<UIChatMessage> onClickChatEntry;

        private bool _aleradySetupReceiveEvent;
        private bool _movingToEnd;
        private ChatChannel _dirtyChatChannel;

        protected override void Awake()
        {
            base.Awake();
            SetOnClientReceiveChatMessage();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            RemoveOnClientReceiveChatMessage();
            ChannelBasedChatMessages?.Clear();
            enterChatActiveObjects.Nullify();
            uiReceiverField = null;
            uiMessageField = null;
            uiPrefab = null;
            uiContainer = null;
            scrollRect = null;
            _cacheList = null;
            onClickChatEntry = null;
        }

        private void Start()
        {
            StartCoroutine(VerticalScroll(0f));
            HideEnterChatField();
            if (uiMessageField != null)
            {
                uiMessageField.onValueChanged.RemoveListener(OnMessageFieldValueChange);
                uiMessageField.onValueChanged.AddListener(OnMessageFieldValueChange);
            }
        }

        private void OnEnable()
        {
            StartCoroutine(VerticalScroll(0f));
        }

        public void SetOnClientReceiveChatMessage()
        {
            if (_aleradySetupReceiveEvent)
                return;
            RemoveOnClientReceiveChatMessage();
            CacheList.HideAll();
            ChatMessages.Clear();
            int index = UIChatHistory.ChatMessages.Count - chatEntrySize;
            if (index < 0)
                index = 0;
            while (index < UIChatHistory.ChatMessages.Count)
            {
                OnReceiveChat(UIChatHistory.ChatMessages[index]);
                index++;
            }
            ClientGenericActions.onClientReceiveChatMessage += OnReceiveChat;
            _aleradySetupReceiveEvent = true;
        }

        public void RemoveOnClientReceiveChatMessage()
        {
            ClientGenericActions.onClientReceiveChatMessage -= OnReceiveChat;
            _aleradySetupReceiveEvent = false;
        }

        private void Update()
        {
            if (_dirtyChatChannel != chatChannel)
            {
                _dirtyChatChannel = chatChannel;
                if (!showingMessagesFromAllChannels)
                    FillChatMessages();
            }
            if (_movingToEnd)
            {
                _movingToEnd = false;
                uiMessageField.MoveTextEnd(false);
            }
            if (InputManager.GetKeyUp(enterChatKey))
            {
                if (!EnterChatFieldVisible)
                    ShowEnterChatField();
                else
                    SendChatMessage();
            }
        }

        public void ToggleEnterChatField()
        {
            if (EnterChatFieldVisible)
                HideEnterChatField();
            else
                ShowEnterChatField();
        }

        public void ShowEnterChatField()
        {
            foreach (GameObject enterChatActiveObject in enterChatActiveObjects)
            {
                if (enterChatActiveObject != null)
                    enterChatActiveObject.SetActive(true);
            }
            if (uiMessageField != null)
            {
                uiMessageField.ActivateInputField();
                if (EventSystem.current != null)
                    EventSystem.current.SetSelectedGameObject(uiMessageField.gameObject);
                _movingToEnd = true;
            }
            EnterChatFieldVisible = true;
        }

        public void HideEnterChatField()
        {
            foreach (GameObject enterChatActiveObject in enterChatActiveObjects)
            {
                if (enterChatActiveObject != null)
                    enterChatActiveObject.SetActive(false);
            }
            if (uiMessageField != null)
            {
                uiMessageField.DeactivateInputField();
                if (EventSystem.current != null)
                    EventSystem.current.SetSelectedGameObject(null);
            }
            EnterChatFieldVisible = false;
        }

        public void SendChatMessage()
        {
            if (GameInstance.PlayingCharacter == null)
                return;

            string trimText = EnterChatMessage.Trim();
            if (trimText.Length == 0)
            {
                HideEnterChatField();
                return;
            }

            EnterChatMessage = string.Empty;
            ChatChannel channel = chatChannel;
            string message = trimText;
            string sender = GameInstance.PlayingCharacter.CharacterName;
            string receiver = string.Empty;
            // Set chat channel by command
            string[] splitedText = trimText.Split(' ');
            string cmd = splitedText[0];
            if (cmd.Equals(whisperCommand) &&
                splitedText.Length > 2)
            {
                channel = ChatChannel.Whisper;
                receiver = splitedText[1];
                message = trimText.Substring(cmd.Length + receiver.Length + 2); // +2 for space
                EnterChatMessage = trimText.Substring(0, cmd.Length + receiver.Length + 2); // +2 for space
            }
            if ((cmd.Equals(globalCommand) ||
                cmd.Equals(partyCommand) ||
                cmd.Equals(guildCommand) ||
                cmd.Equals(systemCommand))
                && splitedText.Length > 1)
            {
                if (cmd.Equals(globalCommand))
                    channel = ChatChannel.Global;
                if (cmd.Equals(partyCommand))
                    channel = ChatChannel.Party;
                if (cmd.Equals(guildCommand))
                    channel = ChatChannel.Guild;
                if (cmd.Equals(systemCommand))
                    channel = ChatChannel.System;
                message = trimText.Substring(cmd.Length + 1); // +1 for space
                EnterChatMessage = trimText.Substring(0, cmd.Length + 1); // +1 for space
            }
            if (channel == ChatChannel.Whisper && uiReceiverField)
            {
                receiver = EnterChatReceiver;
            }
            GameInstance.ClientChatHandlers.SendChatMessage(new ChatMessage()
            {
                channel = channel,
                message = message,
                senderName = sender,
                receiverName = receiver,
            });
            HideEnterChatField();
        }

        private void OnReceiveChat(ChatMessage chatMessage)
        {
            if (this == null)
            {
                RemoveOnClientReceiveChatMessage();
                return;
            }

            if (!ChannelBasedChatMessages.ContainsKey(chatMessage.channel))
                ChannelBasedChatMessages.Add(chatMessage.channel, new List<ChatMessage>());
            ChannelBasedChatMessages[chatMessage.channel].Add(chatMessage);
            if (ChannelBasedChatMessages[chatMessage.channel].Count > chatEntrySize)
                ChannelBasedChatMessages[chatMessage.channel].RemoveAt(0);

            ChatMessages.Add(chatMessage);
            if (ChatMessages.Count > chatEntrySize)
                ChatMessages.RemoveAt(0);

            FillChatMessages();
        }

        private void FillChatMessages()
        {
            if (showingMessagesFromAllChannels)
                CacheList.Generate(ChatMessages, GenerateChatEntry, ValidateChatEntryPrefab);
            else if (ChannelBasedChatMessages.ContainsKey(chatChannel))
                CacheList.Generate(ChannelBasedChatMessages[chatChannel], GenerateChatEntry, ValidateChatEntryPrefab);
            if (gameObject.activeInHierarchy)
                StartCoroutine(VerticalScroll(0f));
        }

        private void GenerateChatEntry(int index, ChatMessage message, GameObject ui)
        {
            UIChatMessage tempUiChatMessage = ui.GetComponent<UIChatMessage>();
            tempUiChatMessage.uiChatHandler = this;
            tempUiChatMessage.Data = message;
            tempUiChatMessage.Show();
        }

        private GameObject ValidateChatEntryPrefab(ChatMessage entry)
        {
            if (uiPrefabOther != null && uiPrefabMine != null)
            {
                if (string.Equals(entry.senderUserId, GameInstance.UserId))
                    return uiPrefabMine.gameObject;
                return uiPrefabOther.gameObject;
            }
            return uiPrefab.gameObject;
        }

        private void OnMessageFieldValueChange(string text)
        {
            if (text.Length > 0 && !EnterChatFieldVisible)
                ShowEnterChatField();
        }

        IEnumerator VerticalScroll(float normalize)
        {
            if (scrollRect != null)
            {
                Canvas.ForceUpdateCanvases();
                yield return null;
                scrollRect.verticalScrollbar.value = normalize;
                Canvas.ForceUpdateCanvases();
            }
        }

        public void OnClickEntry(UIChatMessage uiChatMessage)
        {
            if (uiChatMessage == null)
                return;
            switch (entryClickResponseMode)
            {
                case EntryClickResponseMode.SetWhisterAsCommand:
                    ShowEnterChatField();
                    EnterChatMessage = whisperCommand + " " + uiChatMessage.Data.senderName;
                    break;
                case EntryClickResponseMode.SetWhisterReceiverToField:
                    ShowEnterChatField();
                    EnterChatReceiver = uiChatMessage.Data.senderName;
                    break;
                default:
                    onClickChatEntry.Invoke(uiChatMessage);
                    break;
            }
        }
    }
}
