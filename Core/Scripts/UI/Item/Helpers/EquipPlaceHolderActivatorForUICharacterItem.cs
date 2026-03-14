using UnityEngine;

namespace MultiplayerARPG
{
    [RequireComponent(typeof(UICharacterItem))]
    public class EquipPlaceHolderActivatorForUICharacterItem : MonoBehaviour
    {
        public GameObject[] activateWhileEquippedObjects;
        public GameObject[] activateWhileUnEquippedObjects;
        private UICharacterItem ui;

        private void Start()
        {
            ui = GetComponent<UICharacterItem>();
            ui.onSetEquippedData.AddListener(OnSetEquippedData);
            ui.onSetUnEquippedData.AddListener(OnSetUnEquippedData);
            ui.onSetUnEquippableData.AddListener(OnSetUnEquippedData);
            // Refresh UI data to applies events
            ui.ForceUpdate();
        }

        public void OnSetEquippedData()
        {
            if (activateWhileEquippedObjects != null &&
                activateWhileEquippedObjects.Length > 0)
            {
                foreach (GameObject gameObject in activateWhileEquippedObjects)
                {
                    if (gameObject == null) continue;
                    gameObject.SetActive(true);
                }
            }
            if (activateWhileUnEquippedObjects != null &&
                activateWhileUnEquippedObjects.Length > 0)
            {
                foreach (GameObject gameObject in activateWhileUnEquippedObjects)
                {
                    if (gameObject == null) continue;
                    gameObject.SetActive(false);
                }
            }
        }

        public void OnSetUnEquippedData()
        {
            if (activateWhileEquippedObjects != null &&
                activateWhileEquippedObjects.Length > 0)
            {
                foreach (GameObject gameObject in activateWhileEquippedObjects)
                {
                    if (gameObject == null) continue;
                    gameObject.SetActive(false);
                }
            }
            if (activateWhileUnEquippedObjects != null &&
                activateWhileUnEquippedObjects.Length > 0)
            {
                foreach (GameObject gameObject in activateWhileUnEquippedObjects)
                {
                    if (gameObject == null) continue;
                    gameObject.SetActive(true);
                }
            }
        }
    }
}
