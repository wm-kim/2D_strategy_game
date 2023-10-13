using System;
using Minimax.CoreSystems;
using Minimax.UI.View.Popups;
using UnityEngine;
using UnityEngine.UI;

namespace Minimax
{
    public class GamePlayMenuController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Button m_menuButton;
        
        private void Start()
        {
            m_menuButton.onClick.AddListener(RequestOpenMenuPopup);
        }

        private void OnEnable()
        {
            GlobalManagers.Instance.Input.OnBackButton += OnBackButtonPressed;
        }

        private void OnDisable()
        {
            GlobalManagers.Instance.Input.OnBackButton -= OnBackButtonPressed;
        }

        private void RequestOpenMenuPopup()
        {
            PopupManager.Instance.RegisterPopupToQueue(PopupType.GameMenuPopup, PopupCommandType.Unique);
        }
        
        private void OnBackButtonPressed()
        {
            PopupManager.Instance.MobileBackButtonPopup(PopupType.GameMenuPopup, PopupCommandType.Unique);
        }
    }
}
