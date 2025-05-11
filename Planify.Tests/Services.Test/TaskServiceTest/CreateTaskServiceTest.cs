using NUnit.Framework;
using Moq;
using System;
using System.Threading.Tasks;
using Planify_BackEnd.Services.Tasks;
using Planify_BackEnd.Repositories.Tasks;
using Planify_BackEnd.DTOs.Tasks;
using Planify_BackEnd.DTOs;
using Planify_BackEnd.Models;
using TaskModel = Planify_BackEnd.Models.Task;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Planify_BackEnd.Hub;
using Planify_BackEnd.Services.Notification;

namespace Planify.Tests.Services.Test.TaskServiceTest
{
    [TestFixture]
    public class CreateTaskAsyncTests
    {
        private Mock<ITaskRepository> _taskRepositoryMock;
        private Mock<IHttpContextAccessor> _httpContextAccessorMock;
        private Mock<IEmailSender> _emailSenderMock;
        private Mock<IHubContext<NotificationHub>> _hubContextMock;
        private Mock<IUserRepository> _userRepositoryMock;
        private TaskService _taskService;
        private Guid _organizerId;

        [SetUp]
        public void Setup()
        {
            _taskRepositoryMock = new Mock<ITaskRepository>();
            _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            _hubContextMock = new Mock<IHubContext<NotificationHub>>();
            _emailSenderMock = new Mock<IEmailSender>();
            _userRepositoryMock = new Mock<IUserRepository>();

            _taskService = new TaskService(
                _taskRepositoryMock.Object,
                _httpContextAccessorMock.Object,
                _hubContextMock.Object,
                _emailSenderMock.Object,
                _userRepositoryMock.Object
            );
            _organizerId = Guid.NewGuid();
        }

        [Test]
        public async System.Threading.Tasks.Task CreateTaskAsync_TaskNameIsNull_ReturnsBadRequest()
        {
            var request = new TaskCreateRequestDTO
            {
                TaskName = null,
                EventId = 1,
                StartTime = DateTime.Parse("2025-04-19"),
                Deadline = DateTime.Parse("2025-04-20")
            };

            var result = await _taskService.CreateTaskAsync(request, _organizerId);

            Assert.AreEqual(400, result.Status);
            Assert.AreEqual("Task name is required.", result.Message);
        }

        [Test]
        public async System.Threading.Tasks.Task CreateTaskAsync_TaskNameIsWhitespace_ReturnsBadRequest()
        {
            var request = new TaskCreateRequestDTO
            {
                TaskName = "   ",
                EventId = 1,
                StartTime = DateTime.Parse("2025-04-19"),
                Deadline = DateTime.Parse("2025-04-20")
            };

            var result = await _taskService.CreateTaskAsync(request, _organizerId);

            Assert.AreEqual(400, result.Status);
            Assert.AreEqual("Task name is required.", result.Message);
        }

        [Test]
        public async System.Threading.Tasks.Task CreateTaskAsync_StartTimeEqualToDeadline_ReturnsBadRequest()
        {
            var now = DateTime.UtcNow;
            var request = new TaskCreateRequestDTO
            {
                TaskName = "Test",
                EventId = 1,
                StartTime = now,
                Deadline = now
            };

            var result = await _taskService.CreateTaskAsync(request, _organizerId);

            Assert.AreEqual(400, result.Status);
            Assert.AreEqual("Start time must be earlier than deadline.", result.Message);
        }

        [Test]
        public async System.Threading.Tasks.Task CreateTaskAsync_StartTimeAfterDeadline_ReturnsBadRequest()
        {
            var now = DateTime.UtcNow;
            var request = new TaskCreateRequestDTO
            {
                TaskName = "Test",
                EventId = 1,
                StartTime = DateTime.Parse("2025-04-20"),
                Deadline = DateTime.Parse("2025-04-19")
            };

            var result = await _taskService.CreateTaskAsync(request, _organizerId);

            Assert.AreEqual(400, result.Status);
            Assert.AreEqual("Start time must be earlier than deadline.", result.Message);
        }

        [Test]
        public async System.Threading.Tasks.Task CreateTaskAsync_ValidInput_ReturnsCreated()
        {
            var request = new TaskCreateRequestDTO
            {
                TaskName = "Valid Task",
                EventId = 1,
                StartTime = DateTime.Parse("2025-04-19"),
                Deadline = DateTime.Parse("2025-04-20"),
                AmountBudget = 500
            };

            var expectedTask = new TaskModel
            {
                TaskName = request.TaskName,
                EventId = request.EventId,
                StartTime = request.StartTime,
                Deadline = request.Deadline,
                AmountBudget = request.AmountBudget,
                Status = 1,
                CreateBy = _organizerId,
                CreateDate = DateTime.UtcNow
            };

            _taskRepositoryMock
                .Setup(repo => repo.CreateTaskAsync(It.IsAny<TaskModel>()))
                .ReturnsAsync(expectedTask);


            var result = await _taskService.CreateTaskAsync(request, _organizerId);

            Assert.AreEqual(201, result.Status);
            Assert.AreEqual("Task creates successfully!", result.Message);
            Assert.IsNotNull(result.Result);

            var taskResult = result.Result as TaskModel;
            Assert.AreEqual(expectedTask.TaskName, taskResult.TaskName);
            Assert.AreEqual(expectedTask.AmountBudget, taskResult.AmountBudget);
        }

        [Test]
        public async System.Threading.Tasks.Task CreateTaskAsync_NullRequest_Returns500()
        {
            var result = await _taskService.CreateTaskAsync(null, _organizerId);

            Assert.AreEqual(500, result.Status);
            Assert.AreEqual("Error orcurs while creating task!", result.Message);
        }

        [Test]
        public async System.Threading.Tasks.Task CreateTaskAsync_RepositoryThrowsException_Returns500()
        {
            var request = new TaskCreateRequestDTO
            {
                TaskName = "Valid Task",
                EventId = 1,
                StartTime = DateTime.UtcNow,
                Deadline = DateTime.UtcNow.AddDays(1),
                AmountBudget = 100
            };

            _taskRepositoryMock
                .Setup(repo => repo.CreateTaskAsync(It.IsAny<TaskModel>()))
                .Throws(new Exception("DB error"));

            var result = await _taskService.CreateTaskAsync(request, _organizerId);

            Assert.AreEqual(500, result.Status);
            Assert.AreEqual("Database error while creating task!", result.Message);
            Assert.IsNotNull(result.Result);
            Assert.IsInstanceOf<string>(result.Result);
        }
    }
}
