using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Serialization;

namespace Minimax
{
    public class DeckListAnimationController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private CanvasGroup m_deckListBackground = default;
        [SerializeField] private RectTransform m_deckListPanel = default;
        
        [Header("Animation")]
        [SerializeField, Range(0f, 1f)] private float m_animationDuration = 0.5f;
        [SerializeField] private float m_slideOffset = 420f;
        
        private Tween m_deckListBackgroundTween;
        private Tween m_deckListPanelTween;
        
        private Vector2 m_panelStartPosition;

        private void Start()
        {
            m_panelStartPosition = m_deckListPanel.anchoredPosition;   
            
            m_deckListBackground.alpha = 0f;
            m_deckListBackground.gameObject.SetActive(false);
        }

        public void ShowDeckList()
        {
            KillTweens();
            
            m_deckListBackground.gameObject.SetActive(true);
            m_deckListBackgroundTween = m_deckListBackground.DOFade(1f, m_animationDuration);
            
            m_deckListPanelTween = m_deckListPanel.DOAnchorPosX(m_panelStartPosition.x - m_slideOffset, m_animationDuration);
        }
        
        public void HideDeckList()
        {
            KillTweens();
            
            m_deckListBackgroundTween = m_deckListBackground.DOFade(0f, m_animationDuration)
                .OnComplete(() => m_deckListBackground.gameObject.SetActive(false));
            
            var panelPosition = m_deckListPanel.anchoredPosition;
            m_deckListPanelTween = m_deckListPanel.DOAnchorPosX(m_panelStartPosition.x, m_animationDuration);
        }
        
        private void KillTweens()
        {
            if (m_deckListBackgroundTween != null)
            {
                m_deckListBackgroundTween.Kill();
            }
            
            if (m_deckListPanelTween != null)
            {
                m_deckListPanelTween.Kill();
            }
        }
    }
}
