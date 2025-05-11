using Moq;
using NUnit.Framework;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System;
using Planify_BackEnd.Models;
using Planify_BackEnd.DTOs;
using Planify_BackEnd.Repositories;
using System.Threading.Tasks;

namespace Planify.Tests.Repositories.Test.EventRepositoriesTest
{

    [TestFixture]
    public class SearchEventAsyncRepository
    {
        private Mock<PlanifyContext> _contextMock;
        private Mock<DbSet<Event>> _eventDbSetMock;
        private EventRepository _repository;
        Guid searchId;

        [SetUp]
        public void SetUp()
        {
            searchId = Guid.NewGuid();
            var eventList = new List<Event>
            {
                new Event
                {
                    Id = 1,
                    EventTitle = "Tech Day",
                    StartTime = DateTime.Now.AddDays(1),
                    EndTime = DateTime.Now.AddDays(2),
                    AmountBudget = 500,
                    IsPublic = 1,
                    Status = 2,
                    CategoryEventId = 5,
                    CampusId = 1,
                    Placed = "Main Hall",
                    CreateBy = Guid.NewGuid(),
                    FavouriteEvents = new List<FavouriteEvent>()
                },
                new Event
                {
                    Id = 2,
                    EventTitle = "Science Fair",
                    StartTime = DateTime.Now.AddDays(3),
                    EndTime = DateTime.Now.AddDays(4),
                    AmountBudget = 300,
                    IsPublic = 0,
                    Status = 1,
                    CategoryEventId = 5,
                    CampusId = 1,
                    Placed = "Auditorium",
                    CreateBy = searchId,
                    FavouriteEvents = new List<FavouriteEvent>()
                }
            }.AsQueryable();

            _eventDbSetMock = new Mock<DbSet<Event>>();
            _eventDbSetMock.As<IQueryable<Event>>().Setup(m => m.Provider).Returns(eventList.Provider);
            _eventDbSetMock.As<IQueryable<Event>>().Setup(m => m.Expression).Returns(eventList.Expression);
            _eventDbSetMock.As<IQueryable<Event>>().Setup(m => m.ElementType).Returns(eventList.ElementType);
            _eventDbSetMock.As<IQueryable<Event>>().Setup(m => m.GetEnumerator()).Returns(eventList.GetEnumerator());

            _contextMock = new Mock<PlanifyContext>();
            _contextMock.Setup(c => c.Events).Returns(_eventDbSetMock.Object);

            _repository = new EventRepository(_contextMock.Object);
        }
        [Test]
        public async System.Threading.Tasks.Task TC001_SearchEventAsync_NoFilters_ShouldReturnAllEvents()
        {
            var result = await _repository.SearchEventAsync(
                page: 1,
                pageSize: 10,
                title: null,
                startTime: null,
                endTime: null,
                minBudget: null,
                maxBudget: null,
                isPublic: null,
                status: null,
                CategoryEventId: null,
                placed: null,
                userId: Guid.NewGuid(),
                campusId: 1,
                createBy: null
            );

            Assert.AreEqual(2, result.Items.Count());
        }
        [Test]
        public async System.Threading.Tasks.Task TC002_SearchEventAsync_FilterByTitle_ShouldReturnMatchingEvent()
        {
            var result = await _repository.SearchEventAsync(
                page: 1,
                pageSize: 10,
                title: "Tech",
                startTime: null,
                endTime: null,
                minBudget: null,
                maxBudget: null,
                isPublic: null,
                status: null,
                CategoryEventId: null,
                placed: null,
                userId: Guid.NewGuid(),
                campusId: 1,
                createBy: null
            );

            Assert.AreEqual(1, result.Items.Count());
            Assert.IsTrue(result.Items.First().EventTitle.Contains("Tech"));
        }
        [Test]
        public async System.Threading.Tasks.Task TC003_SearchEventAsync_FilterByStartTime_ShouldReturnFutureEvents()
        {
            var futureDate = DateTime.Now.AddDays(2);
            var result = await _repository.SearchEventAsync(
                page: 1,
                pageSize: 10,
                title: null,
                startTime: futureDate,
                endTime: null,
                minBudget: null,
                maxBudget: null,
                isPublic: null,
                status: null,
                CategoryEventId: null,
                placed: null,
                userId: Guid.NewGuid(),
                campusId: 1,
                createBy: null
            );

            Assert.AreEqual(1, result.Items.Count());
        }
        [Test]
        public async System.Threading.Tasks.Task TC004_SearchEventAsync_FilterByBudgetRange_ShouldReturnMatchingEvents()
        {
            var result = await _repository.SearchEventAsync(
                page: 1,
                pageSize: 10,
                title: null,
                startTime: null,
                endTime: null,
                minBudget: 200,
                maxBudget: 400,
                isPublic: null,
                status: null,
                CategoryEventId: null,
                placed: null,
                userId: Guid.NewGuid(),
                campusId: 1,
                createBy: null
            );

            Assert.AreEqual(1, result.Items.Count());
            Assert.That(result.Items.First().AmountBudget, Is.InRange(200, 400));
        }
        [Test]
        public async System.Threading.Tasks.Task TC005_SearchEventAsync_FilterByIsPublic_ShouldReturnOnlyPublicEvents()
        {
            var result = await _repository.SearchEventAsync(
                page: 1,
                pageSize: 10,
                title: null,
                startTime: null,
                endTime: null,
                minBudget: null,
                maxBudget: null,
                isPublic: 1,
                status: null,
                CategoryEventId: null,
                placed: null,
                userId: Guid.NewGuid(),
                campusId: 1,
                createBy: null
            );

            Assert.AreEqual(1, result.Items.Count());
            Assert.AreEqual(1, result.Items.First().IsPublic);
        }
        [Test]
        public async System.Threading.Tasks.Task TC006_SearchEventAsync_FilterByStatus_ShouldReturnMatchingEvents()
        {
            var result = await _repository.SearchEventAsync(
                page: 1,
                pageSize: 10,
                title: null,
                startTime: null,
                endTime: null,
                minBudget: null,
                maxBudget: null,
                isPublic: null,
                status: 2,
                CategoryEventId: null,
                placed: null,
                userId: Guid.NewGuid(),
                campusId: 1,
                createBy: null
            );

            Assert.AreEqual(1, result.Items.Count());
            Assert.IsTrue(result.Items.All(e => e.Status == 2));
        }
        [Test]
        public async System.Threading.Tasks.Task TC007_SearchEventAsync_FilterByCategoryEventId_ShouldReturnCorrectCategory()
        {
            var result = await _repository.SearchEventAsync(
                page: 1,
                pageSize: 10,
                title: null,
                startTime: null,
                endTime: null,
                minBudget: null,
                maxBudget: null,
                isPublic: null,
                status: null,
                CategoryEventId: 100,
                placed: null,
                userId: Guid.NewGuid(),
                campusId: 1,
                createBy: null
            );

            Assert.AreEqual(0, result.Items.Count());
        }
        [Test]
        public async System.Threading.Tasks.Task TC008_SearchEventAsync_FilterByPlaced_ShouldReturnCorrectEvents()
        {
            var result = await _repository.SearchEventAsync(
                page: 1,
                pageSize: 10,
                title: null,
                startTime: null,
                endTime: null,
                minBudget: null,
                maxBudget: null,
                isPublic: null,
                status: null,
                CategoryEventId: null,
                placed: "Hall",
                userId: Guid.NewGuid(),
                campusId: 1,
                createBy: null
            );

            Assert.AreEqual(1, result.Items.Count());
            Assert.IsTrue(result.Items.First().Placed.Contains("Hall"));
        }
        [Test]
        public async System.Threading.Tasks.Task TC009_SearchEventAsync_FilterByCreateBy_ShouldReturnCreatedEvents()
        {
            var result = await _repository.SearchEventAsync(
                page: 1,
                pageSize: 10,
                title: null,
                startTime: null,
                endTime: null,
                minBudget: null,
                maxBudget: null,
                isPublic: null,
                status: null,
                CategoryEventId: null,
                placed: null,
                userId: Guid.NewGuid(),
                campusId: 1,
                createBy: Guid.NewGuid()
            );

            Assert.AreEqual(0, result.Items.Count());
        }
        [Test]
        public async System.Threading.Tasks.Task TC010_SearchEventAsync_Paging_ShouldReturnLimitedEventsPerPage()
        {
            var result = await _repository.SearchEventAsync(
                page: 1,
                pageSize: 1,
                title: null,
                startTime: null,
                endTime: null,
                minBudget: null,
                maxBudget: null,
                isPublic: null,
                status: null,
                CategoryEventId: null,
                placed: null,
                userId: Guid.NewGuid(),
                campusId: 1,
                createBy: null
            );

            Assert.AreEqual(1, result.Items.Count());
            Assert.AreEqual(2, result.TotalCount);
            Assert.AreEqual(2, result.TotalPages);
        }
        [Test]
        public async System.Threading.Tasks.Task TC011_SearchEventAsync_FilterByStartTime_ShouldReturnMatchingEvents()
        {
            var fromDate = DateTime.Now.AddDays(-2);
            var result = await _repository.SearchEventAsync(
                page: 1,
                pageSize: 10,
                title: null,
                startTime: fromDate,
                endTime: null,
                minBudget: null,
                maxBudget: null,
                isPublic: null,
                status: null,
                CategoryEventId: null,
                placed: null,
                userId: Guid.NewGuid(),
                campusId: 1,
                createBy: null
            );
            Assert.AreEqual(2, result.Items.Count());
            Assert.IsTrue(result.Items.All(e => e.StartTime >= fromDate));
        }
        [Test]
        public async System.Threading.Tasks.Task TC012_SearchEventAsync_FilterByEndTime_ShouldReturnMatchingEvents()
        {
            var toDate = DateTime.Now.AddDays(3);
            var result = await _repository.SearchEventAsync(
                page: 1,
                pageSize: 10,
                title: null,
                startTime: null,
                endTime: toDate,
                minBudget: null,
                maxBudget: null,
                isPublic: null,
                status: null,
                CategoryEventId: null,
                placed: null,
                userId: Guid.NewGuid(),
                campusId: 1,
                createBy: null
            );
            Assert.AreEqual(1, result.Items.Count());
            Assert.IsTrue(result.Items.All(e => e.EndTime <= toDate));
        }
        [Test]
        public async System.Threading.Tasks.Task TC013_SearchEventAsync_FilterByMinBudget_ShouldReturnEventsAboveThreshold()
        {
            var result = await _repository.SearchEventAsync(
                page: 1,
                pageSize: 10,
                title: null,
                startTime: null,
                endTime: null,
                minBudget: 1000,
                maxBudget: null,
                isPublic: null,
                status: null,
                CategoryEventId: null,
                placed: null,
                userId: Guid.NewGuid(),
                campusId: 1,
                createBy: null
            );
            Assert.AreEqual(0, result.Items.Count());
            Assert.IsTrue(result.Items.All(e => e.AmountBudget >= 1000));
        }
        [Test]
        public async System.Threading.Tasks.Task TC014_SearchEventAsync_FilterByMaxBudget_ShouldReturnEventsBelowThreshold()
        {
            var result = await _repository.SearchEventAsync(
                page: 1,
                pageSize: 10,
                title: null,
                startTime: null,
                endTime: null,
                minBudget: null,
                maxBudget: 5000,
                isPublic: null,
                status: null,
                CategoryEventId: null,
                placed: null,
                userId: Guid.NewGuid(),
                campusId: 1,
                createBy: null
            );
            Assert.AreEqual(2, result.Items.Count());
            Assert.IsTrue(result.Items.All(e => e.AmountBudget <= 5000));
        }
        [Test]
        public async System.Threading.Tasks.Task TC015_SearchEventAsync_FilterByMultipleConditions_ShouldReturnMatchingEvents()
        {
            var fromDate = DateTime.Now.AddDays(-2);
            var toDate = DateTime.Now.AddDays(3);
            var result = await _repository.SearchEventAsync(
                page: 1,
                pageSize: 10,
                title: "Tech",
                startTime: fromDate,
                endTime: toDate,
                minBudget: 200,
                maxBudget: 5000,
                isPublic: 1,
                status: 2,
                CategoryEventId: 5,
                placed: "Hall",
                userId: Guid.NewGuid(),
                campusId: 1,
                createBy: null
            );
            Assert.AreEqual(1, result.Items.Count());
            Assert.IsTrue(result.Items.All(e =>
                e.EventTitle.Contains("Tech") &&
                e.StartTime >= fromDate &&
                e.EndTime <= toDate &&
                e.AmountBudget >= 400 &&
                e.AmountBudget <= 5000 &&
                e.IsPublic == 1 &&
                e.Status == 2 &&
                e.CategoryEventId == 5 &&
                e.Placed.Contains("Hall")
            ));
        }
        [Test]
        public async System.Threading.Tasks.Task TC016_SearchEventAsync_FilterByIsPublic_ShouldReturnOnlyPublicEvents()
        {
            var result = await _repository.SearchEventAsync(
                page: 1,
                pageSize: 10,
                title: null,
                startTime: null,
                endTime: null,
                minBudget: null,
                maxBudget: null,
                isPublic: 1,
                status: null,
                CategoryEventId: null,
                placed: null,
                userId: Guid.NewGuid(),
                campusId: 1,
                createBy: null
            );
            Assert.AreEqual(1, result.Items.Count());
            Assert.IsTrue(result.Items.All(e => e.IsPublic == 1));
        }
        [Test]
        public async System.Threading.Tasks.Task TC017_SearchEventAsync_FilterByStatus_ShouldReturnCorrectEvents()
        {
            var result = await _repository.SearchEventAsync(
                page: 1,
                pageSize: 10,
                title: null,
                startTime: null,
                endTime: null,
                minBudget: null,
                maxBudget: null,
                isPublic: null,
                status: 2,
                CategoryEventId: null,
                placed: null,
                userId: Guid.NewGuid(),
                campusId: 1,
                createBy: null
            );
            Assert.AreEqual(1, result.Items.Count());
            Assert.IsTrue(result.Items.All(e => e.Status == 2));
        }
        [Test]
        public async System.Threading.Tasks.Task TC018_SearchEventAsync_FilterByCategoryEventId_ShouldReturnMatchingEvents()
        {
            var result = await _repository.SearchEventAsync(
                page: 1,
                pageSize: 10,
                title: null,
                startTime: null,
                endTime: null,
                minBudget: null,
                maxBudget: null,
                isPublic: null,
                status: null,
                CategoryEventId: 5,
                placed: null,
                userId: Guid.NewGuid(),
                campusId: 1,
                createBy: null
            );
            Assert.AreEqual(2, result.Items.Count());
            Assert.IsTrue(result.Items.All(e => e.CategoryEventId == 5));
        }
        [Test]
        public async System.Threading.Tasks.Task TC019_SearchEventAsync_FilterByPlaced_ShouldReturnMatchingEvents()
        {
            var result = await _repository.SearchEventAsync(
                page: 1,
                pageSize: 10,
                title: null,
                startTime: null,
                endTime: null,
                minBudget: null,
                maxBudget: null,
                isPublic: null,
                status: null,
                CategoryEventId: null,
                placed: "Auditorium",
                userId: Guid.NewGuid(),
                campusId: 1,
                createBy: null
            );
            Assert.AreEqual(1, result.Items.Count());
            Assert.IsTrue(result.Items.All(e => e.Placed.Contains("Auditorium")));
        }
        [Test]
        public async System.Threading.Tasks.Task TC020_SearchEventAsync_FilterByCreateBy_ShouldReturnEventsCreatedByUser()
        {
            var creatorId = Guid.NewGuid();
            var result = await _repository.SearchEventAsync(
                page: 1,
                pageSize: 10,
                title: null,
                startTime: null,
                endTime: null,
                minBudget: null,
                maxBudget: null,
                isPublic: null,
                status: null,
                CategoryEventId: null,
                placed: null,
                userId: Guid.NewGuid(),
                campusId: 1,
                createBy: searchId
            );
            Assert.AreEqual(1, result.Items.Count());
            Assert.IsTrue(result.Items.All(e => e.CreateBy == searchId));
        }


    }
}

