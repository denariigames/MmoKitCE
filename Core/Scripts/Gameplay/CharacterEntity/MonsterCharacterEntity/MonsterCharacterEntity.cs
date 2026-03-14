using LiteNetLibManager;

namespace MultiplayerARPG
{
    public partial class MonsterCharacterEntity : BaseMonsterCharacterEntity
    {
        public override void InitialRequiredComponents()
        {
            base.InitialRequiredComponents();
            if (Movement == null)
                Logging.LogWarning(ToString(), "Did not setup entity movement component to this entity.");
        }
    }
}
