using FoodBook_SS.Application.Dtos.Restaurant;
using FoodBook_SS.Application.Dtos.Reservation;
using FoodBook_SS.Desktop.Core;
using FoodBook_SS.Desktop.Services;
using System.Collections.ObjectModel;
namespace FoodBook_SS.Desktop.ViewModels.Cliente
{
    public class NuevaReservaViewModel : ObservableBase
    {
        private readonly IReservaService _svc;
        private readonly IRestauranteService _restSvc;
        private readonly RestaurantDto? _initialRestaurante;
        public ObservableCollection<RestaurantDto> Restaurantes { get; } = new();
        public ObservableCollection<MesaDisponibleDto> Mesas { get; } = new();
        public List<string> HorariosDisponibles { get; } = new() 
        {
            "11:00 AM", "11:30 AM", "12:00 PM", "12:30 PM", "1:00 PM", "1:30 PM", 
            "2:00 PM", "2:30 PM", "3:00 PM", "3:30 PM", "4:00 PM", "4:30 PM", 
            "5:00 PM", "5:30 PM", "6:00 PM", "6:30 PM", "7:00 PM", "7:30 PM", 
            "8:00 PM", "8:30 PM", "9:00 PM", "9:30 PM", "10:00 PM"
        };
        private RestaurantDto? _restauranteSelected;
        public RestaurantDto? RestauranteSelected
        {
            get => _restauranteSelected;
            set
            {
                if (SetField(ref _restauranteSelected, value))
                {
                    Mesas.Clear();
                    MesaSelected = null;
                    RelayCommand.RaiseCanExecuteChanged();
                }
            }
        }
        private DateTime? _fecha;
        public DateTime? Fecha 
        { 
            get => _fecha; 
            set 
            { 
                if (SetField(ref _fecha, value)) RelayCommand.RaiseCanExecuteChanged(); 
            } 
        }
        private string? _hora = "12:00 PM";
        public string? Hora 
        { 
            get => _hora; 
            set 
            { 
                if (SetField(ref _hora, value)) RelayCommand.RaiseCanExecuteChanged(); 
            } 
        }
        private int _personas = 2;
        public int Personas { get => _personas; set => SetField(ref _personas, value); }
        private MesaDisponibleDto? _mesaSelected;
        public MesaDisponibleDto? MesaSelected 
        { 
            get => _mesaSelected; 
            set 
            {
                if (SetField(ref _mesaSelected, value)) RelayCommand.RaiseCanExecuteChanged();
            } 
        }
        public RelayCommand LoadCommand => new(async () => await LoadAsync());
        public RelayCommand BuscarMesasCommand => new(async () => await BuscarMesasAsync());
        public RelayCommand ReservarCommand => new(async () => await ReservarAsync());
        public NuevaReservaViewModel(IRestauranteService restSvc, IReservaService svc, RestaurantDto? restaurante = null)
        {
            _restSvc = restSvc;
            _svc = svc;
            _initialRestaurante = restaurante;
        }
        public Task LoadAsync()
        {
            if (_initialRestaurante == null)
            {
                SetError("No se seleccionó un restaurante previo.");
                return Task.CompletedTask;
            }
            RestauranteSelected = _initialRestaurante;
            return Task.CompletedTask;
        }
        private async Task BuscarMesasAsync()
        {
            if (RestauranteSelected == null)
            {
                SetError("Deba seleccionar un restaurante."); return;
            }
            if (!Fecha.HasValue)
            {
                SetError("Por favor, seleccione una fecha válida para la reserva."); return;
            }
            if (!TimeOnly.TryParse(Hora, out var timeSelected)) {
                SetError("Formato de hora incorrecto o nulo.");
                return;
            }
            IsBusy = true; ClearMessages();
            var r = await _svc.GetMesasDisponiblesAsync(RestauranteSelected.Id, DateOnly.FromDateTime(Fecha.Value), timeSelected, Personas);
            IsBusy = false;
            if (!r.Success || r.Data == null || r.Data.Count == 0)
            {
                SetError("No hay mesas disponibles para ese horario y esa cantidad de personas. (RN-02)");
                Mesas.Clear();
                return;
            }
            Mesas.Clear();
            foreach (var m in r.Data) Mesas.Add(m);
            SetSuccess($"Se encontraron {Mesas.Count} mesas disponibles.");
        }
        private async Task ReservarAsync()
        {
            if (RestauranteSelected == null || MesaSelected == null || !Fecha.HasValue) return;
            if (!TimeOnly.TryParse(Hora, out var timeSelected)) {
                SetError("Formato de hora incorrecto.");
                return;
            }
            IsBusy = true; ClearMessages();
            var dto = new SaveReservationDto
            {
                RestauranteId = RestauranteSelected.Id,
                FechaReserva  = DateOnly.FromDateTime(Fecha.Value),
                HoraReserva   = timeSelected,
                NumeroPersonas = Personas,
                MesaId        = MesaSelected.Id
            };
            var rReserva = await _svc.CrearAsync(dto);
            IsBusy = false;
            if (rReserva.Success)
            {
                SetSuccess($"¡Reserva confirmada en {RestauranteSelected.Nombre}! Puedes verla en 'Mis Reservas'.");
            }
            else
            {
                SetError(rReserva.Message);
            }
        }
    }
}

