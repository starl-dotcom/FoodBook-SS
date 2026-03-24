using FoodBook_SS.Application.Base;
using FoodBook_SS.Application.Dtos.Restaurant;
using FoodBook_SS.Application.Interfaces;
using FoodBook_SS.Domain.Base;
using FoodBook_SS.Domain.Entities.Configuration;
using FoodBook_SS.Domain.Repository;

namespace FoodBook_SS.Application.Services
{
    public class RestaurantService : IRestaurantService
    {
        private readonly IRestaurantRepository _repo;

        public RestaurantService(IRestaurantRepository repo) => _repo = repo;

        public Task<OperationResult> GetAllAsync() => _repo.GetAllAsync(r => r.Activo);
        public Task<OperationResult> GetByPropietarioAsync(int id) => _repo.GetByPropietarioAsync(id);
        public Task<OperationResult> SearchAsync(string? nombre, string? ciudad, string? tipoCocina) =>
            _repo.SearchAsync(nombre, ciudad, tipoCocina);

        public Task<OperationResult> BuscarAsync(string? ciudad, string? tipoCocina, string? termino) =>
            _repo.SearchAsync(termino, ciudad, tipoCocina);

        public async Task<OperationResult> GetByIdAsync(int id)
        {
            var r = await _repo.GetEntityByIdAsync(id);
            return r is null ? OperationResult.Fail("Restaurante no encontrado.") : OperationResult.Ok(r);
        }

        public async Task<OperationResult> SaveAsync(SaveRestaurantDto dto)
        {
            var r = new Restaurante
            {
                Nombre = dto.Nombre,
                Descripcion = dto.Descripcion,
                TipoCocina = dto.TipoCocina,
                Direccion = dto.Direccion,
                Ciudad = dto.Ciudad,
                Telefono = dto.Telefono,
                RangoPrecio = dto.RangoPrecio
            };
            return await _repo.SaveEntityAsync(r);
        }

        public async Task<OperationResult> CreateAsync(SaveRestaurantDto dto, int propietarioId)
        {
            var r = new Restaurante
            {
                PropietarioId = propietarioId,
                Nombre = dto.Nombre,
                Descripcion = dto.Descripcion,
                TipoCocina = dto.TipoCocina,
                Direccion = dto.Direccion,
                Ciudad = dto.Ciudad,
                Telefono = dto.Telefono,
                RangoPrecio = dto.RangoPrecio
            };
            return await _repo.SaveEntityAsync(r);
        }

        public async Task<OperationResult> UpdateAsync(int id, UpdateRestaurantDto dto)
        {
            var r = await _repo.GetEntityByIdAsync(id);
            if (r is null) return OperationResult.Fail("Restaurante no encontrado.");
            if (dto.Descripcion is not null) r.Descripcion = dto.Descripcion;
            if (dto.Telefono is not null) r.Telefono = dto.Telefono;
            if (dto.RangoPrecio is not null) r.RangoPrecio = dto.RangoPrecio;
            return await _repo.UpdateEntityAsync(r);
        }
    }
}