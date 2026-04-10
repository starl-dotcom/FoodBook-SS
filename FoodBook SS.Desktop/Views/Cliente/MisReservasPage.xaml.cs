using FoodBook_SS.Desktop.Services;
using FoodBook_SS.Desktop.ViewModels.Cliente;
using Microsoft.Extensions.DependencyInjection;
using System.Windows.Controls;

namespace FoodBook_SS.Desktop.Views.Cliente
{
    public partial class MisReservasPage : Page
    {
        private readonly MisReservasViewModel _vm;
        public MisReservasPage(IReservaService svc)
        {
            InitializeComponent();
            _vm = new MisReservasViewModel(svc);
            _vm.OnAnadirOrdenRequested = (reserva) => 
            {
                var menuSvc = App.Services.GetRequiredService<IMenuService>();
                var ordenSvc = App.Services.GetRequiredService<IOrdenService>();
                this.NavigationService.Navigate(new NuevaOrdenPage(reserva, menuSvc, ordenSvc));
            };
            DataContext = _vm;
            Loaded += async (_, _) => await _vm.LoadAsync();
        }
    }
}
