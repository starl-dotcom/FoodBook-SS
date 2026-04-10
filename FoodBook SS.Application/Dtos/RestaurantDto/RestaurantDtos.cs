using FoodBook_SS.Application.Base;
namespace FoodBook_SS.Application.Dtos.Restaurant
{
    public class RestaurantDto : DtoBase
    {
        public int PropietarioId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? TipoCocina { get; set; }
        public string Direccion { get; set; } = string.Empty;
        public string Ciudad { get; set; } = string.Empty;
        public string? RangoPrecio { get; set; }
        public decimal CalificacionProm { get; set; }
        public int TotalResenas { get; set; }
        public bool Activo { get; set; }
        public List<MesaDto> Mesas { get; set; } = new();
    }

    public class MesaDto : DtoBase
    {
        public string NumeroMesa { get; set; } = string.Empty;
        public int Capacidad { get; set; }
        public string? Ubicacion { get; set; }
        public bool Disponible { get; set; }
    }
    public class SaveRestaurantDto
    {
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public string? TipoCocina { get; set; }
        public string Direccion { get; set; } = string.Empty;
        public string Ciudad { get; set; } = string.Empty;
        public string? Telefono { get; set; }
        public string? RangoPrecio { get; set; }
    }
    public class UpdateRestaurantDto
    {
        public string? Descripcion { get; set; }
        public string? Telefono { get; set; }
        public string? RangoPrecio { get; set; }
    }
    public class SaveMesaDto
    {
        public int RestauranteId { get; set; }
        public string NumeroMesa { get; set; } = string.Empty;
        public int Capacidad { get; set; }
        public string? Ubicacion { get; set; }
    }

    public class MesaDisponibleDto
    {
        public int Id { get; set; }
        public string NumeroMesa { get; set; } = string.Empty;
        public int Capacidad { get; set; }
        public string? Ubicacion { get; set; }
    }
}




