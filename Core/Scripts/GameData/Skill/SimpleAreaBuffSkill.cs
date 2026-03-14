using Insthync.AddressableAssetTools;
using Insthync.UnityEditorUtils;
using LiteNetLibManager;
using System.Collections.Generic;
using UnityEngine;

namespace MultiplayerARPG
{
    [CreateAssetMenu(fileName = GameDataMenuConsts.SIMPLE_AREA_BUFF_SKILL_FILE, menuName = GameDataMenuConsts.SIMPLE_AREA_BUFF_SKILL_MENU, order = GameDataMenuConsts.SIMPLE_AREA_BUFF_SKILL_ORDER)]
    public partial class SimpleAreaBuffSkill : BaseAreaSkill
    {
#if UNITY_EDITOR || !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
        [Category("Area Settings")]
        [SerializeField]
#if !DISABLE_ADDRESSABLES
        [AddressableAssetConversion(nameof(addressableAreaBuffEntity))]
#endif
        private AreaBuffEntity areaBuffEntity;
#endif
        public AreaBuffEntity AreaBuffEntity
        {
            get
            {
#if !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
                return areaBuffEntity;
#else
                return null;
#endif
            }
        }

#if !DISABLE_ADDRESSABLES
        [SerializeField]
        private AssetReferenceAreaBuffEntity addressableAreaBuffEntity;
        public AssetReferenceAreaBuffEntity AddressableAreaBuffEntity
        {
            get { return addressableAreaBuffEntity; }
        }
#endif

        [Category(3, "Buff")]
        public Buff buff;
        [Tooltip("If this is `TRUE` buffs will applies to everyone including with an enemies")]
        public bool applyBuffToEveryone;

        [Category(4, "Warp Settings")]
        public bool isWarpToAimPosition;

        protected override void ApplySkillImplement(
            BaseCharacterEntity skillUser,
            int skillLevel,
            WeaponHandlingState weaponHandlingState,
            CharacterItem weapon,
            int simulateSeed,
            byte triggerIndex,
            byte spreadIndex,
            List<Dictionary<DamageElement, MinMaxFloat>> damageAmounts,
            uint targetObjectId,
            AimPosition aimPosition)
        {
            if (BaseGameNetworkManager.Singleton.IsServer)
            {
                // Spawn area entity
                // Aim position type always is `Position`
                int hashAssetId = 0;
                if (AreaBuffEntity != null)
                    hashAssetId = AreaBuffEntity.Identity.HashAssetId;
#if !DISABLE_ADDRESSABLES
                else if (AddressableAreaBuffEntity.IsDataValid())
                    hashAssetId = AddressableAreaBuffEntity.HashAssetId;
#endif
                if (hashAssetId == 0)
                {
                    Logging.LogError("SimpleAreaBuffSkill", $"Unable to spawn area buff entity, skill ID: {Id}");
                    return;
                }
                LiteNetLibIdentity spawnObj = BaseGameNetworkManager.Singleton.Assets.GetObjectInstance(
                    hashAssetId,
                    aimPosition.position,
                    GameInstance.Singleton.GameplayRule.GetSummonRotation(skillUser));
                AreaBuffEntity entity = spawnObj.GetComponent<AreaBuffEntity>();
                entity.Setup(skillUser.GetInfo(), this, skillLevel, applyBuffToEveryone, areaDuration.GetAmount(skillLevel), applyDuration.GetAmount(skillLevel));
                BaseGameNetworkManager.Singleton.Assets.NetworkSpawn(spawnObj);
            }
            // Teleport to aim position
            if (isWarpToAimPosition)
                skillUser.Teleport(aimPosition.position, skillUser.MovementTransform.rotation, false);
        }

        public override void PrepareRelatesData()
        {
            base.PrepareRelatesData();
#if !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
            areaBuffEntity.InitPrefab();
            GameInstance.AddOtherNetworkObjects(areaBuffEntity.Identity);
#endif
        }

        public override bool TryGetBuff(out Buff buff)
        {
            buff = this.buff;
            return true;
        }
    }
}
