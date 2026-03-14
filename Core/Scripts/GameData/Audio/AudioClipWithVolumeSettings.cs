using Insthync.AddressableAssetTools;
using Insthync.AudioManager;
using UnityEngine;

namespace MultiplayerARPG
{
    [System.Serializable]
    public class AudioClipWithVolumeSettings : IAddressableAssetConversable
    {
#if UNITY_EDITOR || !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
        public AudioClip audioClip;
#endif
        public AudioClip AudioClip
        {
            get
            {
#if !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
                return audioClip;
#else
                return null;
#endif
            }
        }

#if !DISABLE_ADDRESSABLES
        public AssetReferenceAudioClip addressableAudioClip;
        public AssetReferenceAudioClip AddressableAudioClip
        {
            get { return addressableAudioClip; }
        }
#endif

        [Range(0f, 1f)]
        public float minRandomVolume = 1f;
        [Range(0f, 1f)]
        public float maxRandomVolume = 1f;

        public float GetRandomedVolume()
        {
            return Random.Range(minRandomVolume, maxRandomVolume);
        }

        public async void Play(AudioSource source)
        {
#if !UNITY_SERVER
            AudioClip clip;
#if !DISABLE_ADDRESSABLES
            clip = await AddressableAudioClip.GetOrLoadObjectAsyncOrUseAsset(AudioClip);
#else
            clip = AudioClip;
#endif
            AudioManager.PlaySfxClipAtAudioSource(clip, source, GetRandomedVolume());
#endif
        }

        public void ProceedAddressableAssetConversion(string groupName)
        {
#if UNITY_EDITOR && !DISABLE_ADDRESSABLES
            AddressableEditorUtils.ConvertObjectRefToAddressable(ref audioClip, ref addressableAudioClip, groupName);
#endif
        }
    }
}
