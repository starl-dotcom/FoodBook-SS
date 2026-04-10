using FoodBook_SS.Desktop.Core;
using FoodBook_SS.Desktop.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using System.Windows.Controls;
namespace FoodBook_SS.Desktop.Views.Cliente
{
    public partial class ClienteShell : Window
    {
        private readonly IReservaService  _reservas;
        private readonly IOrdenService    _ordenes;
        private readonly IPagoService     _pagos;
        private readonly IResenaService   _resenas;
        private readonly NotificationPoller _poller;
        private int _notifCount = 0;
        public ClienteShell(IReservaService reservas, IOrdenService ordenes,
                            IPagoService pagos, IResenaService resenas,
                            NotificationPoller poller)
        {
            InitializeComponent();
            _reservas = reservas; _ordenes = ordenes;
            _pagos    = pagos;    _resenas = resenas;
            _poller   = poller;
            TxtUsuario.Text = SessionManager.GetUsuario() ?? "Cliente";
            _poller.OnNotificacion += AgregarNotificacion;
            _poller.Start();
            NavTo(new ExplorarRestaurantesPage(App.Services.GetRequiredService<IRestauranteService>()));
            SetActiveNav(BtnExplorar);
        }
        private void Nav_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button btn) return;
            SetActiveNav(btn);
            var serviceProvider = App.Services;
            Page page = btn.Name switch
            {
                "BtnExplorar" => new ExplorarRestaurantesPage(serviceProvider.GetRequiredService<IRestauranteService>()),
                "BtnOrdenes"  => new MisOrdenesPage(_ordenes),
                "BtnPagar"    => new PagarPage(_pagos, _ordenes),
                "BtnResenas"  => new ResenaPage(_resenas, _ordenes, _reservas, serviceProvider.GetRequiredService<IRestauranteService>()),
                _             => new MisReservasPage(_reservas)
            };
            NavTo(page);
        }
        public void NavigateToNuevaReservaConRestaurante(FoodBook_SS.Application.Dtos.Restaurant.RestaurantDto restaurante)
        {
            SetActiveNav(BtnReservas); 
            var serviceProvider = App.Services;
            NavTo(new NuevaReservaPage(serviceProvider.GetRequiredService<IRestauranteService>(), _reservas, restaurante));
        }
        private void NavTo(Page page) => MainFrame.Navigate(page);
        private void SetActiveNav(Button? active)
        {
            foreach (var btn in new[] { BtnExplorar, BtnReservas, BtnOrdenes, BtnPagar, BtnResenas })
                btn.Style = (Style)FindResource(btn == active ? "NavItemActive" : "NavItem");
        }
        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            _poller.Stop();
            SessionManager.Clear();
            var login = App.Services.GetService(typeof(LoginWindow)) as Window;
            login?.Show();
            Close();
        }
        private void AgregarNotificacion(string titulo, string cuerpo)
        {
            _notifCount++;
            BadgeNotif.Visibility = Visibility.Visible;
            TxtBadge.Text = _notifCount.ToString();
            NotifList.Items.Insert(0, new ListBoxItem
            {
                Content = $"🔔 {titulo}\n{cuerpo}",
                Foreground = System.Windows.Media.Brushes.WhiteSmoke,
                Background = System.Windows.Media.Brushes.Transparent,
                Padding = new Thickness(12, 8, 12, 8),
                FontSize = 12
            });
        }
        protected override void OnClosed(EventArgs e) { _poller.Stop(); base.OnClosed(e); }
    }
}

