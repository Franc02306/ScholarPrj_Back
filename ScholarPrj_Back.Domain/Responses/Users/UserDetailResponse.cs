using ScholarPrj_Back.Domain.Responses.Commons;

namespace ScholarPrj_Back.Domain.Responses.Users
{
    public class UserDetailResponse
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public UtilsResponse Role { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; } 
        public string Email { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
