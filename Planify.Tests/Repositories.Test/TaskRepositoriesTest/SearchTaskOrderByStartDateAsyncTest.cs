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
    public class SearchTaskOrderByStartDateAsyncTest
    {
        private DbContextOptions<PlanifyContext> _dbContextOptions;
        private PlanifyContext _context;
        private TaskRepository _repository;

        [SetUp]
        public void Setup()
        {
            _dbContextOptions = new DbContextOptionsBuilder<PlanifyContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new PlanifyContext(_dbContextOptions);

            _context.Tasks.AddRange(
                new Planify_BackEnd.Models.Task { TaskName = "Task A", TaskDescription = "Description A", StartTime = DateTime.Now.AddDays(1), Deadline = DateTime.Now.AddDays(5), Status = 1 },
                new Planify_BackEnd.Models.Task { TaskName = "Task B", TaskDescription = "Description B", StartTime = DateTime.Now.AddDays(2), Deadline = DateTime.Now.AddDays(6), Status = 1 },
                new Planify_BackEnd.Models.Task { TaskName = "Task C", TaskDescription = "Description C", StartTime = DateTime.Now.AddDays(-1), Deadline = DateTime.Now.AddDays(3), Status = 0 },
                new Planify_BackEnd.Models.Task { TaskName = "Task D", TaskDescription = "Description D", StartTime = DateTime.Now.AddDays(3), Deadline = DateTime.Now.AddDays(3), Status = 1 }
            );
            _context.SaveChanges();

            _repository = new TaskRepository(_context);
        }
        [Test]
        public async System.Threading.Tasks.Task TC01_PageZero_ShouldThrowExceptionOrReturnEmpty()
        {
            var startDate = DateTime.Today;
            var endDate = DateTime.Today.AddDays(10);

            var result = await _repository.SearchTaskOrderByStartDateAsync(0, 10, "Task", startDate, endDate);

            Assert.That(result, Is.Null);
        }

        [Test]
        public async System.Threading.Tasks.Task TC02_PageOne_ShouldReturnValidTasksOrdered()
        {
            var startDate = DateTime.Today;
            var endDate = DateTime.Today.AddDays(10);

            var result = await _repository.SearchTaskOrderByStartDateAsync(1, 10, "Task", startDate, endDate);

            Assert.That(result.Count, Is.EqualTo(3));
        }

        [Test]
        public async System.Threading.Tasks.Task TC03_PageSizeZero_ShouldThrowExceptionOrReturnEmpty()
        {
            var startDate = DateTime.Today;
            var endDate = DateTime.Today.AddDays(10);

            var result = await _repository.SearchTaskOrderByStartDateAsync(1, 0, "Task", startDate, endDate);

            Assert.That(result, Is.Empty);
        }

        [Test]
        public async System.Threading.Tasks.Task TC04_PageSizeOne_ShouldReturnOneTask()
        {
            var startDate = DateTime.Today;
            var endDate = DateTime.Today.AddDays(10);

            var result = await _repository.SearchTaskOrderByStartDateAsync(1, 1, "Task", startDate, endDate);

            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result[0].TaskName, Is.EqualTo("Task A"));
        }

        [Test]
        public async System.Threading.Tasks.Task TC05_NameIsNull_ShouldReturnAllMatchingTasks()
        {
            var startDate = DateTime.Today;
            var endDate = DateTime.Today.AddDays(10);

            var result = await _repository.SearchTaskOrderByStartDateAsync(1, 10, null, startDate, endDate);

            Assert.That(result.Count, Is.EqualTo(3));
        }

        [Test]
        public async System.Threading.Tasks.Task TC06_NameIsEmpty_ShouldReturnAllMatchingTasks()
        {
            var startDate = DateTime.Today;
            var endDate = DateTime.Today.AddDays(10);

            var result = await _repository.SearchTaskOrderByStartDateAsync(1, 10, "", startDate, endDate);

            Assert.That(result.Count, Is.EqualTo(3));
        }
        [Test]
        public async System.Threading.Tasks.Task TC07_StartDateEqualsEndDate_ShouldReturnTasksOnThatDay()
        {
            var startOfDay = DateTime.Today.AddDays(3);
            DateTime endOfDay = DateTime.Today.AddDays(4).AddSeconds(-1);
            var result = await _repository.SearchTaskOrderByStartDateAsync(1, 10, "", startOfDay, endOfDay);

            Assert.That(result.Count, Is.EqualTo(1));
        }

        [Test]
        public async System.Threading.Tasks.Task TC08_StartDateAndEndDateCloseRange_ShouldReturnTasksInThatRange()
        {
            var startDate = DateTime.Today;
            var endDate = DateTime.Today.AddDays(4);

            var result = await _repository.SearchTaskOrderByStartDateAsync(1, 10, "", startDate, endDate);

            Assert.That(result.Count, Is.EqualTo(1));
        }

        [Test]
        public async System.Threading.Tasks.Task TC09_StartDateAfterEndDate_ShouldReturnEmpty()
        {
            var startDate = DateTime.Today.AddDays(5);
            var endDate = DateTime.Today.AddDays(4);

            var result = await _repository.SearchTaskOrderByStartDateAsync(1, 10, "", startDate, endDate);

            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.Empty);
        }

        [Test]
        public async System.Threading.Tasks.Task TC10_NoMatchingTask_ShouldReturnEmpty()
        {
            var startDate = DateTime.Today;
            var endDate = DateTime.Today.AddDays(10);

            var result = await _repository.SearchTaskOrderByStartDateAsync(1, 10, "NonExistentTask", startDate, endDate);

            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.Empty);
        }

        [Test]
        public async System.Threading.Tasks.Task TC11_NameBoundaryValues_ShouldReturnMatchingTasks()
        {
            var startDate = DateTime.Today;
            var endDate = DateTime.Today.AddDays(10);

            var result1 = await _repository.SearchTaskOrderByStartDateAsync(1, 10, "T", startDate, endDate);
            Assert.That(result1.Count, Is.GreaterThanOrEqualTo(2));

            var longName = new string('A', 1000);
            var result2 = await _repository.SearchTaskOrderByStartDateAsync(1, 10, longName, startDate, endDate);
            Assert.That(result2, Is.Empty);
        }

        [Test]
        public async System.Threading.Tasks.Task TC12_MinMaxDateRange_ShouldReturnAllValidTasks()
        {
            var startDate = DateTime.MinValue;
            var endDate = DateTime.MaxValue;

            var result = await _repository.SearchTaskOrderByStartDateAsync(1, 10, "", startDate, endDate);

            Assert.That(result.Count, Is.EqualTo(3));
        }

    }
}
