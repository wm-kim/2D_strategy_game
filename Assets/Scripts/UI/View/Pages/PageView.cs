using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using DG.Tweening;

namespace WMK
{
    public enum UIVisibleState
    {
        Undefined = -1,
        Appearing,
        Appeared,
        Disappearing,
        Disappeared
    }
    
    [RequireComponent(typeof(CanvasGroup))]
    public abstract class PageView : MonoBehaviour
    {
        [SerializeField, ReadOnly] protected PageType m_pageType = PageType.Undefined;
        
        public PageType PageType => m_pageType;
        [SerializeField, ReadOnly] private UIVisibleState m_currentState = UIVisibleState.Undefined;
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
            DebugStatic.LogWarning($"gameObject {gameObject.name} does not have tag Page, setting it now.");
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

        public void Show(float duration = 0.0f) 
        {
            if (m_currentState == UIVisibleState.Appearing || m_currentState == UIVisibleState.Appeared) return;

            m_currentState = UIVisibleState.Appearing;
            gameObject.SetActive(true);
            
            // Ensure duration is not negative
            duration = Mathf.Max(duration, 0.0f);
            
            // reducing DoTween call overhead
            if (duration == 0)
            {
                m_canvasGroup.alpha = 1;
                m_canvasGroup.blocksRaycasts = true;
                m_currentState = UIVisibleState.Appeared; 
            }
            else
            {
                m_canvasGroup.alpha = 0;
                m_canvasGroup.blocksRaycasts = false;
                if (m_tween != null) m_tween.Kill();
                m_tween = m_canvasGroup.DOFade(1, duration).OnComplete(() =>
                {
                    m_currentState = UIVisibleState.Appeared; 
                    m_canvasGroup.blocksRaycasts = true;
                });
            }
        }

        public void Hide(float duration = 0.0f)
        {
            if (m_currentState == UIVisibleState.Disappearing || m_currentState == UIVisibleState.Disappeared)
                return;

            m_currentState = UIVisibleState.Disappearing;
            m_canvasGroup.blocksRaycasts = false;
            
            // Ensure duration is not negative
            duration = Mathf.Max(duration, 0.0f);

            // reducing DoTween call overhead
            if (duration == 0)
            {
                m_canvasGroup.alpha = 0;
                gameObject.SetActive(false);
                m_currentState = UIVisibleState.Disappeared;
            }
            else
            {
                if (m_tween != null) m_tween.Kill();
                m_tween = m_canvasGroup.DOFade(0f, duration).OnComplete(() =>
                {
                    gameObject.SetActive(false);
                    m_currentState = UIVisibleState.Disappeared;
                });
            }
        }
    }
}
