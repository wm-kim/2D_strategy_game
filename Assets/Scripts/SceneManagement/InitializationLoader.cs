using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace WMK
{
    public class InitializationLoader : MonoBehaviour
    {
        [SerializeField] private SceneSO m_persistentScene = default;
        [SerializeField] private SceneSO m_mainScene = default;
        
        [Header("Broadcasting on")]
        [SerializeField] private AssetReference m_loadSceneEventChannel = default;

        private void Start()
        {
            m_persistentScene.SceneReference.LoadSceneAsync(LoadSceneMode.Additive, true).Completed += LoadEventChannel;
        }
        
        private void LoadEventChannel(AsyncOperationHandle<SceneInstance> obj)
        {
            m_loadSceneEventChannel.LoadAssetAsync<LoadEventSO>().Completed += LoadMainMenu;
        }
        
        private void LoadMainMenu(AsyncOperationHandle<LoadEventSO> obj)
        {
            obj.Result.RaiseEvent(m_mainScene);
            // Initialization is the only scene in BuildSettings, thus it has index 0
            SceneManager.UnloadSceneAsync(0);
        }
    }
}