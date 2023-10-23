using System;
using DG.Tweening;
using Minimax.GamePlay;
using Unity.VisualScripting;
using UnityEngine;

namespace Minimax.UI.View.ComponentViews.GamePlay
{
    /// <summary>
    /// This class is responsible for tweening the hand card view and the card visual.
    /// </summary>
    public class HandCardView : TweenableItem
    {
        public Tween PosTween { get; set; }
        public Tween RotTween { get; set; }
        public Tween ScaleTween { get; set; }
        public Tween FadeTween { get; set; }
        
        [SerializeField] private CardVisual m_cardVisual;
        private CanvasGroup m_canvasGroup;

        private void Awake()
        {
            m_canvasGroup = GetComponent<CanvasGroup>();
        }
        
        public void CreateClientCardAndSetVisual(int cardUID)
        {
            var card = ClientCard.CardsCreatedThisGame[cardUID];
            // card data could be null
            var cardData = card.Data;
            m_cardVisual.Init(cardData);
        }
        
        public void FadeView(float alpha, float duration)
        {
            FadeTween?.Kill();
            FadeTween = m_canvasGroup.DOFade(alpha, duration);            
        }

        private void OnDestroy()
        {
            KillAllTweens();
        }
    }
}