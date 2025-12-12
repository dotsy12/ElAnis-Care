using System.ComponentModel.DataAnnotations;

namespace ElAnis.Entities.DTO.Review
{
    public class CreateReviewDto
    {
        [Required(ErrorMessage = "Service request ID is required")]
        public Guid ServiceRequestId { get; set; }

        [Required(ErrorMessage = "Rating is required")]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
        public int Rating { get; set; }

        [MaxLength(1000, ErrorMessage = "Comment cannot exceed 1000 characters")]
        public string? Comment { get; set; }
    }
}