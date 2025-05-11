using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Moq;
using Planify_BackEnd.DTOs.SubTasks;
using Planify_BackEnd.Hub;
using Planify_BackEnd.Repositories.JoinGroups;
using Planify_BackEnd.Repositories.Tasks;
using Planify_BackEnd.Repositories;
using Planify_BackEnd.Services.SubTasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Planify_BackEnd.Models;
using Planify_BackEnd.Services.Notification;

namespace Planify.Tests.Services.Test.SubtaskServiceTest
{
    [TestFixture]
    public class SubTaskService_UpdateTests
    {
        private Mock<ISubTaskRepository> _subTaskRepoMock;
        private Mock<IHttpContextAccessor> _httpContextAccessorMock;
        private Mock<ITaskRepository> _taskRepoMock;
        private Mock<IHubContext<NotificationHub>> _hubContextMock;
        private Mock<IJoinProjectRepository> _joinProjectRepoMock;
        private Mock<IUserRepository> _userRepoMock;
        private Mock<IEmailSender> _emailSenderMock;
        private SubTaskService _subTaskService;

        [SetUp]
        public void Setup()
        {
            _subTaskRepoMock = new Mock<ISubTaskRepository>();
            _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            _taskRepoMock = new Mock<ITaskRepository>();
            _hubContextMock = new Mock<IHubContext<NotificationHub>>();
            _joinProjectRepoMock = new Mock<IJoinProjectRepository>();
            _userRepoMock = new Mock<IUserRepository>();
            _emailSenderMock = new Mock<IEmailSender>();

            _subTaskService = new SubTaskService(
                _subTaskRepoMock.Object,
                _httpContextAccessorMock.Object,
                _taskRepoMock.Object,
                _hubContextMock.Object,
                _joinProjectRepoMock.Object,
                _userRepoMock.Object,
                _emailSenderMock.Object
            );
        }

        [Test]
        public async System.Threading.Tasks.Task UpdateSubTaskAsync_SubTaskExists_ReturnsSuccess()
        {
            // Arrange
            int subTaskId = 1;
            var existingSubTask = new SubTask
            {
                Id = subTaskId,
                SubTaskName = "Old name",
                SubTaskDescription = "Old desc",
                StartTime = DateTime.Now,
                Deadline = DateTime.Now.AddDays(1),
                AmountBudget = 1000
            };

            var updateDto = new SubTaskUpdateRequestDTO
            {
                SubTaskName = "New name",
                SubTaskDescription = "New desc",
                StartTime = DateTime.Now.AddDays(1),
                Deadline = DateTime.Now.AddDays(2),
                AmountBudget = 2000
            };

            var updatedSubTask = new SubTask
            {
                Id = subTaskId,
                SubTaskName = updateDto.SubTaskName,
                SubTaskDescription = updateDto.SubTaskDescription,
                StartTime = updateDto.StartTime,
                Deadline = updateDto.Deadline,
                AmountBudget = updateDto.AmountBudget
            };

            _subTaskRepoMock.Setup(repo => repo.GetSubTaskByIdAsync(subTaskId))
                            .ReturnsAsync(existingSubTask);

            _subTaskRepoMock.Setup(repo => repo.UpdateSubTaskAsync(subTaskId, It.IsAny<SubTask>()))
                            .ReturnsAsync(updatedSubTask);

            // Act
            var result = await _subTaskService.UpdateSubTaskAsync(subTaskId, updateDto);

            // Assert
            Assert.AreEqual(200, result.Status);
            Assert.AreEqual("Sub-task updated successfully!", result.Message);
            Assert.IsNotNull(result.Result);
            var resultData = result.Result as SubTask;
            Assert.AreEqual(updateDto.SubTaskName, resultData.SubTaskName);
            Assert.AreEqual(updateDto.AmountBudget, resultData.AmountBudget);
        }

        [Test]
        public async System.Threading.Tasks.Task UpdateSubTaskAsync_SubTaskNotFound_ReturnsNotFound()
        {
            // Arrange
            int subTaskId = 999;
            _subTaskRepoMock.Setup(repo => repo.GetSubTaskByIdAsync(subTaskId))
                            .ReturnsAsync((SubTask)null);

            var updateDto = new SubTaskUpdateRequestDTO(); // dữ liệu bất kỳ

            // Act
            var result = await _subTaskService.UpdateSubTaskAsync(subTaskId, updateDto);

            // Assert
            Assert.AreEqual(404, result.Status);
            Assert.AreEqual("Sub-task not found.", result.Message);
            Assert.IsNull(result.Result);
        }

        [Test]
        public async System.Threading.Tasks.Task UpdateSubTaskAsync_ExceptionThrown_ReturnsError()
        {
            // Arrange
            int subTaskId = 1;
            var existingSubTask = new SubTask();
            var updateDto = new SubTaskUpdateRequestDTO();

            _subTaskRepoMock.Setup(repo => repo.GetSubTaskByIdAsync(subTaskId))
                            .ReturnsAsync(existingSubTask);

            _subTaskRepoMock.Setup(repo => repo.UpdateSubTaskAsync(subTaskId, It.IsAny<SubTask>()))
                            .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _subTaskService.UpdateSubTaskAsync(subTaskId, updateDto);

            // Assert
            Assert.AreEqual(500, result.Status);
            Assert.AreEqual("Error occurs while updating sub-task!", result.Message);
            Assert.AreEqual("Database error", result.Result);
        }
    }
}
