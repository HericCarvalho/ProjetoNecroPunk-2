using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace AudioSystem
{
    public class MusicManager : PersistentSingleton<MusicManager>
    {
        const float crossFadeTime = 1.5f; // Aumentado um pouco para suavidade
        float fading;
        AudioSource current;
        AudioSource previous;
        readonly Queue<AudioClip> playlist = new();

        [SerializeField] List<AudioClip> initialPlaylist;
        [SerializeField] AudioMixerGroup musicMixerGroup;

        void Start()
        {
            if (initialPlaylist != null && initialPlaylist.Count > 0)
            {
                ChangePlaylist(initialPlaylist);
            }
        }

        public void Clear()
        {
            playlist.Clear();
        }

        public void ChangePlaylist(List<AudioClip> newPlaylist)
        {
            // Se a nova playlist for igual à atual (mesma primeira música), não reseta
            if (newPlaylist.Count > 0 && current != null && current.clip == newPlaylist[0])
            {
                return;
            }

            Clear();
            foreach (var clip in newPlaylist)
            {
                if (clip != null) playlist.Enqueue(clip);
            }

            if (playlist.Count > 0)
            {
                PlayNextTrack();
            }
        }

        public void PlayNextTrack()
        {
            if (playlist.TryDequeue(out AudioClip nextTrack))
            {
                // RE-ENFILEIRA IMEDIATAMENTE para garantir o loop da playlist
                playlist.Enqueue(nextTrack);
                Play(nextTrack);
            }
        }

        public void Play(AudioClip clip)
        {
            if (clip == null) return;
            if (current && current.clip == clip) return;

            // Gerenciamento de Crossfade: Transfere o atual para o anterior
            if (current != null)
            {
                if (previous != null) Destroy(previous);
                previous = current;
            }

            // Cria um NOVO AudioSource para a nova música (evita conflitos de Fade)
            current = gameObject.AddComponent<AudioSource>();
            current.clip = clip;
            current.outputAudioMixerGroup = musicMixerGroup;
            current.loop = false; // O gerenciamento é feito pelo Update
            current.volume = 0;
            current.bypassListenerEffects = true;
            current.playOnAwake = false;
            current.Play();

            fading = 0.001f;
        }

        void Update()
        {
            HandleCrossFade();

            // Se a música acabou e não estamos em meio a um crossfade, pula para a próxima
            if (fading <= 0 && current != null && !current.isPlaying)
            {
                PlayNextTrack();
            }
        }

        void HandleCrossFade()
        {
            if (fading <= 0f) return;

            fading += Time.deltaTime;
            float fraction = Mathf.Clamp01(fading / crossFadeTime);

            // Suavização logarítmica
            float logFraction = fraction <= 0 ? 0 : Mathf.Log10(fraction * 9 + 1);

            if (previous) previous.volume = 1.0f - logFraction;
            if (current) current.volume = logFraction;

            if (fraction >= 1)
            {
                fading = 0.0f;
                if (previous)
                {
                    Destroy(previous);
                    previous = null;
                }
            }
        }
    }
}