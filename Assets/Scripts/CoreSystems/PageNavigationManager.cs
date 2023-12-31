using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using Minimax.Definitions;
using Minimax.PropertyDrawer;
using Minimax.ScriptableObjects;
using Minimax.UI.Controller;
using Minimax.UI.Controller.ComponentControllers;
using Minimax.UI.View.Pages;
using Minimax.UI.View.Popups;
using UnityEngine;

namespace Minimax.CoreSystems
{
    public class PageNavigationManager : MonoBehaviour
    {
        [SerializeField]
        [ReadOnly]
        private PageNavigation m_currentNavigation;

        [SerializeField]
        private PageNavigationTypeSO m_currentPageNavigationTypeSO;

        [SerializeField]
        [ReadOnly]
        private SerializedDictionary<PageNavigationType, PageNavigation> m_pageNavigations =
            new();

        [SerializeField]
        private BottomUIController m_bottomUIController;

        [Header("Animation Settings")]
        [SerializeField]
        [Range(0, 1)]
        private float m_pageTransitionDuration = 0.15f;

        private List<PageNavigationType> m_navigationTypeOrder = new()
        {
            PageNavigationType.PlayNav,
            PageNavigationType.DeckNav,
            PageNavigationType.StoreNav,
            PageNavigationType.ProfileNav
        };

        private void Awake()
        {
            ResetNavigationCache();
            if (m_currentPageNavigationTypeSO.Value == PageNavigationType.Undefined)
                m_currentPageNavigationTypeSO.Value = m_navigationTypeOrder[0];
            m_bottomUIController.Init(GetIndexOfCurrentNavigationType());
        }

        private int GetIndexOfCurrentNavigationType()
        {
            return m_navigationTypeOrder.IndexOf(m_currentPageNavigationTypeSO.Value);
        }

        private void OnEnable()
        {
            GlobalManagers.Instance.Input.OnBackButton += OnMobileBackButton;
        }

        private void OnDisable()
        {
            GlobalManagers.Instance.Input.OnBackButton -= OnMobileBackButton;
        }

        /// <summary>
        /// 씬에 있는 PageNavigation 컴포넌트를 찾아서 m_pageNavigations에 캐싱한다.
        /// </summary>
        private void ResetNavigationCache()
        {
            m_pageNavigations.Clear();
            var pageNavigations = GameObject.FindGameObjectsWithTag("PageNavigation");
            foreach (var pageNavigation in pageNavigations)
            {
                var pageNav = pageNavigation.GetComponent<PageNavigation>();
                if (pageNav != null && pageNav.NavigationType != PageNavigationType.Undefined)
                    m_pageNavigations.Add(pageNav.NavigationType, pageNav);
            }
        }

        public void SwitchNavigation(int index)
        {
            var navType = m_navigationTypeOrder[index];
            if (m_currentNavigation != null && m_currentNavigation.NavigationType == navType) return;
            CheckIsValidNavType(navType);

            if (m_currentNavigation != null) m_currentNavigation.Hide(m_pageTransitionDuration);
            m_currentNavigation                 = m_pageNavigations[navType];
            m_currentPageNavigationTypeSO.Value = navType;
            m_currentNavigation.Show(m_pageTransitionDuration);
        }

        private void CheckIsValidNavType(PageNavigationType navType)
        {
            UnityEngine.Assertions.Assert.IsTrue(m_pageNavigations.ContainsKey(navType),
                $"PageNavigation {navType} not found in the scene.");
        }

        public PageView PushPage(PageType page)
        {
            return m_currentNavigation.Push(page);
        }

        private void OnMobileBackButton()
        {
            if (!m_currentNavigation.Pop())
            {
                AudioManager.Instance.PlaySFX(AudioLib.Popup);
                PopupManager.Instance.MobileBackButtonPopup(PopupType.ExitAppPopup, PopupCommandType.Unique);
            }
        }
    }
}