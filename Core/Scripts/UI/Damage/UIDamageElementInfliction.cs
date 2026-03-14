using Cysharp.Text;
using UnityEngine;

namespace MultiplayerARPG
{
    public partial class UIDamageElementInfliction : UISelectionEntry<UIDamageElementInflictionData>
    {
        [Header("String Formats")]
        [Tooltip("Format => {1} = {Infliction * 100}")]
        public UILocaleKeySetting formatKeyInfliction = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_DAMAGE_INFLICTION);
        [Tooltip("Format => {0} = {Damage Element Title}, {1} = {Infliction * 100}")]
        public UILocaleKeySetting formatKeyInflictionAsElemental = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_DAMAGE_INFLICTION_AS_ELEMENTAL);

        [Header("UI Elements")]
        public TextWrapper uiTextInfliction;

        protected override void OnDestroy()
        {
            base.OnDestroy();
            uiTextInfliction = null;
        }

        protected override void UpdateData()
        {
            if (uiTextInfliction != null)
            {
                DamageElement element = Data.damageElement;
                if (element == null || element == GameInstance.Singleton.DefaultDamageElement)
                {
                    uiTextInfliction.text = ZString.Format(
                        LanguageManager.GetText(formatKeyInfliction),
                        (Data.infliction * 100f).ToString("N0"));
                }
                else
                {
                    uiTextInfliction.text = ZString.Format(
                        LanguageManager.GetText(formatKeyInflictionAsElemental),
                        element.Title,
                        (Data.infliction * 100f).ToString("N0"));
                }
            }
        }
    }
}
