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
                    
                    //NOTE - This Stream will need to be disposed by the caller
                    var stream = await response.Content.ReadAsStreamAsync();

                    var buffer = new byte[4096];
                    int receivedBytes = 0;
                    var totalBytes = Convert.ToInt32(response.Content.Headers.ContentLength);

                    //This is the seekable stream that will be returned
                    var memStream = new MemoryStream();

                    while (true)
                    {
                        int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);

                        //write the current loop's data into the MemoryStream
                        await memStream.WriteAsync(buffer, 0, bytesRead);

                        //break out when done
                        if (bytesRead == 0)
                            break;

                        receivedBytes += bytesRead;

                        if (progessReporter != null)
                        {
                            var args = new DownloadProgressArgs(receivedBytes, totalBytes);
                            progessReporter.Report(args);
                        }

                        Debug.WriteLine($"Progress: {receivedBytes} of {totalBytes} bytes read");
                    }
                    
                    memStream.Position = 0;
                    return memStream;
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
