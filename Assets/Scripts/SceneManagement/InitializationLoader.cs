using Cysharp.Threading.Tasks;
using Minimax.CoreSystems;
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
        [SerializeField] private SceneType m_mainScene = default;
        
        [Header("Server Start Scene")]
        [SerializeField] private SceneType m_gamePlayScene = default;
        
        private async void Start()
        {
            AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(SceneType.PersistentScene.ToString(), LoadSceneMode.Additive); 
            await UniTask.WaitUntil(() => asyncOperation.isDone);
            LoadInitialScene();
        }
        
        private void LoadInitialScene()
        {
#if !DEDICATED_SERVER
            GlobalManagers.Instance.Scene.RequestLoadScene(m_mainScene);
            SceneManager.UnloadSceneAsync(0);
#else
            GlobalManagers.Instance.Scene.RequestLoadScene(m_gamePlayScene);
            SceneManager.UnloadSceneAsync(0);
#endif
        }
    }
}