using Insthync.AddressableAssetTools;
using Insthync.DevExtension;
using Insthync.SerializationSurrogates;
using LiteNetLibManager;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
#if !DISABLE_ADDRESSABLES
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
#endif

namespace MultiplayerARPG
{
    public static partial class PlayerCharacterDataExtensions
    {
        public static T ValidateCharacterData<T>(this T character) where T : IPlayerCharacterData
        {
            if (!GameInstance.PlayerCharacters.TryGetValue(character.DataId, out PlayerCharacter database))
                return character;
            // Validating character attributes
            int returningStatPoint = 0;
            HashSet<int> validAttributeIds = new HashSet<int>();
            IList<CharacterAttribute> characterAttributes = character.Attributes;
            for (int i = characterAttributes.Count - 1; i >= 0; --i)
            {
                CharacterAttribute characterAttribute = characterAttributes[i];
                int attributeDataId = characterAttribute.dataId;
                // If attribute is invalid
                if (characterAttribute.GetAttribute() == null ||
                    validAttributeIds.Contains(attributeDataId))
                {
                    returningStatPoint += characterAttribute.amount;
                    character.Attributes.RemoveAt(i);
                }
                else
                    validAttributeIds.Add(attributeDataId);
            }
            character.StatPoint += returningStatPoint;
            // Add character's attributes
            foreach (Attribute attribute in GameInstance.Attributes.Values)
            {
                // This attribute is valid, so not have to add it
                if (validAttributeIds.Contains(attribute.DataId))
                    continue;
                character.Attributes.Add(CharacterAttribute.Create(attribute.DataId, 0));
            }
            // Validating character skills
            int returningSkillPoint = 0;
            HashSet<int> validSkillIds = new HashSet<int>();
            HashSet<int> learnableSkillDataIds = database.GetLearnableSkillDataIds();
            IList<CharacterSkill> characterSkills = character.Skills;
            for (int i = characterSkills.Count - 1; i >= 0; --i)
            {
                CharacterSkill characterSkill = characterSkills[i];
                BaseSkill skill = characterSkill.GetSkill();
                // If skill is invalid or this character database does not have skill
                if (characterSkill.GetSkill() == null ||
                    !learnableSkillDataIds.Contains(skill.DataId) ||
                    validSkillIds.Contains(skill.DataId))
                {
                    returningSkillPoint += characterSkill.level;
                    character.Skills.RemoveAt(i);
                }
                else
                    validSkillIds.Add(skill.DataId);
            }
            character.SkillPoint += returningSkillPoint;
            // Add character's skills
            foreach (int skillDataId in learnableSkillDataIds)
            {
                // This skill is valid, so not have to add it
                if (validSkillIds.Contains(skillDataId))
                    continue;
                character.Skills.Add(CharacterSkill.Create(skillDataId, 0));
            }
            // Validating character equip weapons
            List<CharacterItem> returningItems = new List<CharacterItem>();
            EquipWeapons equipWeapons = character.EquipWeapons;
            CharacterItem rightHand = equipWeapons.rightHand;
            CharacterItem leftHand = equipWeapons.leftHand;
            if (rightHand.GetEquipmentItem() == null)
            {
                if (!rightHand.IsEmptySlot())
                    returningItems.Add(rightHand);
                equipWeapons.rightHand = CharacterItem.Empty;
            }
            if (leftHand.GetEquipmentItem() == null)
            {
                if (!leftHand.IsEmptySlot())
                    returningItems.Add(leftHand);
                equipWeapons.leftHand = CharacterItem.Empty;
            }
            // Validating character equip items
            IList<CharacterItem> equipItems = character.EquipItems;
            for (int i = equipItems.Count - 1; i >= 0; --i)
            {
                CharacterItem equipItem = equipItems[i];
                // If equipment is invalid
                if (equipItem.GetEquipmentItem() == null)
                {
                    if (!equipItem.IsEmptySlot())
                        returningItems.Add(equipItem);
                    character.EquipItems.RemoveAt(i);
                }
            }
            // Return items to non equip items
            foreach (CharacterItem returningItem in returningItems)
            {
                if (!returningItem.IsEmptySlot())
                    character.AddOrSetNonEquipItems(returningItem);
            }
            character.FillEmptySlots();
            DevExtUtils.InvokeStaticDevExtMethods(ClassType, "ValidateCharacterData", character);
            return character;
        }

