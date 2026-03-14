using Cysharp.Text;
using UnityEngine;

namespace MultiplayerARPG
{
    public class UIRewarding : UISelectionEntry<UIRewardingData>
    {
        [Tooltip("Format => {0} = {Exp Amount}")]
        public UILocaleKeySetting formatKeyRewardExp = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_REWARD_EXP);
        [Tooltip("Format => {0} = {Gold Amount}")]
        public UILocaleKeySetting formatKeyRewardGold = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_REWARD_GOLD);
        [Tooltip("Format => {0} = {Cash Amount}")]
        public UILocaleKeySetting formatKeyRewardCash = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_REWARD_CASH);
        
        public GameObject textRewardExpRoot;
        public TextWrapper textRewardExp;
        public GameObject textRewardGoldRoot;
        public TextWrapper textRewardGold;
        public GameObject textRewardCashRoot;
        public TextWrapper textRewardCash;
        public UICharacterItems uiRewardItems;

        protected override void OnDestroy()
        {
            base.OnDestroy();
            textRewardExpRoot = null;
            textRewardExp = null;
            textRewardGoldRoot = null;
            textRewardGold = null;
            textRewardCashRoot = null;
            textRewardCash = null;
            uiRewardItems = null;
        }

        protected override void UpdateData()
        {
            if (textRewardExpRoot != null)
                textRewardExpRoot.SetActive(Data.rewardExp != 0);

            if (textRewardExp != null)
            {
                textRewardExp.text = ZString.Format(
                    LanguageManager.GetText(formatKeyRewardExp),
                    Data.rewardExp.ToString("N0"));
                textRewardExp.SetGameObjectActive(Data.rewardExp != 0);
            }

            if (textRewardGoldRoot != null)
                textRewardGoldRoot.SetActive(Data.rewardGold != 0);

            if (textRewardGold != null)
            {
                textRewardGold.text = ZString.Format(
                    LanguageManager.GetText(formatKeyRewardGold),
                    Data.rewardGold.ToString("N0"));
                textRewardGold.SetGameObjectActive(Data.rewardGold != 0);
            }

            if (textRewardCashRoot != null)
                textRewardCashRoot.SetActive(Data.rewardCash != 0);

            if (textRewardCash != null)
            {
                textRewardCash.text = ZString.Format(
                    LanguageManager.GetText(formatKeyRewardCash),
                    Data.rewardCash.ToString("N0"));
                textRewardCash.SetGameObjectActive(Data.rewardCash != 0);
            }

            if (uiRewardItems != null)
            {
                if (Data.rewardItems != null && Data.rewardItems.Count > 0)
                {
                    uiRewardItems.UpdateData(GameInstance.PlayingCharacter, Data.rewardItems);
                    uiRewardItems.Show();
                }
                else
                {
                    uiRewardItems.Hide();
                }
            }
        }
    }
}
