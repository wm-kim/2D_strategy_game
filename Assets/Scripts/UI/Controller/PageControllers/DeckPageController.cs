using System.Collections;
using System.Collections.Generic;
using Minimax.CoreSystems;
using UnityEngine;
using UnityEngine.UI;

namespace Minimax
{
    public class DeckPageController : MonoBehaviour
    {
        [SerializeField] private Button m_buildNewDeckButton;
        
        private void Start()
        {
            m_buildNewDeckButton.onClick.AddListener(RequestLoadDeckBuildScene);
        }
        
        private void RequestLoadDeckBuildScene()
        {
            GlobalManagers.Instance.Scene.RequestLoadScene(SceneType.DeckBuildingScene);
        }
    }
}
