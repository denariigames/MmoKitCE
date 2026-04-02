// CE scalability: #12

using LiteNetLibManager;
using System.Buffers;
using System.Collections.Generic;
using UnityEngine;

namespace MultiplayerARPG
{
    public class NearbyEntityDetector : MonoBehaviour
    {
        public float detectingRadius;
        public int resultAllocSize = 128;
        public bool findPlayer;
        public bool findOnlyAlivePlayers;
        public bool findPlayerToAttack;
        public bool findMonster;
        public bool findOnlyAliveMonsters;
        public bool findMonsterToAttack;
        public bool findNpc;
        public bool findItemDrop;
        public bool findRewardDrop;
        public bool findBuilding;
        public bool findOnlyAliveBuildings;
        public bool findOnlyActivatableBuildings;
        public bool findVehicle;
        public bool findWarpPortal;
        public bool findItemsContainer;
        public bool findActivatableEntity;
        public bool findHoldActivatableEntity;
        public bool findPickupActivatableEntity;
        public readonly List<BaseCharacterEntity> characters = new List<BaseCharacterEntity>();
        public readonly List<BasePlayerCharacterEntity> players = new List<BasePlayerCharacterEntity>();
        public readonly List<BaseMonsterCharacterEntity> monsters = new List<BaseMonsterCharacterEntity>();
        public readonly List<NpcEntity> npcs = new List<NpcEntity>();
        public readonly List<ItemDropEntity> itemDrops = new List<ItemDropEntity>();
        public readonly List<BaseRewardDropEntity> rewardDrops = new List<BaseRewardDropEntity>();
        public readonly List<BuildingEntity> buildings = new List<BuildingEntity>();
        public readonly List<VehicleEntity> vehicles = new List<VehicleEntity>();
        public readonly List<WarpPortalEntity> warpPortals = new List<WarpPortalEntity>();
        public readonly List<ItemsContainerEntity> itemsContainers = new List<ItemsContainerEntity>();
        public readonly List<IActivatableEntity> activatableEntities = new List<IActivatableEntity>();
        public readonly List<IHoldActivatableEntity> holdActivatableEntities = new List<IHoldActivatableEntity>();
        public readonly List<IPickupActivatableEntity> pickupActivatableEntities = new List<IPickupActivatableEntity>();
        private readonly HashSet<BaseCharacterEntity> _characterSet = new HashSet<BaseCharacterEntity>();
        private readonly HashSet<BasePlayerCharacterEntity> _playerSet = new HashSet<BasePlayerCharacterEntity>();
        private readonly HashSet<BaseMonsterCharacterEntity> _monsterSet = new HashSet<BaseMonsterCharacterEntity>();
        private readonly HashSet<NpcEntity> _npcSet = new HashSet<NpcEntity>();
        private readonly HashSet<ItemDropEntity> _itemDropSet = new HashSet<ItemDropEntity>();
        private readonly HashSet<BaseRewardDropEntity> _rewardDropSet = new HashSet<BaseRewardDropEntity>();
        private readonly HashSet<BuildingEntity> _buildingSet = new HashSet<BuildingEntity>();
        private readonly HashSet<VehicleEntity> _vehicleSet = new HashSet<VehicleEntity>();
        private readonly HashSet<WarpPortalEntity> _warpPortalSet = new HashSet<WarpPortalEntity>();
        private readonly HashSet<ItemsContainerEntity> _itemsContainerSet = new HashSet<ItemsContainerEntity>();
        private readonly HashSet<IActivatableEntity> _activatableEntitySet = new HashSet<IActivatableEntity>();
        private readonly HashSet<IHoldActivatableEntity> _holdActivatableEntitySet = new HashSet<IHoldActivatableEntity>();
        private readonly HashSet<IPickupActivatableEntity> _pickupActivatableEntitySet = new HashSet<IPickupActivatableEntity>();
        private readonly HashSet<Collider> _excludeColliders = new HashSet<Collider>();
        private readonly HashSet<Collider2D> _excludeCollider2Ds = new HashSet<Collider2D>();
        private readonly BaseGameEntityDistanceComparer _baseGameEntityDistanceComparer = new BaseGameEntityDistanceComparer();
        private readonly ActivatableEntityDistanceComparer _activatableEntityDistanceComparer = new ActivatableEntityDistanceComparer();

        public System.Action onUpdateList;

