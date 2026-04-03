using UnityEngine;

namespace Insthync.AudioManager
{
    public class StateMachineAudioSourcePlayer : StateMachineBehaviour
    {
        public AudioComponentSettingType type;
        public string otherSettingId;
        public AudioSource audioSourcePrefab;
        [Range(0f, 1f)]
        public float triggerTime;
        public AudioClip[] randomClips;

        private bool _isTriggered;
        private int _dirtyVolume;
        private AudioSource _source;

        public string SettingId
        {
            get
            {
                switch (type)
                {
                    case AudioComponentSettingType.Master:
                        return AudioManager.Singleton.masterVolumeSetting.id;
                    case AudioComponentSettingType.Bgm:
                        return AudioManager.Singleton.bgmVolumeSetting.id;
                    case AudioComponentSettingType.Sfx:
                        return AudioManager.Singleton.sfxVolumeSetting.id;
                    case AudioComponentSettingType.Ambient:
                        return AudioManager.Singleton.ambientVolumeSetting.id;
                }
                return otherSettingId;
            }
        }

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            _isTriggered = false;
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            float volume = AudioManager.Singleton.GetVolumeLevel(SettingId);
            if (_source != null)
            {
                int intVolume = (int)(volume * 100);
                if (_dirtyVolume != intVolume)
                {
                    _dirtyVolume = intVolume;
                    _source.volume = volume;
                }
            }
            if (stateInfo.normalizedTime % 1 >= triggerTime && !_isTriggered)
            {
                _isTriggered = true;
                if (audioSourcePrefab != null)
                {
                    _source = Instantiate(audioSourcePrefab, animator.transform.position, Quaternion.identity);
                }
                else
                {
                    GameObject obj = new GameObject("audioSource");
                    obj.transform.position = animator.transform.position;
                    _source = obj.AddComponent<AudioSource>();
                    _source.spatialBlend = 1f;
                }
                _source.volume = volume;
                if (!AudioListener.pause)
                    _source.PlayOneShot(randomClips[Random.Range(0, randomClips.Length)]);
            }
            else if (stateInfo.normalizedTime % 1 < triggerTime && _isTriggered)
            {
                _isTriggered = false;
            }
        }
    }
}
