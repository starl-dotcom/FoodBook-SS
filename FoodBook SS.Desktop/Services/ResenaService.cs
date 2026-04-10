using FoodBook_SS.Application.Dtos.Review;
using FoodBook_SS.Desktop.Core;

namespace FoodBook_SS.Desktop.Services
{
    public interface IResenaService
    {
        Task<ApiResult<List<ReviewDto>>> GetByRestauranteAsync(int restauranteId);
        Task<ApiResult<List<ReviewDto>>> GetAllAsync();
        Task<ApiResult<object>>          CrearAsync(SaveReviewDto dto);
        Task<ApiResult<object>>          ResponderAsync(int resenaId, string respuesta);
        Task<ApiResult<object>>          ModerarAsync(int resenaId, bool visible);
    }

    public class ResenaService : IResenaService
    {
        private readonly ApiClient _api;
        public ResenaService(ApiClient api) => _api = api;

        public Task<ApiResult<List<ReviewDto>>> GetByRestauranteAsync(int restauranteId)
            => _api.GetAsync<List<ReviewDto>>($"api/review/restaurante/{restauranteId}");

        public Task<ApiResult<List<ReviewDto>>> GetAllAsync()
            => _api.GetAsync<List<ReviewDto>>("api/review");

        public Task<ApiResult<object>> CrearAsync(SaveReviewDto dto)
            => _api.PostAsync<object>("api/review", dto);

        public Task<ApiResult<object>> ResponderAsync(int resenaId, string respuesta)
            => _api.PatchAsync<object>($"api/review/{resenaId}/responder", $"\"{respuesta}\"");

        public Task<ApiResult<object>> ModerarAsync(int resenaId, bool visible)
            => _api.PatchAsync<object>($"api/review/{resenaId}/moderar?visible={visible}");
    }
}
