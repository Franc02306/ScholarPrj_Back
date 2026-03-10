using ScholarPrj_Back.Domain.Responses.Commons;

namespace ScholarPrj_Back.Domain.Responses.Common
{
    public class LoginResponse
    {
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public UtilsResponse Role { get; set; } = new UtilsResponse();
        public string Token { get; set; } = string.Empty;
    }
}
