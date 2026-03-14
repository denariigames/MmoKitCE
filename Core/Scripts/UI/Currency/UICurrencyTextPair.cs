using UnityEngine;
using UnityEngine.UI;

namespace MultiplayerARPG
{
    [System.Serializable]
    public struct UICurrencyTextPair
    {
        public Currency currency;
        public TextWrapper uiText;
        public Image imageIcon;
        public GameObject root;
    }
}
