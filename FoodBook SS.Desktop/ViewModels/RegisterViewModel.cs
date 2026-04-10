using FoodBook_SS.Application.Dtos.User;
using FoodBook_SS.Desktop.Core;
using FoodBook_SS.Desktop.Services;
namespace FoodBook_SS.Desktop.ViewModels
{
    public class RegisterViewModel : ObservableBase
    {
        private readonly IAuthService _auth;
        public RegisterViewModel(IAuthService auth) => _auth = auth;
        private string _nombre = string.Empty;
        public string Nombre { get => _nombre; set => SetField(ref _nombre, value); }
        private string _email = string.Empty;
        public string Email { get => _email; set => SetField(ref _email, value); }
        public string Password { get; set; } = string.Empty;
        public string Confirmar { get; set; } = string.Empty;
        private string _rol = "Cliente";
        public string Rol { get => _rol; set => SetField(ref _rol, value); }
        public string[] Roles { get; } = ["Cliente", "Propietario"];
        public RelayCommand RegisterCommand => new(async () => await ExecuteAsync(),
            () => !IsBusy && !string.IsNullOrWhiteSpace(Nombre) && !string.IsNullOrWhiteSpace(Email));
        private async Task ExecuteAsync()
        {
            ClearMessages();
            if (Password != Confirmar) { SetError("Las contraseñas no coinciden."); return; }
            if (Password.Length < 6)   { SetError("La contraseña debe tener al menos 6 caracteres."); return; }
            IsBusy = true;
            var result = await _auth.RegisterAsync(new SaveUserDto
            {
                Nombre         = Nombre,
                Apellido       = "", 
                Email          = Email,
                Password       = Password,
                RolId          = Rol == "Propietario" ? 2 : 1
            });
            IsBusy = false;
            if (!result.Success) { SetError(result.Message); return; }
            SetSuccess("✅ Cuenta creada exitosamente. Ahora inicia sesión.");
        }
    }
}

