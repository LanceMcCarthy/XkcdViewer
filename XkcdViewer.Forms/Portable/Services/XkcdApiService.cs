using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using ModernHttpClient;
using Newtonsoft.Json;
using Portable.Models;

namespace Portable.Services
{
    public class XkcdApiService
    {
        private readonly HttpClient client;
        private const string BaseUrl = "https://xkcd.com";

        public XkcdApiService()
        {
            client = new HttpClient(new NativeMessageHandler());
        }

        /// <summary>
        /// Gets a specific comic from XKCD, including Gardens
        /// </summary>
        /// <param name="comicNumber">Comic number to retrieve</param>
        /// <returns></returns>
        public async Task<Comic> GetComicAsync(int comicNumber)
        {
            try
            {
                var url = BaseUrl + $"/{comicNumber}/info.0.json";

                using (var response = await client.GetAsync(url, HttpCompletionOption.ResponseContentRead))
                {
                    var jsonResult = await response.Content.ReadAsStringAsync();

                    if (string.IsNullOrEmpty(jsonResult))
                        return new Comic { Title = "Whoops", Transcript = $"There was no comic to be found" };

                    var result = JsonConvert.DeserializeObject<Comic>(jsonResult);

                    return result ??
                           new Comic { Title = "Json Schmason", Transcript = $"Someone didnt like the way the comic's json tasted and spit it back out" };
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"GetComicJsonAsync Exception\r\n{ex}");
                return new Comic { Title = "Exception", Transcript = $"Error getting comic: {ex.Message}" };
            }
        }

        /// <summary>
        /// Gets latest comic available, including Gardens
        /// </summary>
        /// <returns></returns>
        public async Task<Comic> GetNewestComicAsync()
        {
            try
            {
                var url = BaseUrl + "/info.0.json";

                using (var response = await client.GetAsync(url, HttpCompletionOption.ResponseContentRead))
                {
                    var jsonResult = await response.Content.ReadAsStringAsync();

                    if (string.IsNullOrEmpty(jsonResult))
                        return new Comic { Title = "Whoops", Transcript = $"There was no comic to be found" };

                    var result = JsonConvert.DeserializeObject<Comic>(jsonResult);

                    return result ??
                           new Comic { Title = "Json Schmason", Transcript = $"Someone didnt like the way the comic's json tasted and spit it back out" };
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"GetComicJsonAsync Exception\r\n{ex}");
                return new Comic { Title = "Exception", Transcript = $"Error getting comic: {ex.Message}" };
            }
        }
    }
}
