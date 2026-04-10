using FoodBook_SS.Desktop.Services;
using FoodBook_SS.Desktop.ViewModels.Cliente;
using System.Windows.Controls;

namespace FoodBook_SS.Desktop.Views.Cliente
{
    public partial class NuevaReservaPage : Page
    {
        private readonly NuevaReservaViewModel _vm;

        public NuevaReservaPage(IRestauranteService restSvc, IReservaService svc, FoodBook_SS.Application.Dtos.Restaurant.RestaurantDto? restaurante = null)
        {
            InitializeComponent();
            _vm = new NuevaReservaViewModel(restSvc, svc, restaurante);
            DataContext = _vm;
            Loaded += async (_, _) => await _vm.LoadAsync();
        }
    }
}
