using FoodBook_SS.Desktop.Services;
using FoodBook_SS.Desktop.ViewModels.Cliente;
using System.Windows.Controls;

namespace FoodBook_SS.Desktop.Views.Cliente
{
    public partial class PagarPage : Page
    {
        public PagarPage(IPagoService pagos, IOrdenService ordenes)
        {
            InitializeComponent();
            var vm = new PagarViewModel(pagos, ordenes);
            DataContext = vm;
            Loaded += async (_, _) => await vm.LoadAsync();
        }
    }
}
