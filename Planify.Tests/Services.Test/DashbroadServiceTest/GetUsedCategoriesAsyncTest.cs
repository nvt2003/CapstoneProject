using NUnit.Framework;
using Moq;
using Planify_BackEnd.DTOs.Dashboards;
using Planify_BackEnd.Services.Dashboards;
using Planify_BackEnd.Repositories.Dashboards;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace Planify.Tests.Services.Test.DashbroadServiceTest
{
    [TestFixture]
    public class GetUsedCategoriesAsyncTest
    {
        private Mock<IDashboardRepository> _dashboardRepoMock;
        private Mock<ICampusRepository> _campusRepoMock;
        private DashboardService _dashboardService;

        [SetUp]
        public void Setup()
        {
            _dashboardRepoMock = new Mock<IDashboardRepository>();
            _campusRepoMock = new Mock<ICampusRepository>();
            _dashboardService = new DashboardService(
                _dashboardRepoMock.Object,
                _campusRepoMock.Object
            );
        }
        [Test]
        public async Task GetUsedCategoriesAsync_NoData_ReturnsEmptyList()
        {
            _dashboardRepoMock.Setup(r => r.GetUsedCategoriesAsync())
                .ReturnsAsync(new List<CategoryUsageDTO>());

            var result = await _dashboardService.GetUsedCategoriesAsync();

            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
        }
        [Test]
        public async Task GetUsedCategoriesAsync_OneCategory_Total100Percent()
        {
            var data = new List<CategoryUsageDTO>
            {
                new CategoryUsageDTO { CategoryEventId = 1, CategoryEventName = "Cultural", TotalUsed = 10 }
            };

            _dashboardRepoMock.Setup(r => r.GetUsedCategoriesAsync()).ReturnsAsync(data);

            var result = await _dashboardService.GetUsedCategoriesAsync();

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(100, result[0].Percentage);
        }
        [Test]
        public async Task GetUsedCategoriesAsync_TwoEqualCategories_50PercentEach()
        {
            var data = new List<CategoryUsageDTO>
            {
                new CategoryUsageDTO { CategoryEventId = 1, CategoryEventName = "Culture", TotalUsed = 5 },
                new CategoryUsageDTO { CategoryEventId = 2, CategoryEventName = "Science", TotalUsed = 5 }
            };

            _dashboardRepoMock.Setup(r => r.GetUsedCategoriesAsync()).ReturnsAsync(data);

            var result = await _dashboardService.GetUsedCategoriesAsync();

            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(50, result[0].Percentage);
            Assert.AreEqual(50, result[1].Percentage);
        }
        [Test]
        public async Task GetUsedCategoriesAsync_TwoUnequalCategories_CorrectPercentage()
        {
            var data = new List<CategoryUsageDTO>
            {
                new CategoryUsageDTO { CategoryEventId = 1, CategoryEventName = "Art", TotalUsed = 2 },
                new CategoryUsageDTO { CategoryEventId = 2, CategoryEventName = "Tech", TotalUsed = 8 }
            };

            _dashboardRepoMock.Setup(r => r.GetUsedCategoriesAsync()).ReturnsAsync(data);

            var result = await _dashboardService.GetUsedCategoriesAsync();

            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(20, result[0].Percentage);
            Assert.AreEqual(80, result[1].Percentage);
        }
        [Test]
        public async Task GetUsedCategoriesAsync_ThreeCategories_CorrectPercentage()
        {
            var data = new List<CategoryUsageDTO>
            {
                new CategoryUsageDTO { CategoryEventId = 1, CategoryEventName = "A", TotalUsed = 3 },
                new CategoryUsageDTO { CategoryEventId = 2, CategoryEventName = "B", TotalUsed = 3 },
                new CategoryUsageDTO { CategoryEventId = 3, CategoryEventName = "C", TotalUsed = 4 }
            };

            _dashboardRepoMock.Setup(r => r.GetUsedCategoriesAsync()).ReturnsAsync(data);

            var result = await _dashboardService.GetUsedCategoriesAsync();

            Assert.AreEqual(3, result.Count);
            Assert.AreEqual(30, result[0].Percentage);
            Assert.AreEqual(30, result[1].Percentage);
            Assert.AreEqual(40, result[2].Percentage);
        }
        [Test]
        public async Task GetUsedCategoriesAsync_TotalUsedIsZero_ReturnsZeroPercentage()
        {
            var data = new List<CategoryUsageDTO>
            {
                new CategoryUsageDTO { CategoryEventId = 1, CategoryEventName = "X", TotalUsed = 0 },
                new CategoryUsageDTO { CategoryEventId = 2, CategoryEventName = "Y", TotalUsed = 0 }
            };

            _dashboardRepoMock.Setup(r => r.GetUsedCategoriesAsync()).ReturnsAsync(data);

            var result = await _dashboardService.GetUsedCategoriesAsync();

            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(0, result[0].Percentage);
            Assert.AreEqual(0, result[1].Percentage);
        }
    }
}