        private void Awake()
        {
            gameObject.layer = PhysicLayers.IgnoreRaycast;
            NearbyEntityDetectorManager.Register(this);
        }

        private void OnDestroy()
        {
            ClearDetection();
            ClearExclusion();
            onUpdateList = null;
            NearbyEntityDetectorManager.Unregister(this);
        }

        public void ClearDetection()
        {
            characters.Nullify();
            characters?.Clear();
            _characterSet.Clear();
            players.Nullify();
            players?.Clear();
            _playerSet.Clear();
            monsters.Nullify();
            monsters?.Clear();
            _monsterSet.Clear();
            npcs.Nullify();
            npcs?.Clear();
            _npcSet.Clear();
            itemDrops.Nullify();
            itemDrops?.Clear();
            _itemDropSet.Clear();
            rewardDrops.Nullify();
            rewardDrops?.Clear();
            _rewardDropSet.Clear();
            buildings.Nullify();
            buildings?.Clear();
            _buildingSet.Clear();
            vehicles.Nullify();
            vehicles?.Clear();
            _vehicleSet.Clear();
            warpPortals.Nullify();
            warpPortals?.Clear();
            _warpPortalSet.Clear();
            itemsContainers.Nullify();
            itemsContainers?.Clear();
            _itemsContainerSet.Clear();
            activatableEntities?.Clear();
            _activatableEntitySet.Clear();
            holdActivatableEntities?.Clear();
            _holdActivatableEntitySet.Clear();
            pickupActivatableEntities?.Clear();
            _pickupActivatableEntitySet.Clear();
        }


        public void ClearExclusion()
        {
            _excludeColliders.Clear();
            _excludeCollider2Ds.Clear();
        }

        internal void DetectEntities()
        {
            int tempHitCount;
            ClearDetection();
            switch (GameInstance.Singleton.DimensionType)
            {
                case DimensionType.Dimension2D:
                    Collider2D[] collider2Ds = ArrayPool<Collider2D>.Shared.Rent(resultAllocSize);
                    tempHitCount = Physics2D.OverlapCircleNonAlloc(GameInstance.PlayingCharacterEntity.EntityTransform.position, detectingRadius, collider2Ds);
                    for (int i = 0; i < tempHitCount; ++i)
                    {
                        Collider2D other = collider2Ds[i];
                        if (other == null || _excludeCollider2Ds.Contains(other))
                            continue;
                        AddEntity(other.gameObject);
                    }
                    ArrayPool<Collider2D>.Shared.Return(collider2Ds);
                    if (onUpdateList != null)
                        onUpdateList.Invoke();
                    break;
                default:
                    Collider[] colliders = ArrayPool<Collider>.Shared.Rent(resultAllocSize);
                    tempHitCount = Physics.OverlapSphereNonAlloc(GameInstance.PlayingCharacterEntity.EntityTransform.position, detectingRadius, colliders);
                    for (int i = 0; i < tempHitCount; ++i)
                    {
                        Collider other = colliders[i];
                        if (other == null || _excludeColliders.Contains(other))
                            continue;
                        AddEntity(other.gameObject);
                    }
                    ArrayPool<Collider>.Shared.Return(colliders);
                    if (onUpdateList != null)
                        onUpdateList.Invoke();
                    break;
            }
        }

        internal void RemoveInactiveAndSortNearestAllEntity()
        {
            RemoveInactiveAllEntity();
            SortNearestAllEntity();
        }

        internal void RemoveInactiveAllEntity()
        {
            RemoveInactiveEntity(characters, _characterSet);
            RemoveInactiveEntity(players, _playerSet);
            RemoveInactiveEntity(monsters, _monsterSet);
            RemoveInactiveEntity(npcs, _npcSet);
            RemoveInactiveEntity(itemDrops, _itemDropSet);
            RemoveInactiveEntity(rewardDrops, _rewardDropSet);
            RemoveInactiveEntity(buildings, _buildingSet);
            RemoveInactiveEntity(vehicles, _vehicleSet);
            RemoveInactiveEntity(warpPortals, _warpPortalSet);
            RemoveInactiveEntity(itemsContainers, _itemsContainerSet);
            RemoveInactiveActivatableEntity(activatableEntities, _activatableEntitySet);
            RemoveInactiveActivatableEntity(holdActivatableEntities, _holdActivatableEntitySet);
            RemoveInactiveActivatableEntity(pickupActivatableEntities, _pickupActivatableEntitySet);
        }

