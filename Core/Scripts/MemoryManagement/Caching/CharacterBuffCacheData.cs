using Cysharp.Text;

namespace MultiplayerARPG
{
    public class CharacterBuffCacheData : BaseCacheData<CharacterBuff>
    {
        private BuffType _type;
        private int _dataId;
        private int _level;

        private string _cacheKey;
        private CalculatedBuff _cacheBuff = null;
        private bool _recachingBuff = false;

        public EntityInfo BuffApplier { get; private set; }
        public CharacterItem BuffApplierWeapon { get; private set; }

        public string MakeKey(BuffType type, int dataId)
        {
            return ZString.Concat((byte)type, '_', dataId);
        }

        public override BaseCacheData<CharacterBuff> Prepare(in CharacterBuff source)
        {
            base.Prepare(in source);
            if (source.type == _type && source.dataId == _dataId && source.level == _level)
                return this;
            _type = source.type;
            _dataId = source.dataId;
            _level = source.level;
            _cacheKey = MakeKey(_type, _dataId);
            _recachingBuff = true;
            return this;
        }

        public override void Clear()
        {
            _cacheKey = null;
            _cacheBuff = null;
        }

        public BaseSkill GetSkill()
        {
            if (_type != BuffType.SkillBuff && _type != BuffType.SkillDebuff)
                return null;
            if (GameInstance.Skills.TryGetValue(_dataId, out BaseSkill skill))
                return skill;
            return null;
        }

        public BaseItem GetItem()
        {
            if (_type != BuffType.PotionBuff)
                return null;
            if (GameInstance.Items.TryGetValue(_dataId, out BaseItem item))
                return item;
            return null;
        }

        public GuildSkill GetGuildSkill()
        {
            if (_type != BuffType.GuildSkillBuff)
                return null;
            if (GameInstance.GuildSkills.TryGetValue(_dataId, out GuildSkill guildSkill))
                return guildSkill;
            return null;
        }

        public StatusEffect GetStatusEffect()
        {
            if (_type != BuffType.StatusEffect)
                return null;
            if (GameInstance.StatusEffects.TryGetValue(_dataId, out StatusEffect statusEffect))
                return statusEffect;
            return null;
        }

        public CalculatedBuff GetBuff()
        {
            if (_cacheBuff == null)
                _cacheBuff = new CalculatedBuff();
            if (!_recachingBuff)
                return _cacheBuff;
            _recachingBuff = false;
            Buff tempBuff = null;
            BaseSkill tempSkill;
            BaseItem tempItem;
            GuildSkill tempGuildSkill;
            StatusEffect tempStatusEffect;
            switch (_type)
            {
                case BuffType.SkillBuff:
                    tempSkill = GetSkill();
                    if (tempSkill.TryGetBuff(out Buff buff))
                        tempBuff = buff;
                    break;
                case BuffType.SkillDebuff:
                    tempSkill = GetSkill();
                    if (tempSkill.TryGetDebuff(out Buff debuff))
                        tempBuff = debuff;
                    break;
                case BuffType.PotionBuff:
                    tempItem = GetItem();
                    if (tempItem.IsPotion())
                        tempBuff = (tempItem as IPotionItem).BuffData;
                    break;
                case BuffType.GuildSkillBuff:
                    tempGuildSkill = GetGuildSkill();
                    if (tempGuildSkill != null)
                        tempBuff = tempGuildSkill.Buff;
                    break;
                case BuffType.StatusEffect:
                    tempStatusEffect = GetStatusEffect();
                    if (tempStatusEffect != null)
                        tempBuff = tempStatusEffect.Buff;
                    break;
            }
            _cacheBuff.Build(tempBuff, _level);
            return _cacheBuff;
        }

        public string GetKey()
        {
            return _cacheKey;
        }

        public void SetApplier(EntityInfo buffApplier, CharacterItem buffApplierWeapon)
        {
            BuffApplier = buffApplier;
            BuffApplierWeapon = buffApplierWeapon;
        }
    }
}