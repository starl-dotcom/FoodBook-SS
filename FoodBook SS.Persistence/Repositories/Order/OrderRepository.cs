using FoodBook_SS.Domain.Base;
using FoodBook_SS.Domain.Entities.Order;
using FoodBook_SS.Domain.Entities.Reservation;
using FoodBook_SS.Domain.Repository;
using FoodBook_SS.Persistence.Base;
using FoodBook_SS.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace FoodBook_SS.Persistence.Repositories.Order
{
    public class OrderRepository : BaseRepositorycs<Orden>, IOrderRepository
    {
        private readonly FoodBookDbContext _context;

        public OrderRepository(FoodBookDbContext context) : base(context) => _context = context;

        public async Task<OperationResult> GetByReservaIdAsync(int reservaId)
        {
            var lista = await _context.Ordenes.Include(o => o.Items)
                .Where(o => o.ReservaId == reservaId).ToListAsync();
            return OperationResult.Ok(data: lista);
        }

        public async Task<OperationResult> GetByClienteIdAsync(int clienteId)
        {
            var lista = await _context.Ordenes.Include(o => o.Items)
                .Where(o => o.ClienteId == clienteId)
                .OrderByDescending(o => o.CreadoEn).ToListAsync();
            return OperationResult.Ok(data: lista);
        }

        public async Task<OperationResult> GetByRestauranteAndEstadoAsync(int restauranteId, string estado)
        {
            var lista = await _context.Ordenes.Include(o => o.Items)
                .Where(o => o.RestauranteId == restauranteId && o.Estado == estado).ToListAsync();
            return OperationResult.Ok(data: lista);
        }

        public async Task<OperationResult> GetOrdenEntregadaParaResenaAsync(int clienteId, int ordenId)
        {
            var orden = await _context.Ordenes
                .FirstOrDefaultAsync(o => o.ClienteId == clienteId &&
                                          o.Id == ordenId &&
                                          o.Estado == EstadoOrden.Entregada);
            return OperationResult.Ok(data: orden);
        }

        public async Task<OperationResult> AddItemAsync(ItemOrden item)
        {
            _context.ItemsOrden.Add(item);
            await _context.SaveChangesAsync();
            return OperationResult.Ok(data: item.Id);
        }

        public async Task<OperationResult> RemoveItemAsync(int itemId)
        {
            var rows = await _context.ItemsOrden.Where(i => i.Id == itemId).ExecuteDeleteAsync();
            return rows > 0 ? OperationResult.Ok() : OperationResult.Fail("Ítem no encontrado.");
        }

        public async Task<OperationResult> RecalcularTotalesAsync(int ordenId)
        {
            var items = await _context.ItemsOrden.Where(i => i.OrdenId == ordenId).ToListAsync();
            var subtotal = items.Sum(i => i.PrecioUnitario * i.Cantidad);
            var impuesto = Math.Round(subtotal * 0.18m, 2);
            var total = subtotal + impuesto;

            await _context.Ordenes.Where(o => o.Id == ordenId).ExecuteUpdateAsync(s => s
                .SetProperty(o => o.Subtotal, subtotal)
                .SetProperty(o => o.Impuesto, impuesto)
                .SetProperty(o => o.Total, total)
                .SetProperty(o => o.ActualizadoEn, DateTime.UtcNow));

            return OperationResult.Ok();
        }

        public async Task<OperationResult> CambiarEstadoOrdenAsync(int ordenId, string nuevoEstado, int actorId)
        {
            var rows = await _context.Ordenes.Where(o => o.Id == ordenId).ExecuteUpdateAsync(s => s
                .SetProperty(o => o.Estado, nuevoEstado)
                .SetProperty(o => o.ModificadoPor, actorId)
                .SetProperty(o => o.ActualizadoEn, DateTime.UtcNow));
            return rows > 0 ? OperationResult.Ok() : OperationResult.Fail("Orden no encontrada.");
        }

        public async Task<OperationResult> GetVentasPorFechaAsync(int restauranteId, DateOnly desde, DateOnly hasta)
        {
            var ventas = await _context.Ordenes
                .Where(o => o.RestauranteId == restauranteId &&
                            o.Estado == EstadoOrden.Entregada &&
                            DateOnly.FromDateTime(o.CreadoEn) >= desde &&
                            DateOnly.FromDateTime(o.CreadoEn) <= hasta)
                .GroupBy(o => DateOnly.FromDateTime(o.CreadoEn))
                .Select(g => new { Fecha = g.Key, TotalVentas = g.Sum(o => o.Total), Ordenes = g.Count() })
                .OrderBy(v => v.Fecha)
                .ToListAsync();
            return OperationResult.Ok(data: ventas);
        }

        public async Task<OperationResult> GetProductosMasOrdenadosAsync(int restauranteId, DateOnly desde,
                                                                          DateOnly hasta, int top = 10)
        {
            var productos = await _context.ItemsOrden
                .Where(i => i.Orden!.RestauranteId == restauranteId &&
                            DateOnly.FromDateTime(i.CreadoEn) >= desde &&
                            DateOnly.FromDateTime(i.CreadoEn) <= hasta)
                .GroupBy(i => new { i.ProductoId, i.NombreProducto })
                .Select(g => new { g.Key.ProductoId, g.Key.NombreProducto, TotalVendido = g.Sum(i => i.Cantidad) })
                .OrderByDescending(p => p.TotalVendido)
                .Take(top)
                .ToListAsync();
            return OperationResult.Ok(data: productos);
        }
    }
}