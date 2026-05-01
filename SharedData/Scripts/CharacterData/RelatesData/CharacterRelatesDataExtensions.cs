using System.Collections.Generic;

namespace MultiplayerARPG
{
    public static partial class CharacterRelatesDataExtensions
    {
        #region List Clone Functions
        public static T Clone<T>(this IList<EquipWeapons> src, bool generateNewId = false) where T : IList<EquipWeapons>, new()
        {
            T result = new T();
            if (src != null)
            {
                for (int i = 0; i < src.Count; ++i)
                {
                    result.Add(new EquipWeapons()
                    {
                        rightHand = src[i].rightHand.Clone(generateNewId),
                        leftHand = src[i].leftHand.Clone(generateNewId),
                    });
                }
            }
            return result;
        }

        public static List<EquipWeapons> Clone(this IList<EquipWeapons> src, bool generateNewId = false)
        {
            return src.Clone<List<EquipWeapons>>(generateNewId);
        }

        public static T Clone<T>(this IList<CharacterAttribute> src) where T : IList<CharacterAttribute>, new()
        {
            T result = new T();
            if (src != null)
            {
                for (int i = 0; i < src.Count; ++i)
                {
                    result.Add(src[i].Clone());
                }
            }
            return result;
        }

        public static List<CharacterAttribute> Clone(this IList<CharacterAttribute> src)
        {
            return src.Clone<List<CharacterAttribute>>();
        }

        public static T Clone<T>(this IList<CharacterBuff> src, bool generateNewId = false) where T : IList<CharacterBuff>, new()
        {
            T result = new T();
            if (src != null)
            {
                for (int i = 0; i < src.Count; ++i)
                {
                    result.Add(src[i].Clone(generateNewId));
                }
            }
            return result;
        }

        public static List<CharacterBuff> Clone(this IList<CharacterBuff> src, bool generateNewId = false)
        {
            return src.Clone<List<CharacterBuff>>(generateNewId);
        }

        public static T Clone<T>(this IList<CharacterItem> src, bool generateNewId = false) where T : List<CharacterItem>, new()
        {
            T result = new T();
            if (src != null)
            {
                for (int i = 0; i < src.Count; ++i)
                {
                    result.Add(src[i].Clone(generateNewId));
                }
            }
            return result;
        }

        public static List<CharacterItem> Clone(this IList<CharacterItem> src, bool generateNewId = false)
        {
            return src.Clone<List<CharacterItem>>(generateNewId);
        }

        public static T Clone<T>(this IList<CharacterSkill> src) where T : IList<CharacterSkill>, new()
        {
            T result = new T();
            if (src != null)
            {
                for (int i = 0; i < src.Count; ++i)
                {
                    result.Add(src[i].Clone());
                }
            }
            return result;
        }

        public static List<CharacterSkill> Clone(this IList<CharacterSkill> src)
        {
            return src.Clone<List<CharacterSkill>>();
        }

        public static T Clone<T>(this IList<CharacterSkillUsage> src) where T : IList<CharacterSkillUsage>, new()
        {
            T result = new T();
            if (src != null)
            {
                for (int i = 0; i < src.Count; ++i)
                {
                    result.Add(src[i].Clone());
                }
            }
            return result;
        }

        public static List<CharacterSkillUsage> Clone(this IList<CharacterSkillUsage> src)
        {
            return src.Clone<List<CharacterSkillUsage>>();
        }

        public static T Clone<T>(this IList<CharacterSummon> src, bool generateNewId = false) where T : IList<CharacterSummon>, new()
        {
            T result = new T();
            if (src != null)
            {
                for (int i = 0; i < src.Count; ++i)
                {
                    result.Add(src[i].Clone(generateNewId));
                }
            }
            return result;
        }

        public static List<CharacterSummon> Clone(this IList<CharacterSummon> src, bool generateNewId = false)
        {
            return src.Clone<List<CharacterSummon>>(generateNewId);
        }

        public static T Clone<T>(this IList<CharacterHotkey> src) where T : IList<CharacterHotkey>, new()
        {
            T result = new T();
            if (src != null)
            {
                for (int i = 0; i < src.Count; ++i)
                {
                    result.Add(src[i].Clone());
                }
            }
            return result;
        }

        public static List<CharacterHotkey> Clone(this IList<CharacterHotkey> src)
        {
            return src.Clone<List<CharacterHotkey>>();
        }

