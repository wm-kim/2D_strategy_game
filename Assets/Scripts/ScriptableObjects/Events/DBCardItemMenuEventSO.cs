using UnityEngine;
using UnityEngine.Events;

namespace Minimax.ScriptableObjects.Events
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Events/DBCardItemMenuEvent")]
    public class DBCardItemMenuEventSO : ScriptableObject
    {
        public UnityEvent<DBCardItemView, bool> OnEventRaised;
        
        /// <summary>
        /// Show or hide the DB card item menu.
        /// </summary>
        /// <param name="dbCardItem">The DB card item to show or hide the menu for.</param>
        /// <param name="showOrHide">if true, show the menu. If false, hide the menu.</param>
        public void RaiseEvent(DBCardItemView dbCardItem, bool showOrHide = true)
        {
            if (OnEventRaised != null)
                OnEventRaised.Invoke(dbCardItem, showOrHide);
            else
            {
                Debug.LogWarning("A DBCardItemMenu event was requested, but nobody picked it up.");
            }
        }
    }
}