using FoodBook_SS.Desktop.Services;
using FoodBook_SS.Desktop.ViewModels.Cliente;
using System.Windows;
using System.Windows.Controls;
namespace FoodBook_SS.Desktop.Views.Cliente
{
    public partial class ExplorarRestaurantesPage : Page
    {
        private readonly ExplorarRestaurantesViewModel _vm;
        public ExplorarRestaurantesPage(IRestauranteService svc)
        {
            InitializeComponent();
            _vm = new ExplorarRestaurantesViewModel(svc);
            DataContext = _vm;
            Loaded += async (_, _) => await _vm.LoadAsync();
        }
        private void BtnReservar_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.CommandParameter is FoodBook_SS.Application.Dtos.Restaurant.RestaurantDto restaurante)
            {
                if (Window.GetWindow(this) is ClienteShell shell)
                {
                    shell.NavigateToNuevaReservaConRestaurante(restaurante);
                }
            }
        }
    }
}

