using FoodBook_SS.Application.Dtos.Review;
using FoodBook_SS.Desktop.Core;
using FoodBook_SS.Desktop.Services;
using System.Collections.ObjectModel;
using System.Linq;

namespace FoodBook_SS.Desktop.ViewModels.Propietario
{
    public class ResenasViewModel : ObservableBase
    {
        private readonly IResenaService _svc;
        public ResenasViewModel(IResenaService svc) => _svc = svc;

        public ObservableCollection<ReviewDto> Resenas { get; } = new();

        private ReviewDto? _selected;
        public ReviewDto? Selected
        {
            get => _selected;
            set { SetField(ref _selected, value); OnPropertyChanged(nameof(CanResponder)); }
        }

        private string _respuestaText = "";
        public string RespuestaText { get => _respuestaText; set => SetField(ref _respuestaText, value); }

        public bool CanResponder => Selected != null && string.IsNullOrEmpty(Selected.Respuesta);

        public RelayCommand LoadCommand => new(async () => await LoadAsync());
        public RelayCommand ResponderCommand => new(async () => await ResponderAsync(), () => CanResponder && !string.IsNullOrEmpty(RespuestaText));
        
        public RelayCommand SelectCommand => new(param =>
        {
            if (param is ReviewDto dto) Selected = dto;
        });

        public RelayCommand CancelCommand => new(() =>
        {
            Selected = null;
            RespuestaText = "";
        });

        public async Task LoadAsync()
        {
            var restauranteId = SessionManager.GetRestauranteId();
            if (restauranteId == 0) return;

            IsBusy = true; ClearMessages();
            var r = await _svc.GetByRestauranteAsync(restauranteId);
            IsBusy = false;

            if (r.Success && r.Data != null)
            {
                Resenas.Clear();
                foreach (var res in r.Data.OrderByDescending(x => x.CreadoEn))
                    Resenas.Add(res);
            }
            else
            {
                SetError(r.Message);
            }
        }

        private async Task ResponderAsync()
        {
            if (Selected == null) return;

            IsBusy = true; ClearMessages();
            var r = await _svc.ResponderAsync(Selected.Id, RespuestaText);
            IsBusy = false;

            if (r.Success)
            {
                SetSuccess("Respuesta enviada correctamente.");
                RespuestaText = "";
                await LoadAsync();
            }
            else
            {
                SetError(r.Message);
            }
        }
    }
}
