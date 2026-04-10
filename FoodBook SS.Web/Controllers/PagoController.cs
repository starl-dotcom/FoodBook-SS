using FoodBook_SS.Application.Dtos.Payment;
using FoodBook_SS.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
namespace FoodBook_SS.Web.Controllers
{
    [Authorize]
    public class PagoController : Controller
    {
        private readonly IPaymentService _paymentService;
        private readonly IOrderService   _orderService;
        private readonly IReservationService _reservationService;
        public PagoController(IPaymentService paymentService, IOrderService orderService, IReservationService reservationService)
        {
            _paymentService = paymentService;
            _orderService   = orderService;
            _reservationService = reservationService;
        }
        [HttpGet]
        public async Task<IActionResult> Pagar(int ordenId)
        {
            var result = await _orderService.GetByIdAsync(ordenId);
            if (!result.Success || result.Data == null)
            {
                TempData["Error"] = "Orden no encontrada.";
                return RedirectToAction("Index", "Orden");
            }
            var orden = (FoodBook_SS.Domain.Entities.Order.Orden)result.Data;
            if (orden.Estado != "Pendiente")
            {
                TempData["Error"] = "Solo se pueden pagar órdenes en estado Pendiente.";
                return RedirectToAction("Index", "Orden");
            }
            var reservaRes = await _reservationService.GetByIdAsync(orden.ReservaId);
            if (reservaRes.Success && reservaRes.Data is FoodBook_SS.Application.Dtos.Reservation.ReservationDto reservaDto)
            {
                if (reservaDto.Estado == "Pendiente")
                {
                    TempData["Error"] = "No puedes realizar el pago hasta que el propietario acepte (confirme) tu reserva.";
                    return RedirectToAction("Index", "Orden");
                }
            }
            var viewModel = new FoodBook_SS.Web.Models.PagoViewModel
            {
                PagoDto = new SavePaymentDto
                {
                    OrdenId = ordenId,
                    Monto   = orden.Total
                },
                Orden = orden
            };
            return View(viewModel);
        }
        [HttpPost]
        public async Task<IActionResult> Pagar(FoodBook_SS.Web.Models.PagoViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                var ordenRes = await _orderService.GetByIdAsync(viewModel.PagoDto.OrdenId);
                viewModel.Orden = ordenRes.Data as FoodBook_SS.Domain.Entities.Order.Orden;
                return View(viewModel);
            }
            var ordenToPayResult = await _orderService.GetByIdAsync(viewModel.PagoDto.OrdenId);
            if (ordenToPayResult.Success && ordenToPayResult.Data is FoodBook_SS.Domain.Entities.Order.Orden ordenToPay)
            {
                var reservaRes = await _reservationService.GetByIdAsync(ordenToPay.ReservaId);
                if (reservaRes.Success && reservaRes.Data is FoodBook_SS.Application.Dtos.Reservation.ReservationDto reservaDto)
                {
                    if (reservaDto.Estado == "Pendiente")
                    {
                        ModelState.AddModelError(string.Empty, "No puedes realizar el pago hasta que el propietario acepte (confirme) tu reserva.");
                        viewModel.Orden = ordenToPay;
                        return View(viewModel);
                    }
                }
            }
            var result = await _paymentService.ProcesarPagoAsync(viewModel.PagoDto, ObtenerUsuarioId());
            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.Message);
                var ordenResult = await _orderService.GetByIdAsync(viewModel.PagoDto.OrdenId);
                viewModel.Orden = ordenResult.Data as FoodBook_SS.Domain.Entities.Order.Orden;
                return View(viewModel);
            }
            TempData["Mensaje"] = $"Pago procesado exitosamente mediante sp_RegistrarPago. Método: {viewModel.PagoDto.MetodoPago}.";
            return RedirectToAction("Index", "Orden");
        }
        private int ObtenerUsuarioId() =>
            int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
    }
}

