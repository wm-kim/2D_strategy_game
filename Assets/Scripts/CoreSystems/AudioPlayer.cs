using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Minimax
{
    public class AudioPlayer : MonoBehaviour
    {
        public enum AudioType
        {
            BGM,
            SFX
        }

        [SerializeField] private AudioType m_audioType;
        [SerializeField] private AudioLib  m_audioEnum;

        private AudioManager m_audioManager;

        private void Awake()
        {
            m_audioManager = AudioManager.Instance;
        }

        public void Play()
        {
            switch (m_audioType)
            {
                case AudioType.BGM:
                    m_audioManager.PlayBGM(m_audioEnum);
                    break;
                case AudioType.SFX:
                    m_audioManager.PlaySFX(m_audioEnum);
                    break;
            }
        }
    }
}