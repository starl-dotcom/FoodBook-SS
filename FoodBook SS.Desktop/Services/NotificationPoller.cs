using System.Windows.Threading;
namespace FoodBook_SS.Desktop.Services
{
    public class NotificationPoller
    {
        private readonly Core.ApiClient _api;
        private readonly DispatcherTimer _timer;
        public event Action<string, string>? OnNotificacion; 
        public NotificationPoller(Core.ApiClient api)
        {
            _api   = api;
            _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(10) };
            _timer.Tick += async (_, _) => await PollAsync();
        }
        public void Start() => _timer.Start();
        public void Stop()  => _timer.Stop();
        private async Task PollAsync()
        {
            if (!Core.SessionManager.IsLoggedIn()) return;
            try
            {
                var result = await _api.GetAsync<List<NotificacionDto>>("api/notification/pending");
                if (result.Success && result.Data is { Count: > 0 })
                    foreach (var n in result.Data)
                        OnNotificacion?.Invoke(n.Titulo, n.Cuerpo);
            }
            catch {  }
        }
    }
    public record NotificacionDto(string Titulo, string Cuerpo, string Hora);
}

