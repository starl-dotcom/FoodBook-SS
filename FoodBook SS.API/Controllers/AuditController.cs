using FoodBook_SS.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodBook_SS.API.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class AuditController : BaseApiController
    {
        private readonly IAuditService _service;
        public AuditController(IAuditService service) => _service = service;

        [HttpGet("metricas/{restauranteId:int}")]
        public async Task<IActionResult> GetMetricas(int restauranteId,
            [FromQuery] DateOnly desde, [FromQuery] DateOnly hasta)
            => Respond(await _service.GetMetricasAsync(restauranteId, desde, hasta));

        [HttpGet("logs")]
        public async Task<IActionResult> GetAllLogs()
            => Respond(await _service.GetAllLogsAsync());
    }
}

