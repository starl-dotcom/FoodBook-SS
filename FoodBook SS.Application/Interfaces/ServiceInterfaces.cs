using FoodBook_SS.Application.Base;
using FoodBook_SS.Application.Dtos.Menu;
using FoodBook_SS.Application.Dtos.Order;
using FoodBook_SS.Application.Dtos.Payment;
using FoodBook_SS.Application.Dtos.Restaurant;
using FoodBook_SS.Application.Dtos.Reservation;
using FoodBook_SS.Application.Dtos.Review;
using FoodBook_SS.Application.Dtos.User;
using FoodBook_SS.Domain.Base;

namespace FoodBook_SS.Application.Interfaces
{
    
    public interface IUserService : IBaseService<UserDto, SaveUserDto, UpdateUserDto>
    {
        Task<OperationResult> LoginAsync(LoginDto dto);
        Task<OperationResult> RefreshTokenAsync(string refreshToken);
        Task<OperationResult> CambiarPasswordAsync(int usuarioId, string actual, string nueva);
        
        Task<OperationResult> ChangePasswordAsync(int usuarioId, string actual, string nueva);
        Task<OperationResult> UpdateAsync(int id, UpdateUserDto dto, int actorId);
        Task<OperationResult> ActivarDesactivarAsync(int usuarioId, bool activo, int actorId);
    }


    public interface IRestaurantService : IBaseService<RestaurantDto, SaveRestaurantDto, UpdateRestaurantDto>
    {
        Task<OperationResult> GetByPropietarioAsync(int propietarioId);
        Task<OperationResult> SearchAsync(string? nombre, string? ciudad, string? tipoCocina);
        Task<OperationResult> BuscarAsync(string? ciudad, string? tipoCocina, string? termino);
        Task<OperationResult> CreateAsync(SaveRestaurantDto dto, int propietarioId);
    }


    public interface IReservationService : IBaseService<ReservationDto, SaveReservationDto, UpdateReservationDto>
    {
        Task<OperationResult> GetByClienteAsync(int clienteId);
        Task<OperationResult> GetAllByClienteAsync(int clienteId);
        Task<OperationResult> GetByRestauranteAsync(int restauranteId, string? estado);
        Task<OperationResult> GetAllByRestauranteAsync(int restauranteId, string? estado);
        Task<OperationResult> GetByCodigoAsync(string codigo);
        Task<OperationResult> GetMesasDisponiblesAsync(int restauranteId, DateOnly fecha, TimeOnly hora, int personas);
        Task<OperationResult> CreateAsync(SaveReservationDto dto, int clienteId);
        Task<OperationResult> UpdateAsync(int id, UpdateReservationDto dto, int actorId);
        Task<OperationResult> ConfirmarAsync(int reservaId, int actorId);
        Task<OperationResult> CancelarAsync(int reservaId, int actorId, string? motivo);
        Task<OperationResult> CompletarAsync(int reservaId, int actorId);
        Task<OperationResult> MarcarNoShowAsync(int reservaId, int actorId);
    }

    
    public interface IOrderService : IBaseService<OrderDto, SaveOrderDto, SaveOrderDto>
    {
        Task<OperationResult> GetByClienteAsync(int clienteId);
        Task<OperationResult> GetByRestauranteAsync(int restauranteId, string? estado);
        Task<OperationResult> GetByReservaAsync(int reservaId);
        Task<OperationResult> CreateAsync(SaveOrderDto dto, int clienteId);
        Task<OperationResult> AgregarItemAsync(int ordenId, SaveOrderItemDto item);
        Task<OperationResult> AddItemAsync(int ordenId, SaveOrderItemDto item, int actorId);
        Task<OperationResult> RemoverItemAsync(int ordenId, int itemId);
        Task<OperationResult> RemoveItemAsync(int itemId, int actorId);
        Task<OperationResult> CambiarEstadoAsync(int ordenId, string nuevoEstado, int actorId);
        Task<OperationResult> GetVentasAsync(int restauranteId, DateOnly desde, DateOnly hasta);
        Task<OperationResult> GetProductosMasOrdenadosAsync(int restauranteId, DateOnly desde, DateOnly hasta);
    }

    
    public interface IPaymentService : IBaseService<PaymentDto, SavePaymentDto, SavePaymentDto>
    {
        Task<OperationResult> GetByOrdenAsync(int ordenId);
        Task<OperationResult> GetByClienteAsync(int clienteId);
        Task<OperationResult> ProcesarPagoAsync(SavePaymentDto dto, int clienteId);
        Task<OperationResult> ProcessAsync(SavePaymentDto dto, int clienteId);
        Task<OperationResult> GetResumenAsync(int restauranteId, DateOnly desde, DateOnly hasta);
    }

    
    public interface IMenuService
    {
        Task<OperationResult> GetCategoriasByRestauranteAsync(int restauranteId);
        Task<OperationResult> GetProductosByRestauranteAsync(int restauranteId);
        Task<OperationResult> GetProductosByRestauranteAsync(int restauranteId, bool soloDisponibles);
        Task<OperationResult> GetProductosByCategoriaAsync(int categoriaId);
        Task<OperationResult> SaveCategoriaAsync(SaveCategoryDto dto);
        Task<OperationResult> SaveProductoAsync(SaveProductDto dto);
        Task<OperationResult> UpdateProductoAsync(int productoId, UpdateProductDto dto);
        Task<OperationResult> ToggleDisponibilidadAsync(int productoId, int actorId);
    }

    
    public interface IReviewService : IBaseService<ReviewDto, SaveReviewDto, SaveReviewDto>
    {
        Task<OperationResult> GetByRestauranteAsync(int restauranteId);
        Task<OperationResult> GetByClienteAsync(int clienteId);
        Task<OperationResult> CreateAsync(SaveReviewDto dto, int clienteId);
        Task<OperationResult> ModerarAsync(int resenaId, bool visible, int moderadorId);
    }

    
    public interface INotificationService
    {
        Task<OperationResult> EnviarConfirmacionReservaAsync(int clienteId, int reservaId);
        Task<OperationResult> EnviarCancelacionReservaAsync(int clienteId, int reservaId, string? motivo);
        Task<OperationResult> EnviarConfirmacionPagoAsync(int clienteId, int ordenId, decimal monto);
        Task<OperationResult> EnviarRecordatorioReservaAsync(int clienteId, int reservaId);
    }

    
    public interface INotificationSender
    {
        Task EnviarAsync(string destinatario, string asunto, string mensaje);
    }

    
    public interface IAuditService
    {
        Task RegistrarAsync(int? actorId, string accion, string entidad, string? entidadId,
                            object? datosAnteriores = null, object? datosNuevos = null,
                            string resultado = "Exito", string? detalle = null);
        Task<OperationResult> GetMetricasAsync(int restauranteId, DateOnly desde, DateOnly hasta);
    }
}