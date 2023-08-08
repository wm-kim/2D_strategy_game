using System.Collections;
using System.Collections.Generic;
using Minimax.CoreSystems;
using UnityEngine;
using UnityEngine.UI;

namespace Minimax
{
    public class DeckBuildingPageController : MonoBehaviour
    {
        [SerializeField] private CloudSaveManager m_cloudSaveManager;
        
        [Header("References")]
        [Space(10f)]
        [SerializeField] private Button m_exitWithoutSaveButton;
        
        private void Start()
        {
            m_exitWithoutSaveButton.onClick.AddListener(RequestLoadMenuScene);
        }
        
        private void RequestLoadMenuScene()
        {
            
            GlobalManagers.Instance.Scene.RequestLoadScene(SceneType.MenuScene);
        }
    }
}
