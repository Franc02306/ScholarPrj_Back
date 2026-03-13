using ScholarPrj_Back.Domain.Entities;

namespace ScholarPrj_Back.Infrastructure.Repositories.Auth
{
    public interface IAuthRepository
    {
        /// <summary>
        /// Invalida todos los tokens activos de recuperación de contraseña asociados a un usuario específico
        /// </summary>
        Task InvalidateTokenByUserAsync(int userId);

        /// <summary>
        /// Registra un nuevo token de recuperación de contraseña en la base de datos
        /// </summary>
        Task InsertTokenAsync(PasswordReset reset);

        /// <summary>
        /// Actualizar la contraseña del usuario
        /// </summary>
        Task UpdatePaswordAsync(int userId, string hashedPassword);

        /// <summary>
        /// Obtener datos del token generado
        /// </summary>
        Task<PasswordReset?> GetByTokenAsync(string token);

        /// <summary>
        /// Marcar el token generado como usado
        /// </summary>
        Task MarkTokenAsUsedAsync(string token);
    }
}
