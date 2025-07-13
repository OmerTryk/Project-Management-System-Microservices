using System;
using System.Collections.Generic;
using System.Linq;
using TaskApi.Models;
using TaskApi.Models.ViewModels;
using Xunit;

namespace PMS.Tests.TaskApiTests
{
    public class TaskModelTests
    {
        [Fact]
        public void Task_Properties_ShouldBeAssigned()
        {
            var taskId = Guid.NewGuid();
            var projectId = Guid.NewGuid();
            var assignedUserId = Guid.NewGuid();
            var assignedByUserId = Guid.NewGuid();
            var createdAt = DateTime.UtcNow;
            var dueDate = DateTime.UtcNow.AddDays(7);

            var task = new TaskApi.Models.Task
            {
                Id = taskId,
                TaskName = "Test Task",
                Description = "Test Description",
                ProjectId = projectId,
                AssignedUserId = assignedUserId,
                AssignedByUserId = assignedByUserId,
                CreatedAt = createdAt,
                DueDate = dueDate,
                Status = TaskApi.Models.TaskStatus.InProgress,
                Priority = TaskPriority.High,
                EstimatedMinutes = 60,
                Keywords = new List<TaskKeyword>()
            };

            Assert.Equal(taskId, task.Id);
            Assert.Equal("Test Task", task.TaskName);
            Assert.Equal("Test Description", task.Description);
            Assert.Equal(projectId, task.ProjectId);
            Assert.Equal(assignedUserId, task.AssignedUserId);
            Assert.Equal(assignedByUserId, task.AssignedByUserId);
            Assert.Equal(createdAt, task.CreatedAt);
            Assert.Equal(dueDate, task.DueDate);
            Assert.Equal(TaskApi.Models.TaskStatus.InProgress, task.Status);
            Assert.Equal(TaskPriority.High, task.Priority);
            Assert.Equal(60, task.EstimatedMinutes);
            Assert.Empty(task.Keywords);
        }

        [Fact]
        public void TaskKeyword_Properties_ShouldBeAssigned()
        {
            var keywordId = Guid.NewGuid();
            var taskId = Guid.NewGuid();
            var keyword = "test";

            var taskKeyword = new TaskKeyword
            {
                Id = keywordId,
                TaskId = taskId,
                Keyword = keyword
            };

            Assert.Equal(keywordId, taskKeyword.Id);
            Assert.Equal(taskId, taskKeyword.TaskId);
            Assert.Equal(keyword, taskKeyword.Keyword);
        }

        [Fact]
        public void DtoCreateTask_Properties_ShouldBeAssigned()
        {
            var projectId = Guid.NewGuid();
            var assignedUserId = Guid.NewGuid();
            var assignedByUserId = Guid.NewGuid();
            var dueDate = DateTime.UtcNow.AddDays(7);
            var keywords = new List<string> { "test", "important" };

            var dto = new DtoCreateTask
            {
                TaskName = "New Task",
                Description = "New Description",
                ProjectId = projectId,
                AssignedUserId = assignedUserId,
                AssignedByUserId = assignedByUserId,
                DueDate = dueDate,
                Priority = TaskPriority.Critical,
                EstimatedMinutes = 120,
                Keywords = keywords
            };

            Assert.Equal("New Task", dto.TaskName);
            Assert.Equal("New Description", dto.Description);
            Assert.Equal(projectId, dto.ProjectId);
            Assert.Equal(assignedUserId, dto.AssignedUserId);
            Assert.Equal(assignedByUserId, dto.AssignedByUserId);
            Assert.Equal(dueDate, dto.DueDate);
            Assert.Equal(TaskPriority.Critical, dto.Priority);
            Assert.Equal(120, dto.EstimatedMinutes);
            Assert.Equal(keywords, dto.Keywords);
        }

        [Fact]
        public void DtoUpdateTaskStatus_Properties_ShouldBeAssigned()
        {
            var taskId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var completedAt = DateTime.UtcNow;

            var dto = new DtoUpdateTaskStatus
            {
                TaskId = taskId,
                UserId = userId,
                Comments = "Task completed",
                CompletedAt = completedAt,
                ActualMinutes = 90
            };

            Assert.Equal(taskId, dto.TaskId);
            Assert.Equal(userId, dto.UserId);
            Assert.Equal("Task completed", dto.Comments);
            Assert.Equal(completedAt, dto.CompletedAt);
            Assert.Equal(90, dto.ActualMinutes);
        }

        [Fact]
        public void DtoTaskPerformanceResult_Properties_ShouldBeAssigned()
        {
            var taskId = Guid.NewGuid();
            var similarTasks = new List<DtoSimilarTask>
            {
                new DtoSimilarTask
                {
                    TaskId = Guid.NewGuid(),
                    TaskName = "Similar Task",
                    SimilarityPercentage = 85.0,
                    DurationMinutes = 100
                }
            };

            var dto = new DtoTaskPerformanceResult
            {
                TaskId = taskId,
                TaskName = "Performance Task",
                PerformanceScore = 90.5,
                TimeEfficiencyScore = 85.0,
                ActualMinutes = 95,
                AverageMinutesForSimilarTasks = 100,
                SimilarTasks = similarTasks
            };

            Assert.Equal(taskId, dto.TaskId);
            Assert.Equal("Performance Task", dto.TaskName);
            Assert.Equal(90.5, dto.PerformanceScore);
            Assert.Equal(85.0, dto.TimeEfficiencyScore);
            Assert.Equal(95, dto.ActualMinutes);
            Assert.Equal(100, dto.AverageMinutesForSimilarTasks);
            Assert.Single(dto.SimilarTasks);
            Assert.Equal("Similar Task", dto.SimilarTasks.First().TaskName);
            Assert.Equal(85.0, dto.SimilarTasks.First().SimilarityPercentage);
        }
    }
}