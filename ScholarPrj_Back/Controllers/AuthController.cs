using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ScholarPrj_Back.Application.Services.Auth;
using ScholarPrj_Back.Domain.Requests.Auth;
using ScholarPrj_Back.Infrastructure.Configuration;

namespace ScholarPrj_Back.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly JwtSettings _jwtSettings;

        public AuthController(IAuthService authService, IOptions<JwtSettings> jwtSettings)
        {
            _authService = authService;
            _jwtSettings = jwtSettings.Value;
        }

        #region API POST

        /// <summary>
        /// Valida las credencales y determinar si el usuario puede iniciar sesión en la web
        /// </summary>
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var result = await _authService.LoginAsync(request);

            if (!result.Success || result.Data == null)
                return BadRequest(result);

            result.Data.Token = JwtHelper.GenerateToken(result.Data, _jwtSettings);

            return Ok(result);
        }

        /// <summary>
        /// Solicita recuperación de contraseña
        /// </summary>
        [AllowAnonymous]
        [HttpPost("forgot-passowrd")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            if (request == null)
                return BadRequest("Solicitud inválida");

            var result = await _authService.ForgotPasswordAsync(request);

            return Ok(result);
        }

        #endregion

        #region API PUT - PATCH

        /// <summary>
        /// Restablece la contraseña usando token de recuperación
        /// </summary>
        [AllowAnonymous]
        [HttpPatch("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            if (request == null)
                return BadRequest("Solicitud inválida");

            var result = await _authService.ResetPasswordAsync(request);

            return Ok(result);
        }

        /// <summary>
        /// Cambiar la contraseña del usuario con la sesión iniciada
        /// </summary>
        [HttpPatch("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            if (request == null)
                return BadRequest("Solicitud inválida");

            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrWhiteSpace(userIdClaim))
                return Unauthorized();

            var userId = int.Parse(userIdClaim);

            var result = await _authService.ChangePasswordAsync(userId, request);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        #endregion
    }
}
