using BackEndChallenge.Utilities;
using BLLCore;
using BLLCore.Repositories;
using DALCore;
using DALCore.DB;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace BackEndChallengeTest.BLLUnitTest
{
    public class UserRepositoryTest
    {
        UserRepository repos;
        Mock<IUserDataAccess> serviceMock;

        // Arrange 
        public UserRepositoryTest()
        {
            serviceMock = new Mock<IUserDataAccess>();
           
            Users u1=new Users()
            {
                UserId = Guid.Parse("30a480fe-72d5-494f-affc-9a42dd9fb2fd"),
                Email = "m@g.com",
                Name="name",
                PasswordHash= PasswordStorage.CreateHash("password")
            };

            Users u2 = new Users()
            {
                UserId = Guid.NewGuid(),
                Email = "notExists@g.com",
                Name = "name",
                PasswordHash = PasswordStorage.CreateHash("password")
            };

            serviceMock.Setup(dt => dt.GetUserByEmailAsync(u1.Email)).ReturnsAsync(u1);
            serviceMock.Setup(dt => dt.GetUserByEmailAsync(u2.Email)).ReturnsAsync((Users)null);

            serviceMock.Setup(dt => dt.CreateUserAsync(u1)).ReturnsAsync((Users)null);
            serviceMock.Setup(dt => dt.CreateUserAsync(u2)).ReturnsAsync(u2);

            serviceMock.Setup(dt => dt.GetUserByIdAsync(u1.UserId)).ReturnsAsync(u1);
            serviceMock.Setup(dt => dt.GetUserByIdAsync(u2.UserId)).ReturnsAsync((Users)null);


            serviceMock.Setup(dt => dt.UserLoggedIn(u1.UserId)).ReturnsAsync(u1);
            serviceMock.Setup(dt => dt.UserLoggedIn(u2.UserId)).ReturnsAsync((Users)null);

            ILoggerFactory loggerFactory = new LoggerFactory();
            loggerFactory.AddConsole();
            loggerFactory.AddDebug();
            ILogger logger = loggerFactory.CreateLogger<Program>();

            repos = new UserRepository(serviceMock.Object, logger);
        }

        [Fact]
        public async Task UserAlreadyExistsTest()
        {
            //Act
            bool b1 = await repos.UserAlreadyExists("m@g.com");
            bool b2 = await repos.UserAlreadyExists("notExists@g.com");

            // Assert
            Assert.True(b1);
            Assert.False(b2);
        }

        //[Fact]
        //public async Task CreateUserTest()
        //{
        //    // Act
        //    Users u1 = await repos.CreateUser("name", "notExists@g.com", "password", JwtManager.GenerateToken("name"), new string[] { });
        //    Users u2 = await repos.CreateUser("name", "m@g.com", "password", JwtManager.GenerateToken("name"), new string[] { });

        //    // Assert
        //    Assert.Equal("notExists@g.com", u1.Email);
        //    Assert.Null(u2);
        //}

        [Fact]
        public async Task SignInUserAsyncTest()
        {
            // Act
            Users u1 = await repos.SignInUserAsync("notExists@g.com", "password");
            Users u2 = await repos.SignInUserAsync("m@g.com", "password");
            Users u3 = await repos.SignInUserAsync("m@g.com", "wrongPassword");

            // Assert
            Assert.Equal("m@g.com", u2.Email);
            Assert.Null(u1);
            Assert.Null(u3);
        }

        [Fact]
        public async Task GetUserByIdAsyncTest()
        {
            // Act
            Users u1 = await repos.GetUserByIdAsync(Guid.Parse("30a480fe-72d5-494f-affc-9a42dd9fb2fd"));
            Users u2 = await repos.GetUserByIdAsync(new Guid());

            Assert.Equal("m@g.com", u1.Email);
            Assert.Null(u2);
        }
    }
}
