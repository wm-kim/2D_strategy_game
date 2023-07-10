using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace WMK
{
    [RequireComponent(typeof(Button))]
    public class BottomButtonView : ButtonView
    {
        [SerializeField, Range(1, 2)] private float m_scaleFactor = 1.25f;
        private Tween m_tween;
        private Vector2 m_originalSizeDelta, m_scaledSizeDelta;

        private void Awake()
        {
            m_originalSizeDelta = GetComponent<RectTransform>().sizeDelta;
            m_scaledSizeDelta = m_originalSizeDelta * m_scaleFactor;
        }

        public override Button Button => GetComponent<Button>();
        public override void SetVisualActive(bool active, bool isImmediate = false)
        {
            if (isImmediate)
            {
                GetComponent<RectTransform>().sizeDelta = active ? m_scaledSizeDelta : m_originalSizeDelta;
            }
            else
            {
                m_tween?.Kill();
                m_tween = active ? GetComponent<RectTransform>().DOSizeDelta(m_scaledSizeDelta, m_duration)
                    : GetComponent<RectTransform>().DOSizeDelta(m_originalSizeDelta, m_duration);
            }
        }
    }
}
