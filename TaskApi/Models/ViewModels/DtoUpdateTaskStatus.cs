namespace TaskApi.Models.ViewModels
{
    public class DtoUpdateTaskStatus
    {
        public Guid TaskId { get; set; }
        public TaskStatus Status { get; set; }
        public Guid UserId { get; set; }
        public string? Comments { get; set; }
        public DateTime? CompletedAt { get; set; }
        public int? ActualMinutes { get; set; }
    }
}