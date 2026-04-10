using FoodBook_SS.Desktop.Services;
using FoodBook_SS.Desktop.ViewModels.Admin;
using System.Windows.Controls;

namespace FoodBook_SS.Desktop.Views.Admin
{
    public partial class AdminAuditoriaPage : Page
    {
        public AdminAuditoriaPage(IAuditoriaService svc)
        {
            InitializeComponent();
            var vm = new AdminAuditoriaViewModel(svc);
            DataContext = vm;
            Loaded += async (_, _) => await vm.LoadAsync();
        }
    }
}
