using System.ComponentModel.DataAnnotations;

namespace TaskApi.Models
{
    public class Task
    {
        [Key]
        public Guid Id { get; set; }
        public string TaskName { get; set; }
        public string? Description { get; set; }
        public Guid ProjectId { get; set; }
        public Guid AssignedUserId { get; set; }
        public Guid AssignedByUserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? CompletedAt { get; set; }
        public TaskStatus Status { get; set; } = TaskStatus.NotStarted;
        public TaskPriority Priority { get; set; } = TaskPriority.Medium;
        public int? EstimatedMinutes { get; set; }
        public int? ActualMinutes { get; set; }

        public ICollection<TaskKeyword> Keywords { get; set; }
        public ICollection<TaskPerformanceRating> PerformanceRatings { get; set; }
    }

    public enum TaskStatus
    {
        NotStarted,
        InProgress,
        Completed,
        Cancelled
    }

    public enum TaskPriority
    {
        Low,
        Medium,
        High,
        Critical
    }
}