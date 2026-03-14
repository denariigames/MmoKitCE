using UnityEngine;
using UnityEngine.UI;

namespace MultiplayerARPG
{
    [System.Serializable]
    public struct UIAttributeTextPair
    {
        public Attribute attribute;
        public TextWrapper uiText;
        public Image imageIcon;
        public GameObject root;
    }
}
