using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.TaskEvents
{
    public class TaskStartedEvent
    {
        public Guid TaskId { get; set; }
        public Guid ProjectId { get; set; }
        public Guid UserId { get; set; }
        public DateTime StartedAt { get; set; }
        public string TaskName { get; set; }
        public IEnumerable<string> Keywords { get; set; }
    }
}
