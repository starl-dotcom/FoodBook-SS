using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
namespace FoodBook_SS.Desktop.Core
{
    public class ApiClient
    {
        private readonly HttpClient _http;
        private static readonly JsonSerializerOptions _json = new()
        {
            PropertyNameCaseInsensitive = true
        };
        public ApiClient(IHttpClientFactory factory)
        {
            _http = factory.CreateClient("FoodBookAPI");
        }
        private void SetAuthHeader()
        {
            var token = SessionManager.GetToken();
            _http.DefaultRequestHeaders.Authorization = string.IsNullOrEmpty(token)
                ? null
                : new AuthenticationHeaderValue("Bearer", token);
        }
        public async Task<ApiResult<T>> GetAsync<T>(string url)
        {
            SetAuthHeader();
            var resp = await _http.GetAsync(url);
            return await ParseAsync<T>(resp);
        }
        public async Task<ApiResult<T>> PostAsync<T>(string url, object body)
        {
            SetAuthHeader();
            var content = ToJson(body);
            var resp = await _http.PostAsync(url, content);
            return await ParseAsync<T>(resp);
        }
        public async Task<ApiResult<T>> PatchAsync<T>(string url, object? body = null)
        {
            SetAuthHeader();
            var content = body is null ? null : ToJson(body);
            var request = new HttpRequestMessage(HttpMethod.Patch, url) { Content = content };
            var resp = await _http.SendAsync(request);
            return await ParseAsync<T>(resp);
        }
        public async Task<ApiResult<T>> DeleteAsync<T>(string url)
        {
            SetAuthHeader();
            var resp = await _http.DeleteAsync(url);
            return await ParseAsync<T>(resp);
        }
        private static async Task<ApiResult<T>> ParseAsync<T>(HttpResponseMessage resp)
        {
            var raw = await resp.Content.ReadAsStringAsync();
            if (resp.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                SessionManager.Clear();
                return new ApiResult<T>(false, "Sesión expirada. Inicia sesión nuevamente.", default);
            }
            try
            {
                var wrapper = JsonSerializer.Deserialize<JsonWrapper<T>>(raw, _json);
                return new ApiResult<T>(
                    wrapper?.Success ?? false,
                    wrapper?.Message ?? resp.ReasonPhrase ?? "Error desconocido",
                    wrapper != null ? wrapper.Data : default);
            }
            catch
            {
                return new ApiResult<T>(false, $"Error al procesar respuesta: {raw[..Math.Min(raw.Length, 120)]}", default);
            }
        }
        private static StringContent ToJson(object obj) =>
            new(JsonSerializer.Serialize(obj, _json), Encoding.UTF8, "application/json");
        private class JsonWrapper<T>
        {
            public bool Success { get; set; }
            public string? Message { get; set; }
            public T? Data { get; set; }
        }
    }
    public record ApiResult<T>(bool Success, string Message, T? Data);
}

