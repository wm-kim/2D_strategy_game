using Minimax.CoreSystems;
using Minimax.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

namespace Minimax.UI.View.Popups
{
    public class WinPopup : PopupView
    {
        [Header("References")]
        [Space(10f)]
        [SerializeField] private Button m_confirmButton;
        
        protected override void SetPopupType() => Type = PopupType.WinPopup;
        
        private void Start()
        {
            m_confirmButton.onClick.AddListener(OnConfirmButtonClicked);
        }
        
        private void OnConfirmButtonClicked()
        {
            PopupManager.Instance.HideCurrentPopup();
            GlobalManagers.Instance.Scene.LoadScene(SceneType.MenuScene);
        }
    }
}