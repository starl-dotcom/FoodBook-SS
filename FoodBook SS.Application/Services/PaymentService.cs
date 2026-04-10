using FoodBook_SS.Application.Base;
using FoodBook_SS.Application.Dtos.Payment;
using FoodBook_SS.Application.Interfaces;
using FoodBook_SS.Domain.Base;
using FoodBook_SS.Domain.Entities.Payment;
using FoodBook_SS.Domain.Entities.Reservation;
using FoodBook_SS.Domain.Repository;
namespace FoodBook_SS.Application.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _repo;
        private readonly IPaymentGateway _gateway;
        private readonly IAuditService _audit;
        private readonly INotificationService _notify;
        private readonly IOrderService _orders;
        public PaymentService(IPaymentRepository repo, IPaymentGateway gateway,
                              IAuditService audit, INotificationService notify, IOrderService orders)
        { _repo = repo; _gateway = gateway; _audit = audit; _notify = notify; _orders = orders; }
        public Task<OperationResult> GetAllAsync() => _repo.GetAllAsync(p => true);
        public Task<OperationResult> GetByOrdenAsync(int id) => _repo.GetAllByOrdenIdAsync(id);
        public Task<OperationResult> GetByClienteAsync(int clienteId) => _repo.GetByClienteIdAsync(clienteId);
        public async Task<OperationResult> GetByIdAsync(int id)
        {
            var p = await _repo.GetEntityByIdAsync(id);
            return p is null ? OperationResult.Fail("Pago no encontrado.") : OperationResult.Ok(MapToDto(p));
        }
        public Task<OperationResult> SaveAsync(SavePaymentDto dto) => ProcesarPagoAsync(dto, 0);
        public Task<OperationResult> UpdateAsync(int id, SavePaymentDto dto) =>
            Task.FromResult(OperationResult.Fail("Los pagos no se actualizan directamente."));
        public async Task<OperationResult> ProcesarPagoAsync(SavePaymentDto dto, int clienteId)
        {
            var gwResult = await _gateway.ProcesarPagoAsync(Guid.NewGuid().ToString("N"), dto.Monto, dto.MetodoPago);
            var pago = new Pago
            {
                OrdenId = dto.OrdenId,
                ClienteId = clienteId,
                Monto = dto.Monto,
                MetodoPago = dto.MetodoPago,
                CodigoAutorizacion = gwResult.CodigoAutorizacion,
                MensajeRespuesta = gwResult.Mensaje,
                Estado = gwResult.Aprobado ? EstadoPago.Aprobado : EstadoPago.Rechazado,
                FechaPago = gwResult.Aprobado ? DateTime.UtcNow : null
            };
            var r = await _repo.RegistrarPagoAsync(pago);
            if (!r.Success) return r;
            if (gwResult.Aprobado)
            {
                await _orders.CambiarEstadoAsync(dto.OrdenId, EstadoOrden.Confirmada, clienteId);
                await _notify.EnviarConfirmacionPagoAsync(clienteId, dto.OrdenId, dto.Monto);
            }
            await _audit.RegistrarAsync(clienteId, "PROCESS_PAGO", "Pago", pago.Id.ToString(),
                datosNuevos: new { pago.Monto, pago.Estado },
                resultado: gwResult.Aprobado ? "Exito" : "Fallo");
            return OperationResult.Ok(MapToDto(pago));
        }
        public Task<OperationResult> ProcessAsync(SavePaymentDto dto, int clienteId) =>
            ProcesarPagoAsync(dto, clienteId);
        public Task<OperationResult> GetResumenAsync(int restauranteId, DateOnly desde, DateOnly hasta) =>
            _repo.GetResumenTransaccionesAsync(restauranteId, desde, hasta);
        private static PaymentDto MapToDto(Pago p) => new()
        {
            Id = p.Id,
            OrdenId = p.OrdenId,
            Monto = p.Monto,
            MetodoPago = p.MetodoPago,
            Estado = p.Estado,
            CodigoAutorizacion = p.CodigoAutorizacion,
            FechaPago = p.FechaPago
        };
    }
}

