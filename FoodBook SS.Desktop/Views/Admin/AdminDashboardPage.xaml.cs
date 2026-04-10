using FoodBook_SS.Desktop.Services;
using FoodBook_SS.Desktop.ViewModels.Admin;
using System.Windows.Controls;

namespace FoodBook_SS.Desktop.Views.Admin
{
    public partial class AdminDashboardPage : Page
    {
        public AdminDashboardPage(IUsuarioService usuarioSvc, IRestauranteService restauranteSvc, IResenaService resenaSvc)
        {
            InitializeComponent();
            var vm = new AdminDashboardViewModel(usuarioSvc, restauranteSvc, resenaSvc);
            DataContext = vm;
            Loaded += async (_, _) => await vm.LoadAsync();
        }

        private void Modulo_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (sender is Border b && b.Tag is string modulo)
            {
                if (System.Windows.Window.GetWindow(this) is AdminShell shell)
                {
                    shell.NavigateFromDashboard(modulo);
                }
            }
        }
    }
}
