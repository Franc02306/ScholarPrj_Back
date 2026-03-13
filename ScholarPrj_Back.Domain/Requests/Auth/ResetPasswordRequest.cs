namespace ScholarPrj_Back.Domain.Requests.Auth
{
    public class ResetPasswordRequest
    {
        public string Token { get; set; }
        public string NewPassword { get; set; }
    }
}
