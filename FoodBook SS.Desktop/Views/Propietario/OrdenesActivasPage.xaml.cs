using FoodBook_SS.Desktop.Services;
using System.Windows.Controls;

namespace FoodBook_SS.Desktop.Views.Propietario
{
    public partial class OrdenesActivasPage : Page
    {
        public OrdenesActivasPage(IOrdenService svc)
        {
            InitializeComponent();
            var vm = new FoodBook_SS.Desktop.ViewModels.Propietario.OrdenesActivasViewModel(svc);
            DataContext = vm;
            Loaded += (_, _) => vm.LoadCommand.Execute(null);
        }
    }
}
