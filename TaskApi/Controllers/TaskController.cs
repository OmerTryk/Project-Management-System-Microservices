using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared.Settings;
using Shared.TaskEvents;
using TaskApi.Context;
using TaskApi.Models;
using TaskApi.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskStatus = TaskApi.Models.TaskStatus;
using TaskPriority = TaskApi.Models.TaskPriority;

namespace TaskApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskController : ControllerBase
    {
        private readonly TaskDbContext _context;
        private readonly ISendEndpointProvider _sendEndpointProvider;

        public TaskController(TaskDbContext context, ISendEndpointProvider sendEndpointProvider)
        {
            _context = context;
            _sendEndpointProvider = sendEndpointProvider;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateTask(DtoCreateTask dto)
        {
            var task = new Models.Task
            {
                Id = Guid.NewGuid(),
                TaskName = dto.TaskName,
                Description = dto.Description,
                ProjectId = dto.ProjectId,
                AssignedUserId = dto.AssignedUserId,
                AssignedByUserId = dto.AssignedByUserId,
                CreatedAt = DateTime.UtcNow,
                DueDate = dto.DueDate,
                Priority = dto.Priority,
                EstimatedMinutes = dto.EstimatedMinutes,
                Status = TaskStatus.NotStarted
            };

            // Anahtar kelimeleri oluştur
            var keywords = new List<TaskKeyword>();
            foreach (var keyword in dto.Keywords)
            {
                keywords.Add(new TaskKeyword
                {
                    Id = Guid.NewGuid(),
                    TaskId = task.Id,
                    Keyword = keyword.ToLower().Trim() // Normalleştir
                });
            }

            task.Keywords = keywords;

            await _context.Tasks.AddAsync(task);
            await _context.TaskKeywords.AddRangeAsync(keywords);
            await _context.SaveChangesAsync();

            return Ok(new { TaskId = task.Id });
        }

        [HttpPost("start")]
        public async Task<IActionResult> StartTask(DtoUpdateTaskStatus dto)
        {
            var task = await _context.Tasks
                .Include(t => t.Keywords)
                .FirstOrDefaultAsync(t => t.Id == dto.TaskId);

            if (task == null)
                return NotFound();

            if (task.Status != TaskStatus.NotStarted)
                return BadRequest("Task already started");

            task.Status = TaskStatus.InProgress;
            task.StartDate = DateTime.UtcNow;

            // Activity log
            var activity = new TaskActivity
            {
                Id = Guid.NewGuid(),
                TaskId = task.Id,
                UserId = dto.UserId,
                ActivityType = "Started",
                ActivityTime = DateTime.UtcNow,
                Comments = dto.Comments
            };

            await _context.TaskActivities.AddAsync(activity);
            await _context.SaveChangesAsync();

            // Event publish
            var taskStartedEvent = new TaskStartedEvent
            {
                TaskId = task.Id,
                ProjectId = task.ProjectId,
                UserId = dto.UserId,
                StartedAt = task.StartDate.Value,
                TaskName = task.TaskName,
                Keywords = task.Keywords.Select(k => k.Keyword)
            };

            var sendEndpoint = await _sendEndpointProvider.GetSendEndpoint(
                new Uri($"queue:{RabbitMQSettings.TaskStartedEventQueue}"));
            await sendEndpoint.Send(taskStartedEvent);

            return Ok();
        }

        [HttpPost("complete")]
        public async Task<IActionResult> CompleteTask(DtoUpdateTaskStatus dto)
        {
            var task = await _context.Tasks
                .Include(t => t.Keywords)
                .FirstOrDefaultAsync(t => t.Id == dto.TaskId);

            if (task == null)
                return NotFound();

            if (task.Status != TaskStatus.InProgress)
                return BadRequest("Task is not in progress");

            task.Status = TaskStatus.Completed;
            task.CompletedAt = dto.CompletedAt ?? DateTime.UtcNow;
            task.ActualMinutes = dto.ActualMinutes ?? CalculateActualMinutes(task.StartDate, task.CompletedAt);

            // Activity log
            var activity = new TaskActivity
            {
                Id = Guid.NewGuid(),
                TaskId = task.Id,
                UserId = dto.UserId,
                ActivityType = "Completed",
                ActivityTime = task.CompletedAt.Value,
                Comments = dto.Comments
            };

            await _context.TaskActivities.AddAsync(activity);
            await _context.SaveChangesAsync();

            // Event publish
            var taskCompletedEvent = new TaskCompletedEvent
            {
                TaskId = task.Id,
                ProjectId = task.ProjectId,
                UserId = dto.UserId,
                CompletedAt = task.CompletedAt.Value,
                StartedAt = task.StartDate.Value,
                DurationMinutes = task.ActualMinutes.Value,
                Keywords = task.Keywords.Select(k => k.Keyword)
            };

            var sendEndpoint = await _sendEndpointProvider.GetSendEndpoint(
                new Uri($"queue:{RabbitMQSettings.TaskCompletedEventQueue}"));
            await sendEndpoint.Send(taskCompletedEvent);

            // Performans hesapla
            await CalculatePerformance(task);

            return Ok();
        }

        [HttpGet("project/{projectId}")]
        public async Task<IActionResult> GetTasksByProject(string projectId)
        {
            Guid projectGuid;
            
            if (Guid.TryParse(projectId, out projectGuid))
            {
                // ProjectId bir Guid ise
                var tasks = await _context.Tasks
                    .Where(t => t.ProjectId == projectGuid)
                    .Include(t => t.Keywords)
                    .Select(t => new DtoTaskList
                    {
                        TaskId = t.Id,
                        TaskName = t.TaskName,
                        Status = t.Status.ToString(),  // ToString() ile string'e çevirdik
                        Priority = t.Priority.ToString(), // ToString() ile string'e çevirdik
                        DueDate = t.DueDate,
                        EstimatedMinutes = t.EstimatedMinutes,
                        AssignedUserId = t.AssignedUserId,
                        Keywords = t.Keywords.Select(k => k.Keyword)
                    })
                    .ToListAsync();

                return Ok(tasks);
            }
            else
            {
                return Ok(new List<DtoTaskList>());
            }
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetTasksByUser(Guid userId)
        {
            var tasks = await _context.Tasks
                .Where(t => t.AssignedUserId == userId)
                .Include(t => t.Keywords)
                .Select(t => new DtoTaskList
                {
                    TaskId = t.Id,
                    TaskName = t.TaskName,
                    Status = t.Status.ToString(), 
                    Priority = t.Priority.ToString(),
                    DueDate = t.DueDate,
                    EstimatedMinutes = t.EstimatedMinutes,
                    AssignedUserId = t.AssignedUserId,
                    Keywords = t.Keywords.Select(k => k.Keyword)
                })
                .ToListAsync();

            return Ok(tasks);
        }

        [HttpGet("performance/{taskId}")]
        public async Task<IActionResult> GetTaskPerformance(Guid taskId)
        {
            var performance = await _context.TaskPerformanceRatings
                .Where(p => p.TaskId == taskId)
                .Include(p => p.SimilarTasks)
                .FirstOrDefaultAsync();

            if (performance == null)
                return NotFound();

            var task = await _context.Tasks.FirstOrDefaultAsync(t => t.Id == taskId);

            var result = new DtoTaskPerformanceResult
            {
                TaskId = taskId,
                TaskName = task.TaskName,
                PerformanceScore = performance.PerformanceScore,
                TimeEfficiencyScore = performance.TimeEfficiencyScore,
                ActualMinutes = performance.ActualMinutes,
                AverageMinutesForSimilarTasks = performance.AverageMinutesForSimilarTasks,
                SimilarTasks = performance.SimilarTasks.Select(s => new DtoSimilarTask
                {
                    TaskId = s.SimilarTaskId,
                    TaskName = _context.Tasks.FirstOrDefault(t => t.Id == s.SimilarTaskId)?.TaskName ?? "",
                    SimilarityPercentage = s.SimilarityPercentage,
                    DurationMinutes = s.SimilarTaskDurationMinutes
                })
            };

            return Ok(result);
        }

        #region Private Methods

        private int CalculateActualMinutes(DateTime? startDate, DateTime? completedAt)
        {
            if (!startDate.HasValue || !completedAt.HasValue)
                return 0;

            return (int)(completedAt.Value - startDate.Value).TotalMinutes;
        }

        private async System.Threading.Tasks.Task CalculatePerformance(Models.Task task)
        {
            var similarTasks = await FindSimilarTasks(task);

            var averageMinutes = similarTasks.Any()
                ? (int)similarTasks.Average(s => s.DurationMinutes)
                : task.EstimatedMinutes ?? task.ActualMinutes ?? 0;

            var timeEfficiencyScore = CalculateTimeEfficiencyScore(task.ActualMinutes ?? 0, averageMinutes);
            var similarityScore = CalculateSimilarityScore(similarTasks);
            var overallPerformance = (timeEfficiencyScore * 0.6) + (similarityScore * 0.4);

            var performanceRating = new TaskPerformanceRating
            {
                Id = Guid.NewGuid(),
                TaskId = task.Id,
                UserId = task.AssignedUserId,
                PerformanceScore = overallPerformance,
                SimilarityScore = similarityScore,
                TimeEfficiencyScore = timeEfficiencyScore,
                ActualMinutes = task.ActualMinutes ?? 0,
                AverageMinutesForSimilarTasks = averageMinutes,
                SimilarTasksCount = similarTasks.Count(),
                CalculatedAt = DateTime.UtcNow
            };

            await _context.TaskPerformanceRatings.AddAsync(performanceRating);

            // Benzer task referanslarını kaydet
            foreach (var similarTask in similarTasks)
            {
                var reference = new SimilarTaskReference
                {
                    Id = Guid.NewGuid(),
                    PerformanceRatingId = performanceRating.Id,
                    SimilarTaskId = similarTask.TaskId,
                    SimilarityPercentage = similarTask.SimilarityPercentage,
                    SimilarTaskDurationMinutes = similarTask.DurationMinutes
                };
                await _context.SimilarTaskReferences.AddAsync(reference);
            }

            await _context.SaveChangesAsync();

            // Performance event publish
            var performanceEvent = new TaskPerformanceCalculatedEvent
            {
                TaskId = task.Id,
                UserId = task.AssignedUserId,
                PerformanceScore = overallPerformance,
                TimeEfficiencyScore = timeEfficiencyScore,
                ActualMinutes = task.ActualMinutes ?? 0,
                AverageMinutesForSimilarTasks = averageMinutes,
                CalculatedAt = DateTime.UtcNow
            };

            var sendEndpoint = await _sendEndpointProvider.GetSendEndpoint(
                new Uri($"queue:{RabbitMQSettings.TaskPerformanceCalculatedEventQueue}"));
            await sendEndpoint.Send(performanceEvent);
        }

        private async Task<IEnumerable<DtoSimilarTask>> FindSimilarTasks(Models.Task task)
        {
            var taskKeywords = await _context.TaskKeywords
                .Where(k => k.TaskId == task.Id)
                .Select(k => k.Keyword)
                .ToListAsync();

            // Aynı projeden ve tamamlanmış taskları getir
            var completedTasks = await _context.Tasks
                .Where(t => t.ProjectId == task.ProjectId &&
                           t.Status == TaskStatus.Completed &&
                           t.Id != task.Id)
                .Include(t => t.Keywords)
                .ToListAsync();

            var similarTasks = new List<DtoSimilarTask>();

            foreach (var completedTask in completedTasks)
            {
                var completedTaskKeywords = completedTask.Keywords.Select(k => k.Keyword).ToList();
                var similarity = CalculateKeywordSimilarity(taskKeywords, completedTaskKeywords);

                if (similarity >= 0.7) // %70 ve üzeri benzerlik
                {
                    similarTasks.Add(new DtoSimilarTask
                    {
                        TaskId = completedTask.Id,
                        TaskName = completedTask.TaskName,
                        SimilarityPercentage = similarity * 100,
                        DurationMinutes = completedTask.ActualMinutes ?? 0,
                        CompletedAt = completedTask.CompletedAt ?? DateTime.MinValue
                    });
                }
            }

            return similarTasks.OrderByDescending(s => s.SimilarityPercentage).Take(10);
        }

        private double CalculateKeywordSimilarity(List<string> keywords1, List<string> keywords2)
        {
            if (!keywords1.Any() || !keywords2.Any())
                return 0;

            var intersection = keywords1.Intersect(keywords2).Count();
            var union = keywords1.Union(keywords2).Count();

            return (double)intersection / union; // Jaccard similarity
        }

        private double CalculateTimeEfficiencyScore(int actualMinutes, int averageMinutes)
        {
            if (averageMinutes == 0)
                return 100;

            var efficiency = 1 - ((double)(actualMinutes - averageMinutes) / averageMinutes);
            return Math.Max(0, Math.Min(100, efficiency * 100));
        }

        private double CalculateSimilarityScore(IEnumerable<DtoSimilarTask> similarTasks)
        {
            if (!similarTasks.Any())
                return 50; // Varsayılan skor

            return similarTasks.Average(s => s.SimilarityPercentage);
        }

        #endregion
    }
}