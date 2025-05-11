using NUnit.Framework;
using Moq;
using System.Threading.Tasks;
using Planify_BackEnd.Repositories.Tasks;
using Planify_BackEnd.Models;
using Planify_BackEnd.DTOs.SubTasks;
using Planify_BackEnd.Repositories;
using Planify_BackEnd.Services.SubTasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Planify_BackEnd.Hub;
using Planify_BackEnd.Repositories.JoinGroups;
using Planify_BackEnd.Services.Notification;

namespace Planify.Tests.Services.Test.SubtaskServiceTest
{
    [TestFixture]
    public class CreateSubTaskAsyncTests
    {
        private Mock<ITaskRepository> _taskRepositoryMock;
        private Mock<ISubTaskRepository> _subTaskRepositoryMock;
        private Mock<IHttpContextAccessor> _httpContextAccessorMock;
        private Mock<IHubContext<NotificationHub>> _hubContextMock;
        private Mock<IJoinProjectRepository> _joinProjectRepositoryMock;
        private Mock<IUserRepository> _userRepositoryMock;
        private Mock<IEmailSender> _emailSenderMock;

        private SubTaskService _subTaskService;
        private Guid _implementerId;

        [SetUp]
        public void Setup()
        {
            _taskRepositoryMock = new Mock<ITaskRepository>();
            _subTaskRepositoryMock = new Mock<ISubTaskRepository>();
            _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            _hubContextMock = new Mock<IHubContext<NotificationHub>>();
            _joinProjectRepositoryMock = new Mock<IJoinProjectRepository>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _emailSenderMock = new Mock<IEmailSender>();

            _subTaskService = new SubTaskService(
                _subTaskRepositoryMock.Object,
                _httpContextAccessorMock.Object,
                _taskRepositoryMock.Object,
                _hubContextMock.Object,
                _joinProjectRepositoryMock.Object,
                _userRepositoryMock.Object,
                _emailSenderMock.Object
            );

            _implementerId = Guid.NewGuid();
        }

        [Test]
        public async System.Threading.Tasks.Task CreateSubTaskAsync_TaskNotExist_ReturnsBadRequest()
        {
            var request = new SubTaskCreateRequestDTO
            {
                TaskId = 999,
                SubTaskName = "Test",
                StartTime = new DateTime(2025, 4, 19),
                Deadline = new DateTime(2025, 4, 20),
                AmountBudget = 100
            };

            _taskRepositoryMock
                .Setup(repo => repo.IsTaskExists(It.IsAny<int>()))
                .Returns(false);

            var result = await _subTaskService.CreateSubTaskAsync(request, _implementerId);

            Assert.AreEqual(500, result.Status);
            Assert.AreEqual("Error orcurs while creating sub-task!", result.Message);
        }

        [Test]
        public async System.Threading.Tasks.Task CreateSubTaskAsync_SubTaskNameIsNull_ReturnsBadRequest()
        {
            var request = new SubTaskCreateRequestDTO
            {
                TaskId = 1,
                SubTaskName = null,
                StartTime = new DateTime(2025, 4, 19),
                Deadline = new DateTime(2025, 4, 20),
                AmountBudget = 100
            };
            _taskRepositoryMock
            .Setup(repo => repo.IsTaskExists(It.IsAny<int>()))
            .Returns(true);
            var result = await _subTaskService.CreateSubTaskAsync(request, _implementerId);

            Assert.AreEqual(400, result.Status);
            Assert.AreEqual("Sub-task name is required.", result.Message);
        }

        [Test]
        public async System.Threading.Tasks.Task CreateSubTaskAsync_SubTaskNameIsWhitespace_ReturnsBadRequest()
        {
            var request = new SubTaskCreateRequestDTO
            {
                TaskId = 1,
                SubTaskName = "   ",
                StartTime = DateTime.Parse("2025-04-19"),
                Deadline = DateTime.Parse("2025-04-20"),
                AmountBudget = 100,
                SubTaskDescription = "test"
            };
            _taskRepositoryMock
            .Setup(repo => repo.IsTaskExists(It.IsAny<int>()))
            .Returns(true);

            var result = await _subTaskService.CreateSubTaskAsync(request, _implementerId);

            Assert.AreEqual(400, result.Status);
            Assert.AreEqual("Sub-task name is required.", result.Message);
        }

