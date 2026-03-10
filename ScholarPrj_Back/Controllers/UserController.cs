using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ScholarPrj_Back.Application.Services.Users;
using ScholarPrj_Back.Domain.Requests.Users;

namespace ScholarPrj_Back.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        #region API POST

        /// <summary>
        /// Crear al usuario en la web
        /// </summary>
        [AllowAnonymous]
        [HttpPost("create-user")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
        {
            if (request == null)
                return BadRequest("Solicitud inválida");

            var result = await _userService.CreateUserAsync(request);

            return Ok(result);
        }

        #endregion

        #region API PUT - PATCH

        #endregion

        #region API GET

        #endregion
    }
}
