using Cysharp.Text;
using LiteNetLibManager;
using System.Collections.Generic;
using System.Globalization;

namespace MultiplayerARPG
{
    public static partial class CharacterDataExtensions
    {
        public static System.Type ClassType { get; private set; }

        static CharacterDataExtensions()
        {
            ClassType = typeof(CharacterDataExtensions);
        }

        public static int IndexOfAttribute(this ICharacterData data, int dataId)
        {
            return data.Attributes.IndexOf(dataId);
        }

        public static int IndexOfBuff(this ICharacterData data, BuffType type, int dataId)
        {
            return data.Buffs.IndexOf(type, dataId);
        }

        public static int IndexOfBuff(this ICharacterData data, string id)
        {
            return data.Buffs.IndexOf(id);
        }

        public static List<int> IndexesOfBuff(this ICharacterData data, BuffType type, int dataId)
        {
            return data.Buffs.IndexesOf(type, dataId);
        }

        public static int IndexOfEquipItem(this ICharacterData data, int dataId)
        {
            return data.EquipItems.IndexOf(dataId);
        }

        public static int IndexOfEquipItem(this ICharacterData data, string id)
        {
            return data.EquipItems.IndexOf(id);
        }

        public static int IndexOfNonEquipItem(this ICharacterData data, int dataId)
        {
            return data.NonEquipItems.IndexOf(dataId);
        }

        public static int IndexOfNonEquipItem(this ICharacterData data, string id)
        {
            return data.NonEquipItems.IndexOf(id);
        }

        public static int IndexOfSkill(this ICharacterData data, int dataId)
        {
            return data.Skills.IndexOf(dataId);
        }

        public static int IndexOfSkillUsage(this ICharacterData data, SkillUsageType type, int dataId)
        {
            return data.SkillUsages.IndexOf(type, dataId);
        }

        public static int IndexOfSummon(this ICharacterData data, uint objectId)
        {
            return data.Summons.IndexOf(objectId);
        }

        public static int IndexOfSummon(this ICharacterData data, SummonType type)
        {
            return data.Summons.IndexOf(type);
        }

        public static int IndexOfSummon(this ICharacterData data, SummonType type, int dataId)
        {
            return data.Summons.IndexOf(type, dataId);
        }

        public static int IndexOfGuildSkill(this IPlayerCharacterData data, int dataId)
        {
            return data.GuildSkills.IndexOf(dataId);
        }

        public static void FillWeaponSetsIfNeeded(this ICharacterData data, byte equipWeaponSet)
        {
#if UNITY_2017_1_OR_NEWER
            if (data is IGameEntity gameEntity && gameEntity.Entity.Manager != null && !gameEntity.Entity.IsServer)
            {
                Logging.LogWarning("[FillWeaponSetsIfNeeded] Client can't fill weapon sets");
                return;
            }
#endif
            while (data.SelectableWeaponSets.Count <= equipWeaponSet)
            {
                data.SelectableWeaponSets.Add(new EquipWeapons());
            }
        }

        public static List<int> ReadCharacterItemSockets(this string socketsString, char separator = ';')
        {
            List<int> sockets = new List<int>();
            string[] splitTexts = socketsString.Split(separator);
            foreach (string text in splitTexts)
            {
                if (string.IsNullOrEmpty(text))
                    continue;
                sockets.Add(int.Parse(text));
            }
            return sockets;
        }

        public static string WriteCharacterItemSockets(this IList<int> sockets, char separator = ';')
        {
            if (sockets == null || sockets.Count == 0)
                return string.Empty;

            using (Utf16ValueStringBuilder stringBuilder = ZString.CreateStringBuilder(true))
            {
                foreach (int socket in sockets)
                {
                    stringBuilder.Append(socket);
                    stringBuilder.Append(separator);
                }
                return stringBuilder.ToString();
            }
        }

        public static List<CharacterCurrency> ReadCurrencies(this string currenciesString)
        {
            List<CharacterCurrency> currencies = new List<CharacterCurrency>();
            string[] splitSets = currenciesString.Split(';');
            foreach (string set in splitSets)
            {
                if (string.IsNullOrEmpty(set))
                    continue;

                string[] splitData = set.Split(':');
                if (splitData.Length != 2)
                    continue;

                if (!int.TryParse(splitData[0], out int dataId))
                    dataId = splitData[0].GenerateHashId();

                if (!int.TryParse(splitData[1], out int amount))
                    amount = 0;

                CharacterCurrency characterCurrency = new CharacterCurrency();
                characterCurrency.dataId = dataId;
                characterCurrency.amount = amount;
                currencies.Add(characterCurrency);
            }
            return currencies;
        }

