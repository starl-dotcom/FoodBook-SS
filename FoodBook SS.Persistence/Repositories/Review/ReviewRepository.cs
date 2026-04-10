using FoodBook_SS.Domain.Base;
using FoodBook_SS.Domain.Entities.Review;
using FoodBook_SS.Domain.Repository;
using FoodBook_SS.Persistence.Base;
using FoodBook_SS.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace FoodBook_SS.Persistence.Repositories.Review
{
    public class ReviewRepository : BaseRepositorycs<Resena>, IReviewRepository
    {
        private readonly FoodBookDbContext _context;

        public ReviewRepository(FoodBookDbContext context) : base(context) => _context = context;

        
        public Task<OperationResult> GetByRestauranteAsync(int restauranteId) =>
            GetByRestauranteIdAsync(restauranteId, soloVisibles: true);

        public async Task<OperationResult> GetByRestauranteIdAsync(int restauranteId, bool soloVisibles = true)
        {
            var query = _context.Resenas
                .Include(r => r.Cliente)
                .Where(r => r.RestauranteId == restauranteId);
            if (soloVisibles) query = query.Where(r => r.Visible);
            var lista = await query.OrderByDescending(r => r.CreadoEn).ToListAsync();
            return OperationResult.Ok(data: lista);
        }

        
        public async Task<OperationResult> GetByClienteIdAsync(int clienteId)
        {
            var lista = await _context.Resenas
                .Include(r => r.Restaurante)
                .Where(r => r.ClienteId == clienteId)
                .OrderByDescending(r => r.CreadoEn)
                .ToListAsync();
            return OperationResult.Ok(data: lista);
        }

        public Task<bool> ClienteYaResenoAsync(int clienteId, int restauranteId) =>
            _context.Resenas.AnyAsync(r => r.ClienteId == clienteId && r.RestauranteId == restauranteId);

        
        public async Task<OperationResult> ModerarAsync(int resenaId, bool visible, int moderadorId)
        {
            var rows = await _context.Resenas
                .Where(r => r.Id == resenaId)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(r => r.Visible, visible)
                    .SetProperty(r => r.ModificadoPor, moderadorId)
                    .SetProperty(r => r.ActualizadoEn, DateTime.UtcNow));
            return rows > 0 ? OperationResult.Ok() : OperationResult.Fail("Reseña no encontrada.");
        }

        public async Task<OperationResult> ResponderAsync(int resenaId, string respuesta)
        {
            var rows = await _context.Resenas
                .Where(r => r.Id == resenaId)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(r => r.Respuesta, respuesta)
                    .SetProperty(r => r.FechaRespuesta, DateTime.UtcNow)
                    .SetProperty(r => r.ActualizadoEn, DateTime.UtcNow));
            return rows > 0 ? OperationResult.Ok() : OperationResult.Fail("Reseña no encontrada.");
        }
    }
}
