using FoodBook_SS.Desktop.Services;
using FoodBook_SS.Desktop.ViewModels.Admin;
using System.Windows.Controls;

namespace FoodBook_SS.Desktop.Views.Admin
{
    public partial class AdminUsuariosPage : Page
    {
        public AdminUsuariosPage(IUsuarioService svc)
        {
            InitializeComponent();
            var vm = new AdminUsuariosViewModel(svc);
            DataContext = vm;
            Loaded += async (_, _) => await vm.LoadAsync();
        }
    }
}
