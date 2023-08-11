using System.Collections;
using System.Collections.Generic;
using BrunoMikoski.AnimationSequencer;
using Minimax.UI.View;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace Minimax
{
    public class DBDeckItemView : StatefulUIView
    {
        [Header("Inner References")]
        [SerializeField] private TextMeshProUGUI m_deckNameText;
        
        [Header("Animations")]
        [SerializeField] private AnimationSequencerController m_showAnimationSequencer;
        [SerializeField] private AnimationSequencerController m_hideAnimationSequencer;
        
        public void Init(string deckName)
        {
            m_deckNameText.text = deckName;
        }

        protected override void Show(float transitionDuration = 0)
        {
            m_showAnimationSequencer.Play(SetAppearedState);
        }

        protected override void Hide(float transitionDuration = 0)
        {
            m_hideAnimationSequencer.Play(SetDisappearedState);
        }
    }
}
