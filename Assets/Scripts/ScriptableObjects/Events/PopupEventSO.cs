using Minimax.UI.View.Popups;
using Minimax.Utilities;
using UnityEngine;
using UnityEngine.Events;

namespace Minimax.ScriptableObjects.Events
{
    /// <summary>
    /// This is a ScriptableObject that can be used for Showing Popup Events.
    /// </summary>
    [CreateAssetMenu(menuName = "ScriptableObjects/Events/PopupEvent")]
    public class PopupEventSO : ScriptableObject
    {
        public UnityEvent<PopupType> OnShowPopupRequested;
        public UnityEvent OnHidePopupRequested;
        public UnityEvent<PopupType> OnMobileBackButtonPressed;
        
        public void ShowPopup(PopupType popupType)
        {
            if (OnShowPopupRequested != null)
            {
                DebugWrapper.Log("Popup Show Event was requested.");
                OnShowPopupRequested.Invoke(popupType);
            }
            else
            {
                DebugWrapper.LogWarning("A Popup Show Event was requested, but nobody picked it up.");
            }
        }

        // This is a workaround for the fact that UnityEvents cannot take enums as parameters.
        public void ShowPopup(PopupEventSelector popupEventSelector)
        {
            var popupType = popupEventSelector.PopupType;
            ShowPopup(popupType);
        }
        
        public void HidePopup()
        {
            if (OnHidePopupRequested != null)
            {
                DebugWrapper.Log("Popup Hide Event was requested.");
                OnHidePopupRequested.Invoke();
            }
            else
            {
                DebugWrapper.LogWarning("A Popup Hide Event was requested, but nobody picked it up.");
            }
        }
        
        
        public void MobileBackButtonPressed(PopupType popupType)
        {
            if (OnMobileBackButtonPressed != null)
            {
                DebugWrapper.Log("Mobile Back Button Pressed Event was requested.");
                OnMobileBackButtonPressed.Invoke(popupType);
            }
            else
            {
                DebugWrapper.LogWarning("A Mobile Back Button Pressed Event was requested, but nobody picked it up.");
            }
        }
    }
}