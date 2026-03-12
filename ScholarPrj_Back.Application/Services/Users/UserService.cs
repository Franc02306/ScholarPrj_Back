using ScholarPrj_Back.Application.Helpers;
using ScholarPrj_Back.Application.Services.Email;
using ScholarPrj_Back.Domain.Entities;
using ScholarPrj_Back.Domain.Requests.Users;
using ScholarPrj_Back.Domain.Responses.Common;
using ScholarPrj_Back.Domain.Responses.Commons;
using ScholarPrj_Back.Domain.Responses.Users;
using ScholarPrj_Back.Infrastructure.Repositories.Users;

namespace ScholarPrj_Back.Application.Services.Users
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IEmailService _emailService;

        public UserService(IUserRepository userRepository, IEmailService emailService)
        {
            _userRepository = userRepository;
            _emailService = emailService;
        }

        /// <summary>
        /// Crear al usuario en la web
        /// </summary>
        public async Task<ApiResponse<UserDetailResponse>> CreateUserAsync(CreateUserRequest request)
        {
            // 1. Validaciones antes de crear al usuario
            await UserValidationHelper.ValidateUsernameAsync(_userRepository, request.UserName);
            await UserValidationHelper.ValidateEmailAsync(_userRepository, request.Email);

            // 2. Generar una contraseña aleatoria
            var generatedPassword = PasswordHelper.GenerateRandom();

            // 3. Generar hash de la contraseña
            var passwordHash = PasswordHelper.Hash(generatedPassword);

            // 4. Construcción de la entidad
            var user = new User
            {
                UserName = request.UserName,
                Password = passwordHash,
                RoleId = request.RoleId,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Gender = request.Gender,
                Email = request.Email,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            // 5. Proceder a la creación del usuario en la base de datos
            var createdUser = await _userRepository.CreateUserAsync(user);

            // 6. Enviar correo al nuevo usuario con las credenciales generadas
            var isFemale = request.Gender?.ToUpper() == "F";

            await _emailService.SendUserCredentialAsync(request.Email, request.UserName, generatedPassword, isFemale);

            // 7. Retornar respuesta
            return ApiResponse<UserDetailResponse>.Ok(null, "Usuario creado correctamente");
        }

        /// <summary>
        /// Actualizar al usuario en la web
        /// </summary>
        public async Task<ApiResponse<UserDetailResponse>> UpdateUserAsync(int id, UpdateUserRequest request)
        {
            // 1. Obtener usuario existente
            var user = await _userRepository.GetUserByIdAsync(id);

            if (user == null)
                return ApiResponse<UserDetailResponse>.Fail("Usuario no encontrado");

            // 2. Validaciones (evita duplicados)
            UserValidationHelper.ValidateIsActive(user.IsActive);
            await UserValidationHelper.ValidateUsernameAsync(_userRepository, request.UserName, id);
            await UserValidationHelper.ValidateEmailAsync(_userRepository, request.Email, id);

            // 3. Actualizar campos del usuario
            user.UserName = request.UserName;
            user.RoleId = request.RoleId;
            user.FirstName = request.FirstName;
            user.LastName = request.LastName;
            user.Email = request.Email;
            user.UpdatedAt = DateTime.UtcNow;

            // 4. Guardar cambios
            await _userRepository.UpdateUserAsync(user);

            // 5. Respuesta
            return ApiResponse<UserDetailResponse>.Ok(null, "Usuario actualizado correctamente");
        }

        /// <summary>
        /// Alternar el estado activo del usuario (activar/desactivar)
        /// </summary>
        public async Task<ApiResponse<string>> ToggleActiveUserAsync(int id)
        {
            // 1. Obtener al usuario por ID
            var user = await _userRepository.GetUserByIdAsync(id);

            if (user == null)
                return ApiResponse<string>.Fail("Usuario no encontrado");

            // 2. Alternar el estado activo
            user.IsActive = !user.IsActive;
            user.UpdatedAt = DateTime.UtcNow;

            // 3. Guardar cambios
            await _userRepository.UpdateUserAsync(user);
            var status = user.IsActive ? "activado" : "desactivado";

            // 4. Retornar respuesta
            return ApiResponse<string>.Ok(null, $"Usuario {status} correctamente");
        }

        /// <summary>
        /// Eliminar al usuario una vez este inactivado
        /// </summary>
        public async Task<ApiResponse<string>> DeleteUserAsync(int id)
        {
            // 1. Obtener al usuario por ID
            var user = await _userRepository.GetUserByIdAsync(id);

            if (user == null)
                return ApiResponse<string>.Fail("Usuario no encontrado");

            // 2. Validar que este inactivo
            if (user.IsActive)
                return ApiResponse<string>.Fail("No se puede eliminar un usuario activo.");

            // 3. Eliminar usuario
            await _userRepository.DeleteUserAsync(user);

            // 4. Retornar respuesta
            return ApiResponse<string>.Ok(null, "Usuario eliminado correctamente");
        }

        /// <summary>
        /// Obtener datos del usuario por el ID
        /// </summary>
        public async Task<ApiResponse<UserDetailResponse>> GetUserByIdAsync(int id)
        {
            // 1. Obtener al usuario por ID
            var user = await _userRepository.GetUserByIdAsync(id);

            if (user == null)
                return ApiResponse<UserDetailResponse>.Fail("Usuario no encontrado");

            // 2. Mapear la entidad a la respuesta
            var response = new UserDetailResponse
            {
                Id = user.Id,
                UserName = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Gender = user.Gender,
                Email = user.Email,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt,
                Role = new UtilsResponse
                {
                    Id = user.Role.Id,
                    Name = user.Role.Name
                }
            };

            // 3. Retornar respuesta
            return ApiResponse<UserDetailResponse>.Ok(response);
        }

        /// <summary>
        /// Obtener lista de usuarios con filtros opcionales (género, nombre completo, correo electrónico, estado activo)
        /// </summary>
        public async Task<ApiResponse<List<UserListResponse>>> GetListUsersAsync(UserFilterRequest filters)
        {
            // 1. Consultar la lista de usuarios aplicando los filtros
            var users = await _userRepository.GetListUsersAsync(filters);

            // 2. Mapear la lista de entidades a la lista de respuestas
            var response = users.Select(u => new UserListResponse
            {
                Id = u.Id,
                UserName = u.UserName,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Email = u.Email,
                Gender = u.Gender,
                IsActive = u.IsActive,
                CreatedAt = u.CreatedAt,
                UpdatedAt = u.UpdatedAt,
                Role = new UtilsResponse
                {
                    Id = u.Role.Id,
                    Name = u.Role.Name
                }
            }).ToList();

            // 3. Retornar respuesta
            return ApiResponse<List<UserListResponse>>.Ok(response);
        }
    }
}
