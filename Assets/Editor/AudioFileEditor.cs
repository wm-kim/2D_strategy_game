using System.Collections;
using Minimax.ScriptableObjects;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEngine;
using Utilities;
using Debug = Utilities.Debug;

namespace Minimax
{
    [CustomEditor(typeof(AudioFileSO))]
    public class AudioFileEditor : Editor
    {
        private AudioSource m_audioSource;
        private GameObject m_tempGO;

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector(); 
        
            AudioFileSO audioFile = (AudioFileSO)target;

            if (GUILayout.Button("Play"))
            {
                if (m_audioSource != null && m_audioSource.isPlaying && m_audioSource.loop)
                {
                    Debug.LogWarning("Audio is already playing in loop. Stop the current audio before playing a new one.");
                    return;
                }
                
                if (m_tempGO == null)
                {
                    m_tempGO = EditorUtility.CreateGameObjectWithHideFlags("TempAudioSource", HideFlags.HideAndDontSave);
                    m_audioSource = m_tempGO.AddComponent<AudioSource>();
                }

                m_audioSource.volume = audioFile.Volume;
                m_audioSource.pitch = audioFile.Pitch;
                m_audioSource.loop = audioFile.Loop;
                audioFile.Play(m_audioSource);

                if (!audioFile.Loop)
                {
                    EditorCoroutineUtility.StartCoroutineOwnerless(DestroyAfterAudio(m_audioSource.clip.length));
                }
            }

            if (GUILayout.Button("Stop"))
            {
                if (m_audioSource != null)
                {
                    m_audioSource.Stop();
                }

                if (m_tempGO != null)
                {
                    DestroyImmediate(m_tempGO);
                    m_tempGO = null;
                }
            }
        }

        private IEnumerator DestroyAfterAudio(float waitTime)
        {
            yield return new WaitForSecondsRealtime(waitTime);
        
            if (m_tempGO != null)
            {
                DestroyImmediate(m_tempGO);
                m_tempGO = null;
            }
        }
    }
}