        public static T Clone<T>(this IList<CharacterQuest> src) where T : IList<CharacterQuest>, new()
        {
            T result = new T();
            if (src != null)
            {
                for (int i = 0; i < src.Count; ++i)
                {
                    result.Add(src[i].Clone());
                }
            }
            return result;
        }

        public static List<CharacterQuest> Clone(this IList<CharacterQuest> src)
        {
            return src.Clone<List<CharacterQuest>>();
        }

        public static T Clone<T>(this IList<CharacterCurrency> src) where T : IList<CharacterCurrency>, new()
        {
            T result = new T();
            if (src != null)
            {
                for (int i = 0; i < src.Count; ++i)
                {
                    result.Add(src[i].Clone());
                }
            }
            return result;
        }

        public static List<CharacterCurrency> Clone(this IList<CharacterCurrency> src)
        {
            return src.Clone<List<CharacterCurrency>>();
        }

        public static T Clone<T>(this IList<CharacterDataBoolean> src) where T : IList<CharacterDataBoolean>, new()
        {
            T result = new T();
            if (src != null)
            {
                for (int i = 0; i < src.Count; ++i)
                {
                    result.Add(src[i].Clone());
                }
            }
            return result;
        }

        public static List<CharacterDataBoolean> Clone(this IList<CharacterDataBoolean> src)
        {
            return src.Clone<List<CharacterDataBoolean>>();
        }

        public static T Clone<T>(this IList<CharacterDataInt32> src) where T : IList<CharacterDataInt32>, new()
        {
            T result = new T();
            if (src != null)
            {
                for (int i = 0; i < src.Count; ++i)
                {
                    result.Add(src[i].Clone());
                }
            }
            return result;
        }

        public static List<CharacterDataInt32> Clone(this IList<CharacterDataInt32> src)
        {
            return src.Clone<List<CharacterDataInt32>>();
        }

        public static T Clone<T>(this IList<CharacterDataFloat32> src) where T : IList<CharacterDataFloat32>, new()
        {
            T result = new T();
            if (src != null)
            {
                for (int i = 0; i < src.Count; ++i)
                {
                    result.Add(src[i].Clone());
                }
            }
            return result;
        }

        public static List<CharacterDataFloat32> Clone(this IList<CharacterDataFloat32> src)
        {
            return src.Clone<List<CharacterDataFloat32>>();
        }
        #endregion

        #region IndexOf Functions
        public static int IndexOf(this IList<CharacterAttribute> list, int dataId)
        {
            for (int i = 0; i < list.Count; ++i)
            {
                if (list[i].dataId == dataId)
                    return i;
            }
            return -1;
        }

        public static int IndexOf(this IList<CharacterBuff> list, BuffType type, int dataId)
        {
            for (int i = 0; i < list.Count; ++i)
            {
                if (list[i].type == type && list[i].dataId == dataId)
                    return i;
            }
            return -1;
        }

        public static int IndexOf(this IList<CharacterBuff> list, string id)
        {
            for (int i = 0; i < list.Count; ++i)
            {
                if (!string.IsNullOrEmpty(list[i].id) && list[i].id.Equals(id))
                    return i;
            }
            return -1;
        }

        public static List<int> IndexesOf(this IList<CharacterBuff> list, BuffType type, int dataId)
        {
            List<int> result = new List<int>();
            for (int i = 0; i < list.Count; ++i)
            {
                if (list[i].type == type && list[i].dataId == dataId)
                    result.Add(i);
            }
            return result;
        }

        public static int IndexOf(this IList<CharacterItem> list, int dataId)
        {
            for (int i = 0; i < list.Count; ++i)
            {
                if (list[i].dataId == dataId)
                    return i;
            }
            return -1;
        }

        public static int IndexOf(this IList<CharacterItem> list, string id)
        {
            for (int i = 0; i < list.Count; ++i)
            {
                if (!string.IsNullOrEmpty(list[i].id) && list[i].id.Equals(id))
                    return i;
            }
            return -1;
        }

        public static int IndexOf(this IList<CharacterSkill> list, int dataId)
        {
            for (int i = 0; i < list.Count; ++i)
            {
                if (list[i].dataId == dataId)
                    return i;
            }
            return -1;
        }

