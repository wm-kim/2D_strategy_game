using BrunoMikoski.AnimationSequencer;
using Cysharp.Threading.Tasks;
using Minimax.Utilities;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Minimax.SceneManagement
{
    /// <summary>
    /// Wrapper class for loading scenes, shows a loading screen while loading.
    /// </summary>
    public class SceneLoader : NetworkBehaviour
    {
        // To prevent a new loading request while already loading a new scene
        private bool m_isLoading = false;

        // 아래 두 개의 변수는 로딩 화면 애니메이션을 담당하는 컨트롤러입니다.
        [SerializeField] private AnimationSequencerController m_showLoadingScreenAnimation;
        [SerializeField] private AnimationSequencerController m_hideLoadingScreenAnimation;

        [SerializeField, ReadOnly] string m_sceneToLoad;
        [SerializeField, ReadOnly] string m_currentlyLoadedScene;
        public string CurrentlyLoadedScene => m_currentlyLoadedScene;
        
        private NetworkManager m_netManager => NetworkManager.Singleton;
        private bool m_isInitialized = false;
        private bool m_unloadCompleted = false;
        
        bool IsNetworkSceneManagementEnabled =>
            m_netManager != null &&
            m_netManager.SceneManager != null &&
            m_netManager.NetworkConfig.EnableSceneManagement;

        private void Start()
        {
            NetworkManager.OnServerStarted += OnNetworkingSessionStarted;
            NetworkManager.OnClientStarted += OnNetworkingSessionStarted;
            NetworkManager.OnServerStopped += OnNetworkingSessionEnded;
            NetworkManager.OnClientStopped += OnNetworkingSessionEnded;
        }
        
        public override void OnDestroy()
        {

            if (NetworkManager != null)
            {
                NetworkManager.OnServerStarted -= OnNetworkingSessionStarted;
                NetworkManager.OnClientStarted -= OnNetworkingSessionStarted;
                NetworkManager.OnServerStopped -= OnNetworkingSessionEnded;
                NetworkManager.OnClientStopped -= OnNetworkingSessionEnded;
            }
            base.OnDestroy();
        }
        
#if UNITY_EDITOR
        public void RequestColdStartup(SceneType sceneToLoad) => ColdStartup(sceneToLoad.ToString());
#endif
        
        void OnNetworkingSessionStarted()
        {
            // This prevents this to be called twice on a host, which receives both OnServerStarted and OnClientStarted callbacks
            if (!m_isInitialized)
            {
                if (IsNetworkSceneManagementEnabled)
                {
                    NetworkManager.SceneManager.OnSceneEvent += OnSceneEvent;
                    
                    if (NetworkManager.IsServer)
                    {
                        // validate scene before loading for security purposes
                        NetworkManager.SceneManager.VerifySceneBeforeLoading = ServerSideLoadSceneValidation;
                        NetworkManager.SceneManager.VerifySceneBeforeUnloading = ServerSideUnLoadSceneValidation;
                        NetworkManager.SceneManager.SetClientSynchronizationMode(LoadSceneMode.Additive);
                    }
                    else
                    {
                        NetworkManager.SceneManager.PostSynchronizationSceneUnloading = true;
                    }
                }
                m_isInitialized = true;
            }
        }

        

        void OnNetworkingSessionEnded(bool unused)
        {
            if (m_isInitialized)
            {
                if (IsNetworkSceneManagementEnabled)
                {
                    NetworkManager.SceneManager.OnSceneEvent -= OnSceneEvent;
                }
                m_isInitialized = false;
            }
        }

        private bool ServerSideLoadSceneValidation(int sceneIndex, string sceneName, LoadSceneMode loadSceneMode)
        {
            return loadSceneMode == LoadSceneMode.Additive;
        }
        
        private bool ServerSideUnLoadSceneValidation(Scene scene)
        {
            bool isPersistentScene = scene.name == SceneType.PersistentScene.ToString();
            bool isCurrentScene = scene.name == m_currentlyLoadedScene;
            return !isPersistentScene || isCurrentScene;
        }

#if UNITY_EDITOR
        private void ColdStartup(string sceneToLoad) => m_currentlyLoadedScene = sceneToLoad;
#endif 
        private bool IsInitialLoading => string.IsNullOrEmpty(m_currentlyLoadedScene);

        public async void LoadScene(SceneType sceneTypeToLoad, bool useNetwork = false)
        {
            var sceneToLoad = sceneTypeToLoad.ToString();
            
            if (m_currentlyLoadedScene == sceneToLoad)
            {
                Debug.LogWarning($"Trying to load scene {sceneToLoad} which is already loaded");
                return;
            }
            
            if (useNetwork)
            {
                if (IsSpawned && IsNetworkSceneManagementEnabled && !NetworkManager.ShutdownInProgress)
                {
                    // Server에서 scene unload와 load관련한 로직을 처리합니다.
                    if (NetworkManager.IsServer)
                    {
                        ShowLoadingScreenClientRpc();
                        
                        // 처음 로딩할 때는 unload를 하지 않습니다.
                        if (!IsInitialLoading)
                        {
                            var sceneToUnload = SceneManager.GetSceneByName(m_currentlyLoadedScene);
                            var unloadStatus = NetworkManager.SceneManager.UnloadScene(sceneToUnload);
                            CheckStatus(unloadStatus, m_currentlyLoadedScene, false);
                            await UniTask.WaitUntil(() => m_unloadCompleted == true);
                            m_unloadCompleted = false;
                        }
                        
                        var loadStatus = NetworkManager.SceneManager.LoadScene(sceneToLoad, LoadSceneMode.Additive);
                        CheckStatus(loadStatus, sceneToLoad, true);
                    }
                }
            }
            else // 네트워크를 통하지 않고 씬을 로드합니다.
            {
                // Prevent a double loading request
                if (m_isLoading) return;
                m_isLoading = true;
                m_sceneToLoad = sceneToLoad;
                
                if (!IsInitialLoading)
                {   
                    // 로딩 화면 애니메이션을 재생하며, 애니메이션이 끝날 때까지 대기합니다.
                    m_showLoadingScreenAnimation.Play();
                    await UniTask.WaitUntil(() => m_showLoadingScreenAnimation.IsPlaying == false);
                    
                    // 이전에 로드된 씬을 언로드합니다.
                    await SceneManager.UnloadSceneAsync(m_currentlyLoadedScene);
                }
                
                // 네트워크를 통하지 않고 씬을 로드합니다.
                AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(m_sceneToLoad, LoadSceneMode.Additive);
                await UniTask.WaitUntil(() => asyncOperation.isDone); 
                SceneLoadCompleted(m_sceneToLoad);
            }
        }
        
        private void SceneLoadCompleted(string sceneToLoad)
        {
            m_currentlyLoadedScene = sceneToLoad;
             var activeScene = SceneManager.GetSceneByName(m_currentlyLoadedScene);
            // Set the loaded scene as the active scene
            SceneManager.SetActiveScene(activeScene);
            
            m_isLoading = false;
            if (!IsInitialLoading) m_hideLoadingScreenAnimation.Play();
        }
        
        private void OnSceneEvent(SceneEvent sceneEvent)
        {
            var clientOrServer = sceneEvent.ClientId == NetworkManager.ServerClientId ? "server" : "client";
            switch (sceneEvent.SceneEventType)
            {
                case SceneEventType.LoadComplete: 
                {
                    if (NetworkManager.IsClient)
                    {
                        DebugWrapper.Log($"Loaded the {sceneEvent.SceneName} scene on " +
                                                  $"{clientOrServer}-({sceneEvent.ClientId}).");
                    }
                    break;
                }
                case SceneEventType.LoadEventCompleted: // Server told client that all clients finished loading a scene
                case SceneEventType.UnloadEventCompleted:
                {
                    var load = sceneEvent.SceneEventType == SceneEventType.LoadEventCompleted ? "Load" : "Unload";
                    if (NetworkManager.IsClient)
                    {
                        DebugWrapper.Log($"{load} {sceneEvent.SceneName} event completed for the following client " +
                                                  $"identifiers : ({string.Join(",", sceneEvent.ClientsThatCompleted)})");
                    }
                    if (load == "Load") SceneLoadCompleted(sceneEvent.SceneName);
                    if (load == "Unload") m_unloadCompleted = true;
                    break;
                }
            }
        }
        
        [ClientRpc]
        private void ShowLoadingScreenClientRpc()
        {
            // 로딩 화면 애니메이션을 재생하며, 애니메이션이 끝날 때까지 대기합니다.
            m_showLoadingScreenAnimation.Play();
            UniTask.WaitUntil(() => m_showLoadingScreenAnimation.IsPlaying == false);
        }
        
        private void CheckStatus(SceneEventProgressStatus status, string targetScene, bool isLoading = true)
        {
            var sceneEventAction = isLoading ? "load" : "unload";
            if (status != SceneEventProgressStatus.Started)
            {
                DebugWrapper.LogWarning($"Failed to {sceneEventAction} {targetScene} with" +
                                                 $" a {nameof(SceneEventProgressStatus)}: {status}");
            }
        }
    }
}
    