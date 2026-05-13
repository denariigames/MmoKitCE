using System.Collections.Generic;
using UnityEngine;

namespace MultiplayerARPG
{
    public static class PoolSystem
    {
        private static Dictionary<IPoolDescriptor, Queue<IPoolDescriptor>> pooledObjects = new Dictionary<IPoolDescriptor, Queue<IPoolDescriptor>>();
#if UNITY_EDITOR && INIT_POOL_TO_TRANSFORM
        private static Transform poolingTransform;
        private static Transform PoolingTransform
        {
            get
            {
                if (poolingTransform == null)
                    poolingTransform = new GameObject("_PoolingTransform").transform;
                return poolingTransform;
            }
        }
#endif

        public static void Clear()
        {
            foreach (Queue<IPoolDescriptor> queue in pooledObjects.Values)
            {
                while (queue.Count > 0)
                {
                    IPoolDescriptor instance = queue.Dequeue();
                    try
                    {
                        // I tried to avoid null exception but it still ocurring
                        if ((Object)instance != null)
                            Object.Destroy(instance.gameObject);
                    }
                    catch { }
                }
            }
            pooledObjects.Clear();
        }

        public static void InitPool(IPoolDescriptor prefab)
        {
            if ((Object)prefab == null)
            {
                Debug.LogWarning($"[PoolSystem] Cannot init prefab: {prefab}");
                return;
            }

            if (pooledObjects.ContainsKey(prefab))
                return;

            prefab.InitPrefab();

            Queue<IPoolDescriptor> queue = new Queue<IPoolDescriptor>();

            IPoolDescriptor obj;

            for (int i = 0; i < prefab.PoolSize; ++i)
            {
                obj = Object.Instantiate(prefab.gameObject).GetComponent<IPoolDescriptor>();
#if UNITY_EDITOR && INIT_POOL_TO_TRANSFORM
                obj.transform.SetParent(PoolingTransform);
#endif
                obj.ObjectPrefab = prefab;
                obj.gameObject.SetActive(false);
                queue.Enqueue(obj);
            }

            pooledObjects[prefab] = queue;
        }

        public static T GetInstance<T>(T prefab, System.Action<T> onBeforeActivated = null)
            where T : Object, IPoolDescriptor
        {
            if (prefab == null)
                return null;
            T instance = GetInstance(prefab, Vector3.zero, Quaternion.identity, onBeforeActivated);
            return instance;
        }

        public static T GetInstance<T>(T prefab, Vector3 position, Quaternion rotation, System.Action<T> onBeforeActivated = null)
            where T : Object, IPoolDescriptor
        {
            if (prefab == null)
                return null;
            Queue<IPoolDescriptor> queue;
            if (pooledObjects.TryGetValue(prefab, out queue))
            {
                IPoolDescriptor obj;

                if (queue.Count > 0)
                {
                    obj = queue.Dequeue();
                }
                else
                {
                    obj = Object.Instantiate(prefab.gameObject).GetComponent<IPoolDescriptor>();
#if UNITY_EDITOR && INIT_POOL_TO_TRANSFORM
                    obj.transform.SetParent(PoolingTransform);
#endif
                }
                if (onBeforeActivated != null)
                    onBeforeActivated.Invoke(obj as T);
                obj.ObjectPrefab = prefab;
                obj.transform.position = position;
                obj.transform.rotation = rotation;
                obj.gameObject.SetActive(true);
                obj.OnGetInstance();

                return obj as T;
            }

            InitPool(prefab);
            return GetInstance(prefab, position, rotation, onBeforeActivated);
        }

        public static void PushBack<T>(T instance)
            where T : Object, IPoolDescriptor
        {
            if (instance == null)
            {
                Debug.LogWarning($"[PoolSystem] Cannot push back. The instance's is empty.");
                return;
            }
            if (instance.ObjectPrefab == null)
            {
                Debug.LogWarning($"[PoolSystem] Cannot push back ({instance.gameObject}). The instance's prefab is empty");
                return;
            }
            Queue<IPoolDescriptor> queue;
            if (!pooledObjects.TryGetValue(instance.ObjectPrefab, out queue))
            {
                Debug.LogWarning($"[PoolSystem] Cannot push back ({instance.gameObject}). The instance's prefab does not initailized yet.");
                return;
            }
            if (queue.Contains(instance))
            {
                return;
            }
            if (queue.Count >= instance.ObjectPrefab.PoolSize)
            {
                Object.Destroy(instance.gameObject);
            }
            else
            {
#if UNITY_EDITOR && INIT_POOL_TO_TRANSFORM
                instance.transform.SetParent(PoolingTransform);
#else
                instance.transform.SetParent(null);
#endif
                instance.OnPushBack();
                instance.gameObject.SetActive(false);
                queue.Enqueue(instance);
            }
        }
    }
}
