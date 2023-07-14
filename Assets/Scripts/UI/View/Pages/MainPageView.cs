using System;
using System.Threading.Tasks;
using UnityEngine;

namespace WMK
{
    public class MainPageView : PageView
    {
        [SerializeField] private MenuPageSO m_model;
        
        protected override void SetPageType()
        {
            m_pageType = PageType.MainPage;
        }

        private void Awake()
        {
            m_model.GameTitle.Value = "Game Title : " + m_model.GameTitle.Value;
            m_model.GameVersion.Value = "Version : " + m_model.GameVersion.Value;
        }
    }
}