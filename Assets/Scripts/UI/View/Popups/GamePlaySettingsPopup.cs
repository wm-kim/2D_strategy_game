using System;
using Minimax.ScriptableObjects.Events;
using UnityEngine;
using UnityEngine.UI;

namespace Minimax.UI.View.Popups
{
    public class GamePlaySettingsPopup : PopupView
    {
        protected override void SetPopupType() => Type = PopupType.GamePlaySettings;
        
        [Header("Buttons")]
        [SerializeField] private Button m_confirmSurrenderButton = default;
        
        [Header("Broadcasting on")]
        [SerializeField] private LoadSceneEventSO m_loadSceneEventChannel = default;

        private void Start()
        {
            m_confirmSurrenderButton.onClick.AddListener(ConfirmSurrender);
        }

        private void ConfirmSurrender()
        {
            m_loadSceneEventChannel.RaiseEvent(SceneType.MenuScene);
        }
    }
}
