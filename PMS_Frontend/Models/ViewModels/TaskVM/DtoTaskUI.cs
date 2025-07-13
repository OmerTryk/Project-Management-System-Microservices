using System.ComponentModel.DataAnnotations;
using PMS_Frontend.Models.Enums;

namespace PMS_Frontend.Models.ViewModels.TaskVM
{
    public class DtoTaskUI
    {
        [Required]
        public string TaskName { get; set; }

        public string? Description { get; set; }

        [Required]
        public string ProjectId { get; set; } // Dropdown'dan seçilecek

        [Required]
        public string AssignedUser { get; set; } // Kullanıcı adı

        public DateTime? DueDate { get; set; }

        public TaskPriorityEnum Priority { get; set; } = TaskPriorityEnum.Medium;

        public int? EstimatedMinutes { get; set; }

        public List<string> Keywords { get; set; } = new List<string>();
    }
}
