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

        #endregion
    }
}
