using FoodBook_SS.Application.Dtos.Review;
using FoodBook_SS.Application.Dtos.Order;
using FoodBook_SS.Desktop.Core;
using FoodBook_SS.Desktop.Services;
using System.Collections.ObjectModel;
namespace FoodBook_SS.Desktop.ViewModels.Cliente
{
    public class ResenaViewModel : ObservableBase
    {
        private readonly IResenaService _resenas;
        private readonly IOrdenService _ordenes;
        private readonly IReservaService _reservas;
        private readonly IRestauranteService _restaurantes;
        public ResenaViewModel(IResenaService resenas, IOrdenService ordenes, IReservaService reservas, IRestauranteService restaurantes)
        {
            _resenas = resenas;
            _ordenes = ordenes;
            _reservas = reservas;
            _restaurantes = restaurantes;
        }
        public ObservableCollection<OrderDto> OrdenesCompletadas { get; } = new();
        private OrderDto? _ordenSeleccionada;
        public OrderDto? OrdenSeleccionada
        {
            get => _ordenSeleccionada;
            set => SetField(ref _ordenSeleccionada, value);
        }
        private int _calificacion = 5;
        public int Calificacion
        {
            get => _calificacion;
            set => SetField(ref _calificacion, value);
        }
        private string _comentario = string.Empty;
        public string Comentario
        {
            get => _comentario;
            set => SetField(ref _comentario, value);
        }
        public RelayCommand LoadCommand => new(async () => await CargarAsync());
        public RelayCommand GuardarCommand => new(async () => await GuardarAsync(), () => OrdenSeleccionada != null && !IsBusy);
        public async Task CargarAsync()
        {
            IsBusy = true; ClearMessages();
            var r = await _ordenes.GetMisOrdenesAsync();
            IsBusy = false;
            if (!r.Success) return;
            OrdenesCompletadas.Clear();
            var ordenes = r.Data ?? new List<OrderDto>();
            foreach (var o in ordenes.Where(x => x.Estado == "Completada" || x.Estado == "Entregada"))
                OrdenesCompletadas.Add(o);
        }
        private async Task GuardarAsync()
        {
            if (OrdenSeleccionada == null) return;
            if (string.IsNullOrWhiteSpace(Comentario)) { SetError("Por favor ingresa un comentario."); return; }
            IsBusy = true; ClearMessages();
            var rRes = await _reservas.GetMisReservasAsync();
            var reserva = rRes.Data?.FirstOrDefault(x => x.Id == OrdenSeleccionada.ReservaId);
            if (reserva == null)
            {
                IsBusy = false; SetError("No se pudo obtener el restaurante de la orden."); return;
            }
            var rRest = await _restaurantes.GetTodosAsync();
            var restauranteId = rRest.Data?.FirstOrDefault(x => x.Nombre == reserva.NombreRestaurante)?.Id ?? 1;
            var dto = new SaveReviewDto
            {
                OrdenId = OrdenSeleccionada.Id,
                RestauranteId = restauranteId,
                Calificacion = (byte)Calificacion,
                Comentario = Comentario
            };
            var r = await _resenas.CrearAsync(dto);
            IsBusy = false;
            if (r.Success)
            {
                SetSuccess("¡Gracias por tu reseña! (RN-05 completado)");
                Comentario = string.Empty;
                Calificacion = 5;
                OrdenSeleccionada = null;
            }
            else
            {
                SetError(r.Message);
            }
        }
    }
}

