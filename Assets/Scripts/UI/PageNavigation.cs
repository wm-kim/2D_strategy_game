using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace WMK
{
    public enum PageNavigationType
    {
        Undefined = -1,
        PlayNav,
        DeckNav,
        StoreNav,
        ProfileNav,
    }
    
    public class PageNavigation : MonoBehaviour
    {
        [SerializeField] private PageManager m_pageManager;
        [field: SerializeField] public PageNavigationType NavigationType { get; private set; } = PageNavigationType.Undefined;
        [field: SerializeField] public PageType InitialPage { get; private set; } = PageType.Undefined;
        
        private Stack<PageType> m_history = new Stack<PageType>();
        
        private void Awake()
        {
            SetTagIfNotSet();
            CheckIfNavigationTypeIsSet();
            SetInitialPage();
            CheckIfInitialPageIsSet();
            Hide();
        }
        
        private void SetTagIfNotSet()
        {
            if (gameObject.CompareTag("PageNavigation")) return;
#if UNITY_EDITOR
            Debug.LogWarning("PageNavigation object does not have the \"PageNavigation\" tag. Setting it now.");
#endif
            gameObject.tag = "PageNavigation";
        }
        
        private void CheckIfNavigationTypeIsSet()
        {
            UnityEngine.Assertions.Assert.IsTrue(NavigationType != PageNavigationType.Undefined, 
                $"PageNavigationType is undefined in {gameObject.name}, please set it properly in inspector");
        }
        
        private void CheckIfInitialPageIsSet()
        {
            UnityEngine.Assertions.Assert.IsTrue(InitialPage != PageType.Undefined, 
                $"InitialPage is undefined in {gameObject.name}, please set it properly in inspector");
        }
        
        private void SetInitialPage() => m_history.Push(InitialPage);
        
        public PageView Push(PageType page)
        {
            var view = m_pageManager.GetPageView(page);
            // Hide the currently active view (if any)
            if (m_history.Count > 0)
            {
                PageView currentView = m_pageManager.GetPageView(m_history.Peek());
                currentView.Hide();
            }
            // Show the new view
            view.Show();
            m_history.Push(page);
            return view;
        }

        public void Pop()
        {   
            if (m_history.Count > 1)
            {
                // Hide the current view
                PageView currentView = m_pageManager.GetPageView(m_history.Pop());
                currentView.Hide();
                PageView previousView = m_pageManager.GetPageView(m_history.Peek());
                previousView.Show();
            }
            else
            {
                Debug.LogWarning("Cannot pop a view from the navigation stack. There is only initial view left.");
            }
        }
        
        public void Hide(float duration = 0.0f)
        {
            PageView currentView = m_pageManager.GetPageView(m_history.Peek());
            currentView.Hide(duration);
        }
        
        public void Show(float duration = 0.0f)
        {
            PageView currentView = m_pageManager.GetPageView(m_history.Peek());
            currentView.Show(duration);
        }
    }
}
