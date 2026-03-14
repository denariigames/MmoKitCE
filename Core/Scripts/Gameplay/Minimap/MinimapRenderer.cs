using Insthync.UnityEditorUtils;
using UnityEngine;

namespace MultiplayerARPG
{
    public class MinimapRenderer : MonoBehaviour
    {
        [Header("Settings")]
        [Tooltip("You can use Unity's plane as mesh minimap")]
        public float spriteOffsets3D = -100f;
        public float spriteOffsets2D = 1f;
        public Sprite noMinimapSprite = null;
        public UnityLayer layer;
        public SpriteRenderer minimapRendererPrefab;

        [Header("Testing")]
        public bool isTestMode;
        public BaseMapInfo testingMapInfo;
        public DimensionType testingDimensionType;

        private BaseMapInfo _currentMapInfo;
        private SpriteRenderer _spriteRenderer;

        private void Start()
        {
            if (minimapRendererPrefab == null)
                _spriteRenderer = new GameObject("__MinimapRenderer").AddComponent<SpriteRenderer>();
            else
                _spriteRenderer = Instantiate(minimapRendererPrefab);
            _spriteRenderer.gameObject.layer = layer.LayerIndex;
        }

        private void Update()
        {
            BaseMapInfo mapInfo = isTestMode ? testingMapInfo : BaseGameNetworkManager.CurrentMapInfo;
            if (mapInfo == null || mapInfo == _currentMapInfo)
                return;
            _currentMapInfo = mapInfo;
            UpdateMinimap();
        }

        private async void UpdateMinimap()
        {
            // Use bounds size to calculate transforms
            float boundsWidth = _currentMapInfo.MinimapBoundsWidth;
            float boundsLength = _currentMapInfo.MinimapBoundsLength;
            float maxBoundsSize = Mathf.Max(boundsWidth, boundsLength);

            // Set dimention type
            DimensionType dimensionType = GameInstance.Singleton == null || isTestMode ? testingDimensionType : GameInstance.Singleton.DimensionType;

            if (_spriteRenderer != null)
            {
                switch (dimensionType)
                {
                    case DimensionType.Dimension2D:
                        _spriteRenderer.transform.position = _currentMapInfo.MinimapPosition + (Vector3.forward * spriteOffsets2D);
                        _spriteRenderer.transform.eulerAngles = Vector3.zero;
                        break;
                    default:
                        _spriteRenderer.transform.position = _currentMapInfo.MinimapPosition + (Vector3.up * spriteOffsets3D);
                        _spriteRenderer.transform.eulerAngles = new Vector3(90f, 0f, 0f);
                        break;
                }
                Sprite minimapSprite = await _currentMapInfo.GetMinimapSprite();
                if (minimapSprite == null)
                    minimapSprite = noMinimapSprite;
                _spriteRenderer.sprite = minimapSprite;
                if (_spriteRenderer.sprite != null)
                    _spriteRenderer.transform.localScale = new Vector3(1f, 1f) * maxBoundsSize * _spriteRenderer.sprite.pixelsPerUnit / Mathf.Max(_spriteRenderer.sprite.texture.width, _spriteRenderer.sprite.texture.height);
            }
        }
    }
}
