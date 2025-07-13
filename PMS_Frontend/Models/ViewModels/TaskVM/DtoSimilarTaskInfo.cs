namespace PMS_Frontend.Models.ViewModels.TaskVM
{
    public class DtoSimilarTaskInfo
    {
        public Guid TaskId { get; set; }
        public string TaskName { get; set; }
        public double SimilarityPercentage { get; set; }
        public int DurationMinutes { get; set; }
        public DateTime CompletedAt { get; set; }
    }
}