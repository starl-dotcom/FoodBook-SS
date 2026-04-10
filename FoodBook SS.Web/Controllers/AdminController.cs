using FoodBook_SS.Application.Interfaces;
using FoodBook_SS.Application.Dtos.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using FoodBook_SS.Domain.Entities.User;
using FoodBook_SS.Domain.Entities.Configuration;
namespace FoodBook_SS.Web.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class AdminController : Controller
    {
        private readonly IUserService _userService;
        private readonly IRestaurantService _restaurantService;
        private readonly IAuditService _auditService;
        private readonly IReviewService _reviewService;
        private readonly IOrderService _orderService;
        private readonly IReservationService _reservationService;
        public AdminController(IUserService userService, 
                               IRestaurantService restaurantService, 
                               IAuditService auditService,
                               IReviewService reviewService,
                               IOrderService orderService,
                               IReservationService reservationService)
        {
            _userService = userService;
            _restaurantService = restaurantService;
            _auditService = auditService;
            _reviewService = reviewService;
            _orderService = orderService;
            _reservationService = reservationService;
        }
        public async Task<IActionResult> Index()
        {
            var ordersRes = await _orderService.GetAllAsync();
            decimal totalVentas = 0;
            if (ordersRes.Success && ordersRes.Data is IEnumerable<FoodBook_SS.Domain.Entities.Order.Orden> orders)
                totalVentas = orders.Where(o => o.Estado == "G" || o.Estado == "E" || o.Estado == "F" || o.Estado == "Completada" || o.Estado == "Entregada").Sum(o => o.Total);
            var reservRes = await _reservationService.GetAllAsync();
            int reservasActivas = 0;
            if (reservRes.Success && reservRes.Data is IEnumerable<FoodBook_SS.Domain.Entities.Reservation.Reserva> res)
                reservasActivas = res.Count(r => r.Estado == "Pendiente" || r.Estado == "Confirmada");
            var reviewRes = await _reviewService.GetAllAsync();
            int resenasPendientes = 0;
            if (reviewRes.Success && reviewRes.Data is IEnumerable<FoodBook_SS.Domain.Entities.Review.Resena> rev)
                resenasPendientes = rev.Count(r => string.IsNullOrEmpty(r.Respuesta));
            var usersRes = await _userService.GetAllAsync();
            int usersActivos = 0;
            if (usersRes.Success && usersRes.Data is IEnumerable<FoodBook_SS.Application.Dtos.User.UserDto> u)
                usersActivos = u.Count(x => x.Activo);
            ViewBag.TotalVentas = totalVentas;
            ViewBag.ReservasActivas = reservasActivas;
            ViewBag.ResenasPendientes = resenasPendientes;
            ViewBag.UsuariosActivos = usersActivos;
            return View();
        }
        public async Task<IActionResult> Usuarios()
        {
            var res = await _userService.GetAllAsync();
            var usuarios = (res.Data as IEnumerable<UserDto>) ?? Enumerable.Empty<UserDto>();
            return View(usuarios);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleUsuario(int usuarioId, bool activo)
        {
            int actorId = ObtenerUsuarioId();
            if (usuarioId == actorId)
            {
                TempData["Error"] = "No puedes bloquear tu propia cuenta.";
                return RedirectToAction(nameof(Usuarios));
            }
            var targetResult = await _userService.GetByIdAsync(usuarioId);
            if (targetResult.Success && targetResult.Data is FoodBook_SS.Application.Dtos.User.UserDto targetUser
                && (targetUser.RolId == 3 || string.Equals(targetUser.Rol, "Administrador", StringComparison.OrdinalIgnoreCase)))
            {
                TempData["Error"] = "No puedes bloquear la cuenta de otro administrador.";
                return RedirectToAction(nameof(Usuarios));
            }
            await _userService.ActivarDesactivarAsync(usuarioId, activo, actorId);
            TempData["Mensaje"] = $"Usuario {(activo ? "activado" : "bloqueado")} correctamente.";
            return RedirectToAction(nameof(Usuarios));
        }
        public async Task<IActionResult> Restaurantes()
        {
            var res = await _restaurantService.GetAllAsync();
            var restaurantes = (res.Data as IEnumerable<Restaurante>) ?? Enumerable.Empty<Restaurante>();
            return View(restaurantes);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleRestaurante(int restauranteId, bool activo)
        {
            var result = await _restaurantService.ToggleEstadoAsync(restauranteId, activo, ObtenerUsuarioId());
            if (result.Success)
            {
                await _auditService.RegistrarAsync(ObtenerUsuarioId(), "TOGGLE_RESTAURANTE", "Restaurante", restauranteId.ToString(), "Estado actualizado.");
                TempData["Mensaje"] = $"Estado del restaurante actualizado.";
            }
            else
            {
                TempData["Error"] = result.Message;
            }
            return RedirectToAction(nameof(Restaurantes));
        }
        public async Task<IActionResult> Auditoria()
        {
            var res = await _auditService.GetAllLogsAsync();
            return View(res.Data);
        }
        public async Task<IActionResult> Moderacion()
        {
            var res = await _reviewService.GetAllAsync();
            return View(res.Data);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ModerarResena(int resenaId, bool visible)
        {
            await _reviewService.ModerarAsync(resenaId, visible, ObtenerUsuarioId());
            return RedirectToAction(nameof(Moderacion));
        }
        private int ObtenerUsuarioId() =>
            int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int id) ? id : 0;
    }
}

