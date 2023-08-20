using System.Collections.Generic;
using Minimax.SceneManagement;
using Minimax.UnityGamingService.Multiplayer.ConnectionManagement;
using Minimax.Utilities;
using Minimax.Utilities.PubSub;
using UnityEngine;

namespace Minimax.CoreSystems
{
    /// <summary>
    /// This class is service locator for all the persistent managers.
    /// </summary>
    public class GlobalManagers : MonoSingleton<GlobalManagers>
    {
        [SerializeField] private InputManager m_inputManager;
        [SerializeField] private PopupManager m_popupManager;
        [SerializeField] private SceneLoader m_sceneLoader;
        [SerializeField] private CacheManager m_cacheManager;
        [SerializeField] private ConnectionManager m_connectonManager;
        
        public InputManager Input => m_inputManager;
        public PopupManager Popup => m_popupManager;
        public SceneLoader Scene => m_sceneLoader;
        public CacheManager Cache => m_cacheManager;
        public ConnectionManager Connection => m_connectonManager;
        public ServiceLocator ServiceLocator { get; private set; } = new ServiceLocator();
    }
}