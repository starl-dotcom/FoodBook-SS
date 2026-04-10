using FoodBook_SS.Domain.Base;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FoodBook_SS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public abstract class BaseApiController : ControllerBase
    {
        protected IActionResult Respond(OperationResult result)
            => result.Success ? Ok(result) : BadRequest(result);

        protected IActionResult RespondCreated(string action, OperationResult result)
            => result.Success
               ? CreatedAtAction(action, new { id = result.Data }, result)
               : BadRequest(new { result.Message });

        protected int ObtenerUsuarioId()
            => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
    }
}
