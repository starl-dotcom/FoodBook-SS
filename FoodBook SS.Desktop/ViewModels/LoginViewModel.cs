using FoodBook_SS.Application.Dtos.User;
using FoodBook_SS.Desktop.Core;
using FoodBook_SS.Desktop.Services;
using System.Windows;
namespace FoodBook_SS.Desktop.ViewModels
{
    public class LoginViewModel : ObservableBase
    {
        private readonly IAuthService _auth;
        public LoginViewModel(IAuthService auth) => _auth = auth;
        private string _email = string.Empty;
        public string Email
        {
            get => _email;
            set => SetField(ref _email, value);
        }
        public string Password { get; set; } = string.Empty;
        public RelayCommand LoginCommand   => new(async () => await ExecuteLoginAsync(),
                                                  ()    => !IsBusy && !string.IsNullOrWhiteSpace(Email));
        public RelayCommand RegisterCommand => new(() => AbrirRegistro());
        private async Task ExecuteLoginAsync()
        {
            ClearMessages();
            if (string.IsNullOrWhiteSpace(Password)) { SetError("Ingresa tu contraseña."); return; }
            IsBusy = true;
            var result = await _auth.LoginAsync(new LoginDto { Email = Email, Password = Password });
            IsBusy = false;
            if (!result.Success || result.Data is null)
            {
                SetError(result.Message);
                return;
            }
            var auth = result.Data;
            SessionManager.Save(auth.Token, auth.Usuario.NombreCompleto, auth.Usuario.Rol,
                                usuarioId: auth.Usuario.Id);
            if (auth.Usuario.Rol.Equals("Propietario", StringComparison.OrdinalIgnoreCase))
            {
                var rRest = await _auth.GetMiRestauranteIdAsync(auth.Usuario.Id);
                if (rRest.Success && rRest.Data.HasValue)
                    SessionManager.Save(auth.Token, auth.Usuario.NombreCompleto, auth.Usuario.Rol,
                                        usuarioId: auth.Usuario.Id, restauranteId: rRest.Data.Value);
            }
            var shell = App.AbrirShellPorRol(auth.Usuario.Rol);
            shell.Show();
            System.Windows.Application.Current.Windows.OfType<Views.LoginWindow>().FirstOrDefault()?.Close();
        }
        private static void AbrirRegistro()
        {
            var reg = App.Services.GetService(typeof(Views.RegisterWindow)) as Window;
            reg?.Show();
        }
    }
}

