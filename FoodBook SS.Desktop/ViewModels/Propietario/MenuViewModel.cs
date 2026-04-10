using FoodBook_SS.Application.Dtos.Menu;
using FoodBook_SS.Desktop.Core;
using FoodBook_SS.Desktop.Services;
using System.Collections.ObjectModel;
namespace FoodBook_SS.Desktop.ViewModels.Propietario
{
    public class MenuViewModel : ObservableBase
    {
        private readonly IMenuService _svc;
        public MenuViewModel(IMenuService svc) => _svc = svc;
        public ObservableCollection<ProductDto>  Productos   { get; } = new();
        public ObservableCollection<CategoryDto> Categorias  { get; } = new();
        private string _nombre = string.Empty;
        public string NombreProducto { get => _nombre; set => SetField(ref _nombre, value); }
        private decimal _precio;
        public decimal Precio { get => _precio; set => SetField(ref _precio, value); }
        private CategoryDto? _catSelected;
        public CategoryDto? CategoriaSelected { get => _catSelected; set => SetField(ref _catSelected, value); }
        public RelayCommand LoadCommand   => new(async () => await LoadAsync());
        public RelayCommand CrearCommand  => new(async () => await CrearProductoAsync(),
            () => !string.IsNullOrWhiteSpace(NombreProducto) && Precio > 0 && CategoriaSelected != null && !IsBusy);
        public RelayCommand ToggleCommand => new(async (p) => await ToggleAsync(p as ProductDto));
        public async Task LoadAsync(int restauranteId = 1)
        {
            IsBusy = true; ClearMessages();
            var cats  = await _svc.GetCategoriasAsync(restauranteId);
            var prods = await _svc.GetProductosAsync(restauranteId);
            IsBusy = false;
            Categorias.Clear(); foreach (var c in cats.Data  ?? []) Categorias.Add(c);
            Productos.Clear();  foreach (var p in prods.Data ?? []) Productos.Add(p);
        }
        private async Task CrearProductoAsync()
        {
            if (CategoriaSelected is null) return;
            IsBusy = true;
            var r = await _svc.CrearProductoAsync(new SaveProductDto
            {
                CategoriaId    = CategoriaSelected.Id,
                RestauranteId  = 1, 
                Nombre         = NombreProducto,
                Precio         = Precio
            });
            IsBusy = false;
            if (!r.Success) { SetError(r.Message); return; }
            NombreProducto = string.Empty; Precio = 0;
            SetSuccess("Producto creado correctamente.");
            await LoadAsync();
        }
        private async Task ToggleAsync(ProductDto? p)
        {
            if (p is null) return;
            await _svc.ToggleDisponibilidadAsync(p.Id);
            await LoadAsync();
        }
    }
}

