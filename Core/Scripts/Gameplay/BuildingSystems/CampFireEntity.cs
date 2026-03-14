using Cysharp.Text;
using Cysharp.Threading.Tasks;
using Insthync.UnityEditorUtils;
using LiteNetLibManager;
using LiteNetLib;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace MultiplayerARPG
{
    public partial class CampFireEntity : StorageEntity
    {
        [Category(7, "Campfire Settings")]
        [SerializeField]
        protected ConvertItem[] convertItems = new ConvertItem[0];
        public ConvertItem[] ConvertItems { get { return convertItems; } }

        [Category("Events")]
        [SerializeField]
        protected UnityEvent onInitialTurnOn = new UnityEvent();
        [SerializeField]
        protected UnityEvent onInitialTurnOff = new UnityEvent();
        [SerializeField]
        protected UnityEvent onTurnOn = new UnityEvent();
        [SerializeField]
        protected UnityEvent onTurnOff = new UnityEvent();

        [Category("Sync Fields")]
        [SerializeField]
        protected SyncFieldBool isTurnOn = new SyncFieldBool();
        [SerializeField]
        protected SyncFieldFloat turnOnElapsed = new SyncFieldFloat();

        public bool IsTurnOn
        {
            get { return isTurnOn.Value; }
            set { isTurnOn.Value = value; }
        }

        public float TurnOnElapsed
        {
            get { return turnOnElapsed.Value; }
            set { turnOnElapsed.Value = value; }
        }

        public override string ExtraData
        {
            get
            {
                return ZString.Concat(IsTurnOn ? byte.MaxValue : byte.MinValue, ':', TurnOnElapsed);
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                    return;
                string[] splitedTexts = value.Split(':');
                byte isTurnOn;
                if (splitedTexts.Length == 2 && byte.TryParse(splitedTexts[0], out isTurnOn))
                    IsTurnOn = isTurnOn != 0;
                float turnOnElapsed;
                if (splitedTexts.Length == 2 && float.TryParse(splitedTexts[1], out turnOnElapsed))
                    TurnOnElapsed = turnOnElapsed;
            }
        }

        protected readonly Dictionary<int, float> _convertRemainsDuration = new Dictionary<int, float>();

        protected Dictionary<int, ConvertItem> _cacheFuelItems;
        public Dictionary<int, ConvertItem> CacheFuelItems
        {
            get
            {
                if (_cacheFuelItems == null)
                {
                    _cacheFuelItems = new Dictionary<int, ConvertItem>();
                    if (convertItems != null && convertItems.Length > 0)
                    {
                        foreach (ConvertItem convertItem in convertItems)
                        {
                            if (convertItem.item.item == null || !convertItem.isFuel) continue;
                            _cacheFuelItems[convertItem.item.item.DataId] = convertItem;
                        }
                    }
                }
                return _cacheFuelItems;
            }
        }

        protected Dictionary<int, ConvertItem> _cacheConvertItems;
        public Dictionary<int, ConvertItem> CacheConvertItems
        {
            get
            {
                if (_cacheConvertItems == null)
                {
                    _cacheConvertItems = new Dictionary<int, ConvertItem>();
                    if (convertItems != null && convertItems.Length > 0)
                    {
                        foreach (ConvertItem convertItem in convertItems)
                        {
                            if (convertItem.item.item == null) continue;
                            _cacheConvertItems[convertItem.item.item.DataId] = convertItem;
                        }
                    }
                }
                return _cacheConvertItems;
            }
        }

        protected List<StorageConvertItemsEntry> _preparedConvertItems = new List<StorageConvertItemsEntry>();
        protected float _convertCountDown = 1f;

        public override void OnSetup()
        {
            base.OnSetup();
            isTurnOn.onChange += OnIsTurnOnChange;
        }

        protected override void SetupNetElements()
        {
            base.SetupNetElements();
            isTurnOn.syncMode = LiteNetLibSyncFieldMode.ServerToClients;
            turnOnElapsed.syncMode = LiteNetLibSyncFieldMode.ServerToClients;
        }

        protected override void EntityOnDestroy()
        {
            base.EntityOnDestroy();
            isTurnOn.onChange -= OnIsTurnOnChange;
        }

        private void OnIsTurnOnChange(bool isInitial, bool oldIsTurnOn, bool isTurnOn)
        {
            if (isInitial)
            {
                if (isTurnOn)
                    onInitialTurnOn.Invoke();
                else
                    onInitialTurnOff.Invoke();
            }
            else
            {
                if (isTurnOn)
                    onTurnOn.Invoke();
                else
                    onTurnOff.Invoke();
            }
        }

        protected override void EntityUpdate()
        {
            base.EntityUpdate();
            if (!IsServer)
                return;

            if (!IsTurnOn)
            {
                if (_convertRemainsDuration.Count > 0)
                    _convertRemainsDuration.Clear();
                return;
            }

            if (!CanTurnOn())
            {
                IsTurnOn = false;
                TurnOnElapsed = 0f;
                return;
            }

            // Consume fuel and convert item
            float tempDeltaTime = Time.unscaledDeltaTime;
            TurnOnElapsed += tempDeltaTime;

            HashSet<int> convertedItem = new HashSet<int>();
            ConvertItem convertData;
            List<CharacterItem> items = new List<CharacterItem>(GameInstance.ServerStorageHandlers.GetStorageEntityItems(this));
            CharacterItem tempItem;
            for (int i = 0; i < items.Count; ++i)
            {
                tempItem = items[i];
                if (!CacheConvertItems.ContainsKey(tempItem.dataId))
                    continue;

                if (convertedItem.Contains(tempItem.dataId))
                    continue;

                convertedItem.Add(tempItem.dataId);

                convertData = CacheConvertItems[tempItem.dataId];

                if (!_convertRemainsDuration.ContainsKey(tempItem.dataId))
                    _convertRemainsDuration.Add(tempItem.dataId, convertData.convertInterval);

                _convertRemainsDuration[tempItem.dataId] -= tempDeltaTime;

                if (_convertRemainsDuration[tempItem.dataId] <= 0f)
                {
                    _convertRemainsDuration[tempItem.dataId] = convertData.convertInterval;
                    PrepareConvertItems(convertData);
                }
            }

            if (_convertCountDown > 0f)
            {
                _convertCountDown -= tempDeltaTime;
                if (_convertCountDown <= 0f)
                {
                    _convertCountDown = 1f;
                    ProceedConvertItems();
                }
            }
        }

        protected void PrepareConvertItems(ConvertItem convertData)
        {
            StorageConvertItemsEntry convertItemsEntry = new StorageConvertItemsEntry();
            if (convertData.item.item != null && convertData.item.amount > 0)
            {
                int dataId = convertData.item.item.DataId;
                int amount = convertData.item.amount;
                convertItemsEntry.dataId = dataId;
                convertItemsEntry.amount = amount;
            }

            if (convertData.convertedItem.item != null && convertData.convertedItem.amount > 0)
            {
                int convertedDataId = convertData.convertedItem.item.DataId;
                int convertedAmount = convertData.convertedItem.amount;
                convertItemsEntry.convertedDataId = convertedDataId;
                convertItemsEntry.convertedAmount = convertedAmount;
            }

            if (convertItemsEntry.amount + convertItemsEntry.convertedAmount > 0)
                _preparedConvertItems.Add(convertItemsEntry);
        }

        protected async void ProceedConvertItems()
        {
            if (_preparedConvertItems.Count <= 0)
                return;
            StorageId storageId = new StorageId(StorageType.Building, Id);
            List<CharacterItem> droppingItems = new List<CharacterItem>();
            bool isConverted = await GameInstance.ServerStorageHandlers.ConvertStorageItems(storageId, _preparedConvertItems, droppingItems);
            if (!isConverted)
                return;
            _preparedConvertItems.Clear();
            if (droppingItems != null && droppingItems.Count > 0)
            {
                for (int i = 0; i < droppingItems.Count; ++i)
                {
                    // Drop item on ground
                    ItemDropEntity.Drop(this, RewardGivenType.BuildingDrop, droppingItems[i], System.Array.Empty<string>()).Forget();
                }
            }
        }

        public void TurnOn()
        {
            if (!CanTurnOn())
                return;
            IsTurnOn = true;
            TurnOnElapsed = 0f;
        }

        public void TurnOff()
        {
            IsTurnOn = false;
            TurnOnElapsed = 0f;
        }

        public bool CanTurnOn()
        {
            if (CacheFuelItems.Count == 0)
            {
                // Not require fuel
                return true;
            }
            List<CharacterItem> items = GameInstance.ServerStorageHandlers.GetStorageEntityItems(this);
            Dictionary<int, int> countItems = new Dictionary<int, int>();
            foreach (CharacterItem item in items)
            {
                if (CacheFuelItems.ContainsKey(item.dataId))
                {
                    if (!countItems.ContainsKey(item.dataId))
                        countItems.Add(item.dataId, 0);
                    countItems[item.dataId] += item.amount;
                    if (countItems[item.dataId] >= CacheFuelItems[item.dataId].item.amount)
                        return true;
                }
            }
            return false;
        }

        public override void PrepareRelatesData()
        {
            base.PrepareRelatesData();
            List<BaseItem> items = new List<BaseItem>();
            if (convertItems != null && convertItems.Length > 0)
            {
                foreach (ConvertItem convertItem in convertItems)
                {
                    items.Add(convertItem.item.item);
                    items.Add(convertItem.convertedItem.item);
                }
            }
            GameInstance.AddItems(items);
        }
    }
}
