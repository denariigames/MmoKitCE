using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MultiplayerARPG
{
    public partial class EquipmentEntity : BaseEquipmentEntity
    {
        [Header("Refine Effects")]
        public List<EquipmentEntityEffect> effects = new List<EquipmentEntityEffect>();

        private List<GameObject> _allEffectObjects = new List<GameObject>();
        private int _currentLevel = -1;
        private EquipmentEntityEffect _currentEffect = null;
        private MaterialCollection[] _currentVisibleMaterials = null;
        private GameObject[] _currentEffectObjects = null;

        protected override void Awake()
        {
            base.Awake();
            effects.Sort();
            foreach (EquipmentEntityEffect effect in effects)
            {
                if (effect.effectObjects != null && effect.effectObjects.Length > 0)
                {
                    foreach (GameObject effectObject in effect.effectObjects)
                    {
                        effectObject.SetActive(false);
                        _allEffectObjects.Add(effectObject);
                    }
                }
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            effects.Nullify();
            _allEffectObjects.DestroyAndNullify();
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            if (MigrateMaterials())
                EditorUtility.SetDirty(this);
        }
#endif

        [ContextMenu("Migrate Materials")]
        public bool MigrateMaterials()
        {
            bool hasChanges = false;
            Renderer equipmentRenderer = GetComponent<Renderer>();
            if (visibleMaterials == null || visibleMaterials.Length == 0)
            {
                if (equipmentRenderer)
                {
                    visibleMaterials = new MaterialCollection[1]
                    {
                        new MaterialCollection()
                        {
                            renderer = equipmentRenderer,
                            materials = equipmentRenderer.sharedMaterials
                        }
                    };
                    hasChanges = true;
                }
            }

#pragma warning disable CS0618 // Type or member is obsolete
            if (effects != null && effects.Count > 0)
            {
                EquipmentEntityEffect tempEffect;
                for (int i = 0; i < effects.Count; ++i)
                {
                    tempEffect = effects[i];
                    if (tempEffect.materials != null && tempEffect.materials.Length > 0 && (tempEffect.visibleMaterials == null || tempEffect.visibleMaterials.Length == 0))
                    {
                        MaterialCollection[] materials = new MaterialCollection[1]
                        {
                            new MaterialCollection()
                            {
                                renderer = equipmentRenderer,
                                materials = tempEffect.materials,
                            }
                        };
                        tempEffect.visibleMaterials = materials;
                        effects[i] = tempEffect;
                        hasChanges = true;
                    }
                }
            }
#pragma warning restore CS0618 // Type or member is obsolete

            return hasChanges;
        }

        public override void OnItemChanged(CharacterItem item)
        {
            if (_currentLevel == item.level)
                return;
            _currentLevel = item.level;
            foreach (GameObject allEffectObject in _allEffectObjects)
            {
                if (allEffectObject.activeSelf)
                    allEffectObject.SetActive(false);
            }

            _currentEffect = null;
            _currentVisibleMaterials = null;
            _currentEffectObjects = null;
            foreach (EquipmentEntityEffect effect in effects)
            {
                if (_currentLevel >= effect.level)
                {
                    _currentEffect = effect;
                }
                else
                {
                    break;
                }
            }

            if (_currentEffect != null)
            {
                _currentVisibleMaterials = _currentEffect.visibleMaterials;
                _currentEffectObjects = _currentEffect.effectObjects;
            }
            else
            {
                // Not found effect apply default materials
                _currentVisibleMaterials = visibleMaterials;
            }

            bool isVisible = CharacterModel == null || CharacterModel.VisibleState == GameEntityModel.EVisibleState.Visible;
            if (isVisible && _currentVisibleMaterials != null)
            {
                // It is visible, so apply the materials
                _currentVisibleMaterials.ApplyMaterials();
            }

            if (_currentEffectObjects != null && _currentEffectObjects.Length > 0)
            {
                foreach (GameObject effectObject in _currentEffectObjects)
                {
                    if (effectObject.activeSelf != isVisible)
                        effectObject.SetActive(isVisible);
                }
            }
        }

        public override void SetVisibleState(GameEntityModel.EVisibleState visibleState)
        {
            if (_currentVisibleMaterials == null)
                _currentVisibleMaterials = visibleMaterials;
            if (ModelHiddingUpdater)
            {
                switch (visibleState)
                {
                    case GameEntityModel.EVisibleState.Visible:
                        // Visible state is Visible, show all objects and renderers
                        ModelHiddingUpdater.SetHiddingObjectsAndRenderers(hiddingObjects, hiddingRenderers, false);
                        ModelHiddingUpdater.SetHiddingObjectsAndRenderers(fpsHiddingObjects, fpsHiddingRenderers, false);
                        _currentVisibleMaterials.ApplyMaterials();
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
            bool isVisible = CharacterModel && CharacterModel.VisibleState == GameEntityModel.EVisibleState.Visible;
            if (_currentEffectObjects != null && _currentEffectObjects.Length > 0)
            {
                foreach (GameObject effectObject in _currentEffectObjects)
                {
                    if (effectObject.activeSelf != isVisible)
                        effectObject.SetActive(isVisible);
                }
            }
        }
    }
}
