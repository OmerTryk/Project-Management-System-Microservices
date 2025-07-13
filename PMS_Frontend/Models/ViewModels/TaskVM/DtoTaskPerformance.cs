namespace PMS_Frontend.Models.ViewModels.TaskVM
{
    public class DtoTaskPerformance
    {
        public Guid TaskId { get; set; }
        public string TaskName { get; set; }
        public double PerformanceScore { get; set; }
        public double TimeEfficiencyScore { get; set; }
        public int ActualMinutes { get; set; }
        public int AverageMinutesForSimilarTasks { get; set; }
        public List<DtoSimilarTaskInfo> SimilarTasks { get; set; }
    }
}