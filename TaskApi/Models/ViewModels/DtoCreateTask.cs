namespace TaskApi.Models.ViewModels
{
    public class DtoCreateTask
    {
        public string TaskName { get; set; }
        public string? Description { get; set; }
        public Guid ProjectId { get; set; }
        public Guid AssignedUserId { get; set; }
        public Guid AssignedByUserId { get; set; }
        public DateTime? DueDate { get; set; }
        public TaskPriority Priority { get; set; } = TaskPriority.Medium;
        public int? EstimatedMinutes { get; set; }
        public IEnumerable<string> Keywords { get; set; } = new List<string>();
    }
}