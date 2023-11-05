using AYellowpaper.SerializedCollections;
using Minimax.PropertyDrawer;
using Minimax.UI.View.Pages;
using UnityEngine;

namespace Minimax.CoreSystems
{
    // PageManager needs to be executed before any other script in the scene.
    [DefaultExecutionOrder(-1)]
    public class PageManager : MonoBehaviour
    {
        [SerializeField] [ReadOnly] private SerializedDictionary<PageType, PageView> m_pages = new();

        private void Awake()
        {
            ResetPageCache();
            HideAllPages();
        }

        private void ResetPageCache()
        {
            m_pages.Clear();
            var pages = GameObject.FindGameObjectsWithTag("Page");
            foreach (var page in pages)
            {
                var pageView = page.GetComponent<PageView>();
                pageView.Init();
                if (pageView != null) m_pages.Add(pageView.PageType, pageView);
            }
        }

        private void HideAllPages()
        {
            foreach (var page in m_pages) page.Value.gameObject.GetComponent<PageView>().StartHide();
        }

        public PageView GetPageView(PageType pageType)
        {
            UnityEngine.Assertions.Assert.IsTrue(m_pages.ContainsKey(pageType),
                $"Page {pageType} not found in the scene.");
            return m_pages[pageType];
        }
    }
}