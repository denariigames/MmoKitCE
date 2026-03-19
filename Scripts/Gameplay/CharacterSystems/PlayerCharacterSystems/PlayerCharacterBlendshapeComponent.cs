using Cysharp.Text;
using Insthync.ManagedUpdating;
using Insthync.UnityEditorUtils;
using LiteNetLibManager;
using System.Collections.Generic;
using UnityEngine;

namespace MultiplayerARPG
{
    public class PlayerCharacterBlendshapeComponent : BaseGameEntityComponent<BasePlayerCharacterEntity>, IManagedUpdate
    {
        [System.Serializable]
        public class BlendshapeOption
        {
            [Header("Settings for UIs")]
            public string defaultTitle = string.Empty;
            public LanguageData[] languageSpecificTitles = new LanguageData[0];
            [PreviewSprite(50)]
            public Sprite icon;

            [Header("Settings for In-Game Appearances")]
            public string blendshapeName;
            public float defaultValue = 0f;
            public float minValue = -1f;
            public float maxValue = 1f;

            public string Title
            {
                get { return Language.GetText(languageSpecificTitles, defaultTitle); }
            }
        }

        public string settingIdPrefix;
        public SkinnedMeshRenderer skinnedMeshRenderer;
        public BlendshapeOption[] options = new BlendshapeOption[0];

        private float[] _currentValues;
        private Dictionary<int, BlendshapeOption> _optionsByHashedId = new Dictionary<int, BlendshapeOption>();
        private Dictionary<string, int> _blendshapeIndexesByName = new Dictionary<string, int>();
        private bool _isInitialized = false;

        private bool Applying
        {
            set
            {
                if (value)
                {
                    UpdateManager.Register(this);
                }
                else
                {
                    UpdateManager.Unregister(this);
                }
            }
        }

        private void Awake()
        {
            if (skinnedMeshRenderer == null)
                return;
            _currentValues = new float[options.Length];
            for (int i = 0; i < options.Length; ++i)
            {
                _currentValues[i] = options[i].defaultValue;
                _optionsByHashedId[GetHashedSettingId(options[i])] = options[i];
            }
            for (int i = 0; i < skinnedMeshRenderer.sharedMesh.blendShapeCount; ++i)
            {
                _blendshapeIndexesByName[skinnedMeshRenderer.sharedMesh.GetBlendShapeName(i)] = i;
            }
        }

        private void OnEnable()
        {
            if (!_isInitialized)
            {
                _isInitialized = true;
                SetupEvents();
            }
        }

        protected override void OnDestroy()
        {
            ClearEvents();
            Applying = false;
            base.OnDestroy();
        }

        public void ManagedUpdate()
        {
            ApplyBlendShapeWeights();
            Applying = false;
        }

        public void SetupEvents()
        {
            ClearEvents();
#if !DISABLE_CUSTOM_CHARACTER_DATA
            Entity.onSetup += OnSetup;
            Entity.onPublicIntsOperation += OnPublicIntsOperation;
#endif
        }

        public void ClearEvents()
        {
#if !DISABLE_CUSTOM_CHARACTER_DATA
            Entity.onSetup -= OnSetup;
            Entity.onPublicIntsOperation -= OnPublicIntsOperation;
#endif
        }

        private void OnSetup()
        {
            Applying = true;
        }

        private void OnPublicIntsOperation(LiteNetLibSyncListOp operation, int index, CharacterDataInt32 oldItem, CharacterDataInt32 newItem)
        {
            switch (operation)
            {
                case LiteNetLibSyncListOp.Set:
                case LiteNetLibSyncListOp.Dirty:
                    if (oldItem.hashedKey != newItem.hashedKey || oldItem.value != newItem.value)
                        Applying = true;
                    break;
                default:
                    Applying = true;
                    break;
            }
        }

        /// <summary>
        /// This function should be called by server or being called in character creation only, it is not allow client to set custom data.
        /// </summary>
        /// <param name="optionIndex"></param>
        /// <param name="value"></param>
        public void SetData(int optionIndex, float value)
        {
            if (!_isInitialized || optionIndex < 0 || optionIndex >= _currentValues.Length)
                return;
            _currentValues[optionIndex] = value;
            Applying = true;
            // Save to entity's `PublicFloats`
            int hashedSettingId = GetHashedSettingId(options[optionIndex]);
            Entity.SetPublicFloat32(hashedSettingId, value);
        }

        public float GetData(int optionIndex)
        {
            if (!_isInitialized || optionIndex < 0 || optionIndex >= _currentValues.Length)
                return 0f;
            return _currentValues[optionIndex];
        }

        public void ApplyBlendShapeWeights()
        {
            for (int i = 0; i < options.Length; ++i)
            {
                if (!_blendshapeIndexesByName.TryGetValue(options[i].blendshapeName, out int blendshapeIndex))
                    continue;
                int hashedSettingId = GetHashedSettingId(options[i]);
                if (Entity.PublicInts.IndexOf(hashedSettingId) >= 0)
                    _currentValues[i] = Entity.PublicInts[hashedSettingId].value;
                else
                    _currentValues[i] = options[i].defaultValue;
                skinnedMeshRenderer.SetBlendShapeWeight(blendshapeIndex, _currentValues[i]);
            }
        }

        public string GetSettingId(BlendshapeOption option)
        {
            return ZString.Concat(settingIdPrefix, option.blendshapeName);
        }

        public int GetHashedSettingId(BlendshapeOption option)
        {
            return GetSettingId(option).GenerateHashId();
        }
    }
}
