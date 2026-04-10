using FoodBook_SS.Application.Dtos.Menu;
using FoodBook_SS.Application.Dtos.Order;
using FoodBook_SS.Application.Dtos.Restaurant;
using FoodBook_SS.Application.Interfaces;
using FoodBook_SS.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
namespace FoodBook_SS.Web.Controllers
{
    [Authorize(Roles = "Propietario,Administrador")]
    public class DashboardController : Controller
    {
        private readonly IRestaurantService  _restaurantService;
        private readonly IOrderService       _orderService;
        private readonly IMenuService        _menuService;
        private readonly IReservationService _reservationService;
        private readonly IReviewService      _reviewService;
        public DashboardController(IRestaurantService restaurantService,
                                   IOrderService       orderService,
                                   IMenuService        menuService,
                                   IReservationService reservationService,
                                   IReviewService      reviewService)
        {
            _restaurantService  = restaurantService;
            _orderService       = orderService;
            _menuService        = menuService;
            _reservationService = reservationService;
            _reviewService      = reviewService;
        }
        public async Task<IActionResult> Index(int? restauranteId, int diasPasados = 30)
        {
            var viewModel = new DashboardViewModel { DiasPasados = diasPasados };
            var restaurantesResult = User.IsInRole("Administrador")
                ? await _restaurantService.GetAllAsync()
                : await _restaurantService.GetByPropietarioAsync(ObtenerUsuarioId());
            if (restaurantesResult.Success && restaurantesResult.Data != null)
            {
                var entidades = (IEnumerable<FoodBook_SS.Domain.Entities.Configuration.Restaurante>)restaurantesResult.Data;
                viewModel.Restaurantes = entidades.Select(r => new RestaurantDto
                {
                    Id             = r.Id,
                    Nombre         = r.Nombre,
                    TipoCocina     = r.TipoCocina,
                    Direccion      = r.Direccion,
                    Ciudad         = r.Ciudad,
                    RangoPrecio    = r.RangoPrecio,
                    CalificacionProm = r.CalificacionProm,
                    TotalResenas   = r.TotalResenas
                }).ToList();
            }
            if (!restauranteId.HasValue && viewModel.Restaurantes.Any())
                restauranteId = viewModel.Restaurantes.First().Id;
            viewModel.RestauranteIdSeleccionado = restauranteId;
            if (restauranteId.HasValue)
            {
                var hasta = DateOnly.FromDateTime(DateTime.UtcNow);
                var desde = hasta.AddDays(-diasPasados);
                var ventasResult = await _orderService.GetVentasAsync(restauranteId.Value, desde, hasta);
                if (ventasResult.Success && ventasResult.Data != null)
                    viewModel.VentasPorFecha = (IEnumerable<VentaFechaDto>)ventasResult.Data;
                var productosResult = await _orderService.GetProductosMasOrdenadosAsync(restauranteId.Value, desde, hasta);
                if (productosResult.Success && productosResult.Data != null)
                    viewModel.ProductosMasOrdenados = (IEnumerable<ProductoMasOrdenadoDto>)productosResult.Data;
                var rest = viewModel.Restaurantes.FirstOrDefault(r => r.Id == restauranteId.Value);
                viewModel.CalificacionPromedio = rest?.CalificacionProm ?? 0;
                var reservasResult = await _reservationService.GetByRestauranteAsync(restauranteId.Value, null);
                if (reservasResult.Success && reservasResult.Data != null)
                {
                    var reservas = (IEnumerable<FoodBook_SS.Domain.Entities.Reservation.Reserva>)reservasResult.Data;
                    viewModel.TotalReservasHoy = reservas.Count(r => r.FechaReserva == hasta);
                }
            }
            return View(viewModel);
        }
        [HttpGet]
        public async Task<IActionResult> Menu(int? restauranteId)
        {
            var restaurantesList = await ObtenerRestaurantesAsync();
            ViewBag.Restaurantes = restaurantesList;
            var id = restauranteId ?? restaurantesList.FirstOrDefault()?.Id ?? 0;
            ViewBag.RestauranteId = id;
            if (id == 0) return View(new DashboardMenuViewModel());
            var cats = await _menuService.GetCategoriasByRestauranteAsync(id);
            var prods = await _menuService.GetProductosByRestauranteAsync(id);
            return View(new DashboardMenuViewModel
            {
                Categorias = cats.Success
                    ? (cats.Data as IEnumerable<FoodBook_SS.Domain.Entities.Configuration.CategoriaMenu>)
                      ?? Enumerable.Empty<FoodBook_SS.Domain.Entities.Configuration.CategoriaMenu>()
                    : Enumerable.Empty<FoodBook_SS.Domain.Entities.Configuration.CategoriaMenu>(),
                Productos = prods.Success
                    ? (prods.Data as IEnumerable<FoodBook_SS.Domain.Entities.Configuration.ProductoMenu>)
                      ?? Enumerable.Empty<FoodBook_SS.Domain.Entities.Configuration.ProductoMenu>()
                    : Enumerable.Empty<FoodBook_SS.Domain.Entities.Configuration.ProductoMenu>()
            });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GuardarCategoria(SaveCategoryDto dto)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Datos de categoría inválidos.";
                return RedirectToAction(nameof(Menu), new { restauranteId = dto.RestauranteId });
            }
            var result = await _menuService.SaveCategoriaAsync(dto);
            TempData[result.Success ? "Mensaje" : "Error"] =
                result.Success ? "Categoría guardada exitosamente." : result.Message;
            return RedirectToAction(nameof(Menu), new { restauranteId = dto.RestauranteId });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GuardarProducto(SaveProductDto dto)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Datos de producto inválidos.";
                return RedirectToAction(nameof(Menu), new { restauranteId = dto.RestauranteId });
            }
            var result = await _menuService.SaveProductoAsync(dto);
            TempData[result.Success ? "Mensaje" : "Error"] =
                result.Success ? "Producto guardado exitosamente." : result.Message;
            return RedirectToAction(nameof(Menu), new { restauranteId = dto.RestauranteId });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleProducto(int productoId, int restauranteId)
        {
            await _menuService.ToggleDisponibilidadAsync(productoId, ObtenerUsuarioId());
            return RedirectToAction(nameof(Menu), new { restauranteId });
        }
        [HttpGet]
        public async Task<IActionResult> Mesas(int? restauranteId)
        {
            var restaurantesList = await ObtenerRestaurantesAsync();
            ViewBag.Restaurantes = restaurantesList;
            var id = restauranteId ?? restaurantesList.FirstOrDefault()?.Id ?? 0;
            ViewBag.RestauranteId = id;
            if (id == 0) return View(Enumerable.Empty<FoodBook_SS.Domain.Entities.Configuration.Mesa>());
            var result = await _restaurantService.GetByIdAsync(id);
            if (!result.Success || result.Data == null)
                return View(Enumerable.Empty<FoodBook_SS.Domain.Entities.Configuration.Mesa>());
            var restaurante = (FoodBook_SS.Domain.Entities.Configuration.Restaurante)result.Data;
            var mesas = restaurante.Mesas ?? Enumerable.Empty<FoodBook_SS.Domain.Entities.Configuration.Mesa>();
            return View(mesas);
        }
        [HttpGet]
        public async Task<IActionResult> ReservasDelDia(int? restauranteId, string? estado)
        {
            var restaurantesList = await ObtenerRestaurantesAsync();
            ViewBag.Restaurantes = restaurantesList;
            var id = restauranteId ?? restaurantesList.FirstOrDefault()?.Id ?? 0;
            ViewBag.RestauranteId = id;
            ViewBag.EstadoFiltro  = estado;
            if (id == 0) return View(Enumerable.Empty<FoodBook_SS.Domain.Entities.Reservation.Reserva>());
            var result = await _reservationService.GetAllByRestauranteAsync(id, estado);
            if (!result.Success || result.Data == null)
                return View(Enumerable.Empty<FoodBook_SS.Domain.Entities.Reservation.Reserva>());
            var hoy = DateOnly.FromDateTime(DateTime.Now);
            var reservas = (result.Data as IEnumerable<FoodBook_SS.Domain.Entities.Reservation.Reserva>)
                           ?? Enumerable.Empty<FoodBook_SS.Domain.Entities.Reservation.Reserva>();
            if (string.IsNullOrEmpty(estado))
                reservas = reservas.Where(r => r.FechaReserva == hoy ||
                                               ((r.Estado == "Pendiente" || r.Estado == "Confirmada") && r.FechaReserva >= hoy));
            return View(reservas.OrderBy(r => r.FechaReserva).ThenBy(r => r.HoraReserva));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmarReserva(int reservaId, int restauranteId)
        {
            if (!await IsRestauranteActivoAsync(restauranteId))
            {
                TempData["Error"] = "El restaurante está suspendido. No puedes gestionar reservaciones.";
                return RedirectToAction(nameof(ReservasDelDia), new { restauranteId });
            }
            var result = await _reservationService.ConfirmarAsync(reservaId, ObtenerUsuarioId());
            TempData[result.Success ? "Mensaje" : "Error"] =
                result.Success ? "Reserva confirmada (sp_ConfirmarReserva)." : result.Message;
            return RedirectToAction(nameof(ReservasDelDia), new { restauranteId });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CompletarReserva(int reservaId, int restauranteId)
        {
            if (!await IsRestauranteActivoAsync(restauranteId))
            {
                TempData["Error"] = "El restaurante está suspendido. No puedes completar reservaciones u órdenes.";
                return RedirectToAction(nameof(ReservasDelDia), new { restauranteId });
            }
            var result = await _reservationService.CompletarAsync(reservaId, ObtenerUsuarioId());
            if (result.Success)
            {
                var ordenRes = await _orderService.GetByReservaAsync(reservaId);
                if (ordenRes.Success && ordenRes.Data is IEnumerable<FoodBook_SS.Domain.Entities.Order.Orden> ordenes)
                {
                    foreach (var orden in ordenes.Where(o => o.Estado != "Cancelada"))
                    {
                        await _orderService.CambiarEstadoAsync(orden.Id, FoodBook_SS.Domain.Entities.Reservation.EstadoOrden.Entregada, ObtenerUsuarioId());
                    }
                }
            }
            TempData[result.Success ? "Mensaje" : "Error"] =
                result.Success ? "Reserva completada y órdenes asociadas entregadas." : result.Message;
            return RedirectToAction(nameof(ReservasDelDia), new { restauranteId });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelarReserva(int reservaId, int restauranteId, string? motivo)
        {
            if (!await IsRestauranteActivoAsync(restauranteId))
            {
                TempData["Error"] = "El restaurante está suspendido. Acción denegada.";
                return RedirectToAction(nameof(ReservasDelDia), new { restauranteId });
            }
            var result = await _reservationService.CancelarAsync(reservaId, ObtenerUsuarioId(), motivo);
            TempData[result.Success ? "Mensaje" : "Error"] =
                result.Success ? "Reserva cancelada exitosamente." : result.Message;
            return RedirectToAction(nameof(ReservasDelDia), new { restauranteId });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> NoShowReserva(int reservaId, int restauranteId)
        {
            if (!await IsRestauranteActivoAsync(restauranteId))
            {
                TempData["Error"] = "El restaurante está suspendido. Acción denegada.";
                return RedirectToAction(nameof(ReservasDelDia), new { restauranteId });
            }
            var result = await _reservationService.MarcarNoShowAsync(reservaId, ObtenerUsuarioId());
            TempData[result.Success ? "Mensaje" : "Error"] =
                result.Success ? "Reserva marcada como No Show." : result.Message;
            return RedirectToAction(nameof(ReservasDelDia), new { restauranteId });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ToggleMesa(int mesaId, int restauranteId)
        {
            TempData["Mensaje"] = "Disponibilidad de la mesa actualizada.";
            return RedirectToAction(nameof(Mesas), new { restauranteId });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AgregarMesa(FoodBook_SS.Application.Dtos.Restaurant.SaveMesaDto dto)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Datos de mesa inválidos. Verifica el número y la capacidad.";
                return RedirectToAction(nameof(Mesas), new { restauranteId = dto.RestauranteId });
            }
            var result = await _restaurantService.AgregarMesaAsync(dto);
            TempData[result.Success ? "Mensaje" : "Error"] =
                result.Success ? "Mesa agregada exitosamente." : result.Message;
            return RedirectToAction(nameof(Mesas), new { restauranteId = dto.RestauranteId });
        }
        [HttpGet]
        public async Task<IActionResult> Resenas(int? restauranteId)
        {
            var restaurantesList = await ObtenerRestaurantesAsync();
            ViewBag.Restaurantes = restaurantesList;
            var id = restauranteId ?? restaurantesList.FirstOrDefault()?.Id ?? 0;
            ViewBag.RestauranteId = id;
            if (id == 0) return View(Enumerable.Empty<FoodBook_SS.Domain.Entities.Review.Resena>());
            var result = await _reviewService.GetByRestauranteAsync(id);
            var resenas = (result.Data as IEnumerable<FoodBook_SS.Domain.Entities.Review.Resena>) 
                          ?? Enumerable.Empty<FoodBook_SS.Domain.Entities.Review.Resena>();
            return View(resenas);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResponderResena(int resenaId, int restauranteId, string respuesta)
        {
            var result = await _reviewService.ResponderAsync(resenaId, respuesta, ObtenerUsuarioId());
            TempData[result.Success ? "Mensaje" : "Error"] =
                result.Success ? "Respuesta enviada correctamente." : result.Message;
            return RedirectToAction(nameof(Resenas), new { restauranteId });
        }
        private async Task<List<RestaurantDto>> ObtenerRestaurantesAsync()
        {
            var r = User.IsInRole("Administrador")
                ? await _restaurantService.GetAllAsync()
                : await _restaurantService.GetByPropietarioAsync(ObtenerUsuarioId());
            if (!r.Success || r.Data == null) return new();
            var entidades = (IEnumerable<FoodBook_SS.Domain.Entities.Configuration.Restaurante>)r.Data;
            return entidades.Select(x => new RestaurantDto
            {
                Id     = x.Id,
                Nombre = x.Nombre
            }).ToList();
        }
        private int ObtenerUsuarioId() =>
            int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int id) ? id : 0;
        private async Task<bool> IsRestauranteActivoAsync(int restauranteId)
        {
            var res = await _restaurantService.GetByIdAsync(restauranteId);
            if (res.Success && res.Data is FoodBook_SS.Domain.Entities.Configuration.Restaurante r)
            {
                return r.Activo;
            }
            return false;
        }
    }
}

