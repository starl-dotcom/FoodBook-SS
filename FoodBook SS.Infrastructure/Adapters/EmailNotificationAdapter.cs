using FoodBook_SS.Application.Interfaces;
using FoodBook_SS.Infrastructure.Services;
using Microsoft.Extensions.Logging;
namespace FoodBook_SS.Infrastructure.Adapters
{
    public class EmailNotificationAdapter : INotificationSender
    {
        private readonly NotificationInbox _inbox;
        private readonly ILogger<EmailNotificationAdapter> _logger;
        public EmailNotificationAdapter(NotificationInbox inbox, ILogger<EmailNotificationAdapter> logger)
        {
            _inbox = inbox;
            _logger = logger;
        }
        public Task EnviarAsync(string destinatario, string asunto, string mensaje)
        {
            _inbox.Push(destinatario, asunto, mensaje);
            _logger.LogInformation("📬 Notificación → {Destinatario} | {Asunto}", destinatario, asunto);
            return Task.CompletedTask;
        }
    }
}

