using FoodBook_SS.Application.Dtos.Order;
using FoodBook_SS.Application.Dtos.Restaurant;
namespace FoodBook_SS.Web.Models
{
    public class DashboardViewModel
    {
        public IEnumerable<RestaurantDto> Restaurantes { get; set; } = new List<RestaurantDto>();
        public int? RestauranteIdSeleccionado { get; set; }
        public int DiasPasados { get; set; } = 30;
        public IEnumerable<VentaFechaDto> VentasPorFecha { get; set; } = new List<VentaFechaDto>();
        public IEnumerable<ProductoMasOrdenadoDto> ProductosMasOrdenados { get; set; } = new List<ProductoMasOrdenadoDto>();
        public decimal TotalIngresos       => VentasPorFecha.Sum(v => v.TotalVentas);
        public int    TotalOrdenes         => VentasPorFecha.Sum(v => v.Ordenes);
        public decimal CalificacionPromedio { get; set; }
        public int TotalReservasHoy { get; set; }
    }
}

