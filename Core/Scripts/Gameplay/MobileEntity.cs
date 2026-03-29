// ------------------------------------------------------------------------------
// MobileEntity.cs
// ------------------------------------------------------------------------------
// Purpose:
//   New base-class layer for *moving, player-like* entities (“Mobiles”).
//
// Why this exists:
//   BaseGameEntity is currently overloaded (movement + animation + mounts + generic entity).
//   We’re introducing a clean “Mobiles” layer so players can converge onto a better base,
//   without forcing every static entity to pay for Mobile features.
//
//   MobileEntity inherits BaseGameEntity 1:1 for compatibility.
//   We only set Mobile-centric defaults here (notably server simulation policy).
//
//   Move Move/Animation/Mount partials from BaseGameEntity -> MobileEntity,
//   and strip BaseGameEntity down to a minimal “core entity” layer.
// ------------------------------------------------------------------------------

namespace MultiplayerARPG
{
    /// <summary>
    /// “Mobiles” base. Intended for PlayerCharacterEntity (and other player-like movers).
    /// </summary>
    public abstract partial class MobileEntity : BaseGameEntity
    {
        /// <summary>
        /// Mobiles should usually keep simulating on the server even when unobserved,
        /// because they typically have timers/state that must continue (regen, cooldowns,
        /// DOTs, status effects, mount state, etc.).
        ///
        /// This directly affects the early-out gates in BaseGameEntity.ManagedUpdate()
        /// and BaseGameEntity.IsUpdateEntityComponents.
        /// </summary>
        protected override bool NeedsServerUpdateWhenUnobserved => true;
    }
}