        internal void SortNearestAllEntity()
        {
            Vector3 playerPosition = GameInstance.PlayingCharacterEntity.EntityTransform.position;
            _baseGameEntityDistanceComparer.PlayerPosition = playerPosition;
            _activatableEntityDistanceComparer.PlayerPosition = playerPosition;

            SortNearestEntity(characters);
            SortNearestEntity(players);
            SortNearestEntity(monsters);
            SortNearestEntity(npcs);
            SortNearestEntity(itemDrops);
            SortNearestEntity(rewardDrops);
            SortNearestEntity(buildings);
            SortNearestEntity(vehicles);
            SortNearestEntity(warpPortals);
            SortNearestEntity(itemsContainers);
            SortNearestActivatableEntity(activatableEntities);
            SortNearestActivatableEntity(holdActivatableEntities);
            SortNearestActivatableEntity(pickupActivatableEntities);
        }

        public bool AddEntity(GameObject other)
        {
            BasePlayerCharacterEntity player;
            BaseMonsterCharacterEntity monster;
            NpcEntity npc;
            ItemDropEntity itemDrop;
            BaseRewardDropEntity rewardDrop;
            BuildingEntity building;
            VehicleEntity vehicle;
            WarpPortalEntity warpPortal;
            ItemsContainerEntity itemsContainer;
            IActivatableEntity activatableEntity;
            IHoldActivatableEntity holdActivatableEntity;
            IPickupActivatableEntity pickupActivatableEntity;
            FindEntity(other, out player, out monster, out npc, out itemDrop, out rewardDrop, out building, out vehicle, out warpPortal, out itemsContainer, out activatableEntity, out holdActivatableEntity, out pickupActivatableEntity, true);

            bool foundSomething = false;
            if (player != null)
            {
                if (_characterSet.Add(player))
                    characters.Add(player);
                if (_playerSet.Add(player))
                    players.Add(player);
                foundSomething = true;
            }
            if (monster != null)
            {
                if (_characterSet.Add(monster))
                    characters.Add(monster);
                if (_monsterSet.Add(monster))
                    monsters.Add(monster);
                foundSomething = true;
            }
            if (npc != null)
            {
                if (_npcSet.Add(npc))
                    npcs.Add(npc);
                foundSomething = true;
            }
            if (itemDrop != null)
            {
                if (_itemDropSet.Add(itemDrop))
                    itemDrops.Add(itemDrop);
                foundSomething = true;
            }
            if (rewardDrop != null)
            {
                if (_rewardDropSet.Add(rewardDrop))
                    rewardDrops.Add(rewardDrop);
                foundSomething = true;
            }
            if (building != null)
            {
                if (_buildingSet.Add(building))
                    buildings.Add(building);
                foundSomething = true;
            }
            if (vehicle != null)
            {
                if (_vehicleSet.Add(vehicle))
                    vehicles.Add(vehicle);
                foundSomething = true;
            }
            if (warpPortal != null)
            {
                if (_warpPortalSet.Add(warpPortal))
                    warpPortals.Add(warpPortal);
                foundSomething = true;
            }
            if (itemsContainer != null)
            {
                if (_itemsContainerSet.Add(itemsContainer))
                    itemsContainers.Add(itemsContainer);
                foundSomething = true;
            }
            if (!activatableEntity.IsNull())
            {
                if (_activatableEntitySet.Add(activatableEntity))
                    activatableEntities.Add(activatableEntity);
                foundSomething = true;
            }
            if (!holdActivatableEntity.IsNull())
            {
                if (_holdActivatableEntitySet.Add(holdActivatableEntity))
                    holdActivatableEntities.Add(holdActivatableEntity);
                foundSomething = true;
            }
            if (!pickupActivatableEntity.IsNull())
            {
                if (_pickupActivatableEntitySet.Add(pickupActivatableEntity))
                    pickupActivatableEntities.Add(pickupActivatableEntity);
                foundSomething = true;
            }
            return foundSomething;
        }

