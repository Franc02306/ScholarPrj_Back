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


    }
}
