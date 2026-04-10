using FoodBook_SS.Application.Dtos.Payment;
using FoodBook_SS.Desktop.Core;
using FoodBook_SS.Desktop.Services;
using System.Collections.ObjectModel;
using FoodBook_SS.Application.Dtos.Order;

namespace FoodBook_SS.Desktop.ViewModels.Cliente
{
    public class PagarViewModel : ObservableBase
    {
        private readonly IPagoService  _pagos;
        private readonly IOrdenService _ordenes;
        public PagarViewModel(IPagoService pagos, IOrdenService ordenes)
        { _pagos = pagos; _ordenes = ordenes; }

        public ObservableCollection<OrderDto> OrdenesPendientes { get; } = new();

        private OrderDto? _ordenSelected;
        public OrderDto? OrdenSelected
        {
            get => _ordenSelected;
            set { SetField(ref _ordenSelected, value); OnPropertyChanged(nameof(Monto)); }
        }

        public decimal Monto => OrdenSelected?.Total ?? 0;

        private string _metodo = "Tarjeta";
        public string MetodoPago { get => _metodo; set => SetField(ref _metodo, value); }

        public string[] Metodos { get; } = ["Tarjeta", "Efectivo", "Transferencia"];

        public RelayCommand LoadCommand  => new(async () => await LoadAsync());
        public RelayCommand PagarCommand => new(async () => await PagarAsync(),
            () => OrdenSelected != null && !IsBusy);

        public async Task LoadAsync()
        {
            IsBusy = true;
            var r = await _ordenes.GetMisOrdenesAsync();
            IsBusy = false;
            if (!r.Success) { SetError(r.Message); return; }
            OrdenesPendientes.Clear();
            foreach (var o in r.Data?.Where(x => x.Estado == "Pendiente") ?? [])
                OrdenesPendientes.Add(o);
        }

        private async Task PagarAsync()
        {
            if (OrdenSelected is null) return;
            IsBusy = true; ClearMessages();
            var r = await _pagos.ProcesarPagoAsync(new SavePaymentDto
            {
                OrdenId    = OrdenSelected.Id,
                Monto      = OrdenSelected.Total,
                MetodoPago = MetodoPago
            });
            IsBusy = false;
            if (!r.Success) { SetError(r.Message); return; }
            SetSuccess($"✅ Pago procesado. Método: {MetodoPago}. Monto: RD${Monto:N2}");
            await LoadAsync();
        }
    }
}