        public static T SetNewPlayerCharacterData<T>(this T character, string characterName, int dataId, int entityId, int factionId) where T : IPlayerCharacterData
        {
            int startGold = GameInstance.Singleton.NewCharacterSetting != null ? GameInstance.Singleton.NewCharacterSetting.startGold : GameInstance.Singleton.startGold;
            ItemAmount[] startItems = GameInstance.Singleton.NewCharacterSetting != null ? GameInstance.Singleton.NewCharacterSetting.startItems : GameInstance.Singleton.startItems;
            return character.SetNewPlayerCharacterData(GameInstance.PlayerCharacters, GameInstance.Attributes, startGold, startItems, characterName, dataId, entityId, factionId);
        }

        public static T SetNewPlayerCharacterData<T>(this T character, Dictionary<int, PlayerCharacter> playerCharacters, Dictionary<int, Attribute> attributes, int startGold, ItemAmount[] startItems, string characterName, int dataId, int entityId, int factionId) where T : IPlayerCharacterData
        {
            if (!playerCharacters.TryGetValue(dataId, out PlayerCharacter playerCharacter))
                return character;

            // General data
            character.DataId = dataId;
            character.EntityId = entityId;
            character.CharacterName = characterName;
            character.Level = 1;
            // Attributes
            foreach (Attribute attribute in attributes.Values)
            {
                character.Attributes.Add(CharacterAttribute.Create(attribute.DataId, 0));
            }
            // Skills
            HashSet<int> learnableSkillDataIds = playerCharacter.GetLearnableSkillDataIds();
            foreach (int skillDataId in learnableSkillDataIds)
            {
                character.Skills.Add(CharacterSkill.Create(skillDataId, 0));
            }
            // Prepare weapon sets
            character.FillWeaponSetsIfNeeded(character.EquipWeaponSet);
            // Right hand & left hand items
            BaseItem rightHandEquipItem = playerCharacter.RightHandEquipItem;
            BaseItem leftHandEquipItem = playerCharacter.LeftHandEquipItem;
            EquipWeapons equipWeapons = new EquipWeapons();
            // Right hand equipped item
            if (rightHandEquipItem != null)
            {
                CharacterItem newItem = CharacterItem.Create(rightHandEquipItem);
                equipWeapons.rightHand = newItem;
            }
            // Left hand equipped item
            if (leftHandEquipItem != null)
            {
                CharacterItem newItem = CharacterItem.Create(leftHandEquipItem);
                equipWeapons.leftHand = newItem;
            }
            character.EquipWeapons = equipWeapons;
            // Armors
            BaseItem[] armorItems = playerCharacter.ArmorItems;
            foreach (BaseItem armorItem in armorItems)
            {
                if (armorItem == null)
                    continue;
                CharacterItem newItem = CharacterItem.Create(armorItem);
                character.EquipItems.Add(newItem);
            }
            // Start items
            List<ItemAmount> allStartItems = new List<ItemAmount>();
            allStartItems.AddRange(startItems);
            allStartItems.AddRange(playerCharacter.StartItems);
            foreach (ItemAmount startItem in allStartItems)
            {
                if (startItem.item == null || startItem.amount <= 0)
                    continue;
                int amount = startItem.amount;
                while (amount > 0)
                {
                    int addAmount = amount;
                    if (addAmount > startItem.item.MaxStack)
                        addAmount = startItem.item.MaxStack;
                    if (!character.IncreasingItemsWillOverwhelming(startItem.item.DataId, addAmount))
                        character.AddOrSetNonEquipItems(CharacterItem.Create(startItem.item, 1, addAmount));
                    amount -= addAmount;
                }
            }
            character.FillEmptySlots();
            // Set start stats
            CharacterStats stats = character.GetCaches().Stats;
            character.CurrentHp = (int)stats.hp;
            character.CurrentMp = (int)stats.mp;
            character.CurrentStamina = (int)stats.stamina;
            character.CurrentFood = (int)stats.food;
            character.CurrentWater = (int)stats.water;
            character.Gold = startGold;
            character.FactionId = factionId;
            // Start Map
            playerCharacter.GetStartMapAndTransform(character, out BaseMapInfo startMap, out Vector3 startPosition, out Vector3 startRotation);
            character.CurrentMapName = startMap.Id;
            character.CurrentPosition = startPosition;
            character.CurrentRotation = startRotation;
#if !DISABLE_DIFFER_MAP_RESPAWNING
            character.RespawnMapName = startMap.Id;
            character.RespawnPosition = startPosition;
#endif
            DevExtUtils.InvokeStaticDevExtMethods(ClassType, "SetNewCharacterData", character, characterName, dataId, entityId);
            return character;
        }

