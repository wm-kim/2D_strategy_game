using System;
using System.Collections.Generic;
using Minimax.Definitions;
using Minimax.PropertyDrawer;
using Minimax.UI.View;
using Minimax.UI.View.Popups;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Assertions;
using Utilities;
using Debug = Utilities.Debug;

namespace Minimax.CoreSystems
{
    public class PopupManager : MonoBehaviour
    {
        public static PopupManager Instance { get; private set; }

        [Header("Addressable")] [SerializeField]
        private AssetLabelReference m_popupAssetLabel;

        [SerializeField] private AssetLabelReference m_commonPopupAssetLabel;

        [Header("References")] [SerializeField]
        private Canvas m_popupCanvas;

        [SerializeField] private UIFader m_popupBackgroundFader;

        // popup의 대기열을 관리하는 Queue, First come first serve로 popup을 표시한다.
        private List<IPopupCommand> m_popupQueue = new();

        // 현재 표시되고 있는 popup
        [SerializeField] [ReadOnly] private PopupView     m_currentPopupView     = null;
        private                             string        m_currentPopupKey      = string.Empty;
        private                             PopupPriority m_currentPopupPriority = PopupPriority.Low;

        [Header("Settings")] [SerializeField] [Range(0.0f, 1f)]
        private float m_fadeInDuration = 0.2f;

        [SerializeField] [Range(0.0f, 1f)] private float m_fadeOutDuration = 0.2f;

        [Space(10f)] [SerializeField] [ReadOnly]
        private AYellowpaper.SerializedCollections.SerializedDictionary<PopupType, PopupView> m_loadedPopups = new();

        private void Awake()
        {
            Instance = this;

            Assert.IsTrue(m_popupAssetLabel.labelString != Define.CommonPopupAssetLabel);
            Assert.IsTrue(m_commonPopupAssetLabel.labelString == Define.CommonPopupAssetLabel);

            PreLoadPopups();
        }

        private void OnDestroy()
        {
            Instance = null;
        }

        private void PreLoadPopups()
        {
            if (m_popupAssetLabel != null && !string.IsNullOrEmpty(m_popupAssetLabel.labelString))
                Addressables.LoadAssetsAsync<GameObject>(m_popupAssetLabel, null).Completed += handle =>
                {
                    foreach (var popup in handle.Result)
                    {
                        var popupView = popup.GetComponent<PopupView>();
                        popupView.SetPopupTypeAndCheck();
                        m_loadedPopups.Add(popupView.Type, popupView);
                    }
                };

            Addressables.LoadAssetsAsync<GameObject>(m_commonPopupAssetLabel, null).Completed += handle =>
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
                if (popup.Key == command.Key)
                    return true;

            return false;
        }

        /// <summary>
        /// Popup을 표시합니다. PopupType이 queue안에서 중복되는지 확인하기 위한 key가 됩니다.
        /// </summary>
        public void RegisterPopupToQueue(PopupType key, PopupCommandType commandType = PopupCommandType.Duplicate,
            PopupPriority priority = PopupPriority.Low)
        {
            RegisterCommandToQueue(new DefaultPopupCommand(key, commandType, priority));
        }

        /// <param name="key">queue안에서 중복되는지 확인하기 위한 식별자입니다.</param>
        public void RegisterTwoButtonPopupToQueue(string key, string message, string leftButtonText,
            string rightButtonText,
            Action leftButtonAction, Action rightButtonAction,
            PopupCommandType commandType = PopupCommandType.Duplicate,
            PopupPriority priority = PopupPriority.Low
        )
        {
            RegisterCommandToQueue(new TwoButtonPopupCommand(key, message, leftButtonText, rightButtonText,
                leftButtonAction, rightButtonAction, commandType, priority));
        }

        /// <param name="key">queue안에서 중복되는지 확인하기 위한 식별자입니다.</param>
        public void RegisterOneButtonPopupToQueue(string key, string message, string buttonText, Action buttonAction,
            PopupCommandType commandType = PopupCommandType.Duplicate, PopupPriority priority = PopupPriority.Low)
        {
            RegisterCommandToQueue(new OneButtonPopupCommand(key, message, buttonText, buttonAction, commandType,
                priority));
        }

        /// <param name="key">queue안에서 중복되는지 확인하기 위한 식별자입니다.</param>
        public void MobileBackTwoButtonPopup(string key, string message, string leftButtonText, string rightButtonText,
            Action leftButtonAction, Action rightButtonAction,
            PopupCommandType commandType = PopupCommandType.Duplicate)
        {
            MobileBackCommand(new TwoButtonPopupCommand(key, message, leftButtonText, rightButtonText,
                leftButtonAction, rightButtonAction, commandType));
        }

