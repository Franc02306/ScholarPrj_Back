namespace ScholarPrj_Back.Application.Services.Email
{
    public interface IEmailService
    {
        /// <summary>
        /// Envia un correo al nuevo usuario con las credenciales generadas por el sistema
        /// </summary>
        Task SendUserCredentialAsync(string email, string username, string password, bool isFemale);
    }
}
