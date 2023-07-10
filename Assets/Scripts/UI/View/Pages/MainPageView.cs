using System;
using System.Threading.Tasks;
using UnityEngine;

namespace WMK
{
    public class MainPageView : PageView
    {
        [SerializeField] private DataString m_gameTitle;
        [SerializeField] private DataString m_gameVersion;

        protected override void SetPageType()
        {
            m_pageType = PageType.MainPage;
        }

        private void Start()
        {
            SetupData();
        }

        public void SetupData()
        {
            m_gameTitle.Value = GameSettings.Instance.GameTitle;
            m_gameVersion.Value = GameSettings.Instance.GameVersion;
        }
    }
}