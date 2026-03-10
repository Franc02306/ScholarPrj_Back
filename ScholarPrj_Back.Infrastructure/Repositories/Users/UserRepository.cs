using ScholarPrj_Back.Domain.Entities;
using ScholarPrj_Back.Infrastructure.Data;

namespace ScholarPrj_Back.Infrastructure.Repositories.Users
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;
        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Crear al usuario en la web
        /// </summary>
        public async Task<User> CreateUserAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }
    }
}
