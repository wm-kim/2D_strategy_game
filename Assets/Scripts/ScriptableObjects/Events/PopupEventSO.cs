using Minimax.UI.View.Popups;
using UnityEngine;
using UnityEngine.Events;

namespace Minimax
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
                DebugWrapper.Instance.LogWarning("A Popup was requested, but nobody picked it up.");
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
                OnHidePopupRequested.Invoke();
            else
            {
                DebugWrapper.Instance.LogWarning("A Popup was requested, but nobody picked it up.");
            }
        }
    }
}