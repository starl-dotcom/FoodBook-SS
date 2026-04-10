using FoodBook_SS.Desktop.Core;
using System.Text.Json.Serialization;
namespace FoodBook_SS.Desktop.Services
{
    public class AuditLogDto
    {
        public int      Id        { get; set; }
        public int?     ActorId   { get; set; }
        public string   Accion    { get; set; } = string.Empty;
        public string   Entidad   { get; set; } = string.Empty;
        public string?  EntidadId { get; set; }
        public string   Resultado { get; set; } = string.Empty;
        public string?  Detalle   { get; set; }
        public DateTime FechaHora { get; set; }
        [JsonIgnore]
        public bool EsExito => Resultado?.Equals("Exito", StringComparison.OrdinalIgnoreCase) ?? false;
    }
    public interface IAuditoriaService
    {
        Task<ApiResult<List<AuditLogDto>>> GetLogsAsync();
    }
    public class AuditoriaService : IAuditoriaService
    {
        private readonly ApiClient _api;
        public AuditoriaService(ApiClient api) => _api = api;
        public Task<ApiResult<List<AuditLogDto>>> GetLogsAsync()
            => _api.GetAsync<List<AuditLogDto>>("api/audit/logs");
    }
}

