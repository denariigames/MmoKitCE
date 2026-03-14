using Cysharp.Text;
using System.Collections.Generic;
using UnityEngine;

namespace MultiplayerARPG
{
    public class UIEquipmentSockets : UIBaseEquipmentBonus<UIEquipmentSocketsData>
    {
        [Header("String Formats")]
        [Tooltip("Format => {0} = {Socket Index}, {1} = {Item Title}, {2} = {List Of Bonus}")]
        public UILocaleKeySetting formatKeySocketFilled = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_EQUIPMENT_SOCKET_FILLED);
        [Tooltip("Format => {0} = {Socket Index}")]
        public UILocaleKeySetting formatKeySocketEmpty = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_EQUIPMENT_SOCKET_EMPTY);

        [Header("UI Elements")]
        public UICharacterItems uiAppliedSocketEnhancerItems;

        protected override void OnDestroy()
        {
            base.OnDestroy();
            uiAppliedSocketEnhancerItems = null;
        }

        protected override void UpdateData()
        {
            List<CharacterItem> appliedSocketEnhancerItems = new List<CharacterItem>();

            using (Utf16ValueStringBuilder allBonusText = ZString.CreateStringBuilder(false))
            {
                BaseItem tempItem;
                string tempText;
                for (int i = 0; i < Data.availableSocketEnhancerTypes.Length; ++i)
                {
                    if (i < Data.sockets.Count && GameInstance.Items.TryGetValue(Data.sockets[i], out tempItem) && tempItem.IsSocketEnhancer())
                    {
                        appliedSocketEnhancerItems.Add(CharacterItem.Create(Data.sockets[i]));
                        tempText = GetEquipmentBonusText((tempItem as ISocketEnhancerItem).SocketEnhanceEffect);
                        if (allBonusText.Length > 0)
                            allBonusText.Append('\n');
                        allBonusText.AppendFormat(
                            LanguageManager.GetText(formatKeySocketFilled),
                            i + 1,
                            tempItem.Title,
                            tempText);
                    }
                    else
                    {
                        appliedSocketEnhancerItems.Add(CharacterItem.CreateEmptySlot());
                        if (allBonusText.Length > 0)
                            allBonusText.Append('\n');
                        allBonusText.AppendFormat(
                            LanguageManager.GetText(formatKeySocketEmpty),
                            i + 1);
                    }
                }

                if (uiTextAllBonus != null)
                {
                    uiTextAllBonus.SetGameObjectActive(allBonusText.Length > 0);
                    uiTextAllBonus.text = allBonusText.ToString();
                }
            }

            if (uiAppliedSocketEnhancerItems != null)
            {
                uiAppliedSocketEnhancerItems.inventoryType = InventoryType.Unknow;
                uiAppliedSocketEnhancerItems.CacheSelectionManager.selectionMode = UISelectionMode.SelectSingle;
                uiAppliedSocketEnhancerItems.UpdateData(GameInstance.PlayingCharacter, appliedSocketEnhancerItems);
            }
        }
    }
}
