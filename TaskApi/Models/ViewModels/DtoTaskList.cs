namespace TaskApi.Models.ViewModels
{
    public class DtoTaskList
    {
        public Guid TaskId { get; set; }
        public string TaskName { get; set; }
        public string Status { get; set; } 
        public string Priority { get; set; }
        public DateTime? DueDate { get; set; }
        public string AssignedUserName { get; set; }
        public Guid AssignedUserId { get; set; }
        public int? EstimatedMinutes { get; set; }
        public IEnumerable<string> Keywords { get; set; }
    }
}