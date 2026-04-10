using FoodBook_SS.Desktop.Core;
using FoodBook_SS.Desktop.Services;
using System.Windows;
using System.Windows.Controls;

namespace FoodBook_SS.Desktop.Views.Admin
{
    public partial class AdminShell : Window
    {
        private readonly IUsuarioService _usuarioSvc;
        private readonly IRestauranteService _restSvc;
        private readonly IResenaService _resenaSvc;
        private readonly IAuditoriaService _auditSvc;

        public AdminShell(IUsuarioService usuarioSvc, IRestauranteService restSvc,
                          IResenaService resenaSvc,   IAuditoriaService auditSvc)
        {
            InitializeComponent();
            _usuarioSvc = usuarioSvc;
            _restSvc    = restSvc;
            _resenaSvc  = resenaSvc;
            _auditSvc   = auditSvc;

            TxtUsuario.Text = SessionManager.GetUsuario() ?? "Administrador";

            NavTo(new AdminDashboardPage(_usuarioSvc, _restSvc, _resenaSvc));
            SetActiveNav(BtnDashboard);
        }

        private void Nav_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button btn) return;
            SetActiveNav(btn);
            Page page = btn.Name switch
            {
                "BtnUsuarios"     => new AdminUsuariosPage(_usuarioSvc),
                "BtnRestaurantes" => new AdminRestaurantesPage(_restSvc),
                "BtnModeracion"   => new AdminModeracionPage(_resenaSvc),
                "BtnAuditoria"    => new AdminAuditoriaPage(_auditSvc),
                _                 => new AdminDashboardPage(_usuarioSvc, _restSvc, _resenaSvc)
            };
            NavTo(page);
        }

        public void NavigateFromDashboard(string module)
        {
            Button? btn = module switch
            {
                "Usuarios" => BtnUsuarios,
                "Restaurantes" => BtnRestaurantes,
                "Moderacion" => BtnModeracion,
                "Auditoria" => BtnAuditoria,
                _ => BtnDashboard
            };
            if (btn != null) Nav_Click(btn, new RoutedEventArgs());
        }

        private void NavTo(Page p) => MainFrame.Navigate(p);

        private void SetActiveNav(Button active)
        {
            foreach (var b in new[] { BtnDashboard, BtnUsuarios, BtnRestaurantes, BtnModeracion, BtnAuditoria })
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

