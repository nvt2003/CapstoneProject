//using Newtonsoft.Json;
//using Newtonsoft.Json.Linq;
//using Planify_BackEnd.DTOs.Events;
//using Planify_BackEnd.DTOs;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Net;
//using System.Text;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.SignalR;
//using Moq;
//using Planify_BackEnd.DTOs.SendRequests;
//using Planify_BackEnd.Hub;
//using Planify_BackEnd.Repositories.SendRequests;
//using Planify_BackEnd.Repositories;
//using Planify_BackEnd.Services.EventRequests;
//using Planify_BackEnd.Services.Notification;

//namespace Planify.Tests.Services.Test.EventRequestServiceTest
//{
//    public class GetRequestsTest
//    {
//        private Mock<ISendRequestRepository> _mockRequestRepo;
//        private Mock<IEventRepository> _mockEventRepo;
//        private Mock<IHubContext<EventRequestHub>> _mockEventHubContext;
//        private Mock<IHubContext<NotificationHub>> _mockNotificationHubContext;
//        private Mock<IEmailSender> _mockEmailSender;
//        private Mock<IUserRepository> _mockUserRepo;

//        private SendRequestService _service;

//        [SetUp]
//        public void Setup()
//        {
//            _mockRequestRepo = new Mock<ISendRequestRepository>();
//            _mockEventRepo = new Mock<IEventRepository>();
//            _mockEventHubContext = new Mock<IHubContext<EventRequestHub>>();
//            _mockNotificationHubContext = new Mock<IHubContext<NotificationHub>>();
//            _mockEmailSender = new Mock<IEmailSender>();
//            _mockUserRepo = new Mock<IUserRepository>();

//            _service = new SendRequestService(
//                _mockRequestRepo.Object,
//                _mockEventRepo.Object,
//                _mockEventHubContext.Object,
//                _mockNotificationHubContext.Object,
//                _mockEmailSender.Object,
//                _mockUserRepo.Object
//            );
//        }

//        [Test]
//        public async Task GetRequestsAsync_ShouldReturnResponseDTOWithRequests()
//        {
//            var managerId = Guid.NewGuid();
//            var fakeRequests = new List<SendRequestWithEventDTO>
//            {
//                new SendRequestWithEventDTO
//                {
//                    Id = 1,
//                    EventId = 10,
//                    Reason = "Test Reason",
//                    Status = 1,
//                    ManagerId = managerId,
//                    EventTitle = "Test Event",
//                    EventStartTime = DateTime.Now,
//                    EventEndTime = DateTime.Now.AddHours(2)
//                }
//            };

//            _mockRequestRepo.Setup(repo => repo.GetRequestsAsync())
//                .ReturnsAsync(fakeRequests);

//            var result = await _service.GetRequestsAsync();

//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.Status);
//            Assert.AreEqual("Lấy danh sách yêu cầu thành công", result.Message);
//            Assert.IsInstanceOf<List<SendRequestWithEventDTO>>(result.Result);

//            var data = result.Result as List<SendRequestWithEventDTO>;
//            Assert.AreEqual(1, data.Count);
//            Assert.AreEqual("Test Event", data[0].EventTitle);
//        }
//        [Test]
//        public async Task GetRequestsAsync_ShouldReturnEmptyList_WhenNoRequestsExist()
//        {
//            var emptyList = new List<SendRequestWithEventDTO>();
//            _mockRequestRepo.Setup(repo => repo.GetRequestsAsync())
//                .ReturnsAsync(emptyList);

//            var result = await _service.GetRequestsAsync();

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(200, result.Status);
//            Assert.AreEqual("Lấy danh sách yêu cầu thành công", result.Message);
//            Assert.IsInstanceOf<List<SendRequestWithEventDTO>>(result.Result);

//            var data = result.Result as List<SendRequestWithEventDTO>;
//            Assert.IsNotNull(data);
//            Assert.AreEqual(0, data.Count);
//        }
//        [Test]
//        public void GetRequestsAsync_ShouldThrowException_WhenRepositoryFails()
//        {
//            _mockRequestRepo.Setup(repo => repo.GetRequestsAsync())
//                .ThrowsAsync(new Exception("Database connection error"));

//            var ex = Assert.ThrowsAsync<Exception>(async () => await _service.GetRequestsAsync());
//            Assert.AreEqual("Database connection error", ex.Message);
//        }

//    }
//}
