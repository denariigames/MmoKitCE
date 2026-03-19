using Insthync.UnityEditorUtils;
using LiteNetLib;
using LiteNetLibManager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace MultiplayerARPG
{
    public partial class VehicleEntity : DamageableEntity, IVehicleEntity
    {
        [Category(5, "Vehicle Settings")]
        [SerializeField]
        [Tooltip("Set it more than `0` to make it uses this value instead of `GameInstance` -> `conversationDistance` as its activatable distance")]
        private float activatableDistance = 0f;

        [SerializeField]
        protected VehicleType vehicleType = null;
        public VehicleType VehicleType { get { return vehicleType; } }

        [SerializeField]
        protected VehicleMoveSpeedType moveSpeedType = VehicleMoveSpeedType.FixedMovedSpeed;

        [Tooltip("Vehicle move speed")]
        [SerializeField]
        protected float moveSpeed = 5f;

        [Tooltip("This will multiplies with driver move speed as vehicle move speed")]
        [SerializeField]
        protected float driverMoveSpeedRate = 1.5f;

        [Tooltip("First seat is for driver")]
        [SerializeField]
        protected List<VehicleSeat> seats = new List<VehicleSeat>();
        public List<VehicleSeat> Seats { get { return seats; } }

        [SerializeField]
        [Tooltip("If this is `TRUE` this entity will be able to be attacked")]
        protected bool canBeAttacked = true;

        [SerializeField]
        protected IncrementalInt hp = default;

        [SerializeField]
        [ArrayElementTitle("damageElement")]
        protected ResistanceIncremental[] resistances = new ResistanceIncremental[0];

        [SerializeField]
        [ArrayElementTitle("damageElement")]
        protected ArmorIncremental[] armors = new ArmorIncremental[0];

        [SerializeField]
        protected Buff buff = new Buff();

        [SerializeField]
        [Tooltip("Delay before the entity destroyed, you may set some delay to play destroyed animation by `onVehicleDestroy` event before it's going to be destroyed from the game.")]
        protected float destroyDelay = 2f;

        [SerializeField]
        protected float destroyRespawnDelay = 5f;

        [Category("Events")]
        public UnityEvent onVehicleDestroy = new UnityEvent();

        [Category("Sync Fields")]
        [SerializeField]
        protected SyncFieldInt level = new SyncFieldInt();
        [SerializeField]
        protected SyncListUInt passengerIds = new SyncListUInt();

        public int Level { get { return level.Value; } set { level.Value = value; } }
        public virtual bool IsDestroyWhenDriverExit { get { return false; } }
        public virtual bool HasDriver { get { return _passengers.ContainsKey(0); } }
        public Dictionary<DamageElement, float> Resistances { get; private set; }
        public Dictionary<DamageElement, float> Armors { get; private set; }
        public override bool IsInvincible { get { return base.IsInvincible || !canBeAttacked; } set { base.IsInvincible = value; } }
        public override int MaxHp { get { return canBeAttacked ? hp.GetAmount(Level) : 1; } }
        public Vector3 SpawnPosition { get; protected set; }
        public float DestroyDelay { get { return destroyDelay; } }
        public float DestroyRespawnDelay { get { return destroyRespawnDelay; } }

        protected readonly Dictionary<byte, BaseGameEntity> _passengers = new Dictionary<byte, BaseGameEntity>();
        protected readonly Dictionary<uint, UnityAction<LiteNetLibIdentity>> _spawnEvents = new Dictionary<uint, UnityAction<LiteNetLibIdentity>>();
        protected bool _isDestroyed;
        protected CalculatedBuff _cacheBuff = new CalculatedBuff();
        protected int _dirtyLevel = int.MinValue;

        protected override void EntityAwake()
        {
            base.EntityAwake();
            gameObject.tag = CurrentGameInstance.vehicleTag;
            gameObject.layer = CurrentGameInstance.vehicleLayer;
            _isDestroyed = false;
        }

        public override void InitialRequiredComponents()
        {
            CurrentGameInstance.EntitySetting.InitialVehicleEntityComponents(this);
            base.InitialRequiredComponents();
        }

        public virtual void InitStats()
        {
            if (!IsServer)
                return;
            if (Level <= 0)
                Level = 1;
            UpdateStats();
            CurrentHp = MaxHp;
        }

        /// <summary>
        /// Call this when vehicle level up
        /// </summary>
        public void UpdateStats()
        {
            if (Resistances == null)
                Resistances = new Dictionary<DamageElement, float>();
            GameDataHelpers.CombineResistances(resistances, Resistances, Level, 1);
            if (Armors == null)
                Armors = new Dictionary<DamageElement, float>();
            GameDataHelpers.CombineArmors(armors, Armors, Level, 1);
        }

        protected override void SetupNetElements()
        {
            base.SetupNetElements();
            level.syncMode = LiteNetLibSyncFieldMode.ServerToClients;
            isInvincible.syncMode = LiteNetLibSyncFieldMode.ServerToClients;
            currentHp.syncMode = LiteNetLibSyncFieldMode.ServerToClients;
            passengerIds.forOwnerOnly = false;
            passengerIds.onOperation += OnPassengerIdsOperation;
        }

        public override void OnIdentityInitialize()
        {
            base.OnIdentityInitialize();
            InitStats();
            SpawnPosition = EntityTransform.position;
            if (IsServer)
            {
                // Prepare passengers data, add data at server then it wil be synced to clients
                while (passengerIds.Count < Seats.Count)
                {
                    passengerIds.Add(0);
                }
            }
            // Vehicle must not being destroyed when owner player is disconnect to avoid vehicle exiting issues
            Identity.DoNotDestroyWhenDisconnect = true;
        }

        protected override void EntityOnDestroy()
        {
            base.EntityOnDestroy();
            passengerIds.onOperation -= OnPassengerIdsOperation;
        }

        protected override void EntityUpdate()
        {
            base.EntityUpdate();
            if (IsServer && HasDriver)
            {
                BaseGameEntity driver = GetPassenger(0);
                if (driver != null)
                {
                    if (driver.ForceHide != ForceHide)
                        ForceHide = driver.ForceHide;
                }
                else if (ForceHide)
                {
                    ForceHide = false;
                }
            }
        }

        private void OnPassengerIdsOperation(LiteNetLibSyncListOp operation, int index, uint oldItem, uint newItem)
        {
            if (index >= passengerIds.Count)
                return;
            // Set passenger entity to dictionary if the id > 0
            uint passengerId = passengerIds[index];
            if (passengerId == 0)
            {
                _passengers.Remove((byte)index);
                return;
            }
            if (Manager.Assets.TryGetSpawnedObject(passengerId, out LiteNetLibIdentity identity))
            {
                // Set the passenger
                BaseGameEntity passenger = identity.GetComponent<BaseGameEntity>();
                passenger.SetPassengingVehicle((byte)index, this);
                _passengers[(byte)index] = passenger;
            }
            else
            {
                // Create a new event to set passenger when passenger object spawn
                _spawnEvents[passengerId] = (identity) =>
                {
                    if (identity.ObjectId != passengerId)
                        return;
                    BaseGameEntity passenger = identity.GetComponent<BaseGameEntity>();
                    passenger.SetPassengingVehicle((byte)index, this);
                    _passengers[(byte)index] = passenger;
                    // Remove the event after passenger was set
                    Manager.Assets.onObjectSpawn.RemoveListener(_spawnEvents[passengerId]);
                    _spawnEvents.Remove(passengerId);
                };
                // Set the event
                Manager.Assets.onObjectSpawn.AddListener(_spawnEvents[passengerId]);
            }
        }

        public override float GetMoveSpeed_Implementation(MovementState movementState, ExtraMovementState extraMovementState)
        {
            if (moveSpeedType == VehicleMoveSpeedType.FixedMovedSpeed)
                return moveSpeed;
            if (_passengers.TryGetValue(0, out BaseGameEntity driver))
                return driver.GetMoveSpeed(movementState, extraMovementState) * driverMoveSpeedRate;
            return 0f;
        }

        protected override bool CanMove_Implementation()
        {
            if (_passengers.TryGetValue(0, out BaseGameEntity driver))
                return driver.CanMove();
            return true;
        }

        protected override bool CanJump_Implementation()
        {
            return true;
        }

        protected override bool CanTurn_Implementation()
        {
            return true;
        }

        public bool CanBePassenger(byte seatIndex, BaseGameEntity gameEntity)
        {
            return true;
        }

        public List<BaseGameEntity> GetAllPassengers()
        {
            List<BaseGameEntity> result = new List<BaseGameEntity>();
            foreach (BaseGameEntity passenger in _passengers.Values)
            {
                if (passenger)
                    result.Add(passenger);
            }
            return result;
        }

        public BaseGameEntity GetPassenger(byte seatIndex)
        {
            return _passengers[seatIndex];
        }

        public void SetPassenger(byte seatIndex, BaseGameEntity gameEntity)
        {
            if (!IsServer)
                return;
            passengerIds[seatIndex] = gameEntity.ObjectId;
        }

        public virtual bool RemovePassenger(byte seatIndex)
        {
            if (!IsServer)
                return false;
            if (seatIndex >= passengerIds.Count)
                return false;
            // Store exiting object ID
            uint passengerId = passengerIds[seatIndex];
            // Set passenger ID to `0` to tell clients that the passenger is exiting
            passengerIds[seatIndex] = 0;
            // Move passenger to exit transform
            if (Manager.TryGetEntityByObjectId(passengerId, out BaseGameEntity passenger))
            {
                if (Seats[seatIndex].exitTransform != null)
                {
                    passenger.ExitedVehicle(
                        Seats[seatIndex].exitTransform.position,
                        Seats[seatIndex].exitTransform.rotation);
                }
                else
                {
                    passenger.ExitedVehicle(
                        MovementTransform.position,
                        MovementTransform.rotation);
                }
            }
            return true;
        }

        public void RemoveAllPassengers()
        {
            if (!IsServer)
                return;
            for (byte i = 0; i < passengerIds.Count; ++i)
            {
                RemovePassenger(i);
            }
        }

        public bool IsSeatAvailable(byte seatIndex)
        {
            return !_isDestroyed && seatIndex < passengerIds.Count && passengerIds[seatIndex] == 0;
        }

        public bool GetAvailableSeat(out byte seatIndex)
        {
            seatIndex = 0;
            byte count = (byte)Seats.Count;
            for (byte i = 0; i < count; ++i)
            {
                if (IsSeatAvailable(i))
                {
                    seatIndex = i;
                    return true;
                }
            }
            return false;
        }

        public void CallRpcOnVehicleDestroy()
        {
            RPC(RpcOnVehicleDestroy, Identity.DefaultRpcChannelId, DeliveryMethod.ReliableUnordered);
        }

        [AllRpc]
        private void RpcOnVehicleDestroy()
        {
            if (onVehicleDestroy != null)
                onVehicleDestroy.Invoke();
        }

        protected override void ApplyReceiveDamage(HitBoxPosition position, Vector3 fromPosition, EntityInfo instigator, Dictionary<DamageElement, MinMaxFloat> damageAmounts, CharacterItem weapon, BaseSkill skill, int skillLevel, int randomSeed, out CombatAmountType combatAmountType, out int totalDamage)
        {
            if (!canBeAttacked)
            {
                combatAmountType = CombatAmountType.Miss;
                totalDamage = 0;
                return;
            }
            // Calculate damages
            float calculatingTotalDamage = 0f;
            foreach (DamageElement damageElement in damageAmounts.Keys)
            {
                calculatingTotalDamage += damageElement.GetDamageReducedByResistance(Resistances, Armors, damageAmounts[damageElement].Random(randomSeed));
            }
            // Apply damages
            combatAmountType = CombatAmountType.NormalDamage;
            totalDamage = CurrentGameInstance.GameplayRule.GetTotalDamage(fromPosition, instigator, this, calculatingTotalDamage, weapon, skill, skillLevel);
            if (totalDamage < 0)
                totalDamage = 0;
            CurrentHp -= totalDamage;
        }

        public override void ReceivedDamage(HitBoxPosition position, Vector3 fromPosition, EntityInfo instigator, Dictionary<DamageElement, MinMaxFloat> damageAmounts, CombatAmountType combatAmountType, int totalDamage, CharacterItem weapon, BaseSkill skill, int skillLevel, CharacterBuff buff, bool isDamageOverTime = false)
        {
            base.ReceivedDamage(position, fromPosition, instigator, damageAmounts, combatAmountType, totalDamage, weapon, skill, skillLevel, buff, isDamageOverTime);

            if (combatAmountType == CombatAmountType.Miss)
                return;

            // Do something when entity dead
            if (this.IsDead())
                Destroy();
        }

        public override bool CanReceiveDamageFrom(EntityInfo instigator)
        {
            if (passengerIds.Contains(instigator.ObjectId))
                return false;
            return base.CanReceiveDamageFrom(instigator);
        }

        public virtual void Destroy()
        {
            if (!IsServer)
                return;
            CurrentHp = 0;
            if (_isDestroyed)
                return;
            _isDestroyed = true;
            // Kick passengers
            RemoveAllPassengers();
            // Tell clients that the vehicle destroy to play animation at client
            CallRpcOnVehicleDestroy();
            // Respawning later
            if (Identity.IsSceneObject)
                Manager.StartCoroutine(RespawnRoutine());
            // Destroy this entity
            NetworkDestroy(destroyDelay);
        }

        protected IEnumerator RespawnRoutine()
        {
            yield return new WaitForSecondsRealtime(destroyDelay + destroyRespawnDelay);
            _isDestroyed = false;
            InitStats();
            Manager.Assets.NetworkSpawnScene(
                Identity.ObjectId,
                Identity.HashSceneObjectId,
                SpawnPosition,
                CurrentGameInstance.DimensionType == DimensionType.Dimension3D ? Quaternion.Euler(Vector3.up * Random.Range(0, 360)) : Quaternion.identity);
        }

        public virtual float GetActivatableDistance()
        {
            if (activatableDistance > 0f)
                return activatableDistance;
            else
                return GameInstance.Singleton.conversationDistance;
        }

        public virtual bool ShouldClearTargetAfterActivated()
        {
            return true;
        }

        public virtual bool ShouldBeAttackTarget()
        {
            return HasDriver && canBeAttacked && !this.IsDead();
        }

        public virtual bool ShouldNotActivateAfterFollowed()
        {
            return false;
        }

        public virtual bool CanActivate()
        {
            return !this.IsDead() && GameInstance.PlayingCharacterEntity.PassengingVehicleEntity == null;
        }

        public virtual void OnActivate()
        {
            if (!GetAvailableSeat(out byte seatIndex))
                return;
            GameInstance.PlayingCharacterEntity.CallCmdEnterVehicle(ObjectId, seatIndex);
        }

        private void MakeCache()
        {
            if (_dirtyLevel != Level)
            {
                _dirtyLevel = Level;
                _cacheBuff.Build(buff, Level);
            }
        }

        public CalculatedBuff GetBuff()
        {
            MakeCache();
            return _cacheBuff;
        }
    }
}
