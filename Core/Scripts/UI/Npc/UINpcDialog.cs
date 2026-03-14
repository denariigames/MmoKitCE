using Cysharp.Text;
using LiteNetLibManager;
using UnityEngine;
using UnityEngine.UI;

namespace MultiplayerARPG
{
    public partial class UINpcDialog : UISelectionEntry<BaseNpcDialog>
    {
        [Header("String Formats")]
        [Tooltip("Format => {0} = {Title}")]
        public UILocaleKeySetting formatKeyTitle = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_SIMPLE);
        [Tooltip("Format => {0} = {Description}")]
        public UILocaleKeySetting formatKeyDescription = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_SIMPLE);

        [Header("UI Elements")]
        public TextWrapper uiTextTitle;
        public TextWrapper uiTextDescription;
        public Image imageIcon;
        public AudioSource voiceSource;

        protected BaseNpcDialog _lastData;
        protected BasePlayerCharacterEntity _previousEntity;

        protected override void OnDestroy()
        {
            base.OnDestroy();
            uiTextTitle = null;
            uiTextDescription = null;
            imageIcon = null;
            voiceSource = null;
            _lastData = null;
            _previousEntity = null;
            _data = null;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            if (voiceSource != null)
            {
                if (voiceSource.clip != null)
                    voiceSource.Play();
            }
            GameInstance.OnSetPlayingCharacterEvent += GameInstance_onSetPlayingCharacter;
            GameInstance_onSetPlayingCharacter(GameInstance.PlayingCharacterEntity);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            if (voiceSource != null)
            {
                voiceSource.Stop();
                voiceSource.clip = null;
            }
            GameInstance.OnSetPlayingCharacterEvent -= GameInstance_onSetPlayingCharacter;
            GameInstance_onSetPlayingCharacter(null);
        }

        private void GameInstance_onSetPlayingCharacter(IPlayerCharacterData playingCharacterData)
        {
            RemoveEvents(_previousEntity);
            BasePlayerCharacterEntity playerCharacterEntity = playingCharacterData as BasePlayerCharacterEntity;
            _previousEntity = playerCharacterEntity;
            AddEvents(_previousEntity);
            if (_previousEntity != null)
                ReRenderUI();
        }

        private void AddEvents(BasePlayerCharacterEntity PlayingCharacterEntity)
        {
            if (PlayingCharacterEntity == null)
                return;
            PlayingCharacterEntity.onRecached += ReRenderUI;
            PlayingCharacterEntity.onQuestsOperation += PlayingCharacterEntity_onQuestsOperation;
        }

        private void RemoveEvents(BasePlayerCharacterEntity PlayingCharacterEntity)
        {
            if (PlayingCharacterEntity == null)
                return;
            PlayingCharacterEntity.onRecached -= ReRenderUI;
            PlayingCharacterEntity.onQuestsOperation -= PlayingCharacterEntity_onQuestsOperation;
        }

        private void PlayingCharacterEntity_onQuestsOperation(LiteNetLibSyncListOp operation, int index, CharacterQuest oldItem, CharacterQuest newItem)
        {
            ReRenderUI();
        }

        protected async void ReRenderUI()
        {
            if (_lastData != null)
                await _lastData.RenderUI(this);
        }

        protected override async void UpdateData()
        {
            if (_lastData != null)
                _lastData.UnrenderUI(this);

            if (uiTextTitle != null)
            {
                uiTextTitle.text = GameInstance.Singleton.MessageManager.ReplaceKeysToMessages(ZString.Format(
                    LanguageManager.GetText(formatKeyTitle),
                    Data == null ? LanguageManager.GetUnknowTitle() : Data.Title));
            }

            if (uiTextDescription != null)
            {
                uiTextDescription.text = GameInstance.Singleton.MessageManager.ReplaceKeysToMessages(ZString.Format(
                    LanguageManager.GetText(formatKeyDescription),
                    Data == null ? LanguageManager.GetUnknowDescription() : Data.Description));
            }

            imageIcon.SetImageNpcDialogIcon(Data);

            voiceSource.PlayNpcDialogVoice(this, Data);

            _lastData = Data;
            if (_lastData != null)
                await _lastData.RenderUI(this);
        }
    }
}
