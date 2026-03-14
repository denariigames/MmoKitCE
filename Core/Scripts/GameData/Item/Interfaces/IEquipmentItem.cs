namespace MultiplayerARPG
{
    public partial interface IEquipmentItem : IItem, IItemWithRequirement, IItemWithStatusEffectApplyings
    {
        /// <summary>
        /// Equipment set, if character equipping the same set of items, it can increase extra stats to character
        /// </summary>
        EquipmentSet EquipmentSet { get; }
        /// <summary>
        /// Max durability
        /// </summary>
        float MaxDurability { get; }
        /// <summary>
        /// If this is `TRUE` this item will be destroyed if broken (current durability = 0)
        /// </summary>
        bool DestroyIfBroken { get; }
        /// <summary>
        /// Its length is max amount of enhancement sockets
        /// </summary>
        SocketEnhancerType[] AvailableSocketEnhancerTypes { get; }
        /// <summary>
        /// Equipment models, these models will be instantiated when equipping this item, for weapons it will be instantiated when equipping to main-hand (right-hand)
        /// </summary>
        EquipmentModel[] EquipmentModels { get; }
        /// <summary>
        /// Increasing stats while equipping this item
        /// </summary>
        CharacterStatsIncremental IncreaseStats { get; }
        /// <summary>
        /// Increasing stats rate while equipping this item
        /// </summary>
        CharacterStatsIncremental IncreaseStatsRate { get; }
        /// <summary>
        /// Increasing attributes while equipping this item
        /// </summary>
        AttributeIncremental[] IncreaseAttributes { get; }
        /// <summary>
        /// Increasing attributes rate while equipping this item
        /// </summary>
        AttributeIncremental[] IncreaseAttributesRate { get; }
        /// <summary>
        /// Increasing resistances while equipping this item
        /// </summary>
        ResistanceIncremental[] IncreaseResistances { get; }
        /// <summary>
        /// Increasing armors stats while equipping this item
        /// </summary>
        ArmorIncremental[] IncreaseArmors { get; }
        /// <summary>
        /// Increasing armors rate stats while equipping this item
        /// </summary>
        ArmorIncremental[] IncreaseArmorsRate { get; }
        /// <summary>
        /// Increasing damages stats while equipping this item
        /// </summary>
        DamageIncremental[] IncreaseDamages { get; }
        /// <summary>
        /// Increasing damages rate stats while equipping this item
        /// </summary>
        DamageIncremental[] IncreaseDamagesRate { get; }
        /// <summary>
        /// Increasing skills while equipping this item
        /// </summary>
        SkillIncremental[] IncreaseSkills { get; }
        /// <summary>
        /// Increasing status effect resistances while equipping this item
        /// </summary>
        StatusEffectResistanceIncremental[] IncreaseStatusEffectResistances { get; }
        /// <summary>
        /// Random bonus
        /// </summary>
        ItemRandomBonus RandomBonus { get; }
    }
}
