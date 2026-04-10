using FoodBook_SS.Application.Dtos.Reservation;
using FoodBook_SS.Desktop.Core;
using FoodBook_SS.Desktop.Services;
using System.Collections.ObjectModel;

namespace FoodBook_SS.Desktop.ViewModels.Propietario
{
    public class ReservasDelDiaViewModel : ObservableBase
    {
        private readonly IReservaService _svc;
        private int _restauranteId;

        public ReservasDelDiaViewModel(IReservaService svc) => _svc = svc;

        public ObservableCollection<ReservationDto> Reservas { get; } = new();

        private ReservationDto? _selected;
        public ReservationDto? Selected
        {
            get => _selected;
            set { SetField(ref _selected, value); OnPropertyChanged(nameof(CanConfirmar)); OnPropertyChanged(nameof(CanCompletar)); }
        }

        public bool CanConfirmar => Selected?.Estado == "Pendiente";
        public bool CanCompletar => Selected?.Estado == "Confirmada";

        public string FiltroEstado { get; set; } = string.Empty;

        public RelayCommand LoadCommand     => new(async () => await LoadAsync());
        public RelayCommand ConfirmarCommand => new(async () => await CambiarEstadoAsync("confirmar"), () => CanConfirmar && !IsBusy);
        public RelayCommand CompletarCommand => new(async () => await CambiarEstadoAsync("completar"), () => CanCompletar && !IsBusy);
        public RelayCommand NoShowCommand    => new(async () => await CambiarEstadoAsync("noshow"),    () => Selected != null && !IsBusy);
        public RelayCommand CancelarCommand  => new(async () => await CambiarEstadoAsync("cancelar"),  () => Selected != null && !IsBusy);

        public async Task LoadAsync(int restauranteId = 0)
        {
            if (restauranteId > 0) _restauranteId = restauranteId;
            if (_restauranteId == 0) { SetError("No se encontró el restaurante asociado."); return; }

            IsBusy = true; ClearMessages();
            var r = await _svc.GetByRestauranteAsync(_restauranteId, string.IsNullOrEmpty(FiltroEstado) ? null : FiltroEstado);
            IsBusy = false;
            if (!r.Success) { SetError(r.Message); return; }

            Reservas.Clear();
            foreach (var rv in r.Data ?? []) Reservas.Add(rv);
        }

        private async Task CambiarEstadoAsync(string accion)
        {
            if (Selected is null) return;
            IsBusy = true;
            var r = accion switch
            {
                "confirmar" => await _svc.ConfirmarAsync(Selected.Id),
                "completar" => await _svc.CompletarAsync(Selected.Id),
                "noshow"    => await _svc.MarcarNoShowAsync(Selected.Id),
                _           => await _svc.CancelarAsync(Selected.Id)
            };
            IsBusy = false;
            if (!r.Success) { SetError(r.Message); return; }
            SetSuccess($"Reserva actualizada correctamente.");
            await LoadAsync();
        }
    }
}
