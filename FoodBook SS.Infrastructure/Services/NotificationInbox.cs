using FoodBook_SS.Application.Interfaces;
using System.Collections.Concurrent;
namespace FoodBook_SS.Infrastructure.Services
{
    public class NotificationInbox
    {
        private readonly ConcurrentDictionary<string, Queue<InboxMessage>> _store = new();
        public void Push(string email, string titulo, string cuerpo)
        {
            var queue = _store.GetOrAdd(email.ToLowerInvariant(), _ => new Queue<InboxMessage>());
            lock (queue)
            {
                queue.Enqueue(new InboxMessage(titulo, cuerpo, DateTime.Now));
                while (queue.Count > 20) queue.Dequeue();
            }
        }
        public List<InboxMessage> Pop(string email)
        {
            if (!_store.TryGetValue(email.ToLowerInvariant(), out var queue))
                return new();
            lock (queue)
            {
                var msgs = queue.ToList();
                queue.Clear();
                return msgs;
            }
        }
    }
    public record InboxMessage(string Titulo, string Cuerpo, DateTime FechaHora);
}

