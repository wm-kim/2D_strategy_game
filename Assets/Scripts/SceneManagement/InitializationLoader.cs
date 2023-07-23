using Minimax.ScriptableObjects.Events;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace Minimax
{
    public class InitializationLoader : MonoBehaviour
    {
        [SerializeField] private SceneSO m_persistentScene = default;
        
        [Header("Client Start Scene")]
        [SerializeField] private SceneSO m_mainScene = default;
        
        [Header("Server Start Scene")]
        [SerializeField] private SceneSO m_gamePlayScene = default;
        
        [Header("Broadcasting on")]
        [SerializeField] private AssetReference m_loadSceneEventChannel = default;

        private void Start()
        {
            m_persistentScene.SceneReference.LoadSceneAsync(LoadSceneMode.Additive, true).Completed += LoadEventChannel;
        }
        
        private void LoadEventChannel(AsyncOperationHandle<SceneInstance> obj)
        {
#if !DEDICATED_SERVER
            m_loadSceneEventChannel.LoadAssetAsync<LoadEventSO>().Completed += LoadMainMenu;
#else
            m_loadSceneEventChannel.LoadAssetAsync<LoadEventSO>().Completed += LoadGamePlay;
#endif
        }
        
        private void LoadMainMenu(AsyncOperationHandle<LoadEventSO> obj)
        {
            obj.Result.RaiseEvent(m_mainScene);
            // Initialization is the only scene in BuildSettings, thus it has index 0
            SceneManager.UnloadSceneAsync(0);
        }
        
        private void LoadGamePlay(AsyncOperationHandle<LoadEventSO> obj)
        {
            obj.Result.RaiseEvent(m_gamePlayScene);
            SceneManager.UnloadSceneAsync(0);
        }
    }
}