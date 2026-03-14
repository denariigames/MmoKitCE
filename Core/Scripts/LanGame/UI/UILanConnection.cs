using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.UI;
using LiteNetLibManager;
using Newtonsoft.Json;

namespace MultiplayerARPG
{
    public class UILanConnection : UIBase
    {
        [Header("Lan connection UI elements")]
        public InputField inputNetworkAddress;

        [Header("Discovery connection UI elements")]
        public GameObject listEmptyObject;
        public UIDiscoveryEntry discoveryEntryPrefab;
        public Transform discoveryEntryContainer;

        public string DefaultNetworkAddress
        {
            get { return GameInstance.Singleton.NetworkSetting.networkAddress; }
        }

        public int DefaultNetworkPort
        {
            get { return GameInstance.Singleton.NetworkSetting.networkPort; }
        }

        public string NetworkAddress
        {
            get { return inputNetworkAddress == null ? DefaultNetworkAddress : inputNetworkAddress.text; }
        }

        private Dictionary<string, DiscoveryData> _discoveries = new Dictionary<string, DiscoveryData>();
        private Dictionary<string, IPEndPoint> _remoteEndPoints = new Dictionary<string, IPEndPoint>();

        private UIList _cacheList;
        public UIList CacheList
        {
            get
            {
                if (_cacheList == null)
                {
                    _cacheList = gameObject.AddComponent<UIList>();
                    _cacheList.uiPrefab = discoveryEntryPrefab.gameObject;
                    _cacheList.uiContainer = discoveryEntryContainer;
                }
                return _cacheList;
            }
        }

        private UIDiscoveryEntrySelectionManager _cacheSelectionManager;
        public UIDiscoveryEntrySelectionManager CacheSelectionManager
        {
            get
            {
                if (_cacheSelectionManager == null)
                    _cacheSelectionManager = gameObject.AddComponent<UIDiscoveryEntrySelectionManager>();
                _cacheSelectionManager.selectionMode = UISelectionMode.Toggle;
                return _cacheSelectionManager;
            }
        }

        private LiteNetLibDiscovery _cacheDiscovery;
        private LiteNetLibDiscovery CacheDiscovery
        {
            get
            {
                if (_cacheDiscovery == null)
                {
                    LanRpgNetworkManager networkManager = BaseGameNetworkManager.Singleton as LanRpgNetworkManager;
                    if (networkManager == null || networkManager.CacheDiscovery == null)
                    {
                        Debug.LogWarning("[UIDiscoveryConnection] networkManager or its discovery is empty");
                        return null;
                    }
                    _cacheDiscovery = networkManager.CacheDiscovery;
                }
                return _cacheDiscovery;
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            inputNetworkAddress = null;
            listEmptyObject = null;
            discoveryEntryPrefab = null;
            discoveryEntryContainer = null;
            _discoveries?.Clear();
            _remoteEndPoints?.Clear();
            _cacheList = null;
            _cacheSelectionManager = null;
            _cacheDiscovery = null;
        }

        private void OnEnable()
        {
            if (inputNetworkAddress != null)
                inputNetworkAddress.text = DefaultNetworkAddress;
            if (CacheDiscovery != null)
            {
                CacheDiscovery.onReceivedBroadcast += OnReceivedBroadcast;
                CacheDiscovery.StartClient();
            }
        }

        private void OnDisable()
        {
            if (CacheDiscovery != null)
            {
                CacheDiscovery.onReceivedBroadcast -= OnReceivedBroadcast;
                CacheDiscovery.StopClient();
            }
        }

        private void OnReceivedBroadcast(IPEndPoint remoteEndPoint, string data)
        {
            DiscoveryData characterData = JsonConvert.DeserializeObject<DiscoveryData>(data);
            _discoveries[characterData.id] = characterData;
            _remoteEndPoints[characterData.id] = remoteEndPoint;
            UpdateList();
        }

        private void UpdateList()
        {
            CacheSelectionManager.Clear();
            CacheList.Generate(_discoveries.Values, (index, data, ui) =>
            {
                UIDiscoveryEntry entry = ui.GetComponent<UIDiscoveryEntry>();
                entry.Data = data;
                CacheSelectionManager.Add(entry);
            });
            if (listEmptyObject != null)
                listEmptyObject.SetActive(_discoveries.Count == 0);
        }

        public IPEndPoint GetSelectedRemoteEndPoint()
        {
            UIDiscoveryEntry selectedUI = CacheSelectionManager.SelectedUI;
            return _remoteEndPoints[selectedUI.Data.id];
        }
    }
}
