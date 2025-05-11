using NUnit.Framework;
using Moq;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Planify_BackEnd.Hub;
using Planify_BackEnd.Repositories.Tasks;
using Planify_BackEnd.Services.Tasks;
using Planify_BackEnd.Services.Notification;

namespace Planify.Tests.Services.Test.TaskServiceTest
{

    [TestFixture]
    public class TaskServiceDeleteTests
    {
        private Mock<ITaskRepository> _taskRepoMock;
        private Mock<IHttpContextAccessor> _httpContextAccessorMock;
        private Mock<IHubContext<NotificationHub>> _hubContextMock;
        private Mock<IEmailSender> _emailSenderMock;
        private Mock<IUserRepository> _userRepositoryMock;
        private TaskService _taskService;

        [SetUp]
        public void Setup()
        {
            _taskRepoMock = new Mock<ITaskRepository>();
            _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            _hubContextMock = new Mock<IHubContext<NotificationHub>>();
            _emailSenderMock = new Mock<IEmailSender>();
            _userRepositoryMock = new Mock<IUserRepository>();

            _taskService = new TaskService(
                _taskRepoMock.Object,
                _httpContextAccessorMock.Object,
                _hubContextMock.Object,
                _emailSenderMock.Object,
                _userRepositoryMock.Object
            );
        }

        [Test]
        public async Task DeleteTaskAsync_TaskExists_ReturnsSuccess()
        {
            // Arrange
            int taskId = 1;
            _taskRepoMock.Setup(repo => repo.DeleteTaskAsync(taskId)).ReturnsAsync(true);

            // Act
            var result = await _taskService.DeleteTaskAsync(taskId);

            // Assert
            Assert.AreEqual(200, result.Status);
            Assert.AreEqual("Task deleted successfully!", result.Message);
            Assert.IsNull(result.Result);
        }

        [Test]
        public async Task DeleteTaskAsync_TaskNotFound_ReturnsNotFound()
        {
            // Arrange
            int taskId = 999;
            _taskRepoMock.Setup(repo => repo.DeleteTaskAsync(taskId)).ReturnsAsync(false);

            // Act
            var result = await _taskService.DeleteTaskAsync(taskId);

            // Assert
            Assert.AreEqual(404, result.Status);
            Assert.AreEqual("Task not found or already deleted.", result.Message);
            Assert.IsNull(result.Result);
        }

        [Test]
        public async Task DeleteTaskAsync_ExceptionThrown_ReturnsError()
        {
            // Arrange
            int taskId = 1;
            _taskRepoMock.Setup(repo => repo.DeleteTaskAsync(taskId))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _taskService.DeleteTaskAsync(taskId);

            // Assert
            Assert.AreEqual(500, result.Status);
            Assert.AreEqual("Error occurs while deleting task!", result.Message);
            Assert.AreEqual("Database error", result.Result);
        }
    }

}
