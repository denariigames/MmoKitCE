using System.Collections.Generic;
using UnityEngine;

namespace MultiplayerARPG
{
    public class PoolingGameEffectsPlayer : MonoBehaviour, IPoolDescriptorCollection
    {
#if !UNITY_SERVER || UNITY_EDITOR
        public GameEffectPoolContainer[] poolingGameEffects;
#endif

        public IEnumerable<IPoolDescriptor> PoolDescriptors
        {
            get
            {
                List<IPoolDescriptor> effects = new List<IPoolDescriptor>();
#if !UNITY_SERVER || UNITY_EDITOR
                if (poolingGameEffects != null && poolingGameEffects.Length > 0)
                {
                    foreach (GameEffectPoolContainer container in poolingGameEffects)
                    {
#if UNITY_EDITOR || !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
                        effects.Add(container.prefab);
#endif
                    }
                }
#endif
                return effects;
            }
        }

        public void PlayRandomEffect()
        {
#if !UNITY_SERVER || UNITY_EDITOR
            if (poolingGameEffects != null && poolingGameEffects.Length > 0)
                poolingGameEffects[Random.Range(0, poolingGameEffects.Length)].GetInstance();
#endif
        }
    }
}
