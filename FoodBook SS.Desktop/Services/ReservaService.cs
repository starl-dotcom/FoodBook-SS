using FoodBook_SS.Application.Dtos.Reservation;
using FoodBook_SS.Application.Dtos.Restaurant;
using FoodBook_SS.Desktop.Core;

namespace FoodBook_SS.Desktop.Services
{
    public interface IReservaService
    {
        Task<ApiResult<List<ReservationDto>>> GetMisReservasAsync();
        Task<ApiResult<List<ReservationDto>>> GetByRestauranteAsync(int restauranteId, string? estado = null);
        Task<ApiResult<List<MesaDisponibleDto>>>    GetMesasDisponiblesAsync(int restauranteId, DateOnly fecha, TimeOnly hora, int personas);
        Task<ApiResult<object>>               CrearAsync(SaveReservationDto dto);
        Task<ApiResult<object>>               ConfirmarAsync(int id);
        Task<ApiResult<object>>               CancelarAsync(int id, string? motivo = null);
        Task<ApiResult<object>>               CompletarAsync(int id);
        Task<ApiResult<object>>               MarcarNoShowAsync(int id);
    }

    public class ReservaService : IReservaService
    {
        private readonly ApiClient _api;
        public ReservaService(ApiClient api) => _api = api;

        public Task<ApiResult<List<ReservationDto>>> GetMisReservasAsync()
            => _api.GetAsync<List<ReservationDto>>("api/reservation/cliente");

        public Task<ApiResult<List<ReservationDto>>> GetByRestauranteAsync(int restauranteId, string? estado = null)
        {
            var url = $"api/reservation/restaurante/{restauranteId}";
            if (!string.IsNullOrEmpty(estado)) url += $"?estado={estado}";
            return _api.GetAsync<List<ReservationDto>>(url);
        }

        public Task<ApiResult<List<MesaDisponibleDto>>> GetMesasDisponiblesAsync(int restauranteId, DateOnly fecha, TimeOnly hora, int personas)
            => _api.GetAsync<List<MesaDisponibleDto>>(
                $"api/reservation/disponibilidad?restauranteId={restauranteId}&fecha={fecha:yyyy-MM-dd}&hora={hora:HH:mm}&personas={personas}");

        public Task<ApiResult<object>> CrearAsync(SaveReservationDto dto)
            => _api.PostAsync<object>("api/reservation", dto);

        public Task<ApiResult<object>> ConfirmarAsync(int id)
            => _api.PatchAsync<object>($"api/reservation/{id}/confirmar");

        public Task<ApiResult<object>> CancelarAsync(int id, string? motivo = null)
            => _api.PatchAsync<object>($"api/reservation/{id}/cancelar", motivo ?? string.Empty);

        public Task<ApiResult<object>> CompletarAsync(int id)
            => _api.PatchAsync<object>($"api/reservation/{id}/completar");

        public Task<ApiResult<object>> MarcarNoShowAsync(int id)
            => _api.PatchAsync<object>($"api/reservation/{id}/noshow");
    }
}
