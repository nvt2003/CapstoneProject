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
using Planify_BackEnd.Services.Notification;
using Planify_BackEnd.Models;

namespace Planify.Tests.Services.Test.SubtaskServiceTest
{
    [TestFixture]
    public class SubTaskService_GetByTaskIdTests
    {
        private Mock<ISubTaskRepository> _subTaskRepoMock;
        private SubTaskService _subTaskService;
        private Guid _userId;

        [SetUp]
        public void Setup()
        {
            _subTaskRepoMock = new Mock<ISubTaskRepository>();
            _subTaskService = new SubTaskService(
                _subTaskRepoMock.Object,
                new Mock<IHttpContextAccessor>().Object,
                new Mock<ITaskRepository>().Object,
                new Mock<IHubContext<NotificationHub>>().Object,
                new Mock<IJoinProjectRepository>().Object,
                new Mock<IUserRepository>().Object,
                new Mock<IEmailSender>().Object
            );
            _userId = Guid.NewGuid();
        }

        [Test]
        public async System.Threading.Tasks.Task GetSubTasksByTaskIdAsync_SubTasksFound_Returns200()
        {
            // Arrange
            int taskId = 1;
            var subTasks = new List<SubTask>
        {
            new SubTask
            {
                Id = 1,
                TaskId = taskId,
                SubTaskName = "Sub 1",
                SubTaskDescription = "Desc 1",
                StartTime = DateTime.Now,
                Deadline = DateTime.Now.AddDays(1),
                AmountBudget = 500,
                Status = 1,
                CreateBy = _userId
            }
        };

            _subTaskRepoMock.Setup(r => r.GetSubTasksByTaskIdAsync(taskId))
                            .ReturnsAsync(subTasks);

            // Act
            var result = await _subTaskService.GetSubTasksByTaskIdAsync(taskId);

            // Assert
            Assert.AreEqual(200, result.Status);
            Assert.AreEqual("Sub-tasks retrieved successfully", result.Message);
            Assert.IsInstanceOf<List<SubTaskResponseDTO>>(result.Result);
            Assert.AreEqual(1, ((List<SubTaskResponseDTO>)result.Result).Count);
        }

        [Test]
        public async System.Threading.Tasks.Task GetSubTasksByTaskIdAsync_NoSubTasksFound_Returns404()
        {
            // Arrange
            int taskId = 2;
            _subTaskRepoMock.Setup(r => r.GetSubTasksByTaskIdAsync(taskId))
                            .ReturnsAsync(new List<SubTask>());

            // Act
            var result = await _subTaskService.GetSubTasksByTaskIdAsync(taskId);

            // Assert
            Assert.AreEqual(404, result.Status);
            Assert.AreEqual("No sub-tasks found for this task", result.Message);
            Assert.IsNull(result.Result);
        }

        [Test]
        public async System.Threading.Tasks.Task GetSubTasksByTaskIdAsync_ExceptionThrown_Returns500()
        {
            // Arrange
            int taskId = 3;
            _subTaskRepoMock.Setup(r => r.GetSubTasksByTaskIdAsync(taskId))
                            .ThrowsAsync(new Exception("DB error"));

            // Act
            var result = await _subTaskService.GetSubTasksByTaskIdAsync(taskId);

            // Assert
            Assert.AreEqual(500, result.Status);
            Assert.AreEqual("Error occurred while retrieving sub-tasks", result.Message);
            Assert.AreEqual("DB error", result.Result);
        }
    }

}
