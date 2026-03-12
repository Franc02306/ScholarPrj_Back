using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using ScholarPrj_Back.Application.Services.Users;
using ScholarPrj_Back.Domain.Requests.Users;

namespace ScholarPrj_Back.Controllers
{
    [Authorize]
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

        /// <summary>
        /// Actualizar al usuario en la web
        /// </summary>
        [HttpPatch("update-user/{id:int}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserRequest request)
        {
            if (request == null)
                return BadRequest("Solicitud inválida");

            var result = await _userService.UpdateUserAsync(id, request);

            return Ok(result);
        }

        /// <summary>
        /// Alternar el estado activo del usuario (activar/desactivar)
        /// </summary>
        [HttpPatch("toggle-active/{id:int}")]
        public async Task<IActionResult> ToggleActiveUser(int id)
        {
            var result = await _userService.ToggleActiveUserAsync(id);
            return Ok(result);
        }

        #endregion

        #region API GET

        /// <summary>
        /// Obtener datos del usuario por el ID
        /// </summary>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);

            if (user == null)
                return NotFound();

            return Ok(user);
        }

        /// <summary>
        /// Obtener lista de usuarios con filtros opcionales (género, nombre completo, correo electrónico, estado activo)
        /// </summary>
        [HttpGet("list-users")]
        public async Task<IActionResult> GetListUsers([FromQuery] UserFilterRequest filters)
        {
            var result = await _userService.GetListUsersAsync(filters);

            return Ok(result);
        }

        #endregion

        #region API DELETE

        /// <summary>
        /// Eliminar al usuario una vez este inactivado
        /// </summary>
        [HttpDelete("delete-user/{id:int}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var result = await _userService.DeleteUserAsync(id);
            return Ok(result);
        }

        #endregion
    }
}
