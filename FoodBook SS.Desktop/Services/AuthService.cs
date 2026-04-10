using FoodBook_SS.Application.Dtos.User;
using FoodBook_SS.Desktop.Core;
namespace FoodBook_SS.Desktop.Services
{
    public interface IAuthService
    {
        Task<ApiResult<AuthResponseDto>> LoginAsync(LoginDto dto);
        Task<ApiResult<object>>          RegisterAsync(SaveUserDto dto);
        Task<ApiResult<int?>>             GetMiRestauranteIdAsync(int propietarioId);
    }
    public class AuthService : IAuthService
    {
        private readonly ApiClient _api;
        public AuthService(ApiClient api) => _api = api;
        public Task<ApiResult<AuthResponseDto>> LoginAsync(LoginDto dto)
            => _api.PostAsync<AuthResponseDto>("api/user/login", dto);
        public Task<ApiResult<object>> RegisterAsync(SaveUserDto dto)
            => _api.PostAsync<object>("api/user/register", dto);
        public async Task<ApiResult<int?>> GetMiRestauranteIdAsync(int propietarioId)
        {
            var r = await _api.GetAsync<List<FoodBook_SS.Application.Dtos.Restaurant.RestaurantDto>>(
                        $"api/restaurant?propietarioId={propietarioId}");
            if (!r.Success || r.Data == null || r.Data.Count == 0)
                return new ApiResult<int?>(false, r.Message, null);
            return new ApiResult<int?>(true, "OK", r.Data[0].Id);
        }
    }
}

