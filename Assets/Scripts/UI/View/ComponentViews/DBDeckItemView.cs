using System.Collections;
using System.Collections.Generic;
using BrunoMikoski.AnimationSequencer;
using Minimax.UI.View;
using Minimax.UI.View.ComponentViews;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace Minimax
{
    [RequireComponent(typeof(Button))]
    public class DBDeckItemView : ButtonView
    {
        [Header("Inner References")]
        [SerializeField] private TextMeshProUGUI m_deckNameText;
        [SerializeField] private CanvasGroup m_menuCanvasGroup;
        
        [Header("Animations")]
        [SerializeField] private AnimationSequencerController m_showAnimationSequencer;
        [SerializeField] private AnimationSequencerController m_hideAnimationSequencer;
        
        public void Init(string deckName)
        {
            m_deckNameText.text = deckName;
        }

        public override Button Button => GetComponent<Button>();
        
        public override void SetVisualActive(bool active, bool isImmediate = false)
        {
            if (active)
            {
                m_hideAnimationSequencer.Kill();
                if (m_showAnimationSequencer.IsPlaying) return;
                m_showAnimationSequencer.Play(() => m_menuCanvasGroup.blocksRaycasts = true);
            }
            else
            {
                m_showAnimationSequencer.Kill();
                if (m_hideAnimationSequencer.IsPlaying) return;
                m_hideAnimationSequencer.Play(() => m_menuCanvasGroup.blocksRaycasts = false);
            }
        }
    }
}
