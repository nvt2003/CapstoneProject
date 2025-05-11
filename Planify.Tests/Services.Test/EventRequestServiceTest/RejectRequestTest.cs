using Moq;
using Planify_BackEnd.Services.EventRequests;
using Planify_BackEnd.Models;
using Microsoft.AspNetCore.SignalR;
using Planify_BackEnd.Repositories;
using Planify_BackEnd.Hub;
using Planify_BackEnd.Repositories.SendRequests;
using Planify_BackEnd.Services.Notification;

namespace Planify.Tests.Services.Test.EventRequestServiceTest
{
    public class RejectRequestServiceTests
    {
        private Mock<ISendRequestRepository> _mockRequestRepo;
        private Mock<IEventRepository> _mockEventRepo;
        private Mock<IUserRepository> _mockUserRepo;
        private Mock<IEmailSender> _mockEmailSender;
        private Mock<IHubContext<EventRequestHub>> _mockEventHub;
        private Mock<IHubContext<NotificationHub>> _mockNotificationHub;

        private ISendRequestService _service;

        [SetUp]
        public void Setup()
        {
            _mockRequestRepo = new Mock<ISendRequestRepository>();
            _mockEventRepo = new Mock<IEventRepository>();
            _mockUserRepo = new Mock<IUserRepository>();
            _mockEmailSender = new Mock<IEmailSender>();
            _mockEventHub = new Mock<IHubContext<EventRequestHub>>();
            _mockNotificationHub = new Mock<IHubContext<NotificationHub>>();

            _service = new SendRequestService(
                _mockRequestRepo.Object,
                _mockEventRepo.Object,
                _mockEventHub.Object,
                _mockNotificationHub.Object,
                _mockEmailSender.Object,
                _mockUserRepo.Object
            );
        }
        [Test]
        public async System.Threading.Tasks.Task RejectRequestAsync_IdIsInvalid_ReturnsBadRequest()
        {
            var result = await _service.RejectRequestAsync(0, Guid.NewGuid(), "Reason");
            Assert.AreEqual(400, result.Status);
            Assert.AreEqual("ID không hợp lệ", result.Message);
        }

        [Test]
        public async System.Threading.Tasks.Task RejectRequestAsync_ManagerIdIsEmpty_ReturnsBadRequest()
        {
            var result = await _service.RejectRequestAsync(1, Guid.Empty, "Reason");
            Assert.AreEqual(400, result.Status);
            Assert.AreEqual("Manager ID không hợp lệ", result.Message);
        }

        [Test]
        public async System.Threading.Tasks.Task RejectRequestAsync_RequestNotFound_ReturnsNotFound()
        {
            _mockRequestRepo.Setup(r => r.GetRequestByIdAsync(1)).ReturnsAsync((SendRequest?)null);
            var result = await _service.RejectRequestAsync(1, Guid.NewGuid(), "Reason");

            Assert.AreEqual(404, result.Status);
            Assert.AreEqual("Không tìm thấy yêu cầu", result.Message);
        }

        [Test]
        public async System.Threading.Tasks.Task RejectRequestAsync_EventNotFound_ReturnsNotFound()
        {
            var request = new SendRequest
            {
                Id = 1,
                EventId = 123,
                Event = new Event { Status = 1 }
            };
            _mockRequestRepo.Setup(r => r.GetRequestByIdAsync(1)).ReturnsAsync(request);
            _mockEventRepo.Setup(e => e.GetEventByIdAsync(123)).ReturnsAsync((Event?)null);

            var result = await _service.RejectRequestAsync(1, Guid.NewGuid(), "Reason");

            Assert.AreEqual(404, result.Status);
            Assert.AreEqual("Không tìm thấy sự kiện", result.Message);
        }

        [Test]
        public async System.Threading.Tasks.Task RejectRequestAsync_EventInvalidStatus_ReturnsBadRequest()
        {
            var request = new SendRequest
            {
                Id = 1,
                EventId = 123,
                Event = new Event { Status = 0 }
            };
            _mockRequestRepo.Setup(r => r.GetRequestByIdAsync(1)).ReturnsAsync(request);
            _mockEventRepo.Setup(e => e.GetEventByIdAsync(123)).ReturnsAsync(new Event { Status = 0 });

            var result = await _service.RejectRequestAsync(1, Guid.NewGuid(), "Reason");

            Assert.AreEqual(400, result.Status);
            Assert.AreEqual("Yêu cầu không thể bị từ chối vì không ở trạng thái chờ duyệt", result.Message);
        }

        [Test]
        public async System.Threading.Tasks.Task RejectRequestAsync_ValidRequest_ReturnsSuccess()
        {
            var managerId = Guid.NewGuid();
            var request = new SendRequest
            {
                Id = 1,
                EventId = 123,
                Status = 0,
                Event = new Event
                {
                    Id = 123,
                    Status = 1,
                    CreateBy = Guid.NewGuid()
                }
            };
            var eventEntity = new Event
            {
                Id = 123,
                Status = 1,
                CreateBy = Guid.NewGuid()
            };

            _mockRequestRepo.Setup(r => r.GetRequestByIdAsync(1)).ReturnsAsync(request);
            _mockEventRepo.Setup(e => e.GetEventByIdAsync(123)).ReturnsAsync(eventEntity);
            _mockRequestRepo.Setup(r => r.UpdateRequestAsync(It.IsAny<SendRequest>())).Returns(System.Threading.Tasks.Task.CompletedTask);
            _mockEventRepo.Setup(r => r.UpdateEventAsync(It.IsAny<Event>()))
              .ReturnsAsync(It.IsAny<Event>());

            var mockClientProxy = new Mock<IClientProxy>();
            var mockClients = new Mock<IHubClients>();
            mockClients.Setup(c => c.All).Returns(mockClientProxy.Object);
            _mockEventHub.Setup(h => h.Clients).Returns(mockClients.Object);
            mockClientProxy
                .As<IClientProxy>()
                .Setup(x => x.SendCoreAsync(It.IsAny<string>(), It.IsAny<object[]>(), default))
                .Returns(System.Threading.Tasks.Task.CompletedTask);

            var result = await _service.RejectRequestAsync(1, managerId, "Lý do từ chối");

            Assert.AreEqual(200, result.Status);
            Assert.AreEqual("Yêu cầu đã bị từ chối", result.Message);
        }
    }
}
