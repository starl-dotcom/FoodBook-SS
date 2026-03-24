using FoodBook_SS.Application.Dtos.Restaurant;
using FoodBook_SS.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodBook_SS.API.Controllers
{
    public class RestaurantController : BaseApiController
    {
        private readonly IRestaurantService _service;
        public RestaurantController(IRestaurantService service) => _service = service;

        [HttpGet]
        public async Task<IActionResult> Search([FromQuery] string? nombre,
            [FromQuery] string? ciudad, [FromQuery] string? tipoCocina)
            => Respond(await _service.SearchAsync(nombre, ciudad, tipoCocina));

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
            => Respond(await _service.GetByIdAsync(id));

        [Authorize(Roles = "Propietario,Administrador")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] SaveRestaurantDto dto)
        {
            var result = await _service.CreateAsync(dto, ObtenerUsuarioId());
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [Authorize(Roles = "Propietario,Administrador")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateRestaurantDto dto)
            => Respond(await _service.UpdateAsync(id, dto));
    }
}