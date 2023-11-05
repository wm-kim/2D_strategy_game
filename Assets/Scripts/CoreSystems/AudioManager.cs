using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Minimax.ScriptableObjects;
using Minimax.Utilities;
using UnityEngine;
using UnityEngine.Pool;

namespace Minimax.CoreSystems
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        [Header("Volume Controls")] [SerializeField] [Range(0, 1)]
        private float m_masterVolume = 1f;

        [SerializeField] [Range(0, 1)] private float m_bgmVolume = 1f;
        [SerializeField] [Range(0, 1)] private float m_sfxVolume = 1f;

        [SerializeField] private List<AudioFileSO> m_audioFiles;
        public                   List<AudioFileSO> AudioFiles => m_audioFiles;

        // 빠른 접근을 위한 저장소
        private Dictionary<AudioLib, AudioFileSO> m_audioFileDict;

        [SerializeField] private int                      m_audioPoolSize = 10;
        private                  IObjectPool<AudioSource> m_audioSourcePool;

        private AudioSource       m_currentBGMSource;
        private List<AudioSource> m_activeAudioSources = new();

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);

            InitAudioDicts();
            ConfigureAudioSourcePool();
        }

        private void InitAudioDicts()
        {
            m_audioFileDict = new Dictionary<AudioLib, AudioFileSO>();

            foreach (var audioFile in m_audioFiles)
                if (Enum.TryParse(audioFile.name, out AudioLib sfxEnum)) m_audioFileDict[sfxEnum] = audioFile;
                else DebugWrapper.LogError($"Audio File {audioFile.name} 을 SFX enum으로 변환할 수 없습니다.");
        }

        private void ConfigureAudioSourcePool()
        {
            m_audioSourcePool = new ObjectPool<AudioSource>(() =>
                {
                    var audioSource = new GameObject("AudioSource").AddComponent<AudioSource>();
                    audioSource.transform.SetParent(transform);
                    audioSource.gameObject.SetActive(false);
                    return audioSource;
                },
                (audioSource) => { audioSource.gameObject.SetActive(true); },
                (audioSource) => { audioSource.gameObject.SetActive(false); },
                (audioSource) => { Destroy(audioSource.gameObject); },
                maxSize: m_audioPoolSize);
        }

        public async UniTask PlayBGM(AudioLib audio)
        {
            if (!m_audioFileDict.ContainsKey(audio))
            {
                DebugWrapper.LogError($"BGM with name {audio} does not exist in the dictionary.");
                return;
            }

            var audioFile = m_audioFileDict[audio];
            if (m_currentBGMSource != null) m_currentBGMSource.Stop();

            m_currentBGMSource = m_audioSourcePool.Get();
            var volumeFactor = m_bgmVolume * m_masterVolume;
            audioFile.Play(m_currentBGMSource, volumeFactor);

            m_activeAudioSources.Add(m_currentBGMSource);
            if (!audioFile.Loop)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(m_currentBGMSource.clip.length));
                m_activeAudioSources.Remove(m_currentBGMSource);
                m_audioSourcePool.Release(m_currentBGMSource);
            }
        }

        public async UniTask PlaySFX(AudioLib audio)
        {
            if (!m_audioFileDict.ContainsKey(audio))
            {
                DebugWrapper.LogError($"SFX with name {audio} does not exist in the dictionary.");
                return;
            }

            var audioFile    = m_audioFileDict[audio];
            var audioSource  = m_audioSourcePool.Get();
            var volumeFactor = m_sfxVolume * m_masterVolume;
            audioFile.Play(audioSource, volumeFactor);

            m_activeAudioSources.Add(audioSource);

            if (!audioFile.Loop)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(audioSource.clip.length));
                m_activeAudioSources.Remove(audioSource);
                m_audioSourcePool.Release(audioSource);
            }
        }

        public void Stop(AudioLib audio)
        {
            var audioSource = m_activeAudioSources.Find(source => source.clip.name == audio.ToString());

            if (audioSource == null)
            {
                DebugWrapper.LogWarning($"No active audio found for {audio.ToString()}.");
                return;
            }

            audioSource.Stop();
            m_activeAudioSources.Remove(audioSource);
            m_audioSourcePool.Release(audioSource);

            if (m_currentBGMSource != null && m_currentBGMSource == audioSource)
                m_currentBGMSource = null;
        }

        #region Volume Controls

        public void SetMasterVolume(float volume)
        {
            m_masterVolume = Mathf.Clamp01(volume);
            UpdateAllAudioSourceVolumes();
        }

        public void SetBGMVolume(float volume)
        {
            m_bgmVolume = Mathf.Clamp01(volume);
            if (m_currentBGMSource != null) m_currentBGMSource.volume = m_bgmVolume * m_masterVolume;
        }

        public void SetSFXVolume(float volume)
        {
            m_sfxVolume = Mathf.Clamp01(volume);
            UpdateAllAudioSourceVolumes();
        }

        private void UpdateAllAudioSourceVolumes()
        {
            foreach (var audioSource in m_activeAudioSources)
                audioSource.volume = m_sfxVolume * m_masterVolume;
            if (m_currentBGMSource != null)
                m_currentBGMSource.volume = m_bgmVolume * m_masterVolume;
        }

        #endregion
    }
}