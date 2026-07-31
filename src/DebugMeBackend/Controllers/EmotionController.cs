using System.Security.Claims;
using DebugMeBackend.DTOs.Emotion;
using DebugMeBackend.Entities;
using DebugMeBackend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DebugMeBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class EmotionController : ControllerBase
    {
        private readonly EmotionService _emotionService;

        public EmotionController(EmotionService emotionService)
        {
            _emotionService = emotionService;
        }

        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<Emotion>>> GetAll()
        {
            Guid userId = GetUserIdFromToken();
            IEnumerable<Emotion> emotions = await _emotionService.GetByUserIdAsync(userId);
            return Ok(emotions);
        }

        [HttpGet("all-with-count")]
        public async Task<ActionResult<IEnumerable<EmotionWithCountDto>>> GetAllWithCount()
        {
            Guid userId = GetUserIdFromToken();
            IEnumerable<EmotionWithCountDto> dtos = await _emotionService.GetAllWithUserEventCountAsync(userId);
            return Ok(dtos);
        }

        [HttpGet("id/{id:guid}")]
        public async Task<ActionResult<Emotion>> GetById(Guid id)
        {
            Emotion? emotion = await _emotionService.GetByIdAsync(id);

            if (emotion is null)
            {
                return NotFound(new { message = "Emoção não encontrada." });
            }

            return Ok(emotion);
        }

        [HttpGet("name/{name}")]
        public async Task<ActionResult<Emotion>> GetByName(string name)
        {
            Emotion? emotion = await _emotionService.GetByNameAsync(name);

            if (emotion is null)
            {
                return NotFound(new { message = "Emoção não encontrada." });
            }

            return Ok(emotion);
        }

        [HttpPost("create")]
        public async Task<ActionResult<Emotion>> Create([FromBody] Emotion emotion)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            try
            {
                Guid userId = GetUserIdFromToken();
                Emotion createdEmotion = await _emotionService.CreateAsync(emotion, userId);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = createdEmotion.Id },
                    createdEmotion
                );
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("update/{id:guid}")]
        public async Task<ActionResult<Emotion>> Update(Guid id, [FromBody] Emotion emotion)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            try
            {
                Emotion? updatedEmotion = await _emotionService.UpdateAsync(id, emotion);

                if (updatedEmotion is null)
                {
                    return NotFound(new { message = "Emoção não encontrada." });
                }

                return Ok(updatedEmotion);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("delete/{id:guid}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            await _emotionService.DeleteAsync(id);
            return NoContent();
        }

        private Guid GetUserIdFromToken()
        {
            string? sub = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? User.FindFirst("sub")?.Value;

            if (sub is null || !Guid.TryParse(sub, out Guid userId))
                throw new UnauthorizedAccessException("Token inválido: userId não encontrado.");

            return userId;
        }

    }
}