using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Moq;
using Planify_BackEnd.Hub;
using Planify_BackEnd.Repositories.JoinGroups;
using Planify_BackEnd.Repositories.Tasks;
using Planify_BackEnd.Repositories;
using Planify_BackEnd.Services.SubTasks;
using TaskModel = Planify_BackEnd.Models.Task;
using Planify_BackEnd.Services.Notification;
using Planify_BackEnd.Models;

namespace Planify.Tests.Services.Test.SubtaskServiceTest
{
    [TestFixture]
    public class SubTaskService_AssignSubTaskTests
    {
        private Mock<ISubTaskRepository> _subTaskRepoMock;
        private Mock<ITaskRepository> _taskRepoMock;
        private Mock<IJoinProjectRepository> _joinProjectRepoMock;
        private SubTaskService _subTaskService;

        [SetUp]
        public void Setup()
        {
            _subTaskRepoMock = new Mock<ISubTaskRepository>();
            _taskRepoMock = new Mock<ITaskRepository>();
            _joinProjectRepoMock = new Mock<IJoinProjectRepository>();

            _subTaskService = new SubTaskService(
                _subTaskRepoMock.Object,
                new Mock<IHttpContextAccessor>().Object,
                _taskRepoMock.Object,
                new Mock<IHubContext<NotificationHub>>().Object,
                _joinProjectRepoMock.Object,
                new Mock<IUserRepository>().Object,
                new Mock<IEmailSender>().Object
            );
        }

        [Test]
        public async System.Threading.Tasks.Task AssignSubTask_ValidData_AssignsSuccessfully()
        {
            // Arrange
            Guid assignUserId = Guid.NewGuid();
            Guid userId = Guid.NewGuid();
            int subtaskId = 1;

            var subtask = new SubTask { Id = subtaskId, TaskId = 10 };
            var task = new TaskModel { Id = 10, CreateBy = assignUserId, EventId = 5 };

            _subTaskRepoMock.Setup(r => r.GetSubTaskByIdAsync(subtaskId))
                            .ReturnsAsync(subtask);
            _taskRepoMock.Setup(r => r.GetTaskById(subtask.TaskId))
                         .Returns(task);
            _joinProjectRepoMock.Setup(r => r.IsImplementerInProject(userId, (int)task.EventId))
                                .ReturnsAsync(false);
            _joinProjectRepoMock.Setup(r => r.AddImplementerToProject(userId, (int)task.EventId))
                    .ReturnsAsync(true);
            _subTaskRepoMock.Setup(r => r.AssignSubTask(It.IsAny<JoinTask>()))
                            .ReturnsAsync(true);

            // Act
            var result = await _subTaskService.AssignSubTask(assignUserId, userId, subtaskId);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void AssignSubTask_SubTaskNotFound_ThrowsException()
        {
            // Arrange
            _subTaskRepoMock.Setup(r => r.GetSubTaskByIdAsync(It.IsAny<int>()))
                            .ReturnsAsync((SubTask)null);

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(() => _subTaskService.AssignSubTask(Guid.NewGuid(), Guid.NewGuid(), 1));
            Assert.AreEqual("Not found subtask!", ex.Message);
        }

        [Test]
        public void AssignSubTask_AssignerIsNotCreator_ThrowsException()
        {
            // Arrange
            var subtask = new SubTask { Id = 1, TaskId = 10 };
            var task = new TaskModel { Id = 10, CreateBy = Guid.NewGuid(), EventId = 2 };
            var assignerId = Guid.NewGuid(); // khác với CreateBy

            _subTaskRepoMock.Setup(r => r.GetSubTaskByIdAsync(1))
                            .ReturnsAsync(subtask);
            _taskRepoMock.Setup(r => r.GetTaskById(10))
                         .Returns(task);

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(() => _subTaskService.AssignSubTask(assignerId, Guid.NewGuid(), 1));
            Assert.AreEqual("Assign user must be create user!", ex.Message);
        }

        [Test]
        public async System.Threading.Tasks.Task AssignSubTask_UserAlreadyInProject_DoesNotCallAdd()
        {
            // Arrange
            var subtask = new SubTask { Id = 1, TaskId = 10 };
            var assignUserId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var task = new TaskModel { Id = 10, CreateBy = assignUserId, EventId = 3 };

            _subTaskRepoMock.Setup(r => r.GetSubTaskByIdAsync(1)).ReturnsAsync(subtask);
            _taskRepoMock.Setup(r => r.GetTaskById(10)).Returns(task);
            _joinProjectRepoMock.Setup(r => r.IsImplementerInProject(userId, 3)).ReturnsAsync(true);
            _subTaskRepoMock.Setup(r => r.AssignSubTask(It.IsAny<JoinTask>())).ReturnsAsync(true);

            // Act
            var result = await _subTaskService.AssignSubTask(assignUserId, userId, 1);

            // Assert
            Assert.IsTrue(result);
            _joinProjectRepoMock.Verify(r => r.AddImplementerToProject(It.IsAny<Guid>(), It.IsAny<int>()), Times.Never);
        }
    }

}
