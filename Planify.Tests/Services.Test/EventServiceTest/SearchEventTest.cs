//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.SignalR;
//using Moq;
//using NUnit.Framework;
//using Planify_BackEnd.DTOs;
//using Planify_BackEnd.DTOs.Events;
//using Planify_BackEnd.Hub;
//using Planify_BackEnd.Models;
//using Planify_BackEnd.Repositories;
//using Planify_BackEnd.Repositories.JoinGroups;
//using Planify_BackEnd.Repositories.Tasks;
//using Planify_BackEnd.Services;
//using Planify_BackEnd.Services.GoogleDrive;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Runtime.Serialization;
//using System.Threading.Tasks;

//namespace Planify.Tests.Services.Test.EventServiceTest
//{
//    public class SearchEventUnitTest
//    {
//        private Mock<IEventRepository> _mockEventRepository;
//        private Mock<IHttpContextAccessor> _mockHttpContextAccessor;
//        private GoogleDriveService _googleDriveServiceStub;
//        private Mock<ISubTaskRepository> _mockSubTaskRepository;
//        private Mock<ITaskRepository> _mockTaskRepository;
//        private Mock<IHubContext<NotificationHub>> _mockHubContext;
//        private Mock<IJoinProjectRepository> _mockJoinProjectRepository;
//        private EventService _eventService;

//        [SetUp]
//        public void Setup()
//        {
//            _mockEventRepository = new Mock<IEventRepository>();
//            _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
//            _googleDriveServiceStub = (GoogleDriveService)FormatterServices.GetUninitializedObject(typeof(GoogleDriveService));
//            _mockSubTaskRepository = new Mock<ISubTaskRepository>();
//            _mockTaskRepository = new Mock<ITaskRepository>();
//            _mockHubContext = new Mock<IHubContext<NotificationHub>>();
//            _mockJoinProjectRepository = new Mock<IJoinProjectRepository>();

//            _eventService = new EventService(
//                _mockEventRepository.Object,
//                _mockHttpContextAccessor.Object,
//                _googleDriveServiceStub,
//                _mockSubTaskRepository.Object,
//                _mockTaskRepository.Object,
//                _mockHubContext.Object,
//                _mockJoinProjectRepository.Object
//            );
//        }

//        [Test]
//        public async System.Threading.Tasks.Task SearchEventAsync_ReturnsMappedEventList_WhenEventsExist()
//        {
//            var sampleEvent = new Event
//            {
//                Id = 1,
//                EventTitle = "Tech Talk",
//                EventDescription = "Discussion on .NET",
//                StartTime = DateTime.Now,
//                EndTime = DateTime.Now.AddHours(2),
//                AmountBudget = 1000,
//                IsPublic = 1,
//                Status = 2,
//                CampusId = 1,
//                CategoryEventId = 1,
//                Placed = "Hall A",
//                CreateBy = Guid.NewGuid(),
//                CreatedAt = DateTime.Now,
//                ManagerId = Guid.NewGuid(),
//                EventMedia = new List<EventMedium>
//                {
//                    new EventMedium
//                    {
//                        Id = 1,
//                        MediaId = 1,
//                        Status = 1,
//                        Media = new Medium
//                        {
//                            Id = 1,
//                            MediaUrl = "http://image.url"
//                        }
//                    }
//                },
//                FavouriteEvents = new List<FavouriteEvent> { new FavouriteEvent() }
//            };
//            var pageResult = new PageResultDTO<Event>
//            (
//                new List<Event> { sampleEvent },
//                totalCount: 1,
//                pageNumber: 1,
//                pageSize: 5
//            );

//            _mockEventRepository.Setup(repo => repo.SearchEventAsync(
//                It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(),
//                It.IsAny<DateTime?>(), It.IsAny<DateTime?>(),
//                It.IsAny<decimal?>(), It.IsAny<decimal?>(),
//                It.IsAny<int?>(), It.IsAny<int?>(),
//                It.IsAny<int?>(), It.IsAny<string>(),
//                It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<Guid?>()
//            )).ReturnsAsync(pageResult);
//            var result = await _eventService.SearchEventAsync(1, 5, null, null, null, null, null, null, null, null, null, Guid.NewGuid(), 1, null);

//            Assert.IsNotNull(result);
//            Assert.That(result.Items.Count, Is.EqualTo(1));
//            var ev = result.Items.First();
//            Assert.That(ev.EventTitle, Is.EqualTo("Tech Talk"));
//            Assert.That(ev.EventMedias.Count, Is.EqualTo(1));
//            Assert.That(ev.isFavorite, Is.True);
//            Assert.That(result.TotalCount, Is.EqualTo(1));
//        }

//        [Test]
//        public void SearchEventAsync_ThrowsException_WhenRepositoryFails()
//        {
//            _mockEventRepository.Setup(repo => repo.SearchEventAsync(
//                It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(),
//                It.IsAny<DateTime?>(), It.IsAny<DateTime?>(),
//                It.IsAny<decimal?>(), It.IsAny<decimal?>(),
//                It.IsAny<int?>(), It.IsAny<int?>(),
//                It.IsAny<int?>(), It.IsAny<string>(),
//                It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<Guid?>()
//            )).ThrowsAsync(new Exception("DB failed"));

//            var ex = Assert.ThrowsAsync<Exception>(async () =>
//                await _eventService.SearchEventAsync(1, 5, null, null, null, null, null, null, 2, null, null, Guid.NewGuid(), 1, null)
//            );
//            Assert.That(ex.Message, Is.EqualTo("DB failed"));
//        }
//    }
//}
