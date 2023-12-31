using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Minimax.UI.View.ComponentViews
{
    public class BottomButtonView : ButtonView
    {
        [Header("Settings")]
        [SerializeField]
        [Range(1, 2)]
        private float m_scaleFactor = 1.25f;

        [Header("References")]
        [SerializeField]
        private DOTweenAnimation m_tweenAnimation = null;


        private Tween   m_scaleTween;
        private Vector2 m_originalSizeDelta, m_scaledSizeDelta;

        private void Awake()
        {
            m_originalSizeDelta = GetComponent<RectTransform>().sizeDelta;
            m_scaledSizeDelta   = m_originalSizeDelta * m_scaleFactor;
        }

        public override void SetVisualActive(bool active, bool isImmediate = false)
        {
            if (isImmediate)
            {
                GetComponent<RectTransform>().sizeDelta = active ? m_scaledSizeDelta : m_originalSizeDelta;
            }
            else
            {
                m_scaleTween?.Kill();
                m_scaleTween = active
                    ? GetComponent<RectTransform>().DOSizeDelta(m_scaledSizeDelta, m_duration)
                    : GetComponent<RectTransform>().DOSizeDelta(m_originalSizeDelta, m_duration);
                if (active) m_tweenAnimation.DORestart();
            }
        }
    }
}