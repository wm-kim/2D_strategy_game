using Minimax.ScriptableObjects.Events;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Assertions;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace Minimax
{
    [DefaultExecutionOrder(-1)]
    public class EditorColdStartup : MonoBehaviour
    {
#if UNITY_EDITOR
        [SerializeField] private SceneType m_thisScene = default;
        
        [Header("Broadcasting on")]
        [SerializeField] private AssetReference m_coldStartupEvent;
        
        private void Awake()
        {
            if (!SceneManager.GetSceneByName(SceneType.PersistentScene.ToString()).isLoaded)
            {
                SceneManager.LoadSceneAsync(SceneType.PersistentScene.ToString(), LoadSceneMode.Additive).completed += LoadEventChannel;
            }
        }
        
        private void LoadEventChannel(AsyncOperation obj)
        {
            m_coldStartupEvent.LoadAssetAsync<LoadSceneEventSO>().Completed += OnNotifyChannelLoaded;
        }
        
        private void OnNotifyChannelLoaded(AsyncOperationHandle<LoadSceneEventSO> obj)
        {
            Assert.IsTrue(m_thisScene != SceneType.Undefined, "This scene is not defined in the SceneType enum!");
            obj.Result.RaiseEvent(m_thisScene);
        }
#endif
    }
}
