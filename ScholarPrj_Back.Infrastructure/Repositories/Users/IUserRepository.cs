using ScholarPrj_Back.Domain.Entities;
using ScholarPrj_Back.Domain.Requests.Users;

namespace ScholarPrj_Back.Infrastructure.Repositories.Users
{
    public interface IUserRepository
    {
        /// <summary>
        /// Crear al usuario en la web
        /// </summary>
        Task<User> CreateUserAsync(User user);

        /// <summary>
        /// Actualizar al usuario en la web
        /// </summary>
        Task<User> UpdateUserAsync(User user);

        /// <summary>
        /// Eliminar al usuario una vez este inactivado
        /// </summary>
        Task DeleteUserAsync(User user);

        /// <summary>
        /// Obtener datos del usuario por el ID
        /// </summary>
        Task<User?> GetUserByIdAsync(int id);

        /// <summary>
        /// Obtener los datos de un usuario mediante el correo electrónico
        /// </summary>
        Task<User?> GetUserByEmailAsync(string email);

        /// <summary>
        /// Obtener lista de usuarios con filtros opcionales (género, nombre completo, correo electrónico, estado activo)
        /// </summary>
        Task<List<User>> GetListUsersAsync(UserFilterRequest filters);

        /// <summary>
        /// Verificar si existe un usuario con el mismo email
        /// </summary>
        Task<bool> ExistsUserByEmailAsync(string email);

        /// <summary>
        /// Verificar si existe un usuario con el mismo username
        /// </summary>
        Task<bool> ExistsUserByUsernameAsync(string username, int? excludeUserId = null);

        /// <summary>
        /// Obtener datos del usuario con su username
        /// </summary>
        Task<User?> GetUserByUsernameAsync(string username);
    }
}
