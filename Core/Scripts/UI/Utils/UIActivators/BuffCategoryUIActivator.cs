using UnityEngine;

namespace MultiplayerARPG
{
    public class BuffCategoryUIActivator : MonoBehaviour
    {
        public string buffCategory;
        public GameObject[] appliedSignObjects = new GameObject[0];

        private void OnDestroy()
        {
            appliedSignObjects?.Nullify();
            appliedSignObjects = null;
        }

        private void LateUpdate()
        {
            if (appliedSignObjects == null)
                return;
            bool hasABuff = false;
            BaseGameData tempData;
            if (GameInstance.PlayingCharacter != null)
            {
                for (int i = 0; i < GameInstance.PlayingCharacter.Buffs.Count; ++i)
                {
                    CharacterBuff buff = GameInstance.PlayingCharacter.Buffs[i];
                    tempData = null;
                    switch (buff.type)
                    {
                        case BuffType.SkillBuff:
                        case BuffType.SkillDebuff:
                            tempData = buff.GetSkill();
                            break;
                        case BuffType.PotionBuff:
                            tempData = buff.GetItem();
                            break;
                        case BuffType.GuildSkillBuff:
                            tempData = buff.GetGuildSkill();
                            break;
                        case BuffType.StatusEffect:
                            tempData = buff.GetStatusEffect();
                            break;
                    }
                    if (tempData != null && string.Equals(buffCategory, tempData.Category))
                    {
                        hasABuff = true;
                        break;
                    }
                }
            }
            foreach (GameObject obj in appliedSignObjects)
            {
                obj.SetActive(hasABuff);
            }
        }
    }
}
