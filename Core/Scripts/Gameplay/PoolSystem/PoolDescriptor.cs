using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace MultiplayerARPG
{
    public class PoolDescriptor : MonoBehaviour, IPoolDescriptor
    {
        public IPoolDescriptor ObjectPrefab { get; set; }

        [SerializeField]
        private int poolSize = 30;
        public int PoolSize { get { return poolSize; } set { poolSize = value; } }

        public UnityEvent onInitPrefab = new UnityEvent();
        public UnityEvent onGetInstance = new UnityEvent();
        public UnityEvent onPushBack = new UnityEvent();

        public virtual void InitPrefab()
        {
            onInitPrefab.Invoke();
        }

        public virtual void OnGetInstance()
        {
            onGetInstance.Invoke();
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

        public virtual void OnPushBack()
        {
            onPushBack.Invoke();
        }
    }
}
