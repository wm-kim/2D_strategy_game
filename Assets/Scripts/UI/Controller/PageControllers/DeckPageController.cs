using System.Collections.Generic;
using Minimax.CoreSystems;
using Minimax.Utilities;
using Unity.Services.CloudSave;
using UnityEngine;
using UnityEngine.UI;

namespace Minimax.UI.Controller.PageControllers
{
    public class DeckPageController : MonoBehaviour
    {
        [SerializeField] private Button m_buildNewDeckButton;
        [SerializeField] private Button m_cloudCodeTestButton;
        
        private void Start()
        {
            m_buildNewDeckButton.onClick.AddListener(RequestLoadDeckBuildScene);
            m_cloudCodeTestButton.onClick.AddListener(CloudSaveTest);
        }
        
        private void RequestLoadDeckBuildScene()
        {
            GlobalManagers.Instance.Scene.RequestLoadScene(SceneType.DeckBuildingScene);
        }
        
        private void CloudSaveTest()
        {
            
        }
    }
}
