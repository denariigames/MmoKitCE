using UnityEngine;

namespace MultiplayerARPG
{
    public interface IPoolDescriptor
    {
        IPoolDescriptor ObjectPrefab { get; set; }
        GameObject gameObject { get; }
        Transform transform { get; }
        int PoolSize { get; }
        /// <summary>
        /// Use this function to prepare prefab default value
        /// </summary>
        void InitPrefab();
        /// <summary>
        /// Use this function to do something when get instance from pool
        /// </summary>
        void OnGetInstance();
        /// <summary>
        /// Use this function to do something when push back to pool
        /// </summary>
        void OnPushBack();
    }
}
