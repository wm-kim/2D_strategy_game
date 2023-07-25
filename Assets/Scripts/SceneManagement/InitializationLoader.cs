using Minimax.ScriptableObjects.Events;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;

namespace Minimax
{
    public class InitializationLoader : MonoBehaviour
    {
        [Header("Client Start Scene")]
        [SerializeField] private UnityEditor.SceneAsset m_mainScene = default;
        
        [Header("Server Start Scene")]
        [SerializeField] private UnityEditor.SceneAsset m_gamePlayScene = default;
        
        [Header("Broadcasting on")]
        [SerializeField] private AssetReference m_loadSceneEventChannel = default;

        private void Start()
        {
            LoadInitialScene();
        }
        
        private void LoadInitialScene()
        {
#if !DEDICATED_SERVER
            m_loadSceneEventChannel.LoadAssetAsync<LoadSceneEventSO>().Completed += LoadClientStartupScene;
#else
            m_loadSceneEventChannel.LoadAssetAsync<LoadEventSO>().Completed += LoadDedicatedServerStartupScene;
#endif
        }
        
        private void LoadClientStartupScene(AsyncOperationHandle<LoadSceneEventSO> obj)
        {
            obj.Result.RaiseEvent(m_mainScene);
        }
        
        private void LoadDedicatedServerStartupScene(AsyncOperationHandle<LoadSceneEventSO> obj)
        {
            obj.Result.RaiseEvent(m_gamePlayScene);
        }
    }
}