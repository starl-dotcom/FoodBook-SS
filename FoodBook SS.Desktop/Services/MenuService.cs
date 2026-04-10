using FoodBook_SS.Application.Dtos.Menu;
using FoodBook_SS.Desktop.Core;

namespace FoodBook_SS.Desktop.Services
{
    public interface IMenuService
    {
        Task<ApiResult<List<CategoryDto>>> GetCategoriasAsync(int restauranteId);
        Task<ApiResult<List<ProductDto>>>  GetProductosAsync(int restauranteId);
        Task<ApiResult<object>>            CrearCategoriaAsync(SaveCategoryDto dto);
        Task<ApiResult<object>>            CrearProductoAsync(SaveProductDto dto);
        Task<ApiResult<object>>            ActualizarProductoAsync(int id, UpdateProductDto dto);
        Task<ApiResult<object>>            ToggleDisponibilidadAsync(int productoId);
    }

    public class MenuService : IMenuService
    {
        private readonly ApiClient _api;
        public MenuService(ApiClient api) => _api = api;

        public Task<ApiResult<List<CategoryDto>>> GetCategoriasAsync(int restauranteId)
            => _api.GetAsync<List<CategoryDto>>($"api/menu/{restauranteId}/categorias");

        public Task<ApiResult<List<ProductDto>>> GetProductosAsync(int restauranteId)
            => _api.GetAsync<List<ProductDto>>($"api/menu/{restauranteId}/productos");

        public Task<ApiResult<object>> CrearCategoriaAsync(SaveCategoryDto dto)
            => _api.PostAsync<object>("api/menu/categorias", dto);

        public Task<ApiResult<object>> CrearProductoAsync(SaveProductDto dto)
            => _api.PostAsync<object>("api/menu/productos", dto);

        public Task<ApiResult<object>> ActualizarProductoAsync(int id, UpdateProductDto dto)
            => _api.PatchAsync<object>($"api/menu/productos/{id}", dto);

        public Task<ApiResult<object>> ToggleDisponibilidadAsync(int productoId)
            => _api.PatchAsync<object>($"api/menu/productos/{productoId}/disponibilidad");
    }
}
