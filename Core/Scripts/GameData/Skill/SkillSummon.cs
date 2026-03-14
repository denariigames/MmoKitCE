using Insthync.AddressableAssetTools;
using UnityEngine;
using UnityEngine.Serialization;

namespace MultiplayerARPG
{
    [System.Serializable]
    public partial class SkillSummon : IAddressableAssetConversable
    {
#if UNITY_EDITOR || !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
        [Tooltip("Leave `Monster Entity` to NULL to not summon monster entity")]
        [SerializeField]
        [FormerlySerializedAs("monsterEntity")]
        private BaseMonsterCharacterEntity monsterCharacterEntity;
#endif
        public BaseMonsterCharacterEntity MonsterCharacterEntity
        {
            get
            {
#if !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
                return monsterCharacterEntity;
#else
                return null;
#endif
            }
        }

#if !DISABLE_ADDRESSABLES
        [SerializeField]
        private AssetReferenceBaseMonsterCharacterEntity addressableMonsterCharacterEntity;
        public AssetReferenceBaseMonsterCharacterEntity AddressableMonsterCharacterEntity
        {
            get
            {
                return addressableMonsterCharacterEntity;
            }
        }
#endif

        [SerializeField]
        private IncrementalFloat duration;
        public IncrementalFloat Duration { get { return duration; } }

        [SerializeField]
        private bool noDuration;
        public bool NoDuration { get { return noDuration; } }

        [SerializeField]
        private IncrementalInt amountEachTime;
        public IncrementalInt AmountEachTime { get { return amountEachTime; } }

        [SerializeField]
        private IncrementalInt maxStack;
        public IncrementalInt MaxStack { get { return maxStack; } }

        [SerializeField]
        private IncrementalInt level;
        public IncrementalInt Level { get { return level; } }

        public void ProceedAddressableAssetConversion(string groupName)
        {
#if UNITY_EDITOR && !DISABLE_ADDRESSABLES
            AddressableEditorUtils.ConvertObjectRefToAddressable(ref monsterCharacterEntity, ref addressableMonsterCharacterEntity, groupName);
#endif
        }
    }
}
