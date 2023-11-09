using System;
using System.Collections;
using UnityEngine;

namespace Minimax.AddressableSystem
{
    public class DownloadController
    {
        public enum State
        {
            Idle,

            Initialize,
            UpdateCatalog,
            DownloadSize,
            DownloadDependencies,
            Downloading,

            Finished
        }

        private AddressableDownloader m_downloader;

        [SerializeField]
        private string m_labelToDownload;

        [SerializeField]
        private string m_downloadURL;

        public  State CurrentState { get; private set; } = State.Idle;
        private State m_lastValidState = State.Idle;

        private Action<DownloadEvents> m_onEventObtained;

        public IEnumerator StartDownloadRoutine(Action<DownloadEvents> onEventObtained)
        {
            m_downloader      = new AddressableDownloader();
            m_onEventObtained = onEventObtained;

            m_lastValidState = CurrentState = State.Initialize;

            while (CurrentState != State.Finished)
            {
                OnExecute();
                yield return null;
            }
        }


        public void GoNext()
        {
            if (m_lastValidState == State.Initialize)
                CurrentState = State.UpdateCatalog;
            else if (m_lastValidState == State.UpdateCatalog)
                CurrentState = State.DownloadSize;
            else if (m_lastValidState == State.DownloadSize)
                CurrentState = State.DownloadDependencies;
            else if (m_lastValidState == State.Downloading || m_lastValidState == State.DownloadDependencies)
                CurrentState = State.Finished;

            m_lastValidState = CurrentState;
        }

        private void OnExecute()
        {
            if (CurrentState == State.Idle) return;

            if (CurrentState == State.Initialize)
            {
                var events = m_downloader.InitializedSystem(m_labelToDownload, m_downloadURL);
                m_onEventObtained?.Invoke(events);

                CurrentState = State.Idle;
            }
            else if (CurrentState == State.UpdateCatalog)
            {
                m_downloader.UpdateCatalog();
                CurrentState = State.Idle;
            }
            else if (CurrentState == State.DownloadSize)
            {
                m_downloader.DownloadSize();
                CurrentState = State.Idle;
            }
            else if (CurrentState == State.DownloadDependencies)
            {
                m_downloader.StartDownload();
                CurrentState = State.Downloading;
            }
            else if (CurrentState == State.Downloading)
            {
                m_downloader.Update();
            }
        }
    }
}