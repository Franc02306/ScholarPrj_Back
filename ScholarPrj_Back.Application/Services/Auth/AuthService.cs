using ScholarPrj_Back.Application.Helpers;
using ScholarPrj_Back.Application.Services.Email;
using ScholarPrj_Back.Domain.Entities;
using ScholarPrj_Back.Domain.Requests.Auth;
using ScholarPrj_Back.Domain.Responses.Auth;
using ScholarPrj_Back.Domain.Responses.Common;
using ScholarPrj_Back.Domain.Responses.Commons;
using ScholarPrj_Back.Domain.Responses.Users;
using ScholarPrj_Back.Infrastructure.Repositories.Auth;
using ScholarPrj_Back.Infrastructure.Repositories.Users;

namespace ScholarPrj_Back.Application.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IAuthRepository _authRepository;
        private readonly IEmailService _emailService;

        public AuthService(IUserRepository userRepository, IAuthRepository authRepository, IEmailService emailService)
        {
            _userRepository = userRepository;
            _authRepository = authRepository;
            _emailService = emailService;
        }

        /// <summary>
        /// Valida las credencales y determinar si el usuario puede iniciar sesión en la web
        /// </summary>
        public async Task<ApiResponse<LoginResponse>> LoginAsync(LoginRequest request)
        {
            // 1. Validar credenciales
            var user = await _userRepository.GetUserByUsernameAsync(request.Username);

            if (user == null || !PasswordHelper.Verify(request.Password, user.Password))
                return ApiResponse<LoginResponse>.Fail("Credenciales inválidas");

            if (!user.IsActive)
                return ApiResponse<LoginResponse>.Fail("Tu usuario no está activo");

            // 2. Mapear usuario a LoginResponse
            var response = new LoginResponse
            {
                UserDetail = new UserDetailResponse
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
                }
            };

            return ApiResponse<LoginResponse>.Ok(response);
        }

        /// <summary>
        /// Solicita recuperación de contraseña
        /// </summary>
        public async Task<ApiResponse<object>> ForgotPasswordAsync(ForgotPasswordRequest request)
        {
            const string genericMessage = "Si el correo electrónico existe, se enviará un correo con instrucciones.";

            // 1. Validando envío de request
            if (string.IsNullOrWhiteSpace(request.Email))
                return ApiResponse<object>.Ok(null, genericMessage);

            // 2. Verificar si existe el usuario
            var exists = await _userRepository.ExistsUserByEmailAsync(request.Email);

            if (!exists)
                return ApiResponse<object>.Ok(null, genericMessage);

            // 3. Obtener el usuario real
            var user = await _userRepository.GetUserByEmailAsync(request.Email);

            if (user == null)
                return ApiResponse<object>.Ok(null, genericMessage);

            // 4. Invalidar tokens anteriores
            await _authRepository.InvalidateTokenByUserAsync(user.Id);

            // 5. Generar token seguro
            var token = Convert.ToBase64String(System.Security.Cryptography.RandomNumberGenerator.GetBytes(64));

            // 6. Crear entidad
            var reset = new PasswordReset
            {
                UserId = user.Id,
                Token = token,
                ExpiresAt = DateTime.UtcNow.AddMinutes(15),
                IsUsed = false,
                CreatedAt = DateTime.UtcNow
            };

            // 7. Guardar token
            await _authRepository.InsertTokenAsync(reset);

            // 8. Envio de correo
            await _emailService.SendPasswordResetAsync(request.Email, token);

            return ApiResponse<object>.Ok(null, genericMessage);
        }

        /// <summary>
        /// Restablece la contraseña usando token de recuperación
        /// </summary>
        public async Task<ApiResponse<object>> ResetPasswordAsync(ResetPasswordRequest request)
        {
            // 1. Validación básica y de nivel de seguridad de contraseña
            if (string.IsNullOrWhiteSpace(request.Token) || string.IsNullOrWhiteSpace(request.NewPassword))
                return ApiResponse<object>.Fail("Solicitud inválida.");

            if (!PasswordHelper.IsStrongPassword(request.NewPassword))
                return ApiResponse<object>.Fail("La contraseña no cumple con la política de seguridad.");

            // 2. Buscar token en BD
            var tokenEntity = await _authRepository.GetByTokenAsync(request.Token);

            if (tokenEntity == null)
                return ApiResponse<object>.Fail("El enlace no es válido o ha expirado.");

            // 3. Verificar si ya fue usado
            if (tokenEntity.IsUsed)
                return ApiResponse<object>.Fail("El enlace ya fue utilizado.");

            // 4. Verificar expiración (15 minutos según el correo enviado)
            if (tokenEntity.ExpiresAt < DateTime.UtcNow)
                return ApiResponse<object>.Fail("El enlace ha expirado.");

            // 5. Obtener usuario
            var user = await _userRepository.GetUserByIdAsync(tokenEntity.UserId);

            if (user == null)
                return ApiResponse<object>.Fail("No se pudo procesar la solicitud.");

            // 6. Hashear nueva contraseña
            var hashedPassword = PasswordHelper.Hash(request.NewPassword);

            // 7. Actualizar contraseña del usuario
            await _authRepository.UpdatePaswordAsync(user.Id, hashedPassword);

            // 8. Marcar el token como usado
            await _authRepository.MarkTokenAsUsedAsync(request.Token);

            // 9. Retornar respuesta
            return ApiResponse<object>.Ok(null, "La contraseña fue actualizada correctamente.");
        }

        /// <summary>
        /// Cambiar la contraseña del usuario con la sesión iniciada
        /// </summary>

        public async Task<ApiResponse<object>> ChangePasswordAsync(int userId, ChangePasswordRequest request)
        {
            // 1. Validación básica y de nivel de seguridad de contraseña
            if (string.IsNullOrWhiteSpace(request.CurrentPassword) || string.IsNullOrWhiteSpace(request.NewPassword))
                return ApiResponse<object>.Fail("Solicitud inválida.");

            if (!PasswordHelper.IsStrongPassword(request.NewPassword))
                return ApiResponse<object>.Fail("La contraseña no cumple con la política de seguridad.");

            // 2. Obtener usuario
            var user = await _userRepository.GetUserByIdAsync(userId);

            if (user == null)
                return ApiResponse<object>.Fail("Usuario no encontrado.");

            // 3. Validar contraseña actual y que no sea igual a la anterior
            if (!PasswordHelper.Verify(request.CurrentPassword, user.Password))
                return ApiResponse<object>.Fail("La contraseña actual es incorrecta.");

            if (PasswordHelper.Verify(request.NewPassword, user.Password))
                return ApiResponse<object>.Fail("La nueva contraseña no puede ser igual a la actual.");

            // 4. Hashear contraseña nueva y retornar respuesta
            var hashed = PasswordHelper.Hash(request.NewPassword);

            await _authRepository.UpdatePaswordAsync(userId, hashed);

            return ApiResponse<object>.Ok(null, "La contraseña fue actualizada correctamente.");
        }
    }
}
