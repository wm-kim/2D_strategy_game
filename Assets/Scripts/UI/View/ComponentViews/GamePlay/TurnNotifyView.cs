using System.Collections;
using System.Collections.Generic;
using BrunoMikoski.AnimationSequencer;
using Minimax.CoreSystems;
using TMPro;
using UnityEngine;

namespace Minimax
{
    public class TurnNotifyView : MonoBehaviour
    {
        [SerializeField]
        private AnimationSequencerController m_turnNotification;

        [SerializeField]
        private TextMeshProUGUI m_turnText;

        public void Notify(string text)
        {
            m_turnText.text = text;
            AudioManager.Instance.PlaySFX(AudioLib.TurnNotification);
            m_turnNotification.Play();
        }
    }
}