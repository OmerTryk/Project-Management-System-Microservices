using System.ComponentModel.DataAnnotations;

namespace TaskApi.Models
{
    public class TaskPerformanceRating
    {
        [Key]
        public Guid Id { get; set; }
        public Guid TaskId { get; set; }
        public Guid UserId { get; set; }
        public double PerformanceScore { get; set; } 
        public double SimilarityScore { get; set; }
        public double TimeEfficiencyScore { get; set; }
        public DateTime CalculatedAt { get; set; }

        public int ActualMinutes { get; set; }
        public int AverageMinutesForSimilarTasks { get; set; }
        public int SimilarTasksCount { get; set; }

        public Task Task { get; set; }
        public ICollection<SimilarTaskReference> SimilarTasks { get; set; }
    }
}