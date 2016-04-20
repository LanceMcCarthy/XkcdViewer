using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using ModernHttpClient;
using XkcdViewer.Models;

namespace XkcdViewer.Common
{
    public static class Helpers
    {
        /// <summary>
        /// Stand-in replacement for HttpClient.GetStreamAsync that reports download progress.
        /// IMPORTANT - The caller is responsible for disposing the Stream object
        /// </summary>
        /// <param name="url">Url of where to download the stream from</param>
        /// <param name="progessReporter">Args for reporting progress of the download operation</param>
        /// <returns>Stream content result of the GET request</returns>
        public static async Task<Stream> DownloadStreamWithProgressAsync(string url, IProgress<DownloadProgressArgs> progessReporter)
        {
            try
            {
                using (var client = new HttpClient(new NativeMessageHandler()))
                {
                    client.DefaultRequestHeaders.ExpectContinue = false;

                    var response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);

                    //Important - this makes it possible to rewind and re-read the stream
                    await response.Content.LoadIntoBufferAsync();

                    //NOTE - This Stream will need to be disposed by the caller
                    var stream = await response.Content.ReadAsStreamAsync();

                    int receivedBytes = 0;
                    var totalBytes = Convert.ToInt32(response.Content.Headers.ContentLength);

                    while (true)
                    {
                        var buffer = new byte[4096];
                        int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);

                        if (bytesRead == 0)
                        {
                            //TODO Investigate further if this Yield is needed
                            //await Task.Yield();
                            break;
                        }

                        receivedBytes += bytesRead;

                        if (progessReporter != null)
                        {
                            var args = new DownloadProgressArgs(receivedBytes, receivedBytes);
                            progessReporter.Report(args);
                        }

                        Debug.WriteLine($"Progress: {receivedBytes} of {totalBytes} bytes read");
                    }
                    
                    stream.Position = 0;
                    return stream;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"DownloadStreamWithProgressAsync Exception\r\n{ex}");
                return null;
            }
        }

        /// <summary>
        /// Stand-in replacement for HttpClient.GetStringAsync that reports download progress.
        /// IMPORTANT - The caller is responsible for disposing the Stream object
        /// </summary>
        /// <param name="url">Url of where to download the stream from</param>
        /// <param name="progessReporter">Args for reporting progress of the download operation</param>
        /// <returns>String content result of the GET request</returns>
        public static async Task<string> DownloadStringWithProgressAsync(string url, IProgress<DownloadProgressArgs> progessReporter)
        {
            using (var stream = await DownloadStreamWithProgressAsync(url, progessReporter))
            {
                if (stream == null)
                    return "";

                var stringContent = new StreamReader(stream);
                return stringContent.ReadToEnd();
            }
        }
    }
}
