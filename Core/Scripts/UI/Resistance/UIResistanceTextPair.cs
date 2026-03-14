using UnityEngine;
using UnityEngine.UI;

namespace MultiplayerARPG
{
    [System.Serializable]
    public struct UIResistanceTextPair
    {
        public DamageElement damageElement;
        public TextWrapper uiText;
        public Image imageIcon;
        public GameObject root;
    }
}
