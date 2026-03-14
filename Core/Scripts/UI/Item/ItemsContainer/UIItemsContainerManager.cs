using UnityEngine;

namespace MultiplayerARPG
{
    public class UIItemsContainerManager : MonoBehaviour
    {
        public UIItemsContainer uiList;
        public GameObject[] signalObjects;
        public NearbyEntityDetector ItemsContainerEntityDetector { get; protected set; }

        private void Awake()
        {
            GameObject tempGameObject = new GameObject("_ItemsContainerEntityDetector");
            ItemsContainerEntityDetector = tempGameObject.AddComponent<NearbyEntityDetector>();
            ItemsContainerEntityDetector.detectingRadius = GameInstance.Singleton.pickUpItemDistance;
            ItemsContainerEntityDetector.findItemsContainer = true;
            ItemsContainerEntityDetector.onUpdateList += OnUpdate;
            if (uiList != null)
            {
                if (uiList.IsVisible())
                    uiList.Hide();
            }
            if (signalObjects != null && signalObjects.Length > 0)
            {
                foreach (GameObject signalObject in signalObjects)
                {
                    signalObject.SetActive(false);
                }
            }
        }

        private void OnDestroy()
        {
            if (ItemsContainerEntityDetector != null)
            {
                ItemsContainerEntityDetector.onUpdateList -= OnUpdate;
                Destroy(ItemsContainerEntityDetector.gameObject);
            }
        }

        private void OnUpdate()
        {
            // Update list
            if (uiList != null)
            {
                if (ItemsContainerEntityDetector.itemsContainers.Count == 0)
                {
                    if (uiList.IsVisible())
                        uiList.Hide();
                }
                else
                {
                    uiList.UpdateData(ItemsContainerEntityDetector.itemsContainers[0]);
                    if (!uiList.IsVisible())
                        uiList.Show();
                }
            }
            // Update signal objects
            if (signalObjects != null && signalObjects.Length > 0)
            {
                foreach (GameObject signalObject in signalObjects)
                {
                    signalObject.SetActive(ItemsContainerEntityDetector.itemsContainers.Count > 0);
                }
            }
        }
    }
}
