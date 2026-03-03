using FoodBook_SS.Domain.Base;
using FoodBook_SS.Domain.Entities.Payment;

namespace FoodBook_SS.Domain.Repository
{
   
    public interface IPaymentRepository : IBaseRepository<Pago>
    {
      
        Task<OperationResult> GetPagoAprobadoByOrdenIdAsync(int OrdenId);

        Task<OperationResult> GetAllByOrdenIdAsync(int OrdenId);

        Task<OperationResult> GetByClienteIdAsync(int ClienteId);

        Task<OperationResult> GetByReferenciaExternaAsync(string ReferenciaExterna);

        Task<OperationResult> GetByCodigoAutorizacionAsync(string CodigoAutorizacion);

        Task<OperationResult> RegistrarPagoAsync(Pago Pago, bool PagoAprobado, string? MensajeRespuesta);

        Task<OperationResult> ActualizarEstadoPagoAsync(int PagoId, string NuevoEstado, string? CodigoAutorizacion, string? ReferenciaExterna);

        Task<OperationResult> GetPagosPendientesVencidosAsync(int MinutosTimeout);


        Task<OperationResult> GetResumenTransaccionesAsync(int RestauranteId, DateOnly Desde, DateOnly Hasta);
    }
}