using FoodBook_SS.Desktop.Core;
using FoodBook_SS.Desktop.Services;
using System.Windows;
using System.Windows.Controls;

namespace FoodBook_SS.Desktop.Views.Propietario
{
    public partial class PropietarioShell : Window
    {
        private readonly IReservaService     _reservas;
        private readonly IOrdenService       _ordenes;
        private readonly IMenuService        _menu;
        private readonly IRestauranteService _restService;
        private readonly IResenaService      _resenaService;

        public PropietarioShell(IReservaService reservas, IOrdenService ordenes, IMenuService menu,
                                IRestauranteService restService, IResenaService resenaService)
        {
            InitializeComponent();
            _reservas = reservas; _ordenes = ordenes; _menu = menu;
            _restService = restService; _resenaService = resenaService;
            TxtUsuario.Text = SessionManager.GetUsuario() ?? "Propietario";
            NavTo(new ReservasDelDiaPage(_reservas));
            SetActiveNav(BtnReservas);
        }

        private void Nav_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button btn) return;
            SetActiveNav(btn);
            Page page = btn.Name switch
            {
                "BtnOrdenes"  => new OrdenesActivasPage(_ordenes),
                "BtnMenu"     => new MenuPage(_menu),
                "BtnMesas"    => new MesasPage(_restService),
                "BtnResenas"  => new ResenasPage(_resenaService),
                "BtnMetricas" => new MetricasPage(_ordenes, _reservas),
                _             => new ReservasDelDiaPage(_reservas)
            };
            NavTo(page);
        }

        private void NavTo(Page p) => MainFrame.Navigate(p);

        private void SetActiveNav(Button active)
        {
            foreach (var b in new[] { BtnReservas, BtnOrdenes, BtnMenu, BtnMesas, BtnResenas, BtnMetricas })
                b.Style = (Style)FindResource(b == active ? "NavItemActive" : "NavItem");
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            SessionManager.Clear();
            var login = App.Services.GetService(typeof(Views.LoginWindow)) as Window;
            login?.Show();
            Close();
        }
    }
}
