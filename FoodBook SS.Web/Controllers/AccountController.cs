using FoodBook_SS.Application.Dtos.User;
using FoodBook_SS.Application.Interfaces;
using FoodBook_SS.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
namespace FoodBook_SS.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserService _userService;
        private readonly NotificationInbox _inbox;
        public AccountController(IUserService userService, NotificationInbox inbox)
        {
            _userService = userService;
            _inbox = inbox;
        }
        [HttpGet]
        public IActionResult Login() => View();
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginDto dto, string? returnUrl)
        {
            var result = await _userService.LoginAsync(dto);
            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.Message);
                return View(dto);
            }
            var auth = (AuthResponseDto)result.Data!;
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, auth.Usuario.Id.ToString()),
                new(ClaimTypes.Email,          auth.Usuario.Email),
                new(ClaimTypes.Name,           auth.Usuario.NombreCompleto),
                new(ClaimTypes.Role,           auth.Usuario.Rol)
            };
            var identity = new ClaimsIdentity(claims, "Cookies");
            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync("Cookies", principal);
            return Redirect(returnUrl ?? "/Home/Index");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("Cookies");
            return RedirectToAction("Login");
        }
        [HttpGet]
        public IActionResult Register() => View();
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(SaveUserDto dto)
        {
            if (!ModelState.IsValid) return View(dto);
            var result = await _userService.SaveAsync(dto);
            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.Message);
                return View(dto);
            }
            TempData["Mensaje"] = "Registro exitoso. Inicia sesión.";
            return RedirectToAction("Login");
        }
        [HttpGet]
        [Authorize]
        public IActionResult GetNotificaciones()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(email))
                return Json(new List<object>());
            var msgs = _inbox.Pop(email);
            var resultado = msgs.Select(m => new
            {
                titulo  = m.Titulo,
                cuerpo  = m.Cuerpo,
                hora    = m.FechaHora.ToString("HH:mm")
            });
            return Json(resultado);
        }
    }
}
