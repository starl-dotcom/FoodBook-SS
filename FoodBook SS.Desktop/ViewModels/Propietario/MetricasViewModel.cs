using FoodBook_SS.Application.Dtos.Order;
using FoodBook_SS.Desktop.Core;
using FoodBook_SS.Desktop.Services;
using System.Collections.ObjectModel;
using System.Linq;
namespace FoodBook_SS.Desktop.ViewModels.Propietario
{
    public class MetricasViewModel : ObservableBase
    {
        private readonly IOrdenService _ordenService;
        private readonly IReservaService _reservaService;
        public MetricasViewModel(IOrdenService ordenService, IReservaService reservaService)
        {
            _ordenService = ordenService;
            _reservaService = reservaService;
        }
        private decimal _totalIngresos;
        public decimal TotalIngresos { get => _totalIngresos; set => SetField(ref _totalIngresos, value); }
        private int _totalOrdenes;
        public int TotalOrdenes { get => _totalOrdenes; set => SetField(ref _totalOrdenes, value); }
        private double _calificacionPromedio;
        public double CalificacionPromedio { get => _calificacionPromedio; set => SetField(ref _calificacionPromedio, value); }
        private int _reservasHoy;
        public int ReservasHoy { get => _reservasHoy; set => SetField(ref _reservasHoy, value); }
        public ObservableCollection<ProductoMasOrdenadoDto> TopProductos { get; } = new();
        public RelayCommand LoadCommand => new(async () => await LoadAsync());
        public async Task LoadAsync(int restauranteId = 0)
        {
            if (restauranteId == 0) restauranteId = SessionManager.GetRestauranteId();
            if (restauranteId == 0) return;
            IsBusy = true; ClearMessages();
            var hasta = DateOnly.FromDateTime(DateTime.UtcNow);
            var desde = hasta.AddDays(-30);
            var rOrdenes = await _ordenService.GetByRestauranteAsync(restauranteId);
            if (rOrdenes.Success && rOrdenes.Data != null)
            {
                TotalIngresos = rOrdenes.Data.Where(x => x.Estado != "Cancelada").Sum(x => x.Total);
                TotalOrdenes = rOrdenes.Data.Count;
            }
            CalificacionPromedio = 4.8; 
            var rReservas = await _reservaService.GetByRestauranteAsync(restauranteId, null);
            if (rReservas.Success && rReservas.Data != null)
            {
                var hoy = DateOnly.FromDateTime(DateTime.Today);
                ReservasHoy = rReservas.Data.Count(r => r.FechaReserva == hoy);
            }
            var rTop = await _ordenService.GetTopProductosAsync(restauranteId, desde, hasta);
            TopProductos.Clear();
            if (rTop.Success && rTop.Data != null)
            {
                foreach (var prod in rTop.Data)
                    TopProductos.Add(prod);
            }
            IsBusy = false;
        }
    }
}

