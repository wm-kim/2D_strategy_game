using System;
using System.Collections;
using System.Collections.Generic;
using BrunoMikoski.AnimationSequencer;
using UnityEngine;
using UnityEngine.Serialization;

namespace WMK
{
    public abstract class PopupView : MonoBehaviour
    {
        [field : SerializeField] public PopupType Type { get; protected set; } = PopupType.Undefined;
        [SerializeField] private AnimationSequencerController m_showAnimationSequencer;
        [SerializeField] private AnimationSequencerController m_hideAnimationSequencer;
        
        private void Awake()
        {
            SetPopupType();
            CheckIfTypeIsSet();
        }
        
        private void CheckIfTypeIsSet()
        {
            UnityEngine.Assertions.Assert.IsTrue(Type != PopupType.Undefined, 
                $"PopupType is undefined in {gameObject.name}, please set it properly in inspector");
        }
        
        /// <summary>
        /// Popup의 종류를 설정합니다.
        /// </summary>
        protected abstract void SetPopupType();
        
        public void Show()
        {
            if (m_showAnimationSequencer.IsPlaying) return;
            if (m_hideAnimationSequencer.IsPlaying) m_hideAnimationSequencer.Kill();
            m_showAnimationSequencer.Play();
        }
        
        public void Hide()
        {
            if (m_hideAnimationSequencer.IsPlaying) return;
            if (m_showAnimationSequencer.IsPlaying) m_showAnimationSequencer.Kill();
            m_hideAnimationSequencer.Play();
        }
    }
}
