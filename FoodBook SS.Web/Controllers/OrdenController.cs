using FoodBook_SS.Application.Dtos.Order;
using FoodBook_SS.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
namespace FoodBook_SS.Web.Controllers
{
    [Authorize]
    public class OrdenController : Controller
    {
        private readonly IOrderService   _orderService;
        private readonly IMenuService    _menuService;
        private readonly IPaymentService _paymentService;
        public OrdenController(IOrderService orderService, IMenuService menuService,
                               IPaymentService paymentService)
        {
            _orderService   = orderService;
            _menuService    = menuService;
            _paymentService = paymentService;
        }
        public async Task<IActionResult> Index()
        {
            if (User.IsInRole("Propietario") || User.IsInRole("Administrador"))
                return RedirectToAction("Index", "Dashboard");
            var result = await _orderService.GetByClienteAsync(ObtenerUsuarioId());
            return View(result.Data);
        }
        [HttpGet]
        public async Task<IActionResult> Crear(int restauranteId, int reservaId = 0)
        {
            if (User.IsInRole("Propietario") || User.IsInRole("Administrador"))
                return RedirectToAction("Index", "Dashboard");
            var menu = await _menuService.GetProductosByRestauranteAsync(restauranteId, true);
            ViewBag.RestauranteId = restauranteId;
            ViewBag.ReservaId     = reservaId;
            return View(menu.Data);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(SaveOrderDto dto)
        {
            dto.Items = dto.Items.Where(i => i.Cantidad > 0).ToList();
            if (!dto.Items.Any())
            {
                TempData["Error"] = "Debes seleccionar al menos un producto.";
                return RedirectToAction(nameof(Crear), new { restauranteId = dto.RestauranteId, reservaId = dto.ReservaId });
            }
            var result = await _orderService.CreateAsync(dto, ObtenerUsuarioId());
            if (!result.Success)
            {
                TempData["Error"] = result.Message;
                return RedirectToAction(nameof(Crear), new { restauranteId = dto.RestauranteId });
            }
            TempData["Mensaje"] = "¡Orden creada exitosamente!";
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> Detalle(int id)
        {
            var result = await _orderService.GetByIdAsync(id);
            if (!result.Success || result.Data == null)
            {
                TempData["Error"] = "Orden no encontrada.";
                return RedirectToAction(nameof(Index));
            }
            return View(result.Data);
        }
        private int ObtenerUsuarioId() =>
            int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
    }
}

