using ScholarPrj_Back.Domain.Requests.Auth;
using ScholarPrj_Back.Domain.Responses.Auth;
using ScholarPrj_Back.Domain.Responses.Common;

namespace ScholarPrj_Back.Application.Services.Auth
{
    public interface IAuthService
    {
        /// <summary>
        /// Valida las credencales y determinar si el usuario puede iniciar sesión en la web
        /// </summary>
        Task<ApiResponse<LoginResponse>> LoginAsync(LoginRequest request);

        /// <summary>
        /// Solicita recuperación de contraseña
        /// </summary>
        Task<ApiResponse<object>> ForgotPasswordAsync(ForgotPasswordRequest request);

        /// <summary>
        /// Restablece la contraseña usando token de recuperación
        /// </summary>
        Task<ApiResponse<object>> ResetPasswordAsync(ResetPasswordRequest request);

        /// <summary>
        /// Cambiar la contraseña del usuario con la sesión iniciada
        /// </summary>
       Task<ApiResponse<object>> ChangePasswordAsync(int userId, ChangePasswordRequest request);
    }
}
