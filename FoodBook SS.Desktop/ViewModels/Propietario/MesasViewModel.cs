using FoodBook_SS.Application.Dtos.Restaurant;
using FoodBook_SS.Desktop.Core;
using FoodBook_SS.Desktop.Services;
using System.Collections.ObjectModel;
using System.Linq;
namespace FoodBook_SS.Desktop.ViewModels.Propietario
{
    public class MesasViewModel : ObservableBase
    {
        private readonly IRestauranteService _svc;
        public MesasViewModel(IRestauranteService svc) => _svc = svc;
        private RestaurantDto? _restaurante;
        public ObservableCollection<MesaDto> Mesas { get; } = new();
        private string _nuevoNumero = "";
        public string NuevoNumero { get => _nuevoNumero; set => SetField(ref _nuevoNumero, value); }
        private int _nuevaCapacidad = 2;
        public int NuevaCapacidad { get => _nuevaCapacidad; set => SetField(ref _nuevaCapacidad, value); }
        private string _nuevaUbicacion = "";
        public string NuevaUbicacion { get => _nuevaUbicacion; set => SetField(ref _nuevaUbicacion, value); }
        public RelayCommand LoadCommand => new(async () => await LoadAsync());
        public RelayCommand AddCommand => new(async () => await AddMesaAsync(), () => !string.IsNullOrEmpty(NuevoNumero) && NuevaCapacidad > 0);
        public async Task LoadAsync()
        {
            IsBusy = true; ClearMessages();
            var r = await _svc.GetMioAsync();
            IsBusy = false;
            if (!r.Success || r.Data == null) { SetError(r.Message); return; }
            _restaurante = r.Data;
            Mesas.Clear();
            foreach (var m in _restaurante.Mesas.OrderBy(x => x.NumeroMesa))
                Mesas.Add(m);
        }
        private async Task AddMesaAsync()
        {
            if (_restaurante == null) return;
            var dto = new SaveMesaDto
            {
                RestauranteId = _restaurante.Id,
                NumeroMesa = NuevoNumero,
                Capacidad = NuevaCapacidad,
                Ubicacion = NuevaUbicacion
            };
            IsBusy = true; ClearMessages();
            var r = await _svc.AgregarMesaAsync(dto);
            IsBusy = false;
            if (r.Success)
            {
                SetSuccess("Mesa agregada correctamente.");
                NuevoNumero = ""; NuevaCapacidad = 2; NuevaUbicacion = "";
                await LoadAsync();
            }
            else
            {
                SetError(r.Message);
            }
        }
    }
}

