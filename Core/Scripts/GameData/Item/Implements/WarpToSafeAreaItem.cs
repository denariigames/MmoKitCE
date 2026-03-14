using Insthync.UnityEditorUtils;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace MultiplayerARPG
{
    [CreateAssetMenu(fileName = GameDataMenuConsts.WARP_TO_SAFE_AREA_ITEM_FILE, menuName = GameDataMenuConsts.WARP_TO_SAFE_AREA_ITEM_MENU, order = GameDataMenuConsts.WARP_TO_SAFE_AREA_ITEM_ORDER)]
    public class WarpToSafeAreaItem : BaseItem, IUsableItem
    {
        [Category(2, "Requirements")]
        [SerializeField]
        private ItemRequirement requirement = new ItemRequirement();
        public ItemRequirement Requirement
        {
            get { return requirement; }
        }

        [SerializeField]
        private float findGroundDistance = 5f;

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

        [SerializeField]
        private float useItemCooldown = 0f;
        public float UseItemCooldown
        {
            get { return useItemCooldown; }
        }
        public override string TypeTitle
        {
            get { return LanguageManager.GetText(UIItemTypeKeys.UI_ITEM_TYPE_CONSUMABLE.ToString()); }
        }

        public override ItemType ItemType
        {
            get { return ItemType.Potion; }
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

        public bool UseItem(BaseCharacterEntity characterEntity, int itemIndex, CharacterItem characterItem)
        {
            BasePlayerCharacterEntity playerCharacterEntity = characterEntity as BasePlayerCharacterEntity;
            if (playerCharacterEntity == null)
                return false;
            if (!characterEntity.CanUseItem())
                return false;
            SafeArea[] safeAreas = FindObjectsOfType<SafeArea>();
            SafeArea randomedSafeArea = safeAreas.GetRandomObjectInArray(out _);
            if (!randomedSafeArea)
                return false;
            Vector3? foundPosition = null;
            switch (GameInstance.Singleton.DimensionType)
            {
                case DimensionType.Dimension2D:
                    Collider2D collider2d = randomedSafeArea.GetComponent<Collider2D>();
                    if (!collider2d)
                        return false;
                    Vector3 extents2d = collider2d.bounds.extents.GetXY();
                    float radius2d = Mathf.Max(extents2d.x, extents2d.y);
                    Vector3 randomedInsideCircle = Random.insideUnitCircle * radius2d;
                    foundPosition = randomedSafeArea.transform.position + randomedInsideCircle;
                    break;
                default:
                    Collider collider = randomedSafeArea.GetComponent<Collider>();
                    if (!collider)
                        return false;
                    Vector3 extents = collider.bounds.extents.GetXZ();
                    float radius = Mathf.Max(extents.x, extents.z);
                    Vector3 randomedInsideSphere = Random.insideUnitSphere * radius;
                    randomedInsideSphere.y = 0;
                    if (NavMesh.SamplePosition(randomedSafeArea.transform.position + randomedInsideSphere, out NavMeshHit nmHit, findGroundDistance, NavMesh.AllAreas))
                        foundPosition = nmHit.position;
                    break;
            }
            if (!foundPosition.HasValue)
                return false;
            if (!characterEntity.DecreaseItemsByIndex(itemIndex, 1, false))
                return false;
            characterEntity.FillEmptySlots();
            BaseGameNetworkManager.Singleton.WarpCharacter(playerCharacterEntity, string.Empty, foundPosition.Value, false, characterEntity.EntityTransform.eulerAngles);
            return true;
        }
    }
}
