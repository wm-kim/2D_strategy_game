using Minimax.ScriptableObjects.Events;
using Minimax.ScriptableObjects.SceneDatas;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace Minimax
{
    public class InitializationLoader : MonoBehaviour
    {
        [Header("Client Start Scene")]
        [SerializeField] private SceneType m_mainScene = default;
        
        [Header("Server Start Scene")]
        [SerializeField] private SceneType m_gamePlayScene = default;
        
        [Header("Broadcasting on")]
        [SerializeField] private AssetReference m_loadSceneEventChannel = default;

        private void Start()
        {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(SceneType.PersistentScene.ToString(), LoadSceneMode.Additive);
            asyncLoad.completed += LoadInitalScene;
        }
        
        private void LoadInitalScene(AsyncOperation obj)
        {
#if !DEDICATED_SERVER
            m_loadSceneEventChannel.LoadAssetAsync<LoadSceneEventSO>().Completed += LoadMainMenu;
#else
            m_loadSceneEventChannel.LoadAssetAsync<LoadEventSO>().Completed += LoadGamePlay;
#endif
        }
        
        private void LoadMainMenu(AsyncOperationHandle<LoadSceneEventSO> obj)
        {
            obj.Result.RaiseEvent(m_mainScene);
            // Initialization is the only scene in BuildSettings, thus it has index 0
            SceneManager.UnloadSceneAsync(0);
        }
        
        private void LoadGamePlay(AsyncOperationHandle<LoadSceneEventSO> obj)
        {
            obj.Result.RaiseEvent(m_gamePlayScene);
            SceneManager.UnloadSceneAsync(0);
        }
    }
}