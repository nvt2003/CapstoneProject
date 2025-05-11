using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Moq;
using Planify_BackEnd.DTOs.Events;
using Planify_BackEnd.Hub;
using Planify_BackEnd.Repositories.JoinGroups;
using Planify_BackEnd.Repositories.Tasks;
using Planify_BackEnd.Repositories;
using Planify_BackEnd.Services.GoogleDrive;
using Planify_BackEnd.Services.Notification;
using System.Runtime.Serialization;
using System.Security.Claims;
using Planify_BackEnd.Models;
using Microsoft.EntityFrameworkCore.Storage;

namespace Planify.Tests.Services.Test.EventServiceTest
{
    public class CreateSaveDraftTests
    {
        private Mock<IEventRepository> _eventRepoMock;
        private Mock<ITaskRepository> _taskRepoMock;
        private Mock<ISubTaskRepository> _subTaskRepoMock;
        private Mock<IHttpContextAccessor> _httpContextAccessorMock;
        private Mock<IHubContext<NotificationHub>> _hubContextMock;
        private Mock<IJoinProjectRepository> _joinProjectRepoMock;
        private Mock<IEmailSender> _emailSenderMock;
        private Mock<IUserRepository> _userRepoMock;
        private GoogleDriveService _googleDriveServiceStub;

        private EventService _service;
        private EventCreateRequestDTO _validEventDto;
        private Guid _organizerId;

        [SetUp]
        public void Setup()
        {
            _eventRepoMock = new Mock<IEventRepository>();
            _taskRepoMock = new Mock<ITaskRepository>();
            _subTaskRepoMock = new Mock<ISubTaskRepository>();
            _hubContextMock = new Mock<IHubContext<NotificationHub>>();
            _joinProjectRepoMock = new Mock<IJoinProjectRepository>();
            _emailSenderMock = new Mock<IEmailSender>();
            _userRepoMock = new Mock<IUserRepository>();
            _googleDriveServiceStub = (GoogleDriveService)FormatterServices.GetUninitializedObject(typeof(GoogleDriveService));
            var claims = new List<Claim>
            {
                new Claim("campusId", "1")
            };

            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            var mockHttpContext = new Mock<HttpContext>();
            mockHttpContext.Setup(c => c.User).Returns(claimsPrincipal);

            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            mockHttpContextAccessor.Setup(h => h.HttpContext).Returns(mockHttpContext.Object);

            _httpContextAccessorMock = mockHttpContextAccessor;

            _service = new EventService(
                _eventRepoMock.Object,
                _httpContextAccessorMock.Object,
                _googleDriveServiceStub,
                _subTaskRepoMock.Object,
                _taskRepoMock.Object,
                _hubContextMock.Object,
                _joinProjectRepoMock.Object,
                _emailSenderMock.Object,
                _userRepoMock.Object
            );

            _validEventDto = new EventCreateRequestDTO
            {
                EventTitle = "Sự kiện mẫu",
                EventDescription = "Mô tả sự kiện",
                StartTime = new DateTime(2025, 10, 10),
                EndTime = new DateTime(2025, 10, 11),
                CategoryEventId = 1,
                Placed = "Hội trường A",
                MeasuringSuccess = "Theo số người tham gia",
                Goals = "Tăng cường kỹ năng",
                MonitoringProcess = "Theo dõi định kỳ",
                SizeParticipants = 100,
                PromotionalPlan = "Đăng bài trên fanpage",
                TargetAudience = "Sinh viên CNTT",
                SloganEvent = "Học vui, chơi chất",
                Tasks = new List<TaskCreateEventDTO>
                {
                    new TaskCreateEventDTO
                    {
                        TaskName = "Chuẩn bị địa điểm",
                        Description = "Liên hệ đặt hội trường",
                        StartTime = new DateTime(2025, 08, 10),
                        Deadline = new DateTime(2025, 08, 11),
                        Budget = 1000000,
                        SubTasks = new List<SubTaskCreateEventDTO>
                        {
                            new SubTaskCreateEventDTO
                            {
                                SubTaskName = "Gọi điện đặt chỗ",
                                Description = "Gọi ban quản lý hội trường",
                                StartTime = new DateTime(2025, 08, 10),
                                Deadline = new DateTime(2025, 08, 11),
                                Budget = 200000
                            }
                        }
                    }
                },
                Risks = new List<RiskCreateEventDTO>
                {
                    new RiskCreateEventDTO
                    {
                        Name = "Trời mưa",
                        Reason = "Thời tiết xấu",
                        Solution = "Dự phòng hội trường khác",
                        Description = "Ảnh hưởng đến số người tham gia"
                    }
                },
                Activities = new List<ActivityEventDTO>
                {
                    new ActivityEventDTO
                    {
                        Name = "Giới thiệu",
                        Content = "Giới thiệu sự kiện và khách mời"
                    }
                },
                CostBreakdowns = new List<CostBreakdownCreateEventDTO>
                {
                    new CostBreakdownCreateEventDTO
                    {
                        Name = "Thuê hội trường",
                        Quantity = 1,
                        PriceByOne = 500000
                    }
                }
            };
            _organizerId = Guid.NewGuid();
        }
        [Test]
        public async System.Threading.Tasks.Task CreateSaveDraft_EventDto_IsNull_ShouldReturn400()
        {
            EventCreateRequestDTO nullDto = null;

            var result = await _service.CreateSaveDraft(nullDto, _organizerId);

            Assert.AreEqual(400, result.Status);
            Assert.AreEqual("Dữ liệu không hợp lệ", result.Message);
        }


