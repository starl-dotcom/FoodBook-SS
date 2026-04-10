using FoodBook_SS.Application.Interfaces;
using FoodBook_SS.Domain.Base;
using FoodBook_SS.Domain.Entities.Audit;
using FoodBook_SS.Domain.Repository;
using System.Text.Json;

namespace FoodBook_SS.Application.Services
{
    public class AuditService : IAuditService
    {
        private readonly IAuditRepository _repo;
        public AuditService(IAuditRepository repo) => _repo = repo;
        public async Task RegistrarAsync(int? actorId, string accion, string entidad, string? entidadId,
                                          object? datosAnteriores = null, object? datosNuevos = null,
                                          string resultado = "Exito", string? detalle = null)
        {
            await _repo.InsertarAsync(actorId, accion, entidad, entidadId ?? string.Empty,
                datosAnteriores is null ? null : JsonSerializer.Serialize(datosAnteriores),
                datosNuevos is null ? null : JsonSerializer.Serialize(datosNuevos),
                resultado, detalle);
        }
        public Task<OperationResult> GetMetricasAsync(int restauranteId, DateOnly desde, DateOnly hasta)
            => Task.FromResult(OperationResult.Ok());

        public async Task<OperationResult> GetAllLogsAsync()
        {
            var logs = await _repo.GetAllAsync();
            return OperationResult.Ok(data: logs);
        }
    }

    public class NotificationService : INotificationService
    {
        private readonly INotificationSender _sender;
        private readonly IUserRepository _users;
        public NotificationService(INotificationSender sender, IUserRepository users)
        { _sender = sender; _users = users; }

        public async Task<OperationResult> EnviarConfirmacionReservaAsync(int clienteId, int reservaId)
        {
            var u = await _users.GetEntityByIdAsync(clienteId);
            if (u is null) return OperationResult.Fail("Cliente no encontrado.");
            await _sender.EnviarAsync(u.Email, "Confirmación de Reserva", $"Tu reserva #{reservaId} fue creada exitosamente.");
            return OperationResult.Ok();
        }
        public async Task<OperationResult> EnviarCancelacionReservaAsync(int clienteId, int reservaId, string? motivo)
        {
            var u = await _users.GetEntityByIdAsync(clienteId);
            if (u is null) return OperationResult.Fail("Cliente no encontrado.");
            await _sender.EnviarAsync(u.Email, "Reserva Cancelada", $"Tu reserva #{reservaId} fue cancelada. Motivo: {motivo ?? "No especificado"}");
            return OperationResult.Ok();
        }
        public async Task<OperationResult> EnviarConfirmacionPagoAsync(int clienteId, int ordenId, decimal monto)
        {
            var u = await _users.GetEntityByIdAsync(clienteId);
            if (u is null) return OperationResult.Fail("Cliente no encontrado.");
            await _sender.EnviarAsync(u.Email, "Pago Confirmado", $"Tu pago de RD${monto:N2} para la orden #{ordenId} fue aprobado.");
            return OperationResult.Ok();
        }
        public async Task<OperationResult> EnviarRecordatorioReservaAsync(int clienteId, int reservaId)
        {
            var u = await _users.GetEntityByIdAsync(clienteId);
            if (u is null) return OperationResult.Fail("Cliente no encontrado.");
            await _sender.EnviarAsync(u.Email, "Recordatorio de Reserva", $"Tu reserva #{reservaId} es mañana. ¡Te esperamos!");
            return OperationResult.Ok();
        }
    }
}
