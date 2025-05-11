//using Moq;
//using NUnit.Framework;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.SignalR;
//using Planify_BackEnd.DTOs.Events;
//using Planify_BackEnd.Models;
//using Planify_BackEnd.Repositories;
//using Planify_BackEnd.Repositories.Tasks;
//using Planify_BackEnd.Repositories.JoinGroups;
//using Planify_BackEnd.Services.GoogleDrive;
//using Planify_BackEnd.Hub;
//using System.Security.Claims;
//using Microsoft.EntityFrameworkCore.Storage;
//using System.Threading.Tasks;
//using System.Runtime.Serialization;

//namespace Planify.Tests.Services.Test.EventServiceTest
//{
//    [TestFixture]
//    public class CreateEventTest
//    {
//        private Mock<IEventRepository> _eventRepositoryMock;
//        private Mock<ITaskRepository> _taskRepositoryMock;
//        private Mock<ISubTaskRepository> _subTaskRepositoryMock;
//        private Mock<IHttpContextAccessor> _httpContextAccessorMock;
//        private Mock<IHubContext<NotificationHub>> _hubContextMock;
//        private Mock<IJoinProjectRepository> _joinProjectRepositoryMock;
//        private GoogleDriveService _googleDriveServiceStub;
//        private EventService _eventService;

//        [SetUp]
//        public void Setup()
//        {
//            _eventRepositoryMock = new Mock<IEventRepository>();
//            _taskRepositoryMock = new Mock<ITaskRepository>();
//            _subTaskRepositoryMock = new Mock<ISubTaskRepository>();
//            _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
//            _hubContextMock = new Mock<IHubContext<NotificationHub>>();
//            _joinProjectRepositoryMock = new Mock<IJoinProjectRepository>();

//            _googleDriveServiceStub = (GoogleDriveService)FormatterServices.GetUninitializedObject(typeof(GoogleDriveService));

//            _eventService = new EventService(
//                _eventRepositoryMock.Object,
//                _httpContextAccessorMock.Object,
//                _googleDriveServiceStub,
//                _subTaskRepositoryMock.Object,
//                _taskRepositoryMock.Object,
//                _hubContextMock.Object,
//                _joinProjectRepositoryMock.Object
//            );
//        }

//        private void MockHttpContextWithCampusId(string campusId = "1")
//        {
//            var claims = new List<Claim> { new Claim("campusId", campusId) };
//            var identity = new ClaimsIdentity(claims, "TestAuthType");
//            var user = new ClaimsPrincipal(identity);
//            var httpContext = new DefaultHttpContext { User = user };

//            _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);
//        }

//        [Test]
//        public async System.Threading.Tasks.Task CreateEventAsync_EventDtoIsNull_ReturnsBadRequest()
//        {
//            var result = await _eventService.CreateEventAsync(null, Guid.NewGuid());

//            Assert.AreEqual(400, result.Status);
//            Assert.AreEqual("Dữ liệu không hợp lệ", result.Message);
//        }

//        [Test]
//        public async System.Threading.Tasks.Task CreateEventAsync_TitleIsEmpty_ReturnsBadRequest()
//        {
//            MockHttpContextWithCampusId();

//            var request = new EventCreateRequestDTO
//            {
//                EventTitle = "   ",
//                StartTime = DateTime.Parse("2025-07-01"),
//                EndTime = DateTime.Parse("2025-07-02"),
//                SizeParticipants = 10
//            };

//            var result = await _eventService.CreateEventAsync(request, Guid.NewGuid());

//            Assert.AreEqual(400, result.Status);
//            Assert.AreEqual("Tên sự kiện là bắt buộc.", result.Message);
//        }

//        [Test]
//        public async System.Threading.Tasks.Task CreateEventAsync_StartTimeExactlyTwoMonthsFromNow_ReturnsCreated()
//        {
//            MockHttpContextWithCampusId("1");

//            var startTime = DateTime.Parse("2025-06-24");
//            var endTime = DateTime.Parse("2025-07-02");

//            var request = new EventCreateRequestDTO
//            {
//                EventTitle = "Test",
//                StartTime = startTime,
//                EndTime = endTime,
//                SizeParticipants = 10,
//                CategoryEventId = 1,
//                CostBreakdowns = new List<CostBreakdownCreateEventDTO>()
//            };

//            var transactionMock = new Mock<IDbContextTransaction>();
//            _eventRepositoryMock.Setup(r => r.BeginTransactionAsync()).ReturnsAsync(transactionMock.Object);
//            _eventRepositoryMock.Setup(r => r.GetCategoryEventAsync(It.IsAny<int>(), It.IsAny<int>()))
//                .ReturnsAsync(new CategoryEvent());
//            _eventRepositoryMock.Setup(r => r.CreateEventAsync(It.IsAny<Event>()))
//                .ReturnsAsync(new Event { Id = 999, EventTitle = request.EventTitle });

//            var result = await _eventService.CreateEventAsync(request, Guid.NewGuid());

