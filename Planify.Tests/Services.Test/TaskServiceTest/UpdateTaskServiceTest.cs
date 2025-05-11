using Moq;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Http;
using Planify_BackEnd.DTOs.Tasks;
using Planify_BackEnd.Hub;
using Planify_BackEnd.Repositories.Tasks;
using Planify_BackEnd.Services.Tasks;
using Planify_BackEnd.Services.Notification;
using TaskModel = Planify_BackEnd.Models.Task;

namespace Planify.Tests.Services.Test.TaskServiceTest
{
    [TestFixture]
    public class TaskServiceTests
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
        public async Task UpdateTaskAsync_TaskExists_ReturnsSuccess()
        {
            var taskId = 1;
            var taskDto = new TaskUpdateRequestDTO
            {
                TaskName = "Test Task",
                TaskDescription = "Description",
                StartTime = DateTime.Now,
                Deadline = DateTime.Now.AddDays(1),
                AmountBudget = 1000,
                EventId = 2
            };

            var updatedTask = new TaskModel
            {
                Id = taskId,
                TaskName = taskDto.TaskName,
                TaskDescription = taskDto.TaskDescription
            };

            _taskRepoMock
                .Setup(repo => repo.UpdateTaskAsync(taskId, It.IsAny<TaskModel>()))
                .ReturnsAsync(updatedTask);

            var result = await _taskService.UpdateTaskAsync(taskId, taskDto);

            Assert.AreEqual(200, result.Status);
            Assert.AreEqual("Task updated successfully!", result.Message);
            Assert.AreEqual(updatedTask, result.Result);
        }

        [Test]
        public async Task UpdateTaskAsync_TaskNotFound_ReturnsNotFound()
        {
            int taskId = 0;
            var taskDto = new TaskUpdateRequestDTO();

            _taskRepoMock
                .Setup(repo => repo.UpdateTaskAsync(taskId, It.IsAny<TaskModel>()))
                .ReturnsAsync((TaskModel)null);

            var result = await _taskService.UpdateTaskAsync(taskId, taskDto);

            Assert.AreEqual(404, result.Status);
            Assert.AreEqual("Task not found.", result.Message);
        }

        [Test]
        public async Task UpdateTaskAsync_ExceptionThrown_ReturnsError()
        {
            int taskId = 1;
            var taskDto = new TaskUpdateRequestDTO();

            _taskRepoMock
                .Setup(repo => repo.UpdateTaskAsync(taskId, It.IsAny<TaskModel>()))
                .ThrowsAsync(new Exception("Database error"));

            var result = await _taskService.UpdateTaskAsync(taskId, taskDto);

            Assert.AreEqual(500, result.Status);
            Assert.AreEqual("Error occurs while updating task!", result.Message);
            Assert.AreEqual("Database error", result.Result);
        }
    }

}
