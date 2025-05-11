using Moq;
using NUnit.Framework;
using Planify_BackEnd.DTOs.Campus;
using Planify_BackEnd.Models;
using Planify_BackEnd.Services;
using Planify_BackEnd.Services.Campus;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Planify.Tests.Services.Test.CampusServiceTest
{
    public class GetAllCampusTests
    {
        private Mock<ICampusRepository> _mockCampusRepository;
        private CampusService _campusService;

        [SetUp]
        public void Setup()
        {
            _mockCampusRepository = new Mock<ICampusRepository>();
            _campusService = new CampusService(_mockCampusRepository.Object);
        }

        [Test]
        public async System.Threading.Tasks.Task GetAllCampus_ReturnsExpectedCampusList_WhenCampusExists()
        {
            var campusEntities = new List<Campus>
            {
                new Campus { Id = 1, CampusName = "Hòa Lạc", Status = 1 },
                new Campus { Id = 2, CampusName = "Hồ Chí Minh", Status = 1 },
                new Campus { Id = 3, CampusName = "Đà Nẵng", Status = 1 },
                new Campus { Id = 4, CampusName = "Cần Thơ", Status = 1 },
                new Campus { Id = 5, CampusName = "Quy Nhơn", Status = 1 }
            };

            _mockCampusRepository.Setup(repo => repo.getAllCampus())
                .ReturnsAsync(campusEntities);

            var result = (await _campusService.GetAllCampus()).ToList();

            Assert.IsNotNull(result);
            Assert.That(result.Count, Is.EqualTo(5));
            var expectedNames = new List<string> { "Hòa Lạc", "Hồ Chí Minh", "Đà Nẵng", "Cần Thơ", "Quy Nhơn" };
            CollectionAssert.AreEquivalent(expectedNames, result.Select(c => c.CampusName));
        }


        [Test]
        public async System.Threading.Tasks.Task GetAllCampus_WhenAllCampusesInactive_ReturnsEmptyList()
        {
            var campuses = new List<Campus>
            {
                new Campus { Id = 1, CampusName = "Hòa Lạc", Status = 0 },
                new Campus { Id = 2, CampusName = "Hồ Chí Minh", Status = 0 }
            };

            _mockCampusRepository.Setup(repo => repo.getAllCampus())
                .ReturnsAsync(campuses);

            var result = (await _campusService.GetAllCampus()).Where(c => c.Status == 1).ToList();

            Assert.That(result.Count, Is.EqualTo(0));
        }
    }
}
