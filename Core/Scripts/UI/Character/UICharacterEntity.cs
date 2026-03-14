using Cysharp.Text;
using LiteNetLibManager;
using UnityEngine;
using UnityEngine.UI;

namespace MultiplayerARPG
{
    public class UICharacterEntity : UIDamageableEntity<BaseCharacterEntity>
    {
        [Header("Character Entity - String Formats")]
        [Tooltip("Format => {0} = {Level}")]
        public UILocaleKeySetting formatKeyLevel = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_LEVEL);
        [Tooltip("Format => {0} = {Count Down Duration}")]
        public UILocaleKeySetting formatKeySkillCastDuration = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_SIMPLE);

        [Header("Character Entity - UI Elements")]
        public TextWrapper uiTextLevel;
        // Mp
        public UIGageValue uiGageMp;
        // Skill cast
        public GameObject uiSkillCastContainer;
        public TextWrapper uiTextSkillCast;
        public Image imageSkillCastGage;
        public Slider sliderSkillCastGage;
        public UICharacterBuffs uiCharacterBuffs;

        protected float _castingSkillCountDown;
        protected float _castingSkillDuration;
        protected BasePlayerCharacterEntity _previousPlayingCharacterEntity;

        protected override void OnDestroy()
        {
            base.OnDestroy();
            uiTextLevel = null;
            uiGageMp = null;
            uiSkillCastContainer = null;
            uiTextSkillCast = null;
            imageSkillCastGage = null;
            sliderSkillCastGage = null;
            uiCharacterBuffs = null;
            if (_previousPlayingCharacterEntity != null)
                _previousPlayingCharacterEntity.onLevelChange -= PlayingCharacterEntity_onLevelChange;
            _previousPlayingCharacterEntity = null;
        }

        protected override void AddEvents(BaseCharacterEntity entity)
        {
            if (entity == null)
                return;
            base.AddEvents(entity);
            entity.onRecached += OnRecached;
            entity.onLevelChange += OnLevelChange;
            entity.onCurrentMpChange += OnCurrentMpChange;
            entity.onBuffsOperation += OnBuffsOperation;
#if !DISABLE_CLASSIC_PK
            if (entity is BasePlayerCharacterEntity playerEntity)
                playerEntity.onPkPointChange += OnPkPointChange;
#endif
            GameInstance.OnSetPlayingCharacterEvent += GameInstance_onSetPlayingCharacter;
            GameInstance_onSetPlayingCharacter(GameInstance.PlayingCharacterEntity);
        }

        protected override void RemoveEvents(BaseCharacterEntity entity)
        {
            if (entity == null)
                return;
            base.RemoveEvents(entity);
            entity.onRecached -= OnRecached;
            entity.onLevelChange -= OnLevelChange;
            entity.onCurrentMpChange -= OnCurrentMpChange;
            entity.onBuffsOperation -= OnBuffsOperation;
#if !DISABLE_CLASSIC_PK
            if (entity is BasePlayerCharacterEntity playerEntity)
                playerEntity.onPkPointChange -= OnPkPointChange;
#endif
            GameInstance.OnSetPlayingCharacterEvent -= GameInstance_onSetPlayingCharacter;
            GameInstance_onSetPlayingCharacter(null);
        }

        protected void OnRecached()
        {
            UpdateHp();
            UpdateMp();
        }

        protected void OnLevelChange(int level)
        {
            UpdateLevel();
        }

        protected void OnCurrentMpChange(int mp)
        {
            UpdateMp();
        }

        protected void OnBuffsOperation(LiteNetLibSyncListOp operation, int index, CharacterBuff oldItem, CharacterBuff newItem)
        {
            UpdateBuffs();
        }

        protected void OnPkPointChange(int pkPoint)
        {
            UpdateTitle();
        }

        protected void GameInstance_onSetPlayingCharacter(IPlayerCharacterData playingCharacterData)
        {
            if (_previousPlayingCharacterEntity != null)
            {
                _previousPlayingCharacterEntity.onLevelChange += PlayingCharacterEntity_onLevelChange;
            }
            BasePlayerCharacterEntity playerCharacterEntity = playingCharacterData as BasePlayerCharacterEntity;
            _previousPlayingCharacterEntity = playerCharacterEntity;
            if (_previousPlayingCharacterEntity != null)
            {
                _previousPlayingCharacterEntity.onLevelChange -= PlayingCharacterEntity_onLevelChange;
                UpdateTitle();
            }
        }

        protected void PlayingCharacterEntity_onLevelChange(int level)
        {
            UpdateTitle();
        }

        public override void ManagedUpdate()
        {
            base.ManagedUpdate();

            if (Data == null)
            {
                if (uiSkillCastContainer != null)
                    uiSkillCastContainer.SetActive(false);
                return;
            }

            _castingSkillCountDown = Data.CastingSkillCountDown;
            _castingSkillDuration = Data.CastingSkillDuration;

            if (uiSkillCastContainer != null)
                uiSkillCastContainer.SetActive(_castingSkillCountDown > 0 && _castingSkillDuration > 0);

            if (uiTextSkillCast != null)
                uiTextSkillCast.text = ZString.Format(LanguageManager.GetText(formatKeySkillCastDuration), _castingSkillCountDown.ToString("N2"));

            if (imageSkillCastGage != null)
                imageSkillCastGage.fillAmount = _castingSkillDuration <= 0 ? 0 : 1 - (_castingSkillCountDown / _castingSkillDuration);

            if (sliderSkillCastGage != null)
                sliderSkillCastGage.value = _castingSkillDuration <= 0 ? 0 : 1 - (_castingSkillCountDown / _castingSkillDuration);
        }

        protected override void UpdateData()
        {
            if (Data != null)
                Data.ForceMakeCaches();
            base.UpdateData();
            UpdateLevel();
            UpdateMp();
            UpdateBuffs();
        }

        protected void UpdateLevel()
        {
            if (uiTextLevel == null)
                return;
            uiTextLevel.text = ZString.Format(LanguageManager.GetText(formatKeyLevel), Data == null ? "1" : Data.Level.ToString("N0"));
        }

        protected void UpdateMp()
        {
            if (uiGageMp == null)
                return;
            int currentMp = 0;
            int maxMp = 0;
            if (Data != null)
            {
                currentMp = Data.CurrentMp;
                maxMp = Data.MaxMp;
            }
            uiGageMp.Update(currentMp, maxMp);
        }

        protected void UpdateBuffs()
        {
            if (uiCharacterBuffs == null)
                return;
            uiCharacterBuffs.UpdateData(Data);
        }
    }
}
