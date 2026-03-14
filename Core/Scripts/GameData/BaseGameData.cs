using Cysharp.Threading.Tasks;
using Insthync.AddressableAssetTools;
using Insthync.DevExtension;
using Insthync.UnityEditorUtils;
using LiteNetLibManager;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;
#if !DISABLE_ADDRESSABLES
using UnityEngine.AddressableAssets;
#endif
using UnityEngine.Serialization;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MultiplayerARPG
{
    public abstract partial class BaseGameData : ScriptableObject, IGameData, IPatchableData, IComparable
    {
#if UNITY_EDITOR
        [InspectorButton(nameof(Validate), "Force Validate")]
        public bool btnValidate;
#endif

        [Tooltip("Game data ID, if this is empty it will uses file's name as ID")]
        [SerializeField]
        protected string id = string.Empty;
        public virtual string Id
        {
            get { return string.IsNullOrEmpty(id) ? name : id; }
            set { id = value; }
        }

        [Category("Generic Settings")]
        [SerializeField]
        [FormerlySerializedAs("title")]
        protected string defaultTitle = string.Empty;
        [SerializeField]
        [FormerlySerializedAs("titles")]
        protected LanguageData[] languageSpecificTitles;
        public string DefaultTitle
        {
            get { return defaultTitle; }
            set { defaultTitle = value; }
        }
        public LanguageData[] LanguageSpecificTitles
        {
            get { return languageSpecificTitles; }
            set { languageSpecificTitles = value; }
        }
        [JsonIgnore]
        public virtual string Title
        {
            get { return Language.GetText(languageSpecificTitles, defaultTitle); }
        }

        [SerializeField]
        [FormerlySerializedAs("description")]
        [TextArea]
        protected string defaultDescription = string.Empty;
        [SerializeField]
        [FormerlySerializedAs("descriptions")]
        protected LanguageData[] languageSpecificDescriptions;
        public string DefaultDescription
        {
            get { return defaultDescription; }
            set { defaultDescription = value; }
        }
        public LanguageData[] LanguageSpecificDescriptions
        {
            get { return languageSpecificDescriptions; }
            set { languageSpecificDescriptions = value; }
        }
        [JsonIgnore]
        public virtual string Description
        {
            get { return Language.GetText(languageSpecificDescriptions, defaultDescription); }
        }

        [SerializeField]
        protected string category = string.Empty;
        public string Category
        {
            get { return category; }
            set { category = value; }
        }

#if UNITY_EDITOR || !UNITY_SERVER
#if UNITY_EDITOR || !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
        [PreviewSprite(50)]
        [SerializeField]
#if !DISABLE_ADDRESSABLES
        [AddressableAssetConversion(nameof(addressableIcon))]
#endif
        protected Sprite icon;
#endif
        public Sprite Icon
        {
            get
            {
#if !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
                return icon;
#else
                return null;
#endif
            }
            set
            {
#if !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
                icon = value;
#endif
            }
        }

#if !DISABLE_ADDRESSABLES
        [SerializeField]
        protected AssetReferenceSprite addressableIcon;
        public AssetReferenceSprite AddressableIcon
        {
            get
            {
                return addressableIcon;
            }
            set
            {
                addressableIcon = value;
            }
        }
#endif

        public UniTask<Sprite> GetIcon()
        {
#if !DISABLE_ADDRESSABLES
            return AddressableIcon.GetOrLoadObjectAsyncOrUseAsset(Icon);
#else
            return UniTask.FromResult(Icon);
#endif
        }
#endif

        [NonSerialized]
        private bool isDataIdSet = false;
        [NonSerialized]
        private int dataId;
        public int DataId
        {
            get
            {
                if (!isDataIdSet)
                {
                    dataId = MakeDataId(Id);
                    isDataIdSet = true;
                }
                return dataId;
            }
        }

        [NonSerialized]
        private bool isHashCodeSet = false;
        [NonSerialized]
        private int hashCode;
        public int HashCode
        {
            get
            {
                if (!isHashCodeSet)
                {
                    hashCode = (GetType().GetHashCode() * 397) ^ DataId;
                    isHashCodeSet = true;
                }
                return hashCode;
            }
        }

        public readonly static Dictionary<int, string> IdMap = new Dictionary<int, string>();
        public readonly static Dictionary<string, int> DataIdMap = new Dictionary<string, int>();

        public static int MakeDataId(string id)
        {
            if (DataIdMap.TryGetValue(id, out int dataId))
                return dataId;
            dataId = id.GenerateHashId();
            IdMap[dataId] = id;
            DataIdMap[id] = dataId;
            return dataId;
        }

        protected virtual void OnEnable()
        {
            string key = this.GetPatchKey();
            if (PatchDataManager.PatchableData.TryAdd(key, this) &&
                PatchDataManager.PatchingData.TryGetValue(key, out Dictionary<string, object> patch))
            {
                this.ApplyPatch(patch);
            }
        }

        protected virtual void OnDisable()
        {
            string key = this.GetPatchKey();
            PatchDataManager.PatchableData.Remove(key);
        }

#if UNITY_EDITOR
        protected void OnValidate()
        {
            if (Validate())
                EditorUtility.SetDirty(this);
        }
#endif

        [ContextMenu("Force Validate")]
        public virtual bool Validate()
        {
            return ValidateHashAssetID();
        }

        public virtual bool ValidateHashAssetID()
        {
            bool hasChanges = false;
#if UNITY_EDITOR && !DISABLE_ADDRESSABLES
            List<FieldSourceInfo> fieldSourceInfos = this.FindFieldsOfType<AssetReferenceLiteNetLibIdentity>();
            for (int i = 0; i < fieldSourceInfos.Count; ++i)
            {
                AssetReferenceLiteNetLibIdentity assetRefIdentity = fieldSourceInfos[i].Field.GetValue(fieldSourceInfos[i].Source) as AssetReferenceLiteNetLibIdentity;
                if (assetRefIdentity == null)
                    continue;
                if (assetRefIdentity.ValidateHashAssetID())
                {
                    Debug.Log($"Hash asset ID validated, game data {this}");
                    hasChanges = true;
                }
            }
#endif
            return hasChanges;
        }

        public virtual void PrepareRelatesData()
        {
            this.InvokeInstanceDevExtMethods("PrepareRelatesData");
        }

        public int CompareTo(object obj)
        {
            if (obj == null)
                return 1;

            BaseGameData otherGameData = obj as BaseGameData;
            if (otherGameData != null)
                return Id.CompareTo(otherGameData.Id);
            else
                throw new ArgumentException("`Object` is not a `BaseGameData`");
        }

        public override string ToString()
        {
            if (this == null)
                return null;
            return Id;
        }

        public override int GetHashCode()
        {
            return HashCode;
        }

        public override bool Equals(object obj)
        {
            return obj is BaseGameData other
                && other.HashCode == HashCode;
        }

        [ContextMenu("Test Export Data")]
        public void TestExportData()
        {
            var data = this.GetExportData();
            var json = JsonConvert.SerializeObject(data);
            Debug.Log($"Exporting data is:\n{json}");
        }

        [ContextMenu("Debug Data ID")]
        public void DebugDataId()
        {
            Debug.Log(DataId);
        }
    }
}
