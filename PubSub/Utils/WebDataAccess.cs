using System;
using System.Net.Http;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace PubSub.Utils
{
    public class WebClient
    {
        public const string UserAgent = "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_14_0) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/70.0.3538.102 Safari/537.36";

        public static readonly HttpClient Client = new HttpClient();

        public WebClient()
        {
            Client.DefaultRequestHeaders.UserAgent.ParseAdd(UserAgent);
        }

        public static async Task<string> GetString(string url, string accept)
        {
            return await GetString(new Uri(url), accept);
        }

        public static async Task<string> GetString(Uri url, string accept)
        {
            var httpRequest = new HttpRequestMessage();
            httpRequest.Headers.UserAgent.ParseAdd(WebClient.UserAgent);
            httpRequest.Headers.Accept.ParseAdd(accept);
            httpRequest.RequestUri = url;

            var httpResponse = await WebClient.Client.SendAsync(httpRequest);
            var content = await httpResponse.Content.ReadAsStringAsync();
            if (!httpResponse.IsSuccessStatusCode)
            {
                throw new InvalidOperationException($"HTTP Response from {url} was {httpResponse.StatusCode} at {DateTime.Now}.");
            }

            return content;
        }

        public static async Task<byte[]> GetFile(Uri url)
        {
            var httpRequest = new HttpRequestMessage();
            httpRequest.Headers.UserAgent.ParseAdd(WebClient.UserAgent);
            httpRequest.RequestUri = url;

            var httpResponse = await WebClient.Client.SendAsync(httpRequest);
            var content = await httpResponse.Content.ReadAsByteArrayAsync();
            if (!httpResponse.IsSuccessStatusCode)
            {
                throw new InvalidOperationException($"HTTP Response from {url} was {httpResponse.StatusCode} at {DateTime.Now}.");
            }

            return content;
        }

        public static string MD5(byte[] content)
        {
            using (MD5 hashGenerator = System.Security.Cryptography.MD5.Create())
            {
                byte[] hashValue = hashGenerator.ComputeHash(content);
                var result = BitConverter.ToString(hashValue).Replace("-", "");
                return result;
            }
        }
    }
}