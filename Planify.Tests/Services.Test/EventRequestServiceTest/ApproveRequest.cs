using Moq;
using NUnit.Framework;
using System;
using System.Threading.Tasks;
using Planify_BackEnd.Services.EventRequests;
using Planify_BackEnd.Models;
using Microsoft.AspNetCore.SignalR;
using Planify_BackEnd.Repositories;
using Planify_BackEnd.Hub;
using Planify_BackEnd.Repositories.SendRequests;
using Planify_BackEnd.Services.Notification;

namespace Planify.Tests.Services.Test.EventRequestServiceTest
{
    [TestFixture]
    public class ApproveRequestServiceTests
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
        public async System.Threading.Tasks.Task ApproveRequestAsync_ShouldReturnBadRequest_WhenInvalidId()
        {
            var result = await _service.ApproveRequestAsync(0, Guid.NewGuid(), "reason");
            Assert.That(result.Status, Is.EqualTo(400));
            Assert.That(result.Message, Is.EqualTo("ID không hợp lệ"));
        }

        [Test]
        public async System.Threading.Tasks.Task ApproveRequestAsync_ShouldReturnBadRequest_WhenInvalidManagerId()
        {
            var result = await _service.ApproveRequestAsync(1, Guid.Empty, "reason");
            Assert.That(result.Status, Is.EqualTo(400));
            Assert.That(result.Message, Is.EqualTo("Manager ID không hợp lệ"));
        }

        [Test]
        public async System.Threading.Tasks.Task ApproveRequestAsync_ShouldReturnNotFound_WhenRequestNotFound()
        {
            _mockRequestRepo.Setup(r => r.GetRequestByIdAsync(It.IsAny<int>()))
                            .ReturnsAsync((SendRequest)null!);

            var result = await _service.ApproveRequestAsync(1, Guid.NewGuid(), "reason");
            Assert.That(result.Status, Is.EqualTo(404));
            Assert.That(result.Message, Is.EqualTo("Không tìm thấy yêu cầu"));
        }

        [Test]
        public async System.Threading.Tasks.Task ApproveRequestAsync_ShouldReturnBadRequest_WhenRequestStatusInvalid()
        {
            var req = new SendRequest
            {
                Event = new Event { Status = 0 },
                EventId = 1
            };
            _mockRequestRepo.Setup(r => r.GetRequestByIdAsync(It.IsAny<int>()))
                            .ReturnsAsync(req);

            var result = await _service.ApproveRequestAsync(1, Guid.NewGuid(), "reason");
            Assert.That(result.Status, Is.EqualTo(400));
            Assert.That(result.Message, Is.EqualTo("Yêu cầu không thể được duyệt vì không ở trạng thái chờ duyệt"));
        }

        [Test]
        public async System.Threading.Tasks.Task ApproveRequestAsync_ShouldReturnNotFound_WhenEventNotFound()
        {
            var req = new SendRequest
            {
                Event = new Event { Status = 1 },
                EventId = 1
            };
            _mockRequestRepo.Setup(r => r.GetRequestByIdAsync(It.IsAny<int>()))
                            .ReturnsAsync(req);
            _mockEventRepo.Setup(e => e.GetEventByIdAsync(1))
                          .ReturnsAsync((Event)null!);

            var result = await _service.ApproveRequestAsync(1, Guid.NewGuid(), "reason");
            Assert.That(result.Status, Is.EqualTo(404));
            Assert.That(result.Message, Is.EqualTo("Không tìm thấy sự kiện"));
        }

        [Test]
        public async System.Threading.Tasks.Task ApproveRequestAsync_ShouldReturnBadRequest_WhenEventStatusInvalid()
        {
            var req = new SendRequest
            {
                Event = new Event { Status = 1 },
                EventId = 1
            };
            _mockRequestRepo.Setup(r => r.GetRequestByIdAsync(It.IsAny<int>()))
                            .ReturnsAsync(req);
            _mockEventRepo.Setup(e => e.GetEventByIdAsync(1))
                          .ReturnsAsync(new Event { Status = 0 });

            var result = await _service.ApproveRequestAsync(1, Guid.NewGuid(), "reason");
            Assert.That(result.Status, Is.EqualTo(400));
            Assert.That(result.Message, Is.EqualTo("Sự kiện không thể được duyệt do trạng thái hiện tại"));
        }

        [Test]
        public async System.Threading.Tasks.Task ApproveRequestAsync_ShouldReturnOk_WhenValid()
        {
            var managerId = Guid.NewGuid();
            var eventEntity = new Event
            {
                Status = 1,
                IsPublic = 0,
                EventTitle = "Test",
                CreateBy = Guid.NewGuid()
            };

            var req = new SendRequest
            {
                Id = 1,
                EventId = 10,
                Event = new Event { Status = 1 },
                Status = 0
            };

            _mockRequestRepo.Setup(r => r.GetRequestByIdAsync(req.Id)).ReturnsAsync(req);
            _mockEventRepo.Setup(e => e.GetEventByIdAsync(req.EventId)).ReturnsAsync(eventEntity);
            _mockRequestRepo.Setup(r => r.UpdateRequestAsync(It.IsAny<SendRequest>()))
            .Returns(System.Threading.Tasks.Task.CompletedTask);
            _mockEventRepo.Setup(e => e.UpdateEventAsync(It.IsAny<Event>())).ReturnsAsync(eventEntity);

            var mockClientProxy = new Mock<IClientProxy>();
            var mockClients = new Mock<IHubClients>();
            mockClients.Setup(c => c.All).Returns(mockClientProxy.Object);
            _mockEventHub.Setup(h => h.Clients).Returns(mockClients.Object);
            mockClientProxy
                .As<IClientProxy>()
                .Setup(x => x.SendCoreAsync(It.IsAny<string>(), It.IsAny<object[]>(), default))
                .Returns(System.Threading.Tasks.Task.CompletedTask);

            var result = await _service.ApproveRequestAsync(req.Id, managerId, "Lý do duyệt");
            Assert.That(result.Status, Is.EqualTo(200));
            Assert.That(result.Message, Is.EqualTo("Yêu cầu đã được duyệt"));
        }
    }
}