        public static void AddAllCharacterRelatesDataSurrogate(this SurrogateSelector surrogateSelector)
        {
            PlayerCharacterSerializationSurrogate playerCharacterDataSS = new PlayerCharacterSerializationSurrogate();
            CharacterAttributeSerializationSurrogate attributeSS = new CharacterAttributeSerializationSurrogate();
            CharacterBuffSerializationSurrogate buffSS = new CharacterBuffSerializationSurrogate();
            CharacterHotkeySerializationSurrogate hotkeySS = new CharacterHotkeySerializationSurrogate();
            CharacterItemSerializationSurrogate itemSS = new CharacterItemSerializationSurrogate();
            CharacterQuestSerializationSurrogate questSS = new CharacterQuestSerializationSurrogate();
            CharacterCurrencySerializationSurrogate currencySS = new CharacterCurrencySerializationSurrogate();
            CharacterSkillSerializationSurrogate skillSS = new CharacterSkillSerializationSurrogate();
            CharacterSkillUsageSerializationSurrogate skillUsageSS = new CharacterSkillUsageSerializationSurrogate();
            CharacterSummonSerializationSurrogate summonSS = new CharacterSummonSerializationSurrogate();
            CharacterMountSerializationSurrogate mountSS = new CharacterMountSerializationSurrogate();
            surrogateSelector.AddSurrogate(typeof(PlayerCharacterData), new StreamingContext(StreamingContextStates.All), playerCharacterDataSS);
            surrogateSelector.AddSurrogate(typeof(CharacterAttribute), new StreamingContext(StreamingContextStates.All), attributeSS);
            surrogateSelector.AddSurrogate(typeof(CharacterBuff), new StreamingContext(StreamingContextStates.All), buffSS);
            surrogateSelector.AddSurrogate(typeof(CharacterHotkey), new StreamingContext(StreamingContextStates.All), hotkeySS);
            surrogateSelector.AddSurrogate(typeof(CharacterItem), new StreamingContext(StreamingContextStates.All), itemSS);
            surrogateSelector.AddSurrogate(typeof(CharacterQuest), new StreamingContext(StreamingContextStates.All), questSS);
            surrogateSelector.AddSurrogate(typeof(CharacterCurrency), new StreamingContext(StreamingContextStates.All), currencySS);
            surrogateSelector.AddSurrogate(typeof(CharacterSkill), new StreamingContext(StreamingContextStates.All), skillSS);
            surrogateSelector.AddSurrogate(typeof(CharacterSkillUsage), new StreamingContext(StreamingContextStates.All), skillUsageSS);
            surrogateSelector.AddSurrogate(typeof(CharacterSummon), new StreamingContext(StreamingContextStates.All), summonSS);
            surrogateSelector.AddSurrogate(typeof(CharacterMount), new StreamingContext(StreamingContextStates.All), mountSS);
            DevExtUtils.InvokeStaticDevExtMethods(ClassType, "AddAllCharacterRelatesDataSurrogate", surrogateSelector);
        }

