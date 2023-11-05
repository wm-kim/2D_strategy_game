using Minimax.CoreSystems;
using Minimax.Definitions;
using UnityEngine;

namespace Minimax.UI.Controller.ComponentControllers
{
    public class BottomUIController : MonoBehaviour
    {
        [SerializeField] private PageNavigationManager m_pageNavigationManager;
        [SerializeField] private ButtonGroupController m_bottomButtonGroupController;
        private                  bool                  m_initialButtonClicked = false;

        public void Init(int initialIndexToClick = 0)
        {
            m_bottomButtonGroupController.Init(initialIndexToClick);
            m_bottomButtonGroupController.OnButtonClicked  += PlayClickSoundAfterInit;
            m_bottomButtonGroupController.OnButtonSelected += OnBottomButtonSelected;
        }

        private void OnBottomButtonSelected(int index)
        {
            m_pageNavigationManager.SwitchNavigation(index);
        }

        private void PlayClickSoundAfterInit(int index)
        {
            if (!m_bottomButtonGroupController.IsInitialButtonClicked) return;
            AudioManager.Instance.PlaySFX(AudioLib.Button);
        }
    }
}