using System.ComponentModel.DataAnnotations;

namespace TaskApi.Models
{
    public class TaskKeyword
    {
        [Key]
        public Guid Id { get; set; }
        public Guid TaskId { get; set; }
        public string Keyword { get; set; } 
        public double Weight { get; set; } = 1.0;

        public Task Task { get; set; }
    }
}