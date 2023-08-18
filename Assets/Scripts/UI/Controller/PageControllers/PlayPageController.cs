using System.Collections;
using System.Collections.Generic;
using Minimax.CoreSystems;
using Minimax.Utilities;
using UnityEngine;
using UnityEngine.UI;

namespace Minimax.UI.Controller.PageControllers
{
    public class PlayPageController : MonoBehaviour
    {
        [SerializeField] private Button m_startGameButton;
        
        private void Start()
        {
            m_startGameButton.onClick.AddListener(RequestLoadGamePlayScene);
        }
        
        private void RequestLoadGamePlayScene()
        {
            GlobalManagers.Instance.Scene.RequestLoadScene(SceneType.GamePlayScene, true);
        }
    }
}
