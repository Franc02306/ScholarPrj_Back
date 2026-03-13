using System.ComponentModel.DataAnnotations;

namespace ScholarPrj_Back.Domain.Requests.Auth
{
    public class ForgotPasswordRequest
    {
        [Required(ErrorMessage = "El correo es obligatorio.")]
        [EmailAddress(ErrorMessage = "El correo no tiene un formato válido.")]
        public string Email { get; set; }
    }
}
