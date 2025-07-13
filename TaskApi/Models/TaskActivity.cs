using System.ComponentModel.DataAnnotations;

namespace TaskApi.Models
{
    public class TaskActivity
    {
        [Key]
        public Guid Id { get; set; }
        public Guid TaskId { get; set; }
        public Guid UserId { get; set; }
        public string ActivityType { get; set; } 
        public DateTime ActivityTime { get; set; }
        public string? Comments { get; set; }

        public Task Task { get; set; }
    }
}