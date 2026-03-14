using UnityEngine;

namespace MultiplayerARPG
{
    public class AutoUsePotion : MonoBehaviour
    {
        public enum ConditionType
        {
            Hp,
            Mp,
            Stamina,
            Food,
            Water,
        }
        public ConditionType conditionType;
        [Range(0.01f, 1f)]
        public float useWhenRateLessThan = 0.5f;
        [Tooltip("The same key with `PotionItem` -> `AutoUseSettingKey` setting")]
        public string key = "HpPotion";

        private float useCooldown = 0f;

        private void Update()
        {
            if (GameInstance.PlayingCharacterEntity == null)
                return;
            useCooldown -= Time.unscaledDeltaTime;
            if (useCooldown > 0f)
                return;
            float rate = 1f;
            switch (conditionType)
            {
                case ConditionType.Hp:
                    rate = (float)GameInstance.PlayingCharacterEntity.CurrentHp / (float)GameInstance.PlayingCharacterEntity.MaxHp;
                    break;
                case ConditionType.Mp:
                    rate = (float)GameInstance.PlayingCharacterEntity.CurrentMp / (float)GameInstance.PlayingCharacterEntity.MaxMp;
                    break;
                case ConditionType.Stamina:
                    rate = (float)GameInstance.PlayingCharacterEntity.CurrentStamina / (float)GameInstance.PlayingCharacterEntity.MaxStamina;
                    break;
                case ConditionType.Food:
                    rate = (float)GameInstance.PlayingCharacterEntity.CurrentFood / (float)GameInstance.PlayingCharacterEntity.MaxFood;
                    break;
                case ConditionType.Water:
                    rate = (float)GameInstance.PlayingCharacterEntity.CurrentWater / (float)GameInstance.PlayingCharacterEntity.MaxWater;
                    break;
            }
            if (rate < useWhenRateLessThan)
            {
                IPotionItem potionItem;
                for (int i = 0; i < GameInstance.PlayingCharacterEntity.NonEquipItems.Count; ++i)
                {
                    potionItem = GameInstance.PlayingCharacterEntity.NonEquipItems[i].GetPotionItem();
                    if (potionItem == null || !key.Equals(potionItem.AutoUseKey))
                        continue;
                    if (GameInstance.PlayingCharacterEntity.CallCmdUseItem(i))
                    {
                        useCooldown = potionItem.UseItemCooldown;
                        return;
                    }
                }
            }
        }
    }
}
