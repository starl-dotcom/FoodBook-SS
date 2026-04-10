using FoodBook_SS.Application.Dtos.Review;
using FoodBook_SS.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FoodBook_SS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReviewController : ControllerBase
    {
        private readonly IReviewService _reviewService;

        public ReviewController(IReviewService reviewService) => _reviewService = reviewService;

        [HttpGet("restaurante/{restauranteId:int}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetByRestaurante(int restauranteId) =>
            Ok(await _reviewService.GetByRestauranteAsync(restauranteId));

        [HttpGet]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> GetAll() =>
            Ok(await _reviewService.GetAllAsync());

        [HttpGet("cliente")]
        [Authorize]
        public async Task<IActionResult> GetMisResenas() =>
            Ok(await _reviewService.GetByClienteAsync(ObtenerUsuarioId()));

        [HttpPost]
        [Authorize(Roles = "Cliente")]
        public async Task<IActionResult> Create([FromBody] SaveReviewDto dto)
        {
            var result = await _reviewService.CreateAsync(dto, ObtenerUsuarioId());
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPatch("{id:int}/moderar")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Moderar(int id, [FromQuery] bool visible)
        {
            var result = await _reviewService.ModerarAsync(id, visible, ObtenerUsuarioId());
            return result.Success ? Ok(result) : BadRequest(result);
        }

        private int ObtenerUsuarioId() =>
            int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
    }
}
