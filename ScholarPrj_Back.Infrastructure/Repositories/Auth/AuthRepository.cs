using Microsoft.EntityFrameworkCore;
using ScholarPrj_Back.Domain.Entities;
using ScholarPrj_Back.Infrastructure.Data;

namespace ScholarPrj_Back.Infrastructure.Repositories.Auth
{
    public class AuthRepository : IAuthRepository
    {
        private readonly AppDbContext _context;

        public AuthRepository(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Invalida todos los tokens activos de recuperación de contraseña asociados a un usuario específico
        /// </summary>
        public async Task InvalidateTokenByUserAsync(int userId)
        {
            var tokens = await _context.PasswordResets.Where(x => x.UserId == userId && !x.IsUsed)
                .ToListAsync();

            foreach (var token in tokens)
            {
                token.IsUsed = true;
                token.UsedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Registra un nuevo token de recuperación de contraseña en la base de datos
        /// </summary>
        public async Task InsertTokenAsync(PasswordReset reset)
        {
            await _context.PasswordResets.AddAsync(reset);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Actualizar la contraseña del usuario
        /// </summary>
        public async Task UpdatePaswordAsync(int userId, string hashedPassword)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                return;

            user.Password = hashedPassword;
            user.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Obtener datos del token generado
        /// </summary>
        public async Task<PasswordReset?> GetByTokenAsync(string token)
        {
            return await _context.PasswordResets.FirstOrDefaultAsync(x => x.Token == token);
        }

        /// <summary>
        /// Marcar el token generado como usado
        /// </summary>
        public async Task MarkTokenAsUsedAsync(string token)
        {
            var reset = await _context.PasswordResets.FirstOrDefaultAsync(x => x.Token == token && !x.IsUsed);

            if (reset == null)
                return;

            reset.IsUsed = true;
            reset.UsedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }
    }
}
