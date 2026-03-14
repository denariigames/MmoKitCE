using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace MultiplayerARPG
{
    public partial class UINetworkSceneLoading : MonoBehaviour
    {
        public static UINetworkSceneLoading Singleton { get; private set; }
        public GameObject rootObject;
        public TextWrapper uiTextStatus;
        public TextWrapper uiTextProgress;
        public TextWrapper uiTextAdditiveScenesCount;
        public Image imageGage;
        public Slider sliderGage;
        [Tooltip("Delay before deactivate `rootObject`")]
        public float finishedDelay = 0.25f;
        public LanguageTextSetting msgGetFileSize = new LanguageTextSetting()
        {
            defaultText = "Downloading...",
        };
        public LanguageTextSetting msgFileLoading = new LanguageTextSetting()
        {
            defaultText = "Downloading...",
        };
        public LanguageTextSetting msgFileLoaded = new LanguageTextSetting()
        {
            defaultText = "Downloaded",
        };
        public LanguageTextSetting msgSceneLoading = new LanguageTextSetting()
        {
            defaultText = "Scene Loading...",
        };
        public LanguageTextSetting msgSceneLoaded = new LanguageTextSetting()
        {
            defaultText = "Scene Loaded",
        };
        public LanguageTextSetting formatFileLoadingProgress = new LanguageTextSetting()
        {
            defaultText = "{0} {1}%",
        };
        public LanguageTextSetting formatAdditiveSceneLoadingProgress = new LanguageTextSetting()
        {
            defaultText = "Loading {0}/{1} Scenes",
        };

        protected virtual void Awake()
        {
            if (Singleton != null)
            {
                Destroy(gameObject);
                return;
            }

            DontDestroyOnLoad(gameObject);
            Singleton = this;

            if (rootObject != null)
                rootObject.SetActive(false);
            if (sliderGage != null)
            {
                sliderGage.minValue = 0f;
                sliderGage.maxValue = 1f;
            }
        }

        public virtual void OnLoadSceneStart(string sceneName, bool isAdditive, bool isOnline, float progress)
        {
            if (!isAdditive)
            {
                if (uiTextAdditiveScenesCount != null)
                    uiTextAdditiveScenesCount.text = string.Empty;
            }
            if (rootObject != null)
            {
                rootObject.SetActive(true);
            }
            if (uiTextProgress != null)
            {
                uiTextProgress.text = "0.00%";
            }
            if (imageGage != null)
            {
                imageGage.fillAmount = 0f;
            }
            if (sliderGage != null)
            {
                sliderGage.value = 0f;
            }
        }

        public virtual void OnLoadSceneProgress(string sceneName, bool isAdditive, bool isOnline, float progress)
        {
            if (uiTextStatus != null)
            {
                uiTextStatus.text = msgSceneLoading.Text;
            }
            if (uiTextProgress != null)
            {
                uiTextProgress.text = (progress * 100f).ToString("N2") + "%";
            }
            if (imageGage != null)
            {
                imageGage.fillAmount = progress;
            }
            if (sliderGage != null)
            {
                sliderGage.value = progress;
            }
        }

        public virtual void OnLoadSceneFinish(string sceneName, bool isAdditive, bool isOnline, float progress)
        {
            if (isAdditive)
                return;
            StartCoroutine(OnLoadSceneFinishRoutine());
        }

        protected virtual IEnumerator OnLoadSceneFinishRoutine()
        {
            if (uiTextStatus != null)
            {
                uiTextStatus.text = msgSceneLoaded.Text;
            }
            if (uiTextProgress != null)
            {
                uiTextProgress.text = "100.00%";
            }
            if (imageGage != null)
            {
                imageGage.fillAmount = 1f;
            }
            if (sliderGage != null)
            {
                sliderGage.value = 1f;
            }
            yield return new WaitForSecondsRealtime(finishedDelay);
            if (rootObject != null)
            {
                rootObject.SetActive(false);
            }
        }

        public virtual void OnSceneFileSizeRetrieving()
        {
            // Text
            if (uiTextStatus != null)
            {
                uiTextStatus.gameObject.SetActive(true);
                uiTextStatus.text = msgGetFileSize.Text;
            }
            if (uiTextProgress != null)
            {
                uiTextProgress.gameObject.SetActive(true);
                uiTextProgress.text = string.Empty;
            }
            // Gage
            if (imageGage != null)
            {
                imageGage.fillAmount = 0f;
            }
            if (sliderGage != null)
            {
                sliderGage.minValue = 0f;
                sliderGage.value = 0f;
                sliderGage.maxValue = 1f;
            }
        }

        public virtual void OnSceneDepsFileDownloading(long downloadedSize, long fileSize, float percentCompleted)
        {
            // Text
            if (uiTextStatus != null)
            {
                uiTextStatus.text = msgFileLoading.Text;
            }
            if (uiTextProgress != null)
            {
                if (fileSize > 0)
                    uiTextProgress.text = string.Format(formatFileLoadingProgress.Text, GenericUtils.MinMaxSizeSuffix((long)(percentCompleted * fileSize), fileSize), (percentCompleted * 100).ToString("N2"));
                else
                    uiTextProgress.text = string.Empty;
            }
            // Gage
            if (imageGage != null)
            {
                imageGage.fillAmount = percentCompleted;
            }
            if (sliderGage != null)
            {
                sliderGage.value = percentCompleted;
            }
        }

        public virtual void OnSceneDepsDownloaded()
        {
            if (uiTextStatus != null)
            {
                uiTextStatus.text = msgFileLoaded.Text;
            }
            if (uiTextProgress != null)
            {
                uiTextProgress.text = string.Empty;
            }
            // Gage
            if (imageGage != null)
            {
                imageGage.fillAmount = 1f;
            }
            if (sliderGage != null)
            {
                sliderGage.value = 1f;
            }
        }

        public virtual void OnLoadAdditiveSceneStart(int loaded, int total)
        {
            if (uiTextAdditiveScenesCount != null)
            {
                if (total <= 0)
                    uiTextAdditiveScenesCount.text = string.Empty;
                else
                    uiTextAdditiveScenesCount.text = string.Format(formatAdditiveSceneLoadingProgress.Text, loaded, total);
            }
        }

        public virtual void OnLoadAdditiveSceneProgress(int loaded, int total)
        {
            if (uiTextAdditiveScenesCount != null)
            {
                if (total <= 0)
                    uiTextAdditiveScenesCount.text = string.Empty;
                else
                    uiTextAdditiveScenesCount.text = string.Format(formatAdditiveSceneLoadingProgress.Text, loaded, total);
            }
        }

        public virtual void OnLoadAdditiveSceneFinish(int loaded, int total)
        {
            if (uiTextAdditiveScenesCount != null)
            {
                if (total <= 0)
                    uiTextAdditiveScenesCount.text = string.Empty;
                else
                    uiTextAdditiveScenesCount.text = string.Format(formatAdditiveSceneLoadingProgress.Text, loaded, total);
            }
            if (loaded >= total)
            {
                if (rootObject != null)
                    rootObject.SetActive(false);
            }
        }
    }
}