using FoodBook_SS.Desktop.Services;
using FoodBook_SS.Desktop.ViewModels.Propietario;
using System.Windows.Controls;

namespace FoodBook_SS.Desktop.Views.Propietario
{
    public partial class MenuPage : Page
    {
        public MenuPage(IMenuService svc)
        {
            InitializeComponent();
            var vm = new MenuViewModel(svc);
            DataContext = vm;
            Loaded += async (_, _) => await vm.LoadAsync(1);
        }
    }
}
