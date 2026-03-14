using Insthync.ManagedUpdating;
using UnityEngine;

namespace MultiplayerARPG
{
    public class CharacterRecoveryComponent : BaseGameEntityComponent<BaseCharacterEntity>, IManagedUpdate
    {
        private float _updatingTime;
        private float _deltaTime;
        private CharacterRecoveryData _recoveryData;
        private bool _isClearRecoveryData;

        private void Awake()
        {
            _recoveryData = new CharacterRecoveryData(Entity);
            UpdateManager.Register(this);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            UpdateManager.Unregister(this);
        }

        public void ManagedUpdate()
        {
            if (!Entity.IsServer)
                return;

            _deltaTime = Time.unscaledDeltaTime;

            if (Entity.IsDead())
            {
                if (!_isClearRecoveryData)
                {
                    _isClearRecoveryData = true;
                    _recoveryData.Clear();
                }
                return;
            }
            _isClearRecoveryData = false;

            _updatingTime += _deltaTime;
            if (_updatingTime >= CurrentGameplayRule.GetRecoveryUpdateDuration())
            {
                _recoveryData.RecoveryingHp = CurrentGameplayRule.GetRecoveryHpPerSeconds(Entity);
                _recoveryData.DecreasingHp = CurrentGameplayRule.GetDecreasingHpPerSeconds(Entity);
                _recoveryData.RecoveryingMp = CurrentGameplayRule.GetRecoveryMpPerSeconds(Entity);
                _recoveryData.DecreasingMp = CurrentGameplayRule.GetDecreasingMpPerSeconds(Entity);
                _recoveryData.RecoveryingStamina = CurrentGameplayRule.GetRecoveryStaminaPerSeconds(Entity);
                _recoveryData.DecreasingStamina = CurrentGameplayRule.GetDecreasingStaminaPerSeconds(Entity);
                _recoveryData.DecreasingFood = CurrentGameplayRule.GetDecreasingFoodPerSeconds(Entity);
                _recoveryData.DecreasingWater = CurrentGameplayRule.GetDecreasingWaterPerSeconds(Entity);
                _recoveryData.Apply(_updatingTime);
                _updatingTime = 0;
            }
        }
    }
}
