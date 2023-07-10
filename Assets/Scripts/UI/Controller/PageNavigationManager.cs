using System;
using AYellowpaper.SerializedCollections;
using UnityEngine;
using UnityEngine.Serialization;

namespace WMK
{
    public class PageNavigationManager : MonoBehaviour
    {
        [SerializeField, ReadOnly] private PageNavigation m_curNavigation;
        [SerializeField, ReadOnly] private SerializedDictionary<PageNavigationType, PageNavigation> m_pageNavigations = 
            new SerializedDictionary<PageNavigationType, PageNavigation>();
        [SerializeField, Range(0, 1)] private float m_transitionDuration = 0.15f;

        private void Awake()
        {
            ResetNavigationCache();
        }

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

        public void SwitchNavigation(SwitchNavigationComponent navTypeComponent)
        {
            var navType = navTypeComponent.pageNavigationType;
            if (m_curNavigation != null && m_curNavigation.NavigationType == navType) return;
            UnityEngine.Assertions.Assert.IsTrue(m_pageNavigations.ContainsKey(navType), $"PageNavigation {navType} not found in the scene.");
            if (m_curNavigation != null) m_curNavigation.Hide(m_transitionDuration);
            m_curNavigation = m_pageNavigations[navType];
            m_curNavigation.Show(m_transitionDuration);
        }
    }
}
