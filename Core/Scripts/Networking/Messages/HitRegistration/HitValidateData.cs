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
        /// Attack by left-hand weapon?, while aimming?, while in FPS view mode?
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
        /// How many hits were applied will be stored in this collection
        /// It will being used for hack avoidance purposes
        /// </summary>
        public Dictionary<string, int> HitsCount { get; } = new Dictionary<string, int>();
        /// <summary>
        /// Object IDs that were hit by this attacking will be stored in this collection
        /// If object were hit it might not allow to being hitted again (up to implementation)
        /// </summary>
        public HashSet<string> HitObjects { get; } = new HashSet<string>();
        /// <summary>
        /// Set any validator data here
        /// </summary>
        public object ValidatorData { get; set; }
    }
}
