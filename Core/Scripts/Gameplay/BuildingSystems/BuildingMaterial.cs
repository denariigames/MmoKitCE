using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

namespace MultiplayerARPG
{
    public class BuildingMaterial : DamageableHitBox
    {
        public enum State
        {
            Unknow,
            Default,
            CanBuild,
            CannotBuild,
        }

        [Header("3D Settings")]
        public Material[] canBuildMaterials;
        public Material[] cannotBuildMaterials;

        [Header("2D Settings")]
        public Color canBuildColor = Color.green;
        public Color cannotBuildColor = Color.red;

        [Header("Render Components, Only 1 kind will being used.")]
        public MeshRenderer meshRenderer;
        public SpriteRenderer spriteRenderer;
        public Tilemap tilemap;

        [Header("Extra Renderers")]
        [Tooltip("This will being used if `meshRenderer` is set, these mesh renderers must uses the same set of materials with `meshRenderer` to make it works properly")]
        [FormerlySerializedAs("meshRendererList")]
        public MeshRenderer[] extraMeshRenderers = new MeshRenderer[0];
        [Tooltip("This will being used if `spriteRenderer` is set")]
        public SpriteRenderer[] extraSpriteRenderers = new SpriteRenderer[0];
        [Tooltip("This will being used if `tilemap` is set")]
        public Tilemap[] extraTilemaps = new Tilemap[0];

        [Header("Build Mode Settings")]
        [Range(0.1f, 1f)]
        [Tooltip("It will be used to reduce collider's bounds when find other intersecting building materials")]
        public float boundsSizeRateWhilePlacing = 0.9f;

        private State _currentState;
        public State CurrentState
        {
            get { return _currentState; }
            set
            {
                if (_currentState == value)
                    return;
                _currentState = value;
                SetupRenderersByState(_currentState);
            }
        }

        public BuildingEntity BuildingEntity { get; private set; }
        public NavMeshObstacle CacheNavMeshObstacle { get; private set; }

        protected BuildingMaterialBuildModeHandler _buildModeHandler;
        protected Material[] _defaultMaterials;
        protected ShadowCastingMode _defaultShadowCastingMode;
        protected bool _defaultReceiveShadows;
        protected Color _defaultColor;

        protected Color[] _defaultColorForExtraRenderers;
        protected ShadowCastingMode[] _defaultShadowCastingModeForExtraRenderers;
        protected bool[] _defaultReceiveShadowsForExtraRenderers;

        public override void Setup(byte index)
        {
            base.Setup(index);
            BuildingEntity = DamageableEntity as BuildingEntity;
            BuildingEntity.RegisterMaterial(this);
            CacheNavMeshObstacle = GetComponent<NavMeshObstacle>();

            // Find target renderer
            if (meshRenderer == null)
                meshRenderer = GetComponent<MeshRenderer>();
            if (spriteRenderer == null)
                spriteRenderer = GetComponent<SpriteRenderer>();
            if (tilemap == null)
                tilemap = GetComponent<Tilemap>();

            if (meshRenderer != null)
            {
                _defaultMaterials = meshRenderer.sharedMaterials;
                _defaultShadowCastingMode = meshRenderer.shadowCastingMode;
                _defaultReceiveShadows = meshRenderer.receiveShadows;
                PrepareDefaultExtraMeshRenderersValues();
            }
            else if (spriteRenderer != null)
            {
                _defaultColor = spriteRenderer.color;
                _defaultShadowCastingMode = spriteRenderer.shadowCastingMode;
                _defaultReceiveShadows = spriteRenderer.receiveShadows;
                PrepareDefaultExtraSpriteRenderersValues();
            }
            else if (tilemap != null)
            {
                _defaultColor = tilemap.color;
                PrepareDefaultExtraTilemapsValues();
            }

            CurrentState = State.Unknow;
            CurrentState = State.Default;

            if (BuildingEntity.IsBuildMode)
            {
                if (CacheNavMeshObstacle != null)
                    CacheNavMeshObstacle.enabled = false;

                if (_buildModeHandler == null)
                {
                    _buildModeHandler = gameObject.AddComponent<BuildingMaterialBuildModeHandler>();
                    _buildModeHandler.Setup(this);
                }
            }
        }

