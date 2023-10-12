namespace Minimax.AddressableSystem
{
    public struct DownloadProgressStatus
    {
        public long DownloadedBytes;  
        public long TotalBytes;      
        public long RemainedBytes;
        public float TotalProgress;
        
        public DownloadProgressStatus(long downloadedBytes, long totalBytes, long remainedBytes, float totalProgress)
        {
            DownloadedBytes = downloadedBytes;
            TotalBytes = totalBytes;
            RemainedBytes = remainedBytes;
            TotalProgress = totalProgress;
        }
    }
}