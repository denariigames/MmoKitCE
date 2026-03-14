using Insthync.AddressableAssetTools;
using Insthync.UnityEditorUtils;
using UnityEngine;
#if !DISABLE_ADDRESSABLES
using UnityEngine.AddressableAssets;
#endif

namespace MultiplayerARPG
{
    public abstract class BaseAreaSkill : BaseSkill
    {
        [Category("Skill Casting")]
        public IncrementalFloat castDistance;

        [Category(2, "Area Settings")]
        public IncrementalFloat areaDuration;
        public IncrementalFloat applyDuration;
        [Tooltip("If this is `TRUE`, it will use `targetDetectionLayerMask`, not game instance's `GetTargetLayerMask()`")]
        public bool overrideTargetDetectionLayerMask;
        public LayerMask targetDetectionLayerMask;
        [Tooltip("If this is `TRUE`, it will use `groundDetectionLayerMask`, not game instance's `GetAreaSkillGroundDetectionLayerMask()`")]
        public bool overrideGroundDetectionLayerMask;
        public LayerMask groundDetectionLayerMask;

        public int TargetDetectionLayerMask
        {
            get
            {
                if (overrideTargetDetectionLayerMask)
                    return targetDetectionLayerMask.value;
                return GameInstance.Singleton.GetTargetLayerMask();
            }
        }

        public int GroundDetectionLayerMask
        {
            get
            {
                if (overrideGroundDetectionLayerMask)
                    return groundDetectionLayerMask.value;
                return GameInstance.Singleton.GetAreaSkillGroundDetectionLayerMask();
            }
        }

#if UNITY_EDITOR || !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
        [SerializeField]
#if !DISABLE_ADDRESSABLES
        [AddressableAssetConversion(nameof(addressableTargetObjectPrefab))]
#endif
        private GameObject targetObjectPrefab;
#endif
        public GameObject TargetObjectPrefab
        {
            get
            {
#if !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
                return targetObjectPrefab;
#else
                return null;
#endif
            }
        }

#if !DISABLE_ADDRESSABLES
        [SerializeField]
        private AssetReferenceGameObject addressableTargetObjectPrefab;
        public AssetReferenceGameObject AddressableTargetObjectPrefab
        {
            get { return addressableTargetObjectPrefab; }
        }
#endif

        public override SkillType SkillType
        {
            get { return SkillType.Active; }
        }

        public override float GetCastDistance(BaseCharacterEntity skillUser, int skillLevel, bool isLeftHand)
        {
            return castDistance.GetAmount(skillLevel);
        }

        public override float GetCastFov(BaseCharacterEntity skillUser, int skillLevel, bool isLeftHand)
        {
            return 360f;
        }

        public override bool HasCustomAimControls()
        {
            return true;
        }

        public override AimPosition UpdateAimControls(Vector2 aimAxes, params object[] data)
        {
            return BasePlayerCharacterController.Singleton.AreaSkillAimController.UpdateAimControls(aimAxes, this, (int)data[0]);
        }

        public override void FinishAimControls(bool isCancel)
        {
            BasePlayerCharacterController.Singleton.AreaSkillAimController.FinishAimControls(isCancel);
        }

        public override Vector3 GetDefaultAttackAimPosition(BaseCharacterEntity skillUser, int skillLevel, bool isLeftHand, IDamageableEntity target)
        {
            return target.Entity.MovementTransform.position;
        }
    }
}
