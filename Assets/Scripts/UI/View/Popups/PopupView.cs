using System;
using BrunoMikoski.AnimationSequencer;
using UnityEngine;

namespace Minimax.UI.View.Popups
{
    public abstract class PopupView : MonoBehaviour
    {
        [field : SerializeField, ReadOnly] public PopupType Type { get; protected set; } = PopupType.Undefined;
        
        [Header("Animations")]
        [Space(10f)]
        [SerializeField] private AnimationSequencerController m_showAnimationSequencer;
        [SerializeField] private AnimationSequencerController m_hideAnimationSequencer;
        
        public void Init()
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
        /// Popup의 종류를 설정합니다. PopupType은 PopupManager에서 Popup pool을 관리하는데 사용됩니다.
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
