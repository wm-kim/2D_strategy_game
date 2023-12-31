using System;
using DG.Tweening;
using UnityEngine;

namespace Minimax.UI.View
{
    public class UIFader : StatefulUIView
    {
        [SerializeField]
        private CanvasGroup m_canvasGroup = null;

        private Tween m_fadeTween = null;

        public event Action OnFadeInComplete;
        public event Action OnFadeOutComplete;

        private void Awake()
        {
            m_canvasGroup.alpha = 0f;
            m_canvasGroup.gameObject.SetActive(false);
        }

        protected override void Show(float transitionDuration = 0)
        {
            if (m_fadeTween != null && m_fadeTween.IsActive()) m_fadeTween.Kill();

            m_canvasGroup.gameObject.SetActive(true);
            m_fadeTween = m_canvasGroup.DOFade(1f, transitionDuration).OnComplete(
                () =>
                {
                    SetAppearedState();
                    OnFadeInComplete?.Invoke();
                });
        }

        protected override void Hide(float transitionDuration = 0)
        {
            if (m_fadeTween != null && m_fadeTween.IsActive()) m_fadeTween.Kill();

            m_fadeTween = m_canvasGroup.DOFade(0f, transitionDuration).OnComplete(
                () =>
                {
                    m_canvasGroup.gameObject.SetActive(false);
                    SetDisappearedState();
                    OnFadeOutComplete?.Invoke();
                });
        }
    }
}