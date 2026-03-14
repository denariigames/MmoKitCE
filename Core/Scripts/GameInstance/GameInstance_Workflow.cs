using Cysharp.Threading.Tasks;
using Insthync.AddressableAssetTools;
using LiteNetLibManager;
using System.Collections.Generic;
using UnityEngine;
#if !DISABLE_ADDRESSABLES
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
#endif
using UnityEngine.SceneManagement;

namespace MultiplayerARPG
{
    public partial class GameInstance
    {
        [Header("Home Scene")]
        public SceneField homeScene;
#if !DISABLE_ADDRESSABLES
        public AssetReferenceScene addressableHomeScene;
#endif
        [Tooltip("If this is empty, it will use `Home Scene` as home scene")]
        public SceneField homeMobileScene;
#if !DISABLE_ADDRESSABLES
        public AssetReferenceScene addressableHomeMobileScene;
#endif
        [Tooltip("If this is empty, it will use `Home Scene` as home scene")]
        public SceneField homeConsoleScene;
#if !DISABLE_ADDRESSABLES
        public AssetReferenceScene addressableHomeConsoleScene;
#endif

#if !DISABLE_ADDRESSABLES
        private static List<AsyncOperationHandle<SceneInstance>> s_loadingAddressableSceneHandles = new List<AsyncOperationHandle<SceneInstance>>();

        public static AsyncOperationHandle<SceneInstance> LoadAddressableScene(AssetReferenceScene addressableScene, LoadSceneMode loadSceneMode = LoadSceneMode.Single)
        {
            AsyncOperationHandle<SceneInstance> addressableAsyncOp = addressableScene.LoadSceneAsync(loadSceneMode, true);
            s_loadingAddressableSceneHandles.Add(addressableAsyncOp);
            return addressableAsyncOp;
        }
#endif

#if !DISABLE_ADDRESSABLES
        public static async UniTask UnloadAddressableScenes()
        {
            if (s_loadingAddressableSceneHandles.Count == 0)
                return;
            for (int i = 0; i < s_loadingAddressableSceneHandles.Count; ++i)
            {
                AsyncOperationHandle<SceneInstance> addressableAsyncOp = Addressables.UnloadSceneAsync(s_loadingAddressableSceneHandles[i], UnloadSceneOptions.UnloadAllEmbeddedSceneObjects, true);
                while (!addressableAsyncOp.IsDone)
                {
                    await UniTask.Yield();
                }
            }
            s_loadingAddressableSceneHandles.Clear();
        }
#endif

        public async void LoadHomeScene()
        {
            await LoadHomeSceneTask();
        }

        public async UniTask LoadHomeSceneTask()
        {
#if !DISABLE_ADDRESSABLES
            await UnloadAddressableScenes();
#else
            await UniTask.Yield();
#endif
            if (UISceneLoading.Singleton)
            {
                if (GetHomeScene(out SceneField scene
#if !DISABLE_ADDRESSABLES
                    , out AssetReferenceScene addressableScene
#endif
                    ))
                {
#if !DISABLE_ADDRESSABLES
                    await UISceneLoading.Singleton.LoadScene(addressableScene);
#endif
                }
                else
                {
                    await UISceneLoading.Singleton.LoadScene(scene);
                }
            }
            else
            {
                if (GetHomeScene(out SceneField scene
#if !DISABLE_ADDRESSABLES
                    , out AssetReferenceScene addressableScene
#endif
                    ))
                {
#if !DISABLE_ADDRESSABLES
                    await LoadAddressableScene(addressableScene);
#endif
                }
                else
                {
                    await SceneManager.LoadSceneAsync(scene).ToUniTask();
                }
            }
        }

        /// <summary>
        /// Return `TRUE` if it is addressable
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="addressableScene"></param>
        /// <returns></returns>
        public bool GetHomeScene(out SceneField scene
#if !DISABLE_ADDRESSABLES
            , out AssetReferenceScene addressableScene
#endif
            )
        {
#if !DISABLE_ADDRESSABLES
            addressableScene = null;
#endif
            scene = default;
            if (Application.isMobilePlatform || IsMobileTestInEditor())
            {
#if !DISABLE_ADDRESSABLES
                if (addressableHomeMobileScene.IsDataValid())
                {
                    addressableScene = addressableHomeMobileScene;
                    return true;
                }
#endif
                scene = homeMobileScene;
                return false;
            }
            if (Application.isConsolePlatform || IsConsoleTestInEditor())
            {
#if !DISABLE_ADDRESSABLES
                if (addressableHomeConsoleScene.IsDataValid())
                {
                    addressableScene = addressableHomeConsoleScene;
                    return true;
                }
#endif
                scene = homeConsoleScene;
                return false;
            }
#if !DISABLE_ADDRESSABLES
            if (addressableHomeScene.IsDataValid())
            {
                addressableScene = addressableHomeScene;
                return true;
            }
#endif
            scene = homeScene;
            return false;
        }
    }
}