        public bool RemoveEntity(GameObject other)
        {
            BasePlayerCharacterEntity player;
            BaseMonsterCharacterEntity monster;
            NpcEntity npc;
            ItemDropEntity itemDrop;
            BaseRewardDropEntity rewardDrop;
            BuildingEntity building;
            VehicleEntity vehicle;
            WarpPortalEntity warpPortal;
            ItemsContainerEntity itemsContainer;
            IActivatableEntity activatableEntity;
            IHoldActivatableEntity holdActivatableEntity;
            IPickupActivatableEntity pickupActivatableEntity;
            FindEntity(other, out player, out monster, out npc, out itemDrop, out rewardDrop, out building, out vehicle, out warpPortal, out itemsContainer, out activatableEntity, out holdActivatableEntity, out pickupActivatableEntity, false);

            bool removeSomething = false;
            if (player != null)
            {
                _characterSet.Remove(player);
                _playerSet.Remove(player);
                removeSomething = removeSomething || characters.Remove(player) && players.Remove(player);
            }
            if (monster != null)
            {
                _characterSet.Remove(monster);
                _monsterSet.Remove(monster);
                removeSomething = removeSomething || characters.Remove(monster) && monsters.Remove(monster);
            }
            if (npc != null)
            {
                _npcSet.Remove(npc);
                removeSomething = removeSomething || npcs.Remove(npc);
            }
            if (itemDrop != null)
            {
                _itemDropSet.Remove(itemDrop);
                removeSomething = removeSomething || itemDrops.Remove(itemDrop);
            }
            if (rewardDrop != null)
            {
                _rewardDropSet.Remove(rewardDrop);
                removeSomething = removeSomething || rewardDrops.Remove(rewardDrop);
            }
            if (building != null)
            {
                _buildingSet.Remove(building);
                removeSomething = removeSomething || buildings.Remove(building);
            }
            if (vehicle != null)
            {
                _vehicleSet.Remove(vehicle);
                removeSomething = removeSomething || vehicles.Remove(vehicle);
            }
            if (warpPortal != null)
            {
                _warpPortalSet.Remove(warpPortal);
                removeSomething = removeSomething || warpPortals.Remove(warpPortal);
            }
            if (itemsContainer != null)
            {
                _itemsContainerSet.Remove(itemsContainer);
                removeSomething = removeSomething || itemsContainers.Remove(itemsContainer);
            }
            if (!activatableEntity.IsNull())
            {
                _activatableEntitySet.Remove(activatableEntity);
                removeSomething = removeSomething || activatableEntities.Remove(activatableEntity);
            }
            if (!holdActivatableEntity.IsNull())
            {
                _holdActivatableEntitySet.Remove(holdActivatableEntity);
                removeSomething = removeSomething || holdActivatableEntities.Remove(holdActivatableEntity);
            }
            if (!pickupActivatableEntity.IsNull())
            {
                _pickupActivatableEntitySet.Remove(pickupActivatableEntity);
                removeSomething = removeSomething || pickupActivatableEntities.Remove(pickupActivatableEntity);
            }
            return removeSomething;
        }

