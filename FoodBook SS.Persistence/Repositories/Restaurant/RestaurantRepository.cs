using FoodBook_SS.Domain.Base;
using FoodBook_SS.Domain.Entities.Configuration;
using FoodBook_SS.Domain.Repository;
using FoodBook_SS.Persistence.Base;
using FoodBook_SS.Persistence.Context;
using Microsoft.EntityFrameworkCore;
namespace FoodBook_SS.Persistence.Repositories.Restaurant
{
    public class RestaurantRepository : BaseRepositorycs<Restaurante>, IRestaurantRepository
    {
        private readonly FoodBookDbContext _context;
        public RestaurantRepository(FoodBookDbContext context) : base(context) => _context = context;
        public Task<OperationResult> GetByPropietarioAsync(int propietarioId) =>
            GetByPropietarioIdAsync(propietarioId);
        public async Task<OperationResult> GetByPropietarioIdAsync(int propietarioId)
        {
            var lista = await _context.Restaurantes
                .Include(r => r.Mesas)
                .Where(r => r.PropietarioId == propietarioId)
                .OrderBy(r => r.Nombre)
                .ToListAsync();
            return OperationResult.Ok(data: lista);
        }
        public Task<OperationResult> SearchAsync(string? nombre, string? ciudad, string? tipoCocina) =>
            BuscarAsync(ciudad, tipoCocina, nombre);
        public async Task<OperationResult> BuscarAsync(string? ciudad, string? tipoCocina, string? termino)
        {
            var query = _context.Restaurantes.Where(r => r.Activo).AsQueryable();
            if (!string.IsNullOrWhiteSpace(ciudad))
                query = query.Where(r => r.Ciudad.Contains(ciudad));
            if (!string.IsNullOrWhiteSpace(tipoCocina))
                query = query.Where(r => r.TipoCocina != null && r.TipoCocina.Contains(tipoCocina));
            if (!string.IsNullOrWhiteSpace(termino))
                query = query.Where(r => r.Nombre.Contains(termino) ||
                                         (r.Descripcion != null && r.Descripcion.Contains(termino)));
            var lista = await query.OrderByDescending(r => r.CalificacionProm).ToListAsync();
            return OperationResult.Ok(data: lista);
        }
        public async Task<OperationResult> GetActivosAsync()
        {
            var lista = await _context.Restaurantes.Where(r => r.Activo).OrderBy(r => r.Nombre).ToListAsync();
            return OperationResult.Ok(data: lista);
        }
        public async Task<OperationResult> SaveMesaAsync(Mesa mesa)
        {
            var existe = await _context.Mesas
                .AnyAsync(m => m.RestauranteId == mesa.RestauranteId &&
                               m.NumeroMesa    == mesa.NumeroMesa);
            if (existe)
                return OperationResult.Fail(
                    $"Ya existe una mesa con el número '{mesa.NumeroMesa}' en este restaurante.");
            _context.Mesas.Add(mesa);
            await _context.SaveChangesAsync();
            return OperationResult.Ok(mesa.Id, "Mesa creada exitosamente.");
        }
        public async Task<OperationResult> GetMesasByRestauranteAsync(int restauranteId, bool soloActivas = true)
        {
            var query = _context.Mesas.Where(m => m.RestauranteId == restauranteId);
            if (soloActivas) query = query.Where(m => m.Activa);
            var lista = await query.OrderBy(m => m.NumeroMesa).ToListAsync();
            return OperationResult.Ok(data: lista);
        }
        public async Task<OperationResult> GetHorariosAsync(int restauranteId)
        {
            var lista = await _context.Horarios
                .Where(h => h.RestauranteId == restauranteId)
                .OrderBy(h => h.DiaSemana)
                .ToListAsync();
            return OperationResult.Ok(data: lista);
        }
        public async Task<OperationResult> ActualizarCalificacionAsync(int restauranteId)
        {
            var stats = await _context.Resenas
                .Where(r => r.RestauranteId == restauranteId && r.Visible)
                .GroupBy(_ => true)
                .Select(g => new { Promedio = g.Average(r => (double)r.Calificacion), Total = g.Count() })
                .FirstOrDefaultAsync();
            await _context.Restaurantes
                .Where(r => r.Id == restauranteId)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(r => r.CalificacionProm, (decimal)(stats != null ? stats.Promedio : 0))
                    .SetProperty(r => r.TotalResenas, stats != null ? stats.Total : 0)
                    .SetProperty(r => r.ActualizadoEn, DateTime.UtcNow));
            return OperationResult.Ok();
        }
    }
}