        public static string WriteCurrencies(this List<CharacterCurrency> currencies)
        {
            if (currencies == null || currencies.Count == 0)
                return string.Empty;

            using (Utf16ValueStringBuilder stringBuilder = ZString.CreateStringBuilder(true))
            {
                foreach (CharacterCurrency currency in currencies)
                {
                    stringBuilder.Append(currency.dataId);
                    stringBuilder.Append(currency.amount);
                }
                return stringBuilder.ToString();
            }
        }

        public static List<CharacterItem> ReadItems(this string itemsString)
        {
            List<CharacterItem> items = new List<CharacterItem>();
            string[] splitSets = itemsString.Split(';');
            foreach (string set in splitSets)
            {
                if (string.IsNullOrEmpty(set))
                    continue;

                string[] splitData = set.Split(':');
                if (splitData.Length < 1)
                    continue;

                if (!int.TryParse(splitData[0], out int dataId))
                    dataId = splitData[0].GenerateHashId();

                int amount;
                if (splitData.Length < 2 || !int.TryParse(splitData[1], out amount))
                    amount = 1;

                int level;
                if (splitData.Length < 3 || !int.TryParse(splitData[2], out level))
                    level = 1;

                float durability;
                if (splitData.Length < 4 || !float.TryParse(splitData[3].Replace(',', '.'), out durability))
                    durability = 0f;

                int exp;
                if (splitData.Length < 5 || !int.TryParse(splitData[4], out exp))
                    exp = 0;

                float lockRemainsDuration;
                if (splitData.Length < 6 || !float.TryParse(splitData[5].Replace(',', '.'), out lockRemainsDuration))
                    lockRemainsDuration = 0f;

                long expireTime;
                if (splitData.Length < 7 || !long.TryParse(splitData[6], out expireTime))
                    expireTime = 0;

                int randomSeed;
                if (splitData.Length < 8 || !int.TryParse(splitData[7], out randomSeed))
                    randomSeed = 0;

                string sockets;
                if (splitData.Length < 9)
                    sockets = "";
                else
                    sockets = splitData[8];

                int ammoDataId;
                if (splitData.Length < 10 || !int.TryParse(splitData[9], out ammoDataId))
                    ammoDataId = 0;

                int ammo;
                if (splitData.Length < 11 || !int.TryParse(splitData[10], out ammo))
                    ammo = 0;

                byte version;
                if (splitData.Length < 12 || !byte.TryParse(splitData[11], out version))
                    version = 0;

                CharacterItem characterItem = new CharacterItem();
                characterItem.id = GenericUtils.GetUniqueId();
                characterItem.dataId = dataId;
                characterItem.level = level;
                characterItem.amount = amount;
                characterItem.durability = durability;
                characterItem.exp = exp;
                characterItem.lockRemainsDuration = lockRemainsDuration;
                characterItem.expireTime = expireTime;
                characterItem.randomSeed = randomSeed;
                characterItem.ammoDataId = ammoDataId;
                characterItem.ammo = ammo;
                characterItem.version = version;
                characterItem.ReadSockets(sockets, '|');
                items.Add(characterItem);
            }
            return items;
        }

        public static string WriteItems(this List<CharacterItem> items)
        {
            if (items == null || items.Count == 0)
                return string.Empty;

            using (Utf16ValueStringBuilder stringBuilder = ZString.CreateStringBuilder(false))
            {
                foreach (CharacterItem item in items)
                {
                    if (item.IsEmptySlot()) continue;
                    stringBuilder.Append(item.dataId);
                    stringBuilder.Append(':');
                    stringBuilder.Append(item.amount);
                    stringBuilder.Append(':');
                    stringBuilder.Append(item.level);
                    stringBuilder.Append(':');
                    stringBuilder.Append(item.durability.ToString("N2", CultureInfo.InvariantCulture.NumberFormat));
                    stringBuilder.Append(':');
                    stringBuilder.Append(item.exp);
                    stringBuilder.Append(':');
                    stringBuilder.Append(item.lockRemainsDuration.ToString("N2", CultureInfo.InvariantCulture.NumberFormat));
                    stringBuilder.Append(':');
                    stringBuilder.Append(item.expireTime);
                    stringBuilder.Append(':');
                    stringBuilder.Append(item.randomSeed);
                    stringBuilder.Append(':');
                    stringBuilder.Append(item.WriteSockets('|'));
                    stringBuilder.Append(':');
                    stringBuilder.Append(item.ammoDataId);
                    stringBuilder.Append(':');
                    stringBuilder.Append(item.ammo);
                    stringBuilder.Append(':');
                    stringBuilder.Append(item.version);
                    stringBuilder.Append(';');
                }
                return stringBuilder.ToString();
            }
        }
    }
}