        [Test]
        public async System.Threading.Tasks.Task CreateSubTaskAsync_StartTimeEqualToDeadline_ReturnsBadRequest()
        {
            var request = new SubTaskCreateRequestDTO
            {
                TaskId = 1,
                SubTaskName = "Test",
                StartTime = DateTime.Parse("2025-04-20"),
                Deadline = DateTime.Parse("2025-04-20"),
                AmountBudget = 100
            };
            _taskRepositoryMock
            .Setup(repo => repo.IsTaskExists(It.IsAny<int>()))
            .Returns(true);

            var result = await _subTaskService.CreateSubTaskAsync(request, _implementerId);

            Assert.AreEqual(400, result.Status);
            Assert.AreEqual("Start time must be earlier than deadline.", result.Message);
        }

        [Test]
        public async System.Threading.Tasks.Task CreateSubTaskAsync_StartTimeAfterDeadline_ReturnsBadRequest()
        {
            var request = new SubTaskCreateRequestDTO
            {
                TaskId = 1,
                SubTaskName = "Test",
                StartTime = DateTime.Parse("2025-04-20"),
                Deadline = DateTime.Parse("2025-04-19"),
                AmountBudget = 100
            };
            _taskRepositoryMock
            .Setup(repo => repo.IsTaskExists(It.IsAny<int>()))
            .Returns(true);

            var result = await _subTaskService.CreateSubTaskAsync(request, _implementerId);

            Assert.AreEqual(400, result.Status);
            Assert.AreEqual("Start time must be earlier than deadline.", result.Message);
        }

        [Test]
        public async System.Threading.Tasks.Task CreateSubTaskAsync_ValidInput_ReturnsCreated()
        {
            var request = new SubTaskCreateRequestDTO
            {
                TaskId = 1,
                SubTaskName = "Test",
                StartTime = DateTime.Parse("2025-04-19"),
                Deadline = DateTime.Parse("2025-04-20"),
                AmountBudget = 100,
                SubTaskDescription = "test"
            };

            var expectedSubTask = new SubTask
            {
                TaskId = request.TaskId,
                SubTaskName = request.SubTaskName,
                StartTime = request.StartTime,
                Deadline = request.Deadline,
                AmountBudget = request.AmountBudget,
                Status = 1,
                CreateBy = _implementerId
            };

            _taskRepositoryMock
                .Setup(repo => repo.IsTaskExists(It.IsAny<int>()))
                .Returns(true);

            _subTaskRepositoryMock
                .Setup(repo => repo.CreateSubTaskAsync(It.IsAny<SubTask>()))
                .ReturnsAsync(expectedSubTask);

            var result = await _subTaskService.CreateSubTaskAsync(request, _implementerId);

            Assert.AreEqual(201, result.Status);
            Assert.AreEqual("Sub-task creates successfully!", result.Message);
            Assert.IsNotNull(result.Result);
        }

        [Test]
        public async System.Threading.Tasks.Task CreateSubTaskAsync_NullRequest_Returns500()
        {
            var result = await _subTaskService.CreateSubTaskAsync(null, _implementerId);

            Assert.AreEqual(500, result.Status);
            Assert.AreEqual("Error orcurs while creating sub-task!", result.Message);
        }

        [Test]
        public async System.Threading.Tasks.Task CreateSubTaskAsync_RepositoryThrowsException_Returns500()
        {
            var request = new SubTaskCreateRequestDTO
            {
                TaskId = 1,
                SubTaskName = "Test",
                StartTime = DateTime.Parse("2025-04-19"),
                Deadline = DateTime.Parse("2025-04-20"),
                AmountBudget = 100
            };

            _taskRepositoryMock
                .Setup(repo => repo.IsTaskExists(It.IsAny<int>()))
                .Returns(true);

            _subTaskRepositoryMock
                .Setup(repo => repo.CreateSubTaskAsync(It.IsAny<SubTask>()))
                .Throws(new Exception("DB error"));

            var result = await _subTaskService.CreateSubTaskAsync(request, _implementerId);

            Assert.AreEqual(500, result.Status);
            Assert.AreEqual("Database error while creating sub-task!", result.Message);
        }
    }
}
