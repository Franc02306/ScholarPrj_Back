using ScholarPrj_Back.Infrastructure.Repositories.Users;

namespace ScholarPrj_Back.Application.Helpers
{
    public class UserValidationHelper
    {
        /// <summary>
        /// Valida si el username ya existe en otro usuario
        /// </summary>
        public static async Task ValidateUsernameAsync(IUserRepository repository, string username, int? currentUserId = null)
        {
            var exists = await repository.ExistsUserByUsernameAsync(username, currentUserId);

            if (exists)
                throw new ArgumentException($"El nombre de usuario '{username}' ya está en uso.");
        }

        /// <summary>
        /// Valida si el correo electrónico ya existe en otro usuario
        /// </summary>
        public static async Task ValidateEmailAsync(IUserRepository repository, string email, int? currentUserId = null)
        {
            var exists = await repository.ExistsUserByEmailAsync(email);

            if (!exists)
                return;

            if (currentUserId.HasValue)
            {
                throw new ArgumentException($"El correo '{email}' ya está en uso por otro usuario.");
            }

            throw new ArgumentException($"El correo '{email}' ya está en uso.");
        }

        /// <summary>
        /// Valida si el usuario esta activo
        /// </summary>
        public static void ValidateIsActive(bool isActive)
        {
            if (!isActive)
                throw new ArgumentException("No se puede actualizar un usuario desactivado.");
        }
    }
}
