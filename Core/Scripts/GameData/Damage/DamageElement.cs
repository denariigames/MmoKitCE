using Insthync.AddressableAssetTools;
using Insthync.UnityEditorUtils;
using System.Collections.Generic;
using UnityEngine;

namespace MultiplayerARPG
{
    [CreateAssetMenu(fileName = GameDataMenuConsts.DAMAGE_ELEMENT_FILE, menuName = GameDataMenuConsts.DAMAGE_ELEMENT_MENU, order = GameDataMenuConsts.DAMAGE_ELEMENT_ORDER)]
    public partial class DamageElement : BaseGameData
    {
        [Category("Damage Element Settings")]
        [SerializeField]
        private float resistanceBattlePointScore = 5;
        public float ResistanceBattlePointScore
        {
            get { return resistanceBattlePointScore; }
        }

        [SerializeField]
        private float armorBattlePointScore = 5;
        public float ArmorBattlePointScore
        {
            get { return armorBattlePointScore; }
        }

        [SerializeField]
        private float damageBattlePointScore = 10;
        public float DamageBattlePointScore
        {
            get { return damageBattlePointScore; }
        }

        [SerializeField]
        [Range(0f, 1f)]
        private float maxResistanceAmount = 1f;
        public float MaxResistanceAmount
        {
            get { return maxResistanceAmount; }
        }

#if UNITY_EDITOR || !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
        [SerializeField]
#if !DISABLE_ADDRESSABLES
        [AddressableAssetConversion(nameof(addressableDamageHitEffects))]
#endif
        private GameEffect[] damageHitEffects = new GameEffect[0];
#endif
        public GameEffect[] DamageHitEffects
        {
            get
            {
#if !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
                return damageHitEffects;
#else
                return System.Array.Empty<GameEffect>();
#endif
            }
        }

#if !DISABLE_ADDRESSABLES
        [SerializeField]
        private AssetReferenceGameEffect[] addressableDamageHitEffects = new AssetReferenceGameEffect[0];
        public AssetReferenceGameEffect[] AddressableDamageHitEffects
        {
            get { return addressableDamageHitEffects; }
        }
#endif

        public float GetDamageReducedByResistance(Dictionary<DamageElement, float> damageReceiverResistances, Dictionary<DamageElement, float> damageReceiverArmors, float damageAmount)
        {
            return GameInstance.Singleton.GameplayRule.GetDamageReducedByResistance(damageReceiverResistances, damageReceiverArmors, damageAmount, this);
        }

        public DamageElement GenerateDefaultDamageElement(GameEffect[] defaultDamageHitEffects
#if !DISABLE_ADDRESSABLES
            , AssetReferenceGameEffect[] addressableDefaultDamageHitEffects
#endif
            )
        {
            name = GameDataConst.DEFAULT_DAMAGE_ID;
            defaultTitle = GameDataConst.DEFAULT_DAMAGE_TITLE;
            maxResistanceAmount = 1f;
#if !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
            damageHitEffects = defaultDamageHitEffects;
#endif
#if !DISABLE_ADDRESSABLES
            addressableDamageHitEffects = addressableDefaultDamageHitEffects;
#endif
            return this;
        }
    }
}
