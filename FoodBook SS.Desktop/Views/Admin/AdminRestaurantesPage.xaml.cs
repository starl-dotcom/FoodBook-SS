using FoodBook_SS.Desktop.Services;
using FoodBook_SS.Desktop.ViewModels.Admin;
using System.Windows.Controls;

namespace FoodBook_SS.Desktop.Views.Admin
{
    public partial class AdminRestaurantesPage : Page
    {
        public AdminRestaurantesPage(IRestauranteService svc)
        {
            InitializeComponent();
            var vm = new AdminRestaurantesViewModel(svc);
            DataContext = vm;
            Loaded += async (_, _) => await vm.LoadAsync();
        }
    }
}
