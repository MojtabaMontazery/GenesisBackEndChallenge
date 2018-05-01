using BackEndChallenge;
using BackEndChallenge.Controllers;
using BackEndChallenge.Utilities;
using BLLCore;
using BLLCore.Repositories;
using DALCore.DB;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace BackEndChallengeTest.ControllerTest
{
    public class UserControllerTest
    {
        Mock<IUserRepository> serviceMock;
        UsersController controller;

        public UserControllerTest()
        {
            // Arrange
            serviceMock = new Mock<IUserRepository>();

            Users u1 = new Users()
            {
                UserId = Guid.Parse("30a480fe-72d5-494f-affc-9a42dd9fb2fd"),
                Email = "m@g.com",
                Name = "name",
                PasswordHash = PasswordStorage.CreateHash("password"),
                Token=JwtManager.GenerateToken("name"),
                LastLoginOn=DateTime.Now
            };

            Users u2 = new Users()
            {
                UserId = Guid.Parse("10a480fe-72d5-494f-affc-9a42dd9fb2ef"),
                Email = "notExists@g.com",
                Name = "name",
                PasswordHash = PasswordStorage.CreateHash("password"),
                Token= JwtManager.GenerateToken("name")
            };
            

            serviceMock.Setup(dt => dt.CreateUser(u1.Name,u1.Email,"password",u1.Token, It.IsAny<string[]>())).ReturnsAsync((Users)null);
            serviceMock.Setup(dt => dt.CreateUser(u2.Name, u2.Email, "password", u2.Token, It.IsAny<string[]>())).ReturnsAsync(u2);

            serviceMock.Setup(dt => dt.GetUserByIdAsync(u1.UserId)).ReturnsAsync(u1);
            serviceMock.Setup(dt => dt.GetUserByIdAsync(u2.UserId)).ReturnsAsync((Users)null);

            serviceMock.Setup(dt => dt.SignInUserAsync(u1.Email,"password")).ReturnsAsync(u1);
            serviceMock.Setup(dt => dt.SignInUserAsync(u1.Email, "WrongPassword")).ReturnsAsync(u1);
            serviceMock.Setup(dt => dt.SignInUserAsync(u2.Email, "password")).ReturnsAsync((Users)null);


            serviceMock.Setup(dt => dt.UserAlreadyExists(u1.Email)).ReturnsAsync(true);
            serviceMock.Setup(dt => dt.UserAlreadyExists(u2.Email)).ReturnsAsync(false);

            controller = new UsersController(serviceMock.Object, null);
        }

        [Fact]
        public async Task SignUpTest()
        {
            // Arrange
            var input = JObject.FromObject(new
            {
                name = "name",
                email = "m@g.com",
                password = "password",
                telephones = new string[] { },
            });
            var input2 = JObject.FromObject(new
            {
                name = "name",
                email = "notExists@g.com",
                password = "password",
                telephones = new string[] { },
            });

            // Act
            var response = await controller.SignUp(input);
            var response2 = await controller.SignUp(input2);

            // Assert
            Assert.Equal("E-mail already exists.", JObject.FromObject(response)["Value"]["message"].ToString());
            Assert.Equal("notExists@g.com", JObject.FromObject(response2)["Value"]["email"].ToString());
        }

        [Fact]
        public async Task SignInTest()
        {
            // Arrange
            var input = JObject.FromObject(new
            {
                email = "m@g.com",
                password = "password",
            });
            var input2 = JObject.FromObject(new
            {
                email = "notExists@g.com",
                password = "password",
            });
            var input3 = JObject.FromObject(new
            {
                email = "m@g.com",
                password = "wrongPassword",
            });

            // Act
            var response = await controller.SignIn(input);
            var response2 = await controller.SignIn(input2);
            var response3 = await controller.SignIn(input3);

            // Assert
            Assert.Equal("Invalid user and / or password.", JObject.FromObject(response3)["Value"]["message"].ToString());
            Assert.Equal("Invalid user and / or password.", JObject.FromObject(response2)["Value"]["message"].ToString());
            Assert.Equal("30a480fe-72d5-494f-affc-9a42dd9fb2fd", JObject.FromObject(response)["Value"]["id"].ToString());
        }

        [Fact]
        public async Task SearchForUserTest()
        {
            // Act
            var response = await controller.SearchForUser(Guid.Parse("30a480fe-72d5-494f-affc-9a42dd9fb2fd"), "Bearer " + JwtManager.GenerateToken("name"));
            var response2 = await controller.SearchForUser(Guid.Parse("10a480fe-72d5-494f-affc-9a42dd9fb2ef"), "Bearer " + JwtManager.GenerateToken("name"));

            // Assert
            Assert.Equal("There is no such a user.", JObject.FromObject(response2)["Value"]["message"].ToString());
            Assert.Equal("name", JObject.FromObject(response)["Value"]["name"].ToString());
        }
    }
}