        public static void SavePersistentCharacterData<T>(this T characterData) where T : IPlayerCharacterData
        {
            PlayerCharacterData savingData = new PlayerCharacterData();
            characterData.CloneTo(savingData);
            if (string.IsNullOrEmpty(savingData.Id))
                return;
            savingData.LastUpdate = System.DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            SurrogateSelector surrogateSelector = new SurrogateSelector();
            surrogateSelector.AddAllUnitySurrogate();
            surrogateSelector.AddAllCharacterRelatesDataSurrogate();
            binaryFormatter.SurrogateSelector = surrogateSelector;
            binaryFormatter.Binder = new PlayerCharacterDataTypeBinder();
            string path = Application.persistentDataPath + "/" + savingData.Id + ".sav";
            Debug.Log("Character Saving to: " + path);
            FileStream file = File.Open(path, FileMode.OpenOrCreate);
            binaryFormatter.Serialize(file, savingData);
            file.Close();
            Debug.Log("Character Saved to: " + path);
        }

        public static T LoadPersistentCharacterDataById<T>(this T characterData, string id) where T : IPlayerCharacterData
        {
            return LoadPersistentCharacterData(characterData, Application.persistentDataPath + "/" + id + ".sav");
        }

        public static T LoadPersistentCharacterData<T>(this T characterData, string path) where T : IPlayerCharacterData
        {
            if (File.Exists(path))
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                SurrogateSelector surrogateSelector = new SurrogateSelector();
                surrogateSelector.AddAllUnitySurrogate();
                surrogateSelector.AddAllCharacterRelatesDataSurrogate();
                binaryFormatter.SurrogateSelector = surrogateSelector;
                binaryFormatter.Binder = new PlayerCharacterDataTypeBinder();
                FileStream file = File.Open(path, FileMode.Open);
                PlayerCharacterData loadedData = (PlayerCharacterData)binaryFormatter.Deserialize(file);
                file.Close();
                loadedData.CloneTo(characterData);
            }
            return characterData;
        }

        public static List<PlayerCharacterData> LoadAllPersistentCharacterData()
        {
            List<PlayerCharacterData> result = new List<PlayerCharacterData>();
            string path = Application.persistentDataPath;
            string[] files = Directory.GetFiles(path, "*.sav");
            Debug.Log("Characters loading from: " + path);
            PlayerCharacterData characterData;
            foreach (string file in files)
            {
                // If filename is empty or this is not character save, skip it
                if (file.Length <= 4 || file.Contains("_world_") || file.Contains("_storage") || file.Contains("_summon_buffs"))
                    continue;
                characterData = new PlayerCharacterData();
                result.Add(characterData.LoadPersistentCharacterData(file));
            }
            Debug.Log("Characters loaded from: " + path);
            return result;
        }

        public static void DeleteAllPersistentCharacterData()
        {
            string path = Application.persistentDataPath;
            string[] files = Directory.GetFiles(path, "*.sav");
            foreach (string file in files)
            {
                File.Delete(file);
            }
        }

