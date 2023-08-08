using Cysharp.Threading.Tasks;
using Minimax.CoreSystems;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

namespace Minimax
{
    [DefaultExecutionOrder(-1)]
    public class EditorColdStartup : MonoBehaviour
    {
#if UNITY_EDITOR
        [Tooltip("The name of the persistent scene.")]
        [SerializeField] private SceneType m_thisScene = default;
        
        private async void Awake()
        {
            if (!IsSceneLoaded(SceneType.PersistentScene))
            {
               AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(SceneType.PersistentScene.ToString(), LoadSceneMode.Additive); 
               await UniTask.WaitUntil(() => asyncOperation.isDone);
               LoadEventChannel();
            }
            else
            {
                Debug.Log("Persistent scene is already loaded.");
            }
        }
        
        private bool IsSceneLoaded(SceneType sceneType)
        {
            return SceneManager.GetSceneByName(sceneType.ToString()).isLoaded;
        }
        
        private void LoadEventChannel()
        {
            Assert.IsTrue(m_thisScene != SceneType.Undefined, "This scene is not defined in the SceneType enum!");
            GlobalManagers.Instance.Scene.RequestColdStartup(m_thisScene);
        }
#endif
    }
}
