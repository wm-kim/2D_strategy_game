using Minimax.UI.Model;
using UnityEngine;

namespace Minimax.UI.View.Pages
{
    public class MainPageView : PageView
    {
        [SerializeField]
        private MenuPageSO m_model;

        protected override void SetPageType()
        {
            m_pageType = PageType.MainPage;
        }

        private void Awake()
        {
            m_model.GameTitle.Value   = m_model.GameTitle.Value;
            m_model.GameVersion.Value = "Version : " + m_model.GameVersion.Value;
        }
    }
}