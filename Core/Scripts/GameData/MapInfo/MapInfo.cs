using Insthync.UnityEditorUtils;
using LiteNetLib.Utils;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace MultiplayerARPG
{
    [CreateAssetMenu(fileName = GameDataMenuConsts.MAP_INFO_FILE, menuName = GameDataMenuConsts.MAP_INFO_MENU, order = GameDataMenuConsts.MAP_INFO_ORDER)]
    public partial class MapInfo : BaseMapInfo
    {
        [Category("Map Info Settings")]
        [Tooltip("If this is `TRUE`, player can return to save point by `return` key. Else it will able to do that when dead only")]
        public bool canReturnToSavePoint;
        [Tooltip("If this is `Pvp`, player can battle all other players. `FactionPvp`, player can battle difference faction players. `GuildPvp`, player can battle difference guild players")]
        public PvpMode pvpMode;
        [Tooltip("If this length is more than 1 it will find respawn points which its condition is match with the character")]
        [FormerlySerializedAs("overrideRespawnPoints")]
        public WarpPointByCondition[] respawnPointsByCondition;
        [Tooltip("If this is `TRUE`, only duelers can attacks each other, other characters cannot do it, duelers also cannot attacks other")]
        public bool duelersCanAttackEachOtherOnly;

        [System.NonSerialized]
        private Dictionary<int, List<WarpPointByCondition>> _cacheRespawnPointsByCondition;
        public Dictionary<int, List<WarpPointByCondition>> CacheRespawnPointsByCondition
        {
            get
            {
                if (_cacheRespawnPointsByCondition == null)
                {
                    _cacheRespawnPointsByCondition = new Dictionary<int, List<WarpPointByCondition>>();
                    int factionDataId;
                    foreach (WarpPointByCondition overrideRespawnPoint in respawnPointsByCondition)
                    {
                        factionDataId = 0;
                        if (overrideRespawnPoint.forFaction != null)
                            factionDataId = overrideRespawnPoint.forFaction.DataId;
                        if (!_cacheRespawnPointsByCondition.ContainsKey(factionDataId))
                            _cacheRespawnPointsByCondition.Add(factionDataId, new List<WarpPointByCondition>());
                        _cacheRespawnPointsByCondition[factionDataId].Add(overrideRespawnPoint);
                    }
                }
                return _cacheRespawnPointsByCondition;
            }
        }

        public override void GetRespawnPoint(IPlayerCharacterData playerCharacterData, out WarpPortalType portalType, out string mapName, out Vector3 position, out bool overrideRotation, out Vector3 rotation)
        {
            base.GetRespawnPoint(playerCharacterData, out portalType, out mapName, out position, out overrideRotation, out rotation);
            if (CacheRespawnPointsByCondition.Count > 0)
            {
                List<WarpPointByCondition> warpPoints;
                if (CacheRespawnPointsByCondition.TryGetValue(playerCharacterData.FactionId, out warpPoints) ||
                    CacheRespawnPointsByCondition.TryGetValue(0, out warpPoints))
                {
                    WarpPointByCondition warpPoint = warpPoints[Random.Range(0, warpPoints.Count)];
                    portalType = warpPoint.warpPortalType;
                    mapName = warpPoint.warpToMapInfo == null ? string.Empty : warpPoint.warpToMapInfo.Id;
                    position = warpPoint.warpToPosition;
                    overrideRotation = warpPoint.warpOverrideRotation;
                    rotation = warpPoint.warpToRotation;
                }
            }
        }

        protected override bool IsPlayerAlly(BasePlayerCharacterEntity playerCharacter, EntityInfo targetEntity)
        {
            if (string.IsNullOrEmpty(targetEntity.Id))
                return false;

            if (targetEntity.Type == EntityTypes.Player && targetEntity.TryGetEntity(out BasePlayerCharacterEntity targetPlayer))
            {
                // Cannot attack duelers because they're invulnerable to other
                if (duelersCanAttackEachOtherOnly && targetPlayer.DuelingComponent != null && targetPlayer.DuelingComponent.DuelingStartingOrStarted && targetPlayer.DuelingComponent.DuelingCharacterObjectId != playerCharacter.ObjectId)
                    return true;

                // Cannot attack other because dueler invulnerable to other
                if (duelersCanAttackEachOtherOnly && playerCharacter.DuelingComponent != null && playerCharacter.DuelingComponent.DuelingStartingOrStarted && playerCharacter.DuelingComponent.DuelingCharacterObjectId != targetPlayer.ObjectId)
                    return true;

                // Cannot attack another dueler because dueling not started yet
                if (playerCharacter.DuelingComponent != null && playerCharacter.DuelingComponent.DuelingStarting && playerCharacter.DuelingComponent.DuelingCharacterObjectId == targetEntity.ObjectId)
                    return true;

                // Can attack another dueler because dueling started
                if (playerCharacter.DuelingComponent != null && playerCharacter.DuelingComponent.DuelingStarted && playerCharacter.DuelingComponent.DuelingCharacterObjectId == targetEntity.ObjectId)
                    return false;

                // Cannot attack party member
                if (targetEntity.PartyId != 0 && targetEntity.PartyId == playerCharacter.PartyId)
                    return true;

                switch (pvpMode)
                {
                    case PvpMode.Pvp:
                        return false;
                    case PvpMode.FactionPvp:
                        return targetEntity.FactionId != 0 && targetEntity.FactionId == playerCharacter.FactionId;
                    case PvpMode.GuildPvp:
                        return targetEntity.GuildId != 0 && targetEntity.GuildId == playerCharacter.GuildId;
                    default:
#if !DISABLE_CLASSIC_PK
                        return !EnablePkRules || !playerCharacter.IsPkOn || !targetPlayer.IsPkOn;
#else
                        return true;
#endif
                }
            }

            if (targetEntity.Type == EntityTypes.Monster)
            {
                // If this character is summoner so it is ally
                if (targetEntity.HasSummoner)
                {
                    // If summoned by someone, will have same allies with summoner
                    return playerCharacter.IsAlly(targetEntity.Summoner);
                }
                else
                {
                    // If it has faction set, then check the faction between two characters
                    if (targetEntity.FactionId != 0 && playerCharacter.FactionId == targetEntity.FactionId)
                        return true;
                    // Monster always not player's ally
                    return false;
                }
            }

            return false;
        }

        protected override bool IsMonsterAlly(BaseMonsterCharacterEntity monsterCharacter, EntityInfo targetEntity)
        {
            if (string.IsNullOrEmpty(targetEntity.Id))
                return false;

            if (monsterCharacter.IsSummonedAndSummonerExisted)
            {
                // If summoned by someone, will have same allies with summoner
                return targetEntity.Id.Equals(monsterCharacter.Summoner.Id) || monsterCharacter.Summoner.IsAlly(targetEntity);
            }

            if (targetEntity.Type == EntityTypes.Player && targetEntity.TryGetEntity(out BasePlayerCharacterEntity targetPlayer))
            {
                // Cannot attack duelers
                if (duelersCanAttackEachOtherOnly && targetPlayer.DuelingComponent != null && targetPlayer.DuelingComponent.DuelingStartingOrStarted && targetPlayer.DuelingComponent.DuelingCharacterObjectId > 0)
                    return true;

                // If it has faction set, then check the faction between two characters
                if (targetEntity.FactionId != 0 && monsterCharacter.FactionId == targetEntity.FactionId)
                    return true;

                // Players are not monster's ally by default
                return false;
            }

            if (targetEntity.Type == EntityTypes.Monster)
            {
                if (targetEntity.HasSummoner)
                {
                    return monsterCharacter.IsAlly(targetEntity.Summoner);
                }
                else
                {
                    if (targetEntity.FactionId != 0 && monsterCharacter.FactionId == targetEntity.FactionId)
                        return true;
                }
                // If another monster has same allyId so it is ally
                return GameInstance.MonsterCharacters[targetEntity.DataId].AllyId == monsterCharacter.CharacterDatabase.AllyId;
            }

            return false;
        }

        protected override bool IsPlayerEnemy(BasePlayerCharacterEntity playerCharacter, EntityInfo targetEntity)
        {
            if (string.IsNullOrEmpty(targetEntity.Id))
                return false;

            if (targetEntity.Type == EntityTypes.Player && targetEntity.TryGetEntity(out BasePlayerCharacterEntity targetPlayer))
            {
                // Cannot attack duelers because they're invulnerable to other
                if (duelersCanAttackEachOtherOnly && targetPlayer.DuelingComponent != null && targetPlayer.DuelingComponent.DuelingStartingOrStarted && targetPlayer.DuelingComponent.DuelingCharacterObjectId != playerCharacter.ObjectId)
                    return false;

                // Cannot attack other because dueler invulnerable to other
                if (duelersCanAttackEachOtherOnly && playerCharacter.DuelingComponent != null && playerCharacter.DuelingComponent.DuelingStartingOrStarted && playerCharacter.DuelingComponent.DuelingCharacterObjectId != targetPlayer.ObjectId)
                    return false;

                // Cannot attack another dueler because dueling not started yet
                if (playerCharacter.DuelingComponent != null && playerCharacter.DuelingComponent.DuelingStarting && playerCharacter.DuelingComponent.DuelingCharacterObjectId == targetEntity.ObjectId)
                    return false;

                // Can attack another dueler because dueling started
                if (playerCharacter.DuelingComponent != null && playerCharacter.DuelingComponent.DuelingStarted && playerCharacter.DuelingComponent.DuelingCharacterObjectId == targetEntity.ObjectId)
                    return true;

                // Cannot attack party member
                if (targetEntity.PartyId != 0 && targetEntity.PartyId == playerCharacter.PartyId)
                    return false;

                switch (pvpMode)
                {
                    case PvpMode.Pvp:
                        return true;
                    case PvpMode.FactionPvp:
                        return targetEntity.FactionId == 0 || targetEntity.FactionId != playerCharacter.FactionId;
                    case PvpMode.GuildPvp:
                        return targetEntity.GuildId == 0 || targetEntity.GuildId != playerCharacter.GuildId;
                    default:
#if !DISABLE_CLASSIC_PK
                        return EnablePkRules && playerCharacter.IsPkOn && targetPlayer.IsPkOn;
#else
                        return false;
#endif
                }
            }

            if (targetEntity.Type == EntityTypes.Monster)
            {
                // If this character is not summoner so it is enemy
                if (targetEntity.HasSummoner)
                {
                    // If summoned by someone, will have same enemies with summoner
                    return playerCharacter.IsEnemy(targetEntity.Summoner);
                }
                else
                {
                    // If it has faction set, then check the faction between two characters
                    if (targetEntity.FactionId != 0 && playerCharacter.FactionId == targetEntity.FactionId)
                        return false;
                    // Monster always be player's enemy
                    return true;
                }
            }

            return false;
        }

        protected override bool IsMonsterEnemy(BaseMonsterCharacterEntity monsterCharacter, EntityInfo targetEntity)
        {
            if (string.IsNullOrEmpty(targetEntity.Id))
                return false;

            if (monsterCharacter.IsSummonedAndSummonerExisted)
            {
                // If summoned by someone, will have same enemies with summoner
                return monsterCharacter.Summoner.IsEnemy(targetEntity);
            }

            if (targetEntity.Type == EntityTypes.Player && targetEntity.TryGetEntity(out BasePlayerCharacterEntity targetPlayer))
            {
                // Cannot attack duelers
                if (duelersCanAttackEachOtherOnly && targetPlayer.DuelingComponent != null && targetPlayer.DuelingComponent.DuelingStartingOrStarted && targetPlayer.DuelingComponent.DuelingCharacterObjectId > 0)
                    return false;

                // If it has faction set, then check the faction between two characters
                if (targetEntity.FactionId != 0 && monsterCharacter.FactionId == targetEntity.FactionId)
                    return false;

                // Players are monster's enemy by default
                return true;
            }

            if (targetEntity.Type == EntityTypes.Monster && targetEntity.TryGetEntity(out BaseMonsterCharacterEntity targetMonster) && targetMonster.IsSummonedAndSummonerExisted)
            {
                // Attack monster which its summoner is enemy
                return monsterCharacter.IsEnemy(targetEntity.Summoner);
            }

            return false;
        }

        public override void Serialize(NetDataWriter writer)
        {
            base.Serialize(writer);
            writer.Put((byte)pvpMode);
            writer.Put(duelersCanAttackEachOtherOnly);
        }

        public override void Deserialize(NetDataReader reader)
        {
            base.Deserialize(reader);
            pvpMode = (PvpMode)reader.GetByte();
            duelersCanAttackEachOtherOnly = reader.GetBool();
        }
    }
}
