using Minimax.ScriptableObjects.Events;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace Minimax
{
    public class EditorColdStartup : MonoBehaviour
    {
        #if UNITY_EDITOR
        
        [SerializeField, ReadOnly] private bool m_isColdStart = false;
        [SerializeField] private SceneSO m_thisSceneSO;
        [SerializeField] private SceneSO m_persistentManagerSO;
        
        [Header("Broadcasting on")]
        [SerializeField] private AssetReference m_coldStartupEventChannel;
        
        private void Awake()
        {
            if (!SceneManager.GetSceneByName(m_persistentManagerSO.SceneReference.editorAsset.name).isLoaded)
            {
                m_isColdStart = true;
            }
        }
        
        private void Start()
        {
            if (m_isColdStart)
                m_persistentManagerSO.SceneReference.LoadSceneAsync(LoadSceneMode.Additive, true).Completed += LoadEventChannel;
        }
        
        private void LoadEventChannel(AsyncOperationHandle<SceneInstance> obj)
        {
            m_coldStartupEventChannel.LoadAssetAsync<LoadEventSO>().Completed += OnNotifyChannelLoaded;
        }
        
        private void OnNotifyChannelLoaded(AsyncOperationHandle<LoadEventSO> obj)
        {
            UnityEngine.Assertions.Assert.IsNotNull(m_thisSceneSO, "You need to set the current scene SO in the EditorColdStartup script");
            obj.Result.RaiseEvent(m_thisSceneSO);
        }
#endif
    }
}
