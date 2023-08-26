using System;
using Minimax.CoreSystems;
using Minimax.SceneManagement;
using NovaSamples.UIControls;
using UnityEngine;

namespace Minimax.UI.Controller.PageControllers
{
    public class MainPageController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Button m_startButton;

        private void Awake()
        {
            m_startButton.OnClicked.AddListener(RequestLoadMenuScene);
        }

        private void RequestLoadMenuScene()
        {
            GlobalManagers.Instance.Scene.LoadScene(SceneType.MenuScene);
        }
    }
}
