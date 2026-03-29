// ce scalability: #4 adds ServerTickDriver and removes UpdateManager

using UnityEngine;

namespace MultiplayerARPG
{
    public class CharacterRecoveryComponent : BaseGameEntityComponent<BaseCharacterEntity>
    {
        private float _updatingTime;
        private CharacterRecoveryData _recoveryData;
        private bool _isClearRecoveryData;

        private void Start()
        {
            _recoveryData = new CharacterRecoveryData(Entity);
            CharacterRecoveryTickDriver.Register(this);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            CharacterRecoveryTickDriver.Unregister(this);
        }

        public void TickRecovery(float deltaTime)
        {
            if (Entity == null || _recoveryData == null)
                return;
            if (!Entity.IsServer)
                return;

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

            _updatingTime += deltaTime;
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
