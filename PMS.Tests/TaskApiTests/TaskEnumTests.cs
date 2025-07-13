using System;
using System.Collections.Generic;
using System.Linq;
using TaskApi.Models;
using Xunit;

namespace PMS.Tests.TaskApiTests
{
    public class TaskEnumTests
    {
        [Fact]
        public void TaskStatus_HasFourValues()
        {
            var statusValues = Enum.GetValues(typeof(TaskApi.Models.TaskStatus)).Cast<TaskApi.Models.TaskStatus>().ToList();
            
            Assert.Equal(4, statusValues.Count);
            Assert.Contains(TaskApi.Models.TaskStatus.NotStarted, statusValues);
            Assert.Contains(TaskApi.Models.TaskStatus.InProgress, statusValues);
            Assert.Contains(TaskApi.Models.TaskStatus.Completed, statusValues);
            Assert.Contains(TaskApi.Models.TaskStatus.Cancelled, statusValues);
        }
        
        [Fact]
        public void TaskPriority_HasFourValues()
        {
            var priorityValues = Enum.GetValues(typeof(TaskPriority)).Cast<TaskPriority>().ToList();
            
            Assert.Equal(4, priorityValues.Count);
            Assert.Contains(TaskPriority.Low, priorityValues);
            Assert.Contains(TaskPriority.Medium, priorityValues);
            Assert.Contains(TaskPriority.High, priorityValues);
            Assert.Contains(TaskPriority.Critical, priorityValues);
        }
        
        [Theory]
        [InlineData(TaskApi.Models.TaskStatus.NotStarted, 0)]
        [InlineData(TaskApi.Models.TaskStatus.InProgress, 1)]
        [InlineData(TaskApi.Models.TaskStatus.Completed, 2)]
        [InlineData(TaskApi.Models.TaskStatus.Cancelled, 3)]
        public void TaskStatus_HasExpectedIntegerValues(TaskApi.Models.TaskStatus status, int expectedValue)
        {
            Assert.Equal(expectedValue, (int)status);
        }
        
        [Theory]
        [InlineData(TaskPriority.Low, 0)]
        [InlineData(TaskPriority.Medium, 1)]
        [InlineData(TaskPriority.High, 2)]
        [InlineData(TaskPriority.Critical, 3)]
        public void TaskPriority_HasExpectedIntegerValues(TaskPriority priority, int expectedValue)
        {
            Assert.Equal(expectedValue, (int)priority);
        }
        
        [Fact]
        public void TaskStatus_ToString_ReturnsExpectedString()
        {
            Assert.Equal("NotStarted", TaskApi.Models.TaskStatus.NotStarted.ToString());
            Assert.Equal("InProgress", TaskApi.Models.TaskStatus.InProgress.ToString());
            Assert.Equal("Completed", TaskApi.Models.TaskStatus.Completed.ToString());
            Assert.Equal("Cancelled", TaskApi.Models.TaskStatus.Cancelled.ToString());
        }
        
        [Fact]
        public void TaskPriority_ToString_ReturnsExpectedString()
        {
            Assert.Equal("Low", TaskPriority.Low.ToString());
            Assert.Equal("Medium", TaskPriority.Medium.ToString());
            Assert.Equal("High", TaskPriority.High.ToString());
            Assert.Equal("Critical", TaskPriority.Critical.ToString());
        }
    }
}