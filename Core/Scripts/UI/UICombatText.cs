using Cysharp.Text;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MultiplayerARPG
{
    public class UICombatText : MonoBehaviour, IPoolDescriptor
    {
        public const string POSITIVE_SIGNED_FORMAT = "+{0:N0}";
        public const string NEGATIVE_SIGNED_FORMAT = "{0:N0}";

        public float lifeTime = 2f;
        public string format = "{0}";
        public bool showPositiveSign = false;
        public TextWrapper textComponent;
        public IPoolDescriptor ObjectPrefab { get; set; }

        public int poolSize = 30;
        public int PoolSize { get { return poolSize; } set { poolSize = value; } }

        private int _amount;
        public int Amount
        {
            get { return _amount; }
            set
            {
                _amount = value;
                textComponent.text = ZString.Format(format, ZString.Format(showPositiveSign && _amount > 0 ? POSITIVE_SIGNED_FORMAT : NEGATIVE_SIGNED_FORMAT, Amount));
            }
        }

        public void InitPrefab()
        {

        }

        public void OnGetInstance()
        {

        }

        public void OnPushBack()
        {

        }

        public void PushBack(float delay)
        {
            PushBackRoutine(delay).Forget();
        }

        private async UniTaskVoid PushBackRoutine(float delay)
        {
            await UniTask.Delay((int)(delay * 1000));
            PushBack();
        }

        public virtual void PushBack()
        {
            PoolSystem.PushBack(this);
        }

        private void Awake()
        {
            CacheComponents();
        }

        private void OnEnable()
        {
            PushBack(lifeTime);
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
