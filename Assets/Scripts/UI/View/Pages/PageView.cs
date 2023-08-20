using DG.Tweening;
using Minimax.Utilities;
using UnityEngine;

namespace Minimax.UI.View.Pages
{
    [RequireComponent(typeof(CanvasGroup))]
    public abstract class PageView : StatefulUIView
    {
        [SerializeField, ReadOnly] protected PageType m_pageType = PageType.Undefined;
        
        public PageType PageType => m_pageType;
        private CanvasGroup m_canvasGroup;
        private Tween m_tween;

        /// <summary>
        /// Init this page. This is called by the PageManager when the page is created.
        /// </summary>
        public void Init()
        {
            SetTagIfNotSet();
            SetPageType();
            CheckIfPageTypeIsSet();
            InitPageView();
        }

        /// <summary>
        /// Set member m_pageType for this page. This is used by the PageManager to cache the page.
        /// </summary>
        protected abstract void SetPageType();
        
        private void SetTagIfNotSet()
        {
            if (gameObject.CompareTag("Page")) return;
            DebugWrapper.LogWarning($"gameObject {gameObject.name} does not have tag Page, setting it now.");
            gameObject.tag = "Page";
        }
        
        private void CheckIfPageTypeIsSet()
        {
            UnityEngine.Assertions.Assert.IsTrue(m_pageType != PageType.Undefined, 
                $"PageType is undefined in {gameObject.name}, please set it properly in SetPageType()");
        }
        
        private void InitPageView()
        {
            m_canvasGroup = GetComponent<CanvasGroup>();
            gameObject.transform.localPosition = Vector3.zero;
        }

        protected override void Show(float transitionDuration = 0.0f)
        {
            gameObject.SetActive(true);
            
            // Ensure duration is not negative
            transitionDuration = Mathf.Max(transitionDuration, 0.0f);
            
            // reducing DoTween call overhead
            if (transitionDuration == 0)
            {
                m_canvasGroup.alpha = 1;
                m_canvasGroup.blocksRaycasts = true;
                SetAppearedState();
            }
            else
            {
                m_canvasGroup.alpha = 0;
                m_canvasGroup.blocksRaycasts = false;
                if (m_tween != null) m_tween.Kill();
                m_tween = m_canvasGroup.DOFade(1, transitionDuration).OnComplete(() =>
                {
                    SetAppearedState();
                    m_canvasGroup.blocksRaycasts = true;
                });
            }
        }

        protected override void Hide(float transitionDuration = 0.0f)
        {
            m_canvasGroup.blocksRaycasts = false;
            
            // Ensure duration is not negative
            transitionDuration = Mathf.Max(transitionDuration, 0.0f);

            // reducing DoTween call overhead
            if (transitionDuration == 0)
            {
                m_canvasGroup.alpha = 0;
                gameObject.SetActive(false);
                SetDisappearedState();
            }
            else
            {
                if (m_tween != null) m_tween.Kill();
                m_tween = m_canvasGroup.DOFade(0f, transitionDuration).OnComplete(() =>
                {
                    gameObject.SetActive(false);
                    SetDisappearedState();
                });
            }
        }
    }
}
