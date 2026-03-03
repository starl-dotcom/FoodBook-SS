
namespace FoodBook_SS.Domain.Base
{
    public class BaseEntity
    {
        public int Id { get; set; }
        public DateTime CreadoEn { get; set; } = DateTime.UtcNow;
        public DateTime ActualizadoEn { get; set; } = DateTime.UtcNow;
    }
}
