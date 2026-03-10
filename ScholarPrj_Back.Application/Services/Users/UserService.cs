using ScholarPrj_Back.Application.Helpers;
using ScholarPrj_Back.Domain.Entities;
using ScholarPrj_Back.Domain.Requests.Users;
using ScholarPrj_Back.Infrastructure.Repositories.Users;

namespace ScholarPrj_Back.Application.Services.Users
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }


        /// <summary>
        /// Crear al usuario en la web
        /// </summary>
        public async Task<User> CreateUserAsync(CreateUserRequest request)
        {
            // 1. Generar una contraseña aleatoria
            var generatedPassword = PasswordHelper.GenerateRandom();

            // 2. Generar hash de la contraseña
            var passwordHash = PasswordHelper.Hash(generatedPassword);

            // 3. Construcción de la entidad
            var user = new User
            {
                UserName = request.UserName,
                Password = passwordHash,
                RoleId = request.RoleId,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Gender = request.Gender,
                Email = request.Email,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            // 4. Proceder a la creación del usuario en la base de datos
            return await _userRepository.CreateUserAsync(user);
        }
    }
}
