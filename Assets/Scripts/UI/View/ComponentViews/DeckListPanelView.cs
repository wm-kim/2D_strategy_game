using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Minimax.UI.View.ComponentViews
{
    public class DeckListPanelView : StatefulUIView
    {
        [Header("References")]
        [SerializeField] private CanvasGroup m_deckListBackground = default;
        [SerializeField] private RectTransform m_deckListPanel = default;
        
        [Header("Animation")]
        [SerializeField, Range(0f, 1f)] private float m_animationDuration = 0.5f;
        [SerializeField] private float m_slideOffset = 420f;
        
        private Tween m_deckListBackgroundTween;
        private Tween m_deckListPanelTween;
        private Button m_backgroundButton;
        
        private Vector2 m_panelStartPosition;

        private void Start()
        {
            m_panelStartPosition = m_deckListPanel.anchoredPosition;   
            
            m_deckListBackground.alpha = 0f;
            m_deckListBackground.gameObject.SetActive(false);
            m_backgroundButton = m_deckListBackground.GetComponent<Button>();
        }
        
        protected override void Show(float transitionDuration = 0)
        {
            KillTweens();
            
            m_deckListBackground.gameObject.SetActive(true);
            m_deckListBackgroundTween = m_deckListBackground.DOFade(1f, m_animationDuration)
                .OnComplete(AddBackgroundClickToHideEvent);
            m_deckListPanelTween = m_deckListPanel
                .DOAnchorPosX(m_panelStartPosition.x - m_slideOffset, m_animationDuration)
                .OnComplete(SetAppearedState);
        }
        
        protected override void Hide(float transitionDuration = 0)
        {
            KillTweens();

            m_deckListBackgroundTween = m_deckListBackground.DOFade(0f, m_animationDuration)
                .OnComplete(() =>
                {
                    m_deckListBackground.gameObject.SetActive(false);
                    RemoveBackgroundClickToHideEvent();
                }); 
            
            var panelPosition = m_deckListPanel.anchoredPosition;
            m_deckListPanelTween = m_deckListPanel.DOAnchorPosX(m_panelStartPosition.x, m_animationDuration)
                .OnComplete(SetDisappearedState);
        }

        private void KillTweens()
        {
            if (m_deckListBackgroundTween != null) m_deckListBackgroundTween.Kill();
            if (m_deckListPanelTween != null) m_deckListPanelTween.Kill();
        }

        private void AddBackgroundClickToHideEvent() => m_backgroundButton.onClick.AddListener(OnBackgroundClicked);
        
        private void RemoveBackgroundClickToHideEvent() => m_backgroundButton.onClick.RemoveListener(OnBackgroundClicked);
        
        // need this function to be removed from the event listener
        private void OnBackgroundClicked() => StartHide();
    }
}
