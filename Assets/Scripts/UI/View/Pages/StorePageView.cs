using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WMK
{
    public class StorePageView : PageView
    {
        protected override void SetPageType()
        {
            m_pageType = PageType.StorePage;
        }
    }
}
