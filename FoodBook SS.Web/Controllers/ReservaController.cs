using FoodBook_SS.Application.Dtos.Reservation;
using FoodBook_SS.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
namespace FoodBook_SS.Web.Controllers
{
    [Authorize]
    public class ReservaController : Controller
    {
        private readonly IReservationService _reservationService;
        public ReservaController(IReservationService reservationService) =>
            _reservationService = reservationService;
        public async Task<IActionResult> Index(string? estado, string? fecha)
        {
            if (User.IsInRole("Propietario") || User.IsInRole("Administrador"))
                return RedirectToAction("ReservasDelDia", "Dashboard");
            var result = await _reservationService.GetAllByClienteAsync(ObtenerUsuarioId());
            var reservas = (result.Data as IEnumerable<FoodBook_SS.Domain.Entities.Reservation.Reserva>)
                           ?? Enumerable.Empty<FoodBook_SS.Domain.Entities.Reservation.Reserva>();
            if (!string.IsNullOrWhiteSpace(estado))
                reservas = reservas.Where(r => r.Estado == estado);
            if (DateOnly.TryParse(fecha, out var fechaFiltro))
                reservas = reservas.Where(r => r.FechaReserva == fechaFiltro);
            ViewBag.EstadoFiltro = estado;
            ViewBag.FechaFiltro  = fecha;
            return View(reservas);
        }
        [HttpGet]
        public IActionResult Crear(int restauranteId)
        {
            if (User.IsInRole("Propietario") || User.IsInRole("Administrador"))
                return RedirectToAction("ReservasDelDia", "Dashboard");
            return View(new SaveReservationDto { RestauranteId = restauranteId });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(SaveReservationDto dto)
        {
            if (User.IsInRole("Propietario") || User.IsInRole("Administrador"))
                return RedirectToAction("ReservasDelDia", "Dashboard");
            if (!ModelState.IsValid) return View(dto);
            var result = await _reservationService.CreateAsync(dto, ObtenerUsuarioId());
            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.Message);
                return View(dto);
            }
            TempData["Mensaje"] = "¡Reserva creada exitosamente!";
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> MesasDisponibles(int restauranteId, string fecha, string hora, int personas)
        {
            if (!DateOnly.TryParse(fecha, out var dFecha)) return BadRequest("Fecha inválida.");
            if (!TimeOnly.TryParse(hora, out var dHora)) return BadRequest("Hora inválida.");
            var res = await _reservationService.GetMesasDisponiblesAsync(restauranteId, dFecha, dHora, personas);
            if (!res.Success || res.Data is not IEnumerable<FoodBook_SS.Domain.Entities.Configuration.Mesa> mesas || !mesas.Any())
                return Json(new { success = false, message = "No hay mesas." });
            var result = mesas.Select(m => new { m.Id, m.NumeroMesa, m.Capacidad });
            return Json(new { success = true, data = result });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancelar(int id, string? motivo)
        {
            var result = await _reservationService.CancelarAsync(id, ObtenerUsuarioId(), motivo);
            TempData[result.Success ? "Mensaje" : "Error"] =
                result.Success ? "Reserva cancelada." : result.Message;
            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Propietario,Administrador")]
        public async Task<IActionResult> Confirmar(int id)
        {
            var result = await _reservationService.ConfirmarAsync(id, ObtenerUsuarioId());
            TempData[result.Success ? "Mensaje" : "Error"] =
                result.Success ? "Reserva confirmada mediante sp_ConfirmarReserva." : result.Message;
            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Propietario,Administrador")]
        public async Task<IActionResult> Completar(int id)
        {
            var result = await _reservationService.CompletarAsync(id, ObtenerUsuarioId());
            TempData[result.Success ? "Mensaje" : "Error"] =
                result.Success ? "Reserva marcada como Completada." : result.Message;
            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Propietario,Administrador")]
        public async Task<IActionResult> MarcarNoShow(int id)
        {
            var result = await _reservationService.MarcarNoShowAsync(id, ObtenerUsuarioId());
            TempData[result.Success ? "Mensaje" : "Error"] =
                result.Success ? "Reserva marcada como No Show." : result.Message;
            return RedirectToAction(nameof(Index));
        }
        private int ObtenerUsuarioId() =>
            int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
    }
}

