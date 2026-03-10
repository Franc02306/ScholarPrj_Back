using ScholarPrj_Back.Domain.Entities;

namespace ScholarPrj_Back.Infrastructure.Repositories.Users
{
    public interface IUserRepository
    {
        /// <summary>
        /// Crear al usuario en la web
        /// </summary>
        Task<User> CreateUserAsync(User user);
    }
}
