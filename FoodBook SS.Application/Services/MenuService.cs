using FoodBook_SS.Application.Dtos.Menu;
using FoodBook_SS.Application.Interfaces;
using FoodBook_SS.Domain.Base;
using FoodBook_SS.Domain.Entities.Configuration;
using FoodBook_SS.Domain.Repository;

namespace FoodBook_SS.Application.Services
{
    public class MenuService : IMenuService
    {
        private readonly IMenuRepository _repo;
        private readonly IAuditService _audit;

        public MenuService(IMenuRepository repo, IAuditService audit) { _repo = repo; _audit = audit; }

        public Task<OperationResult> GetCategoriasByRestauranteAsync(int rid) =>
            _repo.GetCategoriasByRestauranteAsync(rid);

        public Task<OperationResult> GetProductosByRestauranteAsync(int rid) =>
            _repo.GetProductosByRestauranteAsync(rid);

        public Task<OperationResult> GetProductosByRestauranteAsync(int rid, bool soloDisponibles) =>
            _repo.GetProductosByRestauranteAsync(rid, soloDisponibles);

        public Task<OperationResult> GetProductosByCategoriaAsync(int cid) =>
            _repo.GetProductosByCategoriaAsync(cid);

        public async Task<OperationResult> SaveCategoriaAsync(SaveCategoryDto dto)
        {
            var cat = new CategoriaMenu
            {
                RestauranteId = dto.RestauranteId,
                Nombre = dto.Nombre,
                Descripcion = dto.Descripcion
            };
            return await _repo.SaveCategoriaAsync(cat);
        }

        public async Task<OperationResult> SaveProductoAsync(SaveProductDto dto)
        {
            var p = new ProductoMenu
            {
                CategoriaId = dto.CategoriaId,
                RestauranteId = dto.RestauranteId,
                Nombre = dto.Nombre,
                Descripcion = dto.Descripcion,
                Precio = dto.Precio,
                Imagen = dto.Imagen
            };
            return await _repo.SaveProductoAsync(p);
        }

        public async Task<OperationResult> UpdateProductoAsync(int id, UpdateProductDto dto)
        {
            var p = await _repo.GetProductoByIdAsync(id);
            if (p is null) return OperationResult.Fail("Producto no encontrado.");
            if (dto.Nombre is not null) p.Nombre = dto.Nombre;
            if (dto.Descripcion is not null) p.Descripcion = dto.Descripcion;
            if (dto.Precio.HasValue) p.Precio = dto.Precio.Value;
            if (dto.Imagen is not null) p.Imagen = dto.Imagen;
            return await _repo.UpdateProductoAsync(p);
        }

        public async Task<OperationResult> ToggleDisponibilidadAsync(int id, int actorId)
        {
            var p = await _repo.GetProductoByIdAsync(id);
            if (p is null) return OperationResult.Fail("Producto no encontrado.");
            var nuevoEstado = !p.Disponible;
            var result = await _repo.CambiarDisponibilidadAsync(id, nuevoEstado, actorId);
            if (!result.Success) return result;
            return OperationResult.Ok(
                new { productoId = id, disponible = nuevoEstado },
                nuevoEstado ? "Producto activado correctamente." : "Producto desactivado correctamente."
            );
        }
    }
}