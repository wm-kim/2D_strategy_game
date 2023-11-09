using Minimax.CoreSystems;
using Minimax.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

namespace Minimax.UI.Controller.PageControllers
{
    public class DeckPageController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField]
        private Button m_buildNewDeckButton;

        private void Start()
        {
            m_buildNewDeckButton.onClick.AddListener(RequestLoadDeckBuildScene);
        }

        private void RequestLoadDeckBuildScene()
        {
            GlobalManagers.Instance.Scene.LoadScene(SceneType.DeckBuildingScene);
        }
    }
}