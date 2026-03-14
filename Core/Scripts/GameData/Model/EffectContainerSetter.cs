using System.Collections.Generic;
using LiteNetLibManager;
using UnityEngine;

namespace MultiplayerARPG
{
    public class EffectContainerSetter : MonoBehaviour
    {
        public GameObject defaultModel;

        public void ApplyToCharacterModel(GameEntityModel gameEntityModel)
        {
            if (gameEntityModel == null)
            {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                Logging.LogWarning("Cannot find game entity model");
#endif
                return;
            }
            bool hasChanges = false;
            bool isFound = false;
            List<EffectContainer> effectContainers = new List<EffectContainer>();
            if (gameEntityModel.EffectContainers != null)
                effectContainers.AddRange(gameEntityModel.EffectContainers);
            for (int i = 0; i < effectContainers.Count; ++i)
            {
                EffectContainer effectContainer = effectContainers[i];
                if (effectContainer.transform == transform)
                {
                    isFound = true;
                    hasChanges = true;
                    effectContainer.effectSocket = name;
                    effectContainer.transform = transform;
                    effectContainers[i] = effectContainer;
                    break;
                }
            }
            if (!isFound)
            {
                hasChanges = true;
                EffectContainer newEffectContainer = new EffectContainer();
                newEffectContainer.effectSocket = name;
                newEffectContainer.transform = transform;
                effectContainers.Add(newEffectContainer);
            }
            if (hasChanges)
            {
                gameEntityModel.EffectContainers = effectContainers.ToArray();
            }
        }
    }
}
