using Cysharp.Text;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace MultiplayerARPG
{
    public class UICombatText : MonoBehaviour
    {
        public float lifeTime = 2f;
        public string format = "{0}";
        public bool showPositiveSign;
        public TextWrapper textComponent;

        private int _amount;
        public int Amount
        {
            get { return _amount; }
            set
            {
                _amount = value;
                textComponent.text = ZString.Format(format, (showPositiveSign && _amount > 0 ? "+" : string.Empty) + _amount.ToString("N0"));
            }
        }

        private void Awake()
        {
            CacheComponents();
            Destroy(gameObject, lifeTime);
        }

        private void CacheComponents()
        {
            if (textComponent == null)
            {
                // Try get component which attached to this game object if `textComponent` was not set.
                textComponent = gameObject.GetOrAddComponent<TextWrapper>((comp) =>
                {
                    comp.unityText = GetComponent<Text>();
                    comp.textMeshText = GetComponent<TextMeshProUGUI>();
                });
            }
        }
    }
}
