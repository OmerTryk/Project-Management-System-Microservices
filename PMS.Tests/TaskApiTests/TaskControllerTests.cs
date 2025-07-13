using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using TaskApi.Context;
using TaskApi.Controllers;
using TaskApi.Models;
using TaskApi.Models.ViewModels;
using MassTransit;
using Xunit;
using Task = System.Threading.Tasks.Task;

namespace PMS.Tests.TaskApiTests
{
    public class TaskControllerTests
    {
        [Fact]
        public void DtoCreateTask_HasExpectedProperties()
        {
            var dto = new DtoCreateTask
            {
                TaskName = "Test Task",
                Description = "This is a test task",
                ProjectId = Guid.NewGuid(),
                AssignedUserId = Guid.NewGuid(),
                AssignedByUserId = Guid.NewGuid(),
                DueDate = DateTime.Now.AddDays(7),
                Priority = TaskPriority.High,
                EstimatedMinutes = 120,
                Keywords = new List<string> { "test", "unit" }
            };

            Assert.Equal("Test Task", dto.TaskName);
            Assert.Equal("This is a test task", dto.Description);
            Assert.Equal(TaskPriority.High, dto.Priority);
            Assert.Equal(120, dto.EstimatedMinutes);
            Assert.Contains("test", dto.Keywords);
            Assert.Contains("unit", dto.Keywords);
            Assert.Equal(2, dto.Keywords.Count());
        }

        [Fact]
        public void Task_Status_HasExpectedValues()
        {
            Assert.Equal(0, (int)TaskApi.Models.TaskStatus.NotStarted);
            Assert.Equal(1, (int)TaskApi.Models.TaskStatus.InProgress);
            Assert.Equal(2, (int)TaskApi.Models.TaskStatus.Completed);
            Assert.Equal(3, (int)TaskApi.Models.TaskStatus.Cancelled);
        }

        [Fact]
        public void Task_Priority_HasExpectedValues()
        {
            Assert.Equal(0, (int)TaskPriority.Low);
            Assert.Equal(1, (int)TaskPriority.Medium);
            Assert.Equal(2, (int)TaskPriority.High);
            Assert.Equal(3, (int)TaskPriority.Critical);
        }

        [Fact]
        public void Task_Constructor_InitializesDefaultValues()
        {
            var task = new TaskApi.Models.Task();

            Assert.Equal(TaskApi.Models.TaskStatus.NotStarted, task.Status);
            Assert.Equal(TaskPriority.Medium, task.Priority);
            Assert.Equal(Guid.Empty, task.Id);
            Assert.Null(task.StartDate);
            Assert.Null(task.CompletedAt);
            Assert.Null(task.EstimatedMinutes);
            Assert.Null(task.ActualMinutes);
        }

        [Fact]
        public void DtoTaskList_HasExpectedProperties()
        {
            var dto = new DtoTaskList
            {
                TaskId = Guid.NewGuid(),
                TaskName = "Test Task",
                Status = "InProgress",
                Priority = "High",
                DueDate = DateTime.Now.AddDays(7),
                EstimatedMinutes = 120,
                AssignedUserId = Guid.NewGuid(),
                Keywords = new List<string> { "test", "unit" }
            };

            Assert.Equal("Test Task", dto.TaskName);
            Assert.Equal("InProgress", dto.Status);
            Assert.Equal("High", dto.Priority);
            Assert.Equal(120, dto.EstimatedMinutes);
            Assert.Contains("test", dto.Keywords);
            Assert.Contains("unit", dto.Keywords);
        }

        [Fact]
        public void SimilarTaskReference_Constructor_InitializesDefaultValues()
        {
            var reference = new SimilarTaskReference();

            Assert.Equal(Guid.Empty, reference.Id);
            Assert.Equal(Guid.Empty, reference.PerformanceRatingId);
            Assert.Equal(Guid.Empty, reference.SimilarTaskId);
            Assert.Equal(0, reference.SimilarityPercentage);
            Assert.Equal(0, reference.SimilarTaskDurationMinutes);
        }

        [Fact]
        public void DtoSimilarTask_Constructor_InitializesProperties()
        {
            var taskId = Guid.NewGuid();
            var dto = new DtoSimilarTask
            {
                TaskId = taskId,
                TaskName = "Similar Task",
                SimilarityPercentage = 85.5,
                DurationMinutes = 90,
                CompletedAt = DateTime.Now
            };

            Assert.Equal(taskId, dto.TaskId);
            Assert.Equal("Similar Task", dto.TaskName);
            Assert.Equal(85.5, dto.SimilarityPercentage);
            Assert.Equal(90, dto.DurationMinutes);
            Assert.True(dto.CompletedAt > DateTime.MinValue);
        }
    }
}