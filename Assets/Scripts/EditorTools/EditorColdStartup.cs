using Minimax.ScriptableObjects.Events;
using Minimax.ScriptableObjects.SceneDatas;
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
        [SerializeField] private SceneType m_thisSceneSO;
        
        [Header("Broadcasting on")]
        [SerializeField] private AssetReference m_coldStartupEventChannel;
        
        private void Awake()
        {
            if (!SceneManager.GetSceneByName(SceneType.PersistentScene.ToString()).isLoaded)
            {
                m_isColdStart = true;
            }
        }
        
        private void Start()
        {
            if (m_isColdStart)
            {
                AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(SceneType.PersistentScene.ToString(), LoadSceneMode.Additive);
                asyncLoad.completed += LoadEventChannel;
            }
        }
        
        private void LoadEventChannel(AsyncOperation obj)
        {
            m_coldStartupEventChannel.LoadAssetAsync<LoadSceneEventSO>().Completed += OnNotifyChannelLoaded;
        }
        
        private void OnNotifyChannelLoaded(AsyncOperationHandle<LoadSceneEventSO> obj)
        {
            UnityEngine.Assertions.Assert.IsTrue(m_thisSceneSO != SceneType.Undefined);
            obj.Result.RaiseEvent(m_thisSceneSO);
        }
#endif
    }
}
