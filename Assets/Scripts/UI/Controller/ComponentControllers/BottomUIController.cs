using Minimax.CoreSystems;
using UnityEngine;

namespace Minimax.UI.Controller
{
    public class BottomUIController : MonoBehaviour
    {
        [SerializeField] private PageNavigationManager m_pageNavigationManager;
        [SerializeField] private ButtonGroupController m_bottomButtonGroupController;
        
        private void Awake()
        {
            m_bottomButtonGroupController.OnButtonSelected += OnBottomButtonSelected;
        }

        private void OnBottomButtonSelected(int index) => m_pageNavigationManager.SwitchNavigation(index);
    }
}
