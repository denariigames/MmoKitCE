namespace MultiplayerARPG
{
    public class UIWeaponSets : UIBase
    {
        public UIWeaponSet currentWeaponSet;
        public UIWeaponSet[] otherWeaponSets;

        protected override void OnDestroy()
        {
            base.OnDestroy();
            currentWeaponSet = null;
            otherWeaponSets.Nullify();
        }

        private void OnEnable()
        {
            UpdateData(GameInstance.PlayingCharacter);
            GameInstance.PlayingCharacterEntity.onRecached += UpdateOwningCharacterData;
        }

        private void OnDisable()
        {
            GameInstance.PlayingCharacterEntity.onRecached -= UpdateOwningCharacterData;
        }

        public void UpdateOwningCharacterData()
        {
            if (GameInstance.PlayingCharacter == null) return;
            UpdateData(GameInstance.PlayingCharacter);
        }

        public void ChangeWeaponSet(byte index)
        {
            GameInstance.ClientInventoryHandlers.RequestSwitchEquipWeaponSet(new RequestSwitchEquipWeaponSetMessage()
            {
                equipWeaponSet = index,
            }, ClientInventoryActions.ResponseSwitchEquipWeaponSet);
        }

        public void UpdateData(IPlayerCharacterData playerCharacter)
        {
            byte equipWeaponSet = playerCharacter.EquipWeaponSet;
            if (equipWeaponSet < playerCharacter.SelectableWeaponSets.Count)
                currentWeaponSet.SetData(this, equipWeaponSet, playerCharacter.SelectableWeaponSets[equipWeaponSet]);
            else
                currentWeaponSet.SetData(this, equipWeaponSet, new EquipWeapons());
            EquipWeapons tempEquipWeapons;
            byte j = 0;
            for (byte i = 0; i < GameInstance.Singleton.maxEquipWeaponSet; ++i)
            {
                if (i != equipWeaponSet && j < otherWeaponSets.Length)
                {
                    tempEquipWeapons = i < playerCharacter.SelectableWeaponSets.Count ? playerCharacter.SelectableWeaponSets[i] : new EquipWeapons();
                    otherWeaponSets[j].SetData(this, i, tempEquipWeapons);
                    ++j;
                }
            }
        }
    }
}
