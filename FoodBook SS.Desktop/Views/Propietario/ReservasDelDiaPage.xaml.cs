using FoodBook_SS.Desktop.Services;
using FoodBook_SS.Desktop.ViewModels.Propietario;
using System.Windows.Controls;

namespace FoodBook_SS.Desktop.Views.Propietario
{
    public partial class ReservasDelDiaPage : Page
    {
        public ReservasDelDiaPage(IReservaService svc)
        {
            InitializeComponent();
            var vm = new ReservasDelDiaViewModel(svc);
            DataContext = vm;
            Loaded += async (_, _) => await vm.LoadAsync(FoodBook_SS.Desktop.Core.SessionManager.GetRestauranteId());
        }
    }
}
