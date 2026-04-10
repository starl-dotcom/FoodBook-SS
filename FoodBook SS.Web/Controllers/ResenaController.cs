using FoodBook_SS.Application.Dtos.Review;
using FoodBook_SS.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FoodBook_SS.Web.Controllers
{
    [Authorize]
    public class ResenaController : Controller
    {
        private readonly IReviewService _reviewService;
        private readonly IOrderService  _orderService;

        public ResenaController(IReviewService reviewService, IOrderService orderService)
        {
            _reviewService = reviewService;
            _orderService  = orderService;
        }

        [HttpGet]
        public async Task<IActionResult> Crear(int ordenId, int restauranteId)
        {
            var ordenResult = await _orderService.GetByIdAsync(ordenId);
            if (!ordenResult.Success || ordenResult.Data == null)
            {
                TempData["Error"] = "Orden no encontrada.";
                return RedirectToAction("Index", "Orden");
            }

            var orden = (FoodBook_SS.Domain.Entities.Order.Orden)ordenResult.Data;

            var estadosValidos = new[] { "Completada", "Entregada" };
            if (!estadosValidos.Contains(orden.Estado, StringComparer.OrdinalIgnoreCase))
            {
                TempData["Error"] = $"Solo puedes calificar órdenes finalizadas (Entregada/Completada). Estado actual: {orden.Estado}";
                return RedirectToAction("Index", "Orden");
            }

            var dto = new SaveReviewDto
            {
                OrdenId       = ordenId,
                RestauranteId = restauranteId
            };

            ViewBag.Orden       = orden;
            ViewBag.Restaurante = orden.Restaurante?.Nombre ?? "—";
            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(SaveReviewDto dto)
        {
            if (!ModelState.IsValid)
            {
                var ordenResult = await _orderService.GetByIdAsync(dto.OrdenId);
                ViewBag.Orden       = ordenResult.Data;
                ViewBag.Restaurante = (ordenResult.Data as FoodBook_SS.Domain.Entities.Order.Orden)?.Restaurante?.Nombre ?? "—";
                return View(dto);
            }

            var result = await _reviewService.CreateAsync(dto, ObtenerUsuarioId());
            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.Message);
                return View(dto);
            }

            TempData["Mensaje"] = "¡Gracias por tu reseña! Tu opinión ayuda a mejorar el servicio.";
            return RedirectToAction("Index", "Orden");
        }

        private int ObtenerUsuarioId() =>
            int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
    }
}
