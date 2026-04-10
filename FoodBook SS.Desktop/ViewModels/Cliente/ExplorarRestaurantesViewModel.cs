using FoodBook_SS.Application.Dtos.Restaurant;
using FoodBook_SS.Desktop.Core;
using FoodBook_SS.Desktop.Services;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
namespace FoodBook_SS.Desktop.ViewModels.Cliente
{
    public class ExplorarRestaurantesViewModel : ObservableBase
    {
        private readonly IRestauranteService _svc;
        public ObservableCollection<RestaurantDto> Restaurantes { get; } = new();
        private string _filtroNombre = "";
        public string FiltroNombre { get => _filtroNombre; set => SetField(ref _filtroNombre, value); }
        private string _filtroCiudad = "";
        public string FiltroCiudad { get => _filtroCiudad; set => SetField(ref _filtroCiudad, value); }
        private string _filtroTipoCocina = "";
        public string FiltroTipoCocina { get => _filtroTipoCocina; set => SetField(ref _filtroTipoCocina, value); }
        public RelayCommand SearchCommand { get; }
        public RelayCommand ClearFiltersCommand { get; }
        public ExplorarRestaurantesViewModel(IRestauranteService svc)
        {
            _svc = svc;
            SearchCommand = new RelayCommand(async () => await LoadAsync());
            ClearFiltersCommand = new RelayCommand(async () => await ClearAndLoadAsync());
        }
        public async Task LoadAsync()
        {
            IsBusy = true; ClearMessages();
            var res = await _svc.SearchAsync(FiltroNombre, FiltroCiudad, FiltroTipoCocina);
            IsBusy = false;
            if (res.Success && res.Data != null)
            {
                Restaurantes.Clear();
                foreach (var r in res.Data.Where(x => x.Activo)) 
                {
                    Restaurantes.Add(r);
                }
                if (Restaurantes.Count == 0)
                {
                    SetError("Infelizmente no se encontraron restaurantes con esos criterios.");
                }
            }
            else
            {
                SetError(res.Message);
            }
        }
        private async Task ClearAndLoadAsync()
        {
            FiltroNombre = "";
            FiltroCiudad = "";
            FiltroTipoCocina = "";
            await LoadAsync();
        }
    }
}

