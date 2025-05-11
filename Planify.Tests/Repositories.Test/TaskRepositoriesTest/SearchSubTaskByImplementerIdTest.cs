using Google;
using Microsoft.EntityFrameworkCore;
using Moq;
using SubTask = Planify_BackEnd.Models.SubTask;
using Planify_BackEnd.Models;
using Planify_BackEnd.Repositories.Tasks;

namespace Planify.Tests.Repositories.Test.TaskRepositoriesTest
{
    public class SearchSubTaskByImplementerIdTest
    {
        private readonly PlanifyContext _context;
        private readonly TaskRepository _subTaskService;

        public SearchSubTaskByImplementerIdTest()
        {
            var options = new DbContextOptionsBuilder<PlanifyContext>()
                          .UseInMemoryDatabase(Guid.NewGuid().ToString())
                          .Options;

            _context = new PlanifyContext(options);
            _subTaskService = new TaskRepository(_context);

            SeedDatabase();
        }

        private void SeedDatabase()
        {
            var implementerId = Guid.NewGuid();

            var tasks = new List<JoinTask>
            {
                new JoinTask
                {
                    UserId = implementerId,
                    Task = new SubTask {SubTaskName = "SubTask 1",SubTaskDescription = "Description of SubTask 1",StartTime = new DateTime(2025, 5, 1), Deadline = new DateTime(2025, 6, 1), Status = 1 }
                },
                new JoinTask
                {
                    UserId = implementerId,
                    Task = new SubTask { SubTaskName = "SubTask 2",SubTaskDescription = "Description of SubTask 2",StartTime = new DateTime(2025, 7, 1), Deadline = new DateTime(2025, 8, 1), Status = 1 }
                },
                new JoinTask
                {
                    UserId = implementerId,
                    Task = new SubTask { SubTaskName = "SubTask 3",SubTaskDescription = "Description of SubTask 3",StartTime = new DateTime(2025, 3, 1), Deadline = null, Status = 1 }
                },
                new JoinTask
                {
                    UserId = Guid.NewGuid(), 
                    Task = new SubTask { SubTaskName = "SubTask 4",SubTaskDescription = "Description of SubTask 4",StartTime = new DateTime(2025, 4, 1), Deadline = new DateTime(2025, 5, 1), Status = 1 }
                }
            };  

            _context.JoinTasks.AddRange(tasks);
            _context.SaveChanges();
        }
        [Test]
        public async System.Threading.Tasks.Task SearchSubTaskByImplementerId_ShouldReturnCorrectResult_WhenValidInputIsProvided()
        {
            // Arrange
            var implementerId = _context.JoinTasks.First().UserId;
            var startDate = new DateTime(2025, 1, 1);
            var endDate = new DateTime(2025, 12, 31);
            var page = 1;
            var pageSize = 10;

            // Act
            var result = await _subTaskService.SearchSubTaskByImplementerId(page, pageSize, implementerId, startDate, endDate);

            // Assert
            Assert.That(result.Items.Count, Is.EqualTo(3));
            Assert.That(result.TotalCount, Is.EqualTo(3));
            Assert.That(result.PageNumber, Is.EqualTo(1));
            Assert.That(result.PageSize, Is.EqualTo(10));
        }

        [Test]
        public async System.Threading.Tasks.Task SearchSubTaskByImplementerId_ShouldReturnEmptyList_WhenNoTasksMatchTheCriteria()
        {
            // Arrange
            var implementerId = Guid.NewGuid(); 
            var startDate = new DateTime(2025, 1, 1);
            var endDate = new DateTime(2025, 12, 31);
            var page = 1;
            var pageSize = 10;

            // Act
            var result = await _subTaskService.SearchSubTaskByImplementerId(page, pageSize, implementerId, startDate, endDate);

            // Assert
            Assert.That(result.Items.Count, Is.EqualTo(0)); 
            Assert.That(result.TotalCount, Is.EqualTo(0)); 
        }

    }
}