        private void FindEntity(GameObject other,
            out BasePlayerCharacterEntity player,
            out BaseMonsterCharacterEntity monster,
            out NpcEntity npc,
            out ItemDropEntity itemDrop,
            out BaseRewardDropEntity rewardDrop,
            out BuildingEntity building,
            out VehicleEntity vehicle,
            out WarpPortalEntity warpPortal,
            out ItemsContainerEntity itemsContainer,
            out IActivatableEntity activatableEntity,
            out IHoldActivatableEntity holdActivatableEntity,
            out IPickupActivatableEntity pickupActivatableEntity,
            bool findWithAdvanceOptions)
        {
            player = null;
            monster = null;
            npc = null;
            itemDrop = null;
            rewardDrop = null;
            building = null;
            vehicle = null;
            warpPortal = null;
            itemsContainer = null;
            activatableEntity = null;
            holdActivatableEntity = null;
            pickupActivatableEntity = null;

            IGameEntity gameEntity = other.GetComponent<IGameEntity>();
            if (!gameEntity.IsNull())
            {
                if (findPlayer)
                {
                    player = gameEntity.Entity as BasePlayerCharacterEntity;
                    if (GameInstance.PlayingCharacterEntity.IsServer && player != null && player.Identity.IsHideFrom(GameInstance.PlayingCharacterEntity.Identity))
                        player = null;
                    if (player == GameInstance.PlayingCharacterEntity)
                        player = null;
                    if (findWithAdvanceOptions)
                    {
                        if (findOnlyAlivePlayers && player != null && player.IsDead())
                            player = null;
                        if (findPlayerToAttack && player != null && !player.CanReceiveDamageFrom(GameInstance.PlayingCharacterEntity.GetInfo()))
                            player = null;
                    }
                }

                if (findMonster)
                {
                    monster = gameEntity.Entity as BaseMonsterCharacterEntity;
                    if (GameInstance.PlayingCharacterEntity.IsServer && monster != null && monster.Identity.IsHideFrom(GameInstance.PlayingCharacterEntity.Identity))
                        monster = null;
                    if (findWithAdvanceOptions)
                    {
                        if (findOnlyAliveMonsters && monster != null && monster.IsDead())
                            monster = null;
                        if (findMonsterToAttack && monster != null && !monster.CanReceiveDamageFrom(GameInstance.PlayingCharacterEntity.GetInfo()))
                            monster = null;
                    }
                }

                if (findNpc)
                {
                    npc = gameEntity.Entity as NpcEntity;
                    if (GameInstance.PlayingCharacterEntity.IsServer && npc != null && npc.Identity.IsHideFrom(GameInstance.PlayingCharacterEntity.Identity))
                        npc = null;
                }

                if (findItemDrop)
                {
                    itemDrop = gameEntity.Entity as ItemDropEntity;
                    if (GameInstance.PlayingCharacterEntity.IsServer && itemDrop != null && itemDrop.Identity.IsHideFrom(GameInstance.PlayingCharacterEntity.Identity))
                        itemDrop = null;
                }

                if (findRewardDrop)
                {
                    rewardDrop = gameEntity.Entity as BaseRewardDropEntity;
                    if (GameInstance.PlayingCharacterEntity.IsServer && rewardDrop != null && rewardDrop.Identity.IsHideFrom(GameInstance.PlayingCharacterEntity.Identity))
                        rewardDrop = null;
                }

                if (findBuilding)
                {
                    building = gameEntity.Entity as BuildingEntity;
                    if (GameInstance.PlayingCharacterEntity.IsServer && building != null && building.Identity.IsHideFrom(GameInstance.PlayingCharacterEntity.Identity))
                        building = null;
                    if (findWithAdvanceOptions)
                    {
                        if (findOnlyAliveBuildings && building != null && building.IsDead())
                            building = null;
                        if (findOnlyActivatableBuildings && building != null && !building.CanActivate())
                            building = null;
                    }
                }

                if (findVehicle)
                {
                    vehicle = gameEntity.Entity as VehicleEntity;
                    if (GameInstance.PlayingCharacterEntity.IsServer && vehicle != null && vehicle.Identity.IsHideFrom(GameInstance.PlayingCharacterEntity.Identity))
                        vehicle = null;
                }

                if (findWarpPortal)
                {
                    warpPortal = gameEntity.Entity as WarpPortalEntity;
                    if (GameInstance.PlayingCharacterEntity.IsServer && warpPortal != null && warpPortal.Identity.IsHideFrom(GameInstance.PlayingCharacterEntity.Identity))
                        warpPortal = null;
                }

                if (findItemsContainer)
                {
                    itemsContainer = gameEntity.Entity as ItemsContainerEntity;
                    if (GameInstance.PlayingCharacterEntity.IsServer && itemsContainer != null && itemsContainer.Identity.IsHideFrom(GameInstance.PlayingCharacterEntity.Identity))
                        itemsContainer = null;
                }
            }

            if (findActivatableEntity)
            {
                activatableEntity = other.GetComponent<IActivatableEntity>();
                if (!activatableEntity.IsNull())
                {
                    if (activatableEntity.EntityGameObject == GameInstance.PlayingCharacterEntity.EntityGameObject)
                        activatableEntity = null;
                    if (GameInstance.PlayingCharacterEntity.IsServer && !activatableEntity.IsNull() && activatableEntity.EntityGameObject.TryGetComponent(out LiteNetLibIdentity identity) && identity.IsHideFrom(GameInstance.PlayingCharacterEntity.Identity))
                        activatableEntity = null;
                }
            }

            if (findHoldActivatableEntity)
            {
                holdActivatableEntity = other.GetComponent<IHoldActivatableEntity>();
                if (!holdActivatableEntity.IsNull())
                {
                    if (holdActivatableEntity.EntityGameObject == GameInstance.PlayingCharacterEntity.EntityGameObject)
                        holdActivatableEntity = null;
                    if (GameInstance.PlayingCharacterEntity.IsServer && !holdActivatableEntity.IsNull() && holdActivatableEntity.EntityGameObject.TryGetComponent(out LiteNetLibIdentity identity) && identity.IsHideFrom(GameInstance.PlayingCharacterEntity.Identity))
                        holdActivatableEntity = null;
                }
            }

            if (findPickupActivatableEntity)
            {
                pickupActivatableEntity = other.GetComponent<IPickupActivatableEntity>();
                if (!pickupActivatableEntity.IsNull())
                {
                    if (pickupActivatableEntity.EntityGameObject == GameInstance.PlayingCharacterEntity.EntityGameObject)
                        pickupActivatableEntity = null;
                    if (GameInstance.PlayingCharacterEntity.IsServer && !pickupActivatableEntity.IsNull() && pickupActivatableEntity.EntityGameObject.TryGetComponent(out LiteNetLibIdentity identity) && identity.IsHideFrom(GameInstance.PlayingCharacterEntity.Identity))
                        pickupActivatableEntity = null;
                }
            }
        }

