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
        [Header("Addressable")]
        [SerializeField] private AssetLabelReference m_popupAssetLabelReference;
        [SerializeField, ReadOnly] private string m_path = "Assets/Prefabs/UIPopups/";
        
        [SerializeField] private Canvas m_popupCanvas;
        // popup의 대기열을 관리하는 Queue, First come first serve로 popup을 표시한다.
        private Queue<IPopupCommand> m_popupQueue = new Queue<IPopupCommand>();
        // 현재 표시되고 있는 popup
        [SerializeField, ReadOnly] private PopupView m_currentPopupView = null;
        
        [Space(10f)]
        // Popup pool
        [SerializeField, ReadOnly] private SerializedDictionary<PopupType, PopupView> m_loadedPopups = new SerializedDictionary<PopupType, PopupView>();
       
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

        public void RegisterDefaultPopupToQueue(PopupType popupType) => RegisterCommandToQueue(new DefaultPopupCommand(popupType));
        
        /// <summary>
        /// Popup Command를 대기열에 등록합니다. 만약 현재 표시되고 있는 popup이 없다면 바로 표시합니다.
        /// </summary>
        private void RegisterCommandToQueue(IPopupCommand command)
        {
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
            m_currentPopupView.Hide();
            // Destroy popup
            Addressables.ReleaseInstance(m_currentPopupView.gameObject);
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
        
        public void MobileBackButtonDefaultPopup(PopupType popupType) => MobileBackButtonCommand(new DefaultPopupCommand(popupType));
        
        /// <summary>
        /// 모바일에서 뒤로가기 버튼을 누르면 호출됩니다.
        /// 이미 표시되고 있는 popup이 있다면 숨기고, 없다면 대기열에 인자로 받은 popup을 등록합니다.
        /// </summary>
        private void MobileBackButtonCommand(IPopupCommand command)
        {
            if (IsPopupShowing) HideCurrentPopup();
            else RegisterCommandToQueue(command);
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
            // Instantiate popup
            var instantiateHandle = Addressables.InstantiateAsync(m_path + command.Type + ".prefab", m_popupCanvas.transform);
            
            // Execute different actions according to the type of popup command
            switch (command)
            {
                case DefaultPopupCommand defaultPopupCommand:
                    instantiateHandle.Completed += handle =>
                    {
                        var popup = handle.Result.GetComponent<PopupView>();
                        popup.Show();
                        m_currentPopupView = popup;
                    };
                    break;
               
                default:
                    throw new ArgumentOutOfRangeException(nameof(command), command, "Unknown popup command");
            }
        }
    }
}
