namespace MultiplayerARPG
{
    public partial class UIDealingRequest : UISelectionEntry<BasePlayerCharacterEntity>
    {
        public UICharacter uiAnotherCharacter;

        protected override void OnDestroy()
        {
            base.OnDestroy();
            uiAnotherCharacter = null;
            _data = null;
        }

        protected override void UpdateData()
        {
            BasePlayerCharacterEntity anotherCharacter = Data;

            if (uiAnotherCharacter != null)
            {
                uiAnotherCharacter.NotForOwningCharacter = true;
                uiAnotherCharacter.Data = anotherCharacter;
            }
        }

        public void OnClickAccept()
        {
            GameInstance.PlayingCharacterEntity.DealingComponent.CallCmdAcceptDealingRequest();
            Hide();
        }

        public void OnClickDecline()
        {
            GameInstance.PlayingCharacterEntity.DealingComponent.CallCmdDeclineDealingRequest();
            Hide();
        }
    }
}
