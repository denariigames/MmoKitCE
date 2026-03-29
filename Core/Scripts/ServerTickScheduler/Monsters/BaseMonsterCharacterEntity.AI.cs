using UnityEngine;

namespace MultiplayerARPG
{
    
    // BaseMonsterCharacterEntity (AI Extension)
    // This method is driven by MonsterAISystem ticks.
    // It replaces frame-based AI logic previously in EntityUpdate().
    public abstract partial class BaseMonsterCharacterEntity
    {
        /// <summary>
        /// Server-authoritative AI tick.
        /// Called at a fixed rate by MonsterAISystem.
        /// </summary>
        public virtual void TickAI(double tickTime)
        {
            // Only run on server
            if (!IsServer)
                return;

            // Example: summoned monster follow logic
            if (!IsSummoned)
                return;

            if (!SummonerEntity || SummonerEntity.IsDead())
            {
                UnSummon();
                return;
            }

            if (Vector3.Distance(
                    EntityTransform.position,
                    SummonerEntity.EntityTransform.position)
                > CurrentGameInstance.maxFollowSummonerDistance &&
                tickTime - _lastTeleportToSummonerTime > TELEPORT_TO_SUMMONER_DELAY)
            {
                Teleport(
                    GameInstance.Singleton.GameplayRule.GetSummonPosition(SummonerEntity),
                    GameInstance.Singleton.GameplayRule.GetSummonRotation(SummonerEntity),
                    false
                );

                _lastTeleportToSummonerTime = (float)tickTime;
            }
        }
        public static class AITick
        {
            public static uint Current;
        }

    }
}
