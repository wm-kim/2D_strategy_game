using BrunoMikoski.AnimationSequencer;
using Cysharp.Threading.Tasks;
using Minimax.ScriptableObjects.Events;
using Minimax.ScriptableObjects.SceneDatas;
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
        
        [FormerlySerializedAs("m_loadSceneEvent")]
        [Header("Listening to")]
        [SerializeField] private LoadSceneEventSO mLoadSceneSceneEvent;
        [FormerlySerializedAs("m_coldStartupEvent")] [SerializeField] private LoadSceneEventSO mColdStartupSceneEvent;
        
        // 아래 두 개의 변수는 로딩 화면 애니메이션을 담당하는 컨트롤러입니다.
        [SerializeField] private AnimationSequencerController m_showLoadingScreenAnimation;
        [SerializeField] private AnimationSequencerController m_hideLoadingScreenAnimation;

        [SerializeField, ReadOnly] private SceneType m_sceneToLoad = SceneType.Undefined;
        [SerializeField, ReadOnly] private SceneType m_currentlyLoadedScene = SceneType.Undefined;
        
        private void OnEnable()
        {
            mLoadSceneSceneEvent.OnLoadRequested.AddListener(LoadScene);
            #if UNITY_EDITOR
            mColdStartupSceneEvent.OnLoadRequested.AddListener(ColdStartup);
            #endif
        }
        
        private void OnDisable()
        {
            mLoadSceneSceneEvent.OnLoadRequested.RemoveListener(LoadScene);
            #if UNITY_EDITOR
            mColdStartupSceneEvent.OnLoadRequested.RemoveListener(ColdStartup);
            #endif
        }
        
        private void LoadScene(SceneType sceneToLoad)
        {
            // Prevent a double loading request
            if (m_isLoading) return;
            m_sceneToLoad = sceneToLoad;
            
            // 이전 씬을 언로드하는 작업을 시작합니다.
            UnloadPreviousScene().Forget();
        }
        
        #if UNITY_EDITOR
        private void ColdStartup(SceneType sceneToLoad) => m_currentlyLoadedScene = sceneToLoad;
        #endif 
        
        private async UniTaskVoid UnloadPreviousScene()
        {
            // 로딩 화면 애니메이션을 재생하며, 애니메이션이 끝날 때까지 대기합니다.
            m_showLoadingScreenAnimation.Play();
            await UniTask.WaitUntil(() => m_showLoadingScreenAnimation.IsPlaying == false);

            // Would be null if the game was started in Initialisation
            if (m_currentlyLoadedScene != SceneType.Undefined)
            {
                await SceneManager.UnloadSceneAsync(m_currentlyLoadedScene.ToString());
            }
            
            LoadNewScene();
        }
        
        private void LoadNewScene()
        {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(m_sceneToLoad.ToString(), LoadSceneMode.Additive);
            asyncLoad.completed += OnNewSceneLoaded;
        }
        
        private void OnNewSceneLoaded(AsyncOperation obj)
        {
            m_currentlyLoadedScene = m_sceneToLoad;
            var activeScene = SceneManager.GetSceneByName(m_currentlyLoadedScene.ToString());
            SceneManager.SetActiveScene(activeScene);
            m_isLoading = false;
            m_hideLoadingScreenAnimation.Play();
        }
    }
}
    