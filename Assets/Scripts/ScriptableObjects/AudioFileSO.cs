using System.Collections.Generic;
using UnityEngine;
using Utilities;
using Debug = Utilities.Debug;

namespace Minimax.ScriptableObjects
{
    [CreateAssetMenu(fileName = "New Audio File", menuName = "ScriptableObjects/AudioFileSO")]
    public class AudioFileSO : ScriptableObject
    {
        [Header("Audio Settings")]
        [Range(0, 1)]
        public float Volume = 1f; // 추가한 볼륨 컨트롤

        [Range(0, 3)]
        public float Pitch = 1f;

        public bool Loop       = false;
        public bool PlayRandom = false;

        [Header("Audio Clips")]
        public List<AudioClip> Clips;

        /// <summary>
        /// Play audio clip from audio source.
        /// </summary>
        /// <param name="audioSource">The AudioSource component to play the audio on</param>
        /// <param name="volumeFactor">Factor by which to scale the base volume</param>
        public void Play(AudioSource audioSource, float volumeFactor = 1f)
        {
            if (Clips == null || Clips.Count == 0)
                Debug.LogWarning("AudioFileSO: " + name + " has no audio clips.");

            audioSource.loop   = Loop;
            audioSource.pitch  = Pitch;
            audioSource.volume = Volume * volumeFactor;

            // 랜덤 재생 여부에 따라 클립 선택 및 재생
            audioSource.clip = PlayRandom ? Clips[Random.Range(0, Clips.Count)] : Clips[0];
            audioSource.Play();
        }
    }
}