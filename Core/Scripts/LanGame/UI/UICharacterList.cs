using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace MultiplayerARPG
{
    public class UICharacterList : UIBase
    {
        public UICharacter uiCharacterPrefab;
        public Transform uiCharacterContainer;
        public Transform characterModelContainer;
        [Header("UI Elements")]
        public Button buttonStart;
        public Button buttonDelete;
        [Tooltip("These objects will be activated while character selected")]
        public List<GameObject> selectedCharacterObjects = new List<GameObject>();
        [Header("Event")]
        public UnityEvent eventOnNoCharacter = new UnityEvent();
        public UnityEvent eventOnAbleToCreateCharacter = new UnityEvent();
        public UnityEvent eventOnNotAbleToCreateCharacter = new UnityEvent();
        public CharacterDataEvent eventOnSelectCharacter = new CharacterDataEvent();
        public CharacterModelEvent eventOnBeforeUpdateAnimation = new CharacterModelEvent();
        public CharacterModelEvent eventOnAfterUpdateAnimation = new CharacterModelEvent();
        public CharacterModelEvent eventOnShowInstantiatedCharacter = new CharacterModelEvent();

        private UIList _characterList;
        public UIList CharacterList
        {
            get
            {
                if (_characterList == null)
                {
                    _characterList = gameObject.AddComponent<UIList>();
                    _characterList.uiPrefab = uiCharacterPrefab.gameObject;
                    _characterList.uiContainer = uiCharacterContainer;
                }
                return _characterList;
            }
        }

        private UICharacterSelectionManager _characterSelectionManager;
        public UICharacterSelectionManager CharacterSelectionManager
        {
            get
            {
                if (_characterSelectionManager == null)
                    _characterSelectionManager = gameObject.GetOrAddComponent<UICharacterSelectionManager>();
                _characterSelectionManager.selectionMode = UISelectionMode.Toggle;
                return _characterSelectionManager;
            }
        }

        protected readonly Dictionary<string, BaseCharacterModel> _characterModelById = new Dictionary<string, BaseCharacterModel>();
        protected BaseCharacterModel _selectedModel;
        public BaseCharacterModel SelectedModel { get { return _selectedModel; } }
        protected readonly Dictionary<string, PlayerCharacterData> _playerCharacterDataById = new Dictionary<string, PlayerCharacterData>();
        protected PlayerCharacterData _selectedPlayerCharacterData;
        public PlayerCharacterData SelectedPlayerCharacterData { get { return _selectedPlayerCharacterData; } }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            uiCharacterPrefab = null;
            uiCharacterContainer = null;
            characterModelContainer = null;
            buttonStart = null;
            buttonDelete = null;
            selectedCharacterObjects.Nullify();
            selectedCharacterObjects?.Clear();
            eventOnNoCharacter?.RemoveAllListeners();
            eventOnNoCharacter = null;
            eventOnAbleToCreateCharacter?.RemoveAllListeners();
            eventOnAbleToCreateCharacter = null;
            eventOnNotAbleToCreateCharacter?.RemoveAllListeners();
            eventOnNotAbleToCreateCharacter = null;
            eventOnSelectCharacter?.RemoveAllListeners();
            eventOnSelectCharacter = null;
            eventOnBeforeUpdateAnimation?.RemoveAllListeners();
            eventOnBeforeUpdateAnimation = null;
            eventOnAfterUpdateAnimation?.RemoveAllListeners();
            eventOnAfterUpdateAnimation = null;
            eventOnShowInstantiatedCharacter?.RemoveAllListeners();
            eventOnShowInstantiatedCharacter = null;
            _characterList = null;
            _characterSelectionManager = null;
            _characterModelById?.Clear();
            _selectedModel = null;
            _playerCharacterDataById?.Clear();
            _selectedPlayerCharacterData = null;
        }

        protected virtual void LoadCharacters()
        {
            CharacterSelectionManager.Clear();
            CharacterList.HideAll();
            // Unenable buttons
            if (buttonStart)
                buttonStart.gameObject.SetActive(false);
            if (buttonDelete)
                buttonDelete.gameObject.SetActive(false);
            // Deactivate selected character objects
            foreach (GameObject obj in selectedCharacterObjects)
            {
                obj.SetActive(false);
            }
            // Remove all models
            characterModelContainer.DestroyChildren();
            _characterModelById.Clear();
            // Remove all cached data
            _playerCharacterDataById.Clear();
            // Show list of created characters
            List<PlayerCharacterData> selectableCharacters = GameInstance.Singleton.SaveSystem.LoadCharacters();
            for (int i = selectableCharacters.Count - 1; i >= 0; --i)
            {
                PlayerCharacterData selectableCharacter = selectableCharacters[i];
                if (selectableCharacter == null)
                {
                    // Null data
                    selectableCharacters.RemoveAt(i);
                    continue;
                }
                if (
#if !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
                    !GameInstance.PlayerCharacterEntities.ContainsKey(selectableCharacter.EntityId) &&
#endif
#if !DISABLE_ADDRESSABLES
                    !GameInstance.AddressablePlayerCharacterEntities.ContainsKey(selectableCharacter.EntityId) &&
#endif
                    !GameInstance.PlayerCharacterEntityMetaDataList.ContainsKey(selectableCharacter.EntityId))
                {
                    // Invalid entity data
                    selectableCharacters.RemoveAt(i);
                    continue;
                }
                if (!GameInstance.PlayerCharacters.ContainsKey(selectableCharacter.DataId))
                {
                    // Invalid character data
                    selectableCharacters.RemoveAt(i);
                    continue;
                }
            }

            if (GameInstance.Singleton.maxCharacterSaves > 0 &&
                selectableCharacters.Count >= GameInstance.Singleton.maxCharacterSaves)
                eventOnNotAbleToCreateCharacter.Invoke();
            else
                eventOnAbleToCreateCharacter.Invoke();

            // Clear selected character data, will select first in list if available
            (BaseGameNetworkManager.Singleton as LanRpgNetworkManager).selectedCharacter = _selectedPlayerCharacterData = null;

            // Generate list entry by saved characters
            if (selectableCharacters.Count > 0)
            {
                selectableCharacters.Sort(new PlayerCharacterDataLastUpdateComparer().Desc());
                CharacterList.Generate(selectableCharacters, (index, characterData, ui) =>
                {
                    // Cache player character to dictionary, we will use it later
                    _playerCharacterDataById[characterData.Id] = characterData;
                    // Setup UIs
                    UICharacter uiCharacter = ui.GetComponent<UICharacter>();
                    uiCharacter.NotForOwningCharacter = true;
                    uiCharacter.Data = characterData;
                    // Select trigger when add first entry so deactivate all models is okay beacause first model will active
                    BaseCharacterModel characterModel = characterData.InstantiateModel(characterModelContainer);
                    if (characterModel != null)
                    {
                        _characterModelById[characterData.Id] = characterModel;
                        characterModel.SetupModelBodyParts(characterData);
                        characterModel.SetEquipItemsImmediately(characterData.EquipItems, characterData.SelectableWeaponSets, characterData.EquipWeaponSet, false);
                        characterModel.gameObject.SetActive(false);
                        CharacterSelectionManager.Add(uiCharacter);
                    }
                });
            }
            else
            {
                eventOnNoCharacter.Invoke();
            }
        }

        protected virtual void OnEnable()
        {
            if (buttonStart)
            {
                buttonStart.onClick.RemoveListener(OnClickStart);
                buttonStart.onClick.AddListener(OnClickStart);
                buttonStart.gameObject.SetActive(false);
            }
            if (buttonDelete)
            {
                buttonDelete.onClick.RemoveListener(OnClickDelete);
                buttonDelete.onClick.AddListener(OnClickDelete);
                buttonDelete.gameObject.SetActive(false);
            }
            foreach (GameObject obj in selectedCharacterObjects)
            {
                obj.SetActive(false);
            }
            // Clear selection
            CharacterSelectionManager.eventOnSelect.RemoveListener(OnSelectCharacter);
            CharacterSelectionManager.eventOnSelect.AddListener(OnSelectCharacter);
            CharacterSelectionManager.Clear();
            CharacterList.HideAll();
            // Load characters
            LoadCharacters();
        }

        protected virtual void OnDisable()
        {
            characterModelContainer.DestroyChildren();
        }

        protected virtual void Update()
        {
            if (SelectedModel != null)
            {
                eventOnBeforeUpdateAnimation.Invoke(SelectedModel);
                SelectedModel.UpdateAnimation(Time.deltaTime);
                eventOnAfterUpdateAnimation.Invoke(SelectedModel);
            }
        }

        protected void OnSelectCharacter(UICharacter uiCharacter)
        {
            // Set data
            _selectedPlayerCharacterData = uiCharacter.Data as PlayerCharacterData;
            // Hide models
            characterModelContainer.SetChildrenActive(false);
            // Show selected character model
            _characterModelById.TryGetValue(_selectedPlayerCharacterData.Id, out _selectedModel);
            if (SelectedModel != null)
            {
                SelectedModel.gameObject.SetActive(true);
                eventOnShowInstantiatedCharacter.Invoke(SelectedModel);
            }
            // Run event
            eventOnSelectCharacter.Invoke(_selectedPlayerCharacterData);
            OnSelectCharacter(_selectedPlayerCharacterData);
            // Enable buttons because character was selected
            if (buttonStart)
                buttonStart.gameObject.SetActive(true);
            if (buttonDelete)
                buttonDelete.gameObject.SetActive(true);
            // Activate selected character objects because character was selected
            foreach (GameObject obj in selectedCharacterObjects)
            {
                obj.SetActive(true);
            }
        }

        protected virtual void OnSelectCharacter(IPlayerCharacterData playerCharacterData)
        {
            // Validate map data
            if (!GameInstance.Singleton.GetGameMapIds().Contains(SelectedPlayerCharacterData.CurrentMapName))
            {
                PlayerCharacter database = SelectedPlayerCharacterData.GetDatabase() as PlayerCharacter;
                BaseMapInfo startMap;
                Vector3 startPosition;
                Vector3 startRotation;
                database.GetStartMapAndTransform(SelectedPlayerCharacterData, out startMap, out startPosition, out startRotation);
                SelectedPlayerCharacterData.CurrentMapName = startMap.Id;
                SelectedPlayerCharacterData.CurrentPosition = startPosition;
                SelectedPlayerCharacterData.CurrentRotation = startRotation;
            }
            // Set selected character to network manager
            (BaseGameNetworkManager.Singleton as LanRpgNetworkManager).selectedCharacter = SelectedPlayerCharacterData;
        }

        public virtual void OnClickStart()
        {
            if (SelectedPlayerCharacterData == null)
            {
                UISceneGlobal.Singleton.ShowMessageDialog(LanguageManager.GetText(UITextKeys.UI_LABEL_ERROR.ToString()), LanguageManager.GetText(UITextKeys.UI_ERROR_NO_CHOSEN_CHARACTER_TO_START.ToString()));
                Debug.LogWarning("Cannot start game, No chosen character");
                return;
            }
            (BaseGameNetworkManager.Singleton as LanRpgNetworkManager).StartGame();
        }

        public virtual void OnClickDelete()
        {
            if (SelectedPlayerCharacterData == null)
            {
                UISceneGlobal.Singleton.ShowMessageDialog(LanguageManager.GetText(UITextKeys.UI_LABEL_ERROR.ToString()), LanguageManager.GetText(UITextKeys.UI_ERROR_NO_CHOSEN_CHARACTER_TO_DELETE.ToString()));
                Debug.LogWarning("Cannot delete character, No chosen character");
                return;
            }
            SelectedPlayerCharacterData.DeletePersistentCharacterData();
            // Reload characters
            LoadCharacters();
        }
    }
}