        private void RemoveInactiveEntity<T>(List<T> entities, HashSet<T> entitiesSet) where T : BaseGameEntity
        {
            bool hasUpdate = false;
            for (int i = entities.Count - 1; i >= 0; --i)
            {
                T entity = entities[i];
                // ReferenceEquals avoids Unity's overloaded == which can behave
                // unexpectedly on destroyed objects during HashSet operations.
                if (ReferenceEquals(entity, null) || entity == null || !entity.gameObject.activeInHierarchy)
                {
                    if (!ReferenceEquals(entity, null))
                        entitiesSet.Remove(entity);
                    entities.RemoveAt(i);
                    hasUpdate = true;
                }
            }
            if (hasUpdate && onUpdateList != null)
                onUpdateList.Invoke();
        }

        private void RemoveInactiveActivatableEntity<T>(List<T> entities, HashSet<T> entitiesSet) where T : IBaseActivatableEntity
        {
            bool hasUpdate = false;
            for (int i = entities.Count - 1; i >= 0; --i)
            {
                T entity = entities[i];
                bool isTrulyNull = ReferenceEquals(entity, null);
                if (isTrulyNull || entity == null || (entity is Object unityObj && unityObj == null) ||
                    !entity.EntityGameObject.activeInHierarchy)
                {
                    if (!isTrulyNull)
                        entitiesSet.Remove(entity);
                    entities.RemoveAt(i);
                    hasUpdate = true;
                }
            }
            if (hasUpdate && onUpdateList != null)
                onUpdateList.Invoke();
        }

        private void SortNearestEntity<T>(List<T> entities) where T : BaseGameEntity
        {
            if (entities.Count > 1)
                entities.Sort(_baseGameEntityDistanceComparer);
        }

        private void SortNearestActivatableEntity<T>(List<T> entities) where T : IBaseActivatableEntity
        {
            if (entities.Count > 1)
                entities.Sort((x, y) => _activatableEntityDistanceComparer.Compare(x, y));
        }

        private sealed class BaseGameEntityDistanceComparer : IComparer<BaseGameEntity>
        {
            public Vector3 PlayerPosition;

            public int Compare(BaseGameEntity x, BaseGameEntity y)
            {
                if (ReferenceEquals(x, y))
                    return 0;
                if (x == null)
                    return 1;
                if (y == null)
                    return -1;

                float xSqrDistance = (x.transform.position - PlayerPosition).sqrMagnitude;
                float ySqrDistance = (y.transform.position - PlayerPosition).sqrMagnitude;
                return xSqrDistance.CompareTo(ySqrDistance);
            }
        }

        private sealed class ActivatableEntityDistanceComparer : IComparer<IBaseActivatableEntity>
        {
            public Vector3 PlayerPosition;

            public int Compare(IBaseActivatableEntity x, IBaseActivatableEntity y)
            {
                if (ReferenceEquals(x, y))
                    return 0;
                if (x == null || (x is Object xUnityObj && xUnityObj == null))
                    return 1;
                if (y == null || (y is Object yUnityObj && yUnityObj == null))
                    return -1;

                float xSqrDistance = (x.EntityTransform.position - PlayerPosition).sqrMagnitude;
                float ySqrDistance = (y.EntityTransform.position - PlayerPosition).sqrMagnitude;
                return xSqrDistance.CompareTo(ySqrDistance);
            }
        }
    }
}
