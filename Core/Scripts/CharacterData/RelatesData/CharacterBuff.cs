using LiteNetLib.Utils;
using LiteNetLibManager;
using Newtonsoft.Json;

namespace MultiplayerARPG
{
    public partial struct CharacterBuff
    {
        [JsonIgnore]
        public EntityInfo BuffApplier => MemoryManager.CharacterBuffs.GetBuffApplier(in this);
        [JsonIgnore]
        public CharacterItem BuffApplierWeapon => MemoryManager.CharacterBuffs.GetBuffApplierWeapon(in this);

        public BaseSkill GetSkill()
        {
            return MemoryManager.CharacterBuffs.GetSkill(in this);
        }

        public BaseItem GetItem()
        {
            return MemoryManager.CharacterBuffs.GetItem(in this);
        }

        public GuildSkill GetGuildSkill()
        {
            return  MemoryManager.CharacterBuffs.GetGuildSkill(in this);
        }

        public StatusEffect GetStatusEffect()
        {
            return  MemoryManager.CharacterBuffs.GetStatusEffect(in this);
        }

        public CalculatedBuff GetBuff()
        {
            return MemoryManager.CharacterBuffs.GetBuff(in this);
        }

        public string GetKey()
        {
            return MemoryManager.CharacterBuffs.GetKey(in this);
        }

        public void SetApplier(EntityInfo buffApplier, CharacterItem buffApplierWeapon)
        {
            MemoryManager.CharacterBuffs.SetApplier(in this, buffApplier, buffApplierWeapon);
        }

        public bool ShouldRemove()
        {
            return !GetBuff().NoDuration() && buffRemainsDuration <= 0f;
        }

        public void Apply(EntityInfo buffApplier, CharacterItem buffApplierWeapon)
        {
            SetApplier(buffApplier, buffApplierWeapon);
            buffRemainsDuration = GetBuff().GetDuration();
        }

        public void Update(float deltaTime)
        {
            if (GetBuff().NoDuration())
                return;
            buffRemainsDuration -= deltaTime;
        }
    }

    [System.Serializable]
    public class SyncListCharacterBuff : LiteNetLibSyncList<CharacterBuff>
    {
        protected override CharacterBuff DeserializeValueForSetOrDirty(int index, NetDataReader reader)
        {
            CharacterBuff result = this[index];
            result.buffRemainsDuration = reader.GetFloat();
            return result;
        }

        protected override void SerializeValueForSetOrDirty(int index, NetDataWriter writer, CharacterBuff value)
        {
            writer.Put(value.buffRemainsDuration);
        }
    }
}
