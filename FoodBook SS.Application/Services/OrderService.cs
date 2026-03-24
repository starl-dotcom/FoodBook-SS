using FoodBook_SS.Application.Base;
using FoodBook_SS.Application.Dtos.Order;
using FoodBook_SS.Application.Interfaces;
using FoodBook_SS.Domain.Base;
using FoodBook_SS.Domain.Entities.Order;
using FoodBook_SS.Domain.Entities.Reservation;
using FoodBook_SS.Domain.Repository;

namespace FoodBook_SS.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _repo;
        private readonly IMenuRepository _menu;
        private readonly IAuditService _audit;

        public OrderService(IOrderRepository repo, IMenuRepository menu, IAuditService audit)
        { _repo = repo; _menu = menu; _audit = audit; }

        public Task<OperationResult> GetAllAsync() => _repo.GetAllAsync(o => true);
        public Task<OperationResult> GetByReservaAsync(int id) => _repo.GetByReservaIdAsync(id);
        public Task<OperationResult> GetByClienteAsync(int clienteId) => _repo.GetByClienteIdAsync(clienteId);

        public Task<OperationResult> GetByRestauranteAsync(int restauranteId, string? estado) =>
            _repo.GetByRestauranteAndEstadoAsync(restauranteId, estado ?? EstadoOrden.Pendiente);

        public async Task<OperationResult> GetByIdAsync(int id)
        {
            var o = await _repo.GetEntityByIdAsync(id);
            return o is null ? OperationResult.Fail("Orden no encontrada.") : OperationResult.Ok(o);
        }

        public async Task<OperationResult> SaveAsync(SaveOrderDto dto) => await CreateAsync(dto, 0);

        public async Task<OperationResult> CreateAsync(SaveOrderDto dto, int clienteId)
        {
            var orden = new Orden
            {
                ReservaId = dto.ReservaId,
                ClienteId = clienteId,
                RestauranteId = dto.RestauranteId,
                Notas = dto.Notas,
                Estado = EstadoOrden.Pendiente
            };
            var r = await _repo.SaveEntityAsync(orden);
            if (!r.Success) return r;
            foreach (var item in dto.Items)
            {
                var ar = await AgregarItemAsync(orden.Id, item);
                if (!ar.Success) return ar;
            }
            return OperationResult.Ok(orden.Id);
        }

        public async Task<OperationResult> UpdateAsync(int id, SaveOrderDto dto)
        {
            var o = await _repo.GetEntityByIdAsync(id);
            if (o is null) return OperationResult.Fail("Orden no encontrada.");
            if (o.Estado != EstadoOrden.Pendiente) return OperationResult.Fail("Solo se pueden modificar órdenes Pendientes.");
            o.Notas = dto.Notas;
            return await _repo.UpdateEntityAsync(o);
        }

        public async Task<OperationResult> AgregarItemAsync(int ordenId, SaveOrderItemDto itemDto)
        {
            var producto = await _menu.GetProductoByIdAsync(itemDto.ProductoId);
            if (producto is null || !producto.Disponible) return OperationResult.Fail("Producto no disponible.");
            var item = new ItemOrden
            {
                OrdenId = ordenId,
                ProductoId = itemDto.ProductoId,
                NombreProducto = producto.Nombre,
                PrecioUnitario = producto.Precio,
                Cantidad = itemDto.Cantidad,
                Notas = itemDto.Notas
            };
            var r = await _repo.AddItemAsync(item);
            if (r.Success) await _repo.RecalcularTotalesAsync(ordenId);
            return r;
        }

        public Task<OperationResult> AddItemAsync(int ordenId, SaveOrderItemDto item, int actorId) =>
            AgregarItemAsync(ordenId, item);

        public async Task<OperationResult> RemoverItemAsync(int ordenId, int itemId)
        {
            var r = await _repo.RemoveItemAsync(itemId);
            if (r.Success) await _repo.RecalcularTotalesAsync(ordenId);
            return r;
        }

        public Task<OperationResult> RemoveItemAsync(int itemId, int actorId) =>
            _repo.RemoveItemAsync(itemId);

        public async Task<OperationResult> CambiarEstadoAsync(int ordenId, string nuevoEstado, int actorId)
        {
            var r = await _repo.CambiarEstadoOrdenAsync(ordenId, nuevoEstado, actorId);
            if (r.Success) await _audit.RegistrarAsync(actorId, $"CAMBIO_ESTADO_ORDEN_{nuevoEstado}", "Orden", ordenId.ToString());
            return r;
        }

        public Task<OperationResult> GetVentasAsync(int restauranteId, DateOnly desde, DateOnly hasta) =>
            _repo.GetVentasPorFechaAsync(restauranteId, desde, hasta);

        public Task<OperationResult> GetProductosMasOrdenadosAsync(int restauranteId, DateOnly desde, DateOnly hasta) =>
            _repo.GetProductosMasOrdenadosAsync(restauranteId, desde, hasta);
    }
}