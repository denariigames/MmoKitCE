using System.Collections.Generic;
using MultiplayerARPG.Server.Scheduling;
using MultiplayerARPG.Server.Time;
using UnityEngine;

namespace MultiplayerARPG.Server.AI
{
    // MonsterAISystem
    // Centralized AI decision system for monsters.
    //
    // - Runs on a fixed tick rate (e.g. 5 Hz)
    // - Server-authoritative
    // - Replaces AI decision logic previously run in Update()
    // - Does NOT handle movement interpolation
    // - ONLY makes gameplay decisions
    ///
    public sealed class MonsterAISystem : ITickSystem
    {
        public string Name => "MonsterAI";

        // Registry of active monsters
        // This avoids FindObjectsOfType and keeps things deterministic
        private static readonly HashSet<BaseMonsterCharacterEntity> monsters = new HashSet<BaseMonsterCharacterEntity>();

        // Registration API
        // Called from monster lifecycle events (OnSetup / OnDestroy)
        public static void Register(BaseMonsterCharacterEntity monster)
        {
            if (monster != null && monster.IsServer)
                monsters.Add(monster);
        }

        public static void Unregister(BaseMonsterCharacterEntity monster)
        {
            if (monster != null)
                monsters.Remove(monster);
        }

        // No-op for now, but exists for future threading.
        public void Prepare(in TickContext ctx)
        {
            // Intentionally empty
        }

        // This is where Update()-based AI logic now lives.
        public void Execute(in TickContext ctx)
        {
            foreach (var monster in monsters)
            {
                if (monster == null || monster.IsDead())
                    continue;
                monster.TickAI(ctx.FixedDelta);
            }
        }

        
        // Authoritative state application.
        // Currently unused because TickAI mutates state directly,
        public void Commit(in TickContext ctx)
        {
            // No-op
        }
    }
}
