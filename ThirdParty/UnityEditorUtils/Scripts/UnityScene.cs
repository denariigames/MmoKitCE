using UnityEngine;

namespace Insthync.UnityEditorUtils
{
    [System.Serializable]
    public struct UnityScene
    {
        [SerializeField]
        private Object sceneAsset;
        [SerializeField]
        private string sceneName;

        public Object SceneAsset
        {
            get { return sceneAsset; }
        }

        public string SceneName
        {
            get { return sceneName; }
            set { sceneName = value; }
        }

        public static implicit operator string(UnityScene unityScene)
        {
            return unityScene.SceneName;
        }

        public bool IsSet()
        {
            return !string.IsNullOrEmpty(sceneName);
        }
    }
}
