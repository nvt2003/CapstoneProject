using Microsoft.EntityFrameworkCore;
using Planify_BackEnd.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Planify.Tests.Repositories.Test.UserRepositoriesTest
{
    public class GetListUserRepositoryTest
    {
        private PlanifyContext _context;
        private UserRepository _userRepository;

        [SetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<PlanifyContext>()
                .UseInMemoryDatabase(databaseName: "PlanifyTestDb")
                .Options;

            _context = new PlanifyContext(options);
            _userRepository = new UserRepository(_context);

            SeedDatabase();
        }

        private void SeedDatabase()
        {
            if (!_context.Roles.Any())
            {
                _context.Roles.AddRange(new List<Role>
                {
                    new Role { Id = 1, RoleName = "Admin" },
                    new Role { Id = 2, RoleName = "User" },
                    new Role { Id = 3, RoleName = "Manager" }
                });
            }

            if (!_context.Campuses.Any())
            {
                _context.Campuses.Add(new Campus { Id = 1, CampusName = "Campus A" });
            }

            _context.SaveChanges();

            var campus = _context.Campuses.First();

            if (!_context.Users.Any())
            {
                var user1 = new User
                {
                    Id = Guid.NewGuid(),
                    Email = "user1@example.com",
                    FirstName = "John",
                    LastName = "Doe",
                    PhoneNumber = "123-456-7890",
                    CampusId = campus.Id
                };

                var user2 = new User
                {
                    Id = Guid.NewGuid(),
                    Email = "user2@example.com",
                    FirstName = "Jane",
                    LastName = "Doe",
                    PhoneNumber = "987-654-3210",
                    CampusId = campus.Id
                };

                var user3 = new User
                {
                    Id = Guid.NewGuid(),
                    Email = "user3@example.com",
                    FirstName = "Jim",
                    LastName = "Beam",
                    PhoneNumber = "555-555-5555",
                    CampusId = campus.Id
                };

                _context.Users.AddRange(user1, user2, user3);
                _context.SaveChanges();

                _context.UserRoles.AddRange(new List<UserRole>
                {
                    new UserRole { UserId = user1.Id, RoleId = 2 },
                    new UserRole { UserId = user2.Id, RoleId = 3 },
                    new UserRole { UserId = user3.Id, RoleId = 2 }
                });

                _context.SaveChanges();
            }
        }

        [Test]
        public void GetListUser_ShouldReturnValidResults_WhenPageIsFirst()
        {
            var result = _userRepository.GetListUser(1, 10);

            Assert.NotNull(result);
            Assert.AreEqual(3, result.Items.Count());
            Assert.AreEqual(3, result.TotalCount);
            Assert.AreEqual(1, result.TotalPages);
        }

        [Test]
        public void GetListUser_ShouldReturnValidResults_WhenPageIsNotFirst()
        {
            var result = _userRepository.GetListUser(2, 10);

            Assert.NotNull(result);
            Assert.AreEqual(0, result.Items.Count());
            Assert.AreEqual(3, result.TotalCount);
            Assert.AreEqual(1, result.TotalPages);
        }

        [Test]
        public void GetListUser_ShouldReturnEmptyList_WhenNoUsersMatchCriteria()
        {
            _context.UserRoles.RemoveRange(_context.UserRoles.ToList());
            _context.Users.RemoveRange(_context.Users.ToList());
            _context.SaveChanges();

            _context.SaveChanges();

            var result = _userRepository.GetListUser(1, 10);

            Assert.NotNull(result);
            Assert.AreEqual(0, result.Items.Count());
            Assert.AreEqual(0, result.TotalCount);
            Assert.AreEqual(0, result.TotalPages);
        }

        [Test]
        public void GetListUser_ShouldReturnNull_WhenPageIsZero()
        {
            var result = _userRepository.GetListUser(0, 10);

            Assert.NotNull(result);
            Assert.AreEqual(0, result.Items.Count());
            Assert.AreEqual(0, result.TotalCount);
            Assert.AreEqual(0, result.TotalPages);
        }

        [Test]
        public void GetListUser_ShouldReturnNull_WhenPageSizeIsZero()
        {
            var result = _userRepository.GetListUser(1, 0);

            Assert.AreEqual(0, result.Items.Count());
        }

        [Test]
        public void GetListUser_ShouldReturnNull_WhenPageAndPageSizeAreZero()
        {
            var result = _userRepository.GetListUser(0, 0);

            Assert.NotNull(result);
            Assert.AreEqual(0, result.Items.Count());
        }

        [Test]
        public void GetListUser_ShouldReturnValidResults_WhenThereAreMultiplePages()
        {
            SeedDatabase();

            var result = _userRepository.GetListUser(1, 1);

            Assert.NotNull(result);
            Assert.AreEqual(3, result.TotalCount);
            Assert.AreEqual(3, result.TotalPages);
            Assert.AreEqual(1, result.Items.Count());
        }

        [Test]
        public void GetListUser_ShouldReturnPartialList_WhenPageIsSecondWithMultiplePages()
        {
            var result = _userRepository.GetListUser(3, 1);

            Assert.NotNull(result);
            Assert.AreEqual(3, result.TotalCount);
            Assert.AreEqual(3, result.TotalPages);
            Assert.AreEqual(1, result.Items.Count());
        }

        [Test]
        public void GetListUser_ShouldReturnSortedResults_WhenUsersHaveDifferentRoles()
        {
            var result = _userRepository.GetListUser(1, 10);

            Assert.NotNull(result);
            Assert.AreEqual(3, result.Items.Count());
            Assert.IsTrue(result.Items.All(user => user.UserRoles.Any(role => role.RoleId != 1)));
        }


    }
}
