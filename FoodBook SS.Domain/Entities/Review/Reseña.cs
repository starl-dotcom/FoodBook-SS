using FoodBook_SS.Domain.Base;
using FoodBook_SS.Domain.Entities.Configuration;
using FoodBook_SS.Domain.Entities.User;
namespace FoodBook_SS.Domain.Entities.Review
{
    public class Resena : BaseEntity
    {
        public int ClienteId { get; set; }
        public int RestauranteId { get; set; }
        public int OrdenId { get; set; }
        public byte Calificacion { get; set; }
        public string? Comentario { get; set; }
        public string? Respuesta { get; set; }
        public DateTime? FechaRespuesta { get; set; }
        public bool Visible { get; set; } = true;
        public int? ModeradaPor { get; set; }
        public DateTime? FechaModeracion { get; set; }
        public Usuario? Cliente { get; set; }
        public Restaurante? Restaurante { get; set; }
    }
}
