using ScholarPrj_Back.Application.Helpers;
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

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
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

            // 6. Retornar respuesta
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
    }
}
