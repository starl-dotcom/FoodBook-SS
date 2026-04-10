using FoodBook_SS.Application.Dtos.Payment;

namespace FoodBook_SS.Web.Models
{
    public class PagoViewModel
    {
        public SavePaymentDto PagoDto { get; set; } = new SavePaymentDto();
        public FoodBook_SS.Domain.Entities.Order.Orden? Orden { get; set; }
    }
}
