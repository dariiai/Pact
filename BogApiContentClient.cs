using System;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Bog.Api.Domain.Models.Http;

namespace MyPactTest
{
    /// <summary>
    /// https://github.com/pact-foundation/pact-net
    /// </summary>
    public class BogApiContentClient
    {
        private const string BOG_API_HOST_URL = "https://bogapi9a005f.azurewebsites.net:443";
        private readonly HttpClient _client;

        public BogApiContentClient(string baseUri = null)
        {
            _client = new HttpClient
            {
                BaseAddress = new Uri(baseUri ?? BOG_API_HOST_URL)
            };
        }

        public async Task<ContentResponse> GetArticleContent(string contentId)
        {
            contentId ??= "a26ce357-e3c5-4828-2a4b-08d8422b1ed9";
            string contentRequestUri = $"/api/content/article/{contentId}";
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, contentRequestUri);
            //httpRequestMessage.Headers.Add("Accept", "application/json");

            var httpResponse = await _client.SendAsync(httpRequestMessage);

            var reasonPhrase = httpResponse.ReasonPhrase;

            if (!httpResponse.IsSuccessStatusCode)
            {
                throw new Exception(reasonPhrase);
            }

            var content = await httpResponse.Content.ReadAsStringAsync();

            if (string.IsNullOrWhiteSpace(content))
            {
                return null;
            }

            var contentResponse = JsonSerializer.Deserialize<ContentResponse>(content);
            return contentResponse;
        }
    }
}



/*
             static JsonSerializerOptions Options => new JsonSerializerOptions
                                                {
                                                    PropertyNameCaseInsensitive = true,
                                                    IgnoreNullValues = true,
                                                    AllowTrailingCommas = true,
                                                    WriteIndented = true
                                                };
             */