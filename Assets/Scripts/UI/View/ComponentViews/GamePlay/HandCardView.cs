using DG.Tweening;
using Minimax.GamePlay;
using Minimax.Utilities;
using UnityEngine;

namespace Minimax.UI.View.ComponentViews.GamePlay
{
    /// <summary>
    /// This class is responsible for tweening the hand card view and the card visual.
    /// </summary>
    public class HandCardView : TweenableItem
    {
        [SerializeField] private CardVisual m_cardVisual;
        [SerializeField] private CanvasGroup m_canvasGroup;
        
        public Tweener FadeTween { get; private set; }

        public override void KillAllTweens()
        {
            FadeTween?.Kill();
            base.KillAllTweens();
        }

        public void CreateClientCardAndSetVisual(int cardUID)
        {
            var card = ClientCard.CardsCreatedThisGame[cardUID];
            // card data could be null
            var cardData = card.Data;
            m_cardVisual.Init(cardData);
        }
        
        public void UpdateCardVisual(int cardUID)
        {
            var card = ClientCard.CardsCreatedThisGame[cardUID];
            // card data could be null
            var cardData = card.Data;
            m_cardVisual.Init(cardData);
        }
        
        public Tweener StartFadeTween(float targetAlpha, float duration = 0.0f)
        {
            if (duration.Equals(0f))
            {
                m_canvasGroup.alpha = targetAlpha;
                return null; 
            }
            
            FadeTween?.Kill();
            return FadeTween = m_canvasGroup.DOFade(targetAlpha, duration);
        }

        private void OnDestroy()
        {
            KillAllTweens();
        }
    }
}