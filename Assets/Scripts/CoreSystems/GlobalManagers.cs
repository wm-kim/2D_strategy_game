using Minimax.UnityGamingService.CloudSave;
using Minimax.Utilities;
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
        public InputManager Input => m_inputManager;
        public PopupManager Popup => m_popupManager;
        public SceneLoader Scene => m_sceneLoader;
    }
}