using FoodBook_SS.Domain.Base;
using FoodBook_SS.Domain.Entities.Order;

namespace FoodBook_SS.Domain.Repository
{
   
    public interface IOrderRepository : IBaseRepository<Orden>
    {
        Task<OperationResult> GetByReservaIdAsync(int ReservaId);

        Task<OperationResult> GetByClienteIdAsync(int ClienteId);

        Task<OperationResult> GetByRestauranteAndEstadoAsync(int RestauranteId, string Estado);

        Task<OperationResult> ClienteTieneOrdenEntregadaAsync(int ClienteId, int RestauranteId);

        Task<OperationResult> GetOrdenEntregadaParaResenaAsync(int ClienteId, int ReservaId);

        Task<OperationResult> GetItemsByOrdenIdAsync(int OrdenId);

        Task<OperationResult> AddItemAsync(ItemOrden Item);

        Task<OperationResult> RemoveItemAsync(int ItemId);

        Task<OperationResult> RecalcularTotalesAsync(int OrdenId);

        Task<OperationResult> CambiarEstadoOrdenAsync(int OrdenId, string NuevoEstado, int ActorId);

        Task<OperationResult> GetVentasPorFechaAsync(int RestauranteId, DateOnly Desde, DateOnly Hasta);

        Task<OperationResult> GetProductosMasOrdenadosAsync(int RestauranteId, DateOnly Desde, DateOnly Hasta, int Top = 10);
    }
}