//            Assert.AreEqual(201, result.Status);
//            Assert.AreEqual("Tạo sự kiện thành công!", result.Message);
//        }

//        [Test]
//        public async System.Threading.Tasks.Task CreateEventAsync_StartTimeOneDayBeforeTwoMonths_ReturnsBadRequest()
//        {
//            MockHttpContextWithCampusId("1");

//            var now = DateTime.UtcNow;
//            var startTime = DateTime.Parse("2025-06-18");
//            var endTime = DateTime.Parse("2025-07-02");

//            var request = new EventCreateRequestDTO
//            {
//                EventTitle = "Test",
//                StartTime = startTime,
//                EndTime = endTime,
//                SizeParticipants = 10,
//                CategoryEventId = 1,
//                CostBreakdowns = new List<CostBreakdownCreateEventDTO>()
//            };

//            var result = await _eventService.CreateEventAsync(request, Guid.NewGuid());

//            Assert.AreEqual(400, result.Status);
//            Assert.AreEqual("Thời gian bắt đầu phải cách thời gian hiện tại ít nhất 2 tháng.", result.Message);
//        }


//        [Test]
//        public async System.Threading.Tasks.Task CreateEventAsync_StartAfterEndTime_ReturnsBadRequest()
//        {
//            MockHttpContextWithCampusId();

//            var request = new EventCreateRequestDTO
//            {
//                EventTitle = "Event",
//                StartTime = DateTime.Parse("2025-07-01"),
//                EndTime = DateTime.Parse("2025-06-30"),
//                SizeParticipants = 10
//            };

//            var result = await _eventService.CreateEventAsync(request, Guid.NewGuid());

//            Assert.AreEqual(400, result.Status);
//            Assert.AreEqual("Thời gian bắt đầu phải sớm hơn thời gian kết thúc.", result.Message);
//        }

//        [Test]
//        public async System.Threading.Tasks.Task CreateEventAsync_InvalidCampusId_ReturnsBadRequest()
//        {
//            MockHttpContextWithCampusId("abc");

//            var request = new EventCreateRequestDTO
//            {
//                EventTitle = "Test",
//                StartTime = DateTime.Parse("2025-07-01"),
//                EndTime = DateTime.Parse("2025-07-02"),
//                SizeParticipants = 10
//            };

//            var result = await _eventService.CreateEventAsync(request, Guid.NewGuid());

//            Assert.AreEqual(400, result.Status);
//            Assert.AreEqual("ID campus không hợp lệ.", result.Message);
//        }

//        [Test]
//        public async System.Threading.Tasks.Task CreateEventAsync_ValidInput_ReturnsCreated()
//        {
//            MockHttpContextWithCampusId("1");

//            var request = new EventCreateRequestDTO
//            {
//                EventTitle = "Test",
//                StartTime = DateTime.Parse("2025-07-01"),
//                EndTime = DateTime.Parse("2025-07-02"),
//                SizeParticipants = 10,
//                CategoryEventId = 1,
//                CostBreakdowns = new List<CostBreakdownCreateEventDTO>
//                {
//                    new CostBreakdownCreateEventDTO
//                    {
//                        Name = "Cost Item",
//                        Quantity = 2,
//                        PriceByOne = 100
//                    }
//                }
//            };

//            var transactionMock = new Mock<IDbContextTransaction>();
//            _eventRepositoryMock.Setup(r => r.BeginTransactionAsync()).ReturnsAsync(transactionMock.Object);
//            _eventRepositoryMock.Setup(r => r.GetCategoryEventAsync(It.IsAny<int>(), It.IsAny<int>()))
//                .ReturnsAsync(new CategoryEvent());
//            _eventRepositoryMock.Setup(r => r.CreateEventAsync(It.IsAny<Event>()))
//                .ReturnsAsync(new Event { Id = 123, EventTitle = request.EventTitle });
//            _eventRepositoryMock.Setup(r => r.CreateCostBreakdownAsync(It.IsAny<CostBreakdown>()))
//                .Returns(System.Threading.Tasks.Task.CompletedTask);

//            var result = await _eventService.CreateEventAsync(request, Guid.NewGuid());

//            Assert.AreEqual(201, result.Status);
//            Assert.AreEqual("Tạo sự kiện thành công!", result.Message);
//        }
//        [Test]
//        public async System.Threading.Tasks.Task CreateEventAsync_SizeParticipantsNegative_ReturnsBadRequest()
//        {
//            MockHttpContextWithCampusId("1");

//            var request = new EventCreateRequestDTO
//            {
//                EventTitle = "Test",
//                StartTime = DateTime.Parse("2025-07-01"),
//                EndTime = DateTime.Parse("2025-07-02"),
//                SizeParticipants = -1,
//                CategoryEventId = 1,
//                CostBreakdowns = new List<CostBreakdownCreateEventDTO>()
//            };

//            var result = await _eventService.CreateEventAsync(request, Guid.NewGuid());