        /// <param name="key">queue안에서 중복되는지 확인하기 위한 식별자입니다.</param>
        public void MobileBackButtonPopup(PopupType key, PopupCommandType commandType = PopupCommandType.Duplicate,
            PopupPriority priority = PopupPriority.Low)
        {
            MobileBackCommand(new DefaultPopupCommand(key, commandType, priority));
        }

        /// <param name="key">queue안에서 중복되는지 확인하기 위한 식별자입니다.</param>
        public void RegisterLoadingPopupToQueue(string key, string message,
            PopupCommandType commandType = PopupCommandType.Duplicate, PopupPriority priority = PopupPriority.Low)
        {
            RegisterCommandToQueue(new LoadingPopupCommand(key, message, commandType, priority));
        }

        /// <summary>
        /// 모바일에서 뒤로가기 버튼을 누르면 호출됩니다.
        /// 이미 표시되고 있는 popup이 있다면 숨기고, 대기열에 더 이상 숨길 popup이 없다면 인자로 받은 Popup을 표시합니다.
        /// </summary>
        private void MobileBackCommand(IPopupCommand command)
        {
            if (IsPopupShowing)
            {
                if (m_currentPopupView as LoadingPopup)
                {
                    Debug.LogWarning("Loading popup is showing. Cannot hide loading popup.");
                    return;
                }

                HideCurrentPopup();
            }
            else
            {
                RegisterCommandToQueue(command);
            }
        }

        /// <summary>
        /// PopupCommand Type에 따라 Popup Command를 대기열에 등록합니다.
        /// 만약 현재 표시되고 있는 popup이 없다면 바로 표시합니다.
        /// </summary>
        private void RegisterCommandToQueue(IPopupCommand command)
        {
            Debug.Log($"Registering popup to queue. Key : {command.Key}, Type : {command.Type}, " +
                             $"CommandType : {command.CommandType}, Priority : {command.Priority}");

            switch (command.CommandType)
            {
                case PopupCommandType.Unique:
                    if ((IsPopupShowing && m_currentPopupKey == command.Key) || IsPopupInQueue(command)) return;
                    break;
                case PopupCommandType.Duplicate:
                    break;
                default:
                    throw new ArgumentOutOfRangeException($"Not supported PopupCommandType : {command.CommandType}");
            }


            // 우선순위에 따라 새로운 팝업의 위치를 찾아 대기열에 삽입합니다. 
            var insertIndex = m_popupQueue.Count; // 기본값은 맨 뒤
            for (var i = 0; i < m_popupQueue.Count; i++)
                if (m_popupQueue[i].Priority < command.Priority)
                {
                    insertIndex = i;
                    break;
                }

            m_popupQueue.Insert(insertIndex, command);

            // 현재 표시된 팝업이 있고 새로운 팝업의 우선순위가 더 높으면 현재 팝업을 숨깁니다.
            if (IsPopupShowing && command.Priority > m_currentPopupPriority) HideCurrentPopup();

            if (!IsPopupShowing) ShowNextPopup();
        }

        /// <summary>
        /// 현재 표시되고 있는 popup을 숨기고 파괴합니다. 이후 대기열에서 다음 popup을 표시합니다.
        /// if popupKey is null, hide current popup
        /// </summary>
        public void HideCurrentPopup(string popupKey = null)
        {
            if (m_currentPopupView == null) return;

            if (!string.IsNullOrEmpty(popupKey) && m_currentPopupKey != popupKey)
            {
                Debug.LogWarning($"Current popup key is not matched with popup key to hide." +
                                        $" Current : {m_currentPopupKey}, Hide : {popupKey}");
                return;
            }

            m_popupBackgroundFader.StartHide(m_fadeOutDuration);
            m_currentPopupView.StartHide(m_fadeOutDuration);

            m_currentPopupView     = null;
            m_currentPopupKey      = null;
            m_currentPopupPriority = PopupPriority.Low;

            ShowNextPopup();
        }

        /// <summary>
        /// 대기열에서 다음 popup을 인스턴스화하고 표시합니다. 표시되는 popup은 대기열에서 제거됩니다.
        /// </summary>
        private void ShowNextPopup()
        {
            if (m_popupQueue.Count == 0)
            {
                Debug.Log("No more popup to show.");
                return;
            }

            var command = m_popupQueue[0];
            m_popupQueue.RemoveAt(0);

            m_currentPopupKey      = command.Key;
            m_currentPopupPriority = command.Priority;
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

            m_popupBackgroundFader.StartShow(m_fadeInDuration);
            m_currentPopupView.StartShow(m_fadeInDuration);
        }
    }
}