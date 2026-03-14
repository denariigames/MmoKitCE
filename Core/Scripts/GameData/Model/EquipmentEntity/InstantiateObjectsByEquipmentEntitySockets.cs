using System.Collections.Generic;
using UnityEngine;

namespace MultiplayerARPG
{
    public class InstantiateObjectsByEquipmentEntitySockets : MonoBehaviour
    {
        [System.Serializable]
        public class ObjectSetting
        {
            public GameObject prefab;
            public Vector3 localPosition = Vector3.zero;
            public Vector3 localEulerAngles = Vector3.zero;
            public Vector3 localScale = Vector3.one;
        }

        [System.Serializable]
        public class Setting
        {
            public BaseItem socketEnhancerItem;
            public ObjectSetting[] instantiatingObjects = new ObjectSetting[0];
        }
        public ObjectSetting[] defaultInstantiatingObjects = new ObjectSetting[0];
        public Setting[] settingsBySocketEnhancerItem = new Setting[0];
        public Transform container;

        private BaseEquipmentEntity _equipmentEntity;
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
                        if (settingsBySocketEnhancerItem[i].instantiatingObjects.Length <= 0)
                            continue;
                        if (!_cacheSettingsBySocketEnhancerItem.ContainsKey(settingsBySocketEnhancerItem[i].socketEnhancerItem.DataId))
                            _cacheSettingsBySocketEnhancerItem[settingsBySocketEnhancerItem[i].socketEnhancerItem.DataId] = settingsBySocketEnhancerItem[i];
                    }
                }
                return _cacheSettingsBySocketEnhancerItem;
            }
        }

        private void Awake()
        {
            _equipmentEntity = GetComponent<BaseEquipmentEntity>();
            _equipmentEntity.onItemChanged.AddListener(OnItemChanged);
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

            container.DestroyChildren();
            GameObject tempNewObject;
            if (itemData != null && CacheSettingsBySocketEnhancerItem.TryGetValue(itemData.DataId, out Setting setting))
            {
                for (int i = 0; i < setting.instantiatingObjects.Length; ++i)
                {
                    if (setting.instantiatingObjects[i].prefab == null)
                        continue;
                    tempNewObject = Instantiate(setting.instantiatingObjects[i].prefab, container);
                    if (tempNewObject != null)
                    {
                        tempNewObject.transform.localPosition = setting.instantiatingObjects[i].localPosition;
                        tempNewObject.transform.localEulerAngles = setting.instantiatingObjects[i].localEulerAngles;
                        tempNewObject.transform.localScale = setting.instantiatingObjects[i].localScale;
                    }
                }
            }
            else
            {
                for (int i = 0; i < defaultInstantiatingObjects.Length; ++i)
                {
                    if (defaultInstantiatingObjects[i].prefab == null)
                        continue;
                    tempNewObject = Instantiate(defaultInstantiatingObjects[i].prefab, container);
                    if (tempNewObject != null)
                    {
                        tempNewObject.transform.localPosition = defaultInstantiatingObjects[i].localPosition;
                        tempNewObject.transform.localEulerAngles = defaultInstantiatingObjects[i].localEulerAngles;
                        tempNewObject.transform.localScale = defaultInstantiatingObjects[i].localScale;
                    }    
                }
            }
        }
    }
}
