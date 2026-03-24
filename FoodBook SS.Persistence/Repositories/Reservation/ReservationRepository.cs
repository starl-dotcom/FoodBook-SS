using FoodBook_SS.Domain.Base;
using FoodBook_SS.Domain.Entities.Reservation;
using FoodBook_SS.Domain.Repository;
using FoodBook_SS.Persistence.Base;
using FoodBook_SS.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace FoodBook_SS.Persistence.Repositories.Reservation
{
    public class ReservationRepository : BaseRepositorycs<Reserva>, IReservationRepository
    {
        private readonly FoodBookDbContext _context;

        public ReservationRepository(FoodBookDbContext context) : base(context) => _context = context;

        public async Task<OperationResult> GetByClienteIdAsync(int clienteId)
        {
            var lista = await _context.Reservas
                .Include(r => r.Restaurante).Include(r => r.Mesa)
                .Where(r => r.ClienteId == clienteId)
                .OrderByDescending(r => r.FechaReserva).ToListAsync();
            return OperationResult.Ok(data: lista);
        }

        public async Task<OperationResult> GetByRestauranteAndFechaAsync(int restauranteId, DateOnly fecha)
        {
            var lista = await _context.Reservas
                .Include(r => r.Cliente)
                .Where(r => r.RestauranteId == restauranteId && r.FechaReserva == fecha)
                .ToListAsync();
            return OperationResult.Ok(data: lista);
        }

        public async Task<OperationResult> GetByRestauranteAndEstadoAsync(int restauranteId, string estado)
        {
            var lista = await _context.Reservas
                .Include(r => r.Cliente)
                .Where(r => r.RestauranteId == restauranteId && r.Estado == estado)
                .ToListAsync();
            return OperationResult.Ok(data: lista);
        }

        public async Task<OperationResult> IsMesaDisponibleAsync(int mesaId, DateOnly fecha, TimeOnly hora)
        {
            var ocupada = await _context.Reservas.AnyAsync(r =>
                r.MesaId == mesaId &&
                r.FechaReserva == fecha &&
                r.HoraReserva == hora &&
                r.Estado != EstadoReserva.Cancelada &&
                r.Estado != EstadoReserva.NoShow);
            return OperationResult.Ok(data: !ocupada);
        }

        public async Task<OperationResult> GetMesasDisponiblesAsync(int restauranteId, DateOnly fecha,
                                                                     TimeOnly hora, int personas)
        {
            var mesasOcupadas = await _context.Reservas
                .Where(r => r.RestauranteId == restauranteId &&
                            r.FechaReserva == fecha &&
                            r.HoraReserva == hora &&
                            r.Estado != EstadoReserva.Cancelada &&
                            r.Estado != EstadoReserva.NoShow)
                .Select(r => r.MesaId)
                .ToListAsync();

            var mesasDisponibles = await _context.Mesas
                .Where(m => m.RestauranteId == restauranteId &&
                            m.Activa == true &&
                            m.Capacidad >= personas &&
                            !mesasOcupadas.Contains((int?)m.Id))
                .OrderBy(m => m.Capacidad)
                .ToListAsync();

            return mesasDisponibles.Any()
                ? OperationResult.Ok(data: mesasDisponibles)
                : OperationResult.Fail("No hay mesas disponibles para la fecha, hora y número de personas.");
        }

        public async Task<OperationResult> GetByCodigoConfirmacionAsync(string codigo)
        {
            var r = await _context.Reservas
                .Include(r => r.Cliente).Include(r => r.Restaurante).Include(r => r.Mesa)
                .FirstOrDefaultAsync(r => r.CodigoConfirmacion == codigo);
            return r is null
                ? OperationResult.Fail("Código de confirmación no encontrado.")
                : OperationResult.Ok(data: r);
        }

        public async Task<OperationResult> GetReservaActivaAsync(int clienteId, int restauranteId, DateOnly fecha)
        {
            var r = await _context.Reservas
                .FirstOrDefaultAsync(r => r.ClienteId == clienteId &&
                                          r.RestauranteId == restauranteId &&
                                          r.FechaReserva == fecha &&
                                          (r.Estado == EstadoReserva.Pendiente || r.Estado == EstadoReserva.Confirmada));
            return OperationResult.Ok(data: r);
        }

        public async Task<OperationResult> ConfirmarReservaAsync(int reservaId, int actorId) =>
            await CambiarEstadoAsync(reservaId, EstadoReserva.Confirmada, actorId);

        public async Task<OperationResult> CancelarReservaAsync(int reservaId, int actorId, string? motivo)
        {
            var reserva = await _context.Reservas.FindAsync(reservaId);
            if (reserva is null) return OperationResult.Fail("Reserva no encontrada.");

            var estadosNoCancelables = new[] { EstadoReserva.Cancelada, EstadoReserva.Completada, EstadoReserva.NoShow };
            if (estadosNoCancelables.Contains(reserva.Estado))
                return OperationResult.Fail($"No se puede cancelar una reserva en estado '{reserva.Estado}'.");

            reserva.Estado = EstadoReserva.Cancelada;
            reserva.ModificadoPor = actorId;
            reserva.ActualizadoEn = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return OperationResult.Ok("Reserva cancelada.");
        }


        public async Task<OperationResult> CompletarReservaAsync(int reservaId, int actorId)
        {
            var reserva = await _context.Reservas.FindAsync(reservaId);
            if (reserva is null) return OperationResult.Fail("Reserva no encontrada.");

            reserva.Estado = EstadoReserva.Completada;
            reserva.ModificadoPor = actorId;
            reserva.ActualizadoEn = DateTime.UtcNow;

            var ordenes = await _context.Ordenes
                .Where(o => o.ReservaId == reservaId &&
                            (o.Estado == EstadoOrden.Confirmada ||
                             o.Estado == EstadoOrden.EnPreparacion ||
                             o.Estado == EstadoOrden.Lista))
                .ToListAsync();

            foreach (var orden in ordenes)
            {
                orden.Estado = EstadoOrden.Entregada;
                orden.ActualizadoEn = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
            return OperationResult.Ok("Reserva marcada como Completada.");
        }

        public Task<OperationResult> MarcarNoShowAsync(int reservaId, int actorId) =>
            CambiarEstadoAsync(reservaId, EstadoReserva.NoShow, actorId);

        public async Task<OperationResult> GetReservasParaRecordatorioAsync(DateTime desde, DateTime hasta)
        {
            var desdeDate = DateOnly.FromDateTime(desde);
            var hastaDate = DateOnly.FromDateTime(hasta);
            var lista = await _context.Reservas
                .Include(r => r.Cliente)
                .Where(r => r.FechaReserva >= desdeDate &&
                            r.FechaReserva <= hastaDate &&
                            r.Estado == EstadoReserva.Confirmada)
                .ToListAsync();
            return OperationResult.Ok(data: lista);
        }

        public async Task<OperationResult> GetConteosPorEstadoAsync(int restauranteId, DateOnly desde, DateOnly hasta)
        {
            var conteos = await _context.Reservas
                .Where(r => r.RestauranteId == restauranteId &&
                            r.FechaReserva >= desde && r.FechaReserva <= hasta)
                .GroupBy(r => r.Estado)
                .Select(g => new { Estado = g.Key, Total = g.Count() })
                .ToListAsync();
            return OperationResult.Ok(data: conteos);
        }

        private async Task<OperationResult> CambiarEstadoAsync(int reservaId, string nuevoEstado, int actorId)
        {
            var reserva = await _context.Reservas.FindAsync(reservaId);
            if (reserva is null) return OperationResult.Fail("Reserva no encontrada.");

            reserva.Estado = nuevoEstado;
            reserva.ModificadoPor = actorId;
            reserva.ActualizadoEn = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return OperationResult.Ok($"Reserva marcada como {nuevoEstado}.");
        }
    }
}