#if !DISABLE_ADDRESSABLES
using Insthync.AddressableAssetTools;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace MultiplayerARPG
{
    public class UIAddressableAssetDownloadManager : AddressableAssetDownloadManager
    {
        [FormerlySerializedAs("root")]
        public GameObject rootObject;
        public TextWrapper uiTextStatus;
        public TextWrapper uiTextProgress;
        public TextWrapper uiTextTotalProgress;
        [FormerlySerializedAs("loadGage")]
        public Image imageGage;
        public Slider sliderGage;
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
        public LanguageTextSetting msgFileAllLoaded = new LanguageTextSetting()
        {
            defaultText = "Initializing...",
        };
        public LanguageTextSetting formatFileLoadingProgress = new LanguageTextSetting()
        {
            defaultText = "{0} {1}%",
        };
        public LanguageTextSetting formatLoadedFilesProgress = new LanguageTextSetting()
        {
            defaultText = "{0}/{1}",
        };

        private void Awake()
        {
            onStart.AddListener(OnStart);
            onEnd.AddListener(OnEnd);
            onDownloadedAll.AddListener(OnDownloadedAll);
            if (sliderGage != null)
            {
                sliderGage.minValue = 0f;
                sliderGage.maxValue = 1f;
            }
        }

        private void OnDestroy()
        {
            onStart.RemoveAllListeners();
            onStart = null;
            onEnd.RemoveAllListeners();
            onEnd = null;
            onDownloadedAll.RemoveAllListeners();
            onDownloadedAll = null;
        }

        private void OnStart()
        {
            if (rootObject != null)
                rootObject.SetActive(true);
        }

        private void OnEnd()
        {
            if (rootObject != null)
                rootObject.SetActive(false);
        }

        protected override void OnFileSizeRetrieving()
        {
            base.OnFileSizeRetrieving();
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
            if (uiTextTotalProgress != null)
            {
                uiTextTotalProgress.gameObject.SetActive(true);
                uiTextTotalProgress.text = string.Format(formatLoadedFilesProgress.Text, LoadedCount, TotalCount);
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

        protected override void OnDepsFileDownloading(long downloadSize, long fileSize, float percentComplete)
        {
            base.OnDepsFileDownloading(downloadSize, fileSize, percentComplete);
            // Text
            if (uiTextStatus != null)
            {
                uiTextStatus.text = msgFileLoading.Text;
            }
            if (uiTextProgress != null)
            {
                if (fileSize > 0)
                    uiTextProgress.text = string.Format(formatFileLoadingProgress.Text, GenericUtils.MinMaxSizeSuffix((long)(percentComplete * fileSize), fileSize), (percentComplete * 100).ToString("N2"));
                else
                    uiTextProgress.text = string.Empty;
            }
            if (uiTextTotalProgress != null)
            {
                uiTextTotalProgress.text = string.Format(formatLoadedFilesProgress.Text, LoadedCount, TotalCount);
            }
            // Gage
            if (imageGage != null)
            {
                imageGage.fillAmount = percentComplete;
            }
            if (sliderGage != null)
            {
                sliderGage.value = percentComplete;
            }
        }

        protected override void OnDepsDownloaded()
        {
            base.OnDepsDownloaded();
            if (uiTextStatus != null)
            {
                uiTextStatus.text = msgFileLoaded.Text;
            }
            if (uiTextProgress != null)
            {
                uiTextProgress.text = string.Empty;
            }
            if (uiTextTotalProgress != null)
            {
                uiTextTotalProgress.text = string.Format(formatLoadedFilesProgress.Text, LoadedCount, TotalCount);
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

        private void OnDownloadedAll()
        {
            if (uiTextStatus != null)
            {
                uiTextStatus.text = msgFileAllLoaded.Text;
            }
            if (uiTextProgress != null)
            {
                uiTextProgress.gameObject.SetActive(false);
            }
            if (uiTextTotalProgress != null)
            {
                uiTextTotalProgress.gameObject.SetActive(false);
            }
        }
    }
}
#endif