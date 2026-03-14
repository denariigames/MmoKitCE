using System.Collections.Generic;
using UnityEngine;

namespace MultiplayerARPG
{
    [RequireComponent(typeof(BaseEquipmentEntity))]
    public class ToggleObjectsByEquipmentEntitySockets : MonoBehaviour
    {
        [System.Serializable]
        public class Setting
        {
            public BaseItem socketEnhancerItem;
            public GameObject[] togglingObjects = new GameObject[0];
        }
        public GameObject[] defaultTogglingObjects = new GameObject[0];
        public Setting[] settingsBySocketEnhancerItem = new Setting[0];

        private BaseEquipmentEntity _equipmentEntity;
        private List<GameObject> _allTogglingObjects = new List<GameObject>();
        private Dictionary<int, Setting> _cacheSettingsBySocketEnhancerItem;
        public Dictionary<int, Setting> CacheSettingsBySocketEnhancerItem
        {
            get
            {
                if (_cacheSettingsBySocketEnhancerItem == null)
                {
                    _cacheSettingsBySocketEnhancerItem = new Dictionary<int, Setting>();
                    for (int i = 0; i < settingsBySocketEnhancerItem.Length; ++i)
                    {
                        if (settingsBySocketEnhancerItem[i].socketEnhancerItem == null || !settingsBySocketEnhancerItem[i].socketEnhancerItem.IsSocketEnhancer())
                            continue;
                        if (settingsBySocketEnhancerItem[i].togglingObjects.Length <= 0)
                            continue;
                        if (!_cacheSettingsBySocketEnhancerItem.ContainsKey(settingsBySocketEnhancerItem[i].socketEnhancerItem.DataId))
                            _cacheSettingsBySocketEnhancerItem[settingsBySocketEnhancerItem[i].socketEnhancerItem.DataId] = settingsBySocketEnhancerItem[i];
                        _allTogglingObjects.AddRange(settingsBySocketEnhancerItem[i].togglingObjects);
                    }
                }
                return _cacheSettingsBySocketEnhancerItem;
            }
        }

        private void Awake()
        {
            _equipmentEntity = GetComponent<BaseEquipmentEntity>();
            _equipmentEntity.onItemChanged.AddListener(OnItemChanged);
            _allTogglingObjects.AddRange(defaultTogglingObjects);
        }

        private void OnDestroy()
        {
            _equipmentEntity.onItemChanged.RemoveListener(OnItemChanged);
        }

        private void OnItemChanged(CharacterItem item)
        {
            BaseItem itemData = null;
            for (int i = 0; i < item.sockets.Count; ++i)
            {
                if (!GameInstance.Items.TryGetValue(item.sockets[i], out itemData) || !itemData.IsSocketEnhancer())
                {
                    itemData = null;
                    continue;
                }
                break;
            }

            for (int i = 0; i < _allTogglingObjects.Count; ++i)
            {
                if (_allTogglingObjects[i] == null)
                    continue;
                if (_allTogglingObjects[i].activeSelf)
                    _allTogglingObjects[i].SetActive(false);
            }

            if (itemData != null && CacheSettingsBySocketEnhancerItem.TryGetValue(itemData.DataId, out Setting setting))
            {
                for (int i = 0; i < setting.togglingObjects.Length; ++i)
                {
                    if (setting.togglingObjects[i] == null)
                        continue;
                    if (setting.togglingObjects[i].activeSelf)
                        setting.togglingObjects[i].SetActive(true);
                }
            }
            else
            {
                for (int i = 0; i < defaultTogglingObjects.Length; ++i)
                {
                    if (defaultTogglingObjects[i] == null)
                        continue;
                    if (defaultTogglingObjects[i].activeSelf)
                        defaultTogglingObjects[i].SetActive(true);
                }
            }
        }
    }
}
