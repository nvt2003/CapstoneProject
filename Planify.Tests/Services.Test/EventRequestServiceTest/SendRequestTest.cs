using Microsoft.AspNetCore.SignalR;
using Moq;
using Planify_BackEnd.DTOs.SendRequests;
using Planify_BackEnd.Hub;
using Planify_BackEnd.Models;
using Planify_BackEnd.Repositories.SendRequests;
using Planify_BackEnd.Repositories;
using Planify_BackEnd.Services.EventRequests;
using Planify_BackEnd.Services.Notification;

namespace Planify.Tests.Services.Test.EventRequestServiceTest;
[TestFixture]
public class SendRequestServiceTests
{
    private Mock<ISendRequestRepository> _mockRequestRepo;
    private Mock<IEventRepository> _mockEventRepo;
    private Mock<IHubContext<EventRequestHub>> _mockEventHubContext;
    private Mock<IClientProxy> _mockClientProxy;
    private Mock<IHubClients> _mockClients;
    private Mock<IHubContext<NotificationHub>> _mockNotificationHubContext;
    private Mock<IEmailSender> _mockEmailSender;
    private Mock<IUserRepository> _mockUserRepo;

    private SendRequestService _service;

    [SetUp]
    public void Setup()
    {
        _mockRequestRepo = new Mock<ISendRequestRepository>();
        _mockEventRepo = new Mock<IEventRepository>();
        _mockEventHubContext = new Mock<IHubContext<EventRequestHub>>();
        _mockNotificationHubContext = new Mock<IHubContext<NotificationHub>>();
        _mockEmailSender = new Mock<IEmailSender>();
        _mockUserRepo = new Mock<IUserRepository>();

        _mockClients = new Mock<IHubClients>();
        _mockClientProxy = new Mock<IClientProxy>();
        _mockClients.Setup(c => c.All).Returns(_mockClientProxy.Object);
        _mockEventHubContext.Setup(h => h.Clients).Returns(_mockClients.Object);

        _service = new SendRequestService(
            _mockRequestRepo.Object,
            _mockEventRepo.Object,
            _mockEventHubContext.Object,
            _mockNotificationHubContext.Object,
            _mockEmailSender.Object,
            _mockUserRepo.Object
        );
    }

    [Test]
    public async System.Threading.Tasks.Task CreateRequestAsync_ShouldReturnBadRequest_WhenRequestIsNull()
    {
        var result = await _service.CreateRequestAsync(null);
        Assert.AreEqual(400, result.Status);
        Assert.AreEqual("Dữ liệu không hợp lệ", result.Message);
    }

    [Test]
    public async System.Threading.Tasks.Task CreateRequestAsync_ShouldReturnBadRequest_WhenEventIdInvalid()
    {
        var result = await _service.CreateRequestAsync(new SendRequestDTO { EventId = 0 });
        Assert.AreEqual(400, result.Status);
        Assert.AreEqual("Event ID không hợp lệ", result.Message);
    }

    [Test]
    public async System.Threading.Tasks.Task CreateRequestAsync_ShouldReturnNotFound_WhenEventNotFound()
    {
        _mockEventRepo.Setup(r => r.GetEventByIdAsync(111111)).ReturnsAsync((Event)null!);

        var result = await _service.CreateRequestAsync(new SendRequestDTO { EventId = 111111 });

        Assert.AreEqual(404, result.Status);
        Assert.AreEqual("Không tìm thấy sự kiện", result.Message);
    }

    [Test]
    public async System.Threading.Tasks.Task CreateRequestAsync_ShouldReturnBadRequest_WhenEventStatusIsNot0()
    {
        _mockEventRepo.Setup(r => r.GetEventByIdAsync(2))
            .ReturnsAsync(new Event { Id = 2, Status = 1 });

        var result = await _service.CreateRequestAsync(new SendRequestDTO { EventId = 2 });

        Assert.AreEqual(400, result.Status);
        Assert.AreEqual("Sự kiện không thể tạo yêu cầu mới do trạng thái hiện tại", result.Message);
    }

    [Test]
    public async System.Threading.Tasks.Task CreateRequestAsync_ShouldReturnBadRequest_WhenPendingRequestExists()
    {
        var eventObj = new Event { Id = 3, Status = 0 };
        _mockEventRepo.Setup(r => r.GetEventByIdAsync(3)).ReturnsAsync(eventObj);
        _mockRequestRepo.Setup(r => r.GetRequestByIdAsync(3))
            .ReturnsAsync(new SendRequest { Id = 99, EventId = 3, Status = 0 }); // Chờ duyệt

        var result = await _service.CreateRequestAsync(new SendRequestDTO { EventId = 3 });

        Assert.AreEqual(400, result.Status);
        Assert.AreEqual("Sự kiện đã có yêu cầu đang chờ duyệt", result.Message);
    }

    [Test]
    public async System.Threading.Tasks.Task CreateRequestAsync_ShouldReturnSuccess_WhenValidRequest()
    {
        var eventObj = new Event { Id = 1, Status = 0 };

        _mockEventRepo.Setup(r => r.GetEventByIdAsync(1)).ReturnsAsync(eventObj);
        _mockRequestRepo.Setup(r => r.GetRequestByIdAsync(1)).ReturnsAsync((SendRequest)null!);
        _mockRequestRepo.Setup(r => r.CreateRequestAsync(It.IsAny<SendRequest>()))
            .ReturnsAsync((SendRequest r) => r);

        _mockEventRepo.Setup(r => r.UpdateEventAsync(It.IsAny<Event>()))
        .ReturnsAsync((Event e) => e);

        _mockClientProxy.Setup(c => c.SendCoreAsync(
                "ReceiveEventRequest",
                It.IsAny<object[]>(),
                default))
            .Returns(System.Threading.Tasks.Task.CompletedTask);

        var result = await _service.CreateRequestAsync(new SendRequestDTO { EventId = 1 });

        Assert.AreEqual(201, result.Status);
        Assert.AreEqual("Tạo yêu cầu thành công", result.Message);
        Assert.IsNotNull(result.Result);
    }

    [Test]
    public async System.Threading.Tasks.Task CreateRequestAsync_ShouldReturnServerError_WhenCreateFails()
    {
        var eventObj = new Event { Id = 1, Status = 0 };

        _mockEventRepo.Setup(r => r.GetEventByIdAsync(1)).ReturnsAsync(eventObj);
        _mockRequestRepo.Setup(r => r.GetRequestByIdAsync(1)).ReturnsAsync((SendRequest)null!);
        _mockRequestRepo.Setup(r => r.CreateRequestAsync(It.IsAny<SendRequest>()))
            .ReturnsAsync((SendRequest)null!); // Fail to create

        var result = await _service.CreateRequestAsync(new SendRequestDTO { EventId = 1 });

        Assert.AreEqual(500, result.Status);
        Assert.AreEqual("Không thể tạo yêu cầu", result.Message);
    }

    [Test]
    public async System.Threading.Tasks.Task CreateRequestAsync_ShouldReturnServerError_WhenExceptionOccurs()
    {
        _mockEventRepo.Setup(r => r.GetEventByIdAsync(It.IsAny<int>()))
            .ThrowsAsync(new Exception("DB error"));

        var result = await _service.CreateRequestAsync(new SendRequestDTO { EventId = 1 });

        Assert.AreEqual(500, result.Status);
        Assert.AreEqual("Đã xảy ra lỗi khi tạo yêu cầu", result.Message);
    }
}
