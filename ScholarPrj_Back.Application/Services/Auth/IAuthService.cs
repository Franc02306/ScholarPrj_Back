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
    }
}
