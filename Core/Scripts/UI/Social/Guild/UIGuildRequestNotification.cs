using UnityEngine;

namespace MultiplayerARPG
{
    public class UIGuildRequestNotification : UIBase
    {
        public GameObject[] notificationObjects = new GameObject[0];
        public TextWrapper[] notificationCountTexts = new TextWrapper[0];

        protected override void OnDestroy()
        {
            base.OnDestroy();
            notificationObjects.Nullify();
            notificationCountTexts.Nullify();
        }

        private void OnEnable()
        {
            Refresh();
            GuildRequestNotificationCacheManager.onSetGuildRequestNotification += SetNotificationCount;
        }

        private void OnDisable()
        {
            GuildRequestNotificationCacheManager.onSetGuildRequestNotification -= SetNotificationCount;
        }

        public void Refresh()
        {
            Refresh(false);
        }

        public void Refresh(bool force)
        {
            if (GameInstance.ClientGuildHandlers == null)
            {
                SetNotificationCount(0);
                return;
            }
            GuildRequestNotificationCacheManager.LoadOrGetGuildRequestNotificationFromCache(SetNotificationCount, force);
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
