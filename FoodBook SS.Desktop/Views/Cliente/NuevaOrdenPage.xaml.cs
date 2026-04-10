using FoodBook_SS.Application.Dtos.Reservation;
using FoodBook_SS.Desktop.Services;
using FoodBook_SS.Desktop.ViewModels.Cliente;
using System.Windows.Controls;
namespace FoodBook_SS.Desktop.Views.Cliente
{
    public partial class NuevaOrdenPage : Page
    {
        private readonly NuevaOrdenViewModel _vm;
        public NuevaOrdenPage(ReservationDto reserva, IMenuService menuSvc, IOrdenService ordenSvc)
        {
            InitializeComponent();
            _vm = new NuevaOrdenViewModel(reserva, menuSvc, ordenSvc);
            _vm.OnOrdenConfirmada = () => this.NavigationService.GoBack();
            _vm.OnCancelada = () => this.NavigationService.GoBack();
            DataContext = _vm;
            Loaded += (_, _) => _vm.LoadCommand.Execute(null);
        }
    }
}

