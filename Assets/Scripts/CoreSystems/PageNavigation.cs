using System.Collections.Generic;
using Minimax.ScriptableObjects;
using Minimax.UI.View.Pages;
using UnityEngine;
using Utilities;

namespace Minimax.CoreSystems
{
    public enum PageNavigationType
    {
        Undefined = -1,
        PlayNav,
        DeckNav,
        StoreNav,
        ProfileNav
    }

    public class PageNavigation : MonoBehaviour
    {
        [SerializeField] private PageManager m_pageManager;

        [field: SerializeField]
        public PageNavigationType NavigationType { get; private set; } = PageNavigationType.Undefined;

        [field: SerializeField] public PageType InitialPage { get; private set; } = PageType.Undefined;

        [SerializeField] private PageStackSO     m_pageStackSO;
        private                  Stack<PageType> m_pageStack => m_pageStackSO.PageStack;

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
            DebugWrapper.LogWarning("PageNavigation object does not have the \"PageNavigation\" tag. Setting it now.");
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

        private void SetInitialPage()
        {
            if (m_pageStack.Count == 0) Push(InitialPage);
        }

        // Don't use this method directly. Use the PushPage method of PageNavigationManager instead.
        public PageView Push(PageType page)
        {
            var view = m_pageManager.GetPageView(page);

            // Hide the currently active view (if any)
            if (m_pageStack.Count > 0)
            {
                var currentView = m_pageManager.GetPageView(m_pageStack.Peek());
                currentView.StartHide();
            }

            // Show the new view
            view.StartShow();
            m_pageStack.Push(page);
            return view;
        }

        // Don't use this method directly. Use the PopPage method of PageNavigationManager instead.
        public bool Pop()
        {
            if (m_pageStack.Count > 1)
            {
                // Hide the current view
                var currentView = m_pageManager.GetPageView(m_pageStack.Pop());
                currentView.StartHide();
                var previousView = m_pageManager.GetPageView(m_pageStack.Peek());
                previousView.StartShow();
                return true;
            }

            return false;
        }

        public void Hide(float duration = 0.0f)
        {
            var currentView = m_pageManager.GetPageView(m_pageStack.Peek());
            currentView.StartHide(duration);
        }

        public void Show(float duration = 0.0f)
        {
            var currentView = m_pageManager.GetPageView(m_pageStack.Peek());
            currentView.StartShow(duration);
        }
    }
}