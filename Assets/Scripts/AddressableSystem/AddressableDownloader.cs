using System;
using System.Collections.Generic;
using Minimax.Utilities;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Minimax.AddressableSystem
{
    /// <summary>
    /// Addressable 다운도드를 수행합니다.
    /// </summary>
    public class AddressableDownloader
    {
        /// <summary> 다운로드 받을 URL
        /// Addressable Profile 쪽에서 LoadPath 에 {AddressableDownloader.DownloadURL} 설정
        /// </summary>
        public static string DownloadURL;

        /// <summary>
        /// 이벤트 클래스
        /// </summary>
        private DownloadEvents m_events;

        /// <summary>
        /// 다운로드 받을 Label
        /// </summary>
        private string m_labelToDownload;

        /// <summary>
        /// 다운로드 받을 전체 사이즈
        /// </summary>
        private long m_totalSize;

        /// <summary>
        /// 번들 다운로드 핸들 - 비동기로 진행되는 다운로드를 이 변수를 통해 진행 상황 알수있음
        /// </summary>
        private AsyncOperationHandle m_downloadHandle;

        public DownloadEvents InitializedSystem(string label, string downloadURL)
        {
            m_events = new DownloadEvents();

            Addressables.InitializeAsync().Completed += OnInitialized;

            DownloadURL       = downloadURL;
            m_labelToDownload = label;

            ResourceManager.ExceptionHandler += OnException;

            return m_events;
        }

        public void UpdateCatalog()
        {
            Addressables.CheckForCatalogUpdates().Completed += (result) =>
            {
                var catalogToUpdate = result.Result;
                if (catalogToUpdate.Count > 0)
                    Addressables.UpdateCatalogs(catalogToUpdate).Completed += OnCatalogUpdated;
                else
                    m_events.NotifyCatalogUpdated();
            };
        }

        public void DownloadSize()
        {
            Addressables.GetDownloadSizeAsync(m_labelToDownload).Completed += OnSizeDownloaded;
        }

        public void StartDownload()
        {
            m_downloadHandle           =  Addressables.DownloadDependenciesAsync(m_labelToDownload);
            m_downloadHandle.Completed += OnDependenciesDownloaded;
        }

        public void Update()
        {
            if (m_downloadHandle.IsValid()
                && m_downloadHandle.IsDone == false
                && m_downloadHandle.Status != AsyncOperationStatus.Failed)
            {
                var status = m_downloadHandle.GetDownloadStatus();

                var curDownloadSize = status.DownloadedBytes;
                var remainedSize    = m_totalSize - curDownloadSize;

                m_events.NotifyDownloadProgress(new DownloadProgressStatus(
                    status.DownloadedBytes,
                    m_totalSize,
                    remainedSize,
                    status.Percent));
            }
        }

        /// <summary>
        /// 초기화 완료시 호출 
        /// </summary>
        private void OnInitialized(AsyncOperationHandle<IResourceLocator> obj)
        {
            m_events.NotifyInitialized();
        }

        /// <summary>
        /// 카탈로그 업데이트 완료시 호출
        /// </summary>
        private void OnCatalogUpdated(AsyncOperationHandle<List<IResourceLocator>> obj)
        {
            m_events.NotifyCatalogUpdated();
        }

        /// <summary>
        /// 사이즈 다운로드 완료시 호출
        /// </summary>
        private void OnSizeDownloaded(AsyncOperationHandle<long> result)
        {
            m_totalSize = result.Result;
            m_events.NotifySizeDownloaded(result.Result);
        }

        /// <summary>
        /// 번들 다운로드 완료시 호출
        /// </summary>
        private void OnDependenciesDownloaded(AsyncOperationHandle result)
        {
            m_events.NotifyDownloadFinished(result.Status == AsyncOperationStatus.Succeeded);
        }


        /// <summary>
        /// 예외 발생시 호출
        /// </summary>
        private void OnException(AsyncOperationHandle handle, Exception exception)
        {
            DebugWrapper.LogError(exception.Message);

            if (exception is UnityEngine.ResourceManagement.Exceptions.RemoteProviderException)
            {
                // Remote 관련 에러 발생시
            }
        }
    }
}