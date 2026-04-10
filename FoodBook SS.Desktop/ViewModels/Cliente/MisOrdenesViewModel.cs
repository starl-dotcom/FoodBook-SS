using FoodBook_SS.Application.Dtos.Order;
using FoodBook_SS.Desktop.Core;
using FoodBook_SS.Desktop.Services;
using System.Collections.ObjectModel;

namespace FoodBook_SS.Desktop.ViewModels.Cliente
{
    public class MisOrdenesViewModel : ObservableBase
    {
        private readonly IOrdenService _svc;
        public MisOrdenesViewModel(IOrdenService svc) => _svc = svc;

        public ObservableCollection<OrderDto> Ordenes { get; } = new();

        private OrderDto? _selected;
        public OrderDto? Selected { get => _selected; set => SetField(ref _selected, value); }

        public RelayCommand LoadCommand => new(async () => await LoadAsync());

        public async Task LoadAsync()
        {
            IsBusy = true; ClearMessages();
            var r = await _svc.GetMisOrdenesAsync();
            IsBusy = false;
            if (!r.Success) { SetError(r.Message); return; }
            Ordenes.Clear();
            foreach (var o in r.Data ?? []) Ordenes.Add(o);
        }
    }
}
