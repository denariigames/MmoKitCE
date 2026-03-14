namespace MultiplayerARPG
{
    public abstract partial class BaseMonsterActivityComponent : BaseGameEntityComponent<BaseMonsterCharacterEntity>
    {
        public MonsterCharacter CharacterDatabase
        {
            get { return Entity.CharacterDatabase; }
        }
    }
}
