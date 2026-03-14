using UnityEngine;

namespace MultiplayerARPG.GameData.Model.Playables
{
    public class PlayableCharacterModelInitializer : MonoBehaviour
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Initialize()
        {
            AnimationPlayableBehaviour.ClearCaches();
        }
    }
}
