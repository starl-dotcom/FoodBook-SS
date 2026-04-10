using FoodBook_SS.Desktop.Core;
using FoodBook_SS.Desktop.Services;
using FoodBook_SS.Desktop.Views;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
namespace FoodBook_SS.Desktop
{
    public partial class App : System.Windows.Application
    {
        public static IServiceProvider Services { get; private set; } = null!;
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            var config = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false)
                .Build();
            var baseUrl = config["ApiBaseUrl"] ?? "https://localhost:7062";
            var services = new ServiceCollection();
            services.AddHttpClient("FoodBookAPI", c =>
            {
                c.BaseAddress = new Uri(baseUrl.TrimEnd('/') + "/");
                c.Timeout     = TimeSpan.FromSeconds(30);
            });
            services.AddSingleton<ApiClient>();
            services.AddSingleton<NotificationPoller>();
            services.AddTransient<IAuthService,        AuthService>();
            services.AddTransient<IReservaService,     ReservaService>();
            services.AddTransient<IOrdenService,       OrdenService>();
            services.AddTransient<IMenuService,        MenuService>();
            services.AddTransient<IPagoService,        PagoService>();
            services.AddTransient<IRestauranteService, RestauranteService>();
            services.AddTransient<IUsuarioService,     UsuarioService>();
            services.AddTransient<IResenaService,      ResenaService>();
            services.AddTransient<IAuditoriaService,   AuditoriaService>();
            services.AddTransient<ViewModels.LoginViewModel>();
            services.AddTransient<ViewModels.RegisterViewModel>();
            services.AddTransient<ViewModels.Cliente.MisReservasViewModel>();
            services.AddTransient<ViewModels.Cliente.NuevaReservaViewModel>();
            services.AddTransient<ViewModels.Cliente.MisOrdenesViewModel>();
            services.AddTransient<ViewModels.Cliente.PagarViewModel>();
            services.AddTransient<ViewModels.Cliente.ResenaViewModel>();
            services.AddTransient<ViewModels.Propietario.ReservasDelDiaViewModel>();
            services.AddTransient<ViewModels.Propietario.MenuViewModel>();
            services.AddTransient<ViewModels.Propietario.OrdenesActivasViewModel>();
            services.AddTransient<ViewModels.Propietario.MetricasViewModel>();
            services.AddTransient<ViewModels.Admin.AdminDashboardViewModel>();
            services.AddTransient<ViewModels.Admin.AdminUsuariosViewModel>();
            services.AddTransient<ViewModels.Admin.AdminRestaurantesViewModel>();
            services.AddTransient<ViewModels.Admin.AdminModeracionViewModel>();
            services.AddTransient<ViewModels.Admin.AdminAuditoriaViewModel>();
            services.AddTransient<ViewModels.Cliente.NuevaOrdenViewModel>();
            services.AddTransient<ViewModels.Propietario.MesasViewModel>();
            services.AddTransient<ViewModels.Propietario.ResenasViewModel>();
            services.AddTransient<LoginWindow>();
            services.AddTransient<Views.Cliente.ClienteShell>();
            services.AddTransient<Views.Propietario.PropietarioShell>();
            services.AddTransient<Views.Admin.AdminShell>();
            Services = services.BuildServiceProvider();
            var startWindow = SessionManager.IsLoggedIn()
                ? AbrirShellPorRol(SessionManager.GetRol())
                : Services.GetRequiredService<LoginWindow>();
            startWindow.Show();
        }
        public static Window AbrirShellPorRol(string? rol) => (rol?.Trim().ToLower()) switch
        {
            "propietario"  => Services.GetRequiredService<Views.Propietario.PropietarioShell>(),
            "administrador"=> Services.GetRequiredService<Views.Admin.AdminShell>(),
            _              => Services.GetRequiredService<Views.Cliente.ClienteShell>()
        };
    }
}

