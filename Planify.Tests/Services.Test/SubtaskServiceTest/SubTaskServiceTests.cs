using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Moq;
using Planify_BackEnd.DTOs.SubTasks;
using Planify_BackEnd.Hub;
using Planify_BackEnd.Repositories.JoinGroups;
using Planify_BackEnd.Repositories.Tasks;
using Planify_BackEnd.Repositories;
using Planify_BackEnd.Services.SubTasks;
using Planify_BackEnd.Services.Notification;
using Planify_BackEnd.Models;

namespace Planify.Tests.Services.Test.SubtaskServiceTest
{
    [TestFixture]
    public class SubTaskServiceTests
    {
        private Mock<ISubTaskRepository> _subTaskRepoMock;
        private Mock<IHttpContextAccessor> _httpContextAccessorMock;
        private Mock<ITaskRepository> _taskRepoMock;
        private Mock<IHubContext<NotificationHub>> _hubContextMock;
        private Mock<IJoinProjectRepository> _joinProjectRepoMock;
        private Mock<IUserRepository> _userRepoMock;
        private Mock<IEmailSender> _emailSenderMock;
        private SubTaskService _subTaskService;
        private Guid _userId;

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
            _userId = Guid.NewGuid();
        }

        [Test]
        public async System.Threading.Tasks.Task GetSubTaskByIdAsync_SubTaskExists_ReturnsSuccess()
        {
            // Arrange
            int subTaskId = 1;
            var mockSubTask = new SubTask
            {
                Id = subTaskId,
                TaskId = 10,
                SubTaskName = "Sub-task A",
                SubTaskDescription = "Description",
                StartTime = DateTime.Now,
                Deadline = DateTime.Now.AddDays(2),
                AmountBudget = 500,
                Status = 1,
                CreateBy = _userId
            };

            _subTaskRepoMock.Setup(repo => repo.GetSubTaskByIdAsync(subTaskId)).ReturnsAsync(mockSubTask);

            // Act
            var result = await _subTaskService.GetSubTaskByIdAsync(subTaskId);

            // Assert
            Assert.AreEqual(200, result.Status);
            Assert.AreEqual("Sub-task retrieved successfully", result.Message);
            var data = result.Result as SubTaskResponseDTO;
            Assert.NotNull(data);
            Assert.AreEqual(subTaskId, data.Id);
            Assert.AreEqual("Sub-task A", data.SubTaskName);
        }

        [Test]
        public async System.Threading.Tasks.Task GetSubTaskByIdAsync_SubTaskNotFound_ReturnsNotFound()
        {
            // Arrange
            int subTaskId = 999;
            _subTaskRepoMock.Setup(repo => repo.GetSubTaskByIdAsync(subTaskId)).ReturnsAsync((SubTask)null);

            // Act
            var result = await _subTaskService.GetSubTaskByIdAsync(subTaskId);

            // Assert
            Assert.AreEqual(404, result.Status);
            Assert.AreEqual("Sub-task not found", result.Message);
            Assert.IsNull(result.Result);
        }

        [Test]
        public async System.Threading.Tasks.Task GetSubTaskByIdAsync_ExceptionThrown_ReturnsError()
        {
            // Arrange
            int subTaskId = 1;
            _subTaskRepoMock.Setup(repo => repo.GetSubTaskByIdAsync(subTaskId))
                .ThrowsAsync(new Exception("Database failure"));

            // Act
            var result = await _subTaskService.GetSubTaskByIdAsync(subTaskId);

            // Assert
            Assert.AreEqual(500, result.Status);
            Assert.AreEqual("Error occurred while retrieving sub-task", result.Message);
            Assert.AreEqual("Database failure", result.Result);
        }
    }
}
