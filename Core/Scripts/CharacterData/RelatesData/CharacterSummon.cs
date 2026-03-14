using LiteNetLib.Utils;
using LiteNetLibManager;
using Newtonsoft.Json;

namespace MultiplayerARPG
{
    public partial struct CharacterSummon
    {
        [JsonIgnore]
        public BaseMonsterCharacterEntity CacheEntity
        {
            set => MemoryManager.CharacterSummons.SetEntity(in this, value);
            get => MemoryManager.CharacterSummons.GetEntity(in this);
        }
        [JsonIgnore]
        public int Level { get { return CacheEntity != null ? CacheEntity.Level : level; } }
        [JsonIgnore]
        public int Exp { get { return CacheEntity != null ? CacheEntity.Exp : exp; } }
        [JsonIgnore]
        public int CurrentHp { get { return CacheEntity != null ? CacheEntity.CurrentHp : currentHp; } }
        [JsonIgnore]
        public int CurrentMp { get { return CacheEntity != null ? CacheEntity.CurrentMp : currentMp; } }

        public void Summon(BaseCharacterEntity summoner, int summonLevel, float duration)
        {
            LiteNetLibIdentity spawnObj;
            if (GetPrefab(out BaseMonsterCharacterEntity prefab
#if !DISABLE_ADDRESSABLES
                , out AssetReferenceBaseMonsterCharacterEntity addressablePrefab
#endif
                ))
            {
#if !DISABLE_ADDRESSABLES
                spawnObj = BaseGameNetworkManager.Singleton.Assets.GetObjectInstance(
                    addressablePrefab.HashAssetId,
                    GameInstance.Singleton.GameplayRule.GetSummonPosition(summoner),
                    GameInstance.Singleton.GameplayRule.GetSummonRotation(summoner));
                CacheEntity = spawnObj.GetComponent<BaseMonsterCharacterEntity>();
#else
                spawnObj = null;
#endif
            }
            else if (prefab != null)
            {
                spawnObj = BaseGameNetworkManager.Singleton.Assets.GetObjectInstance(
                    prefab.Identity.HashAssetId,
                    GameInstance.Singleton.GameplayRule.GetSummonPosition(summoner),
                    GameInstance.Singleton.GameplayRule.GetSummonRotation(summoner));
                CacheEntity = spawnObj.GetComponent<BaseMonsterCharacterEntity>();
            }
            else
            {
                return;
            }

            BaseGameNetworkManager.Singleton.Assets.NetworkSpawn(spawnObj);
            CacheEntity.Summon(summoner, type, summonLevel);
            objectId = CacheEntity.ObjectId;
            summonRemainsDuration = duration;
            level = summonLevel;
        }

        public void Summon(BaseCharacterEntity summoner, int summonLevel, float duration, int summonExp)
        {
            Summon(summoner, summonLevel, duration);
            CacheEntity.Exp = summonExp;
            exp = summonExp;
        }

        public void Summon(BaseCharacterEntity summoner, int summonLevel, float duration, int summonExp, int summonCurrentHp, int summonCurrentMp)
        {
            Summon(summoner, summonLevel, duration, summonExp);
            CacheEntity.CurrentHp = summonCurrentHp;
            CacheEntity.CurrentMp = summonCurrentMp;
            currentHp = summonCurrentHp;
            currentMp = summonCurrentMp;

        }

        public void UnSummon(BaseCharacterEntity summoner)
        {
            switch (type)
            {
                case SummonType.PetItem:
                    int indexOfItem = summoner.NonEquipItems.IndexOf(sourceId);
                    if (indexOfItem >= 0)
                    {
                        CharacterItem item = summoner.NonEquipItems[indexOfItem];
                        item.Lock(CurrentHp <= 0 ?
                            GameInstance.Singleton.petDeadLockDuration :
                            GameInstance.Singleton.petUnSummonLockDuration);
                        summoner.NonEquipItems[indexOfItem] = item;
                    }
                    break;
                case SummonType.Custom:
                    GameInstance.CustomSummonManager.UnSummon(this, summoner);
                    break;
            }

            if (CacheEntity)
                CacheEntity.UnSummon();
        }

        public BaseSkill GetSkill()
        {
            return MemoryManager.CharacterSummons.GetSkill(in this);
        }

        public IPetItem GetPetItem()
        {
            return MemoryManager.CharacterSummons.GetPetItem(in this);
        }

        /// <summary>
        /// Return `TRUE` if it is addressable
        /// </summary>
        /// <param name="prefab"></param>
        /// <param name="addressablePrefab"></param>
        /// <returns></returns>
        public bool GetPrefab(out BaseMonsterCharacterEntity prefab
#if !DISABLE_ADDRESSABLES
            , out AssetReferenceBaseMonsterCharacterEntity addressablePrefab
#endif
            )
        {
            return MemoryManager.CharacterSummons.GetPrefab(in this, out prefab
#if !DISABLE_ADDRESSABLES
                , out addressablePrefab
#endif
                );
        }

        public CalculatedBuff GetBuff()
        {
            return MemoryManager.CharacterSummons.GetBuff(in this);
        }

        public bool ShouldRemove(ICharacterData characterData)
        {
            if (CacheEntity && CacheEntity.CurrentHp <= 0)
                return true;
            switch (type)
            {
                case SummonType.Skill:
                    BaseSkill skill = GetSkill();
                    if (skill == null || !skill.TryGetSummon(out SkillSummon summon))
                        return true;
                    if (summon.NoDuration)
                        return false;
                    return summonRemainsDuration <= 0f;
                case SummonType.PetItem:
                    if (!string.IsNullOrWhiteSpace(sourceId))
                    {
                        int tempIndexOfItem = characterData.IndexOfNonEquipItem(sourceId);
                        if (tempIndexOfItem < 0)
                            return true;
                    }
                    IPetItem petItem = GetPetItem();
                    if (petItem == null)
                        return true;
                    if (petItem.NoSummonDuration)
                        return false;
                    return summonRemainsDuration <= 0f;
                case SummonType.Custom:
                    return GameInstance.CustomSummonManager.ShouldRemove(this, characterData);
            }
            return false;
        }

        public void Update(BaseCharacterEntity summoner, float deltaTime)
        {
            if (summonRemainsDuration > 0f)
            {
                summonRemainsDuration -= deltaTime;
                if (summonRemainsDuration < 0f)
                    summonRemainsDuration = 0f;
            }
            // Makes update in main thread to collects data to use in other threads (save to database thread)
            level = Level;
            exp = Exp;
            currentHp = CurrentHp;
            currentMp = CurrentMp;
            // Update data to item in inventory
            switch (type)
            {
                case SummonType.PetItem:
                    int indexOfItem = summoner.NonEquipItems.IndexOf(sourceId);
                    if (indexOfItem >= 0)
                    {
                        CharacterItem item = summoner.NonEquipItems[indexOfItem];
                        item.level = level;
                        item.exp = exp;
                        summoner.NonEquipItems[indexOfItem] = item;
                    }
                    break;
            }
        }
    }

    [System.Serializable]
    public class SyncListCharacterSummon : LiteNetLibSyncList<CharacterSummon>
    {
        protected override CharacterSummon DeserializeValueForSetOrDirty(int index, NetDataReader reader)
        {
            CharacterSummon result = this[index];
            result.summonRemainsDuration = reader.GetFloat();
            result.objectId = reader.GetPackedUInt();
            return result;
        }

        protected override void SerializeValueForSetOrDirty(int index, NetDataWriter writer, CharacterSummon value)
        {
            writer.Put(value.summonRemainsDuration);
            writer.PutPackedUInt(value.objectId);
        }
    }
}
