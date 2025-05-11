using Microsoft.EntityFrameworkCore;
using Planify_BackEnd.Models;
using Planify_BackEnd.Repositories.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Planify.Tests.Repositories.Test.TaskRepositoriesTest
{
    public class GetAllTasksRepositoryTest
    {
        private DbContextOptions<PlanifyContext> _dbContextOptions;
        private PlanifyContext _context;
        private TaskRepository _repository;
        private int _testEventId = 1;

        [SetUp]
        public void Setup()
        {
            _dbContextOptions = new DbContextOptionsBuilder<PlanifyContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new PlanifyContext(_dbContextOptions);

            _context.Tasks.AddRange(
                new Planify_BackEnd.Models.Task { TaskName = "Task A", TaskDescription = "Desc A", EventId = _testEventId, Status = 1 },
                new Planify_BackEnd.Models.Task { TaskName = "Task B", TaskDescription = "Desc B", EventId = _testEventId, Status = 1 },
                new Planify_BackEnd.Models.Task { TaskName = "Task C", TaskDescription = "Desc C", EventId = _testEventId, Status = 0 }, 
                new Planify_BackEnd.Models.Task { TaskName = "Task D", TaskDescription = "Desc D", EventId = _testEventId + 1, Status = 1 } 
            );

            _context.SaveChanges();
            _repository = new TaskRepository(_context);
        }
        [Test]
        public void TC01_PageZero_ShouldThrowOrReturnEmpty()
        {
            var result = _repository.GetAllTasks(_testEventId, 0, 2); 
            Assert.That(result.Items, Is.Null);
        }
        [Test]
        public void TC02_PageOne_ShouldReturnFirstPage()
        {
            var result = _repository.GetAllTasks(_testEventId, 1, 1);

            Assert.That(result.Items.Count, Is.EqualTo(1));
        }
        [Test]
        public void TC03_PageSizeZero_ShouldThrowOrReturnEmpty()
        {
            var result = _repository.GetAllTasks(_testEventId, 1, 0);
            Assert.That(result.Items, Is.Empty);
        }
        [Test]
        public void TC04_PageSizeOne_ShouldReturnOneTask()
        {
            var result = _repository.GetAllTasks(_testEventId, 1, 1);

            Assert.That(result.Items.Count, Is.EqualTo(1));
        }
        [Test]
        public void TC05_NoMatchingTasks_ShouldReturnEmptyList()
        {
            var result = _repository.GetAllTasks(999, 1, 10);

            Assert.That(result.Items, Is.Empty);
            Assert.That(result.TotalCount, Is.EqualTo(0));
        }
        [Test]
        public void TC06_PageOutOfRange_ShouldReturnEmptyList()
        {
            var result = _repository.GetAllTasks(_testEventId, 10, 1);

            Assert.That(result.Items, Is.Empty);
        }
        [Test]
        public void TC07_FullPage_ShouldReturnCorrectTasks()
        {
            var result = _repository.GetAllTasks(_testEventId, 1, 2);

            Assert.That(result.TotalCount, Is.EqualTo(2));
            Assert.That(result.Items.Count, Is.EqualTo(2));
        }

    }
}
