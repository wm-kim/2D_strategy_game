using System;
using System.Collections;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Serialization;

namespace WMK
{
    public class PopupManager : MonoBehaviour
    {
        [SerializeField] private AssetLabelReference m_popupAssetLabelReference;
        [SerializeField] private Canvas m_popupCanvas;
        // popup의 대기열을 관리하는 Queue, First come first serve로 popup을 표시한다.
        private Queue<PopupView> m_popupQueue = new Queue<PopupView>();
        // 현재 표시되고 있는 popup
        [SerializeField, ReadOnly] private PopupView m_currentPopupView = null;
        
        [Header("Listening to")]
        [SerializeField] private PopupEventSO m_popupEventSO;

        [Space(10f)]
        // Popup pool
        [SerializeField, ReadOnly] private SerializedDictionary<PopupType, PopupView> m_loadedPopups = new SerializedDictionary<PopupType, PopupView>();
       
        private void Awake()
        {
            PreLoadPopup();
        }
        
        private void OnEnable()
        {
            m_popupEventSO.OnShowPopupRequested.AddListener(RegisterToQueue);
            m_popupEventSO.OnHidePopupRequested.AddListener(HideCurrentPopup);
        }
        
        private void OnDisable()
        {
            m_popupEventSO.OnShowPopupRequested.RemoveListener(RegisterToQueue);
            m_popupEventSO.OnHidePopupRequested.RemoveListener(HideCurrentPopup);
        }

        private void PreLoadPopup()
        {
            Addressables.LoadAssetsAsync<GameObject>(m_popupAssetLabelReference, null).Completed += handle =>
            {
                foreach (var popup in handle.Result)
                {
                    var popupView = popup.GetComponent<PopupView>();
                    m_loadedPopups.Add(popupView.Type, popupView);
                }
            };
        }
        
        /// <summary>
        /// Popup을 대기열에 등록합니다. 만약 현재 표시되고 있는 popup이 없다면 바로 표시합니다.
        /// </summary>
        private void RegisterToQueue(PopupType popupType)
        {
            m_popupQueue.Enqueue(m_loadedPopups[popupType]);
            if (m_currentPopupView == null)
            {
                ShowNextPopup();
            }
        }
        
        /// <summary>
        /// 현재 표시되고 있는 popup을 숨기고 파괴합니다. 이후 대기열에서 다음 popup을 표시합니다.
        /// </summary>
        private void HideCurrentPopup()
        {
            if (m_currentPopupView == null) return;
            m_currentPopupView.Hide();
            // Destroy popup
            Destroy(m_currentPopupView.gameObject);
            ShowNextPopup();
        }
        
        /// <summary>
        /// 이미 등록된 Popup을 대기열에서 제거합니다. 
        /// </summary>
        private void PopFromQueue()
        {
            if (m_popupQueue.Count == 0)
            {
                DebugStatic.LogWarning("Popup queue is empty");
                return;
            }
            
            m_popupQueue.Dequeue();
        }

        /// <summary>
        /// 대기열에서 다음 popup을 인스턴스화하고 표시합니다. 표시되는 popup은 대기열에서 제거됩니다.
        /// </summary>
        private void ShowNextPopup()
        {
            if (m_popupQueue.Count == 0) return;
            m_currentPopupView = m_popupQueue.Dequeue();
            // Instantiate popup
            var popup = Instantiate(m_currentPopupView, m_popupCanvas.transform);
            m_currentPopupView = popup;
            popup.Show();
        }
    }
}
