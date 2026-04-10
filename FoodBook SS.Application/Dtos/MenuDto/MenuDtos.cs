using FoodBook_SS.Application.Base;
namespace FoodBook_SS.Application.Dtos.Menu
{
    public class CategoryDto : DtoBase
    {
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public int Orden { get; set; }
    }
    public class SaveCategoryDto
    {
        public int RestauranteId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
    }
    public class ProductDto : DtoBase
    {
        public int CategoriaId { get; set; }
        public int RestauranteId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public decimal Precio { get; set; }
        public bool Disponible { get; set; }
        public string? Imagen { get; set; }
        public CategoryDto? Categoria { get; set; }
    }
    public class SaveProductDto
    {
        public int CategoriaId { get; set; }
        public int RestauranteId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public decimal Precio { get; set; }
        public string? Imagen { get; set; }
    }
    public class UpdateProductDto
    {
        public string? Nombre { get; set; }
        public string? Descripcion { get; set; }
        public decimal? Precio { get; set; }
        public string? Imagen { get; set; }
    }
}
