namespace TaskApi.Models.ViewModels
{
    public class DtoTaskPerformanceResult
    {
        public Guid TaskId { get; set; }
        public string TaskName { get; set; }
        public double PerformanceScore { get; set; }
        public double TimeEfficiencyScore { get; set; }
        public int ActualMinutes { get; set; }
        public int AverageMinutesForSimilarTasks { get; set; }
        public IEnumerable<DtoSimilarTask> SimilarTasks { get; set; }
    }
}