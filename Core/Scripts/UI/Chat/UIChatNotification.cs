using UnityEngine;
using UnityEngine.UI;

namespace MultiplayerARPG
{
    public class UIChatNotification : UIBase
    {
        public ChatChannel chatChannel = ChatChannel.Local;
        public bool notifyForAllChannels = true;
        [Tooltip("It will stop counting while any of these objects is activated")]
        public GameObject[] objectsToStopCounting = new GameObject[0];
        [Tooltip("It will stop counting while any of these toggles is on")]
        public Toggle[] togglesToStopCounting = new Toggle[0];
        public GameObject[] notificationObjects = new GameObject[0];
        public TextWrapper[] notificationCountTexts = new TextWrapper[0];
        public bool StopCounting { get; set; } = false;

        private int _notificationCount = 0;

        protected override void Awake()
        {
            base.Awake();
            SetOnClientReceiveChatMessage();
            SetNotificationCount(0);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            RemoveOnClientReceiveChatMessage();
            objectsToStopCounting.Nullify();
            togglesToStopCounting.Nullify();
            notificationObjects.Nullify();
            notificationCountTexts.Nullify();
        }

        private void Update()
        {
            for (int i = 0; i < objectsToStopCounting.Length; ++i)
            {
                if (objectsToStopCounting[i].activeSelf)
                {
                    if (!StopCounting)
                    {
                        _notificationCount = 0;
                        SetNotificationCount(_notificationCount);
                    }
                    StopCounting = true;
                    return;
                }
            }
            for (int i = 0; i < togglesToStopCounting.Length; ++i)
            {
                if (togglesToStopCounting[i].isOn)
                {
                    if (!StopCounting)
                    {
                        _notificationCount = 0;
                        SetNotificationCount(_notificationCount);
                    }
                    StopCounting = true;
                    return;
                }
            }
            StopCounting = false;
        }

        public void SetOnClientReceiveChatMessage()
        {
            RemoveOnClientReceiveChatMessage();
            ClientGenericActions.onClientReceiveChatMessage += OnReceiveChat;
        }

        public void RemoveOnClientReceiveChatMessage()
        {
            ClientGenericActions.onClientReceiveChatMessage -= OnReceiveChat;
        }

        private void OnReceiveChat(ChatMessage chatMessage)
        {
            if (StopCounting)
            {
                return;
            }
            System.DateTime chatDateTime = new System.DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc).AddMilliseconds(chatMessage.timestamp).ToLocalTime();
            System.DateTime currentDataTime = System.DateTime.UtcNow.ToLocalTime();
            if ((currentDataTime - chatDateTime).TotalSeconds > 5)
                return;
            if (notifyForAllChannels)
            {
                _notificationCount++;
            }
            else if (chatMessage.channel == ChatChannel.Local &&
                chatChannel == ChatChannel.Local)
            {
                _notificationCount++;
            }
            else if (chatMessage.channel == ChatChannel.Global &&
                chatChannel == ChatChannel.Global)
            {
                _notificationCount++;
            }
            else if (chatMessage.channel == ChatChannel.Whisper &&
                chatChannel == ChatChannel.Whisper)
            {
                _notificationCount++;
            }
            else if (chatMessage.channel == ChatChannel.Party &&
                chatChannel == ChatChannel.Party)
            {
                _notificationCount++;
            }
            else if (chatMessage.channel == ChatChannel.Guild &&
                chatChannel == ChatChannel.Guild)
            {
                _notificationCount++;
            }
            else if (chatMessage.channel == ChatChannel.System &&
                chatChannel == ChatChannel.System)
            {
                _notificationCount++;
            }
            SetNotificationCount(_notificationCount);
        }

        public void SetNotificationCount(int count)
        {
            if (notificationObjects != null && notificationObjects.Length > 0)
            {
                foreach (GameObject obj in notificationObjects)
                {
                    obj.SetActive(count > 0);
                }
            }
            if (notificationCountTexts != null && notificationCountTexts.Length > 0)
            {
                foreach (TextWrapper txt in notificationCountTexts)
                {
                    if (count >= 99)
                        txt.text = "99+";
                    else if (count > 89)
                        txt.text = "90+";
                    else if (count > 79)
                        txt.text = "80+";
                    else if (count > 69)
                        txt.text = "70+";
                    else if (count > 59)
                        txt.text = "60+";
                    else if (count > 49)
                        txt.text = "50+";
                    else if (count > 39)
                        txt.text = "40+";
                    else if (count > 29)
                        txt.text = "30+";
                    else if (count > 19)
                        txt.text = "20+";
                    else if (count > 9)
                        txt.text = "10+";
                    else
                        txt.text = count.ToString();
                    txt.SetGameObjectActive(count > 0);
                }
            }
        }
    }
}
