using UnityEngine;

namespace MultiplayerARPG
{
    public class BaseBuffEntity : PoolDescriptor
    {
        /// <summary>
        /// If this is `TRUE` buffs will applies to everyone including with an enemies
        /// </summary>
        protected bool _applyBuffToEveryone;
        protected EntityInfo _buffApplier;
        protected BaseSkill _skill;
        protected int _skillLevel;

        public GameInstance CurrentGameInstance
        {
            get { return GameInstance.Singleton; }
        }

        public BaseGameplayRule CurrentGameplayRule
        {
            get { return CurrentGameInstance.GameplayRule; }
        }

        public BaseGameNetworkManager CurrentGameManager
        {
            get { return BaseGameNetworkManager.Singleton; }
        }

        public bool IsServer
        {
            get { return CurrentGameManager.IsServer; }
        }

        public bool IsClient
        {
            get { return CurrentGameManager.IsClient; }
        }
        
        public Transform CacheTransform { get; private set; }
        private FxCollection _fxCollection;
        public FxCollection FxCollection
        {
            get
            {
                if (_fxCollection == null)
                    _fxCollection = new FxCollection(gameObject);
                return _fxCollection;
            }
        }
        private bool _playFxOnEnable;

        protected virtual void Awake()
        {
            CacheTransform = transform;
        }

        protected virtual void OnDestroy()
        {
            _skill = null;
            _fxCollection = null;
            CacheTransform = null;
        }

        protected virtual void OnEnable()
        {
            if (_playFxOnEnable)
                PlayFx();
        }

        public virtual void Setup(
            EntityInfo buffApplier,
            BaseSkill skill,
            int skillLevel,
            bool applyBuffToEveryone)
        {
            _buffApplier = buffApplier;
            _skill = skill;
            _skillLevel = skillLevel;
            _applyBuffToEveryone = applyBuffToEveryone;
        }

        public virtual void ApplyBuffTo(BaseCharacterEntity target)
        {
            if (!IsServer || target == null || target.IsDead() || (!_applyBuffToEveryone && !target.IsAlly(_buffApplier)))
                return;
            target.ApplyBuff(_skill.DataId, BuffType.SkillBuff, _skillLevel, _buffApplier, CharacterItem.Empty);
        }

        public override void InitPrefab()
        {
            if (this == null)
            {
                Debug.LogWarning("The Base Bufff Entity is null, this should not happens");
                return;
            }
            FxCollection.InitPrefab();
            base.InitPrefab();
        }

        public override void OnGetInstance()
        {
            PlayFx();
            base.OnGetInstance();
        }

        public override void OnPushBack()
        {
            StopFx();
            base.OnPushBack();
        }

        public virtual void PlayFx()
        {
            if (!gameObject.activeInHierarchy)
            {
                _playFxOnEnable = true;
                return;
            }
            FxCollection.Play();
            _playFxOnEnable = false;
        }

        public virtual void StopFx()
        {
            FxCollection.Stop();
        }
    }
}
