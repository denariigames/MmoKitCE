using UnityEngine.Video;

namespace Insthync.AudioManager
{
    public class VideoPlayerVolumeSetter : AudioComponent
    {
        private VideoPlayer _cacheVideoPlayer;
        private int _dirtyVolume;
    	private int _dirtyTrackCount;

        void Start()
        {
            _cacheVideoPlayer = GetComponent<VideoPlayer>();
            if (_cacheVideoPlayer == null)
                enabled = false;
        }

        private void Update()
        {
            float volume = AudioManager.Singleton.GetVolumeLevel(SettingId);
            int intVolume = (int)(volume * 100);
            if (_dirtyVolume != intVolume && _cacheVideoPlayer.audioTrackCount != _dirtyTrackCount)
            {
                _dirtyVolume = intVolume;
            	_dirtyTrackCount = _cacheVideoPlayer.audioTrackCount;
                if (_cacheVideoPlayer.canSetDirectAudioVolume)
                {
                    for (ushort i = 0; i < _cacheVideoPlayer.audioTrackCount; ++i)
                    {
                        _cacheVideoPlayer.SetDirectAudioVolume(i, volume);
                    }
                }
            }
        }
    }
}
