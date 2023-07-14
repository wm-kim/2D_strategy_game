using BrunoMikoski.AnimationSequencer;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace WMK
{
    public class SceneLoader : MonoBehaviour
    {
        // To prevent a new loading request while already loading a new scene
        private bool m_isLoading = false;
        
        [Header("Listening to")]
        [SerializeField] private LoadEventSO m_loadSceneEvent;
        [SerializeField] private LoadEventSO m_coldStartupEvent;
        
        [SerializeField] private AnimationSequencerController m_showLoadingScreenAnimation;
        [SerializeField] private AnimationSequencerController m_hideLoadingScreenAnimation;

        [SerializeField, ReadOnly] private SceneSO m_sceneToLoad;
        [SerializeField, ReadOnly] private SceneSO m_currentlyLoadedScene;
        
        private AsyncOperationHandle<SceneInstance> m_loadingOperationHandle;
        
        private void OnEnable()
        {
            m_loadSceneEvent.OnLoadRequested.AddListener(LoadScene);
            #if UNITY_EDITOR
            m_coldStartupEvent.OnLoadRequested.AddListener(ColdStartup);
            #endif
        }
        
        private void OnDisable()
        {
            m_loadSceneEvent.OnLoadRequested.RemoveListener(LoadScene);
            #if UNITY_EDITOR
            m_coldStartupEvent.OnLoadRequested.RemoveListener(ColdStartup);
            #endif
        }
        
        private void LoadScene(SceneSO sceneToLoad)
        {
            // Prevent a double loading request
            if (m_isLoading) return;
            m_sceneToLoad = sceneToLoad;
            
            UnloadPreviousScene();
        }
        
        #if UNITY_EDITOR
        private void ColdStartup(SceneSO sceneToLoad) => m_currentlyLoadedScene = sceneToLoad;
        #endif
        
        private async UniTaskVoid UnloadPreviousScene()
        {
            m_showLoadingScreenAnimation.Play();
            await UniTask.WaitUntil(() => m_showLoadingScreenAnimation.IsPlaying == false);

            // Would be null if the game was started in Initialisation
            if (m_currentlyLoadedScene != null)
            {
                if (m_currentlyLoadedScene.SceneReference.OperationHandle.IsValid())
                {
                    // Unload the scene through its AssetReference, i.e. through the Addressable system
                    m_currentlyLoadedScene.SceneReference.UnLoadScene();
                }
                #if UNITY_EDITOR
                else
                {
                    // In the editor, since the operation handle has not been used, we need to unload the scene by its name.
                    SceneManager.UnloadSceneAsync(m_currentlyLoadedScene.SceneReference.editorAsset.name);
                }
                #endif
            }
            
            LoadNewScene();
        }
        
        private void LoadNewScene()
        {
            m_loadingOperationHandle = m_sceneToLoad.SceneReference.LoadSceneAsync(LoadSceneMode.Additive, true, 0);
            m_loadingOperationHandle.Completed += OnNewSceneLoaded;
        }
        
        private void OnNewSceneLoaded(AsyncOperationHandle<SceneInstance> obj)
        {
            m_currentlyLoadedScene = m_sceneToLoad;
            
            Scene s = obj.Result.Scene;
            SceneManager.SetActiveScene(s);

            m_isLoading = false;
            
            m_hideLoadingScreenAnimation.Play();
        }
    }
}
    