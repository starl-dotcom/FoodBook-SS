using FoodBook_SS.Application.Base;
using FoodBook_SS.Application.Dtos.User;
using FoodBook_SS.Application.Interfaces;
using FoodBook_SS.Domain.Base;
using FoodBook_SS.Domain.Entities.User;
using FoodBook_SS.Domain.Repository;

namespace FoodBook_SS.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repo;
        private readonly IPasswordHasher _hasher;
        private readonly IJwtTokenService _jwt;
        private readonly IAuditService _audit;

        public UserService(IUserRepository repo, IPasswordHasher hasher,
                           IJwtTokenService jwt, IAuditService audit)
        { _repo = repo; _hasher = hasher; _jwt = jwt; _audit = audit; }

        public async Task<OperationResult> GetAllAsync()
        {
            var lista = await _repo.GetAllAsync();
            return OperationResult.Ok(lista.Select(MapToDto));
        }

        public async Task<OperationResult> GetByIdAsync(int id)
        {
            var u = await _repo.GetEntityByIdAsync(id);
            return u is null ? OperationResult.Fail("Usuario no encontrado.") : OperationResult.Ok(MapToDto(u));
        }

        public async Task<OperationResult> SaveAsync(SaveUserDto dto)
        {
            if (await _repo.EmailExisteAsync(dto.Email))
                return OperationResult.Fail("El correo ya está registrado.");
            var u = new Usuario
            {
                RolId = dto.RolId,
                Nombre = dto.Nombre,
                Apellido = dto.Apellido,
                Email = dto.Email,
                PasswordHash = _hasher.Hash(dto.Password),
                Telefono = dto.Telefono
            };
            var r = await _repo.SaveEntityAsync(u);
            if (r.Success) await _audit.RegistrarAsync(null, "CREATE_USUARIO", "Usuario", u.Id.ToString(),
                                                        datosNuevos: new { u.Email, u.RolId });
            return r;
        }

        public async Task<OperationResult> UpdateAsync(int id, UpdateUserDto dto)
        {
            var u = await _repo.GetEntityByIdAsync(id);
            if (u is null) return OperationResult.Fail("Usuario no encontrado.");
            if (dto.Nombre is not null) u.Nombre = dto.Nombre;
            if (dto.Apellido is not null) u.Apellido = dto.Apellido;
            if (dto.Telefono is not null) u.Telefono = dto.Telefono;
            u.ActualizadoEn = DateTime.UtcNow;
            return await _repo.UpdateEntityAsync(u);
        }

        
        public async Task<OperationResult> UpdateAsync(int id, UpdateUserDto dto, int actorId)
        {
            var result = await UpdateAsync(id, dto);
            if (result.Success)
                await _audit.RegistrarAsync(actorId, "UPDATE_USUARIO", "Usuario", id.ToString());
            return result;
        }

        public async Task<OperationResult> LoginAsync(LoginDto dto)
        {
            var u = await _repo.GetByEmailAsync(dto.Email);
            if (u is null || !u.Activo || !_hasher.Verify(dto.Password, u.PasswordHash))
                return OperationResult.Fail("Credenciales incorrectas.");
            var (token, refresh, exp) = _jwt.Generate(u);
            await _repo.ActualizarRefreshTokenAsync(u.Id, refresh, exp);
            await _audit.RegistrarAsync(u.Id, "LOGIN", "Usuario", u.Id.ToString());
            return OperationResult.Ok(new AuthResponseDto { Token = token, RefreshToken = refresh, ExpiresAt = exp, Usuario = MapToDto(u) });
        }

        public async Task<OperationResult> RefreshTokenAsync(string refreshToken)
        {
            var todos = await _repo.GetAllAsync();
            var u = todos.FirstOrDefault(u => u.RefreshToken == refreshToken && u.RefreshTokenExp > DateTime.UtcNow);
            if (u is null) return OperationResult.Fail("Token inválido o expirado.");
            var (token, newRefresh, exp) = _jwt.Generate(u);
            await _repo.ActualizarRefreshTokenAsync(u.Id, newRefresh, exp);
            return OperationResult.Ok(new AuthResponseDto { Token = token, RefreshToken = newRefresh, ExpiresAt = exp, Usuario = MapToDto(u) });
        }

        public async Task<OperationResult> CambiarPasswordAsync(int id, string actual, string nueva)
        {
            var u = await _repo.GetEntityByIdAsync(id);
            if (u is null || !_hasher.Verify(actual, u.PasswordHash))
                return OperationResult.Fail("Contraseña actual incorrecta.");
            return await _repo.CambiarPasswordAsync(id, _hasher.Hash(nueva));
        }

        
        public Task<OperationResult> ChangePasswordAsync(int id, string actual, string nueva) =>
            CambiarPasswordAsync(id, actual, nueva);

        public async Task<OperationResult> ActivarDesactivarAsync(int id, bool activo, int actorId)
        {
            var r = await _repo.ActivarDesactivarAsync(id, activo, actorId);
            if (r.Success) await _audit.RegistrarAsync(actorId, activo ? "ACTIVAR_USUARIO" : "DESACTIVAR_USUARIO", "Usuario", id.ToString());
            return r;
        }

        private static UserDto MapToDto(Usuario u) => new()
        {
            Id             = u.Id,
            RolId          = u.RolId,
            NombreCompleto = $"{u.Nombre} {u.Apellido}",
            Email          = u.Email,
            Telefono       = u.Telefono,
            Rol            = u.Rol?.Nombre ?? string.Empty,
            Activo         = u.Activo
        };
    }
}
