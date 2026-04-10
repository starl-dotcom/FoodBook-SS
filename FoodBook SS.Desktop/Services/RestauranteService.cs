using FoodBook_SS.Application.Dtos.Restaurant;
using FoodBook_SS.Application.Dtos.User;
using FoodBook_SS.Desktop.Core;
namespace FoodBook_SS.Desktop.Services
{
    public interface IRestauranteService
    {
        Task<ApiResult<List<RestaurantDto>>> GetTodosAsync();
        Task<ApiResult<List<RestaurantDto>>> SearchAsync(string? nombre, string? ciudad, string? tipoCocina);
        Task<ApiResult<RestaurantDto>>       GetByIdAsync(int id);
        Task<ApiResult<RestaurantDto>>       GetMioAsync();
        Task<ApiResult<object>>              AgregarMesaAsync(SaveMesaDto dto);
        Task<ApiResult<object>>              ToggleEstadoAsync(int id, bool activo);
    }
    public class RestauranteService : IRestauranteService
    {
        private readonly ApiClient _api;
        public RestauranteService(ApiClient api) => _api = api;
        public Task<ApiResult<List<RestaurantDto>>> GetTodosAsync()
            => _api.GetAsync<List<RestaurantDto>>("api/restaurant/todos");
        public Task<ApiResult<List<RestaurantDto>>> SearchAsync(string? nombre, string? ciudad, string? tipoCocina)
        {
            var query = new List<string>();
            if (!string.IsNullOrEmpty(nombre)) query.Add($"nombre={Uri.EscapeDataString(nombre)}");
            if (!string.IsNullOrEmpty(ciudad)) query.Add($"ciudad={Uri.EscapeDataString(ciudad)}");
            if (!string.IsNullOrEmpty(tipoCocina)) query.Add($"tipoCocina={Uri.EscapeDataString(tipoCocina)}");
            var qs = query.Count > 0 ? "?" + string.Join("&", query) : "";
            return _api.GetAsync<List<RestaurantDto>>($"api/restaurant{qs}");
        }
        public Task<ApiResult<RestaurantDto>> GetByIdAsync(int id)
            => _api.GetAsync<RestaurantDto>($"api/restaurant/{id}");
        public async Task<ApiResult<RestaurantDto>> GetMioAsync()
        {
            var res = await _api.GetAsync<List<RestaurantDto>>("api/restaurant/mio");
            return new ApiResult<RestaurantDto>(res.Success, res.Message ?? "", res.Data?.FirstOrDefault());
        }
        public Task<ApiResult<object>> AgregarMesaAsync(SaveMesaDto dto)
            => _api.PostAsync<object>("api/restaurant/mesa", dto);
        public Task<ApiResult<object>> ToggleEstadoAsync(int id, bool activo)
            => _api.PatchAsync<object>($"api/restaurant/{id}/estado?activo={activo}");
    }
    public interface IUsuarioService
    {
        Task<ApiResult<List<UserDto>>> GetTodosAsync();
        Task<ApiResult<object>>        ActivarDesactivarAsync(int id, bool activo);
    }
    public class UsuarioService : IUsuarioService
    {
        private readonly ApiClient _api;
        public UsuarioService(ApiClient api) => _api = api;
        public Task<ApiResult<List<UserDto>>> GetTodosAsync()
            => _api.GetAsync<List<UserDto>>("api/user");
        public Task<ApiResult<object>> ActivarDesactivarAsync(int id, bool activo)
            => _api.PatchAsync<object>($"api/user/{id}/estado?activo={activo}");
    }
}

