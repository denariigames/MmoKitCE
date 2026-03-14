using Insthync.UnityEditorUtils;
using UnityEngine;

namespace MultiplayerARPG
{
    [CreateAssetMenu(fileName = GameDataMenuConsts.EQUIPMENT_SET_FILE, menuName = GameDataMenuConsts.EQUIPMENT_SET_MENU, order = GameDataMenuConsts.EQUIPMENT_SET_ORDER)]
    public partial class EquipmentSet : BaseGameData
    {
        [Category("Equipment Set Settings")]
        [SerializeField]
        private EquipmentBonus[] effects = new EquipmentBonus[0];
        public EquipmentBonus[] Effects { get { return effects; } }

        public override void PrepareRelatesData()
        {
            base.PrepareRelatesData();
            if (effects != null && effects.Length > 0)
            {
                foreach (EquipmentBonus effect in effects)
                {
                    GameInstance.AddAttributes(effect.Attributes.Keys);
                    GameInstance.AddAttributes(effect.AttributesRate.Keys);
                    GameInstance.AddDamageElements(effect.Resistances.Keys);
                    GameInstance.AddDamageElements(effect.Armors.Keys);
                    GameInstance.AddDamageElements(effect.ArmorsRate.Keys);
                    GameInstance.AddDamageElements(effect.Damages.Keys);
                    GameInstance.AddDamageElements(effect.DamagesRate.Keys);
                    GameInstance.AddSkills(effect.Skills.Keys);
                    GameInstance.AddStatusEffects(effect.StatusEffectResistances.Keys);
                }
            }
        }
    }
}
