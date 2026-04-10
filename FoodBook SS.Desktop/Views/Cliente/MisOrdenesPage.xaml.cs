using FoodBook_SS.Desktop.Services;
using FoodBook_SS.Desktop.ViewModels.Cliente;
using System.Windows.Controls;

namespace FoodBook_SS.Desktop.Views.Cliente
{
    public partial class MisOrdenesPage : Page
    {
        public MisOrdenesPage(IOrdenService svc)
        {
            InitializeComponent();
            var vm = new MisOrdenesViewModel(svc);
            DataContext = vm;
            Loaded += async (_, _) => await vm.LoadAsync();
        }
    }
}
