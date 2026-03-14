using Cysharp.Threading.Tasks;
using LiteNetLibManager;
using UnityEngine;

namespace MultiplayerARPG
{
    public partial class BaseCharacterEntity
    {

        protected float _lastMountTime;

        public virtual async void SpawnMount(MountType mountType, string sourceId, float duration, int level = 1, int currentHp = 0)
        {
            if (!IsServer)
                return;

            if (mountType == MountType.None)
                return;

            if (Time.unscaledTime - _lastMountTime < CurrentGameInstance.mountDelay)
                return;

            _lastMountTime = Time.unscaledTime;

            Vector3 enterPosition = EntityTransform.position;
            if (PassengingVehicleEntity != null)
            {
                enterPosition = PassengingVehicleEntity.Entity.EntityTransform.position;
                await ExitVehicle();
            }

            // Instantiate new mount entity
            LiteNetLibIdentity spawnObj = null;
            if (mountType.GetPrefab(this, sourceId
                , out VehicleEntity prefab
#if !DISABLE_ADDRESSABLES
                , out AssetReferenceVehicleEntity addressablePrefab
#endif
                ))
            {
#if !DISABLE_ADDRESSABLES
                // Update mount data
                Mount = new CharacterMount()
                {
                    type = mountType,
                    sourceId = sourceId,
                    mountRemainsDuration = duration,
                    level = level,
                    currentHp = currentHp,
                };
                spawnObj = BaseGameNetworkManager.Singleton.Assets.GetObjectInstance(
                    addressablePrefab.HashAssetId, enterPosition,
                    Quaternion.Euler(0, EntityTransform.eulerAngles.y, 0));
#endif
            }
            else if (prefab != null)
            {
                // Update mount data
                Mount = new CharacterMount()
                {
                    type = mountType,
                    sourceId = sourceId,
                    mountRemainsDuration = duration,
                    level = level,
                    currentHp = currentHp,
                };
                spawnObj = BaseGameNetworkManager.Singleton.Assets.GetObjectInstance(
                    prefab.Identity.HashAssetId, enterPosition,
                    Quaternion.Euler(0, EntityTransform.eulerAngles.y, 0));
            }
            else
            {
                Mount = new CharacterMount();
                return;
            }


            VehicleEntity vehicle = spawnObj.GetComponent<VehicleEntity>();
            vehicle.InitStats();
            vehicle.Level = level;
            if (currentHp <= 0f)
                currentHp = vehicle.MaxHp;
            vehicle.CurrentHp = currentHp;
            BaseGameNetworkManager.Singleton.Assets.NetworkSpawn(spawnObj, 0, ConnectionId);

            // Seat index for mount entity always 0
            await EnterVehicle(vehicle, 0);
        }

        public async override UniTask<bool> ExitVehicle()
        {
            if (await base.ExitVehicle())
            {
                Mount = new CharacterMount();
                return true;
            }
            return false;
        }

        public override void SetPassengingVehicle(byte seatIndex, IVehicleEntity vehicleEntity)
        {
            base.SetPassengingVehicle(seatIndex, vehicleEntity);
            IsRecaching = true;
        }

        public override bool CanEnterVehicle(IVehicleEntity vehicleEntity, byte seatIndex, out UITextKeys gameMessage)
        {
            if (!base.CanEnterVehicle(vehicleEntity, seatIndex, out gameMessage))
                return false;

            if (!IsGameEntityInDistance(vehicleEntity))
            {
                gameMessage = UITextKeys.UI_ERROR_CHARACTER_IS_TOO_FAR;
                return false;
            }

            return true;
        }

        public override bool PlayerCanExitVehicle()
        {
            // Can't exit from buff's mount
            if (Mount.type == MountType.Buff)
                return false;
            return true;
        }
    }
}