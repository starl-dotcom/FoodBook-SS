using FoodBook_SS.Application.Base;

namespace FoodBook_SS.Application.Dtos.Order
{
    public class OrderItemDto
    {
        public int ProductoId { get; set; }
        public string NombreProducto { get; set; } = string.Empty;
        public decimal PrecioUnitario { get; set; }
        public int Cantidad { get; set; }
        public decimal Subtotal { get; set; }
    }

    public class OrderDto : DtoBase
    {
        public int ReservaId { get; set; }
        public string Estado { get; set; } = string.Empty;
        public decimal Subtotal { get; set; }
        public decimal Impuesto { get; set; }
        public decimal Total { get; set; }
        public DateTime FechaOrden { get; set; }
        public string? Notas { get; set; }
        public List<OrderItemDto> Items { get; set; } = new();
    }

    public class SaveOrderDto
    {
        public int ReservaId { get; set; }
        public int RestauranteId { get; set; }   
        public string? Notas { get; set; }
        public List<SaveOrderItemDto> Items { get; set; } = new();
    }

    public class SaveOrderItemDto
    {
        public int ProductoId { get; set; }
        public int Cantidad { get; set; }
        public string? Notas { get; set; }
    }

    public class VentaFechaDto
    {
        public DateOnly Fecha { get; set; }
        public decimal TotalVentas { get; set; }
        public int Ordenes { get; set; }
    }

    public class ProductoMasOrdenadoDto
    {
        public int ProductoId { get; set; }
        public string NombreProducto { get; set; } = string.Empty;
        public int TotalVendido { get; set; }
    }
}
