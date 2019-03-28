using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using XkcdViewer.Forms.NetStandard.Models;

namespace XkcdViewer.Forms.NetStandard.Services
{
    public class XkcdApiService
    {
        private readonly HttpClient client;
        private const string BaseUrl = "https://xkcd.com";

        public XkcdApiService()
        {
            var handler = new HttpClientHandler();
            if (handler.SupportsAutomaticDecompression)
            {
                handler.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
            }

            client = new HttpClient(handler);
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
                    
                    return result ?? new Comic { Title = "Json Schmason", Transcript = $"Someone didnt like the way the comic's json tasted and spit it back out" };
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"GetComicJsonAsync Exception\r\n{ex}");
                return new Comic { Title = "Exception", Transcript = $"Error getting comic: {ex.Message}" };
            }
        }

       

        //{
        //    "month": "5",
        //    "num": 1991,
        //    "link": "",
        //    "year": "2018",
        //    "news": "",
        //    "safe_title": "Research Areas by Size and Countedness",
        //    "transcript": "",
        //    "alt": "Mathematicians give a third answer on the vertical axis, \"That question is poorly defined, but we have a sub-field devoted to every plausible version of it.\"",
        //    "img": "https://imgs.xkcd.com/comics/research_areas_by_size_and_countedness.png",
        //    "title": "Research Areas by Size and Countedness",
        //    "day": "9"
        //}
}
}
