using Microsoft.EntityFrameworkCore;
using ScholarPrj_Back.Domain.Entities;
using ScholarPrj_Back.Domain.Requests.Users;
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

        /// <summary>
        /// Actualizar al usuario en la web
        /// </summary>
        public async Task<User> UpdateUserAsync(User user)
        {
            await _context.SaveChangesAsync();
            return user;
        }

        /// <summary>
        /// Eliminar al usuario una vez este inactivado
        /// </summary>
        public async Task DeleteUserAsync(User user)
        {
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Obtener datos del usuario por el ID
        /// </summary>
        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        /// <summary>
        /// Obtener lista de usuarios con filtros opcionales (género, nombre completo, correo electrónico, estado activo)
        /// </summary>
        public async Task<List<User>> GetListUsersAsync(UserFilterRequest filters)
        {
            var query = _context.Users.Include(u => u.Role).AsQueryable();

            if (!string.IsNullOrWhiteSpace(filters.Gender))
                query = query.Where(u => u.Gender == filters.Gender);

            if (!string.IsNullOrWhiteSpace(filters.Email))
                query = query.Where(u => u.Email.Contains(filters.Email));

            if (filters.IsActive.HasValue)
                query = query.Where(u => u.IsActive == filters.IsActive);

            if (!string.IsNullOrWhiteSpace(filters.FullName))
            {
                var name = filters.FullName.ToLower();

                query = query.Where(u =>
                    (u.FirstName + " " + u.LastName).ToLower().Contains(name));
            }

            return await query.ToListAsync();
        }

        /// <summary>
        /// Verificar si existe un usuario con el mismo email
        /// </summary>
        public async Task<bool> ExistsUserByEmailAsync(string email)
        {
            return await _context.Users
                .AnyAsync(u => u.Email == email);
        }

        /// <summary>
        /// Verificar si existe un usuario con el mismo username
        /// </summary>
        public async Task<bool> ExistsUserByUsernameAsync(string username, int? excludeUserId = null)
        {
            return await _context.Users
                .AnyAsync(u =>
                    u.UserName == username &&
                    (!excludeUserId.HasValue || u.Id != excludeUserId));
        }

        /// <summary>
        /// Obtener datos del usuario con su username
        /// </summary>
        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            return await _context.Users
                .Include(u => u.Role).FirstOrDefaultAsync(u => u.UserName == username);
        }
    }
}
