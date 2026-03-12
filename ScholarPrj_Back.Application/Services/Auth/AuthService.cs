using ScholarPrj_Back.Application.Helpers;
using ScholarPrj_Back.Domain.Requests.Auth;
using ScholarPrj_Back.Domain.Responses.Auth;
using ScholarPrj_Back.Domain.Responses.Common;
using ScholarPrj_Back.Domain.Responses.Commons;
using ScholarPrj_Back.Domain.Responses.Users;
using ScholarPrj_Back.Infrastructure.Repositories.Users;

namespace ScholarPrj_Back.Application.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;

        public AuthService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
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
    }
}
