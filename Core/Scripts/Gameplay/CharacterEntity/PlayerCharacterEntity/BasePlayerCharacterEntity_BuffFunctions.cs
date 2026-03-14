namespace MultiplayerARPG
{
    public partial class BasePlayerCharacterEntity
    {
        public override void OnApplyBuff(CharacterBuff characterBuff)
        {
            base.OnApplyBuff(characterBuff);
            if (IsServer)
                GameInstance.ServerLogHandlers.LogBuffApply(this, characterBuff);
        }

        public override void OnRemoveBuff(CharacterBuff characterBuff, BuffRemoveReasons reason)
        {
            base.OnRemoveBuff(characterBuff, reason);
            if (IsServer)
                GameInstance.ServerLogHandlers.LogBuffRemove(this, characterBuff, reason);
        }
    }
}