        [Test]
        public async System.Threading.Tasks.Task CreateSaveDraft_EventTitle_IsEmpty_ShouldReturn400()
        {
            _validEventDto.EventTitle = string.Empty;

            var result = await _service.CreateSaveDraft(_validEventDto, _organizerId);

            Assert.AreEqual(400, result.Status);
            Assert.AreEqual("Tên sự kiện là bắt buộc.", result.Message);
        }
        [Test]
        public async System.Threading.Tasks.Task CreateSaveDraft_StartTime_TooClose_ShouldReturn400()
        {
            _validEventDto.StartTime = new DateTime(2025, 7, 8);
            _validEventDto.EndTime = new DateTime(2025, 7, 10);
            _eventRepoMock
                .Setup(repo => repo.GetCategoryEventAsync(1, 1))
                .ReturnsAsync(new CategoryEvent { Id = 1, CampusId = 1 });
            var mockTransaction = new Mock<IDbContextTransaction>();
            mockTransaction.Setup(t => t.CommitAsync(It.IsAny<CancellationToken>()))
                           .Returns(System.Threading.Tasks.Task.CompletedTask);
            mockTransaction.Setup(t => t.RollbackAsync(It.IsAny<CancellationToken>()))
                           .Returns(System.Threading.Tasks.Task.CompletedTask);

            _eventRepoMock.Setup(r => r.BeginTransactionAsync())
                          .ReturnsAsync(mockTransaction.Object);

            var result = await _service.CreateSaveDraft(_validEventDto, _organizerId);

            Assert.AreEqual(400, result.Status);
            Assert.AreEqual("Thời gian bắt đầu phải cách thời gian hiện tại ít nhất 2 tháng.", result.Message);
        }
        [Test]
        public async System.Threading.Tasks.Task CreateSaveDraft_StartTime_Valid_ShouldContinueProcessing()
        {
            _validEventDto.StartTime = new DateTime(2025, 7, 9);
            _validEventDto.EndTime = new DateTime(2025, 7, 10);

            _eventRepoMock
                .Setup(repo => repo.GetCategoryEventAsync(1, 1))
                .ReturnsAsync(new CategoryEvent { Id = 1, CampusId = 1 });
            var mockTransaction = new Mock<IDbContextTransaction>();
            mockTransaction.Setup(t => t.CommitAsync(It.IsAny<CancellationToken>()))
                           .Returns(System.Threading.Tasks.Task.CompletedTask);
            mockTransaction.Setup(t => t.RollbackAsync(It.IsAny<CancellationToken>()))
                           .Returns(System.Threading.Tasks.Task.CompletedTask);

            _eventRepoMock.Setup(r => r.BeginTransactionAsync())
                          .ReturnsAsync(mockTransaction.Object);
            var result = await _service.CreateSaveDraft(_validEventDto, _organizerId);

            Assert.AreEqual(201, result.Status);
            Assert.AreEqual("Lưu bản nháp thành công!", result.Message);
        }
        [Test]
        public async System.Threading.Tasks.Task CreateSaveDraft_StartTime_Equals_EndTime_ShouldReturn400()
        {
            _validEventDto.StartTime = new DateTime(2025, 07, 09);
            _validEventDto.EndTime = new DateTime(2025, 07, 09);

            var result = await _service.CreateSaveDraft(_validEventDto, _organizerId);

            Assert.AreEqual(400, result.Status);
            Assert.AreEqual("Thời gian bắt đầu phải sớm hơn thời gian kết thúc.", result.Message);
        }
        [Test]
        public async System.Threading.Tasks.Task CreateSaveDraft_SizeParticipants_Negative_ShouldReturn400()
        {
            _validEventDto.SizeParticipants = -1;

            var result = await _service.CreateSaveDraft(_validEventDto, _organizerId);

            Assert.AreEqual(400, result.Status);
            Assert.AreEqual("Số lượng người tham gia không thể âm.", result.Message);
        }
        [Test]
        public async System.Threading.Tasks.Task CreateSaveDraft_SizeParticipants_Zero_ShouldContinueProcessing()
        {
            _validEventDto.SizeParticipants = 0;

            _eventRepoMock
                .Setup(repo => repo.GetCategoryEventAsync(1, 1))
                .ReturnsAsync(new CategoryEvent { Id = 1, CampusId = 1 });
            var mockTransaction = new Mock<IDbContextTransaction>();
            mockTransaction.Setup(t => t.CommitAsync(It.IsAny<CancellationToken>()))
                           .Returns(System.Threading.Tasks.Task.CompletedTask);
            mockTransaction.Setup(t => t.RollbackAsync(It.IsAny<CancellationToken>()))
                           .Returns(System.Threading.Tasks.Task.CompletedTask);

            _eventRepoMock.Setup(r => r.BeginTransactionAsync())
                          .ReturnsAsync(mockTransaction.Object);
            var result = await _service.CreateSaveDraft(_validEventDto, _organizerId);

            Assert.AreEqual(201, result.Status);
            Assert.AreEqual("Lưu bản nháp thành công!", result.Message);
        }
        [Test]
        public async System.Threading.Tasks.Task CreateSaveDraft_InvalidCampusClaim_ShouldReturn400()
        {
            var mockHttpContext = new DefaultHttpContext();
            mockHttpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim("campusId", "abc")
            }));

            _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(mockHttpContext);

            var mockTransaction = new Mock<IDbContextTransaction>();
            mockTransaction.Setup(t => t.CommitAsync(It.IsAny<CancellationToken>()))
                           .Returns(System.Threading.Tasks.Task.CompletedTask);
            mockTransaction.Setup(t => t.RollbackAsync(It.IsAny<CancellationToken>()))
                           .Returns(System.Threading.Tasks.Task.CompletedTask);

            _eventRepoMock.Setup(r => r.BeginTransactionAsync())
                          .ReturnsAsync(mockTransaction.Object);

            // Act
            var result = await _service.CreateSaveDraft(_validEventDto, _organizerId);

            // Assert
            Assert.AreEqual(400, result.Status);
            Assert.AreEqual("ID campus không hợp lệ.", result.Message);
        }

        [Test]
        public async System.Threading.Tasks.Task TC007_CategoryNotFound_ShouldReturn400()
        {
            var mockHttpContext = new DefaultHttpContext();
            mockHttpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim("campusId", "1")
            }));

            _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(mockHttpContext);
            _eventRepoMock.Setup(repo => repo.GetCategoryEventAsync(It.IsAny<int>(), It.IsAny<int>()))
                          .ReturnsAsync((CategoryEvent?)null);

            var result = await _service.CreateSaveDraft(_validEventDto, _organizerId);

            Assert.AreEqual(400, result.Status);
            Assert.AreEqual("Danh mục không tồn tại hoặc không thuộc campus này.", result.Message);
        }
        [Test]
        public async System.Threading.Tasks.Task TC008_TaskMissingName_ShouldReturn400()
        {
            _validEventDto.Tasks[0].TaskName = null;

            var mockHttpContext = new DefaultHttpContext();
            mockHttpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
        new Claim("campusId", "1")
            }));
            _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(mockHttpContext);
            _eventRepoMock.Setup(r => r.GetCategoryEventAsync(It.IsAny<int>(), It.IsAny<int>()))
                          .ReturnsAsync(new CategoryEvent());

            var result = await _service.CreateSaveDraft(_validEventDto, _organizerId);

            Assert.AreEqual(400, result.Status);
            Assert.AreEqual("Tên task là bắt buộc.", result.Message);
        }
        [Test]
        public async System.Threading.Tasks.Task TC009_TaskDeadlineAfterToday_ShouldReturn400()
        {
            _validEventDto.Tasks[0].Deadline = new DateTime(2025,05,07);

            var mockHttpContext = new DefaultHttpContext();
            mockHttpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
        new Claim("campusId", "1")
            }));
            _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(mockHttpContext);
            _eventRepoMock.Setup(r => r.GetCategoryEventAsync(It.IsAny<int>(), It.IsAny<int>()))
                          .ReturnsAsync(new CategoryEvent());

            _eventRepoMock
                .Setup(repo => repo.GetCategoryEventAsync(1, 1))
                .ReturnsAsync(new CategoryEvent { Id = 1, CampusId = 1 });
            var mockTransaction = new Mock<IDbContextTransaction>();
            mockTransaction.Setup(t => t.CommitAsync(It.IsAny<CancellationToken>()))
                           .Returns(System.Threading.Tasks.Task.CompletedTask);
            mockTransaction.Setup(t => t.RollbackAsync(It.IsAny<CancellationToken>()))
                           .Returns(System.Threading.Tasks.Task.CompletedTask);

            _eventRepoMock.Setup(r => r.BeginTransactionAsync())
                          .ReturnsAsync(mockTransaction.Object);

            var result = await _service.CreateSaveDraft(_validEventDto, _organizerId);

            Assert.AreEqual(400, result.Status);
            Assert.AreEqual($"Deadline of task '{_validEventDto.Tasks[0].TaskName}' must be after now.", result.Message);
        }
        [Test]
        public async System.Threading.Tasks.Task TC010a_TaskBudgetNegative_ShouldReturn400()
        {
            _validEventDto.Tasks[0].Budget = -1000;

            var mockHttpContext = new DefaultHttpContext();
            mockHttpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim("campusId", "1")
            }));
            _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(mockHttpContext);
            _eventRepoMock.Setup(r => r.GetCategoryEventAsync(It.IsAny<int>(), It.IsAny<int>()))
                          .ReturnsAsync(new CategoryEvent());

            var result = await _service.CreateSaveDraft(_validEventDto, _organizerId);

            Assert.AreEqual(400, result.Status);
            Assert.AreEqual($"Ngân sách của task '{_validEventDto.Tasks[0].TaskName}' không thể âm.", result.Message);
        }
        [Test]
        public async System.Threading.Tasks.Task TC010b_TaskBudgetZero_ShouldContinue()
        {
            _validEventDto.Tasks[0].Budget = 0;

            var mockHttpContext = new DefaultHttpContext();
            mockHttpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim("campusId", "1")
            }));
            _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(mockHttpContext);
            _eventRepoMock.Setup(r => r.GetCategoryEventAsync(It.IsAny<int>(), It.IsAny<int>()))
                          .ReturnsAsync(new CategoryEvent());

            _eventRepoMock
                .Setup(repo => repo.GetCategoryEventAsync(1, 1))
                .ReturnsAsync(new CategoryEvent { Id = 1, CampusId = 1 });
            var mockTransaction = new Mock<IDbContextTransaction>();
            mockTransaction.Setup(t => t.CommitAsync(It.IsAny<CancellationToken>()))
                           .Returns(System.Threading.Tasks.Task.CompletedTask);
            mockTransaction.Setup(t => t.RollbackAsync(It.IsAny<CancellationToken>()))
                           .Returns(System.Threading.Tasks.Task.CompletedTask);

            _eventRepoMock.Setup(r => r.BeginTransactionAsync())
                          .ReturnsAsync(mockTransaction.Object);

            var result = await _service.CreateSaveDraft(_validEventDto, _organizerId);

            Assert.AreEqual(201, result.Status);
            Assert.AreEqual("Lưu bản nháp thành công!", result.Message);
        }
        [Test]
        public async System.Threading.Tasks.Task TC011_SubTaskMissingName_ShouldReturn400()
        {
            _validEventDto.Tasks[0].SubTasks[0].SubTaskName = "";

            _eventRepoMock.Setup(r => r.GetCategoryEventAsync(It.IsAny<int>(), It.IsAny<int>()))
                          .ReturnsAsync(new CategoryEvent());

            var result = await _service.CreateSaveDraft(_validEventDto, _organizerId);

            Assert.AreEqual(400, result.Status);
            Assert.AreEqual("Tên subtask là bắt buộc.", result.Message);
        }
        [Test]
        public async System.Threading.Tasks.Task TC012a_SubTaskStartAfterDeadline_ShouldReturn400()
        {
            _validEventDto.Tasks[0].SubTasks[0].Deadline = new DateTime(2025, 08, 9);

            _eventRepoMock.Setup(r => r.GetCategoryEventAsync(It.IsAny<int>(), It.IsAny<int>()))
                          .ReturnsAsync(new CategoryEvent());

            var result = await _service.CreateSaveDraft(_validEventDto, _organizerId);

            Assert.AreEqual(400, result.Status);
            Assert.AreEqual($"Thời gian bắt đầu của subtask '{_validEventDto.Tasks[0].SubTasks[0].SubTaskName}' phải sớm hơn thời hạn.", result.Message);
        }
        [Test]
        public async System.Threading.Tasks.Task TC012b_SubTaskStartEqualsDeadline_ShouldReturn400()
        {
            _validEventDto.Tasks[0].SubTasks[0].StartTime = new DateTime(2025, 08, 11);

            _eventRepoMock.Setup(r => r.GetCategoryEventAsync(It.IsAny<int>(), It.IsAny<int>()))
                          .ReturnsAsync(new CategoryEvent());

            var result = await _service.CreateSaveDraft(_validEventDto, _organizerId);

            Assert.AreEqual(400, result.Status);
            Assert.AreEqual($"Thời gian bắt đầu của subtask '{_validEventDto.Tasks[0].SubTasks[0].SubTaskName}' phải sớm hơn thời hạn.", result.Message);
        }
        [Test]
        public async System.Threading.Tasks.Task TC013a_SubTaskNegativeBudget_ShouldReturn400()
        {
            _validEventDto.Tasks[0].SubTasks[0].Budget = -1;

            _eventRepoMock.Setup(r => r.GetCategoryEventAsync(It.IsAny<int>(), It.IsAny<int>()))
                          .ReturnsAsync(new CategoryEvent());

            var result = await _service.CreateSaveDraft(_validEventDto, _organizerId);

            Assert.AreEqual(400, result.Status);
            Assert.AreEqual($"Ngân sách của subtask '{_validEventDto.Tasks[0].SubTasks[0].SubTaskName}' không thể âm.", result.Message);
        }
        [Test]
        public async System.Threading.Tasks.Task TC013b_SubTaskBudgetZero_ShouldPass()
        {
            _validEventDto.Tasks[0].SubTasks[0].Budget = 0;

            _eventRepoMock.Setup(r => r.GetCategoryEventAsync(It.IsAny<int>(), It.IsAny<int>()))
                          .ReturnsAsync(new CategoryEvent());

            _eventRepoMock
                .Setup(repo => repo.GetCategoryEventAsync(1, 1))
                .ReturnsAsync(new CategoryEvent { Id = 1, CampusId = 1 });
            var mockTransaction = new Mock<IDbContextTransaction>();
            mockTransaction.Setup(t => t.CommitAsync(It.IsAny<CancellationToken>()))
                           .Returns(System.Threading.Tasks.Task.CompletedTask);
            mockTransaction.Setup(t => t.RollbackAsync(It.IsAny<CancellationToken>()))
                           .Returns(System.Threading.Tasks.Task.CompletedTask);

            _eventRepoMock.Setup(r => r.BeginTransactionAsync())
                          .ReturnsAsync(mockTransaction.Object);

            var result = await _service.CreateSaveDraft(_validEventDto, _organizerId);

            Assert.AreEqual(201, result.Status);
            Assert.AreEqual("Lưu bản nháp thành công!", result.Message);
        }
        [Test]
        public async System.Threading.Tasks.Task TC014_RiskMissingName_ShouldReturn400()
        {
            _validEventDto.Risks[0].Name = null;

            _eventRepoMock.Setup(r => r.GetCategoryEventAsync(It.IsAny<int>(), It.IsAny<int>()))
                          .ReturnsAsync(new CategoryEvent());

            var result = await _service.CreateSaveDraft(_validEventDto, _organizerId);

            Assert.AreEqual(400, result.Status);
            Assert.AreEqual("Tên rủi ro là bắt buộc.", result.Message);
        }
        [Test]
        public async System.Threading.Tasks.Task TC015_ActivityMissingName_ShouldReturn400()
        {
            _validEventDto.Activities[0].Name = null;

            _eventRepoMock.Setup(r => r.GetCategoryEventAsync(It.IsAny<int>(), It.IsAny<int>()))
                          .ReturnsAsync(new CategoryEvent());

            var result = await _service.CreateSaveDraft(_validEventDto, _organizerId);

            Assert.AreEqual(400, result.Status);
            Assert.AreEqual("Tên hoạt động là bắt buộc.", result.Message);
        }
        [Test]
        public async System.Threading.Tasks.Task TC016_CostBreakdownMissingName_ShouldReturn400()
        {
            _validEventDto.CostBreakdowns[0].Name = null;

            _eventRepoMock.Setup(r => r.GetCategoryEventAsync(It.IsAny<int>(), It.IsAny<int>()))
                          .ReturnsAsync(new CategoryEvent());

            var result = await _service.CreateSaveDraft(_validEventDto, _organizerId);

            Assert.AreEqual(400, result.Status);
            Assert.AreEqual("Tên chi phí là bắt buộc.", result.Message);
        }
        [Test]
        public async System.Threading.Tasks.Task TC017a_CostBreakdownNegativeQuantity_ShouldReturn400()
        {
            _validEventDto.CostBreakdowns[0].Quantity = -1;

            _eventRepoMock.Setup(r => r.GetCategoryEventAsync(It.IsAny<int>(), It.IsAny<int>()))
                          .ReturnsAsync(new CategoryEvent());

            var result = await _service.CreateSaveDraft(_validEventDto, _organizerId);

            Assert.AreEqual(400, result.Status);
            Assert.AreEqual($"Số lượng của chi phí '{_validEventDto.CostBreakdowns[0].Name}' không thể âm.", result.Message);
        }
        [Test]
        public async System.Threading.Tasks.Task TC017b_CostBreakdownQuantityZero_ShouldPass()
        {
            _validEventDto.CostBreakdowns[0].Quantity = 0;

            _eventRepoMock.Setup(r => r.GetCategoryEventAsync(It.IsAny<int>(), It.IsAny<int>()))
                          .ReturnsAsync(new CategoryEvent());

            _eventRepoMock
                .Setup(repo => repo.GetCategoryEventAsync(1, 1))
                .ReturnsAsync(new CategoryEvent { Id = 1, CampusId = 1 });
            var mockTransaction = new Mock<IDbContextTransaction>();
            mockTransaction.Setup(t => t.CommitAsync(It.IsAny<CancellationToken>()))
                           .Returns(System.Threading.Tasks.Task.CompletedTask);
            mockTransaction.Setup(t => t.RollbackAsync(It.IsAny<CancellationToken>()))
                           .Returns(System.Threading.Tasks.Task.CompletedTask);

            _eventRepoMock.Setup(r => r.BeginTransactionAsync())
                          .ReturnsAsync(mockTransaction.Object);

            var result = await _service.CreateSaveDraft(_validEventDto, _organizerId);

            Assert.AreEqual(201, result.Status);
            Assert.AreEqual("Lưu bản nháp thành công!", result.Message);
        }
        [Test]
        public async System.Threading.Tasks.Task TC018a_CostBreakdownNegativePrice_ShouldReturn400()
        {
            _validEventDto.CostBreakdowns[0].PriceByOne = -1;

            _eventRepoMock.Setup(r => r.GetCategoryEventAsync(It.IsAny<int>(), It.IsAny<int>()))
                          .ReturnsAsync(new CategoryEvent());

            var result = await _service.CreateSaveDraft(_validEventDto, _organizerId);

            Assert.AreEqual(400, result.Status);
            Assert.AreEqual($"Đơn giá của chi phí '{_validEventDto.CostBreakdowns[0].Name}' không thể âm.", result.Message);
        }
        [Test]
        public async System.Threading.Tasks.Task TC018b_CostBreakdownPriceZero_ShouldPass()
        {
            _validEventDto.CostBreakdowns[0].PriceByOne = 0;

            _eventRepoMock.Setup(r => r.GetCategoryEventAsync(It.IsAny<int>(), It.IsAny<int>()))
                          .ReturnsAsync(new CategoryEvent());

            _eventRepoMock
                .Setup(repo => repo.GetCategoryEventAsync(1, 1))
                .ReturnsAsync(new CategoryEvent { Id = 1, CampusId = 1 });
            var mockTransaction = new Mock<IDbContextTransaction>();
            mockTransaction.Setup(t => t.CommitAsync(It.IsAny<CancellationToken>()))
                           .Returns(System.Threading.Tasks.Task.CompletedTask);
            mockTransaction.Setup(t => t.RollbackAsync(It.IsAny<CancellationToken>()))
                           .Returns(System.Threading.Tasks.Task.CompletedTask);

            _eventRepoMock.Setup(r => r.BeginTransactionAsync())
                          .ReturnsAsync(mockTransaction.Object);

            var result = await _service.CreateSaveDraft(_validEventDto, _organizerId);

            Assert.AreEqual(201, result.Status);
            Assert.AreEqual("Lưu bản nháp thành công!", result.Message);
        }
        [Test]
        public async System.Threading.Tasks.Task TC019_SaveDraftSuccess_ShouldReturn201()
        {
            _eventRepoMock.Setup(r => r.GetCategoryEventAsync(It.IsAny<int>(), It.IsAny<int>()))
                          .ReturnsAsync(new CategoryEvent());

            _eventRepoMock
                .Setup(repo => repo.GetCategoryEventAsync(1, 1))
                .ReturnsAsync(new CategoryEvent { Id = 1, CampusId = 1 });
            var mockTransaction = new Mock<IDbContextTransaction>();
            mockTransaction.Setup(t => t.CommitAsync(It.IsAny<CancellationToken>()))
                           .Returns(System.Threading.Tasks.Task.CompletedTask);
            mockTransaction.Setup(t => t.RollbackAsync(It.IsAny<CancellationToken>()))
                           .Returns(System.Threading.Tasks.Task.CompletedTask);

            _eventRepoMock.Setup(r => r.BeginTransactionAsync())
                          .ReturnsAsync(mockTransaction.Object);

            var result = await _service.CreateSaveDraft(_validEventDto, _organizerId);

            Assert.AreEqual(201, result.Status);
            Assert.AreEqual("Lưu bản nháp thành công!", result.Message);
            Assert.IsNotNull(result.Result);
        }
        [Test]
        public async System.Threading.Tasks.Task TC020_ExceptionWhileSaving_ShouldReturn500()
        {
            _eventRepoMock.Setup(r => r.GetCategoryEventAsync(It.IsAny<int>(), It.IsAny<int>()))
                          .ReturnsAsync(new CategoryEvent());

            _eventRepoMock.Setup(r => r.CreateEventAsync(It.IsAny<Event>()))
                          .ThrowsAsync(new Exception("Lỗi hệ thống"));

            var mockTransaction = new Mock<IDbContextTransaction>();
            mockTransaction.Setup(t => t.RollbackAsync(It.IsAny<CancellationToken>())).Returns(System.Threading.Tasks.Task.CompletedTask);
            _eventRepoMock.Setup(r => r.BeginTransactionAsync()).ReturnsAsync(mockTransaction.Object);

            var result = await _service.CreateSaveDraft(_validEventDto, _organizerId);

            Assert.AreEqual(500, result.Status);
            Assert.AreEqual("Đã xảy ra lỗi khi lưu bản nháp.", result.Message);
        }


    }
}
