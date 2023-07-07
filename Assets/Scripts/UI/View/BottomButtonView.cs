using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Serialization;

namespace WMK
{
    [RequireComponent(typeof(Button))]
    public class BottomButtonView : ButtonView
    {
        [SerializeField, Range(1, 2)] private float m_scaleFactor = 1.25f;
        private Tween m_tween;
        private Vector2 m_originalSizeDelta;

        private void Awake()
        {
            m_originalSizeDelta = GetComponent<RectTransform>().sizeDelta;
        }

        public override Button Button => GetComponent<Button>();
        public override void SetVisualActive(bool active, bool isImmediate = false)
        {
            Debug.Log("SetVisualActive " + active + " " + isImmediate);
            if (isImmediate)
            {
                GetComponent<RectTransform>().sizeDelta = active ? m_originalSizeDelta * m_scaleFactor : m_originalSizeDelta;
            }
            else
            {
                m_tween?.Kill();
                m_tween = active ? GetComponent<RectTransform>().DOSizeDelta(m_originalSizeDelta * m_scaleFactor, m_duration)
                    : GetComponent<RectTransform>().DOSizeDelta(m_originalSizeDelta, m_duration);
            }
        }
    }
}
