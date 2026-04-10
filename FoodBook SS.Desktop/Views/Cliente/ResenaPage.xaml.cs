using FoodBook_SS.Desktop.Services;
using System.Windows.Controls;

using FoodBook_SS.Desktop.ViewModels.Cliente;

namespace FoodBook_SS.Desktop.Views.Cliente
{
    public partial class ResenaPage : Page
    {
        public ResenaPage(IResenaService resenas, IOrdenService ordenes, IReservaService reservas, IRestauranteService restaurantes)
        {
            InitializeComponent();
            var vm = new ResenaViewModel(resenas, ordenes, reservas, restaurantes);
            DataContext = vm;
            Loaded += async (_, _) => await vm.CargarAsync();
        }
    }
}
