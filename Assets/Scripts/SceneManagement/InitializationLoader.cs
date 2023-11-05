using Cysharp.Threading.Tasks;
using Minimax.CoreSystems;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Minimax.SceneManagement
{
    public class InitializationLoader : MonoBehaviour
    {
        [Header("Client Start Scene")] [SerializeField]
        private SceneType m_clientStartScene = default;

        [Header("Server Start Scene")] [SerializeField]
        private SceneType m_serverStartScene = default;

        private async void Start()
        {
            var asyncOperation =
                SceneManager.LoadSceneAsync(SceneType.PersistentScene.ToString(), LoadSceneMode.Additive);
            await UniTask.WaitUntil(() => asyncOperation.isDone);
            LoadInitialScene();
        }

        private void LoadInitialScene()
        {
#if !DEDICATED_SERVER
            GlobalManagers.Instance.Scene.LoadScene(m_clientStartScene);
            SceneManager.UnloadSceneAsync(0);
#else
            GlobalManagers.Instance.Scene.LoadScene(m_serverStartScene);
            SceneManager.UnloadSceneAsync(0);
#endif
        }
    }
}