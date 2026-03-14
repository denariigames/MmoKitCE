using Insthync.AddressableAssetTools;
using Insthync.UnityEditorUtils;
using System.Collections.Generic;
using UnityEngine;

namespace MultiplayerARPG
{
    [CreateAssetMenu(fileName = GameDataMenuConsts.PET_ITEM_FILE, menuName = GameDataMenuConsts.PET_ITEM_MENU, order = GameDataMenuConsts.PET_ITEM_ORDER)]
    public partial class PetItem : BaseItem, IPetItem
    {
        public override string TypeTitle
        {
            get { return LanguageManager.GetText(UIItemTypeKeys.UI_ITEM_TYPE_PET.ToString()); }
        }

        public override ItemType ItemType
        {
            get { return ItemType.Pet; }
        }

        [Category(2, "Requirements")]
        [SerializeField]
        private ItemRequirement requirement = new ItemRequirement();
        public ItemRequirement Requirement
        {
            get { return requirement; }
        }

        [System.NonSerialized]
        private Dictionary<Attribute, float> _cacheRequireAttributeAmounts = null;
        public Dictionary<Attribute, float> RequireAttributeAmounts
        {
            get
            {
                if (_cacheRequireAttributeAmounts == null)
                {
                    _cacheRequireAttributeAmounts = new Dictionary<Attribute, float>();
                    GameDataHelpers.CombineAttributes(requirement.attributeAmounts, _cacheRequireAttributeAmounts, 1f);
                }
                return _cacheRequireAttributeAmounts;
            }
        }

#if UNITY_EDITOR || !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
        [Category(3, "Pet Settings")]
        [SerializeField]
#if !DISABLE_ADDRESSABLES
        [AddressableAssetConversion(nameof(addressablePetEntity))]
#endif
        private BaseMonsterCharacterEntity petEntity = null;
#endif
        public BaseMonsterCharacterEntity MonsterCharacterEntity
        {
            get
            {
#if !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
                return petEntity;
#else
                return null;
#endif
            }
        }

#if !DISABLE_ADDRESSABLES
        [SerializeField]
        private AssetReferenceBaseMonsterCharacterEntity addressablePetEntity = null;
        public AssetReferenceBaseMonsterCharacterEntity AddressableMonsterCharacterEntity
        {
            get { return addressablePetEntity; }
        }
#endif

        [SerializeField]
        private IncrementalFloat summonDuration = default;
        public IncrementalFloat SummonDuration { get { return summonDuration; } }

        [SerializeField]
        private bool noSummonDuration = true;
        public bool NoSummonDuration { get { return noSummonDuration; } }

        [SerializeField]
        private float useItemCooldown = 0f;
        public float UseItemCooldown
        {
            get { return useItemCooldown; }
        }

        public bool UseItem(BaseCharacterEntity characterEntity, int itemIndex, CharacterItem characterItem)
        {
            if (!characterEntity.CanUseItem())
                return false;
            // Clear all summoned pets
            bool doNotSummonNewOne = false;
            CharacterSummon tempSummon;
            for (int i = characterEntity.Summons.Count - 1; i >= 0; --i)
            {
                tempSummon = characterEntity.Summons[i];
                if (tempSummon.type != SummonType.PetItem)
                    continue;
                characterEntity.Summons.RemoveAt(i);
                tempSummon.UnSummon(characterEntity);
                if (!doNotSummonNewOne && string.Equals(characterItem.id, tempSummon.sourceId))
                    doNotSummonNewOne = true;
            }
            if (doNotSummonNewOne)
                return true;
            // Summon new pet
            CharacterSummon newSummon = CharacterSummon.Create(SummonType.PetItem, characterItem.id, DataId);
            newSummon.Summon(characterEntity, characterItem.level, 0f, characterItem.exp);
            characterEntity.Summons.Add(newSummon);
            return true;
        }

        public bool HasCustomAimControls()
        {
            return false;
        }

        public AimPosition UpdateAimControls(Vector2 aimAxes, params object[] data)
        {
            return default;
        }

        public void FinishAimControls(bool isCancel)
        {

        }

        public bool IsChanneledAbility()
        {
            return false;
        }

        public override void PrepareRelatesData()
        {
            base.PrepareRelatesData();
#if !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
            GameInstance.AddMonsterCharacterEntities(MonsterCharacterEntity);
#endif
#if !DISABLE_ADDRESSABLES
            GameInstance.AddAssetReferenceMonsterCharacterEntities(AddressableMonsterCharacterEntity);
#endif
        }
    }
}
