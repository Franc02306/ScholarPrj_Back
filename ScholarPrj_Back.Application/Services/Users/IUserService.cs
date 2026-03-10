using ScholarPrj_Back.Domain.Entities;
using ScholarPrj_Back.Domain.Requests.Users;

namespace ScholarPrj_Back.Application.Services.Users
{
    public interface IUserService
    {
        /// <summary>
        /// Crear al usuario en la web
        /// </summary>
        Task<User> CreateUserAsync(CreateUserRequest request);
    }
}
