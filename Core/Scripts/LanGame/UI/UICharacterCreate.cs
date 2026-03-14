using Cysharp.Threading.Tasks;
using Insthync.AddressableAssetTools;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

#if !DISABLE_ADDRESSABLES
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets;
#endif
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MultiplayerARPG
{
    public class UICharacterCreate : UIBase
    {
        [Header("Game Object Elements")]
        public Transform characterModelContainer;

        [Header("UI Elements")]
        public CharacterRaceTogglePair[] raceToggles = new CharacterRaceTogglePair[0];
        public UICharacter uiCharacterPrefab;
        public Transform uiCharacterContainer;
        public UICharacterClass uiCharacterClassPrefab;
        public Transform uiCharacterClassContainer;
        public UIFaction uiFactionPrefab;
        public Transform uiFactionContainer;

        [System.Obsolete("Deprecated, use `uiInputCharacterName` instead.")]
        [HideInInspector]
        public InputField inputCharacterName;
        public InputFieldWrapper uiInputCharacterName;
        public Button buttonCreate;
        public UIBodyPartManager[] uiBodyPartManagers = new UIBodyPartManager[0];
        public UIBlendshapeManager uiBlendshapeManager;

        [Header("Event")]
        public UnityEvent eventOnCreateCharacter = new UnityEvent();
        public CharacterDataEvent eventOnSelectCharacter = new CharacterDataEvent();
        public FactionEvent eventOnSelectFaction = new FactionEvent();
        public CharacterClassEvent eventOnSelectCharacterClass = new CharacterClassEvent();
        public CharacterModelEvent eventOnBeforeUpdateAnimation = new CharacterModelEvent();
        public CharacterModelEvent eventOnAfterUpdateAnimation = new CharacterModelEvent();
        public CharacterModelEvent eventOnShowInstantiatedCharacter = new CharacterModelEvent();

        private Toggle firstRaceToggle;
        private Dictionary<CharacterRace, Toggle> _raceToggles;
        public Dictionary<CharacterRace, Toggle> RaceToggles
        {
            get
            {
                if (_raceToggles == null)
                {
                    _raceToggles = new Dictionary<CharacterRace, Toggle>();
                    foreach (CharacterRaceTogglePair raceToggle in raceToggles)
                    {
                        if (raceToggle.race == null || raceToggle.toggle == null)
                            continue;
                        _raceToggles[raceToggle.race] = raceToggle.toggle;
                        if (firstRaceToggle == null)
                            firstRaceToggle = raceToggle.toggle;
                    }
                }
                return _raceToggles;
            }
        }

        private UIList _characterList;
        public UIList CharacterList
        {
            get
            {
                if (_characterList == null)
                {
                    _characterList = gameObject.AddComponent<UIList>();
                    if (uiCharacterPrefab != null && uiCharacterContainer != null)
                    {
                        _characterList.uiPrefab = uiCharacterPrefab.gameObject;
                        _characterList.uiContainer = uiCharacterContainer;
                    }
                }
                return _characterList;
            }
        }

        private UIList _characterClassList;
        public UIList CharacterClassList
        {
            get
            {
                if (_characterClassList == null)
                {
                    _characterClassList = gameObject.AddComponent<UIList>();
                    if (uiCharacterClassPrefab != null && uiCharacterClassContainer != null)
                    {
                        _characterClassList.uiPrefab = uiCharacterClassPrefab.gameObject;
                        _characterClassList.uiContainer = uiCharacterClassContainer;
                    }
                }
                return _characterClassList;
            }
        }

        private UIList _factionList;
        public UIList FactionList
        {
            get
            {
                if (_factionList == null)
                {
                    _factionList = gameObject.AddComponent<UIList>();
                    if (uiFactionPrefab != null && uiFactionContainer != null)
                    {
                        _factionList.uiPrefab = uiFactionPrefab.gameObject;
                        _factionList.uiContainer = uiFactionContainer;
                    }
                }
                return _factionList;
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

        private UICharacterClassSelectionManager _characterClassSelectionManager;
        public UICharacterClassSelectionManager CharacterClassSelectionManager
        {
            get
            {
                if (_characterClassSelectionManager == null)
                    _characterClassSelectionManager = gameObject.GetOrAddComponent<UICharacterClassSelectionManager>();
                _characterClassSelectionManager.selectionMode = UISelectionMode.Toggle;
                return _characterClassSelectionManager;
            }
        }

        private UIFactionSelectionManager _factionSelectionManager;
        public UIFactionSelectionManager FactionSelectionManager
        {
            get
            {
                if (_factionSelectionManager == null)
                    _factionSelectionManager = GetComponent<UIFactionSelectionManager>();
                if (_factionSelectionManager == null)
                    _factionSelectionManager = gameObject.AddComponent<UIFactionSelectionManager>();
                _factionSelectionManager.selectionMode = UISelectionMode.Toggle;
                return _factionSelectionManager;
            }
        }

        protected readonly Dictionary<int, BaseCharacterModel> _characterModelByEntityId = new Dictionary<int, BaseCharacterModel>();
        protected BaseCharacterModel _selectedModel;
        public BaseCharacterModel SelectedModel { get { return _selectedModel; } }
        protected readonly Dictionary<int, List<PlayerCharacter>> _playerCharactersByEntityId = new Dictionary<int, List<PlayerCharacter>>();
        protected List<PlayerCharacter> _selectableCharacterClasses;
        public List<PlayerCharacter> SelectableCharacterClasses { get { return _selectableCharacterClasses; } }
        protected PlayerCharacter _selectedPlayerCharacter;
        public PlayerCharacter SelectedPlayerCharacter { get { return _selectedPlayerCharacter; } }
        protected PlayerCharacterData _selectedPlayerCharacterData;
        public PlayerCharacterData SelectedPlayerCharacterData { get { return _selectedPlayerCharacterData; } }
        protected readonly HashSet<CharacterRace> SelectedRaces = new HashSet<CharacterRace>();
        protected Faction _selectedFaction;
        public Faction SelectedFaction { get { return _selectedFaction; } }
        public int SelectedEntityId { get; protected set; }
        public int SelectedDataId { get; protected set; }
        public int SelectedFactionId { get; protected set; }
        public List<CharacterDataBoolean> PublicBools { get; protected set; } = new List<CharacterDataBoolean>();
        public List<CharacterDataInt32> PublicInts { get; protected set; } = new List<CharacterDataInt32>();
        public List<CharacterDataFloat32> PublicFloats { get; protected set; } = new List<CharacterDataFloat32>();

        protected override void Awake()
        {
            base.Awake();
            MigrateInputComponent();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            characterModelContainer = null;
            uiCharacterPrefab = null;
            uiCharacterContainer = null;
            uiCharacterClassPrefab = null;
            uiCharacterClassContainer = null;
            uiFactionPrefab = null;
            uiFactionContainer = null;
            uiInputCharacterName = null;
            buttonCreate = null;
            uiBodyPartManagers.Nullify();
            uiBlendshapeManager = null;
            eventOnCreateCharacter?.RemoveAllListeners();
            eventOnCreateCharacter = null;
            eventOnSelectCharacter?.RemoveAllListeners();
            eventOnSelectCharacter = null;
            eventOnSelectFaction?.RemoveAllListeners();
            eventOnSelectFaction = null;
            eventOnSelectCharacterClass?.RemoveAllListeners();
            eventOnSelectCharacterClass = null;
            eventOnBeforeUpdateAnimation?.RemoveAllListeners();
            eventOnBeforeUpdateAnimation = null;
            eventOnAfterUpdateAnimation?.RemoveAllListeners();
            eventOnAfterUpdateAnimation = null;
            eventOnShowInstantiatedCharacter?.RemoveAllListeners();
            eventOnShowInstantiatedCharacter = null;
            firstRaceToggle = null;
            _raceToggles?.Clear();
            _characterList = null;
            _characterClassList = null;
            _factionList = null;
            _characterSelectionManager = null;
            _characterClassSelectionManager = null;
            _factionSelectionManager = null;
            _characterModelByEntityId?.Clear();
            _selectedModel = null;
            _playerCharactersByEntityId?.Clear();
            _selectableCharacterClasses.Nullify();
            _selectedPlayerCharacter = null;
            _selectedPlayerCharacterData = null;
            SelectedRaces?.Clear();
            _selectedFaction = null;
            PublicBools?.Clear();
            PublicInts?.Clear();
            PublicFloats?.Clear();
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (MigrateInputComponent())
                EditorUtility.SetDirty(this);
        }
#endif

        public bool MigrateInputComponent()
        {
            bool hasChanges = false;
            InputFieldWrapper wrapper;
#pragma warning disable CS0618 // Type or member is obsolete
            if (inputCharacterName != null)
            {
                hasChanges = true;
                wrapper = inputCharacterName.gameObject.GetOrAddComponent<InputFieldWrapper>();
                wrapper.unityInputField = inputCharacterName;
                uiInputCharacterName = wrapper;
                inputCharacterName = null;
            }
#pragma warning restore CS0618 // Type or member is obsolete
            return hasChanges;
        }

        protected virtual async Task<List<PlayerCharacterData>> GetCreatableCharacters()
        {
            List<PlayerCharacterData> result = new List<PlayerCharacterData>();
#if !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
            if (GameInstance.PlayerCharacterEntities.Count > 0)
            {
                foreach (BasePlayerCharacterEntity prefab in GameInstance.PlayerCharacterEntities.Values)
                {
                    if (RaceToggles.Count > 0 && prefab.Race != null && !SelectedRaces.Contains(prefab.Race))
                        continue;
                    PlayerCharacterData data = prefab.CloneTo(new PlayerCharacterData(), true, true, true, false, false, true, false, false, false, false, false, false, false, false, false);
                    data.CharacterName = prefab.EntityTitle;
                    result.Add(data);
                    _playerCharactersByEntityId[prefab.EntityId] = new List<PlayerCharacter>(prefab.CharacterDatabases);
                }
            }
#endif
#if !DISABLE_ADDRESSABLES
            if (GameInstance.AddressablePlayerCharacterEntities.Count > 0)
            {
                List<AsyncOperationHandle<GameObject>> asyncOps = new List<AsyncOperationHandle<GameObject>>();
                List<Task<GameObject>> loadTasks = new List<Task<GameObject>>();
                foreach (AssetReferenceBasePlayerCharacterEntity entry in GameInstance.AddressablePlayerCharacterEntities.Values)
                {
                    AsyncOperationHandle<GameObject> asyncOp = Addressables.LoadAssetAsync<GameObject>(entry.RuntimeKey);
                    asyncOps.Add(asyncOp);
                    loadTasks.Add(asyncOp.Task);
                }
                await Task.WhenAll(loadTasks);
                for (int i = 0; i < loadTasks.Count; ++i)
                {
                    GameObject loadedObject = asyncOps[i].Result;
                    if (!loadedObject.TryGetComponent(out BasePlayerCharacterEntity prefab))
                    {
                        Addressables.Release(asyncOps[i]);
                        continue;
                    }
                    if (RaceToggles.Count > 0 && prefab.Race != null && !SelectedRaces.Contains(prefab.Race))
                    {
                        Addressables.Release(asyncOps[i]);
                        continue;
                    }
                    PlayerCharacterData data = prefab.CloneTo(new PlayerCharacterData(), true, true, true, false, false, true, false, false, false, false, false, false, false, false, false);
                    data.CharacterName = prefab.EntityTitle;
                    result.Add(data);
                    _playerCharactersByEntityId[prefab.EntityId] = new List<PlayerCharacter>(prefab.CharacterDatabases);
                    if (RaceToggles.Count > 0 && prefab.Race != null && !SelectedRaces.Contains(prefab.Race))
                        Addressables.Release(asyncOps[i]);
                }
            }
#endif
            if (GameInstance.PlayerCharacterEntityMetaDataList.Count > 0)
            {
#if !DISABLE_ADDRESSABLES
                List<AsyncOperationHandle<GameObject>> asyncOps = new List<AsyncOperationHandle<GameObject>>();
                List<Task<GameObject>> loadTasks = new List<Task<GameObject>>();
#endif
                List<PlayerCharacterEntityMetaData> loadMetaDataList = new List<PlayerCharacterEntityMetaData>();
                foreach (PlayerCharacterEntityMetaData entry in GameInstance.PlayerCharacterEntityMetaDataList.Values)
                {
                    if (RaceToggles.Count > 0 && entry.Race != null && !SelectedRaces.Contains(entry.Race))
                        continue;
#if !DISABLE_ADDRESSABLES
                    if (entry.AddressableEntityPrefab.IsDataValid())
                    {
                        AsyncOperationHandle<GameObject> asyncOp = Addressables.LoadAssetAsync<GameObject>(entry.AddressableEntityPrefab.RuntimeKey);
                        asyncOps.Add(asyncOp);
                        loadTasks.Add(asyncOp.Task);
                        loadMetaDataList.Add(entry);
                    }
#else
                    if (false) { }
#endif
                    else if (entry.EntityPrefab != null)
                    {
                        BasePlayerCharacterEntity prefab = entry.EntityPrefab;
                        PlayerCharacterData data = prefab.CloneTo(new PlayerCharacterData(), true, true, true, false, false, true, false, false, false, false, false, false, false, false, false);
                        data.CharacterName = entry.Title;
                        data.EntityId = entry.DataId;
                        result.Add(data);
                    }
                    _playerCharactersByEntityId[entry.DataId] = new List<PlayerCharacter>(entry.CharacterDatabases);
                }
#if !DISABLE_ADDRESSABLES
                await Task.WhenAll(loadTasks);
                for (int i = 0; i < loadTasks.Count; ++i)
                {
                    GameObject loadedObject = asyncOps[i].Result;
                    if (!loadedObject.TryGetComponent(out BasePlayerCharacterEntity prefab))
                    {
                        Addressables.Release(asyncOps[i]);
                        continue;
                    }
                    PlayerCharacterData data = prefab.CloneTo(new PlayerCharacterData(), true, true, true, false, false, true, false, false, false, false, false, false, false, false, false);
                    data.CharacterName = loadMetaDataList[i].Title;
                    data.EntityId = loadMetaDataList[i].DataId;
                    result.Add(data);
                    Addressables.Release(asyncOps[i]);
                }
#endif
            }
            await UniTask.Yield();
            return result;
        }

        protected virtual List<Faction> GetSelectableFactions()
        {
            return GameInstance.Factions.Values.Where(o => !o.IsLocked).ToList();
        }

        protected virtual async void LoadCharacters()
        {
            // Remove all models
            characterModelContainer.DestroyChildren();
            _characterModelByEntityId.Clear();
            // Remove all cached data
            _playerCharactersByEntityId.Clear();
            // Clear character selection
            CharacterSelectionManager.Clear();
            CharacterList.HideAll();
            // Show list of characters that can be created
            PlayerCharacterData firstData = null;
            CharacterList.Generate(await GetCreatableCharacters(), (index, characterData, ui) =>
            {
                // Prepare data
                BaseCharacter playerCharacter = _playerCharactersByEntityId[characterData.EntityId][0];
                PlayerCharacterData playerCharacterData = new PlayerCharacterData();
                playerCharacterData.SetNewPlayerCharacterData(characterData.CharacterName, playerCharacter.DataId, characterData.EntityId, characterData.FactionId);
                // Hide all model, the first one will be shown later
                BaseCharacterModel characterModel = playerCharacterData.InstantiateModel(characterModelContainer);
                _characterModelByEntityId[playerCharacterData.EntityId] = characterModel;
                characterModel.gameObject.SetActive(false);
                // Setup UI
                if (ui != null)
                {
                    UICharacter uiCharacter = ui.GetComponent<UICharacter>();
                    uiCharacter.NotForOwningCharacter = true;
                    uiCharacter.Data = playerCharacterData;
                    CharacterSelectionManager.Add(uiCharacter);
                }
                if (index == 0)
                    firstData = playerCharacterData;
            });
            // Select first entry
            if (CharacterSelectionManager.Count > 0)
            {
                CharacterSelectionManager.Select(0);
            }
            else
            {
                SetSelectCharacter(firstData);
            }
        }

        protected virtual void LoadFactions()
        {
            // Clear faction selection
            FactionSelectionManager.Clear();
            FactionList.HideAll();
            // Show list of factions that can be selected
            Faction firstData = null;
            FactionList.Generate(GetSelectableFactions(), (index, faction, ui) =>
            {
                // Setup UI
                if (ui != null)
                {
                    UIFaction uiFaction = ui.GetComponent<UIFaction>();
                    uiFaction.Data = faction;
                    FactionSelectionManager.Add(uiFaction);
                }
                if (index == 0)
                    firstData = faction;
            });
            // Select first entry
            if (FactionSelectionManager.Count > 0)
            {
                FactionSelectionManager.Select(0);
            }
            else
            {
                SetSelectFaction(firstData);
            }
        }

        protected virtual void OnEnable()
        {
            // Setup Events
            if (buttonCreate)
            {
                buttonCreate.onClick.RemoveListener(OnClickCreate);
                buttonCreate.onClick.AddListener(OnClickCreate);
            }
            CharacterSelectionManager.eventOnSelect.RemoveListener(OnSelectCharacter);
            CharacterSelectionManager.eventOnSelect.AddListener(OnSelectCharacter);
            CharacterClassSelectionManager.eventOnSelect.RemoveListener(OnSelectCharacterClass);
            CharacterClassSelectionManager.eventOnSelect.AddListener(OnSelectCharacterClass);
            FactionSelectionManager.eventOnSelect.RemoveListener(OnSelectFaction);
            FactionSelectionManager.eventOnSelect.AddListener(OnSelectFaction);
            SelectedRaces.Clear();
            if (RaceToggles.Count > 0)
            {
                foreach (KeyValuePair<CharacterRace, Toggle> raceToggle in RaceToggles)
                {
                    raceToggle.Value.SetIsOnWithoutNotify(false);
                    raceToggle.Value.onValueChanged.RemoveAllListeners();
                    raceToggle.Value.onValueChanged.AddListener((isOn) =>
                    {
                        OnRaceToggleUpdate(raceToggle.Key, isOn);
                    });
                }
                firstRaceToggle.isOn = true;
            }
            else
            {
                LoadCharacters();
            }
            LoadFactions();
        }

        protected virtual void OnDisable()
        {
            characterModelContainer.DestroyChildren();
            uiInputCharacterName.text = string.Empty;
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
            SetSelectCharacter(uiCharacter.Data as PlayerCharacterData);
        }

        protected void SetSelectCharacter(PlayerCharacterData playerCharacterData)
        {
            _selectedPlayerCharacterData = playerCharacterData;
            SelectedDataId = _selectedPlayerCharacterData.DataId;
            SelectedEntityId = _selectedPlayerCharacterData.EntityId;
            PublicBools.Clear();
            PublicInts.Clear();
            PublicFloats.Clear();
            // Hide models
            characterModelContainer.SetChildrenActive(false);
            // Show selected character model
            _characterModelByEntityId.TryGetValue(SelectedEntityId, out _selectedModel);
            if (SelectedModel != null)
            {
                SelectedModel.gameObject.SetActive(true);
                eventOnShowInstantiatedCharacter.Invoke(SelectedModel);
                for (int i = 0; i < uiBodyPartManagers.Length; ++i)
                {
                    uiBodyPartManagers[i].onSetModelValue = PublicInts.SetValue;
                    uiBodyPartManagers[i].onSetColorValue = PublicInts.SetValue;
                    uiBodyPartManagers[i].SetCharacterModel(SelectedModel);
                }
                if (uiBlendshapeManager != null)
                {
                    uiBlendshapeManager.onSetBlendshapeValue = PublicFloats.SetValue;
                    uiBlendshapeManager.SetCharacterModel(SelectedModel);
                }
            }
            // Run event
            eventOnSelectCharacter.Invoke(_selectedPlayerCharacterData);
            OnSelectCharacter(_selectedPlayerCharacterData);
            // Clear character class selection
            CharacterClassSelectionManager.Clear();
            CharacterClassList.HideAll();
            // Setup character class list
            PlayerCharacter firstData = null;
            _playerCharactersByEntityId.TryGetValue(SelectedEntityId, out _selectableCharacterClasses);
            CharacterClassList.Generate(_selectableCharacterClasses, (index, playerCharacter, ui) =>
            {
                // Setup UI
                if (ui != null)
                {
                    UICharacterClass uiCharacterClass = ui.GetComponent<UICharacterClass>();
                    uiCharacterClass.Data = playerCharacter;
                    CharacterClassSelectionManager.Add(uiCharacterClass);
                }
                if (index == 0)
                    firstData = playerCharacter;
            });
            // Select first entry
            if (CharacterClassSelectionManager.Count > 0)
            {
                CharacterClassSelectionManager.Select(0);
            }
            else
            {
                SetSelectCharacterClass(firstData);
            }
        }

        protected virtual void OnSelectCharacter(IPlayerCharacterData playerCharacterData)
        {
            // NOTE: Override this function to do something as you wish
        }

        protected void OnSelectCharacterClass(UICharacterClass uiCharacterClass)
        {
            SetSelectCharacterClass(uiCharacterClass.Data as PlayerCharacter);
        }

        protected void SetSelectCharacterClass(PlayerCharacter playerCharacter)
        {
            _selectedPlayerCharacter = playerCharacter;
            if (SelectedPlayerCharacter != null)
            {
                // Set creating player character data
                SelectedDataId = _selectedPlayerCharacter.DataId;
                // Prepare equip items
                List<CharacterItem> equipItems = new List<CharacterItem>();
                foreach (BaseItem armorItem in SelectedPlayerCharacter.ArmorItems)
                {
                    if (armorItem == null)
                        continue;
                    equipItems.Add(CharacterItem.Create(armorItem));
                }
                // Prepare equip weapons
                EquipWeapons equipWeapons = new EquipWeapons();
                if (SelectedPlayerCharacter.RightHandEquipItem != null)
                    equipWeapons.rightHand = CharacterItem.Create(SelectedPlayerCharacter.RightHandEquipItem);
                if (SelectedPlayerCharacter.LeftHandEquipItem != null)
                    equipWeapons.leftHand = CharacterItem.Create(SelectedPlayerCharacter.LeftHandEquipItem);
                // Set model equip weapons
                IList<EquipWeapons> selectableWeaponSets = new List<EquipWeapons>
                {
                    equipWeapons
                };
                // Set model equip items
                SelectedModel.SetEquipItemsImmediately(equipItems, selectableWeaponSets, 0, false);
            }
            // Run event
            eventOnSelectCharacterClass.Invoke(_selectedPlayerCharacter);
            OnSelectCharacterClass(_selectedPlayerCharacter);
        }

        protected virtual void OnSelectCharacterClass(BaseCharacter baseCharacter)
        {
            // NOTE: Override this function to do something as you wish
        }

        protected void OnSelectFaction(UIFaction uiFaction)
        {
            SetSelectFaction(uiFaction.Data);
        }

        protected void SetSelectFaction(Faction faction)
        {
            _selectedFaction = faction;
            if (SelectedFaction != null)
            {
                // Set creating player character's faction
                SelectedFactionId = _selectedFaction.DataId;
            }
            // Run event
            eventOnSelectFaction.Invoke(_selectedFaction);
            OnSelectFaction(_selectedFaction);
        }

        protected virtual void OnSelectFaction(Faction faction)
        {
            // NOTE: Override this function to do something as you wish
        }

        protected virtual void OnRaceToggleUpdate(CharacterRace race, bool isOn)
        {
            if (isOn)
            {
                SelectedRaces.Add(race);
                LoadCharacters();
            }
            else
            {
                SelectedRaces.Remove(race);
            }
        }

        protected virtual void OnClickCreate()
        {
            GameInstance gameInstance = GameInstance.Singleton;
            // Validate character name
            string characterName = uiInputCharacterName.text.Trim();
            int minCharacterNameLength = gameInstance.minCharacterNameLength;
            int maxCharacterNameLength = gameInstance.maxCharacterNameLength;
            if (characterName.Length < minCharacterNameLength)
            {
                UISceneGlobal.Singleton.ShowMessageDialog(LanguageManager.GetText(UITextKeys.UI_LABEL_ERROR.ToString()), LanguageManager.GetText(UITextKeys.UI_ERROR_CHARACTER_NAME_TOO_SHORT.ToString()));
                Debug.LogWarning("Cannot create character, character name is too short");
                return;
            }
            if (characterName.Length > maxCharacterNameLength)
            {
                UISceneGlobal.Singleton.ShowMessageDialog(LanguageManager.GetText(UITextKeys.UI_LABEL_ERROR.ToString()), LanguageManager.GetText(UITextKeys.UI_ERROR_CHARACTER_NAME_TOO_LONG.ToString()));
                Debug.LogWarning("Cannot create character, character name is too long");
                return;
            }

            SaveCreatingPlayerCharacter(characterName);

            if (eventOnCreateCharacter != null)
                eventOnCreateCharacter.Invoke();
        }

        protected virtual void SaveCreatingPlayerCharacter(string characterName)
        {
            PlayerCharacterData characterData = new PlayerCharacterData();
            characterData.Id = GenericUtils.GetUniqueId();
            characterData.SetNewPlayerCharacterData(characterName, SelectedDataId, SelectedEntityId, SelectedFactionId);
#if !DISABLE_CUSTOM_CHARACTER_DATA
            characterData.PublicBools = PublicBools;
            characterData.PublicInts = PublicInts;
            characterData.PublicFloats = PublicFloats;
#endif
            GameInstance.Singleton.SaveSystem.SaveCharacter(characterData);
        }
    }
}
