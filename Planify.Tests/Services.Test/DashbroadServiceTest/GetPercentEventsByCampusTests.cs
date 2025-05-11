using Moq;
using Planify_BackEnd.DTOs.Dashboards;
using Planify_BackEnd.Repositories.Dashboards;
using Planify_BackEnd.Services.Dashboards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Planify.Tests.Services.Test.DashbroadServiceTest
{
    public class GetPercentEventsByCampusTests
    {
        private Mock<IDashboardRepository> _dashboardRepoMock;
        private Mock<ICampusRepository> _campusRepoMock;
        private DashboardService _service;

        [SetUp]
        public void Setup()
        {
            _dashboardRepoMock = new Mock<IDashboardRepository>();
            _campusRepoMock = new Mock<ICampusRepository>();
            _service = new DashboardService(_dashboardRepoMock.Object, _campusRepoMock.Object);
        }

        [Test]
        public async Task GetPercentEventsByCampus_MultipleCampuses_CorrectPercentage()
        {
            var data = new List<PercentEventByCampus>
            {
                new PercentEventByCampus { CampusName = "A", TotalEvent = 10 },
                new PercentEventByCampus { CampusName = "B", TotalEvent = 20 },
                new PercentEventByCampus { CampusName = "C", TotalEvent = 30 }
            };

            _dashboardRepoMock.Setup(r => r.GetPercentEventsByCampus()).ReturnsAsync(data);

            var result = await _service.GetPercentEventsByCampus();

            Assert.AreEqual(3, result.Count);
            Assert.That(result[0].Percent, Is.EqualTo(16.666666666666664m).Within(0.01));
            Assert.That(result[1].Percent, Is.EqualTo(33.33333333333333m).Within(0.01));
            Assert.That(result[2].Percent, Is.EqualTo(50m).Within(0.01));
        }

        [Test]
        public async Task GetPercentEventsByCampus_AllZeroEvents_ShouldReturnZeroPercent()
        {
            var data = new List<PercentEventByCampus>
            {
                new PercentEventByCampus { CampusName = "A", TotalEvent = 0 },
                new PercentEventByCampus { CampusName = "B", TotalEvent = 0 }
            };

            _dashboardRepoMock.Setup(r => r.GetPercentEventsByCampus()).ReturnsAsync(data);

            var result = await _service.GetPercentEventsByCampus();

            Assert.True(result.All(r => r.Percent == 0));
        }


        [Test]
        public async Task GetPercentEventsByCampus_EmptyList_ReturnsEmpty()
        {
            _dashboardRepoMock.Setup(r => r.GetPercentEventsByCampus()).ReturnsAsync(new List<PercentEventByCampus>());

            var result = await _service.GetPercentEventsByCampus();

            Assert.IsNotNull(result);
            Assert.IsEmpty(result);
        }

        [Test]
        public async Task GetPercentEventsByCampus_OneCampus_ShouldBe100Percent()
        {
            var data = new List<PercentEventByCampus>
            {
                new PercentEventByCampus { CampusName = "A", TotalEvent = 50 }
            };

            _dashboardRepoMock.Setup(r => r.GetPercentEventsByCampus()).ReturnsAsync(data);

            var result = await _service.GetPercentEventsByCampus();

            Assert.AreEqual(100, result.First().Percent);
        }


        [Test]
        public void GetPercentEventsByCampus_RepositoryThrowsException_ShouldPropagate()
        {
            _dashboardRepoMock.Setup(r => r.GetPercentEventsByCampus()).ThrowsAsync(new System.Exception("DB error"));

            var ex = Assert.ThrowsAsync<System.Exception>(async () =>
            {
                await _service.GetPercentEventsByCampus();
            });

            Assert.AreEqual("DB error", ex.Message);
        }

        [Test]
        public async Task GetPercentEventsByCampus_CampusNameNull_StillCalculatesPercent()
        {
            var data = new List<PercentEventByCampus>
        {
            new PercentEventByCampus { CampusName = null, TotalEvent = 10 },
            new PercentEventByCampus { CampusName = "B", TotalEvent = 10 }
        };

            _dashboardRepoMock.Setup(r => r.GetPercentEventsByCampus()).ReturnsAsync(data);

            var result = await _service.GetPercentEventsByCampus();

            Assert.AreEqual(50, result[0].Percent);
            Assert.AreEqual(50, result[1].Percent);
        }

        [Test]
        public async Task GetPercentEventsByCampus_OneZeroOtherNonZero_ShouldBeZeroAnd100()
        {
            var data = new List<PercentEventByCampus>
            {
                new PercentEventByCampus { CampusName = "A", TotalEvent = 0 },
                new PercentEventByCampus { CampusName = "B", TotalEvent = 10 }
            };

            _dashboardRepoMock.Setup(r => r.GetPercentEventsByCampus()).ReturnsAsync(data);

            var result = await _service.GetPercentEventsByCampus();

            Assert.AreEqual(0, result[0].Percent);
            Assert.AreEqual(100, result[1].Percent);
        }

        [Test]
        public async Task GetPercentEventsByCampus_OneEvent_TotalPercentShouldBe100()
        {
            var data = new List<PercentEventByCampus>
            {
                new PercentEventByCampus { CampusName = "A", TotalEvent = 1 }
            };

            _dashboardRepoMock.Setup(r => r.GetPercentEventsByCampus()).ReturnsAsync(data);

            var result = await _service.GetPercentEventsByCampus();

            Assert.AreEqual(100, result.First().Percent);
        }

    }
}
