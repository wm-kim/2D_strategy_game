using System;
using AYellowpaper.SerializedCollections;
using Cysharp.Threading.Tasks;
using Minimax.ScriptableObjects.Events;
using Minimax.ScriptableObjects.Events.Primitives;
using Minimax.UI.View.Pages;
using Minimax.UI.View.Popups;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace Minimax.CoreSystems
{
    public class PageNavigationManager : MonoBehaviour
    {
        [SerializeField, ReadOnly] private PageNavigation m_currentNavigation;
        [SerializeField, ReadOnly] private SerializedDictionary<PageNavigationType, PageNavigation> m_pageNavigations = 
            new SerializedDictionary<PageNavigationType, PageNavigation>();
        [SerializeField, Range(0, 1)] private float m_pageTransitionDuration = 0.15f;
        
        private void Awake()
        {
            ResetNavigationCache();
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
                {
                    m_pageNavigations.Add(pageNav.NavigationType, pageNav);  
                }
            }
        }

        public void SwitchNavigation(int index)
        {
            var navType = (PageNavigationType) index;
            if (m_currentNavigation != null && m_currentNavigation.NavigationType == navType) return;
            UnityEngine.Assertions.Assert.IsTrue(m_pageNavigations.ContainsKey(navType), $"PageNavigation {navType} not found in the scene.");
            if (m_currentNavigation != null) m_currentNavigation.Hide(m_pageTransitionDuration);
            m_currentNavigation = m_pageNavigations[navType];
            m_currentNavigation.Show(m_pageTransitionDuration);
        }
        
        public PageView PushPage(PageType page) => m_currentNavigation.Push(page);

        private void OnMobileBackButton()
        {
            if (!m_currentNavigation.Pop())
            {
                GlobalManagers.Instance.Popup.MobileBackButtonDefaultPopup(PopupType.QuitAppPopup);
            }
        }
    }
}
