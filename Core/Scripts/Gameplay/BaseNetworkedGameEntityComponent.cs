using UnityEngine;
using LiteNetLibManager;

namespace MultiplayerARPG
{
    public abstract class BaseNetworkedGameEntityComponent<T> : LiteNetLibBehaviour
        where T : BaseGameEntity
    {
        private bool _isFoundEntity;
        private T _cacheEntity;
        public T Entity
        {
            get
            {
                if (!_isFoundEntity)
                {
                    _cacheEntity = GetComponent<T>();
                    _isFoundEntity = _cacheEntity != null;
                }
                return _cacheEntity;
            }
        }
        [System.Obsolete("Keeping this for backward compatibility, use `Entity` instead.")]
        public T CacheEntity { get { return Entity; } }

        public GameInstance CurrentGameInstance { get { return Entity.CurrentGameInstance; } }
        public BaseGameplayRule CurrentGameplayRule { get { return Entity.CurrentGameplayRule; } }
        public BaseGameNetworkManager CurrentGameManager { get { return Entity.CurrentGameManager; } }
        public Transform EntityTransform { get { return Entity.EntityTransform; } }

        protected virtual void OnDestroy()
        {
            _cacheEntity = null;
        }
    }
}
