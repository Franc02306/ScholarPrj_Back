using ScholarPrj_Back.Domain.Requests.Users;
using ScholarPrj_Back.Domain.Responses.Common;
using ScholarPrj_Back.Domain.Responses.Users;

namespace ScholarPrj_Back.Application.Services.Users
{
    public interface IUserService
    {
        /// <summary>
        /// Crear al usuario en la web
        /// </summary>
        Task<ApiResponse<UserDetailResponse>> CreateUserAsync(CreateUserRequest request);

        /// <summary>
        /// Actualizar al usuario en la web
        /// </summary>
        Task<ApiResponse<UserDetailResponse>> UpdateUserAsync(int id, UpdateUserRequest request);

        /// <summary>
        /// Obtener datos del usuario por el ID
        /// </summary>
        Task<ApiResponse<UserDetailResponse>> GetUserByIdAsync(int id);

        /// <summary>
        /// Obtener lista de usuarios con filtros opcionales (género, nombre completo, correo electrónico, estado activo)
        /// </summary>
        Task<ApiResponse<List<UserListResponse>>> GetListUsersAsync(UserFilterRequest filters);
    }
}
