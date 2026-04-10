using FoodBook_SS.Domain.Base;
using FoodBook_SS.Domain.Entities.Configuration;
using FoodBook_SS.Domain.Entities.Order;
using FoodBook_SS.Domain.Entities.Payment;
using FoodBook_SS.Domain.Entities.Reservation;
using FoodBook_SS.Domain.Entities.Review;
using FoodBook_SS.Domain.Entities.User;

namespace FoodBook_SS.Domain.Repository
{
    public interface IUserRepository : IBaseRepository<Usuario>
    {
        Task<Usuario?> GetByEmailAsync(string email);
        Task<bool> EmailExisteAsync(string email);
        Task<OperationResult> ActualizarRefreshTokenAsync(int id, string? token, DateTime? exp);
        Task<OperationResult> CambiarPasswordAsync(int id, string nuevoHash);
        Task<OperationResult> ActivarDesactivarAsync(int id, bool activo, int actorId);
    }

    public interface IRestaurantRepository : IBaseRepository<Restaurante>
    {
        Task<OperationResult> GetByPropietarioAsync(int propietarioId);
        Task<OperationResult> GetByPropietarioIdAsync(int propietarioId);
        Task<OperationResult> SearchAsync(string? nombre, string? ciudad, string? tipoCocina);
        Task<OperationResult> BuscarAsync(string? ciudad, string? tipoCocina, string? termino);
        Task<OperationResult> SaveMesaAsync(Mesa mesa);
        Task<OperationResult> ActualizarCalificacionAsync(int restauranteId);
    }

    public interface IReservationRepository : IBaseRepository<Reserva>
    {
        Task<OperationResult> GetByClienteIdAsync(int clienteId);
        Task<OperationResult> GetByRestauranteAndEstadoAsync(int restauranteId, string estado);
        Task<OperationResult> GetByRestauranteAndFechaAsync(int restauranteId, DateOnly fecha);
        Task<OperationResult> GetAllByRestauranteAsync(int restauranteId);
        Task<OperationResult> GetMesasDisponiblesAsync(int restauranteId, DateOnly fecha, TimeOnly hora, int personas);
        Task<OperationResult> GetByCodigoConfirmacionAsync(string codigo);
        Task<OperationResult> ConfirmarReservaAsync(int reservaId, int actorId);
        Task<OperationResult> CancelarReservaAsync(int reservaId, int actorId, string? motivo);
        Task<OperationResult> CompletarReservaAsync(int reservaId, int actorId);
        Task<OperationResult> MarcarNoShowAsync(int reservaId, int actorId);
    }

    public interface IOrderRepository : IBaseRepository<Orden>
    {
        Task<OperationResult> GetByReservaIdAsync(int reservaId);
        Task<OperationResult> GetByClienteIdAsync(int clienteId);
        Task<OperationResult> GetByRestauranteAndEstadoAsync(int restauranteId, string estado);
        Task<OperationResult> GetOrdenEntregadaParaResenaAsync(int clienteId, int reservaId);
        Task<OperationResult> AddItemAsync(ItemOrden item);
        Task<OperationResult> RemoveItemAsync(int itemId);
        Task<OperationResult> RecalcularTotalesAsync(int ordenId);
        Task<OperationResult> CambiarEstadoOrdenAsync(int ordenId, string nuevoEstado, int actorId);
        Task<OperationResult> GetVentasPorFechaAsync(int restauranteId, DateOnly desde, DateOnly hasta);
        Task<OperationResult> GetProductosMasOrdenadosAsync(int restauranteId, DateOnly desde, DateOnly hasta, int top = 10);
    }

    public interface IPaymentRepository : IBaseRepository<Pago>
    {
        Task<OperationResult> GetAllByOrdenIdAsync(int ordenId);
        Task<OperationResult> GetByClienteIdAsync(int clienteId);
        Task<OperationResult> GetResumenTransaccionesAsync(int restauranteId, DateOnly desde, DateOnly hasta);
        
        Task<OperationResult> RegistrarPagoAsync(Pago pago);
    }

    public interface IMenuRepository
    {
        Task<OperationResult> GetCategoriasByRestauranteAsync(int restauranteId);
        Task<OperationResult> GetProductosByRestauranteAsync(int restauranteId, bool soloDisponibles = false);
        Task<OperationResult> GetProductosByCategoriaAsync(int categoriaId);
        Task<OperationResult> SaveCategoriaAsync(CategoriaMenu categoria);
        Task<OperationResult> SaveProductoAsync(ProductoMenu producto);
        Task<OperationResult> UpdateProductoAsync(ProductoMenu producto);
        Task<OperationResult> CambiarDisponibilidadAsync(int productoId, bool disponible, int actorId);
        Task<ProductoMenu?> GetProductoByIdAsync(int productoId);
    }

    public interface IReviewRepository : IBaseRepository<Resena>
    {
        Task<OperationResult> GetByRestauranteAsync(int restauranteId);
        Task<OperationResult> GetByClienteIdAsync(int clienteId);
        Task<OperationResult> ModerarAsync(int resenaId, bool visible, int moderadorId);
        Task<bool> ClienteYaResenoAsync(int clienteId, int restauranteId);
        Task<OperationResult> ResponderAsync(int resenaId, string respuesta);
    }

    public interface IAuditRepository
    {
        Task InsertarAsync(int? actorId, string accion, string entidad, string? entidadId,
                           string? datosAnteriores, string? datosNuevos,
                           string resultado, string? detalle);
        Task<IEnumerable<object>> GetByEntidadAsync(string entidad, string entidadId);
        Task<IEnumerable<object>> GetAllAsync();
    }
}
