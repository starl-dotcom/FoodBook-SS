using FoodBook_SS.Application.Dtos.Order;
using FoodBook_SS.Desktop.Core;
using FoodBook_SS.Desktop.Services;
using System.Collections.ObjectModel;
namespace FoodBook_SS.Desktop.ViewModels.Propietario
{
    public class OrdenesActivasViewModel : ObservableBase
    {
        private readonly IOrdenService _svc;
        public ObservableCollection<OrderDto> OrdenesActivas { get; } = new();
        public OrdenesActivasViewModel(IOrdenService svc) => _svc = svc;
        private OrderDto? _selected;
        public OrderDto? Selected
        {
            get => _selected;
            set { SetField(ref _selected, value); OnPropertyChanged(nameof(CanAvanzar)); }
        }
        public bool CanAvanzar => Selected != null && Selected.Estado != "Completada" && Selected.Estado != "Cancelada";
        public string SiguienteEstadoTexto
        {
            get
            {
                if (Selected == null) return "Actualizar Estado";
                return Selected.Estado switch 
                {
                    "Pendiente" => "Empezar a Preparar",
                    "En Preparación" => "Marcar como Lista",
                    "Lista para Entregar" => "Entregar/Completar",
                    _ => "Actualizar Estado"
                };
            }
        }
        public RelayCommand LoadCommand => new(async () => await LoadAsync(SessionManager.GetRestauranteId()));
        public RelayCommand AvanzarCommand => new(async () => await AvanzarEstadoAsync(), () => CanAvanzar);
        public async Task LoadAsync(int restauranteId)
        {
            if (restauranteId == 0) { SetError("No se encontró tu restaurante asociado."); return; }
            IsBusy = true; ClearMessages();
            var r = await _svc.GetByRestauranteAsync(restauranteId);
            IsBusy = false;
            if (!r.Success) { SetError(r.Message); return; }
            OrdenesActivas.Clear();
            var filtradas = (r.Data ?? Enumerable.Empty<OrderDto>())
                .Where(o => o.Estado != "Completada" && o.Estado != "Cancelada" && o.Estado != "Entregada")
                .OrderByDescending(o => o.FechaOrden);
            foreach (var o in filtradas)
            {
                OrdenesActivas.Add(o);
            }
        }
        private async Task AvanzarEstadoAsync()
        {
            if (Selected is null) return;
            string sgte = Selected.Estado switch 
            {
                "Pendiente" => "En Preparación",
                "En Preparación" => "Lista para Entregar",
                "Lista para Entregar" => "Completada",
                _ => ""
            };
            if (string.IsNullOrEmpty(sgte)) return;
            IsBusy = true; ClearMessages();
            var r = await _svc.CambiarEstadoAsync(Selected.Id, sgte);
            IsBusy = false;
            if (r.Success)
            {
                SetSuccess($"Orden #{Selected.Id} actualizada a {sgte}.");
                Selected = null; 
                await LoadAsync(SessionManager.GetRestauranteId()); 
            }
            else
            {
                SetError(r.Message);
            }
        }
    }
}

