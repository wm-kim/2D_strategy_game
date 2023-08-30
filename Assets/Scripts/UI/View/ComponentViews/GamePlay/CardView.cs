using System;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

namespace Minimax.UI.View.ComponentViews.GamePlay
{
    public class CardView : MonoBehaviour
    {
        public Tween PosTween { get; set; }
        public Tween RotTween { get; set; }
        public Tween ScaleTween { get; set; }
        public Tween FadeTween { get; set; }
        
        private CanvasGroup m_canvasGroup;

        private void Awake()
        {
            m_canvasGroup = GetComponent<CanvasGroup>();
        }
        
        public void FadeView(float alpha, float duration)
        {
            FadeTween?.Kill();
            FadeTween = m_canvasGroup.DOFade(alpha, duration);            
        }

        public void KillTweens()
        {
            PosTween?.Kill();
            RotTween?.Kill();
            ScaleTween?.Kill();
        }

        private void OnDestroy()
        {
            KillTweens();
        }
        
       
    }
}