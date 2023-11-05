using Minimax.GamePlay;
using Minimax.SceneManagement;
using Minimax.UnityGamingService.Multiplayer.ConnectionManagement;
using UnityEngine;
using UnityEngine.Serialization;
using Utilities;

namespace Minimax.CoreSystems
{
    /// <summary>
    /// This class is service locator for all the persistent managers.
    /// </summary>
    public class GlobalManagers : MonoSingleton<GlobalManagers>
    {
        [SerializeField] private InputManager      m_inputManager;
        [SerializeField] private SceneLoader       m_sceneLoader;
        [SerializeField] private CacheManager      m_cacheManager;
        [SerializeField] private ConnectionManager m_connectonManager;

        public InputManager      Input          => m_inputManager;
        public SceneLoader       Scene          => m_sceneLoader;
        public CacheManager      Cache          => m_cacheManager;
        public ConnectionManager Connection     => m_connectonManager;
        public ServiceLocator    ServiceLocator { get; private set; } = new();
    }
}