        private void OnDestroy()
        {
            canBuildMaterials.Nullify();
            cannotBuildMaterials.Nullify();
            meshRenderer = null;
            spriteRenderer = null;
            tilemap = null;
            extraMeshRenderers.Nullify();
            extraSpriteRenderers.Nullify();
            extraTilemaps.Nullify();
            BuildingEntity = null;
            CacheNavMeshObstacle = null;
            _buildModeHandler = null;
            _defaultMaterials.Nullify();
        }

        private void PrepareDefaultExtraMeshRenderersValues()
        {
            if (extraMeshRenderers == null || extraMeshRenderers.Length == 0)
                return;
            _defaultShadowCastingModeForExtraRenderers = new ShadowCastingMode[extraMeshRenderers.Length];
            _defaultReceiveShadowsForExtraRenderers = new bool[extraMeshRenderers.Length];
            for (int i = 0; i < extraMeshRenderers.Length; ++i)
            {
                if (extraMeshRenderers[i] == null)
                    continue;
                _defaultShadowCastingModeForExtraRenderers[i] = extraMeshRenderers[i].shadowCastingMode;
                _defaultReceiveShadowsForExtraRenderers[i] = extraMeshRenderers[i].receiveShadows;
            }
        }

        private void PrepareDefaultExtraSpriteRenderersValues()
        {
            if (extraSpriteRenderers == null || extraSpriteRenderers.Length == 0)
                return;
            _defaultColorForExtraRenderers = new Color[0];
            _defaultShadowCastingModeForExtraRenderers = new ShadowCastingMode[extraSpriteRenderers.Length];
            _defaultReceiveShadowsForExtraRenderers = new bool[extraSpriteRenderers.Length];
            for (int i = 0; i < extraSpriteRenderers.Length; ++i)
            {
                if (extraSpriteRenderers[i] == null)
                    continue;
                _defaultColorForExtraRenderers[i] = extraSpriteRenderers[i].color;
                _defaultShadowCastingModeForExtraRenderers[i] = extraSpriteRenderers[i].shadowCastingMode;
                _defaultReceiveShadowsForExtraRenderers[i] = extraSpriteRenderers[i].receiveShadows;
            }
        }

        private void PrepareDefaultExtraTilemapsValues()
        {
            if (extraTilemaps == null || extraTilemaps.Length == 0)
                return;
            _defaultColorForExtraRenderers = new Color[0];
            for (int i = 0; i < extraTilemaps.Length; ++i)
            {
                if (extraTilemaps[i] == null)
                    continue;
                _defaultColorForExtraRenderers[i] = extraTilemaps[i].color;
            }
        }

