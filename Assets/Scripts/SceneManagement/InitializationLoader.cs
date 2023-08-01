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
        [SerializeField] private AssetReference  m_loadSceneEvent = default;
        
        private void Start()
        {   
            SceneManager.LoadSceneAsync(SceneType.PersistentScene, LoadSceneMode.Additive).completed += LoadInitialScene;
        }
        
        private void LoadInitialScene(AsyncOperation obj)
        {
#if !DEDICATED_SERVER
            m_loadSceneEvent.LoadAssetAsync<LoadSceneEventSO>().Completed += (operation) =>
            {
                operation.Result.LoadScene(m_mainScene);
                SceneManager.UnloadSceneAsync(0);
            };
#else
            m_loadSceneEvent.LoadAssetAsync<LoadSceneEventSO>().Completed += (operation) =>
            {
                operation.Result.RaiseEvent(m_gamePlayScene);
                SceneManager.UnloadSceneAsync(0);
            };
#endif
        }
    }
}