using DALCore;
using DALCore.DB;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace BackEndChallengeTest.DALUnitTest
{
    public class UserDataAccessTest
    {
        UserDataAccess dt;
        GenesisChallengeContext db;

        public UserDataAccessTest()
        {
            var options = new DbContextOptionsBuilder<GenesisChallengeContext>()
                .UseInMemoryDatabase(databaseName: "Test")
                .Options;

            db = new GenesisChallengeContext(options);

            ILoggerFactory loggerFactory = new LoggerFactory()
                         .AddConsole()
                         .AddDebug();
            ILogger logger = loggerFactory.CreateLogger<Program>();


            dt = new UserDataAccess(db, logger);
        }

        [Fact]
        public async Task GetUserByEmailAsyncTest()
        {
            // Arrange

            Users user = new Users
            {
                Email = "mojtabamontazery@gmail.com",
                UserId = Guid.NewGuid()
            };

            await db.Users.AddAsync(user);
            await db.SaveChangesAsync();


            // Act
            Users u1 = await dt.GetUserByEmailAsync(user.Email);
            Users u2 = await dt.GetUserByEmailAsync("saf@df.df");

            //Assert
            Assert.Equal(user.Email,u1.Email);
            Assert.Null(u2);
        }

        [Fact]
        public async Task CreateUserAsyncTest()
        {
            // Arrange
            Users user = new Users
            {
                Email = "mojtabamontazery@gmail.com",
                UserId = Guid.NewGuid(),
                CreatedOn = DateTime.Now,
                Name = "Mojtaba",
            };

            //Act
            await dt.CreateUserAsync(user);

            // Assert
            Users u1 = await db.Users.FirstAsync(ent => ent.UserId == user.UserId);
            Assert.Equal(user.Email, u1.Email);
        }

        [Fact]
        public async Task GetUserByIdAsync()
        {
            // Arrange
            Users user = new Users
            {
                Email = "mojtabamontazery@gmail.com",
                UserId = Guid.NewGuid(),
                CreatedOn = DateTime.Now,
                Name = "Mojtaba",
            };

            await db.Users.AddAsync(user);
            await db.SaveChangesAsync();

            // Act
            Users u1 = await dt.GetUserByIdAsync(user.UserId);
            Users u2 = await dt.GetUserByIdAsync(Guid.NewGuid());

            //Assert
            Assert.Equal(u1.UserId, user.UserId);
            Assert.Null(u2);
        }

        [Fact]
        public async Task UserLoggedInTest()
        {
            // Arrange
            Users user = new Users
            {
                Email = "mojtabamontazery@gmail.com",
                UserId = Guid.NewGuid(),
                CreatedOn = DateTime.Now.AddDays(-3),
                Name = "Mojtaba",
                LastLoginOn= DateTime.Now.AddDays(-3),
            };

            await db.Users.AddAsync(user);
            await db.SaveChangesAsync();

            //Act
            Users u1 = await dt.UserLoggedIn(user.UserId);
            Users u2 = await dt.UserLoggedIn(Guid.NewGuid());

            // Assert
            Assert.Equal(user.Email, u1.Email);
            Assert.True(u1.LastLoginOn > DateTime.Now.AddDays(-2));
            Assert.Null(u2);
        }
    }
}
