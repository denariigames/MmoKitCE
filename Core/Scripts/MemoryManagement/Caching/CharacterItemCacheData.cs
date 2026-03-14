namespace MultiplayerARPG
{
    public class CharacterItemCacheData : BaseCacheData<CharacterItem>
    {
        private int _dataId;
        private int _level;
        private int _randomSeed;
        private byte _version;

        private CalculatedItemBuff _cacheBuff;
        private bool _recachingBuff = false;

        public override BaseCacheData<CharacterItem> Prepare(in CharacterItem source)
        {
            base.Prepare(in source);
            if (source.dataId == _dataId && source.level == _level && source.randomSeed == _randomSeed && source.level == _version)
                return this;
            _dataId = source.dataId;
            _level = source.level;
            _randomSeed = source.randomSeed;
            _version = source.version;
            _recachingBuff = true;
            return this;
        }

        public override void Clear()
        {
            _cacheBuff = null;
        }

        public BaseItem GetItem()
        {
            if (GameInstance.Items.TryGetValue(_dataId, out BaseItem item))
                return item;
            return null;
        }

        public IUsableItem GetUsableItem()
        {
            if (GameInstance.Items.TryGetValue(_dataId, out BaseItem item) && item.IsUsable())
                return item as IUsableItem;
            return null;
        }

        public IEquipmentItem GetEquipmentItem()
        {
            if (GameInstance.Items.TryGetValue(_dataId, out BaseItem item) && item.IsEquipment())
                return item as IEquipmentItem;
            return null;
        }

        public IDefendEquipmentItem GetDefendItem()
        {
            if (GameInstance.Items.TryGetValue(_dataId, out BaseItem item) && item.IsDefendEquipment())
                return item as IDefendEquipmentItem;
            return null;
        }

        public IArmorItem GetArmorItem()
        {
            if (GameInstance.Items.TryGetValue(_dataId, out BaseItem item) && item.IsArmor())
                return item as IArmorItem;
            return null;
        }

        public IWeaponItem GetWeaponItem()
        {
            if (GameInstance.Items.TryGetValue(_dataId, out BaseItem item) && item.IsWeapon())
                return item as IWeaponItem;
            return null;
        }

        public IShieldItem GetShieldItem()
        {
            if (GameInstance.Items.TryGetValue(_dataId, out BaseItem item) && item.IsShield())
                return item as IShieldItem;
            return null;
        }

        public IPotionItem GetPotionItem()
        {
            if (GameInstance.Items.TryGetValue(_dataId, out BaseItem item) && item.IsPotion())
                return item as IPotionItem;
            return null;
        }

        public IAmmoItem GetAmmoItem()
        {
            if (GameInstance.Items.TryGetValue(_dataId, out BaseItem item) && item.IsAmmo())
                return item as IAmmoItem;
            return null;
        }

        public IBuildingItem GetBuildingItem()
        {
            if (GameInstance.Items.TryGetValue(_dataId, out BaseItem item) && item.IsBuilding())
                return item as IBuildingItem;
            return null;
        }

        public IPetItem GetPetItem()
        {
            if (GameInstance.Items.TryGetValue(_dataId, out BaseItem item) && item.IsPet())
                return item as IPetItem;
            return null;
        }

        public ISocketEnhancerItem GetSocketEnhancerItem()
        {
            if (GameInstance.Items.TryGetValue(_dataId, out BaseItem item) && item.IsSocketEnhancer())
                return item as ISocketEnhancerItem;
            return null;
        }

        public IMountItem GetMountItem()
        {
            if (GameInstance.Items.TryGetValue(_dataId, out BaseItem item) && item.IsMount())
                return item as IMountItem;
            return null;
        }

        public ISkillItem GetSkillItem()
        {
            if (GameInstance.Items.TryGetValue(_dataId, out BaseItem item) && item.IsSkill())
                return item as ISkillItem;
            return null;
        }

        public CalculatedItemBuff GetBuff()
        {
            if (_cacheBuff == null)
                _cacheBuff = new CalculatedItemBuff();
            if (!_recachingBuff)
                return _cacheBuff;
            _recachingBuff = false;
            _cacheBuff.Build(GetEquipmentItem(), _level, _randomSeed, _version);
            return _cacheBuff;
        }
    }
}