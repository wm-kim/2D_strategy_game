using Minimax.CoreSystems;
using Minimax.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

namespace Minimax.UI.Controller.PageControllers
{
    public class MainPageController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Button m_startButton;

        private void Start()
        {
            m_startButton.onClick.AddListener(RequestLoadMenuScene);
        }
        
        private void RequestLoadMenuScene()
        {
            GlobalManagers.Instance.Scene.RequestLoadScene(SceneType.MenuScene);
        }
    }
}