        public static int IndexOf(this IList<CharacterSkillUsage> list, SkillUsageType type, int dataId)
        {
            for (int i = 0; i < list.Count; ++i)
            {
                if (list[i].type == type && list[i].dataId == dataId)
                    return i;
            }
            return -1;
        }

        public static int IndexOf(this IList<CharacterSummon> list, uint objectId)
        {
            for (int i = 0; i < list.Count; ++i)
            {
                if (list[i].objectId == objectId)
                    return i;
            }
            return -1;
        }

        public static int IndexOf(this IList<CharacterSummon> list, SummonType type)
        {
            for (int i = 0; i < list.Count; ++i)
            {
                if (list[i].type == type)
                    return i;
            }
            return -1;
        }

        public static int IndexOf(this IList<CharacterSummon> list, SummonType type, int dataId)
        {
            for (int i = 0; i < list.Count; ++i)
            {
                if (list[i].type == type && list[i].dataId == dataId)
                    return i;
            }
            return -1;
        }

        public static int IndexOf(this IList<CharacterHotkey> list, string hotkeyId)
        {
            for (int i = 0; i < list.Count; ++i)
            {
                if (!string.IsNullOrEmpty(list[i].hotkeyId) && list[i].hotkeyId.Equals(hotkeyId))
                    return i;
            }
            return -1;
        }

        public static int IndexOf(this IList<CharacterQuest> list, int dataId)
        {
            for (int i = 0; i < list.Count; ++i)
            {
                if (list[i].dataId == dataId)
                    return i;
            }
            return -1;
        }

        public static int IndexOf(this IList<CharacterCurrency> list, int dataId)
        {
            for (int i = 0; i < list.Count; ++i)
            {
                if (list[i].dataId == dataId)
                    return i;
            }
            return -1;
        }

        public static int IndexOf(this IList<CharacterDataBoolean> list, int hashedKey)
        {
            for (int i = 0; i < list.Count; ++i)
            {
                if (list[i].hashedKey == hashedKey)
                    return i;
            }
            return -1;
        }

        public static int IndexOf(this IList<CharacterDataInt32> list, int hashedKey)
        {
            for (int i = 0; i < list.Count; ++i)
            {
                if (list[i].hashedKey == hashedKey)
                    return i;
            }
            return -1;
        }

        public static int IndexOf(this IList<CharacterDataFloat32> list, int hashedKey)
        {
            for (int i = 0; i < list.Count; ++i)
            {
                if (list[i].hashedKey == hashedKey)
                    return i;
            }
            return -1;
        }
        #endregion

        #region Custom Data getter/setter
        public static bool GetValue(this IList<CharacterDataBoolean> list, int hashedKey, bool defaultValue = false)
        {
            int index = list.IndexOf(hashedKey);
            if (index < 0)
                return defaultValue;
            return list[index].value;
        }

        public static void SetValue(this IList<CharacterDataBoolean> list, int hashedKey, bool value = false)
        {
            int index = list.IndexOf(hashedKey);
            if (index < 0)
            {
                list.Add(CharacterDataBoolean.Create(hashedKey, value));
                return;
            }
            list[index] = CharacterDataBoolean.Create(hashedKey, value);
        }

        public static int GetValue(this IList<CharacterDataInt32> list, int hashedKey, int defaultValue = 0)
        {
            int index = list.IndexOf(hashedKey);
            if (index < 0)
                return defaultValue;
            return list[index].value;
        }

        public static void SetValue(this IList<CharacterDataInt32> list, int hashedKey, int value)
        {
            int index = list.IndexOf(hashedKey);
            if (index < 0)
            {
                list.Add(CharacterDataInt32.Create(hashedKey, value));
                return;
            }
            list[index] = CharacterDataInt32.Create(hashedKey, value);
        }
        public static float GetValue(this IList<CharacterDataFloat32> list, int hashedKey, float defaultValue = 0f)
        {
            int index = list.IndexOf(hashedKey);
            if (index < 0)
                return defaultValue;
            return list[index].value;
        }

        public static void SetValue(this IList<CharacterDataFloat32> list, int hashedKey, float value)
        {
            int index = list.IndexOf(hashedKey);
            if (index < 0)
            {
                list.Add(CharacterDataFloat32.Create(hashedKey, value));
                return;
            }
            list[index] = CharacterDataFloat32.Create(hashedKey, value);
        }
        #endregion
    }
}