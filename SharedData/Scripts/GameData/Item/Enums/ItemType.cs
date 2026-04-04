namespace MultiplayerARPG
{
    /// <summary>
    /// You may doubt why it still has this enum while it has an interface for item classes declaration.
    /// It is because the old `Item` class implementation uses enum to define the kind of item.
    /// So... to make it still able to use old `Item` class, I have to use this enum
    /// to define kind of item because the `Item` class has to implements all item interfaces.
    /// If you want to create consumable item like potion, spell scrolls you will have to implements
    /// `IUsableItem` interface and set `ItemType` to `Potion` (I actually think `Potion` should be renamed to `Consumable`,
    /// but I afraid that it may affects customer projects.)
    /// </summary>
    public enum ItemType : byte
    {
        Junk,
        Armor,
        Weapon,
        Shield,
        Potion,
        Ammo,
        Building,
        Pet,
        SocketEnhancer,
        Mount,
        Skill
    }
}
