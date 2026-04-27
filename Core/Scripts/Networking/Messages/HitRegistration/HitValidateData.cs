using System.Collections.Generic;

namespace MultiplayerARPG
{
    public class HitValidateData
    {
        /// <summary>
        /// Who do an attacking?
        /// </summary>
        public BaseGameEntity Attacker { get; set; }
        /// <summary>
        /// ObjectId of attacker at prepare time. Stored so validation caches can be pruned even after despawn.
        /// </summary>
        public uint AttackerObjectId { get; set; }
        /// <summary>
        /// Server timestamp when this validation data was prepared. Used to bound static validation cache lifetime.
        /// </summary>
        public long PreparedServerTimestamp { get; set; }
        /// <summary>
        /// Trigger durations (each trigger)
        /// </summary>
        public float[] TriggerDurations { get; set; }
        /// <summary>
        /// How many launched bullets each fire
        /// </summary>
        public byte FireSpread { get; set; }
        /// <summary>
        /// Which kind of attacking?
        /// </summary>
        public DamageInfo DamageInfo { get; set; }
        /// <summary>
        /// Damage amounts each trigger
        /// </summary>
        public List<Dictionary<DamageElement, MinMaxFloat>> DamageAmounts { get; set; }
        /// <summary>
        /// Attack by left-hand weapon?, while aiming?, while in FPS view mode?
        /// </summary>
        public WeaponHandlingState WeaponHandlingState { get; set; }
        /// <summary>
        /// Weapon which being used for attacking
        /// </summary>
        public CharacterItem Weapon { get; set; }
        /// <summary>
        /// Skill which being used for attacking
        /// </summary>
        public BaseSkill Skill { get; set; }
        /// <summary>
        /// Skill level which being used for attacking
        /// </summary>
        public int SkillLevel { get; set; }
        /// <summary>
        /// How many hits were applied will be stored in this collection.
        /// Used for hack avoidance. Must be cleared whenever this object is reused for a new attack validation.
        /// </summary>
        public Dictionary<string, int> HitsCount { get; } = new Dictionary<string, int>();
        /// <summary>
        /// Object IDs that were hit by this attacking will be stored in this collection.
        /// Must be cleared whenever this object is reused for a new attack validation.
        /// </summary>
        public HashSet<string> HitObjects { get; } = new HashSet<string>();
        /// <summary>
        /// Set any validator data here.
        /// </summary>
        public object ValidatorData { get; set; }

        public void ResetForReuse()
        {
            HitsCount.Clear();
            HitObjects.Clear();
            ValidatorData = null;
        }

        public void ClearReferences()
        {
            Attacker = null;
            AttackerObjectId = 0;
            PreparedServerTimestamp = 0;
            TriggerDurations = null;
            FireSpread = 0;
            DamageInfo = null;
            DamageAmounts = null;
            WeaponHandlingState = default;
            Weapon = default;
            Skill = null;
            SkillLevel = 0;
            ResetForReuse();
        }
    }
}
