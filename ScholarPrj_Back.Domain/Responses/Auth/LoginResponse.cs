using ScholarPrj_Back.Domain.Responses.Users;

namespace ScholarPrj_Back.Domain.Responses.Auth
{
    public class LoginResponse
    {
        public string Token { get; set; } = string.Empty;
        public UserDetailResponse UserDetail { get; set; }
    }
}
