using FoodBook_SS.Application.Dtos.Order;
using FoodBook_SS.Desktop.Core;

namespace FoodBook_SS.Desktop.Services
{
    public interface IOrdenService
    {
        Task<ApiResult<List<OrderDto>>> GetMisOrdenesAsync();
        Task<ApiResult<List<OrderDto>>> GetByRestauranteAsync(int restauranteId);
        Task<ApiResult<OrderDto>>       GetByIdAsync(int id);
        Task<ApiResult<object>>         CrearAsync(SaveOrderDto dto);
        Task<ApiResult<object>>         CambiarEstadoAsync(int id, string nuevoEstado);
        Task<ApiResult<List<VentaFechaDto>>> GetVentasAsync(int restauranteId, DateOnly desde, DateOnly hasta);
        Task<ApiResult<List<ProductoMasOrdenadoDto>>> GetTopProductosAsync(int restauranteId, DateOnly desde, DateOnly hasta);
    }

    public class OrdenService : IOrdenService
    {
        private readonly ApiClient _api;
        public OrdenService(ApiClient api) => _api = api;

        public Task<ApiResult<List<OrderDto>>> GetMisOrdenesAsync()
            => _api.GetAsync<List<OrderDto>>("api/order/cliente");

        public Task<ApiResult<List<OrderDto>>> GetByRestauranteAsync(int restauranteId)
            => _api.GetAsync<List<OrderDto>>($"api/order/restaurante/{restauranteId}");

        public Task<ApiResult<OrderDto>> GetByIdAsync(int id)
            => _api.GetAsync<OrderDto>($"api/order/{id}");

        public Task<ApiResult<object>> CrearAsync(SaveOrderDto dto)
            => _api.PostAsync<object>("api/order", dto);

        public Task<ApiResult<object>> CambiarEstadoAsync(int id, string nuevoEstado)
            => _api.PatchAsync<object>($"api/order/{id}/estado", $"\"{nuevoEstado}\"");

        public Task<ApiResult<List<VentaFechaDto>>> GetVentasAsync(int restauranteId, DateOnly desde, DateOnly hasta)
            => _api.GetAsync<List<VentaFechaDto>>($"api/order/restaurante/{restauranteId}/ventas?desde={desde:yyyy-MM-dd}&hasta={hasta:yyyy-MM-dd}");

        public Task<ApiResult<List<ProductoMasOrdenadoDto>>> GetTopProductosAsync(int restauranteId, DateOnly desde, DateOnly hasta)
            => _api.GetAsync<List<ProductoMasOrdenadoDto>>($"api/order/restaurante/{restauranteId}/top-productos?desde={desde:yyyy-MM-dd}&hasta={hasta:yyyy-MM-dd}");
    }
}
