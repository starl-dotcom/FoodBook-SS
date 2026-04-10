using FoodBook_SS.Desktop.Services;
using FoodBook_SS.Desktop.ViewModels.Propietario;
using System.Windows.Controls;

namespace FoodBook_SS.Desktop.Views.Propietario
{
    public partial class MetricasPage : Page
    {
        public MetricasPage(IOrdenService svc, IReservaService rSvc)
        {
            InitializeComponent();
            var vm = new MetricasViewModel(svc, rSvc);
            DataContext = vm;
            Loaded += async (_, _) => await vm.LoadAsync(FoodBook_SS.Desktop.Core.SessionManager.GetRestauranteId());
        }
    }
}
