using Insthync.AddressableAssetTools;

namespace MultiplayerARPG
{
    public static class AssetReferenceExtensions
    {
        /// <summary>
        /// Return `TRUE` if it is addressable
        /// </summary>
        /// <param name="summonType"></param>
        /// <param name="dataId"></param>
        /// <param name="prefab"></param>
        /// <param name="addressablePrefab"></param>
        /// <returns></returns>
        public static bool GetPrefab(this SummonType summonType, int dataId
            , out BaseMonsterCharacterEntity prefab
#if !DISABLE_ADDRESSABLES
            , out AssetReferenceBaseMonsterCharacterEntity addressablePrefab
#endif
            )
        {
            prefab = null;
#if !DISABLE_ADDRESSABLES
            addressablePrefab = null;
#endif
            switch (summonType)
            {
                case SummonType.Skill:
                    if (GameInstance.Skills.TryGetValue(dataId, out BaseSkill skill) &&
                        skill.TryGetSummon(out SkillSummon skillSummon))
                    {
                        if (skillSummon.MonsterCharacterEntity != null)
                        {
                            prefab = skillSummon.MonsterCharacterEntity;
                            return false;
                        }
#if !DISABLE_ADDRESSABLES
                        else if (skillSummon.AddressableMonsterCharacterEntity.IsDataValid())
                        {
                            addressablePrefab = skillSummon.AddressableMonsterCharacterEntity;
                            return true;
                        }
#endif
                    }
                    break;
                case SummonType.PetItem:
                    if (GameInstance.Items.TryGetValue(dataId, out BaseItem item) &&
                        item.IsPet() && item is IPetItem petItem)
                    {
                        if (petItem.MonsterCharacterEntity != null)
                        {
                            prefab = petItem.MonsterCharacterEntity;
                            return false;
                        }
#if !DISABLE_ADDRESSABLES
                        else if (petItem.AddressableMonsterCharacterEntity.IsDataValid())
                        {
                            addressablePrefab = petItem.AddressableMonsterCharacterEntity;
                            return true;
                        }
#endif
                    }
                    break;
                case SummonType.Custom:
                    return GameInstance.CustomSummonManager.GetPrefab(
                        out prefab
#if !DISABLE_ADDRESSABLES
                        , out addressablePrefab
#endif
                        );
            }
            return false;
        }

        public static int GetPrefabEntityId(this SummonType summonType, int dataId)
        {
            if (summonType.GetPrefab(dataId
                , out BaseMonsterCharacterEntity prefab
#if !DISABLE_ADDRESSABLES
                , out AssetReferenceBaseMonsterCharacterEntity addressablePrefab
#endif
                ))
            {
#if !DISABLE_ADDRESSABLES
                if (addressablePrefab.IsDataValid())
                    return addressablePrefab.HashAssetId;
#endif
                return 0;
            }
            else
            {
                if (prefab != null)
                    return prefab.EntityId;
                return 0;
            }
        }

        public static bool GetPrefab(this MountType mountType, ICharacterData characterData, string sourceId
            , out VehicleEntity prefab
#if !DISABLE_ADDRESSABLES
            , out AssetReferenceVehicleEntity addressablePrefab
#endif
            )
        {
            prefab = null;
#if !DISABLE_ADDRESSABLES
            addressablePrefab = null;
#endif
            int tempIndexOfData;
            BaseItem tempItem;
            BaseSkill tempSkill;
            CalculatedBuff tempCalculatedBuff;
            BuffMount tempBuffMount;
            switch (mountType)
            {
                case MountType.Skill:
                    if (GameInstance.Skills.TryGetValue(BaseGameData.MakeDataId(sourceId), out tempSkill) &&
                        tempSkill.TryGetMount(out SkillMount skillMount))
                    {
                        if (skillMount.MountEntity != null)
                        {
                            prefab = skillMount.MountEntity;
                            return false;
                        }
#if !DISABLE_ADDRESSABLES
                        else if (skillMount.AddressableMountEntity.IsDataValid())
                        {
                            addressablePrefab = skillMount.AddressableMountEntity;
                            return true;
                        }
#endif
                    }
                    break;
                case MountType.MountItem:
                    tempIndexOfData = characterData.IndexOfNonEquipItem(sourceId);
                    if (tempIndexOfData < 0)
                        return false;
                    tempItem = characterData.NonEquipItems[tempIndexOfData].GetItem();
                    if (tempItem.IsMount() && tempItem is IMountItem mountItem)
                    {
                        if (mountItem.VehicleEntity != null)
                        {
                            prefab = mountItem.VehicleEntity;
                            return false;
                        }
#if !DISABLE_ADDRESSABLES
                        else if (mountItem.AddressableVehicleEntity.IsDataValid())
                        {
                            addressablePrefab = mountItem.AddressableVehicleEntity;
                            return true;
                        }
#endif
                    }
                    break;
                case MountType.Buff:
                    tempIndexOfData = characterData.IndexOfBuff(sourceId);
                    if (tempIndexOfData < 0)
                        return false;
                    tempCalculatedBuff = characterData.Buffs[tempIndexOfData].GetBuff();
                    if (tempCalculatedBuff != null && tempCalculatedBuff.TryGetMount(out tempBuffMount))
                    {
                        if (tempBuffMount.MountEntity != null)
                        {
                            prefab = tempBuffMount.MountEntity;
                            return false;
                        }
#if !DISABLE_ADDRESSABLES
                        else if (tempBuffMount.AddressableMountEntity.IsDataValid())
                        {
                            addressablePrefab = tempBuffMount.AddressableMountEntity;
                            return true;
                        }
#endif
                    }
                    break;
                case MountType.Custom:
                    // TODO: Implement this
                    return false;
            }
            return false;
        }

        public static int GetPrefabEntityId(this MountType mountType, ICharacterData characterData, string sourceId)
        {
            if (mountType.GetPrefab(characterData, sourceId
                , out VehicleEntity prefab
#if !DISABLE_ADDRESSABLES
                , out AssetReferenceVehicleEntity addressablePrefab
#endif
                ))
            {
#if !DISABLE_ADDRESSABLES
                if (addressablePrefab.IsDataValid())
                    return addressablePrefab.HashAssetId;
#endif
                return 0;
            }
            else
            {
                if (prefab != null)
                    return prefab.EntityId;
                return 0;
            }
        }
    }
}
