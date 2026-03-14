using Insthync.AudioManager;
using UnityEngine;

namespace MultiplayerARPG
{
    public partial class FxCollection
    {
        private ParticleSystem[] _particles;
        private LineRenderer[] _lineRenderers;
        private AudioSource[] _audioSources;
        private AudioSourceSetter[] _audioSourceSetters;
        private bool[] _particleDefaultLoops;
        private bool[] _lineRendererDefaultLoops;
        private bool[] _audioSourceDefaultLoops;

        public FxCollection(GameObject gameObject)
        {
            int i;
            _particles = gameObject.GetComponentsInChildren<ParticleSystem>(true);
            _particleDefaultLoops = new bool[_particles.Length];
            for (i = 0; i < _particles.Length; ++i)
            {
                _particleDefaultLoops[i] = _particles[i].main.loop;
            }
            _lineRenderers = gameObject.GetComponentsInChildren<LineRenderer>(true);
            _lineRendererDefaultLoops = new bool[_lineRenderers.Length];
            for (i = 0; i < _lineRenderers.Length; ++i)
            {
                _lineRendererDefaultLoops[i] = _lineRenderers[i].loop;
            }
            _audioSources = gameObject.GetComponentsInChildren<AudioSource>(true);
            _audioSourceDefaultLoops = new bool[_audioSources.Length];
            for (i = 0; i < _audioSources.Length; ++i)
            {
                _audioSourceDefaultLoops[i] = _audioSources[i].loop;
            }
            _audioSourceSetters = gameObject.GetComponentsInChildren<AudioSourceSetter>(true);
        }

        ~FxCollection()
        {
            _particles?.Nullify();
            _lineRenderers?.Nullify();
            _audioSources?.Nullify();
            _audioSourceSetters?.Nullify();
        }

        public void RevertLoop()
        {
            int i;
            ParticleSystem.MainModule mainEmitter;
            for (i = 0; i < _particles.Length; ++i)
            {
                mainEmitter = _particles[i].main;
                mainEmitter.loop = _particleDefaultLoops[i];
            }
            for (i = 0; i < _lineRenderers.Length; ++i)
            {
                _lineRenderers[i].loop = _lineRendererDefaultLoops[i];
            }
            for (i = 0; i < _audioSources.Length; ++i)
            {
                _audioSources[i].loop = _audioSourceDefaultLoops[i];
            }
        }

        public void SetLoop(bool loop)
        {
            int i;
            ParticleSystem.MainModule mainEmitter;
            for (i = 0; i < _particles.Length; ++i)
            {
                mainEmitter = _particles[i].main;
                mainEmitter.loop = loop;
            }
            for (i = 0; i < _lineRenderers.Length; ++i)
            {
                _lineRenderers[i].loop = loop;
            }
            for (i = 0; i < _audioSources.Length; ++i)
            {
                _audioSources[i].loop = loop;
            }
        }

        public void InitPrefab()
        {
            // Prepare particles
            ParticleSystem.MainModule mainEmitter;
            foreach (ParticleSystem particle in _particles)
            {
                mainEmitter = particle.main;
                mainEmitter.playOnAwake = false;
            }
            // Prepare audio sources
            foreach (AudioSource audioSource in _audioSources)
            {
                audioSource.playOnAwake = false;
            }
            // Prepare audio source setters
            foreach (AudioSourceSetter audioSourceSetter in _audioSourceSetters)
            {
                audioSourceSetter.playOnAwake = false;
                audioSourceSetter.playOnEnable = false;
            }
        }

        public void Play()
        {
            if (Application.isBatchMode)
                return;
            int i;
            // Play particles
            ParticleSystem.MainModule mainEmitter;
            for (i = 0; i < _particles.Length; ++i)
            {
                mainEmitter = _particles[i].main;
                mainEmitter.loop = _particleDefaultLoops[i];
                _particles[i].Play();
            }
            // Revert line renderers loop
            for (i = 0; i < _lineRenderers.Length; ++i)
            {
                _lineRenderers[i].loop = _lineRendererDefaultLoops[i];
            }
            // Play audio sources
            float volume = AudioManager.Singleton == null ? 1f : AudioManager.Singleton.sfxVolumeSetting.Level;
            for (i = 0; i < _audioSources.Length; ++i)
            {
                _audioSources[i].loop = _audioSourceDefaultLoops[i];
                _audioSources[i].volume = volume;
                _audioSources[i].Play();
            }
            // Play audio source setters
            for (i = 0; i < _audioSourceSetters.Length; ++i)
            {
                _audioSourceSetters[i].Play();
            }
        }

        public void Stop()
        {
            if (Application.isBatchMode)
                return;
            int i;
            // Stop particles
            for (i = 0; i < _particles.Length; ++i)
            {
                _particles[i].Stop();
            }
            // Stop audio sources
            for (i = 0; i < _audioSources.Length; ++i)
            {
                _audioSources[i].Stop();
            }
            // Stop audio source setters
            for (i = 0; i < _audioSourceSetters.Length; ++i)
            {
                _audioSourceSetters[i].Stop();
            }
        }
    }
}