//            Assert.AreEqual(400, result.Status);
//            Assert.AreEqual("Số lượng người tham gia không thể âm.", result.Message);
//        }

//        [Test]
//        public async System.Threading.Tasks.Task CreateEventAsync_SizeParticipantsZero_ReturnsCreated()
//        {
//            MockHttpContextWithCampusId("1");

//            var request = new EventCreateRequestDTO
//            {
//                EventTitle = "Test",
//                StartTime = DateTime.Parse("2025-07-01"),
//                EndTime = DateTime.Parse("2025-07-02"),
//                SizeParticipants = 0,
//                CategoryEventId = 1,
//                CostBreakdowns = new List<CostBreakdownCreateEventDTO>()
//            };

//            _eventRepositoryMock.Setup(r => r.BeginTransactionAsync()).ReturnsAsync(Mock.Of<IDbContextTransaction>());
//            _eventRepositoryMock.Setup(r => r.GetCategoryEventAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(new CategoryEvent());
//            _eventRepositoryMock.Setup(r => r.CreateEventAsync(It.IsAny<Event>())).ReturnsAsync(new Event());

//            var result = await _eventService.CreateEventAsync(request, Guid.NewGuid());

//            Assert.AreEqual(201, result.Status);
//            Assert.AreEqual("Tạo sự kiện thành công!", result.Message);
//        }

//        [Test]
//        public async System.Threading.Tasks.Task CreateEventAsync_SizeParticipantsMaxInt_ReturnsCreated()
//        {
//            MockHttpContextWithCampusId("1");

//            var request = new EventCreateRequestDTO
//            {
//                EventTitle = "Test",
//                StartTime = DateTime.Parse("2025-07-01"),
//                EndTime = DateTime.Parse("2025-07-02"),
//                SizeParticipants = int.MaxValue,
//                CategoryEventId = 1,
//                CostBreakdowns = new List<CostBreakdownCreateEventDTO>()
//            };

//            _eventRepositoryMock.Setup(r => r.BeginTransactionAsync()).ReturnsAsync(Mock.Of<IDbContextTransaction>());
//            _eventRepositoryMock.Setup(r => r.GetCategoryEventAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(new CategoryEvent());
//            _eventRepositoryMock.Setup(r => r.CreateEventAsync(It.IsAny<Event>())).ReturnsAsync(new Event());

//            var result = await _eventService.CreateEventAsync(request, Guid.NewGuid());

//            Assert.AreEqual(201, result.Status);
//            Assert.AreEqual("Tạo sự kiện thành công!", result.Message);
//        }

//        [Test]
//        public async System.Threading.Tasks.Task CreateEventAsync_EventTitleIsSingleCharacter_ReturnsCreated()
//        {
//            MockHttpContextWithCampusId("1");

//            var request = new EventCreateRequestDTO
//            {
//                EventTitle = "A",
//                StartTime = DateTime.Parse("2025-07-01"),
//                EndTime = DateTime.Parse("2025-07-02"),
//                SizeParticipants = 10,
//                CategoryEventId = 1,
//                CostBreakdowns = new List<CostBreakdownCreateEventDTO>()
//            };

//            _eventRepositoryMock.Setup(r => r.BeginTransactionAsync()).ReturnsAsync(Mock.Of<IDbContextTransaction>());
//            _eventRepositoryMock.Setup(r => r.GetCategoryEventAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(new CategoryEvent());
//            _eventRepositoryMock.Setup(r => r.CreateEventAsync(It.IsAny<Event>())).ReturnsAsync(new Event());

//            var result = await _eventService.CreateEventAsync(request, Guid.NewGuid());

//            Assert.AreEqual(201, result.Status);
//            Assert.AreEqual("Tạo sự kiện thành công!", result.Message);
//        }

//        [Test]
//        public async System.Threading.Tasks.Task CreateEventAsync_EventTitleIs255Characters_ReturnsCreated()
//        {
//            MockHttpContextWithCampusId("1");

//            var title = new string('A', 255);

//            var request = new EventCreateRequestDTO
//            {
//                EventTitle = title,
//                StartTime = DateTime.Parse("2025-07-01"),
//                EndTime = DateTime.Parse("2025-07-02"),
//                SizeParticipants = 10,
//                CategoryEventId = 1,
//                CostBreakdowns = new List<CostBreakdownCreateEventDTO>()
//            };

//            _eventRepositoryMock.Setup(r => r.BeginTransactionAsync()).ReturnsAsync(Mock.Of<IDbContextTransaction>());
//            _eventRepositoryMock.Setup(r => r.GetCategoryEventAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(new CategoryEvent());
//            _eventRepositoryMock.Setup(r => r.CreateEventAsync(It.IsAny<Event>())).ReturnsAsync(new Event());

//            var result = await _eventService.CreateEventAsync(request, Guid.NewGuid());

//            Assert.AreEqual(201, result.Status);
//            Assert.AreEqual("Tạo sự kiện thành công!", result.Message);
//        }

//    }
//}
