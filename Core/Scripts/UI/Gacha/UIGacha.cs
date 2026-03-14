using System.Collections;
using System.Collections.Generic;
using Cysharp.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace MultiplayerARPG
{
    public class UIGacha : UISelectionEntry<Gacha>
    {
        [Header("String Formats")]
        [Tooltip("Format => {0} = {Title}")]
        public UILocaleKeySetting formatKeyTitle = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_SIMPLE);
        [Tooltip("Format => {0} = {Description}")]
        public UILocaleKeySetting formatKeyDescription = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_SIMPLE);
        [Tooltip("Format => {0} = {Open Price}")]
        public UILocaleKeySetting formatKeyOpenPriceCash = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_SELL_PRICE);

        [Header("UI Elements")]
        public UIGachas uiGachas;
        public TextWrapper uiTextTitle;
        public TextWrapper uiTextDescription;
        public TextWrapper uiTextSingleModeOpenPrice;
        public TextWrapper uiTextMultipleModeOpenPrice;
        public Image imageIcon;
        public RawImage rawImageExternalIcon;

        protected override void OnDestroy()
        {
            base.OnDestroy();
            uiGachas = null;
            uiTextTitle = null;
            uiTextDescription = null;
            uiTextSingleModeOpenPrice = null;
            uiTextMultipleModeOpenPrice = null;
            imageIcon = null;
            rawImageExternalIcon = null;
            _data = null;
        }

        protected override void UpdateData()
        {
            if (uiTextTitle != null)
            {
                uiTextTitle.text = ZString.Format(
                    LanguageManager.GetText(formatKeyTitle),
                    Data == null || string.IsNullOrEmpty(Data.Title) ? LanguageManager.GetUnknowTitle() : Data.Title);
            }

            if (uiTextDescription != null)
            {
                uiTextDescription.text = ZString.Format(
                    LanguageManager.GetText(formatKeyDescription),
                    Data == null || string.IsNullOrEmpty(Data.Description) ? LanguageManager.GetUnknowDescription() : Data.Description);
            }

            imageIcon.SetImageGameDataIcon(Data);

#if UNITY_EDITOR || !UNITY_SERVER
            if (rawImageExternalIcon != null)
            {
                rawImageExternalIcon.gameObject.SetActive(Data != null && !string.IsNullOrEmpty(Data.ExternalIconUrl));
                if (Data != null && !string.IsNullOrEmpty(Data.ExternalIconUrl))
                    StartCoroutine(LoadExternalIcon());
            }
#endif

            if (uiTextSingleModeOpenPrice != null)
            {
                uiTextSingleModeOpenPrice.text = ZString.Format(
                    LanguageManager.GetText(formatKeyOpenPriceCash),
                    Data == null ? LanguageManager.GetUnknowTitle() : Data.SingleModeOpenPrice.ToString("N0"));
            }

            if (uiTextMultipleModeOpenPrice != null)
            {
                uiTextMultipleModeOpenPrice.text = ZString.Format(
                    LanguageManager.GetText(formatKeyOpenPriceCash),
                    Data == null ? LanguageManager.GetUnknowTitle() : Data.MultipleModeOpenPrice.ToString("N0"));
            }
        }

        IEnumerator LoadExternalIcon()
        {
            UnityWebRequest www = UnityWebRequestTexture.GetTexture(Data.ExternalIconUrl);
            yield return www.SendWebRequest();
            if (!www.IsError())
                rawImageExternalIcon.texture = ((DownloadHandlerTexture)www.downloadHandler).texture;
        }

        public void OnClickOpenSingle()
        {
            if (uiGachas != null)
                uiGachas.Buy(Data.DataId, GachaOpenMode.Single);
        }

        public void OnClickOpenMultiple()
        {
            if (uiGachas != null)
                uiGachas.Buy(Data.DataId, GachaOpenMode.Multiple);
        }
    }
}
