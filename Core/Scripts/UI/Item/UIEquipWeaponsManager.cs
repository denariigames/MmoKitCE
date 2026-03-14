using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MultiplayerARPG
{
    public class UIEquipWeaponsManager : MonoBehaviour
    {
        [Tooltip("Index of this array is equip weapons set")]
        public ActivatingGameObjects[] activatingGameObjects;

        private byte dirtyEquipWeaponSet;
        private bool updatingActivatingGameObjects;

        private void Awake()
        {
            for (int i = 0; i < activatingGameObjects.Length; ++i)
            {
                if (activatingGameObjects[i].toggle == null)
                    continue;
                int j = i;
                activatingGameObjects[i].toggle.onValueChanged.AddListener((isOn) =>
                {
                    if (updatingActivatingGameObjects)
                        return;

                    if (isOn)
                    {
                        OnClickSwitchEquipWeaponSet(activatingGameObjects[j].equipWeaponSet);
                    }
                });
            }
        }

        private void Start()
        {
            updatingActivatingGameObjects = false;
            dirtyEquipWeaponSet = GameInstance.PlayingCharacter.EquipWeaponSet;
            UpdateActivatingGameObjects();
        }

        private void LateUpdate()
        {
            if (GameInstance.PlayingCharacter == null)
                return;

            if (dirtyEquipWeaponSet != GameInstance.PlayingCharacter.EquipWeaponSet)
            {
                dirtyEquipWeaponSet = GameInstance.PlayingCharacter.EquipWeaponSet;
                UpdateActivatingGameObjects();
            }
        }

        private void UpdateActivatingGameObjects()
        {
            updatingActivatingGameObjects = true;
            for (int i = 0; i < activatingGameObjects.Length; ++i)
            {
                if (activatingGameObjects[i].toggle != null)
                    activatingGameObjects[i].toggle.isOn = activatingGameObjects[i].equipWeaponSet == GameInstance.PlayingCharacter.EquipWeaponSet;

                for (int j = 0; j < activatingGameObjects[i].gameObjects.Length; ++j)
                {
                    if (activatingGameObjects[i].gameObjects[j] == null)
                        continue;
                    activatingGameObjects[i].gameObjects[j].SetActive(activatingGameObjects[i].equipWeaponSet == GameInstance.PlayingCharacter.EquipWeaponSet);
                }
            }
            updatingActivatingGameObjects = false;
        }

        public void OnClickSwitchEquipWeaponSet(byte equipWeaponSet)
        {
            if (equipWeaponSet == GameInstance.PlayingCharacter.EquipWeaponSet)
                return;
            GameInstance.ClientInventoryHandlers.RequestSwitchEquipWeaponSet(new RequestSwitchEquipWeaponSetMessage()
            {
                equipWeaponSet = equipWeaponSet,
            }, ClientInventoryActions.ResponseSwitchEquipWeaponSet);
        }

        [System.Serializable]
        public struct ActivatingGameObjects
        {
            [Range(0, 15)]
            public byte equipWeaponSet;
            public Toggle toggle;
            public GameObject[] gameObjects;
        }
    }
}
