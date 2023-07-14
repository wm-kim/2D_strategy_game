using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using DG.Tweening;
using UnityEngine;

namespace WMK
{
    public enum AnimationType
    {
        FadeIn,
        FadeOut,
        SlideIn,
        SlideOut,
        ScaleIn,
        ScaleOut,
    }
    
    public class UIAnimationController : MonoBehaviour
    {
        [SerializeField] private CanvasGroup m_canvasGroup = default;
        [SerializeField, Range(0f, 10f)] private float m_animationDuration = 0.5f;
        [SerializeField] private AnimationType m_animationType = AnimationType.FadeIn;
        
        [field: SerializeField] public bool AllowAnimationOverride { get; private set; } = false;
        [field: SerializeField, ReadOnly] public bool IsAnimating { get; private set; } = false; 
        
        [Header("Broadcasting on")]
        [SerializeField] private VoidEventSO m_OnAnimationComplete = default;
        
        private Tween m_tween;
        
        public void SetAnimationType(AnimationType animationType) => m_animationType = animationType;

        private void FadeIn()
        {
            if (IsAnimating) return;
            IsAnimating = true;
            m_tween = m_canvasGroup.DOFade(1f, m_animationDuration).OnComplete(() =>
            {
                IsAnimating = false;
                m_OnAnimationComplete.RaiseEvent();
            });
        }
        
        private void FadeOut()
        {
            if (IsAnimating) return;
            IsAnimating = true;
            m_tween = m_canvasGroup.DOFade(0f, m_animationDuration).OnComplete(() =>
            {
                IsAnimating = false;
                m_OnAnimationComplete.RaiseEvent();
            });
        }
        
    }
}
