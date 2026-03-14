using Insthync.DevExtension;
using Insthync.UnityEditorUtils;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MultiplayerARPG
{
    [DisallowMultipleComponent]
    public abstract partial class BaseEquipmentEntity : ComponentWithPrefabRef<BaseEquipmentEntity>, IPoolDescriptorCollection
    {
        public BaseCharacterModel CharacterModel { get; set; }
        public ModelHiddingUpdater ModelHiddingUpdater { get; protected set; }
        public string EquipSocket { get; set; }
        public string EquipPosition { get; set; }

        private CharacterItem _item;
        public CharacterItem Item
        {
            get { return _item; }
            set
            {
                if (_item.IsDiffer(value, true, true, true, true))
                {
                    _item = value;
                    OnItemChanged(_item);
                    onItemChanged.Invoke(_item);
                }
                else
                {
                    _item = value;
                }
            }
        }

        [Header("Appearances")]
        [FormerlySerializedAs("defaultMaterials")]
        [Tooltip("Materials which will be applied while entity is visible")]
        public MaterialCollection[] visibleMaterials = new MaterialCollection[0];
        [Tooltip("Materials which will be applied while entity is invisible")]
        public MaterialCollection[] invisibleMaterials = new MaterialCollection[0];
        [Tooltip("Materials which will be applied while view mode is FPS")]
        public MaterialCollection[] fpsMaterials = new MaterialCollection[0];
        [Tooltip("These objects will be deactivated while entity is invisible")]
        public List<GameObject> hiddingObjects = new List<GameObject>();
        [Tooltip("These renderers will be disabled while entity is invisible")]
        public List<Renderer> hiddingRenderers = new List<Renderer>();
        [Tooltip("These object will be deactivated while view mode is FPS")]
        public List<GameObject> fpsHiddingObjects = new List<GameObject>();
        [Tooltip("These renderers will be disabled while view mode is FPS")]
        public List<Renderer> fpsHiddingRenderers = new List<Renderer>();

        [Header("Effects")]
#if UNITY_EDITOR || !UNITY_SERVER
        [Tooltip("These game effects must placed as this children, it will be activated when launch (can place muzzle effects here)")]
        public GameEffect[] weaponLaunchEffects = new GameEffect[0];
        [Tooltip("These game effects prefabs will instantiates to container when launch (can place muzzle effects here)")]
        public GameEffectPoolContainer[] poolingWeaponLaunchEffects = new GameEffectPoolContainer[0];
        [Tooltip("This is overriding missile damage transform, if this is not empty, it will spawn missile damage entity from this transform")]
#endif
        public Transform missileDamageTransform;

        [Header("Events")]
        public UnityEvent onSetup = new UnityEvent();
        public UnityEvent onEnable = new UnityEvent();
        public UnityEvent onDisable = new UnityEvent();
        public UnityEvent onPlayLaunch = new UnityEvent();
        public UnityEvent onPlayReload = new UnityEvent();
        public UnityEvent onPlayReloaded = new UnityEvent();
        public UnityEvent onPlayCharge = new UnityEvent();
        public EquipmentEntityItemEvent onItemChanged = new EquipmentEntityItemEvent();

#if UNITY_EDITOR
        [Header("Tools")]
        [InspectorButton(nameof(SetProperMissileDamageTransformForIK))]
        public bool setProperMissileDamageTransformForIK;
#endif

        public IEnumerable<IPoolDescriptor> PoolDescriptors
        {
            get
            {
                List<IPoolDescriptor> effects = new List<IPoolDescriptor>();
#if !UNITY_SERVER
                if (poolingWeaponLaunchEffects != null && poolingWeaponLaunchEffects.Length > 0)
                {
                    foreach (GameEffectPoolContainer container in poolingWeaponLaunchEffects)
                    {
#if !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
                        effects.Add(container.prefab);
#endif
                    }
                }
#endif
                return effects;
            }
        }

        public virtual void Setup(BaseCharacterModel characterModel, string equipSocket, string equipPosition, CharacterItem item)
        {
            CharacterModel = characterModel;
            if (CharacterModel != null)
                CharacterModel.onVisibleStateChange += SetVisibleState;
            EquipSocket = equipSocket;
            EquipPosition = equipPosition;
            Item = item;
            onSetup.Invoke();
        }

        public void InvokeOnItemChanged()
        {
            OnItemChanged(_item);
            onItemChanged.Invoke(_item);
        }

        protected virtual void Awake()
        {
            ModelHiddingUpdater = gameObject.GetOrAddComponent<ModelHiddingUpdater>();
        }

        protected virtual void OnDestroy()
        {
            if (CharacterModel)
                CharacterModel.onVisibleStateChange -= SetVisibleState;
            CharacterModel = null;
            onSetup.RemoveAllListeners();
            onEnable.RemoveAllListeners();
            onDisable.RemoveAllListeners();
            onPlayLaunch.RemoveAllListeners();
            onPlayReload.RemoveAllListeners();
            onPlayReloaded.RemoveAllListeners();
            onPlayCharge.RemoveAllListeners();
            onItemChanged.RemoveAllListeners();
            hiddingRenderers.DestroyAndNullify();
            fpsHiddingRenderers.DestroyAndNullify();
            hiddingObjects.DestroyAndNullify();
            fpsHiddingObjects.DestroyAndNullify();
#if UNITY_EDITOR || !UNITY_SERVER
            weaponLaunchEffects.DestroyAndNullify();
#endif
        }

        protected virtual void OnEnable()
        {
#if !UNITY_SERVER
            if (weaponLaunchEffects != null && weaponLaunchEffects.Length > 0)
            {
                foreach (GameEffect weaponLaunchEffect in weaponLaunchEffects)
                {
                    weaponLaunchEffect.gameObject.SetActive(false);
                }
            }
#endif
            onEnable.Invoke();
        }

        protected virtual void OnDisable()
        {
            onDisable.Invoke();
        }

        public virtual void PlayLaunch()
        {
            if (!gameObject.activeInHierarchy)
                return;

#if !UNITY_SERVER
            // Play effects at clients only
            if (BaseGameNetworkManager.Singleton.IsClientConnected)
            {
                if (weaponLaunchEffects != null && weaponLaunchEffects.Length > 0)
                    weaponLaunchEffects[Random.Range(0, weaponLaunchEffects.Length)].Play();

                if (poolingWeaponLaunchEffects != null && poolingWeaponLaunchEffects.Length > 0)
                    poolingWeaponLaunchEffects[Random.Range(0, poolingWeaponLaunchEffects.Length)].GetInstance();
            }
#endif

            onPlayLaunch.Invoke();
        }

        public virtual void PlayReload()
        {
            onPlayReload.Invoke();
        }

        public virtual void PlayReloaded()
        {
            onPlayReloaded.Invoke();
        }

        public virtual void PlayCharge()
        {
            onPlayCharge.Invoke();
        }

        [ContextMenu("Set `missileDamageTransform` as `poolingWeaponLaunchEffects` container")]
        public void SetMissileDamageTransformAsPoolingEffectsContainer()
        {
#if !UNITY_SERVER
            if (poolingWeaponLaunchEffects != null && poolingWeaponLaunchEffects.Length > 0)
            {
                for (int i = 0; i < poolingWeaponLaunchEffects.Length; ++i)
                {
                    GameEffectPoolContainer container = poolingWeaponLaunchEffects[i];
                    container.container = missileDamageTransform;
                    poolingWeaponLaunchEffects[i] = container;
                }
            }
#endif
        }

#if UNITY_EDITOR
        protected virtual void OnDrawGizmos()
        {
            Gizmos.color = new Color(0, 1, 0, 0.5f);
            Gizmos.DrawWireSphere(transform.position, 0.1f);
            Gizmos.DrawSphere(transform.position, 0.03f);
            Handles.Label(transform.position, name + "(Pivot)");
            if (missileDamageTransform != null)
            {
                Gizmos.color = new Color(1, 0, 0, 0.5f);
                Gizmos.DrawSphere(missileDamageTransform.position, 0.03f);
                Handles.Label(missileDamageTransform.position, name + "(MissleDamage)");
                Gizmos.color = new Color(0, 0, 1, 0.5f);
                DrawArrow.ForGizmo(missileDamageTransform.position, missileDamageTransform.forward, 0.5f, 0.1f);
                Gizmos.color = new Color(0, 1, 0, 0.5f);
                DrawArrow.ForGizmo(missileDamageTransform.position, -missileDamageTransform.up, 0.5f, 0.1f);
            }

            this.InvokeInstanceDevExtMethods("OnDrawGizmos");
        }
#endif

#if UNITY_EDITOR
        protected virtual void OnValidate()
        {

        }
#endif

#if UNITY_EDITOR
        public void SetProperMissileDamageTransformForIK()
        {
            if (missileDamageTransform == null)
                return;

            if (!missileDamageTransform.eulerAngles.Equals(Vector3.zero))
            {
                Debug.LogWarning($"[EquipmentEntity] {name} `missileDamageTransform` global euler angles must be 0,0,0)");
                missileDamageTransform.eulerAngles = Vector3.zero;
                EditorUtility.SetDirty(this);
            }

            if (!Mathf.Approximately(missileDamageTransform.position.x, 0f))
            {
                Debug.LogWarning($"[EquipmentEntity] {name} `missileDamageTransform` global X position must be 0)");
                Vector3 position = missileDamageTransform.position;
                position.x = 0f;
                missileDamageTransform.position = position;
                EditorUtility.SetDirty(this);
            }
        }
#endif

        public virtual void SetVisibleState(GameEntityModel.EVisibleState visibleState)
        {
            switch (visibleState)
            {
                case GameEntityModel.EVisibleState.Visible:
                    // Visible state is Visible, show all objects and renderers
                    ModelHiddingUpdater.SetHiddingObjectsAndRenderers(hiddingObjects, hiddingRenderers, false);
                    ModelHiddingUpdater.SetHiddingObjectsAndRenderers(fpsHiddingObjects, fpsHiddingRenderers, false);
                    visibleMaterials.ApplyMaterials();
                    break;
                case GameEntityModel.EVisibleState.Invisible:
                    // Visible state is Invisible, hide all objects and renderers
                    ModelHiddingUpdater.SetHiddingObjectsAndRenderers(hiddingObjects, hiddingRenderers, true);
                    ModelHiddingUpdater.SetHiddingObjectsAndRenderers(fpsHiddingObjects, fpsHiddingRenderers, true);
                    invisibleMaterials.ApplyMaterials();
                    break;
                case GameEntityModel.EVisibleState.Fps:
                    // Visible state is Fps, hide Fps objects and renderers
                    ModelHiddingUpdater.SetHiddingObjectsAndRenderers(hiddingObjects, hiddingRenderers, false);
                    ModelHiddingUpdater.SetHiddingObjectsAndRenderers(fpsHiddingObjects, fpsHiddingRenderers, true);
                    fpsMaterials.ApplyMaterials();
                    break;
            }
        }

        public abstract void OnItemChanged(CharacterItem item);
    }
}
