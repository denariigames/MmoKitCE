using UnityEngine;
using UnityEngine.UI;

namespace MultiplayerARPG
{
    [System.Serializable]
    public struct UISkillTextPair
    {
        public BaseSkill skill;
        public TextWrapper uiText;
        public Image imageIcon;
        public GameObject root;
    }
}
