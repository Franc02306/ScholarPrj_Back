namespace ScholarPrj_Back.Domain.Requests.Auth
{
    public class ChangePasswordRequest
    {
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
    }
}
