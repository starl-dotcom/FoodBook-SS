using FoodBook_SS.Domain.Base;
using FoodBook_SS.Domain.Entities.Reservation;

namespace FoodBook_SS.Domain.Repository
{
    
    public interface IReservationRepository : IBaseRepository<Reserva>
    {
        
        Task<OperationResult> GetByClienteIdAsync(int ClienteId);

        Task<OperationResult> GetByRestauranteAndFechaAsync(int RestauranteId, DateOnly FechaReserva);

        Task<OperationResult> GetByRestauranteAndEstadoAsync(int RestauranteId, string Estado);

        Task<OperationResult> IsMesaDisponibleAsync(int MesaId, DateOnly FechaReserva, TimeOnly HoraReserva);

        Task<OperationResult> GetMesasDisponiblesAsync(int RestauranteId, DateOnly FechaReserva, TimeOnly HoraReserva, int NumeroPersonas);

        Task<OperationResult> GetByCodigoConfirmacionAsync(string CodigoConfirmacion);

        Task<OperationResult> GetReservaActivaAsync(int ClienteId, int RestauranteId, DateOnly FechaReserva);

        Task<OperationResult> ConfirmarReservaAsync(int ReservaId, int ActorId);

        Task<OperationResult> CancelarReservaAsync(int ReservaId, int ActorId, string? Motivo);

        Task<OperationResult> CompletarReservaAsync(int ReservaId, int ActorId);

        Task<OperationResult> MarcarNoShowAsync(int ReservaId, int ActorId);

        Task<OperationResult> GetReservasParaRecordatorioAsync(DateTime Desde, DateTime Hasta);

        Task<OperationResult> GetConteosPorEstadoAsync(int RestauranteId, DateOnly Desde, DateOnly Hasta);
    }
}