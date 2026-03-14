using LiteNetLibManager;
using UnityEngine;

namespace MultiplayerARPG
{
    public class UINetworkSceneLoadingEventsSetup : MonoBehaviour
    {
        public LiteNetLibAssets assets;

        private void Awake()
        {
            if (assets == null)
                assets = GetComponent<LiteNetLibAssets>();
            if (assets == null || UINetworkSceneLoading.Singleton == null)
                return;
            assets.onLoadSceneStart.AddListener(UINetworkSceneLoading.Singleton.OnLoadSceneStart);
            assets.onLoadSceneProgress.AddListener(UINetworkSceneLoading.Singleton.OnLoadSceneProgress);
            assets.onLoadSceneFinish.AddListener(UINetworkSceneLoading.Singleton.OnLoadSceneFinish);
            assets.onSceneFileSizeRetrieving.AddListener(UINetworkSceneLoading.Singleton.OnSceneFileSizeRetrieving);
            assets.onSceneDepsFileDownloading.AddListener(UINetworkSceneLoading.Singleton.OnSceneDepsFileDownloading);
            assets.onSceneDepsDownloaded.AddListener(UINetworkSceneLoading.Singleton.OnSceneDepsDownloaded);
            assets.onLoadAdditiveSceneStart.AddListener(UINetworkSceneLoading.Singleton.OnLoadAdditiveSceneStart);
            assets.onLoadAdditiveSceneProgress.AddListener(UINetworkSceneLoading.Singleton.OnLoadAdditiveSceneProgress);
            assets.onLoadAdditiveSceneFinish.AddListener(UINetworkSceneLoading.Singleton.OnLoadAdditiveSceneFinish);
        }

        private void OnDestroy()
        {
            if (assets == null || UINetworkSceneLoading.Singleton == null)
                return;
            assets.onLoadSceneStart.RemoveListener(UINetworkSceneLoading.Singleton.OnLoadSceneStart);
            assets.onLoadSceneProgress.RemoveListener(UINetworkSceneLoading.Singleton.OnLoadSceneProgress);
            assets.onLoadSceneFinish.RemoveListener(UINetworkSceneLoading.Singleton.OnLoadSceneFinish);
            assets.onSceneFileSizeRetrieving.RemoveListener(UINetworkSceneLoading.Singleton.OnSceneFileSizeRetrieving);
            assets.onSceneDepsFileDownloading.RemoveListener(UINetworkSceneLoading.Singleton.OnSceneDepsFileDownloading);
            assets.onSceneDepsDownloaded.RemoveListener(UINetworkSceneLoading.Singleton.OnSceneDepsDownloaded);
            assets.onLoadAdditiveSceneStart.RemoveListener(UINetworkSceneLoading.Singleton.OnLoadAdditiveSceneStart);
            assets.onLoadAdditiveSceneProgress.RemoveListener(UINetworkSceneLoading.Singleton.OnLoadAdditiveSceneProgress);
            assets.onLoadAdditiveSceneFinish.RemoveListener(UINetworkSceneLoading.Singleton.OnLoadAdditiveSceneFinish);
        }
    }
}
