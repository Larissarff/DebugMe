using DebugMeBackend.DTOs.User;
using DebugMeBackend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DebugMeBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;

        public UserController(UserService userService)
        {
            _userService = userService;
        }

        [HttpPost("create")]
        [AllowAnonymous]
        public async Task<ActionResult<TokenResponseDto>> Create([FromBody] CreateUserDto dto)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            try
            {
                TokenResponseDto tokenResponse = await _userService.CreateAsync(dto);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = tokenResponse.User.Id },
                    tokenResponse
                );
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<TokenResponseDto>> Login([FromBody] LoginUserDto dto)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            TokenResponseDto? tokenResponse = await _userService.LoginAsync(dto);

            if (tokenResponse is null)
            {
                return Unauthorized(new { message = "E-mail ou senha inválidos." });
            }

            return Ok(tokenResponse);
        }

        [HttpPost("refresh")]
        [AllowAnonymous]
        public async Task<ActionResult<TokenResponseDto>> Refresh([FromBody] RefreshTokenRequestDto dto)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            TokenResponseDto? tokenResponse = await _userService.RefreshTokenAsync(dto.RefreshToken);

            if (tokenResponse is null)
            {
                return Unauthorized(new { message = "Refresh token inválido ou expirado." });
            }

            return Ok(tokenResponse);
        }

        [HttpGet("all")]
        public async Task<ActionResult<List<UserResponseDto>>> GetAll()
        {
            List<UserResponseDto> users = await _userService.GetAllAsync();
            return Ok(users);
        }

        [HttpGet("getById/{id:guid}")]
        public async Task<ActionResult<UserResponseDto>> GetById(Guid id)
        {
            UserResponseDto? user = await _userService.GetByIdAsync(id);

            if (user is null)
            {
                return NotFound(new { message = "Usuário não encontrado." });
            }

            return Ok(user);
        }

        [HttpPut("update/{id:guid}")]
        public async Task<ActionResult<UserResponseDto>> Update(Guid id, [FromBody] UpdateUserDto dto)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            try
            {
                UserResponseDto? updatedUser = await _userService.UpdateAsync(id, dto);

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
        public async Task<ActionResult> Delete(Guid id)
        {
            bool deleted = await _userService.DeleteAsync(id);

            if (!deleted)
            {
                return NotFound(new { message = "Usuário não encontrado." });
            }

            return NoContent();
        }
    }
}
