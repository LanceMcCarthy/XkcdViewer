using System;

namespace XkcdViewer.Models
{
    public class DownloadProgressArgs : EventArgs
    {
        public DownloadProgressArgs(int bytesReceived, int totalBytes)
        {
            BytesReceived = bytesReceived;
            TotalBytes = totalBytes;
        }

        public int TotalBytes { get; }

        public int BytesReceived { get; }

        public float PercentComplete => 100 * ((float) BytesReceived / TotalBytes);
    }
}
