using Cysharp.Text;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace MultiplayerARPG
{
    [System.Serializable]
    public class BuffSourceData
    {
        public BuffType type;
        public BaseGameData data;

        public string Title
        {
            get
            {
                if (data == null)
                    return LanguageManager.GetUnknowTitle();
                return data.Title;
            }
        }

        public string Description
        {
            get
            {
                if (data == null)
                    return LanguageManager.GetUnknowDescription();
                return data.Description;
            }
        }

#if UNITY_EDITOR || !UNITY_SERVER
        public async UniTask<Sprite> GetIcon()
        {
            if (data == null)
                return null;
            return await data.GetIcon();
        }
#endif

        public bool IsValid()
        {
            if (data == null)
                return false;
            switch (type)
            {
                case BuffType.SkillBuff:
                    return data is BaseSkill skillWithBuff && skillWithBuff.TryGetBuff(out _);
                case BuffType.SkillDebuff:
                    return data is BaseSkill skillWithDebuff && skillWithDebuff.TryGetDebuff(out _);
                case BuffType.PotionBuff:
                    return data is BaseItem item && item.ItemType == ItemType.Potion;
                case BuffType.GuildSkillBuff:
                    return data is GuildSkill guildSkill && guildSkill.IsActive;
                case BuffType.StatusEffect:
                    return data is StatusEffect;
            }
            return false;
        }

        public string GetId()
        {
            if (data == null)
                return string.Empty;
            return ZString.Concat((byte)type, '_', data.DataId);
        }

        public override int GetHashCode()
        {
            if (data == null)
                return 0;
            return GetId().GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return GetHashCode() == obj.GetHashCode();
        }

        public override string ToString()
        {
            return GetId();
        }
    }
}