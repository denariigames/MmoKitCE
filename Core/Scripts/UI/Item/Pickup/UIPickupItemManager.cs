using Cysharp.Text;
using System.Collections.Generic;
using UnityEngine;

namespace MultiplayerARPG
{
    public class UIPickupItemManager : MonoBehaviour
    {
        public UIPickupItemList uiList;
        public GameObject[] signalObjects;
        public NearbyEntityDetector ItemDropEntityDetector { get; protected set; }

        private void Awake()
        {
            GameObject tempGameObject = new GameObject("_ItemDropEntityDetector");
            ItemDropEntityDetector = tempGameObject.AddComponent<NearbyEntityDetector>();
            ItemDropEntityDetector.detectingRadius = GameInstance.Singleton.pickUpItemDistance;
            ItemDropEntityDetector.findItemDrop = true;
            ItemDropEntityDetector.findRewardDrop = true;
            ItemDropEntityDetector.onUpdateList += OnUpdate;
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
            if (ItemDropEntityDetector != null)
            {
                ItemDropEntityDetector.onUpdateList -= OnUpdate;
                Destroy(ItemDropEntityDetector.gameObject);
            }
        }

        private void OnUpdate()
        {
            // Prepare dropped items
            List<CharacterItem> droppedItems = new List<CharacterItem>();
            string tempEntryId;
            CharacterItem tempCharacterItem;
            foreach (ItemDropEntity entity in ItemDropEntityDetector.itemDrops)
            {
                using (Utf16ValueStringBuilder strBuilder = ZString.CreateStringBuilder(true))
                {
                    strBuilder.Append(entity.ObjectId);
                    tempEntryId = strBuilder.ToString();
                }
                tempCharacterItem = entity.ItemDropData.characterItem;
                tempCharacterItem.id = tempEntryId;
                droppedItems.Add(tempCharacterItem);
            }
            foreach (BaseRewardDropEntity entity in ItemDropEntityDetector.rewardDrops)
            {
                if (entity.RepresentItem == null)
                    continue;
                using (Utf16ValueStringBuilder strBuilder = ZString.CreateStringBuilder(true))
                {
                    strBuilder.Append(entity.ObjectId);
                    tempEntryId = strBuilder.ToString();
                }
                tempCharacterItem = CharacterItem.Create(entity.RepresentItem, 1, entity.Amount);
                tempCharacterItem.id = tempEntryId;
                droppedItems.Add(tempCharacterItem);
            }

            // Update list
            if (uiList != null)
            {
                if (droppedItems.Count == 0)
                {
                    if (uiList.IsVisible())
                        uiList.Hide();
                }
                else
                {
                    uiList.UpdateData(droppedItems);
                    if (!uiList.IsVisible())
                        uiList.Show();
                }
            }
            // Update signal objects
            if (signalObjects != null && signalObjects.Length > 0)
            {
                foreach (GameObject signalObject in signalObjects)
                {
                    signalObject.SetActive(droppedItems.Count > 0);
                }
            }
        }
    }
}
