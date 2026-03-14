using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MultiplayerARPG
{
    public partial class BasePlayerCharacterEntity
    {
        private enum ItemDropSource
        {
            EquipWeapons,
            EquipItems,
            NonEquipItems,
        }

        private struct ItemDropData
        {
            public ItemDropSource source;
            public int index;
            public bool isLeftHand;
            public CharacterItem item;
        }

        public virtual void OnKillMonster(BaseMonsterCharacterEntity monsterCharacterEntity)
        {
            if (!IsServer || monsterCharacterEntity == null)
                return;

            for (int i = 0; i < Quests.Count; ++i)
            {
                CharacterQuest quest = Quests[i];
                if (quest.AddKillMonster(monsterCharacterEntity, 1))
                    quests[i] = quest;
            }
        }

        public override void Killed(EntityInfo lastAttacker)
        {
            // Prepare data
            DeadPunishmentType deadPunishmentType = DeadPunishmentType.Unspecified;
            LastDeadTime = System.DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            lastAttacker.TryGetEntity(out BaseCharacterEntity attackerEntity);
            BasePlayerCharacterEntity attackPlayer = attackerEntity as BasePlayerCharacterEntity;
            bool attackerIsPlayer = attackPlayer != null;

            // Find punishment type
            if (DuelingComponent != null && DuelingComponent.DuelingStarted)
            {
                deadPunishmentType = DeadPunishmentType.Duel;
                DuelingComponent.EndDueling(this);
            }
            else if (CurrentMapInfo.EnablePkRules && attackerIsPlayer)
            {
                deadPunishmentType = DeadPunishmentType.PK;
            }

            // Dead Penalty
            CurrentGameInstance.GameplayRule.GetPlayerDeadPunishment(deadPunishmentType, this, attackerEntity, out int decraseExp, out int decreaseGold, out int decreaseItems, out int attackerPkPoint);
            if (deadPunishmentType == DeadPunishmentType.PK)
            {
#if !DISABLE_CLASSIC_PK
                attackPlayer.PkPoint += attackerPkPoint;
                attackPlayer.ConsecutivePkKills++;
                if (attackPlayer.PkPoint > attackPlayer.HighestPkPoint)
                    attackPlayer.HighestPkPoint = attackPlayer.PkPoint;
                if (attackPlayer.ConsecutivePkKills > attackPlayer.HighestConsecutivePkKills)
                    attackPlayer.HighestConsecutivePkKills = attackPlayer.ConsecutivePkKills;
                PkPoint = 0;
                ConsecutivePkKills = 0;
#endif
            }

            // Decrease Exp
            if (Exp > decraseExp)
                Exp -= decraseExp;
            else
                Exp = 0;

            // Decrease Gold
            if (Gold > decreaseGold)
                Gold -= decreaseGold;
            else
                Gold = 0;

            // Clear data
            NpcActionComponent.ClearNpcDialogData();

            // Drop items
            KilledDropItems(lastAttacker, deadPunishmentType, decreaseItems);

            base.Killed(lastAttacker);

            if (IsServer)
                GameInstance.ServerLogHandlers.LogKilled(this, lastAttacker);

#if UNITY_EDITOR || UNITY_SERVER || !EXCLUDE_SERVER_CODES
            if (BaseGameNetworkManager.CurrentMapInfo.AutoRespawnWhenDead)
                GameInstance.ServerCharacterHandlers.Respawn(0, this);
#endif
        }

        protected async void KilledDropItems(EntityInfo lastAttacker, DeadPunishmentType deadPunishmentType, int decreaseItems)
        {
            if (decreaseItems <= 0)
                return;

            // Add killer to looters
            HashSet<string> looters = new HashSet<string>();
            string killerObjectId;
            if (lastAttacker.HasSummoner)
                killerObjectId = lastAttacker.Summoner.Id;
            else
                killerObjectId = lastAttacker.Id;
            if (!string.IsNullOrEmpty(killerObjectId))
                looters.Add(killerObjectId);

            // Add this character to make players able to get items from their own corpses immediately
            looters.Add(Id);

            // Which kind of items will be dropped
            bool playerDeadDropsEquipWeapons = false;
            bool playerDeadDropsEquipItems = false;
            bool playerDeadDropsNonEquipItems = false;
            bool playerDeadDropsGold = false;

            switch (CurrentMapInfo.PlayerDeadDropsEquipWeapons)
            {
                case PlayerItemDropMode.AlwaysDrop:
                    playerDeadDropsEquipWeapons = true;
                    break;
                case PlayerItemDropMode.PkPunishmentDrop:
                    playerDeadDropsEquipWeapons = deadPunishmentType == DeadPunishmentType.PK;
                    break;
            }

            switch (CurrentMapInfo.PlayerDeadDropsEquipItems)
            {
                case PlayerItemDropMode.AlwaysDrop:
                    playerDeadDropsEquipItems = true;
                    break;
                case PlayerItemDropMode.PkPunishmentDrop:
                    playerDeadDropsEquipItems = deadPunishmentType == DeadPunishmentType.PK;
                    break;
            }

            switch (CurrentMapInfo.PlayerDeadDropsNonEquipItems)
            {
                case PlayerItemDropMode.AlwaysDrop:
                    playerDeadDropsNonEquipItems = true;
                    break;
                case PlayerItemDropMode.PkPunishmentDrop:
                    playerDeadDropsNonEquipItems = deadPunishmentType == DeadPunishmentType.PK;
                    break;
            }

            switch (CurrentMapInfo.PlayerDeadDropsGold)
            {
                case PlayerItemDropMode.AlwaysDrop:
                    playerDeadDropsGold = true;
                    break;
                case PlayerItemDropMode.PkPunishmentDrop:
                    playerDeadDropsGold = deadPunishmentType == DeadPunishmentType.PK;
                    break;
            }

            // Drop an items
            List<ItemDropData> droppingItems = new List<ItemDropData>();
            if (playerDeadDropsEquipWeapons)
            {
                for (int i = 0; i < SelectableWeaponSets.Count; ++i)
                {
                    if (!SelectableWeaponSets[i].IsEmptyRightHandSlot() && !CurrentMapInfo.ExcludeItemFromDropping(SelectableWeaponSets[i].GetRightHandItem()))
                    {
                        droppingItems.Add(new ItemDropData()
                        {
                            source = ItemDropSource.EquipWeapons,
                            index = i,
                            isLeftHand = false,
                            item = SelectableWeaponSets[i].rightHand,
                        });
                    }
                    if (!SelectableWeaponSets[i].IsEmptyLeftHandSlot() && !CurrentMapInfo.ExcludeItemFromDropping(SelectableWeaponSets[i].GetLeftHandItem()))
                    {
                        droppingItems.Add(new ItemDropData()
                        {
                            source = ItemDropSource.EquipWeapons,
                            index = i,
                            isLeftHand = true,
                            item = SelectableWeaponSets[i].leftHand,
                        });
                    }
                }
            }

            if (playerDeadDropsEquipItems)
            {
                for (int i = EquipItems.Count - 1; i >= 0; --i)
                {
                    if (!EquipItems[i].IsEmptySlot() && !CurrentMapInfo.ExcludeItemFromDropping(EquipItems[i].GetItem()))
                    {
                        droppingItems.Add(new ItemDropData()
                        {
                            source = ItemDropSource.EquipItems,
                            index = i,
                            item = EquipItems[i],
                        });
                    }
                }
            }

            if (playerDeadDropsNonEquipItems)
            {
                for (int i = NonEquipItems.Count - 1; i >= 0; --i)
                {
                    if (!NonEquipItems[i].IsEmptySlot() && !CurrentMapInfo.ExcludeItemFromDropping(NonEquipItems[i].GetItem()))
                    {
                        droppingItems.Add(new ItemDropData()
                        {
                            source = ItemDropSource.NonEquipItems,
                            index = i,
                            item = NonEquipItems[i],
                        });
                    }
                }
            }

            if (playerDeadDropsGold)
            {
                if (Gold > 0 && CurrentGameInstance.monsterGoldRewardingMode == RewardingMode.DropOnGround)
                    GoldDropEntity.Drop(this, 1f, RewardGivenType.PlayerDead, Level, Level, Gold, looters).Forget();
                Gold = 0;
            }

            if (droppingItems.Count > 0)
            {
                droppingItems.Shuffle();
                List<ItemDropData> removingItems = new List<ItemDropData>();
                List<CharacterItem> removingItemInstances = new List<CharacterItem>();
                for (int i = 0; i < droppingItems.Count; ++i)
                {
                    removingItems.Add(droppingItems[i]);
                    removingItemInstances.Add(droppingItems[i].item);
                    if (removingItems.Count >= decreaseItems)
                        break;
                }

                removingItems = removingItems.OrderByDescending(o => o.index).ToList();
                for (int i = 0; i < removingItems.Count; ++i)
                {
                    switch (removingItems[i].source)
                    {
                        case ItemDropSource.EquipWeapons:
                            EquipWeapons updatingEquipWeapons = SelectableWeaponSets[removingItems[i].index].Clone();
                            if (removingItems[i].isLeftHand)
                                updatingEquipWeapons.leftHand = CharacterItem.Empty;
                            else
                                updatingEquipWeapons.rightHand = CharacterItem.Empty;
                            SelectableWeaponSets[removingItems[i].index] = updatingEquipWeapons;
                            break;
                        case ItemDropSource.EquipItems:
                            EquipItems.RemoveAt(removingItems[i].index);
                            break;
                        case ItemDropSource.NonEquipItems:
                            NonEquipItems.RemoveAt(removingItems[i].index);
                            break;
                    }
                }

                if (removingItemInstances.Count > 0)
                {
                    this.FillEmptySlots();
                    switch (CurrentGameInstance.playerDeadDropItemMode)
                    {
                        case DeadDropItemMode.DropOnGround:
                            for (int i = 0; i < removingItemInstances.Count; ++i)
                            {
                                ItemDropEntity.Drop(this, RewardGivenType.PlayerDead, removingItemInstances[i], looters).Forget();
                            }
                            break;
                        case DeadDropItemMode.CorpseLooting:
                            if (removingItemInstances.Count > 0)
                            {
                                ItemsContainerEntity loadedPrefab = await CurrentGameInstance.GetLoadedPlayerCorpsePrefab();
                                if (loadedPrefab != null)
                                    ItemsContainerEntity.DropItems(loadedPrefab, this, RewardGivenType.PlayerDead, removingItemInstances, looters, CurrentGameInstance.playerCorpseAppearDuration);
                            }
                            break;
                    }
                }
            }
        }

        public override void ReceivedDamage(HitBoxPosition position, Vector3 fromPosition, EntityInfo instigator, Dictionary<DamageElement, MinMaxFloat> damageAmounts, CombatAmountType combatAmountType, int totalDamage, CharacterItem weapon, BaseSkill skill, int skillLevel, CharacterBuff buff, bool isDamageOverTime = false)
        {
            base.ReceivedDamage(position, fromPosition, instigator, damageAmounts, combatAmountType, totalDamage, weapon, skill, skillLevel, buff, isDamageOverTime);
            if (IsServer)
                GameInstance.ServerLogHandlers.LogDamageReceived(this, position, fromPosition, instigator, damageAmounts, combatAmountType, totalDamage, weapon, skill, skillLevel, buff, isDamageOverTime);
        }
    }
}
