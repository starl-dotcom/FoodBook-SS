using FoodBook_SS.Application.Dtos.Order;
using FoodBook_SS.Application.Dtos.Menu;
using FoodBook_SS.Application.Dtos.Reservation;
using FoodBook_SS.Desktop.Core;
using FoodBook_SS.Desktop.Services;
using System.Collections.ObjectModel;
namespace FoodBook_SS.Desktop.ViewModels.Cliente
{
    public class MenuItemViewModel : ObservableBase
    {
        public ProductDto Producto { get; }
        public MenuItemViewModel(ProductDto producto) => Producto = producto;
        public bool Disponible => Producto.Disponible;
        public decimal Precio => Producto.Precio;
        public string Nombre => Producto.Nombre;
        public string Descripcion => Producto.Descripcion ?? "";
        private int _cantidad;
        public int Cantidad
        {
            get => _cantidad;
            set
            {
                if (SetField(ref _cantidad, value >= 0 ? value : 0))
                {
                    OnPropertyChanged(nameof(Subtotal));
                    OnCantidadChanged?.Invoke();
                }
            }
        }
        public decimal Subtotal => Cantidad * Precio;
        public Action? OnCantidadChanged { get; set; }
        public RelayCommand AddCommand => new(() => Cantidad++);
        public RelayCommand RemoveCommand => new(() => Cantidad--);
    }
    public class NuevaOrdenViewModel : ObservableBase
    {
        private readonly IMenuService _menuSvc;
        private readonly IOrdenService _ordenSvc;
        private readonly ReservationDto _reserva;
        public NuevaOrdenViewModel(ReservationDto reserva, IMenuService menuSvc, IOrdenService ordenSvc)
        {
            _reserva = reserva;
            _menuSvc = menuSvc;
            _ordenSvc = ordenSvc;
        }
        public ObservableCollection<MenuItemViewModel> Productos { get; } = new();
        private string _notas = "";
        public string Notas { get => _notas; set => SetField(ref _notas, value); }
        public decimal TotalOrd => Productos.Sum(p => p.Subtotal);
        public bool PuedeOrdenar => TotalOrd > 0 && !IsBusy;
        public Action? OnOrdenConfirmada;
        public Action? OnCancelada;
        public RelayCommand LoadCommand => new(async () => await LoadAsync());
        public RelayCommand CancelarCommand => new(() => OnCancelada?.Invoke());
        public RelayCommand ConfirmarCommand => new(async () => await ConfirmarAsync(), () => PuedeOrdenar);
        private async Task LoadAsync()
        {
            IsBusy = true; ClearMessages();
            var r = await _menuSvc.GetProductosAsync(_reserva.RestauranteId);
            IsBusy = false;
            if (!r.Success)
            {
                SetError(r.Message);
                return;
            }
            Productos.Clear();
            foreach (var prod in r.Data ?? new List<ProductDto>())
            {
                if (prod.Disponible)
                {
                    var vm = new MenuItemViewModel(prod);
                    vm.OnCantidadChanged = () => 
                    {
                        OnPropertyChanged(nameof(TotalOrd));
                        System.Windows.Input.CommandManager.InvalidateRequerySuggested();
                    };
                    Productos.Add(vm);
                }
            }
            if (Productos.Count == 0)
            {
                SetError("El restaurante no tiene productos disponibles actualmente.");
            }
        }
        private async Task ConfirmarAsync()
        {
            if (TotalOrd == 0) return;
            IsBusy = true; ClearMessages();
            var dto = new SaveOrderDto
            {
                ReservaId = _reserva.Id,
                RestauranteId = _reserva.RestauranteId,
                Notas = Notas,
                Items = Productos
                        .Where(p => p.Cantidad > 0)
                        .Select(p => new SaveOrderItemDto
                        {
                            ProductoId = p.Producto.Id,
                            Cantidad = p.Cantidad,
                            Notas = ""
                        }).ToList()
            };
            var r = await _ordenSvc.CrearAsync(dto);
            IsBusy = false;
            if (r.Success)
            {
                OnOrdenConfirmada?.Invoke();
            }
            else
            {
                SetError(r.Message);
            }
        }
    }
}

