using FoodBook_SS.Application.Dtos.Reservation;
using FoodBook_SS.Desktop.Core;
using FoodBook_SS.Desktop.Services;
using System.Collections.ObjectModel;

namespace FoodBook_SS.Desktop.ViewModels.Cliente
{
    public class MisReservasViewModel : ObservableBase
    {
        private readonly IReservaService _svc;
        public MisReservasViewModel(IReservaService svc) => _svc = svc;

        public ObservableCollection<ReservationDto> Reservas { get; } = new();

        private ReservationDto? _selected;
        public ReservationDto? Selected
        {
            get => _selected;
            set { SetField(ref _selected, value); OnPropertyChanged(nameof(CanCancelar)); }
        }

        public bool CanCancelar => Selected?.Estado == "Pendiente" || Selected?.Estado == "Confirmada" || Selected?.Estado == "Aceptada";
        public bool CanOrdenar  => Selected?.Estado == "Pendiente" || Selected?.Estado == "Confirmada" || Selected?.Estado == "Aceptada";

        public Action<ReservationDto>? OnAnadirOrdenRequested;

        public RelayCommand LoadCommand   => new(async () => await LoadAsync());
        public RelayCommand CancelarCommand => new(async () => await CancelarAsync(),
                                                   () => CanCancelar && !IsBusy);
        public RelayCommand AnadirOrdenCommand => new(() => OnAnadirOrdenRequested?.Invoke(Selected!),
                                                      () => CanOrdenar && !IsBusy);

        public async Task LoadAsync()
        {
            IsBusy = true; ClearMessages();
            var r = await _svc.GetMisReservasAsync();
            IsBusy = false;
            if (!r.Success) { SetError(r.Message); return; }
            Reservas.Clear();
            foreach (var item in r.Data ?? []) Reservas.Add(item);
        }

        private async Task CancelarAsync()
        {
            if (Selected is null) return;
            IsBusy = true;
            var r = await _svc.CancelarAsync(Selected.Id);
            IsBusy = false;
            if (!r.Success) { SetError(r.Message); return; }
            SetSuccess("Reserva cancelada correctamente.");
            await LoadAsync();
        }
    }
}
