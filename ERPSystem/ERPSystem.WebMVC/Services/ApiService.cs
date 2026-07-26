using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace ERPSystem.WebMVC.Services
{
    public class ApiService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ApiService(
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            IHttpContextAccessor httpContextAccessor)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<T?> GetAsync<T>(string endpoint)
        {
            var client = CreateClient();

            var response = await client.GetAsync(endpoint);

            var json = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<T>(json, GetJsonOptions());
        }

        public async Task<TResponse?> PostAsync<TRequest, TResponse>(string endpoint, TRequest data)
        {
            var client = CreateClient();

            var jsonData = JsonSerializer.Serialize(data);
            var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

            var response = await client.PostAsync(endpoint, content);

            var json = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<TResponse>(json, GetJsonOptions());
        }

        private HttpClient CreateClient()
        {
            var client = _httpClientFactory.CreateClient();

            var baseUrl = _configuration["ApiSettings:BaseUrl"];
            client.BaseAddress = new Uri(baseUrl!);

            var token = _httpContextAccessor.HttpContext?.Session.GetString("JWToken");

            if (!string.IsNullOrWhiteSpace(token))
            {
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }

            return client;
        }

        private static JsonSerializerOptions GetJsonOptions()
        {
            return new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        public async Task<TResponse?> PutAsync<TRequest, TResponse>(string endpoint, TRequest data)
        {
            var client = CreateClient();

            var jsonData = JsonSerializer.Serialize(data);
            var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

            var response = await client.PutAsync(endpoint, content);

            var json = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<TResponse>(json, GetJsonOptions());
        }

        public async Task<T?> DeleteAsync<T>(string endpoint)
        {
            var client = CreateClient();

            var response = await client.DeleteAsync(endpoint);

            var json = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<T>(json, GetJsonOptions());
        }
    }


}
