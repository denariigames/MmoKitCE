using UnityEngine;

namespace MultiplayerARPG
{
    public class UIAmmoAmount : UIBase
    {
        [Header("Right Hand Ammo Amount")]
        public GameObject uiRightHandAmmoRoot;
        public TextWrapper uiTextRightHandCurrentAmmo;
        public TextWrapper uiTextRightHandReserveAmmo;
        public TextWrapper uiTextRightHandSumAmmo;
        public GameObject[] rightHandRequireAmmoSymbols;
        public GameObject[] rightHandNoRequireAmmoSymbols;
        public UIGageValue gageRightHandAmmo;
        [Header("Left Hand Ammo Amount")]
        public GameObject uiLeftHandAmmoRoot;
        public TextWrapper uiTextLeftHandCurrentAmmo;
        public TextWrapper uiTextLeftHandReserveAmmo;
        public TextWrapper uiTextLeftHandSumAmmo;
        public GameObject[] leftHandRequireAmmoSymbols;
        public GameObject[] leftHandNoRequireAmmoSymbols;
        public UIGageValue gageLeftHandAmmo;

        protected override void OnDestroy()
        {
            base.OnDestroy();
            uiRightHandAmmoRoot = null;
            uiTextRightHandCurrentAmmo = null;
            uiTextRightHandReserveAmmo = null;
            uiTextRightHandSumAmmo = null;
            rightHandRequireAmmoSymbols.Nullify();
            rightHandNoRequireAmmoSymbols.Nullify();
            gageRightHandAmmo = null;
            uiLeftHandAmmoRoot = null;
            uiTextLeftHandCurrentAmmo = null;
            uiTextLeftHandReserveAmmo = null;
            uiTextLeftHandSumAmmo = null;
            leftHandRequireAmmoSymbols.Nullify();
            leftHandNoRequireAmmoSymbols.Nullify();
            gageLeftHandAmmo = null;
        }

        protected virtual void OnEnable()
        {
            UpdateOwningCharacterData();
            if (!GameInstance.PlayingCharacterEntity) return;
            GameInstance.PlayingCharacterEntity.onUpdateRightWeaponAmmoSim += OnUpdateRightWeaponAmmoSim;
            GameInstance.PlayingCharacterEntity.onUpdateLeftWeaponAmmoSim += OnUpdateLeftWeaponAmmoSim;
        }

        protected virtual void OnDisable()
        {
            if (!GameInstance.PlayingCharacterEntity) return;
            GameInstance.PlayingCharacterEntity.onUpdateRightWeaponAmmoSim -= OnUpdateRightWeaponAmmoSim;
            GameInstance.PlayingCharacterEntity.onUpdateLeftWeaponAmmoSim -= OnUpdateLeftWeaponAmmoSim;
        }

        public void UpdateOwningCharacterData()
        {
            if (!GameInstance.PlayingCharacterEntity) return;
            UpdateData();
        }

        public void UpdateData()
        {
            UpdateUI(uiRightHandAmmoRoot, uiTextRightHandCurrentAmmo, uiTextRightHandReserveAmmo, uiTextRightHandSumAmmo, rightHandRequireAmmoSymbols, rightHandNoRequireAmmoSymbols, gageRightHandAmmo, GameInstance.PlayingCharacterEntity.EquipWeapons.rightHand, false);
            UpdateUI(uiLeftHandAmmoRoot, uiTextLeftHandCurrentAmmo, uiTextLeftHandReserveAmmo, uiTextLeftHandSumAmmo, leftHandRequireAmmoSymbols, leftHandNoRequireAmmoSymbols, gageLeftHandAmmo, GameInstance.PlayingCharacterEntity.EquipWeapons.leftHand, true);
        }

        public void OnUpdateRightWeaponAmmoSim(int ammo)
        {
            UpdateUI(uiRightHandAmmoRoot, uiTextRightHandCurrentAmmo, uiTextRightHandReserveAmmo, uiTextRightHandSumAmmo, rightHandRequireAmmoSymbols, rightHandNoRequireAmmoSymbols, gageRightHandAmmo, GameInstance.PlayingCharacterEntity.EquipWeapons.rightHand, false);
        }

        public void OnUpdateLeftWeaponAmmoSim(int ammo)
        {
            UpdateUI(uiLeftHandAmmoRoot, uiTextLeftHandCurrentAmmo, uiTextLeftHandReserveAmmo, uiTextLeftHandSumAmmo, leftHandRequireAmmoSymbols, leftHandNoRequireAmmoSymbols, gageLeftHandAmmo, GameInstance.PlayingCharacterEntity.EquipWeapons.leftHand, true);
        }

        protected virtual void UpdateUI(GameObject root, TextWrapper textCurrentAmmo, TextWrapper textReserveAmmo, TextWrapper textSumAmmo, GameObject[] requireAmmoSymbols, GameObject[] noRequireAmmoSymbols, UIGageValue gageAmmo, CharacterItem characterItem, bool isLeftHand)
        {
            IWeaponItem weaponItem = characterItem.GetWeaponItem();
            bool isActive = weaponItem != null && (weaponItem.WeaponType.AmmoType != null || weaponItem.AmmoItemIds.Count > 0);
            if (root != null)
                root.SetActive(isActive);

            int currentAmmo = characterItem.ammo;
            int reserveAmmo = 0;

            if (GameInstance.PlayingCharacterEntity && isActive)
            {
                if (!isLeftHand)
                {
                    currentAmmo = GameInstance.PlayingCharacterEntity.RightWeaponAmmoSim;
                }
                else
                {
                    currentAmmo = GameInstance.PlayingCharacterEntity.LeftWeaponAmmoSim;
                }
                reserveAmmo = GameInstance.PlayingCharacterEntity.CountAllAmmos(weaponItem);
            }

            if (textCurrentAmmo != null)
            {
                textCurrentAmmo.SetGameObjectActive(isActive && weaponItem.AmmoCapacity > 0);
                if (isActive)
                    textCurrentAmmo.text = currentAmmo.ToString("N0");
            }

            if (textReserveAmmo != null)
            {
                textReserveAmmo.SetGameObjectActive(isActive);
                if (isActive)
                    textReserveAmmo.text = reserveAmmo.ToString("N0");
            }

            if (textSumAmmo != null)
            {
                textSumAmmo.SetGameObjectActive(isActive);
                textSumAmmo.text = (currentAmmo + reserveAmmo).ToString("N0");
            }

            if (requireAmmoSymbols != null)
            {
                foreach (GameObject symbol in requireAmmoSymbols)
                {
                    symbol.SetActive(isActive);
                }
            }

            if (noRequireAmmoSymbols != null)
            {
                foreach (GameObject symbol in noRequireAmmoSymbols)
                {
                    symbol.SetActive(!isActive);
                }
            }

            if (gageAmmo != null)
            {
                gageAmmo.SetVisible(isActive && weaponItem.AmmoCapacity > 0);
                if (isActive)
                    gageAmmo.Update(currentAmmo, weaponItem.AmmoCapacity);
            }
        }
    }
}
