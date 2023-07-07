using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WMK
{
    public class UINavigation : MonoBehaviour
    {
        private static Stack<PageView> s_history = new Stack<PageView>();
        
        public static PageView Push<T>() where T : PageView => Push(PageView.Get<T>());
        
        public static PageView Push(PageView view)
        {
            if (view != null)
            {
                // Hide the currently active view (if any)
                if (s_history.Count > 0)
                {
                    PageView currentView = s_history.Peek();
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
                PageView currentView = s_history.Pop();
                currentView.Hide();

                // Show the previous view (if any)
                if (s_history.Count > 0)
                {
                    PageView previousView = s_history.Peek();
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
