using Insthync.UnityEditorUtils;
using System.Collections.Generic;
using UnityEngine;

namespace MultiplayerARPG
{
    public partial class WorkbenchEntity : BuildingEntity
    {
        [Category(6, "Workbench Settings")]
        [SerializeField]
        protected ItemCraft[] itemCrafts = new ItemCraft[0];
        public ItemCraft[] ItemCrafts { get { return itemCrafts; } }


        private Dictionary<int, ItemCraft> _cacheItemCrafts;
        public Dictionary<int, ItemCraft> CacheItemCrafts
        {
            get
            {
                if (_cacheItemCrafts == null)
                {
                    _cacheItemCrafts = new Dictionary<int, ItemCraft>();
                    foreach (ItemCraft itemCraft in itemCrafts)
                    {
                        if (itemCraft.CraftingItem == null)
                            continue;
                        _cacheItemCrafts[itemCraft.CraftingItem.DataId] = itemCraft;
                    }
                }
                return _cacheItemCrafts;
            }
        }

        public void CraftItem(BasePlayerCharacterEntity playerCharacterEntity, int dataId)
        {
            ItemCraft itemCraft;
            if (!CacheItemCrafts.TryGetValue(dataId, out itemCraft))
                return;

            UITextKeys gameMessage;
            if (!itemCraft.CanCraft(playerCharacterEntity, out gameMessage))
                GameInstance.ServerGameMessageHandlers.SendGameMessage(playerCharacterEntity.ConnectionId, gameMessage);
            else
                itemCraft.CraftItem(playerCharacterEntity);
        }

        public override void PrepareRelatesData()
        {
            base.PrepareRelatesData();
            if (CacheItemCrafts.Count > 0)
            {
                List<BaseItem> items = new List<BaseItem>();
                foreach (ItemCraft itemCraft in CacheItemCrafts.Values)
                {
                    items.Add(itemCraft.CraftingItem);
                    GameInstance.AddItems(itemCraft.RequireItems);
                    GameInstance.AddCurrencies(itemCraft.RequireCurrencies);
                }
                GameInstance.AddItems(items);
            }
        }

        public override bool CanActivate()
        {
            if (Identity != null && Identity.IsServer && GameInstance.PlayingCharacterEntity != null && Identity.IsHideFrom(GameInstance.PlayingCharacterEntity.Identity))
                return false;
            return !this.IsDead();
        }

        public override void OnActivate()
        {
            BaseUISceneGameplay.Singleton.ShowWorkbenchDialog(this);
        }
    }
}
