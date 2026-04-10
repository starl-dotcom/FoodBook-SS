namespace FoodBook_SS.Web.Models
{
    public class DashboardMenuViewModel
    {
        public IEnumerable<FoodBook_SS.Domain.Entities.Configuration.CategoriaMenu> Categorias { get; set; }
            = Enumerable.Empty<FoodBook_SS.Domain.Entities.Configuration.CategoriaMenu>();

        public IEnumerable<FoodBook_SS.Domain.Entities.Configuration.ProductoMenu> Productos { get; set; }
            = Enumerable.Empty<FoodBook_SS.Domain.Entities.Configuration.ProductoMenu>();
    }
}
