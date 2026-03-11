namespace ScholarPrj_Back.Domain.Requests.Users
{
    public class UserFilterRequest
    {
        public string? Gender { get; set; }         // M (Masculino), F (Femenino)
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public bool? IsActive { get; set; }
    }
}
