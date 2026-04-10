using FoodBook_SS.Desktop.Services;
using FoodBook_SS.Desktop.ViewModels.Propietario;
using System.Windows.Controls;

namespace FoodBook_SS.Desktop.Views.Propietario
{
    public partial class MesasPage : Page
    {
        public MesasPage(IRestauranteService svc)
        {
            InitializeComponent();
            var vm = new MesasViewModel(svc);
            DataContext = vm;
            Loaded += async (_, _) => await vm.LoadAsync();
        }
    }
}
