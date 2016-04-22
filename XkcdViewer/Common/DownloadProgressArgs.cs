using System;

namespace XkcdViewer.Common
{
    /// <summary>
    /// To be used with Progress for DownloadStreamWithProgressAsync
    /// </summary>
    public class DownloadProgressArgs : EventArgs
    {
        public DownloadProgressArgs(int bytesReceived, int totalBytes)
        {
            BytesReceived = bytesReceived;
            TotalBytes = totalBytes;
        }

        public double TotalBytes { get; }

        public double BytesReceived { get; }

        public double PercentComplete => 100 * ((double) BytesReceived / TotalBytes);
    }
}
