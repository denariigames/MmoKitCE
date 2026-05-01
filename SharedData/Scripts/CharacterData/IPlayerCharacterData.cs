using System.Collections.Generic;

namespace MultiplayerARPG
{
    public partial interface IPlayerCharacterData : ICharacterData
    {
        /// <summary>
        /// User Account ID
        /// </summary>
        string UserId { get; set; }
        /// <summary>
        /// Stat point which uses for increase attribute amount
        /// </summary>
        float StatPoint { get; set; }
        /// <summary>
        /// Skill point which uses for increase skill level
        /// </summary>
        float SkillPoint { get; set; }
        /// <summary>
        /// Gold which uses for buy things
        /// </summary>
        int Gold { get; set; }
        /// <summary>
        /// Gold which store in the bank
        /// </summary>
        int UserGold { get; set; }
        /// <summary>
        /// Cash which uses for buy special items
        /// </summary>
        int UserCash { get; set; }
        /// <summary>
        /// Joined party id
        /// </summary>
        int PartyId { get; set; }
        /// <summary>
        /// Joined guild id
        /// </summary>
        int GuildId { get; set; }
        /// <summary>
        /// Current guild role
        /// </summary>
        byte GuildRole { get; set; }
        string CurrentChannel { get; set; }
        /// <summary>
        /// Current Map Name will be work with MMORPG system only
        /// For Lan game it will be scene name which set in game instance
        /// </summary>
        string CurrentMapName { get; set; }
        Vec3 CurrentPosition { get; set; }
        Vec3 CurrentRotation { get; set; }
        string CurrentSafeArea { get; set; }
#if !DISABLE_DIFFER_MAP_RESPAWNING
        /// <summary>
        /// Respawn Map Name will be work with MMORPG system only
        /// For Lan game it will be scene name which set in game instance
        /// </summary>
        string RespawnMapName { get; set; }
        Vec3 RespawnPosition { get; set; }
#endif
        long LastDeadTime { get; set; }
        long UnmuteTime { get; set; }
        long LastUpdate { get; set; }
#if !DISABLE_CLASSIC_PK
        bool IsPkOn { get; set; }
        long LastPkOnTime { get; set; }
        int PkPoint { get; set; }
        int ConsecutivePkKills { get; set; }
        int HighestPkPoint { get; set; }
        int HighestConsecutivePkKills { get; set; }
#endif
        IList<CharacterHotkey> Hotkeys { get; set; }
        IList<CharacterQuest> Quests { get; set; }
#if !DISABLE_CUSTOM_CHARACTER_CURRENCIES
        IList<CharacterCurrency> Currencies { get; set; }
#endif
#if !DISABLE_CUSTOM_CHARACTER_DATA
        /// <summary>
        /// Server boolean will not being synced with clients
        /// </summary>
        IList<CharacterDataBoolean> ServerBools { get; set; }
        /// <summary>
        /// Server integer will not being synced with clients
        /// </summary>
        IList<CharacterDataInt32> ServerInts { get; set; }
        /// <summary>
        /// Server float will not being synced with clients
        /// </summary>
        IList<CharacterDataFloat32> ServerFloats { get; set; }
        /// <summary>
        /// Private boolean will be synced to owner client only
        /// </summary>
        IList<CharacterDataBoolean> PrivateBools { get; set; }
        /// <summary>
        /// Private integer will be synced to owner client only
        /// </summary>
        IList<CharacterDataInt32> PrivateInts { get; set; }
        /// <summary>
        /// Private float will be synced to owner client only
        /// </summary>
        IList<CharacterDataFloat32> PrivateFloats { get; set; }
        /// <summary>
        /// Public boolean will be synced to all clients
        /// </summary>
        IList<CharacterDataBoolean> PublicBools { get; set; }
        /// <summary>
        /// Public integer will be synced to all clients
        /// </summary>
        IList<CharacterDataInt32> PublicInts { get; set; }
        /// <summary>
        /// Public float will be synced to all clients
        /// </summary>
        IList<CharacterDataFloat32> PublicFloats { get; set; }
#endif
        IList<CharacterSkill> GuildSkills { get; set; }
    }
}
