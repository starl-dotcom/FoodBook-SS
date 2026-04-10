using FoodBook_SS.Application.Dtos.Payment;
using FoodBook_SS.Desktop.Core;

namespace FoodBook_SS.Desktop.Services
{
    public interface IPagoService
    {
        Task<ApiResult<object>> ProcesarPagoAsync(SavePaymentDto dto);
        Task<ApiResult<List<PaymentDto>>> GetMisPagosAsync();
    }

    public class PagoService : IPagoService
    {
        private readonly ApiClient _api;
        public PagoService(ApiClient api) => _api = api;

        public Task<ApiResult<object>> ProcesarPagoAsync(SavePaymentDto dto)
            => _api.PostAsync<object>("api/payment", dto);

        public Task<ApiResult<List<PaymentDto>>> GetMisPagosAsync()
            => _api.GetAsync<List<PaymentDto>>("api/payment/cliente");
    }
}