        public virtual void SetupRenderersByState(State state)
        {
            if (meshRenderer != null)
            {
                switch (state)
                {
                    case State.Default:
                        meshRenderer.sharedMaterials = _defaultMaterials;
                        meshRenderer.shadowCastingMode = _defaultShadowCastingMode;
                        meshRenderer.receiveShadows = _defaultReceiveShadows;
                        break;
                    case State.CanBuild:
                        meshRenderer.sharedMaterials = canBuildMaterials;
                        meshRenderer.shadowCastingMode = ShadowCastingMode.Off;
                        meshRenderer.receiveShadows = false;
                        break;
                    case State.CannotBuild:
                        meshRenderer.sharedMaterials = cannotBuildMaterials;
                        meshRenderer.shadowCastingMode = ShadowCastingMode.Off;
                        meshRenderer.receiveShadows = false;
                        break;
                }
                SetupExtraMeshRenderersByState(state);
            }
            else if (spriteRenderer != null)
            {
                switch (state)
                {
                    case State.Default:
                        spriteRenderer.color = _defaultColor;
                        spriteRenderer.shadowCastingMode = _defaultShadowCastingMode;
                        spriteRenderer.receiveShadows = _defaultReceiveShadows;
                        break;
                    case State.CanBuild:
                        spriteRenderer.color = canBuildColor;
                        spriteRenderer.shadowCastingMode = ShadowCastingMode.Off;
                        spriteRenderer.receiveShadows = false;
                        break;
                    case State.CannotBuild:
                        spriteRenderer.color = cannotBuildColor;
                        spriteRenderer.shadowCastingMode = ShadowCastingMode.Off;
                        spriteRenderer.receiveShadows = false;
                        break;
                }
                SetupExtraSpriteRenderersByState(state);
            }
            else if (tilemap != null)
            {
                switch (state)
                {
                    case State.Default:
                        tilemap.color = _defaultColor;
                        break;
                    case State.CanBuild:
                        tilemap.color = canBuildColor;
                        break;
                    case State.CannotBuild:
                        tilemap.color = cannotBuildColor;
                        break;
                }
                SetupExtraTilemapsByState(state);
            }
        }

        protected void SetupExtraMeshRenderersByState(State state)
        {
            if (extraMeshRenderers == null || extraMeshRenderers.Length == 0)
                return;
            for (int i = 0; i < extraMeshRenderers.Length; ++i)
            {
                MeshRenderer comp = extraMeshRenderers[i];
                if (comp == null)
                    continue;

                switch (state)
                {
                    case State.Default:
                        comp.sharedMaterials = _defaultMaterials;
                        comp.shadowCastingMode = _defaultShadowCastingModeForExtraRenderers[i];
                        comp.receiveShadows = _defaultReceiveShadowsForExtraRenderers[i];
                        break;
                    case State.CanBuild:
                        comp.sharedMaterials = canBuildMaterials;
                        comp.shadowCastingMode = ShadowCastingMode.Off;
                        comp.receiveShadows = false;
                        break;
                    case State.CannotBuild:
                        comp.sharedMaterials = cannotBuildMaterials;
                        comp.shadowCastingMode = ShadowCastingMode.Off;
                        comp.receiveShadows = false;
                        break;
                }
            }
        }

        protected void SetupExtraSpriteRenderersByState(State state)
        {
            if (extraSpriteRenderers == null || extraSpriteRenderers.Length == 0)
                return;
            for (int i = 0; i < extraSpriteRenderers.Length; ++i)
            {
                SpriteRenderer comp = extraSpriteRenderers[i];
                if (comp == null)
                    continue;

                switch (state)
                {
                    case State.Default:
                        comp.color = _defaultColorForExtraRenderers[i];
                        comp.shadowCastingMode = _defaultShadowCastingModeForExtraRenderers[i];
                        comp.receiveShadows = _defaultReceiveShadowsForExtraRenderers[i];
                        break;
                    case State.CanBuild:
                        comp.color = canBuildColor;
                        comp.shadowCastingMode = ShadowCastingMode.Off;
                        comp.receiveShadows = false;
                        break;
                    case State.CannotBuild:
                        comp.color = cannotBuildColor;
                        comp.shadowCastingMode = ShadowCastingMode.Off;
                        comp.receiveShadows = false;
                        break;
                }
            }
        }

        protected void SetupExtraTilemapsByState(State state)
        {
            if (extraTilemaps == null || extraTilemaps.Length == 0)
                return;
            for (int i = 0; i < extraTilemaps.Length; ++i)
            {
                Tilemap comp = extraTilemaps[i];
                if (comp == null)
                    continue;

                switch (state)
                {
                    case State.Default:
                        comp.color = _defaultColorForExtraRenderers[i];
                        break;
                    case State.CanBuild:
                        comp.color = canBuildColor;
                        break;
                    case State.CannotBuild:
                        comp.color = cannotBuildColor;
                        break;
                }
            }
        }
    }
}
