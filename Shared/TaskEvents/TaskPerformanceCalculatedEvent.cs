using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.TaskEvents
{
    public class TaskPerformanceCalculatedEvent
    {
        public Guid TaskId { get; set; }
        public Guid UserId { get; set; }
        public double PerformanceScore { get; set; }
        public double TimeEfficiencyScore { get; set; }
        public int ActualMinutes { get; set; }
        public int AverageMinutesForSimilarTasks { get; set; }
        public DateTime CalculatedAt { get; set; }
    }
}
