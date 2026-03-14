using Cysharp.Text;
using UnityEngine;

namespace MultiplayerARPG
{
    public class UIGameMessageHandler : MonoBehaviour
    {
        [Tooltip("Format => {0} = {Exp Amount}")]
        public UILocaleKeySetting formatKeyRewardExp = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_NOTIFY_REWARD_EXP);
        [Tooltip("Format => {0} = {Gold Amount}")]
        public UILocaleKeySetting formatKeyRewardGold = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_NOTIFY_REWARD_GOLD);
        [Tooltip("Format => {0} = {Item Title}, {1} => {Amount}")]
        public UILocaleKeySetting formatKeyRewardItem = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_NOTIFY_REWARD_ITEM);
        [Tooltip("Format => {0} = {Item Title}, {1} => {Amount}")]
        public UILocaleKeySetting formatKeyRewardCurrency = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_NOTIFY_REWARD_CURRENCY);
        public TextWrapper messagePrefab;
        public TextWrapper rewardExpPrefab;
        public TextWrapper rewardGoldPrefab;
        public TextWrapper rewardItemPrefab;
        public TextWrapper rewardCurrencyPrefab;
        public Color errorMessageColor = Color.red;
        public Transform messageContainer;
        public float visibleDuration;

        protected UITextKeys _lastMessage;
        protected float _lastMessagedTime;

        protected virtual void Awake()
        {
            if (!rewardExpPrefab)
                rewardExpPrefab = messagePrefab;
            if (!rewardGoldPrefab)
                rewardGoldPrefab = messagePrefab;
            if (!rewardItemPrefab)
                rewardItemPrefab = messagePrefab;
            if (!rewardCurrencyPrefab)
                rewardCurrencyPrefab = messagePrefab;
        }

        protected virtual void OnEnable()
        {
            ClientGenericActions.onClientReceiveGameMessage += OnReceiveGameMessage;
            ClientGenericActions.onClientReceiveFormattedGameMessage += OnReceiveFormattedGameMessage;
            ClientGenericActions.onNotifyRewardExp += OnNotifyRewardExp;
            ClientGenericActions.onNotifyRewardGold += OnNotifyRewardGold;
            ClientGenericActions.onNotifyRewardItem += OnNotifyRewardItem;
            ClientGenericActions.onNotifyRewardCurrency += OnNotifyRewardCurrency;
        }

        protected virtual void OnDisable()
        {
            ClientGenericActions.onClientReceiveGameMessage -= OnReceiveGameMessage;
            ClientGenericActions.onClientReceiveFormattedGameMessage -= OnReceiveFormattedGameMessage;
            ClientGenericActions.onNotifyRewardExp -= OnNotifyRewardExp;
            ClientGenericActions.onNotifyRewardGold -= OnNotifyRewardGold;
            ClientGenericActions.onNotifyRewardItem -= OnNotifyRewardItem;
            ClientGenericActions.onNotifyRewardCurrency -= OnNotifyRewardCurrency;
        }

        private void OnReceiveGameMessage(UITextKeys message)
        {
            if (messagePrefab == null)
                return;
            float time = Time.unscaledTime;
            if (_lastMessage == message && time - _lastMessagedTime < visibleDuration)
                return;
            _lastMessage = message;
            _lastMessagedTime = time;
            TextWrapper newMessage = AddMessage(messagePrefab);
            if (message.ToString().ToUpper().StartsWith("UI_ERROR"))
                newMessage.color = errorMessageColor;
            newMessage.text = LanguageManager.GetText(message.ToString());
        }

        private void OnReceiveFormattedGameMessage(UIFormatKeys format, string[] args)
        {
            if (messagePrefab == null)
                return;
            TextWrapper newMessage = AddMessage(messagePrefab);
            newMessage.text = string.Format(LanguageManager.GetText(format.ToString()), args);
        }

        private void OnNotifyRewardExp(RewardGivenType givenType, int exp)
        {
            if (rewardExpPrefab == null)
                return;

            TextWrapper newMessage = AddMessage(rewardExpPrefab);
            newMessage.text = ZString.Format(LanguageManager.GetText(formatKeyRewardExp.ToString()), exp);
        }

        private void OnNotifyRewardGold(RewardGivenType givenType, int gold)
        {
            if (rewardGoldPrefab == null)
                return;

            TextWrapper newMessage = AddMessage(rewardGoldPrefab);
            newMessage.text = ZString.Format(LanguageManager.GetText(formatKeyRewardGold.ToString()), gold);
        }

        private void OnNotifyRewardItem(RewardGivenType givenType, int dataId, int amount)
        {
            BaseItem item;
            if (rewardItemPrefab == null || !GameInstance.Items.TryGetValue(dataId, out item))
                return;

            TextWrapper newMessage = AddMessage(rewardItemPrefab);
            newMessage.text = ZString.Format(LanguageManager.GetText(formatKeyRewardItem.ToString()), item.Title, amount);
        }

        private void OnNotifyRewardCurrency(RewardGivenType givenType, int dataId, int amount)
        {
            Currency currency;
            if (rewardCurrencyPrefab == null || !GameInstance.Currencies.TryGetValue(dataId, out currency))
                return;

            TextWrapper newMessage = AddMessage(rewardCurrencyPrefab);
            newMessage.text = ZString.Format(LanguageManager.GetText(formatKeyRewardCurrency.ToString()), currency.Title, amount);
        }

        public TextWrapper AddMessage(TextWrapper prefab)
        {
            TextWrapper newMessage = Instantiate(prefab);
            newMessage.transform.SetParent(messageContainer);
            newMessage.transform.localScale = Vector3.one;
            newMessage.transform.localRotation = Quaternion.identity;
            Destroy(newMessage.gameObject, visibleDuration);
            return newMessage;
        }
    }
}
