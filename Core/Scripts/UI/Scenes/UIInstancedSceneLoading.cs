using Cysharp.Threading.Tasks;
using Insthync.AddressableAssetTools;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace MultiplayerARPG
{
    public partial class UIInstancedSceneLoading : MonoBehaviour
    {
        public GameObject rootObject;
        public TextWrapper uiTextProgress;
        public Image imageGage;
        public Slider sliderGage;
        [Tooltip("Delay before deactivate `rootObject`")]
        public float finishedDelay = 0.25f;

        protected virtual void Awake()
        {
            if (rootObject != null)
                rootObject.SetActive(false);
        }

        public virtual async UniTask LoadScene(string sceneName)
        {
            if (SceneManager.GetActiveScene().name.Equals(sceneName))
                return;
            if (rootObject != null)
                rootObject.SetActive(true);
            if (uiTextProgress != null)
                uiTextProgress.text = "0.00%";
            if (imageGage != null)
                imageGage.fillAmount = 0;
            if (sliderGage != null)
                sliderGage.value = 0;
            await UniTask.Yield();
            AsyncOperation asyncOp = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
            while (!asyncOp.isDone)
            {
                if (uiTextProgress != null)
                    uiTextProgress.text = (asyncOp.progress * 100f).ToString("N2") + "%";
                if (imageGage != null)
                    imageGage.fillAmount = asyncOp.progress;
                if (sliderGage != null)
                    sliderGage.value = asyncOp.progress;
                await UniTask.Yield();
            }
            await UniTask.Yield();
            if (uiTextProgress != null)
                uiTextProgress.text = "100.00%";
            if (imageGage != null)
                imageGage.fillAmount = 1;
            if (sliderGage != null)
                sliderGage.value = 1;
            await UniTask.Delay(Mathf.CeilToInt(finishedDelay * 1000));
            if (rootObject != null)
                rootObject.SetActive(false);
#if !DISABLE_ADDRESSABLES
            AddressableAssetsManager.ReleaseAll();
#endif
            await Resources.UnloadUnusedAssets();
        }

#if !DISABLE_ADDRESSABLES
        public virtual async UniTask LoadScene(AssetReferenceScene sceneRef)
        {
            if (SceneManager.GetActiveScene().name.Equals(sceneRef.SceneName))
                return;
            if (rootObject != null)
                rootObject.SetActive(true);
            if (uiTextProgress != null)
                uiTextProgress.text = "0.00%";
            if (imageGage != null)
                imageGage.fillAmount = 0;
            if (sliderGage != null)
                sliderGage.value = 0;
            await UniTask.Yield();
            var asyncOp = GameInstance.LoadAddressableScene(sceneRef, LoadSceneMode.Single);
            while (!asyncOp.IsDone)
            {
                if (uiTextProgress != null)
                    uiTextProgress.text = (asyncOp.PercentComplete * 100f).ToString("N2") + "%";
                if (imageGage != null)
                    imageGage.fillAmount = asyncOp.PercentComplete;
                if (sliderGage != null)
                    sliderGage.value = asyncOp.PercentComplete;
                await UniTask.Yield();
            }
            await UniTask.Yield();
            if (uiTextProgress != null)
                uiTextProgress.text = "100.00%";
            if (imageGage != null)
                imageGage.fillAmount = 1;
            if (sliderGage != null)
                sliderGage.value = 1;
            await UniTask.Delay(Mathf.CeilToInt(finishedDelay * 1000));
            if (rootObject != null)
                rootObject.SetActive(false);
            AddressableAssetsManager.ReleaseAll();
            await Resources.UnloadUnusedAssets();
        }
#endif
    }
}
