using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Minimax.UI.View;
using UnityEngine;

namespace Minimax
{
    public class UIFader : StatefulUIView
    {
        [SerializeField] private CanvasGroup m_canvasGroup = null;
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
