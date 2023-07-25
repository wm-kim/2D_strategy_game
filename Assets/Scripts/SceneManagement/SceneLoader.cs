using BrunoMikoski.AnimationSequencer;
using Cysharp.Threading.Tasks;
using Minimax.ScriptableObjects.Events;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace Minimax
{
    public class SceneLoader : MonoBehaviour
    {
        // To prevent a new loading request while already loading a new scene
        private bool m_isLoading = false;

        [Header("Listening to")] 
        [SerializeField] private LoadSceneEventSO m_loadSceneSceneEvent;

        // 아래 두 개의 변수는 로딩 화면 애니메이션을 담당하는 컨트롤러입니다.
        [SerializeField] private AnimationSequencerController m_showLoadingScreenAnimation;
        [SerializeField] private AnimationSequencerController m_hideLoadingScreenAnimation;

        [SerializeField, ReadOnly] private UnityEditor.SceneAsset m_sceneToLoad = default;

        private void OnEnable()
        {
            m_loadSceneSceneEvent.OnLoadRequested.AddListener(LoadScene);
        }

        private void OnDisable()
        {
            m_loadSceneSceneEvent.OnLoadRequested.RemoveListener(LoadScene);
        }

        private async void LoadScene(UnityEditor.SceneAsset sceneToLoad)
        {
            // Prevent a double loading request
            if (m_isLoading) return;
            m_sceneToLoad = sceneToLoad;

            // 로딩 화면 애니메이션을 재생하며, 애니메이션이 끝날 때까지 대기합니다.
            m_showLoadingScreenAnimation.Play();
            await UniTask.WaitUntil(() => m_showLoadingScreenAnimation.IsPlaying == false);

            SceneManager.LoadScene(m_sceneToLoad.name, LoadSceneMode.Single);
            DebugWrapper.Log($"Loading scene {m_sceneToLoad.ToString()}");

            m_isLoading = false;
            m_hideLoadingScreenAnimation.Play();
        }
        
        
    }
}
    