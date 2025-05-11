using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Moq;
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

namespace Planify.Tests.Services.Test.SubtaskServiceTest
{
    [TestFixture]
    public class SubTaskService_DeleteTests
    {
        private Mock<ISubTaskRepository> _subTaskRepoMock;
        private SubTaskService _subTaskService;

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
        }

        [Test]
        public async Task DeleteSubTaskAsync_DeletedSuccessfully_Returns200()
        {
            // Arrange
            int subTaskId = 1;
            _subTaskRepoMock.Setup(r => r.DeleteSubTaskAsync(subTaskId))
                            .ReturnsAsync(true);

            // Act
            var result = await _subTaskService.DeleteSubTaskAsync(subTaskId);

            // Assert
            Assert.AreEqual(200, result.Status);
            Assert.AreEqual("Sub-task deleted successfully!", result.Message);
            Assert.IsNull(result.Result);
        }

        [Test]
        public async Task DeleteSubTaskAsync_SubTaskNotFound_Returns404()
        {
            // Arrange
            int subTaskId = 999;
            _subTaskRepoMock.Setup(r => r.DeleteSubTaskAsync(subTaskId))
                            .ReturnsAsync(false);

            // Act
            var result = await _subTaskService.DeleteSubTaskAsync(subTaskId);

            // Assert
            Assert.AreEqual(404, result.Status);
            Assert.AreEqual("Sub-task not found.", result.Message);
            Assert.IsNull(result.Result);
        }

        [Test]
        public async Task DeleteSubTaskAsync_RepositoryThrowsException_Returns500()
        {
            // Arrange
            int subTaskId = 2;
            _subTaskRepoMock.Setup(r => r.DeleteSubTaskAsync(subTaskId))
                            .ThrowsAsync(new Exception("DB error"));

            // Act
            var result = await _subTaskService.DeleteSubTaskAsync(subTaskId);

            // Assert
            Assert.AreEqual(500, result.Status);
            Assert.AreEqual("Error occurs while deleting sub-task!", result.Message);
            Assert.AreEqual("DB error", result.Result);
        }
    }

}
