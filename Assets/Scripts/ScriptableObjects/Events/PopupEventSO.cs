using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace WMK
{
    /// <summary>
    /// This is a ScriptableObject that can be used to raise PopupEvents.
    /// </summary>
    [CreateAssetMenu(menuName = "ScriptableObjects/Events/PopupEvent")]
    public class PopupEventSO : ScriptableObject
    {
        public UnityEvent<PopupType> OnShowPopupRequested;
        public UnityEvent OnHidePopupRequested;
        
        public void ShowPopup(PopupType popupType)
        {
            if (OnShowPopupRequested != null)
                OnShowPopupRequested.Invoke(popupType);
            else
            {
                Debug.LogWarning("A Popup was requested, but nobody picked it up.");
            }
        }
        
        public void HidePopup()
        {
            if (OnHidePopupRequested != null)
                OnHidePopupRequested.Invoke();
            else
            {
                Debug.LogWarning("A Popup was requested, but nobody picked it up.");
            }
        }
    }
}