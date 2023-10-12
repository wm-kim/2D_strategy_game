using System;

namespace Minimax.AddressableSystem
{
    public class DownloadEvents
    {
        /// <summary>
        /// 시스템 초기화가 완료되었을 때 호출됩니다.
        /// </summary>
        public event Action SystemInitializedListener;
        public void NotifyInitialized() => SystemInitializedListener?.Invoke();
        
        /// <summary>
        /// Catalog 업데이트 완료되었을 때 호출됩니다.
        /// </summary>
        public event Action CatalogUpdatedListener;
        public void NotifyCatalogUpdated() => CatalogUpdatedListener?.Invoke();
        
        /// <summary>
        /// Size 다운로드 완료되었을 때 호출됩니다.
        /// </summary>
        public event Action<long> SizeDownloadedListener;
        public void NotifySizeDownloaded(long size) => SizeDownloadedListener?.Invoke(size);
        
        /// <summary>
        /// 다운로드 진행 중일 때 호출됩니다.
        /// </summary>
        public event Action<DownloadProgressStatus> DownloadProgressListener;
        public void NotifyDownloadProgress(DownloadProgressStatus status) => DownloadProgressListener?.Invoke(status);
        
        /// <summary>
        /// Bundle 다운로드 완료되었을 때 호출됩니다.
        /// </summary>
        public event Action<bool> DownloadFinished;
        public void NotifyDownloadFinished(bool isSuccess) => DownloadFinished?.Invoke(isSuccess);
    }
}
    
