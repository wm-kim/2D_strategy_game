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
        [SerializeField] private UnityEditor.SceneAsset m_thisScene = default;
        
        [Header("Broadcasting on")]
        [SerializeField] private AssetReference m_coldStartupEvent;
        
        private void Awake()
        {
            if (!SceneManager.GetSceneByName(SceneType.PersistentScene).isLoaded)
            {
                SceneManager.LoadSceneAsync(SceneType.PersistentScene, LoadSceneMode.Additive).completed += LoadEventChannel;
            }
        }
        
        private void LoadEventChannel(AsyncOperation obj)
        {
            m_coldStartupEvent.LoadAssetAsync<LoadSceneEventSO>().Completed += OnNotifyChannelLoaded;
        }
        
        private void OnNotifyChannelLoaded(AsyncOperationHandle<LoadSceneEventSO> obj)
        {
            Assert.IsNotNull(m_thisScene, "This scene is not set in the EditorColdStartup component.");
            obj.Result.RaiseEvent(m_thisScene);
        }
#endif
    }
}
