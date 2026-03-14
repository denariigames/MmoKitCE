namespace MultiplayerARPG
{
    public partial class BaseCharacterEntity
    {
        public const int FRAMES_BEFORE_UPDATE_AMMO_SIM = 4;

        public System.Action<int> onUpdateRightWeaponAmmoSim;
        public System.Action<int> onUpdateLeftWeaponAmmoSim;

        protected int _rightWeaponAmmoSim;
        public int RightWeaponAmmoSim
        {
            get
            {
                return _rightWeaponAmmoSim;
            }
            set
            {
                if (value < 0)
                    value = 0;
                if (_rightWeaponAmmoSim != value)
                {
                    _rightWeaponAmmoSim = value;
                    onUpdateRightWeaponAmmoSim?.Invoke(value);
                }
            }
        }
        protected int _leftWeaponAmmoSim;
        public int LeftWeaponAmmoSim
        {
            get
            {
                return _leftWeaponAmmoSim;
            }
            set
            {
                if (value < 0)
                    value = 0;
                if (_leftWeaponAmmoSim != value)
                {
                    _leftWeaponAmmoSim = value;
                    onUpdateLeftWeaponAmmoSim?.Invoke(value);
                }
            }
        }

        protected int _countDownToUpdateAmmoSim = FRAMES_BEFORE_UPDATE_AMMO_SIM;
        public void MarkToUpdateAmmoSim()
        {
            if (_countDownToUpdateAmmoSim > 0)
                return;
            _countDownToUpdateAmmoSim = FRAMES_BEFORE_UPDATE_AMMO_SIM;
        }

        public void UpdateAmmoSim()
        {
            if (!EquipWeapons.rightHand.IsEmptySlot() &&
                EquipWeapons.rightHand.GetWeaponItem() != null)
                RightWeaponAmmoSim = EquipWeapons.rightHand.ammo;
            else
                RightWeaponAmmoSim = 0;

            if (!EquipWeapons.leftHand.IsEmptySlot() &&
                EquipWeapons.leftHand.GetWeaponItem() != null)
                LeftWeaponAmmoSim = EquipWeapons.leftHand.ammo;
            else
                LeftWeaponAmmoSim = 0;
        }
    }
}
