using FoodBook_SS.Application.Dtos.Restaurant;
using FoodBook_SS.Application.Dtos.Review;
using FoodBook_SS.Application.Dtos.User;
using FoodBook_SS.Desktop.Core;
using FoodBook_SS.Desktop.Services;
using System.Collections.ObjectModel;
namespace FoodBook_SS.Desktop.ViewModels.Admin
{
    public class AdminDashboardViewModel : ObservableBase
    {
        private readonly IUsuarioService     _usuarioSvc;
        private readonly IRestauranteService _restauranteSvc;
        private readonly IResenaService      _resenaSvc;
        public AdminDashboardViewModel(IUsuarioService usuarioSvc, IRestauranteService restauranteSvc, IResenaService resenaSvc)
        {
            _usuarioSvc    = usuarioSvc;
            _restauranteSvc = restauranteSvc;
            _resenaSvc      = resenaSvc;
        }
        private int _totalUsuarios;
        public int TotalUsuarios { get => _totalUsuarios; set => SetField(ref _totalUsuarios, value); }
        private int _usuariosActivos;
        public int UsuariosActivos { get => _usuariosActivos; set => SetField(ref _usuariosActivos, value); }
        private int _totalRestaurantes;
        public int TotalRestaurantes { get => _totalRestaurantes; set => SetField(ref _totalRestaurantes, value); }
        private int _restaurantesActivos;
        public int RestaurantesActivos { get => _restaurantesActivos; set => SetField(ref _restaurantesActivos, value); }
        private int _totalResenas;
        public int TotalResenas { get => _totalResenas; set => SetField(ref _totalResenas, value); }
        private int _resenasPendienteRespuesta;
        public int ResenasPendienteRespuesta { get => _resenasPendienteRespuesta; set => SetField(ref _resenasPendienteRespuesta, value); }
        private int _resenasOcultas;
        public int ResenasOcultas { get => _resenasOcultas; set => SetField(ref _resenasOcultas, value); }
        public async Task LoadAsync()
        {
            IsBusy = true;
            try
            {
                var usersRes = await _usuarioSvc.GetTodosAsync();
                if (usersRes.Success && usersRes.Data != null)
                {
                    TotalUsuarios   = usersRes.Data.Count;
                    UsuariosActivos = usersRes.Data.Count(u => u.Activo);
                }
                var restRes = await _restauranteSvc.GetTodosAsync();
                if (restRes.Success && restRes.Data != null)
                {
                    TotalRestaurantes   = restRes.Data.Count;
                    RestaurantesActivos = restRes.Data.Count(r => r.Activo);
                }
                var resenasRes = await _resenaSvc.GetAllAsync();
                if (resenasRes.Success && resenasRes.Data != null)
                {
                    TotalResenas               = resenasRes.Data.Count;
                    ResenasPendienteRespuesta  = resenasRes.Data.Count(r => string.IsNullOrEmpty(r.Respuesta));
                    ResenasOcultas             = resenasRes.Data.Count(r => !r.Visible);
                }
            }
            finally { IsBusy = false; }
        }
    }
    public class AdminUsuariosViewModel : ObservableBase
    {
        private readonly IUsuarioService _svc;
        public ObservableCollection<UserDto> Usuarios { get; } = new();
        public RelayCommand ToggleCommand { get; }
        private string? _statusMessage;
        public string? StatusMessage { get => _statusMessage; set => SetField(ref _statusMessage, value); }
        public AdminUsuariosViewModel(IUsuarioService svc)
        {
            _svc = svc;
            ToggleCommand = new RelayCommand(async obj =>
            {
                if (obj is not UserDto u) return;
                bool nuevoEstado = !u.Activo;
                var r = await _svc.ActivarDesactivarAsync(u.Id, nuevoEstado);
                if (r.Success)
                {
                    StatusMessage = $"Usuario {u.NombreCompleto} {(nuevoEstado ? "activado" : "bloqueado")}.";
                    await LoadAsync();
                }
                else
                {
                    ErrorMessage = r.Message;
                }
            });
        }
        public async Task LoadAsync()
        {
            IsBusy = true;
            try
            {
                var res = await _svc.GetTodosAsync();
                Usuarios.Clear();
                if (res.Success && res.Data != null)
                    foreach (var u in res.Data.OrderBy(x => x.Email))
                        Usuarios.Add(u);
                else
                    ErrorMessage = res.Message;
            }
            finally { IsBusy = false; }
        }
    }
    public class AdminRestaurantesViewModel : ObservableBase
    {
        private readonly IRestauranteService _svc;
        public ObservableCollection<RestaurantDto> Restaurantes { get; } = new();
        public RelayCommand ToggleCommand { get; }
        private string? _statusMessage;
        public string? StatusMessage { get => _statusMessage; set => SetField(ref _statusMessage, value); }
        public AdminRestaurantesViewModel(IRestauranteService svc)
        {
            _svc = svc;
            ToggleCommand = new RelayCommand(async obj =>
            {
                if (obj is not RestaurantDto r) return;
                bool nuevoEstado = !r.Activo;
                var res = await _svc.ToggleEstadoAsync(r.Id, nuevoEstado);
                if (res.Success)
                {
                    StatusMessage = $"Restaurante '{r.Nombre}' {(nuevoEstado ? "aprobado" : "suspendido")}.";
                    await LoadAsync();
                }
                else
                {
                    ErrorMessage = res.Message;
                }
            });
        }
        public async Task LoadAsync()
        {
            IsBusy = true;
            try
            {
                var res = await _svc.GetTodosAsync();
                Restaurantes.Clear();
                if (res.Success && res.Data != null)
                    foreach (var r in res.Data.OrderByDescending(x => x.Id))
                        Restaurantes.Add(r);
                else
                    ErrorMessage = res.Message;
            }
            finally { IsBusy = false; }
        }
    }
    public class AdminModeracionViewModel : ObservableBase
    {
        private readonly IResenaService _svc;
        public ObservableCollection<ReviewDto> Resenas { get; } = new();
        public RelayCommand ToggleVisibleCommand { get; }
        private string? _statusMessage;
        public string? StatusMessage { get => _statusMessage; set => SetField(ref _statusMessage, value); }
        public AdminModeracionViewModel(IResenaService svc)
        {
            _svc = svc;
            ToggleVisibleCommand = new RelayCommand(async obj =>
            {
                if (obj is not ReviewDto r) return;
                bool nuevoEstado = !r.Visible;
                var res = await _svc.ModerarAsync(r.Id, nuevoEstado);
                if (res.Success)
                {
                    StatusMessage = $"Reseña {(nuevoEstado ? "visible" : "ocultada")}.";
                    await LoadAsync();
                }
                else
                {
                    ErrorMessage = res.Message;
                }
            });
        }
        public async Task LoadAsync()
        {
            IsBusy = true;
            try
            {
                var res = await _svc.GetAllAsync();
                Resenas.Clear();
                if (res.Success && res.Data != null)
                    foreach (var r in res.Data.OrderByDescending(x => x.CreadoEn))
                        Resenas.Add(r);
                else
                    ErrorMessage = res.Message;
            }
            finally { IsBusy = false; }
        }
    }
    public class AdminAuditoriaViewModel : ObservableBase
    {
        private readonly IAuditoriaService _svc;
        public ObservableCollection<AuditLogDto> Logs { get; } = new();
        public AdminAuditoriaViewModel(IAuditoriaService svc) => _svc = svc;
        public async Task LoadAsync()
        {
            IsBusy = true;
            try
            {
                var res = await _svc.GetLogsAsync();
                Logs.Clear();
                if (res.Success && res.Data != null)
                    foreach (var l in res.Data.OrderByDescending(x => x.FechaHora))
                        Logs.Add(l);
                else
                    ErrorMessage = res.Message;
            }
            finally { IsBusy = false; }
        }
    }
}

