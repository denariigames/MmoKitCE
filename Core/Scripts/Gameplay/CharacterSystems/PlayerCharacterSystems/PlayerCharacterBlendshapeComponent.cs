using Cysharp.Text;
using Insthync.ManagedUpdating;
using Insthync.UnityEditorUtils;
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

        private bool _setupProperly;
        private bool _applying;
        private float[] _currentValues;
        private Dictionary<int, BlendshapeOption> _optionsByHashedId = new Dictionary<int, BlendshapeOption>();
        private Dictionary<string, int> _blendshapeIndexesByName = new Dictionary<string, int>();

        private void Awake()
        {
            if (skinnedMeshRenderer == null)
                return;
            _applying = true;
            _setupProperly = true;
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
            UpdateManager.Register(this);
        }

        private void OnDisable()
        {
            UpdateManager.Unregister(this);
        }

        public void ManagedUpdate()
        {
            if (_applying)
            {
                Apply();
                _applying = false;
            }
        }

        /// <summary>
        /// This function should be called by server or being called in character creation only, it is not allow client to set custom data.
        /// </summary>
        /// <param name="optionIndex"></param>
        /// <param name="value"></param>
        public void SetData(int optionIndex, float value)
        {
            if (!_setupProperly || optionIndex < 0 || optionIndex >= _currentValues.Length)
                return;
            _currentValues[optionIndex] = value;
            _applying = true;
            // Save to entity's `PublicFloats`
            Entity.SetPublicFloat32(GetHashedSettingId(options[optionIndex]), value);
        }

        public float GetData(int optionIndex)
        {
            if (!_setupProperly || optionIndex < 0 || optionIndex >= _currentValues.Length)
                return 0f;
            return _currentValues[optionIndex];
        }

        public void Apply()
        {
            for (int i = 0; i < options.Length; ++i)
            {
                if (!_blendshapeIndexesByName.TryGetValue(options[i].blendshapeName, out int blendshapeIndex))
                    continue;
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
