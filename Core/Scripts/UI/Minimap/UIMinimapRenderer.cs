using Insthync.DevExtension;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MultiplayerARPG
{
    public partial class UIMinimapRenderer : MonoBehaviour
    {
        public struct MarkerData
        {
            public bool IsRequiredEntity { get; set; }
            public BaseGameEntity Entity { get; set; }
            public IMapMarker MapMarker { get; set; }
            public RectTransform Marker { get; set; }
            public RectTransform Prefab { get; set; }
            public Vector3 MarkerRotateOffsets { get; set; }
            public string MapMarkerType { get; set; }
        }

        [Serializable]
        public struct MapMarkerPrefabData
        {
            public string mapMarkerType;
            public RectTransform prefab;
            public Vector3 markerRotateOffsets;
        }

        public enum MinimapMode
        {
            Default,
            FollowPlayingCharacter,
        }

        public enum MinimapType
        {
            Type1,
            Type2,
        }

        [Header("Settings")]
        public MinimapMode mode = MinimapMode.Default;
        public MinimapType type = MinimapType.Type2;
        [Tooltip("Marker's anchor min, max and pivot must be 0.5")]
        public RectTransform playingCharacterMarker;
        public Vector3 playingCharacterRotateOffsets = Vector3.zero;
        [Tooltip("Marker's anchor min, max and pivot must be 0.5")]
        public RectTransform followingCameraMarker;
        public Vector3 followingCameraRotateOffsets = Vector3.zero;
        [Tooltip("Marker's anchor min, max and pivot must be 0.5")]
        public RectTransform allyMemberMarkerPrefab;
        public Vector3 allyMemberRotateOffsets = Vector3.zero;
        [Tooltip("Marker's anchor min, max and pivot must be 0.5")]
        public RectTransform partyMemberMarkerPrefab;
        public Vector3 partyMemberRotateOffsets = Vector3.zero;
        [Tooltip("Marker's anchor min, max and pivot must be 0.5")]
        public RectTransform guildMemberMarkerPrefab;
        public Vector3 guildMemberRotateOffsets = Vector3.zero;
        [Tooltip("Marker's anchor min, max and pivot must be 0.5")]
        public RectTransform enemyMarkerPrefab;
        public Vector3 enemyRotateOffsets = Vector3.zero;
        [Tooltip("Marker's anchor min, max and pivot must be 0.5")]
        public RectTransform neutralMarkerPrefab;
        public Vector3 neutralRotateOffsets = Vector3.zero;
        public MapMarkerPrefabData[] mapMarkerPrefabs = new MapMarkerPrefabData[0];

        [Space]
        public float allyMarkerDistance = 10_000f;
        public float enemyOrNeutralMarkerDistance = 5f;
        public float updateMarkerDuration = 1f;
        [Tooltip("Image's anchor min, max and pivot must be 0.5")]
        public Image imageMinimap;
        public ScrollRect scrollRectMinimap;
        [Header("Testing")]
        public bool isTestMode;
        public BaseMapInfo testingMapInfo;
        public Transform testingPlayingCharacterTransform;

        // Events
        public Action onInstantiateEntitiesMarkersStart;
        public Action<uint> onInstantiateGuildMemberMarker;
        public Action<uint> onInstantiatePartyMemberMarker;
        public Action<uint> onInstantiateAllyMemberMarker;
        public Action<uint> onInstantiateEnemyMarker;
        public Action<uint> onInstantiateNeutralMarker;
        public Action onInstantiateEntitiesMarkersFinish;

        protected bool _markContainersPrepared = false;
        protected RectTransform _nonPlayingCharacterMarkerContainer;
        protected RectTransform _mapMarkerContainer;
        protected RectTransform _playingCharacterMarkerContainer;
        protected float _updateMarkerCountdown;
        protected BaseMapInfo _currentMapInfo;
        protected List<MarkerData> _markers = new List<MarkerData>();
        protected Dictionary<int, Queue<RectTransform>> _markersPool = new Dictionary<int, Queue<RectTransform>>();
        protected Dictionary<string, MapMarkerPrefabData> _cachedMapMarkerPrefabs;
        public Dictionary<string, MapMarkerPrefabData> CachedMapMarkerPrefabs
        {
            get
            {
                if (_cachedMapMarkerPrefabs == null)
                {
                    _cachedMapMarkerPrefabs = new Dictionary<string, MapMarkerPrefabData>();
                    for (int i = 0; i < mapMarkerPrefabs.Length; ++i)
                    {
                        MapMarkerPrefabData prefabData = mapMarkerPrefabs[i];
                        if (string.IsNullOrWhiteSpace(prefabData.mapMarkerType) || prefabData.prefab == null)
                            continue;
                        _cachedMapMarkerPrefabs[prefabData.mapMarkerType] = prefabData;
                    }
                }
                return _cachedMapMarkerPrefabs;
            }
        }
        protected Dictionary<string, bool> CachedMapMarkerVisibleState = new Dictionary<string, bool>();
        public float CurrentSizeRate { get; protected set; } = 1f;

        public void SetModeToDefault()
        {
            mode = MinimapMode.Default;
        }

        public void SetModeToFollowPlayingCharacter()
        {
            mode = MinimapMode.FollowPlayingCharacter;
        }

        protected virtual void Awake()
        {
            if (imageMinimap == null)
            {
                enabled = false;
                Debug.LogWarning($"[{nameof(UIMinimapRenderer)}] No image minimap set.", this);
                return;
            }
            DevExtUtils.InvokeInstanceDevExtMethods(this, "Awake");
            PrepareMarkerContainer();
        }

        protected virtual void PrepareMarkerContainer()
        {
            if (_markContainersPrepared)
                return;
            _markContainersPrepared = true;

            imageMinimap.rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            imageMinimap.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            imageMinimap.rectTransform.pivot = new Vector2(0.5f, 0.5f);

            _mapMarkerContainer = new GameObject("_mapMarkerContainer", typeof(RectTransform)).GetComponent<RectTransform>();
            _mapMarkerContainer.SetParent(imageMinimap.transform);
            _mapMarkerContainer.anchorMin = new Vector2(0.5f, 0.5f);
            _mapMarkerContainer.anchorMax = new Vector2(0.5f, 0.5f);
            _mapMarkerContainer.pivot = new Vector2(0.5f, 0.5f);
            _mapMarkerContainer.sizeDelta = imageMinimap.rectTransform.rect.size;
            _mapMarkerContainer.anchoredPosition = Vector2.zero;
            _mapMarkerContainer.transform.localScale = Vector3.one;

            _nonPlayingCharacterMarkerContainer = new GameObject("_nonPlayingCharacterMarkerContainer", typeof(RectTransform)).GetComponent<RectTransform>();
            _nonPlayingCharacterMarkerContainer.SetParent(imageMinimap.transform);
            _nonPlayingCharacterMarkerContainer.anchorMin = new Vector2(0.5f, 0.5f);
            _nonPlayingCharacterMarkerContainer.anchorMax = new Vector2(0.5f, 0.5f);
            _nonPlayingCharacterMarkerContainer.pivot = new Vector2(0.5f, 0.5f);
            _nonPlayingCharacterMarkerContainer.sizeDelta = imageMinimap.rectTransform.rect.size;
            _nonPlayingCharacterMarkerContainer.anchoredPosition = Vector2.zero;
            _nonPlayingCharacterMarkerContainer.transform.localScale = Vector3.one;

            _playingCharacterMarkerContainer = new GameObject("_playingCharacterMarkerContainer", typeof(RectTransform)).GetComponent<RectTransform>();
            _playingCharacterMarkerContainer.SetParent(imageMinimap.transform);
            _playingCharacterMarkerContainer.anchorMin = new Vector2(0.5f, 0.5f);
            _playingCharacterMarkerContainer.anchorMax = new Vector2(0.5f, 0.5f);
            _playingCharacterMarkerContainer.pivot = new Vector2(0.5f, 0.5f);
            _playingCharacterMarkerContainer.sizeDelta = imageMinimap.rectTransform.rect.size;
            _playingCharacterMarkerContainer.anchoredPosition = Vector2.zero;
            _playingCharacterMarkerContainer.transform.localScale = Vector3.one;

            if (followingCameraMarker != null)
            {
                followingCameraMarker.SetParent(_playingCharacterMarkerContainer.transform);
                followingCameraMarker.anchorMin = new Vector2(0.5f, 0.5f);
                followingCameraMarker.anchorMax = new Vector2(0.5f, 0.5f);
                followingCameraMarker.pivot = new Vector2(0.5f, 0.5f);
                followingCameraMarker.anchoredPosition = Vector2.zero;
                followingCameraMarker.transform.localScale = Vector3.one;
            }

            if (playingCharacterMarker != null)
            {
                playingCharacterMarker.SetParent(_playingCharacterMarkerContainer.transform);
                playingCharacterMarker.anchorMin = new Vector2(0.5f, 0.5f);
                playingCharacterMarker.anchorMax = new Vector2(0.5f, 0.5f);
                playingCharacterMarker.pivot = new Vector2(0.5f, 0.5f);
                playingCharacterMarker.anchoredPosition = Vector2.zero;
                playingCharacterMarker.transform.localScale = Vector3.one;
            }
        }

        protected virtual void Update()
        {
            UpdateMapInfo();
        }

        public void UpdateMapInfo()
        {
            BaseMapInfo mapInfo = isTestMode ? testingMapInfo : BaseGameNetworkManager.CurrentMapInfo;

            if (mapInfo == null)
            {
                _updateMarkerCountdown = 0f;
                if (imageMinimap.gameObject.activeSelf)
                    imageMinimap.gameObject.SetActive(false);
                return;
            }

            bool changeOccurs = _currentMapInfo != mapInfo;
            _currentMapInfo = mapInfo;
            UpdateMinimap(changeOccurs);
        }

        protected async void UpdateMinimap(bool changeOccurs)
        {
            // TODO: May calculate with marker's anchor to find proper marker's position

            // Use bounds size to calculate transforms
            float boundsWidth = _currentMapInfo.MinimapBoundsWidth;
            float boundsLength = _currentMapInfo.MinimapBoundsLength;
            float maxBoundsSize = Mathf.Max(boundsWidth, boundsLength);

            // Prepare target transform to follow
            Transform playingCharacterTransform = null;
            if (isTestMode)
                playingCharacterTransform = testingPlayingCharacterTransform;
            else if (GameInstance.PlayingCharacterEntity != null)
                playingCharacterTransform = GameInstance.PlayingCharacterEntity.EntityTransform;
            if (playingCharacterTransform == null)
                return;

            if (imageMinimap != null)
            {
                if (changeOccurs)
                {
                    Sprite spr = null;
                    switch (type)
                    {
                        case MinimapType.Type1:
                            spr = await _currentMapInfo.GetMinimapSprite();
                            break;
                        case MinimapType.Type2:
                            spr = await _currentMapInfo.GetMinimapSprite2();
                            break;
                    }
                    imageMinimap.sprite = spr;
                }

                if (!imageMinimap.gameObject.activeSelf)
                    imageMinimap.gameObject.SetActive(true);

                float imageSizeX = imageMinimap.rectTransform.sizeDelta.x;
                float imageSizeY = imageMinimap.rectTransform.sizeDelta.y;
                float minImageSize = Mathf.Min(imageSizeX, imageSizeY);

                CurrentSizeRate = -(minImageSize / maxBoundsSize);

                _updateMarkerCountdown -= Time.deltaTime;
                if (_updateMarkerCountdown <= 0f)
                {
                    _updateMarkerCountdown = updateMarkerDuration;
                    InstantiateAllMarkers();
                }

                UpdateMarkersPositionAndVisibility(CurrentSizeRate);

                if (followingCameraMarker != null)
                {
                    IGameplayCameraController gameplayCamera = BasePlayerCharacterController.Singleton.GetComponent<IGameplayCameraController>();
                    if (gameplayCamera != null)
                        SetMarkerPositionAndRotation(followingCameraMarker, playingCharacterTransform.position, gameplayCamera.CameraTransform.eulerAngles, CurrentSizeRate, followingCameraRotateOffsets);
                    else
                        SetMarkerPositionAndRotation(followingCameraMarker, playingCharacterTransform, CurrentSizeRate, followingCameraRotateOffsets);
                }

                if (playingCharacterMarker != null)
                {
                    SetMarkerPositionAndRotation(playingCharacterMarker, playingCharacterTransform, CurrentSizeRate, playingCharacterRotateOffsets);
                }

                if (mode == MinimapMode.Default)
                {
                    imageMinimap.transform.localPosition = Vector2.zero;
                }
                else
                {
                    if (scrollRectMinimap != null)
                    {
                        // marker map pos
                        GetMarkerPositionAndAngles(playingCharacterTransform.position, playingCharacterTransform.eulerAngles, CurrentSizeRate, followingCameraRotateOffsets, out Vector3 markerPosition, out _);
                        float calcX = (markerPosition.x + (imageMinimap.rectTransform.sizeDelta.x * 0.5f)) / imageMinimap.rectTransform.sizeDelta.x;
                        float calcY = (markerPosition.y + (imageMinimap.rectTransform.sizeDelta.y * 0.5f)) / imageMinimap.rectTransform.sizeDelta.y;
                        scrollRectMinimap.normalizedPosition = new Vector2(calcX, calcY);
                    }
                    else
                    {
                        float x;
                        float y;
                        switch (GameInstance.Singleton.DimensionType)
                        {
                            case DimensionType.Dimension2D:
                                x = _currentMapInfo.MinimapPosition.x - playingCharacterTransform.position.x;
                                y = _currentMapInfo.MinimapPosition.y - playingCharacterTransform.position.y;
                                break;
                            default:
                                x = _currentMapInfo.MinimapPosition.x - playingCharacterTransform.position.x;
                                y = _currentMapInfo.MinimapPosition.z - playingCharacterTransform.position.z;
                                break;
                        }
                        imageMinimap.transform.localPosition = -new Vector2(x * CurrentSizeRate, y * CurrentSizeRate);
                    }
                }
            }
        }

        protected void UpdateMarkersPositionAndVisibility(float sizeRate)
        {
            for (int i = _markers.Count - 1; i >= 0; --i)
            {
                MarkerData markerData = _markers[i];
                if (markerData.IsRequiredEntity && !markerData.Entity)
                {
                    PutMarkerBack(markerData.Prefab, markerData.Marker);
                    continue;
                }
                if (!IsMapMarkerVisible(markerData.MapMarkerType))
                {
                    if (markerData.Marker.gameObject.activeSelf)
                        markerData.Marker.gameObject.SetActive(false);
                    continue;
                }
                if (markerData.Entity)
                {
                    SetMarkerPositionAndRotation(markerData.Marker, markerData.Entity.EntityTransform, sizeRate, markerData.MarkerRotateOffsets);
                }
                if (!markerData.Marker.gameObject.activeSelf)
                    markerData.Marker.gameObject.SetActive(true);
            }
        }

        protected void InstantiateAllMarkers()
        {
            for (int i = 0; i < _markers.Count; ++i)
            {
                PutMarkerBack(_markers[i].Prefab, _markers[i].Marker);
            }
            _markers.Clear();
            InstantiateEntitiesMarkers();
            InstantiateMapMarkers();
        }

        protected void InstantiateMapMarkers()
        {
            // Add all added markers
            foreach (IMapMarker marker in MapMarkerManager.MarkerValues)
            {
                if (string.IsNullOrWhiteSpace(marker.MapMarkerType))
                    continue;
                if (!CachedMapMarkerPrefabs.TryGetValue(marker.MapMarkerType, out MapMarkerPrefabData prefabData))
                    continue;
                RectTransform newMarker = InstantiateOrGetMarkerFromPool(prefabData.prefab, _mapMarkerContainer);
                if (newMarker == null)
                    continue;
                newMarker.transform.localScale = Vector3.one;
                SetMarkerPositionAndRotation(newMarker, marker.transform.position, Vector3.zero, CurrentSizeRate, prefabData.markerRotateOffsets);
                _markers.Add(OnMarkerInstantiated(new MarkerData()
                {
                    IsRequiredEntity = false,
                    Entity = null,
                    MapMarker = marker,
                    Marker = newMarker,
                    Prefab = prefabData.prefab,
                    MarkerRotateOffsets = prefabData.markerRotateOffsets,
                    MapMarkerType = prefabData.mapMarkerType,
                }));
            }
        }

        protected void InstantiateEntitiesMarkers()
        {
            if (onInstantiateEntitiesMarkersStart != null)
                onInstantiateEntitiesMarkersStart.Invoke();

            if (GameInstance.PlayingCharacterEntity != null)
            {
                int overlapMask = GameInstance.Singleton.playerLayer.Mask | GameInstance.Singleton.playingLayer.Mask | GameInstance.Singleton.monsterLayer.Mask;
                List<BaseCharacterEntity> allies = GameInstance.PlayingCharacterEntity.FindEntities<BaseCharacterEntity>(allyMarkerDistance, true, true, false, false, overlapMask);
                List<BaseCharacterEntity> enemies = GameInstance.PlayingCharacterEntity.FindEntities<BaseCharacterEntity>(enemyOrNeutralMarkerDistance, true, false, true, true, overlapMask);
                EntityInfo entityInfo;
                RectTransform markerPrefab;
                Vector3 markerRotateOffsets;
                foreach (BaseCharacterEntity entry in allies)
                {
                    markerPrefab = null;
                    markerRotateOffsets = Vector3.zero;
                    entityInfo = entry.GetInfo();
                    if (partyMemberMarkerPrefab != null && entityInfo.PartyId > 0 && entityInfo.PartyId == GameInstance.PlayingCharacterEntity.PartyId)
                    {
                        markerPrefab = partyMemberMarkerPrefab;
                        markerRotateOffsets = partyMemberRotateOffsets;
                        onInstantiatePartyMemberMarker.Invoke(entry.ObjectId);
                    }
                    else if (guildMemberMarkerPrefab != null && entityInfo.GuildId > 0 && entityInfo.GuildId == GameInstance.PlayingCharacterEntity.GuildId)
                    {
                        markerPrefab = guildMemberMarkerPrefab;
                        markerRotateOffsets = guildMemberRotateOffsets;
                        onInstantiateGuildMemberMarker.Invoke(entry.ObjectId);
                    }
                    else if (allyMemberMarkerPrefab != null)
                    {
                        markerPrefab = allyMemberMarkerPrefab;
                        markerRotateOffsets = allyMemberRotateOffsets;
                        onInstantiateAllyMemberMarker.Invoke(entry.ObjectId);
                    }
                    if (markerPrefab != null)
                    {
                        InstantiateEntityMarker(entry, markerRotateOffsets, CurrentSizeRate, markerPrefab);
                    }
                }
                foreach (BaseCharacterEntity entry in enemies)
                {
                    markerPrefab = null;
                    markerRotateOffsets = Vector3.zero;
                    entityInfo = entry.GetInfo();
                    if (enemyMarkerPrefab != null && GameInstance.PlayingCharacterEntity.IsEnemy(entityInfo))
                    {
                        markerPrefab = enemyMarkerPrefab;
                        markerRotateOffsets = enemyRotateOffsets;
                        onInstantiateEnemyMarker.Invoke(entry.ObjectId);
                    }
                    else if (neutralMarkerPrefab != null)
                    {
                        markerPrefab = neutralMarkerPrefab;
                        markerRotateOffsets = neutralRotateOffsets;
                        onInstantiateNeutralMarker.Invoke(entry.ObjectId);
                    }
                    if (markerPrefab != null)
                    {
                        InstantiateEntityMarker(entry, markerRotateOffsets, CurrentSizeRate, markerPrefab);
                    }
                }
            }

            if (onInstantiateEntitiesMarkersFinish != null)
                onInstantiateEntitiesMarkersFinish.Invoke();
        }

        public void InstantiateEntityMarker(BaseCharacterEntity character, Vector3 markerRotateOffsets, float sizeRate, RectTransform prefab)
        {
            InstantiateEntityMarker(markerRotateOffsets, sizeRate, prefab,
                character.EntityTransform.position,
                character.EntityTransform.eulerAngles,
                character);
        }

        public void InstantiateEntityMarker(Vector3 markerRotateOffsets, float sizeRate, RectTransform prefab, Vector3 entityPosition, Vector3 entityEulerAngles, BaseCharacterEntity character = null, bool isRequiredEntity = true)
        {
            RectTransform newMarker = InstantiateOrGetMarkerFromPool(prefab, _nonPlayingCharacterMarkerContainer);
            if (newMarker == null)
                return;
            newMarker.transform.localScale = Vector3.one;
            SetMarkerPositionAndRotation(newMarker, entityPosition, entityEulerAngles, sizeRate, markerRotateOffsets);
            _markers.Add(OnMarkerInstantiated(new MarkerData()
            {
                IsRequiredEntity = isRequiredEntity,
                Entity = character,
                MapMarker = null,
                Marker = newMarker,
                Prefab = prefab,
                MarkerRotateOffsets = markerRotateOffsets,
                MapMarkerType = null,
            }));
        }

        public RectTransform InstantiateOrGetMarkerFromPool(RectTransform prefab, RectTransform container)
        {
            if (prefab == null)
                return null;
            int prefabInstanceID = prefab.GetInstanceID();
            if (!_markersPool.TryGetValue(prefabInstanceID, out Queue<RectTransform> instances))
            {
                instances = new Queue<RectTransform>();
                _markersPool[prefabInstanceID] = instances;
            }
            RectTransform instance;
            if (instances.Count > 0)
            {
                instance = instances.Dequeue();
                instance.gameObject.SetActive(true);
                return instance;
            }
            // Instantiate a new one
            instance = Instantiate(prefab, container);
            instance.gameObject.SetActive(true);
            return instance;
        }

        public void PutMarkerBack(RectTransform prefab, RectTransform instance)
        {
            if (prefab == null)
                return;
            int prefabInstanceID = prefab.GetInstanceID();
            if (!_markersPool.TryGetValue(prefabInstanceID, out Queue<RectTransform> instances))
                return;
            instances.Enqueue(instance);
            instance.gameObject.SetActive(false);
        }

        protected void SetMarkerPositionAndRotation(RectTransform markerTransform, Transform entityTransform, float sizeRate, Vector3 markerRotateOffsets)
        {
            SetMarkerPositionAndRotation(markerTransform, entityTransform.position, entityTransform.eulerAngles, sizeRate, markerRotateOffsets);
        }

        protected void SetMarkerPositionAndRotation(RectTransform markerTransform, Vector3 position, Vector3 eulerAngles, float sizeRate, Vector3 markerRotateOffsets)
        {
            GetMarkerPositionAndAngles(position, eulerAngles, sizeRate, markerRotateOffsets, out Vector3 markerPosition, out Vector3 markerEulerAngles);
            markerTransform.localPosition = markerPosition;
            markerTransform.localEulerAngles = markerEulerAngles;
        }

        protected void GetMarkerPositionAndAngles(Vector3 position, Vector3 eulerAngles, float sizeRate, Vector3 markerRotateOffsets, out Vector3 markerPosition, out Vector3 markerEulerAngles)
        {
            switch (GameInstance.Singleton.DimensionType)
            {
                case DimensionType.Dimension2D:
                    if (_currentMapInfo == null)
                    {
                        markerPosition = new Vector3(
                        position.x * sizeRate,
                        position.y * sizeRate);
                        markerEulerAngles = Vector3.zero;
                        break;
                    }

                    markerPosition = new Vector3(
                        (_currentMapInfo.MinimapPosition.x - position.x) * sizeRate,
                        (_currentMapInfo.MinimapPosition.y - position.y) * sizeRate);
                    markerEulerAngles = Vector3.zero;
                    break;
                default:

                    if (_currentMapInfo == null)
                    {
                        markerPosition = new Vector3(
                        position.x * sizeRate,
                        position.z * sizeRate);
                        markerEulerAngles = Vector3.zero;
                        break;
                    }

                    markerPosition = new Vector3(
                        (_currentMapInfo.MinimapPosition.x - position.x) * sizeRate,
                        (_currentMapInfo.MinimapPosition.z - position.z) * sizeRate);
                    markerEulerAngles = markerRotateOffsets + (Vector3.back * eulerAngles.y);
                    break;
            }
        }

        public void SetMapMarkerVisible(string type, bool isVisible)
        {
            if (string.IsNullOrWhiteSpace(type))
                return;
            CachedMapMarkerVisibleState[type] = isVisible;
        }

        public bool IsMapMarkerVisible(string type)
        {
            if (string.IsNullOrWhiteSpace(type))
                return true;
            if (!CachedMapMarkerVisibleState.TryGetValue(type, out bool isVisible))
                return true;
            return isVisible;
        }

        protected virtual MarkerData OnMarkerInstantiated(MarkerData markerData)
        {
            if (markerData.Marker.TryGetComponent(out UIMinimapMarker uiMinimapMarker))
                uiMinimapMarker.Setup(markerData);
            return markerData;
        }
    }
}
