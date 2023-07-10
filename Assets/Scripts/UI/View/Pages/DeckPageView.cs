using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WMK
{
    public class DeckPageView : PageView
    {
        protected override void SetPageType()
        {
            m_pageType = PageType.DeckPage;
        }
    }
}