        public static void DeletePersistentCharacterData(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                Debug.LogWarning("Cannot delete character: character id is empty");
                return;
            }
            File.Delete(Application.persistentDataPath + "/" + id + ".sav");
        }

        public static void DeletePersistentCharacterData<T>(this T characterData) where T : IPlayerCharacterData
        {
            if (characterData == null)
            {
                Debug.LogWarning("Cannot delete character: character data is empty");
                return;
            }
            DeletePersistentCharacterData(characterData.Id);
        }

        public static bool AddAttribute(this IPlayerCharacterData characterData, out UITextKeys gameMessage, int dataId, int amount = 1, int itemIndex = -1)
        {
            if (!GameInstance.Attributes.TryGetValue(dataId, out Attribute attribute))
            {
                gameMessage = UITextKeys.UI_ERROR_INVALID_ATTRIBUTE_DATA;
                return false;
            }

            CharacterAttribute characterAtttribute;
            int index = characterData.IndexOfAttribute(dataId);
            if (index < 0)
            {
                characterAtttribute = CharacterAttribute.Create(attribute, 0);
                if (!attribute.CanIncreaseAmount(characterData, characterAtttribute.amount + amount - 1, out gameMessage, itemIndex < 0))
                    return false;
                if (itemIndex >= 0)
                {
                    if (characterData.DecreaseItemsByIndex(itemIndex, 1, false))
                        characterData.FillEmptySlots();
                    else
                        return false;
                }
                characterAtttribute.amount += amount;
                characterData.Attributes.Add(characterAtttribute);
            }
            else
            {
                characterAtttribute = characterData.Attributes[index];
                if (!attribute.CanIncreaseAmount(characterData, characterAtttribute.amount + amount - 1, out gameMessage, itemIndex < 0))
                    return false;
                if (itemIndex >= 0)
                {
                    if (characterData.DecreaseItemsByIndex(itemIndex, 1, false))
                        characterData.FillEmptySlots();
                    else
                        return false;
                }
                characterAtttribute.amount += amount;
                characterData.Attributes[index] = characterAtttribute;
            }
            GameInstance.ServerLogHandlers.LogAddAttribute(characterData, attribute, amount, characterAtttribute);
            return true;
        }

        public static bool ResetAttributes(this IPlayerCharacterData characterData, int itemIndex = -1)
        {
            if (itemIndex >= 0)
            {
                if (characterData.DecreaseItemsByIndex(itemIndex, 1, false))
                    characterData.FillEmptySlots();
                else
                    return false;
            }

            int countStatPoint = 0;
            Attribute attribute;
            CharacterAttribute characterAttribute;
            for (int i = characterData.Attributes.Count - 1; i >= 0; --i)
            {
                characterAttribute = characterData.Attributes[i];
                attribute = characterAttribute.GetAttribute();
                if (attribute.CannotReset)
                    continue;
                countStatPoint += characterAttribute.amount;
                characterData.Attributes.RemoveAt(i);
            }
            characterData.StatPoint += countStatPoint;
            GameInstance.ServerLogHandlers.LogResetAttributes(characterData);
            return true;
        }

        public static bool AddSkill(this IPlayerCharacterData characterData, out UITextKeys gameMessageType, int dataId, int level = 1, int itemIndex = -1)
        {
            if (!GameInstance.Skills.TryGetValue(dataId, out BaseSkill skill))
            {
                gameMessageType = UITextKeys.UI_ERROR_INVALID_SKILL_DATA;
                return false;
            }

            CharacterSkill characterSkill;
            int index = characterData.IndexOfSkill(dataId);
            if (index < 0)
            {
                characterSkill = CharacterSkill.Create(skill, 0);
                if (!skill.CanLevelUp(characterData, (characterSkill.level + level - 1), out gameMessageType, itemIndex < 0, itemIndex < 0))
                    return false;
                if (itemIndex >= 0)
                {
                    if (characterData.DecreaseItemsByIndex(itemIndex, 1, false))
                        characterData.FillEmptySlots();
                    else
                        return false;
                }
                characterSkill.level += level;
                characterData.Skills.Add(characterSkill);
            }
            else
            {
                characterSkill = characterData.Skills[index];
                if (!skill.CanLevelUp(characterData, (characterSkill.level + level - 1), out gameMessageType, itemIndex < 0, itemIndex < 0))
                    return false;
                if (itemIndex >= 0)
                {
                    if (characterData.DecreaseItemsByIndex(itemIndex, 1, false))
                        characterData.FillEmptySlots();
                    else
                        return false;
                }
                characterSkill.level += level;
                characterData.Skills[index] = characterSkill;
            }
            GameInstance.ServerLogHandlers.LogAddSkill(characterData, skill, level, characterSkill);
            return true;
        }

        public static bool ResetSkills(this IPlayerCharacterData characterData, int itemIndex = -1)
        {
            if (itemIndex >= 0)
            {
                if (characterData.DecreaseItemsByIndex(itemIndex, 1, false))
                    characterData.FillEmptySlots();
                else
                    return false;
            }

            int countSkillPoint = 0;
            BaseSkill skill;
            CharacterSkill characterSkill;
            for (int i = characterData.Skills.Count - 1; i >= 0; --i)
            {
                characterSkill = characterData.Skills[i];
                skill = characterSkill.GetSkill();
                if (skill.cannotReset)
                    continue;
                for (int j = 0; j < characterSkill.level; ++j)
                {
                    countSkillPoint += Mathf.CeilToInt(skill.GetRequireCharacterSkillPoint(j));
                }
                characterData.Skills.RemoveAt(i);
            }
            characterData.SkillPoint += countSkillPoint;
            GameInstance.ServerLogHandlers.LogResetSkills(characterData);
            return true;
        }

        public static Dictionary<Currency, int> GetCurrencies(this IPlayerCharacterData data)
        {
            if (data == null)
                return new Dictionary<Currency, int>();
            Dictionary<Currency, int> result = new Dictionary<Currency, int>();
#if !DISABLE_CUSTOM_CHARACTER_CURRENCIES
            foreach (CharacterCurrency characterCurrency in data.Currencies)
            {
                Currency key = characterCurrency.GetCurrency();
                int value = characterCurrency.amount;
                if (key == null)
                    continue;
                if (!result.ContainsKey(key))
                    result[key] = value;
                else
                    result[key] += value;
            }
#endif
            return result;
        }

        public static Dictionary<int, int> GetCurrenciesByDataId(this IPlayerCharacterData data)
        {
            if (data == null)
                return new Dictionary<int, int>();
            Dictionary<int, int> result = new Dictionary<int, int>();
#if !DISABLE_CUSTOM_CHARACTER_CURRENCIES
            foreach (CharacterCurrency characterCurrency in data.Currencies)
            {
                Currency key = characterCurrency.GetCurrency();
                int value = characterCurrency.amount;
                if (key == null)
                    continue;
                if (!result.ContainsKey(key.DataId))
                    result[key.DataId] = value;
                else
                    result[key.DataId] += value;
            }
#endif
            return result;
        }

        public static void IncreaseCurrencies(this IPlayerCharacterData character, Dictionary<Currency, int> currencyAmounts, float multiplier = 1)
        {
            if (currencyAmounts == null)
                return;
            foreach (KeyValuePair<Currency, int> currencyAmount in currencyAmounts)
            {
                character.IncreaseCurrency(currencyAmount.Key, Mathf.CeilToInt(currencyAmount.Value * multiplier));
            }
        }

        public static void IncreaseCurrencies(this IPlayerCharacterData character, IEnumerable<CurrencyAmount> currencyAmounts, float multiplier = 1)
        {
            if (currencyAmounts == null)
                return;
            foreach (CurrencyAmount currencyAmount in currencyAmounts)
            {
                character.IncreaseCurrency(currencyAmount.currency, Mathf.CeilToInt(currencyAmount.amount * multiplier));
            }
        }

        public static void IncreaseCurrencies(this IPlayerCharacterData character, IEnumerable<CharacterCurrency> currencies, float multiplier = 1)
        {
            if (currencies == null)
                return;
            foreach (CharacterCurrency currency in currencies)
            {
                character.IncreaseCurrency(currency.GetCurrency(), Mathf.CeilToInt(currency.amount * multiplier));
            }
        }

        public static void IncreaseCurrency(this IPlayerCharacterData character, Currency currency, int amount)
        {
#if !DISABLE_CUSTOM_CHARACTER_CURRENCIES
            if (currency == null) return;
            int indexOfCurrency = character.IndexOfCurrency(currency.DataId);
            if (indexOfCurrency >= 0)
            {
                CharacterCurrency characterCurrency = character.Currencies[indexOfCurrency];
                characterCurrency.amount = characterCurrency.amount.Increase(amount);
                character.Currencies[indexOfCurrency] = characterCurrency;
            }
            else
            {
                character.Currencies.Add(CharacterCurrency.Create(currency, amount));
            }
#endif
        }

        public static void DecreaseCurrencies(this IPlayerCharacterData character, Dictionary<Currency, int> currencyAmounts, float multiplier = 1)
        {
            if (currencyAmounts == null)
                return;
            foreach (KeyValuePair<Currency, int> currencyAmount in currencyAmounts)
            {
                character.DecreaseCurrency(currencyAmount.Key, Mathf.CeilToInt(currencyAmount.Value * multiplier));
            }
        }

        public static void DecreaseCurrencies(this IPlayerCharacterData character, IEnumerable<CurrencyAmount> currencyAmounts, float multiplier = 1)
        {
            if (currencyAmounts == null)
                return;
            foreach (CurrencyAmount currencyAmount in currencyAmounts)
            {
                character.DecreaseCurrency(currencyAmount.currency, Mathf.CeilToInt(currencyAmount.amount * multiplier));
            }
        }

        public static void DecreaseCurrencies(this IPlayerCharacterData character, IEnumerable<CharacterCurrency> currencies, float multiplier = 1)
        {
            if (currencies == null)
                return;
            foreach (CharacterCurrency currency in currencies)
            {
                character.DecreaseCurrency(currency.GetCurrency(), Mathf.CeilToInt(currency.amount * multiplier));
            }
        }

        public static void DecreaseCurrency(this IPlayerCharacterData character, Currency currency, int amount)
        {
#if !DISABLE_CUSTOM_CHARACTER_CURRENCIES
            if (currency == null) return;
            if (amount < 0) return;
            int indexOfCurrency = character.IndexOfCurrency(currency.DataId);
            if (indexOfCurrency >= 0)
            {
                CharacterCurrency characterCurrency = character.Currencies[indexOfCurrency];
                characterCurrency.amount -= amount;
                character.Currencies[indexOfCurrency] = characterCurrency;
            }
            else
            {
                character.Currencies.Add(CharacterCurrency.Create(currency, -amount));
            }
#endif
        }

        public static bool HasEnoughCurrencyAmounts(this IPlayerCharacterData data, Dictionary<Currency, int> requiredCurrencyAmounts, out UITextKeys gameMessage, out Dictionary<Currency, int> currentCurrencyAmounts, float multiplier = 1)
        {
            gameMessage = UITextKeys.NONE;
            currentCurrencyAmounts = data.GetCurrencies();
            foreach (Currency requireCurrency in requiredCurrencyAmounts.Keys)
            {
                int requiredCurrency = Mathf.CeilToInt(requiredCurrencyAmounts[requireCurrency] * multiplier);
                if (requiredCurrency < 0 || !currentCurrencyAmounts.ContainsKey(requireCurrency) ||
                    currentCurrencyAmounts[requireCurrency] < requiredCurrency)
                {
                    gameMessage = UITextKeys.UI_ERROR_NOT_ENOUGH_CURRENCY_AMOUNTS;
                    return false;
                }
            }
            return true;
        }

        public static void ClearParty(this IPlayerCharacterData character)
        {
            character.PartyId = 0;
        }

        public static void ClearGuild(this IPlayerCharacterData character)
        {
            character.GuildId = 0;
            character.GuildRole = 0;
        }

        public static bool IsMuting(this IPlayerCharacterData character)
        {
            return character.UnmuteTime > 0 && character.UnmuteTime > (BaseGameNetworkManager.Singleton.ServerTimestamp / 1000);
        }

        public static bool TryGetEntityPrefab(this IPlayerCharacterData data, out BasePlayerCharacterEntity prefab, out int metaDataId)
        {
            int hashAssetId = GameInstance.GetPlayerCharacterEntityHashAssetId(data.EntityId, out metaDataId);
            if (metaDataId != 0)
            {
                if (GameInstance.PlayerCharacterEntityMetaDataList.TryGetValue(metaDataId, out PlayerCharacterEntityMetaData metaData) && metaData.EntityPrefab != null)
                {
                    prefab = metaData.EntityPrefab;
                    return prefab != null;
                }
            }
#if !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
            return GameInstance.PlayerCharacterEntities.TryGetValue(hashAssetId, out prefab);
#else
            prefab = null;
            return false;
#endif
        }

