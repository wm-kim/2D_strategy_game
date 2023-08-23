using System;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using Minimax.UI.View.Popups;
using Minimax.Utilities;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Minimax.CoreSystems
{
    public class PopupManager : MonoBehaviour
    {
        [Header("Addressable")] [SerializeField]
        private AssetLabelReference m_popupAssetLabelReference;

        [SerializeField, ReadOnly] private string m_path = "Assets/Prefabs/UIPopups/";

        [SerializeField] private Canvas m_popupCanvas;

        // popup의 대기열을 관리하는 Queue, First come first serve로 popup을 표시한다.
        private Queue<IPopupCommand> m_popupQueue = new Queue<IPopupCommand>();

        // 현재 표시되고 있는 popup
        [SerializeField, ReadOnly] private PopupView m_currentPopupView = null;
        private string m_currentPopupKey = string.Empty;

        [Header("Settings")] [SerializeField, Range(0.0f, 1f)]
        private float m_popupFadeDuration = 0.2f;

        [SerializeField, Range(0.0f, 1f)] private float m_hidePopupFadeDuration = 0.2f;

        [Space(10f)]
        // Popup pool
        [SerializeField, ReadOnly]
        private SerializedDictionary<PopupType, PopupView> m_loadedPopups =
            new SerializedDictionary<PopupType, PopupView>();

        private void Awake()
        {
            PreLoadPopup();
        }

        private void PreLoadPopup()
        {
            Addressables.LoadAssetsAsync<GameObject>(m_popupAssetLabelReference, null).Completed += handle =>
            {
                foreach (var popup in handle.Result)
                {
                    var popupView = popup.GetComponent<PopupView>();
                    popupView.SetPopupTypeAndCheck();
                    m_loadedPopups.Add(popupView.Type, popupView);
                }
            };
        }

        private bool IsPopupShowing => m_currentPopupView != null;

        private bool IsPopupInQueue(IPopupCommand command)
        {
            foreach (var popup in m_popupQueue)
            {
                if (popup.Key == command.Key) return true;
            }

            return false;
        }

        /// <summary>
        /// Popup을 표시합니다. PopupType이 queue안에서 중복되는지 확인하기 위한 key가 됩니다.
        /// </summary>
        public void RegisterPopupToQueue(PopupType key, PopupCommandType commandType = PopupCommandType.Duplicate)
            => RegisterCommandToQueue(new DefaultPopupCommand(key, commandType));


        /// <param name="key">queue안에서 중복되는지 확인하기 위한 식별자입니다.</param>
        public void RegisterTwoButtonPopupToQueue(string key, string message, string leftButtonText,
            string rightButtonText,
            Action leftButtonAction,
            Action rightButtonAction,
            PopupCommandType commandType = PopupCommandType.Duplicate)
            => RegisterCommandToQueue(new TwoButtonPopupCommand(key, message, leftButtonText, rightButtonText,
                leftButtonAction, rightButtonAction, commandType));

        /// <param name="key">queue안에서 중복되는지 확인하기 위한 식별자입니다.</param>
        public void RegisterOneButtonPopupToQueue(string key, string message, string buttonText, Action buttonAction,
            PopupCommandType commandType = PopupCommandType.Duplicate)
            => RegisterCommandToQueue(new OneButtonPopupCommand(key, message, buttonText, buttonAction, commandType));

        /// <param name="key">queue안에서 중복되는지 확인하기 위한 식별자입니다.</param>
        public void MobileBackTwoButtonPopup(string key, string message, string leftButtonText, string rightButtonText,
            Action leftButtonAction, Action rightButtonAction,
            PopupCommandType commandType = PopupCommandType.Duplicate)
            => MobileBackCommand(new TwoButtonPopupCommand(key, message, leftButtonText, rightButtonText,
                leftButtonAction, rightButtonAction, commandType));

        /// <param name="key">queue안에서 중복되는지 확인하기 위한 식별자입니다.</param>
        public void MobileBackButtonPopup(PopupType key, PopupCommandType commandType = PopupCommandType.Duplicate)
            => MobileBackCommand(new DefaultPopupCommand(key, commandType));

        /// <param name="key">queue안에서 중복되는지 확인하기 위한 식별자입니다.</param>
        public void RegisterLoadingPopupToQueue(string key, string message,
            PopupCommandType commandType = PopupCommandType.Duplicate)
            => RegisterCommandToQueue(new LoadingPopupCommand(key, message, commandType));

        /// <summary>
        /// 모바일에서 뒤로가기 버튼을 누르면 호출됩니다.
        /// 이미 표시되고 있는 popup이 있다면 숨기고, 대기열에 더 이상 숨길 popup이 없다면 인자로 받은 Popup을 표시합니다.
        /// </summary>
        private void MobileBackCommand(IPopupCommand command)
        {
            if (IsPopupShowing) HideCurrentPopup();
            else RegisterCommandToQueue(command);
        }

        /// <summary>
        /// PopupCommand Type에 따라 Popup Command를 대기열에 등록합니다.
        /// 만약 현재 표시되고 있는 popup이 없다면 바로 표시합니다.
        /// </summary>
        private void RegisterCommandToQueue(IPopupCommand command)
        {
            switch (command.CommandType)
            {
                case PopupCommandType.Unique:
                    if (IsPopupShowing && m_currentPopupKey == command.Key || IsPopupInQueue(command)) return;
                    break;
                case PopupCommandType.Duplicate:
                    break;
                case PopupCommandType.CancelIfExist:
                    if (IsPopupShowing || m_popupQueue.Count > 0) return;
                    break;
                default:
                    throw new ArgumentOutOfRangeException($"Not supported PopupCommandType : {command.CommandType}");
            }

            m_popupQueue.Enqueue(command);
            if (m_currentPopupView == null)
            {
                ShowNextPopup();
            }
        }

        /// <summary>
        /// 현재 표시되고 있는 popup을 숨기고 파괴합니다. 이후 대기열에서 다음 popup을 표시합니다.
        /// </summary>
        public void HideCurrentPopup()
        {
            if (m_currentPopupView == null) return;
            m_currentPopupView.StartHide(m_hidePopupFadeDuration);
            ShowNextPopup();
        }

        /// <summary>
        /// 이미 등록된 Popup을 대기열에서 제거합니다. 
        /// </summary>
        private void PopFromQueue()
        {
            if (m_popupQueue.Count == 0)
            {
                DebugWrapper.LogWarning("Popup queue is empty");
                return;
            }

            m_popupQueue.Dequeue();
        }

        /// <summary>
        /// 대기열에서 다음 popup을 인스턴스화하고 표시합니다. 표시되는 popup은 대기열에서 제거됩니다.
        /// </summary>
        private void ShowNextPopup()
        {
            if (m_popupQueue.Count == 0)
            {
                m_currentPopupView = null;
                return;
            }

            var command = m_popupQueue.Dequeue();
            m_currentPopupKey = command.Key;
            CreateAndConfigurePopup(command);
        }

        private void CreateAndConfigurePopup(IPopupCommand command)
        {
            var popup = Instantiate(m_loadedPopups[command.Type], m_popupCanvas.transform);
            m_currentPopupView = popup;

            switch (command)
            {
                case TwoButtonPopupCommand twoButtonPopupCommand:
                    popup.GetComponent<TwoButtonPopup>().ConfigureWithCommand(twoButtonPopupCommand);
                    break;

                case LoadingPopupCommand loadingPopupCommand:
                    popup.GetComponent<LoadingPopup>().ConfigureWithCommand(loadingPopupCommand);
                    break;

                case OneButtonPopupCommand oneButtonPopupCommand:
                    popup.GetComponent<OneButtonPopup>().ConfigureWithCommand(oneButtonPopupCommand);
                    break;
            }

            m_currentPopupView.StartShow(m_popupFadeDuration);
        }
    }
}
