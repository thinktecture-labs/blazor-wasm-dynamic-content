using Blazor.DynamicContent.Client.Models;
using System.Net.Http.Json;

namespace Blazor.DynamicContent.Client.Services
{
    public class DynamicControlDataService
    {
        private readonly HttpClient _httpClient;

        public DynamicControlDataService(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public Task<Section[]> LoadFormData()
        {
            return _httpClient.GetFromJsonAsync<Section[]>("sample-data/render-fragment-data.json");
        }

        public Task<Dictionary<string, object>> LoadFormDataValues()
        {
            return _httpClient.GetFromJsonAsync<Dictionary<string, object>>($"sample-data/render-fragment-data-values.json");
        }
    }
}