#if !DISABLE_ADDRESSABLES
        public static bool TryGetEntityAddressablePrefab(this IPlayerCharacterData data, out AssetReferenceLiteNetLibBehaviour<BasePlayerCharacterEntity> assetRef, out int metaDataId)
        {
            int hashAssetId = GameInstance.GetPlayerCharacterEntityHashAssetId(data.EntityId, out metaDataId);
            if (metaDataId != 0)
            {
                if (GameInstance.PlayerCharacterEntityMetaDataList.TryGetValue(metaDataId, out PlayerCharacterEntityMetaData metaData) && metaData.AddressableEntityPrefab.IsDataValid())
                {
                    assetRef = metaData.AddressableEntityPrefab;
                    return assetRef.IsDataValid();
                }
            }
            return GameInstance.AddressablePlayerCharacterEntities.TryGetValue(hashAssetId, out assetRef);
        }
#endif

        public static BaseCharacterModel InstantiateModel(this IPlayerCharacterData data, Transform parent)
        {
            BaseCharacterEntity result;
            int metaDataId;
#if !DISABLE_ADDRESSABLES
            if (data.TryGetEntityAddressablePrefab(out var assetRef, out metaDataId))
            {
                AsyncOperationHandle<GameObject> handler = Addressables.LoadAssetAsync<GameObject>(assetRef.RuntimeKey);
                handler.WaitForCompletion();
                result = Object.Instantiate(handler.Result).GetComponent<BaseCharacterEntity>();
                result.gameObject.AddComponent<AssetReferenceReleaser>();
                Addressables.Release(handler);
            }
#else
            if (false) { }
#endif
            else if (data.TryGetEntityPrefab(out var prefab, out metaDataId))
            {
                result = Object.Instantiate(prefab);
            }
            else
            {
                Logging.LogWarning($"[InstantiateModel] Cannot find character entity with id: {data.EntityId}");
                return null;
            }
            GameInstance.SetupByMetaData(result as BasePlayerCharacterEntity, metaDataId);
            LiteNetLibBehaviour[] networkBehaviours = result.GetComponentsInChildren<LiteNetLibBehaviour>();
            foreach (LiteNetLibBehaviour networkBehaviour in networkBehaviours)
            {
                networkBehaviour.enabled = false;
            }
            IEntityMovementComponent movementComponent = result.GetComponent<IEntityMovementComponent>();
            if (!movementComponent.IsNull())
                movementComponent.enabled = false;
            GameObject[] ownerObjects = result.OwnerObjects;
            foreach (GameObject ownerObject in ownerObjects)
            {
                ownerObject.SetActive(false);
            }
            GameObject[] nonOwnerObjects = result.NonOwnerObjects;
            foreach (GameObject nonOwnerObject in nonOwnerObjects)
            {
                nonOwnerObject.SetActive(false);
            }
            result.gameObject.SetLayerRecursively(GameInstance.Singleton.playerLayer, true);
            result.gameObject.SetActive(true);
            result.transform.SetParent(parent);
            result.transform.localPosition = Vector3.zero;
            result.transform.localEulerAngles = Vector3.zero;
            result.CharacterModel.IKsDisablers.Add(BaseCharacterModel.DefaultIKDisabler);
            return result.CharacterModel;
        }

        public static void SetupModelBodyParts(this BaseCharacterModel characterModel, IPlayerCharacterData data)
        {
#if !DISABLE_CUSTOM_CHARACTER_DATA
            PlayerCharacterBodyPartComponent[] comps = characterModel.GetComponentsInChildren<PlayerCharacterBodyPartComponent>();
            for (int i = 0; i < comps.Length; ++i)
            {
                comps[i].SetupCharacterModelEvents(characterModel);
                comps[i].ApplyModelAndColorBySavedData(data.PublicInts);
            }
#endif
        }
    }
}
