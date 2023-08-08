using System;
using System.Collections;
using System.Collections.Generic;
using Minimax.UI.View.Popups;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Minimax
{
    public class CommonTwoButtonPopup : PopupView
    {
        [Header("References")]
        [SerializeField] TextMeshProUGUI m_messageText;
        [SerializeField] Button m_leftButton;
        [SerializeField] Button m_rightButton;

        protected override void SetPopupType() => Type = PopupType.CommonTwoButtonPopup;
        
        public void Init(string message, string leftButtonText, string rightButtonText, Action leftButtonCallback, Action rightButtonCallback)
        {
            m_messageText.text = message;
            m_leftButton.GetComponentInChildren<TextMeshProUGUI>().text = leftButtonText;
            m_rightButton.GetComponentInChildren<TextMeshProUGUI>().text = rightButtonText;
            m_leftButton.onClick.AddListener(() =>
            {
                leftButtonCallback?.Invoke();
            });
            m_rightButton.onClick.AddListener(() =>
            {
                rightButtonCallback?.Invoke();
            });
        }
    }
}
