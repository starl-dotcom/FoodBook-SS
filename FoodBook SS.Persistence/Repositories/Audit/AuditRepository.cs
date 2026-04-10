using FoodBook_SS.Domain.Repository;
using Microsoft.Data.SqlClient;
using FoodBook_SS.Persistence.Context;
using Microsoft.EntityFrameworkCore;
namespace FoodBook_SS.Persistence.Repositories.Audit
{
    public class AuditRepository : IAuditRepository
    {
        private readonly FoodBookDbContext _context;
        public AuditRepository(FoodBookDbContext context) => _context = context;
        public async Task InsertarAsync(int? actorId, string accion, string entidad, string? entidadId,
                                string? datosAnteriores, string? datosNuevos,
                                string resultado, string? detalle)
        {
            var sql = @"INSERT INTO Auditoria
                    (ActorId, Accion, Entidad, EntidadId,
                     DatosAnteriores, DatosNuevos, Resultado, Detalle, FechaHora)
                VALUES
                    (@ActorId, @Accion, @Entidad, @EntidadId,
                     @DatosAnteriores, @DatosNuevos, @Resultado, @Detalle, @FechaHora)";
            var parameters = new[]
            {
        Param("@ActorId",         actorId),
        Param("@Accion",          accion),
        Param("@Entidad",         entidad),
        Param("@EntidadId",       entidadId),
        Param("@DatosAnteriores", datosAnteriores),
        Param("@DatosNuevos",     datosNuevos),
        Param("@Resultado",       resultado),
        Param("@Detalle",         detalle),
        Param("@FechaHora",       DateTime.UtcNow)
    };
            await _context.Database.ExecuteSqlRawAsync(sql, parameters);
        }
        private static SqlParameter Param(string name, object? value) =>
            new(name, value ?? DBNull.Value);
        public async Task<IEnumerable<object>> GetByEntidadAsync(string entidad, string entidadId)
        {
            var lista = await _context.Database
                .SqlQueryRaw<AuditRow>(
                    "SELECT * FROM Auditoria WHERE Entidad = {0} AND EntidadId = {1} ORDER BY FechaHora DESC",
                    entidad, entidadId)
                .ToListAsync();
            return lista;
        }
        public async Task<IEnumerable<object>> GetAllAsync()
        {
            var lista = await _context.Database
                .SqlQueryRaw<AuditRow>("SELECT * FROM Auditoria ORDER BY FechaHora DESC")
                .ToListAsync();
            return lista;
        }
    }
    public class AuditRow
    {
        public long AuditoriaId { get; set; }
        public int? ActorId { get; set; }
        public string Accion { get; set; } = string.Empty;
        public string Entidad { get; set; } = string.Empty;
        public string? EntidadId { get; set; }
        public string? DatosAnteriores { get; set; }
        public string? DatosNuevos { get; set; }
        public string Resultado { get; set; } = string.Empty;
        public string? Detalle { get; set; }
        public DateTime FechaHora { get; set; }   
    }
}

