using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WMK
{
    public class UINavigation : MonoBehaviour
    {
        private static Stack<UIView> s_history = new Stack<UIView>();
        
        public static UIView Push<T>() where T : UIView => Push(UIView.Get<T>());
        
        public static UIView Push(UIView view)
        {
            if (view != null)
            {
                // Hide the currently active view (if any)
                if (s_history.Count > 0)
                {
                    UIView currentView = s_history.Peek();
                    currentView.Hide();
                }

                // Show the new view
                view.Show();
                s_history.Push(view);
            }
            else
            {
                Debug.LogError("Cannot push a null view to the navigation stack.");
            }

            return view;
        }

        public static void Pop()
        {   
            if (s_history.Count > 0)
            {
                // Hide the current view
                UIView currentView = s_history.Pop();
                currentView.Hide();

                // Show the previous view (if any)
                if (s_history.Count > 0)
                {
                    UIView previousView = s_history.Peek();
                    previousView.Show();
                }
            }
            else
            {
                Debug.LogWarning("Cannot pop a view from an empty navigation stack.");
            }
        }
    }
}
