namespace ScholarPrj_Back.Domain.Requests.Users
{
    public class UpdateUserRequest
    {
        public string UserName { get; set; }
        public int RoleId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public string Email { get; set; }
    }
}
