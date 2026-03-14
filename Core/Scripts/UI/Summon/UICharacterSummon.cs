using System.Collections.Generic;
using Cysharp.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace MultiplayerARPG
{
    public partial class UICharacterSummon : UIDataForCharacter<CharacterSummon>
    {
        public CharacterSummon CharacterSummon { get { return Data; } }

        [Header("String Formats")]
        [Tooltip("Format => {0} = {Title}")]
        public UILocaleKeySetting formatKeyTitle = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_SIMPLE);
        [Tooltip("Format => {0} = {Remains Duration}")]
        public UILocaleKeySetting formatKeySummonRemainsDuration = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_SIMPLE);
        [Tooltip("Format => {0} = {Stack Amount}")]
        public UILocaleKeySetting formatKeySummonStack = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_SIMPLE);

        [Header("UI Elements")]
        public TextWrapper uiTextTitle;
        public Image imageIcon;
        public TextWrapper uiTextRemainsDuration;
        public TextWrapper uiTextStack;
        public UICharacter uiCharacter;

        [Header("Events")]
        public UnityEvent onTypeIsSkill = new UnityEvent();
        public UnityEvent onTypeIsPet = new UnityEvent();
        public UnityEvent onStackEntriesEmpty = new UnityEvent();
        public UnityEvent onStackEntriesNotEmpty = new UnityEvent();

        protected readonly Dictionary<uint, CharacterSummon> _stackingEntries = new Dictionary<uint, CharacterSummon>();
        protected float _summonRemainsDuration;
        private BaseGameData _tempSummonData;

        protected override void OnDestroy()
        {
            base.OnDestroy();
            uiTextTitle = null;
            imageIcon = null;
            uiTextRemainsDuration = null;
            uiTextStack = null;
            uiCharacter = null;
            onTypeIsSkill?.RemoveAllListeners();
            onTypeIsSkill = null;
            onTypeIsPet?.RemoveAllListeners();
            onTypeIsPet = null;
            onStackEntriesEmpty?.RemoveAllListeners();
            onStackEntriesEmpty = null;
            onStackEntriesNotEmpty?.RemoveAllListeners();
            onStackEntriesNotEmpty = null;
            _stackingEntries?.Clear();
            _tempSummonData = null;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            _summonRemainsDuration = 0f;
        }

        public override void ManagedUpdate()
        {
            base.ManagedUpdate();

            if (_summonRemainsDuration > 0f)
            {
                _summonRemainsDuration -= Time.deltaTime;
                if (_summonRemainsDuration <= 0f)
                    _summonRemainsDuration = 0f;
            }
            else
            {
                _summonRemainsDuration = 0f;
            }

            // Update UIs
            if (uiTextRemainsDuration != null)
            {
                uiTextRemainsDuration.SetGameObjectActive(_summonRemainsDuration > 0);
                uiTextRemainsDuration.text = ZString.Format(
                    LanguageManager.GetText(formatKeySummonRemainsDuration),
                    _summonRemainsDuration.ToString("N0"));
            }
        }

        protected override void UpdateUI()
        {
            base.UpdateUI();

            // Update remains duration
            if (_summonRemainsDuration <= 0f)
                _summonRemainsDuration = CharacterSummon.summonRemainsDuration;
        }

        protected override void UpdateData()
        {
            // Update remains duration
            if (CharacterSummon.summonRemainsDuration - _summonRemainsDuration > 1)
                _summonRemainsDuration = CharacterSummon.summonRemainsDuration;

            _tempSummonData = null;
            switch (Data.type)
            {
                case SummonType.Skill:
                    onTypeIsSkill.Invoke();
                    _tempSummonData = Data.GetSkill();
                    break;
                case SummonType.PetItem:
                    onTypeIsPet.Invoke();
                    _tempSummonData = Data.GetPetItem() as BaseItem;
                    break;
            }

            if (uiTextTitle != null)
            {
                uiTextTitle.text = ZString.Format(
                    LanguageManager.GetText(formatKeyTitle),
                    !_tempSummonData ? LanguageManager.GetUnknowTitle() : _tempSummonData.Title);
            }

            imageIcon.SetImageGameDataIcon(_tempSummonData);

            if (uiCharacter != null)
            {
                if (!_tempSummonData || !Data.CacheEntity)
                {
                    uiCharacter.Hide();
                }
                else
                {
                    uiCharacter.Show();
                    uiCharacter.NotForOwningCharacter = true;
                    uiCharacter.Data = Data.CacheEntity;
                }
            }
        }

        public override void Setup(CharacterSummon data, ICharacterData character, int indexOfData)
        {
            base.Setup(data, character, indexOfData);
            ClearStackingEntries();
        }

        private void OnStackingEntriesUpdate()
        {
            if (uiTextStack != null)
            {
                uiTextStack.text = ZString.Format(
                    LanguageManager.GetText(formatKeySummonStack),
                    _stackingEntries.Count + 1);
            }

            if (_stackingEntries.Count > 0)
                onStackEntriesNotEmpty.Invoke();
            else
                onStackEntriesEmpty.Invoke();
        }

        public void AddStackingEntry(CharacterSummon summon)
        {
            _stackingEntries[summon.objectId] = summon;
            OnStackingEntriesUpdate();
        }

        public void RemoveStackingEntry(uint objectId)
        {
            _stackingEntries.Remove(objectId);
            OnStackingEntriesUpdate();
        }

        public void ClearStackingEntries()
        {
            _stackingEntries.Clear();
            OnStackingEntriesUpdate();
        }

        public void OnClickUnSummon()
        {
            if (CharacterSummon.type == SummonType.PetItem ||
                CharacterSummon.type == SummonType.Custom)
                GameInstance.PlayingCharacterEntity.CallCmdUnSummon(CharacterSummon.objectId);
        }
    }
}
