using System.ComponentModel.DataAnnotations;

namespace TaskApi.Models
{
    public class SimilarTaskReference
    {
        [Key]
        public Guid Id { get; set; }
        public Guid PerformanceRatingId { get; set; }
        public Guid SimilarTaskId { get; set; }
        public double SimilarityPercentage { get; set; }
        public int SimilarTaskDurationMinutes { get; set; }

        public TaskPerformanceRating PerformanceRating { get; set; }
    }
}