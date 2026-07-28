using System.ComponentModel.DataAnnotations;

namespace DebugMeBackend.DTOs.EventLog
{
    public class CreateEventLogDto
    {
        [Required(ErrorMessage = "O usuário é obrigatório.")]
        public Guid UserId { get; set; }

        [Required(ErrorMessage = "A emoção é obrigatória.")]
        public Guid EmotionId { get; set; }

        [StringLength(500, ErrorMessage = "A descrição deve ter no máximo 500 caracteres.")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "A intensidade é obrigatória.")]
        [Range(1, 10, ErrorMessage = "A intensidade deve estar entre 1 e 10.")]
        public int Intensity { get; set; }

        [Required(ErrorMessage = "A data do evento é obrigatória.")]
        public DateTime EventDate { get; set; }
    }
}
