using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Moq;
using Planify_BackEnd.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Planify.Tests.Repositories.Test.EventRepositoriesTest
{
    [TestFixture]
    public class GetAllEventRepositoryTest
    {
        private DbContextOptions<PlanifyContext> _dbContextOptions;
        private PlanifyContext _context;
        private EventRepository _repository;
        private Mock<PlanifyContext> _contextMock;

        [SetUp]
        public void SetUp()
        {
            _contextMock = new Mock<PlanifyContext>();
            _dbContextOptions = new DbContextOptionsBuilder<PlanifyContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;

            _context = new PlanifyContext(_dbContextOptions);
            _repository = new EventRepository(_context);
            _repository = new EventRepository(_contextMock.Object);
        }

        [Test]
        public void GetAllEvent_ShouldReturnCorrectEventsForPage1()
        {
            var campusId = 1;
            var page = 1;
            var pageSize = 2;
            var userId = Guid.NewGuid();

            var mockData = new List<Event>
            {
                new Event { EventTitle = "Event 1", CampusId = 1, StartTime = DateTime.Now.AddDays(1), EndTime = DateTime.Now.AddDays(2), Status = 2 },
                new Event { EventTitle = "Event 2", CampusId = 1, StartTime = DateTime.Now.AddDays(3), EndTime = DateTime.Now.AddDays(4), Status = 2 },
                new Event { EventTitle = "Event 3", CampusId = 1, StartTime = DateTime.Now.AddDays(5), EndTime = DateTime.Now.AddDays(6), Status = 2 },
                new Event { EventTitle = "Event 4", CampusId = 1, StartTime = DateTime.Now.AddDays(7), EndTime = DateTime.Now.AddDays(8), Status = 2 }
            }.AsQueryable();

            var mockDbSet = new Mock<DbSet<Event>>();
            mockDbSet.As<IQueryable<Event>>().Setup(m => m.Provider).Returns(mockData.Provider);
            mockDbSet.As<IQueryable<Event>>().Setup(m => m.Expression).Returns(mockData.Expression);
            mockDbSet.As<IQueryable<Event>>().Setup(m => m.ElementType).Returns(mockData.ElementType);
            mockDbSet.As<IQueryable<Event>>().Setup(m => m.GetEnumerator()).Returns(mockData.GetEnumerator());

            _contextMock.Setup(c => c.Events).Returns(mockDbSet.Object);

            var result = _repository.GetAllEvent(campusId, page, pageSize, userId);

            Assert.AreEqual(2, result.Items.Count());
            Assert.AreEqual(4, result.TotalCount);
            Assert.AreEqual(2, result.TotalPages);
            Assert.AreEqual(1, result.PageNumber);
        }


        [Test]
        public void GetAllEvent_ShouldReturnCorrectEventsForPage2()
        {
            var campusId = 1;
            var page = 2;
            var pageSize = 2;
            var userId = Guid.NewGuid();

            var mockData = new List<Event>
            {
                new Event { EventTitle = "Event 1", CampusId = 1, StartTime = DateTime.Now.AddDays(1), EndTime = DateTime.Now.AddDays(2), Status = 2 },
                new Event { EventTitle = "Event 2", CampusId = 1, StartTime = DateTime.Now.AddDays(3), EndTime = DateTime.Now.AddDays(4), Status = 2 },
                new Event { EventTitle = "Event 3", CampusId = 1, StartTime = DateTime.Now.AddDays(5), EndTime = DateTime.Now.AddDays(6), Status = 2 },
                new Event { EventTitle = "Event 4", CampusId = 1, StartTime = DateTime.Now.AddDays(7), EndTime = DateTime.Now.AddDays(8), Status = 2 }
            }.AsQueryable();

            var mockDbSet = new Mock<DbSet<Event>>();
            mockDbSet.As<IQueryable<Event>>().Setup(m => m.Provider).Returns(mockData.Provider);
            mockDbSet.As<IQueryable<Event>>().Setup(m => m.Expression).Returns(mockData.Expression);
            mockDbSet.As<IQueryable<Event>>().Setup(m => m.ElementType).Returns(mockData.ElementType);
            mockDbSet.As<IQueryable<Event>>().Setup(m => m.GetEnumerator()).Returns(mockData.GetEnumerator());

            _contextMock.Setup(c => c.Events).Returns(mockDbSet.Object);

            var result = _repository.GetAllEvent(campusId, page, pageSize, userId);

            Assert.AreEqual(2, result.Items.Count());
            Assert.AreEqual(4, result.TotalCount);
            Assert.AreEqual(2, result.TotalPages);
            Assert.AreEqual(2, result.PageNumber);
        }

        [Test]
        public void GetAllEvent_ShouldReturnEmptyWhenNoEvents()
        {
            var campusId = 1;
            var page = 1;
            var pageSize = 2;
            var userId = Guid.NewGuid();

            var mockData = new List<Event>
            {
            }.AsQueryable();

            var mockDbSet = new Mock<DbSet<Event>>();
            mockDbSet.As<IQueryable<Event>>().Setup(m => m.Provider).Returns(mockData.Provider);
            mockDbSet.As<IQueryable<Event>>().Setup(m => m.Expression).Returns(mockData.Expression);
            mockDbSet.As<IQueryable<Event>>().Setup(m => m.ElementType).Returns(mockData.ElementType);
            mockDbSet.As<IQueryable<Event>>().Setup(m => m.GetEnumerator()).Returns(mockData.GetEnumerator());

            _contextMock.Setup(c => c.Events).Returns(mockDbSet.Object);
            var result = _repository.GetAllEvent(campusId, page, pageSize, userId);

            Assert.AreEqual(0, result.Items.Count());
            Assert.AreEqual(0, result.TotalCount);
            Assert.AreEqual(0, result.TotalPages);
        }

        [Test]
        public void GetAllEvent_ShouldReturnCorrectEventsWhenSinglePage()
        {
            var campusId = 1;
            var page = 1;
            var pageSize = 3;
            var userId = Guid.NewGuid();

            var mockData = new List<Event>
            {
                new Event { EventTitle = "Event 1", CampusId = 1, StartTime = DateTime.Now.AddDays(1), EndTime = DateTime.Now.AddDays(2), Status = 2 },
                new Event { EventTitle = "Event 2", CampusId = 1, StartTime = DateTime.Now.AddDays(3), EndTime = DateTime.Now.AddDays(4), Status = 2 },
                new Event { EventTitle = "Event 3", CampusId = 1, StartTime = DateTime.Now.AddDays(5), EndTime = DateTime.Now.AddDays(6), Status = 2 }
            }.AsQueryable();

            var mockDbSet = new Mock<DbSet<Event>>();
            mockDbSet.As<IQueryable<Event>>().Setup(m => m.Provider).Returns(mockData.Provider);
            mockDbSet.As<IQueryable<Event>>().Setup(m => m.Expression).Returns(mockData.Expression);
            mockDbSet.As<IQueryable<Event>>().Setup(m => m.ElementType).Returns(mockData.ElementType);
            mockDbSet.As<IQueryable<Event>>().Setup(m => m.GetEnumerator()).Returns(mockData.GetEnumerator());

            _contextMock.Setup(c => c.Events).Returns(mockDbSet.Object);

            var result = _repository.GetAllEvent(campusId, page, pageSize, userId);

            Assert.AreEqual(3, result.Items.Count());
            Assert.AreEqual(3, result.TotalCount);
            Assert.AreEqual(1, result.TotalPages);
        }

        [Test]
        public void GetAllEvent_ShouldReturnCorrectEventStatusOrder()
        {
            var campusId = 1;
            var page = 1;
            var pageSize = 2;
            var userId = Guid.NewGuid();

            var mockData = new List<Event>
            {
                new Event { EventTitle = "Event 1", CampusId = 1, StartTime = DateTime.Now.AddDays(1), EndTime = DateTime.Now.AddDays(2), Status = 2 },
                new Event { EventTitle = "Event 2", CampusId = 1, StartTime = DateTime.Now.AddDays(3), EndTime = DateTime.Now.AddDays(4), Status = 2 },
                new Event { EventTitle = "Event 3", CampusId = 1, StartTime = DateTime.Now.AddDays(5), EndTime = DateTime.Now.AddDays(6), Status = 2 }
            }.AsQueryable();

            var mockDbSet = new Mock<DbSet<Event>>();
            mockDbSet.As<IQueryable<Event>>().Setup(m => m.Provider).Returns(mockData.Provider);
            mockDbSet.As<IQueryable<Event>>().Setup(m => m.Expression).Returns(mockData.Expression);
            mockDbSet.As<IQueryable<Event>>().Setup(m => m.ElementType).Returns(mockData.ElementType);
            mockDbSet.As<IQueryable<Event>>().Setup(m => m.GetEnumerator()).Returns(mockData.GetEnumerator());

            _contextMock.Setup(c => c.Events).Returns(mockDbSet.Object);

            var result = _repository.GetAllEvent(campusId, page, pageSize, userId);

            Assert.AreEqual("Event 1", result.Items.ElementAt(0).EventTitle);
            Assert.AreEqual("Event 2", result.Items.ElementAt(1).EventTitle);
        }

        [Test]
        public void GetAllEvent_ShouldReturnNoEventsIfCampusIdDoesNotMatch()
        {
            var campusId = 999; 
            var page = 1;
            var pageSize = 2;
            var userId = Guid.NewGuid();

            var mockData = new List<Event>
            {
                new Event { EventTitle = "Event 1", CampusId = 1, StartTime = DateTime.Now.AddDays(1), EndTime = DateTime.Now.AddDays(2), Status = 2 },
                new Event { EventTitle = "Event 2", CampusId = 1, StartTime = DateTime.Now.AddDays(3), EndTime = DateTime.Now.AddDays(4), Status = 2 },
                new Event { EventTitle = "Event 3", CampusId = 1, StartTime = DateTime.Now.AddDays(5), EndTime = DateTime.Now.AddDays(6), Status = 2 }
            }.AsQueryable();

            var mockDbSet = new Mock<DbSet<Event>>();
            mockDbSet.As<IQueryable<Event>>().Setup(m => m.Provider).Returns(mockData.Provider);
            mockDbSet.As<IQueryable<Event>>().Setup(m => m.Expression).Returns(mockData.Expression);
            mockDbSet.As<IQueryable<Event>>().Setup(m => m.ElementType).Returns(mockData.ElementType);
            mockDbSet.As<IQueryable<Event>>().Setup(m => m.GetEnumerator()).Returns(mockData.GetEnumerator());

            _contextMock.Setup(c => c.Events).Returns(mockDbSet.Object);
            var result = _repository.GetAllEvent(campusId, page, pageSize, userId);

            Assert.AreEqual(0, result.Items.Count());
            Assert.AreEqual(0, result.TotalCount);
        }

        [Test]
        public void GetAllEvent_ShouldReturnEventsWithCorrectPaginationForLargePage()
        {
            var campusId = 1;
            var page = 10; 
            var pageSize = 2;
            var userId = Guid.NewGuid();

            var mockData = new List<Event>
            {
                new Event { EventTitle = "Event 1", CampusId = 1, StartTime = DateTime.Now.AddDays(1), EndTime = DateTime.Now.AddDays(2), Status = 2 },
                new Event { EventTitle = "Event 2", CampusId = 1, StartTime = DateTime.Now.AddDays(3), EndTime = DateTime.Now.AddDays(4), Status = 2 },
                new Event { EventTitle = "Event 3", CampusId = 1, StartTime = DateTime.Now.AddDays(5), EndTime = DateTime.Now.AddDays(6), Status = 2 },
                new Event { EventTitle = "Event 4", CampusId = 1, StartTime = DateTime.Now.AddDays(7), EndTime = DateTime.Now.AddDays(8), Status = 2 }
            }.AsQueryable();

            var mockDbSet = new Mock<DbSet<Event>>();
            mockDbSet.As<IQueryable<Event>>().Setup(m => m.Provider).Returns(mockData.Provider);
            mockDbSet.As<IQueryable<Event>>().Setup(m => m.Expression).Returns(mockData.Expression);
            mockDbSet.As<IQueryable<Event>>().Setup(m => m.ElementType).Returns(mockData.ElementType);
            mockDbSet.As<IQueryable<Event>>().Setup(m => m.GetEnumerator()).Returns(mockData.GetEnumerator());

            _contextMock.Setup(c => c.Events).Returns(mockDbSet.Object);
            var result = _repository.GetAllEvent(campusId, page, pageSize, userId);

            Assert.AreEqual(0, result.Items.Count());
            Assert.AreEqual(4, result.TotalCount);
            Assert.AreEqual(2, result.TotalPages);
        }


    }

}
