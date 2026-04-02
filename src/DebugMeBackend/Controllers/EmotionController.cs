using DebugMeBackend.Entities;
using DebugMeBackend.Services;
using Microsoft.AspNetCore.Mvc;

namespace DebugMeBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
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
            IEnumerable<Emotion> emotions = await _emotionService.GetAllAsync();
            return Ok(emotions);
        }

        [HttpGet("id/{id:guid}")]
        public async Task<ActionResult<Emotion>> GetById(Guid id)
        {
            Emotion? emotion = await _emotionService.GetByIdAsync(id);
            return Ok(emotion);
        }

        [HttpGet("name/{name}")]
        public async Task<ActionResult<Emotion>> GetByName(string name)
        {
            Emotion? emotion = await _emotionService.GetByNameAsync(name);
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
                Emotion createdEmotion = await _emotionService.CreateAsync(emotion);

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

    }
}