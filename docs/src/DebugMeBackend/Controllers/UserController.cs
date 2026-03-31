using DebugMeBackend.DTOs.User;
using DebugMeBackend.Services;
using Microsoft.AspNetCore.Mvc;

namespace DebugMeBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;

        public UserController(UserService userService)
        {
            _userService = userService;
        }

        [HttpPost("create")]
        public ActionResult<UserResponseDto> Create([FromBody] CreateUserDto dto)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            try
            {
                UserResponseDto createdUser = _userService.Create(dto);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = createdUser.Id },
                    createdUser
                );
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("login")]
        public ActionResult Login([FromBody] LoginUserDto dto)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            bool success = _userService.Login(dto);

            if (!success)
            {
                return Unauthorized(new { message = "E-mail ou senha inválidos." });
            }

            return Ok(new { message = "Login realizado com sucesso." });
        }

        [HttpGet("all")]
        public ActionResult<List<UserResponseDto>> GetAll()
        {
            List<UserResponseDto> users = _userService.GetAll();
            return Ok(users);
        }

        [HttpGet("getById/{id:guid}")]
        public ActionResult<UserResponseDto> GetById(Guid id)
        {
            UserResponseDto? user = _userService.GetById(id);

            if (user is null)
            {
                return NotFound(new { message = "Usuário não encontrado." });
            }

            return Ok(user);
        }

        [HttpPut("update/{id:guid}")]
        public ActionResult<UserResponseDto> Update(Guid id, [FromBody] UpdateUserDto dto)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            try
            {
                UserResponseDto? updatedUser = _userService.Update(id, dto);

                if (updatedUser is null)
                {
                    return NotFound(new { message = "Usuário não encontrado." });
                }

                return Ok(updatedUser);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id:guid}")]
        public ActionResult Delete(Guid id)
        {
            bool deleted = _userService.Delete(id);

            if (!deleted)
            {
                return NotFound(new { message = "Usuário não encontrado." });
            }

            return NoContent();
        }
    }
}