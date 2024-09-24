using Crud_Operation.Controllers;
using Crud_Operation.Model;
using Crud_Operation.Services.Interface;
using Crud_Operation.Services.Token;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace TestProjectCrudOperation.Controller
{
    public class UserControllerTest
    {
        private readonly UserController _controller;
        private readonly Mock<IUserService> _mockUserService;
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly Mock<ITokenService> _mockTokenService;


        public UserControllerTest()
        {
            _mockUserService = new Mock<IUserService>();
            _mockConfiguration = new Mock<IConfiguration>();
            _mockTokenService = new Mock<ITokenService>();
            _controller = new UserController(_mockConfiguration.Object, _mockUserService.Object, _mockTokenService.Object);
        }

        #region Getall

        // positive
        [Fact]
        public async Task GetAllTest_ReturnsOkResult_WithListOfUsers()
        {
            // Arrange
            var mockUsers = new List<User>
            {
                new User { Id = 1, FirstName = "string" ,LastName = "string", Email="ee@example.com", Password = "string", PhoneNumber = "1111111111" },
            };

            _mockUserService.Setup(service => service.GetAll())
                .ReturnsAsync(mockUsers);

            // Act
            var result = await _controller.GetAll();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var responseData = Assert.IsType<ResponseData>(okResult.Value);
            var users = Assert.IsType<List<User>>(responseData.data);

            Assert.True(responseData.success);
            Assert.Equal("Get All Users succesfully", responseData.message);
            Assert.Equal(200, responseData.code);
            //Assert.Equal(6, users.Count);
        }

        //negative
        [Fact]
        public async Task GetAllTest_ReturnsOkResult_WithEmptyListOfUsers()
        {
            // Arrange
            var mockUsers = new List<User>();

            _mockUserService.Setup(service => service.GetAll())
                .ReturnsAsync(mockUsers);

            // Act
            var result = await _controller.GetAll();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var responseData = Assert.IsType<ResponseData>(okResult.Value);
            var users = Assert.IsType<List<User>>(responseData.data);

            Assert.True(responseData.success);
            Assert.Equal("Get All Users succesfully", responseData.message);
            Assert.Equal(200, responseData.code);
            Assert.Empty(users);
        }

        // exception
        [Fact]
        public async Task GetAllTest_ReturnsServerError_WhenExceptionThrown()
        {
            // Arrange
            _mockUserService.Setup(service => service.GetAll())
                .ThrowsAsync(new System.Exception("Service exception"));

            // Act
            var result = await _controller.GetAll();

            // Assert
            var okResult = Assert.IsType<ObjectResult>(result.Result);
            var responseData = Assert.IsType<ResponseData>(okResult.Value);

            Assert.False(responseData.success);
            Assert.Equal("Service exception", responseData.message);
            Assert.Equal(500, responseData.code);
        }
        #endregion

        #region GetById
        // positive
        [Fact]
        public async Task GetByIdTest_ReturnsOkResult_WithUser()
        {
            // Arrange
            var userId = 1;
            var mockUser = new User
            {
                Id = userId,
                FirstName = "string",
                LastName = "string",
                Email = "ee@example.com",
                Password = "string",
                PhoneNumber = "1111111111"
            };

            _mockTokenService.Setup(service => service.GetUserIdFromToken(It.IsAny<string>()))
                .Returns(userId.ToString());

            _mockUserService.Setup(service => service.GetById(userId))
                .ReturnsAsync(mockUser);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    Request =
            {
                Headers =
                {
                    ["Authorization"] = $"Bearer some_valid_token"
                }
            }
                }
            };

            // Act
            var result = await _controller.GetById();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var responseData = Assert.IsType<ResponseData>(okResult.Value);
            var user = Assert.IsType<User>(responseData.data);

            Assert.True(responseData.success);
            Assert.Equal("Get User succesfully", responseData.message);
            Assert.Equal(200, responseData.code);
            Assert.Equal(userId, user.Id);
            Assert.Equal("string", user.FirstName);
            Assert.Equal("string", user.LastName);
            Assert.Equal("ee@example.com", user.Email);
            Assert.Equal("1111111111", user.PhoneNumber);
            Assert.Equal("string", user.Password);
        }

        [Fact]
        public async Task GetByIdTest_ReturnsNotFound_WhenUserDoesNotExist()
        {
            // Arrange
            var userId = 999; // An ID that does not exist
            _mockTokenService.Setup(service => service.GetUserIdFromToken(It.IsAny<string>()))
                .Returns(userId.ToString());

            User mockUser = null;
            _mockUserService.Setup(service => service.GetById(userId))
                .ReturnsAsync(mockUser);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    Request =
            {
                Headers =
                {
                    ["Authorization"] = $"Bearer some_valid_token"
                }
            }
                }
            };

            // Act
            var result = await _controller.GetById();

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            var responseData = Assert.IsType<ResponseData>(notFoundResult.Value);

            Assert.False(responseData.success);
            Assert.Equal("User Not Found!", responseData.message);
            Assert.Equal(404, responseData.code);
            Assert.Null(responseData.data);
        }

        // Exception case
        [Fact]
        public async Task GetByIdTest_ReturnsServerError_WhenExceptionThrown()
        {
            // Arrange
            var userId = 1;
            _mockTokenService.Setup(service => service.GetUserIdFromToken(It.IsAny<string>()))
                .Returns(userId.ToString());

            _mockUserService.Setup(service => service.GetById(userId))
                .ThrowsAsync(new System.Exception("Service exception"));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    Request =
            {
                Headers =
                {
                    ["Authorization"] = $"Bearer some_valid_token"
                }
            }
                }
            };

            // Act
            var result = await _controller.GetById();

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result.Result);
            var responseData = Assert.IsType<ResponseData>(objectResult.Value);

            Assert.False(responseData.success);
            Assert.Equal("Service exception", responseData.message);
            Assert.Equal(500, responseData.code);
        }
        #endregion


        #region Delete
        [Fact]
        public async Task DeleteTest_ReturnsOkResult_WhenUserIsDeletedSuccessfully()
        {
            // Arrange
            var token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYW1laWQiOiIxIiwibmJmIjoxNzI1NDI5MjM5LCJleHAiOjE3MjU0MzAyMzksImlhdCI6MTcyNTQyOTIzOSwiaXNzIjoiSldUQXV0aGVudGljYXRpb25TZXJ2ZXIiLCJhdWQiOiJKV1RTZXJ2aWNlUG9zdG1hbkNsaWVudCJ9.vydxtrnIORhsaX5K2FJZuVreY6iXkOMmqCje_1zUE50";
            var userId = 1;
            var isUserDeleted = true;

            _mockTokenService.Setup(service => service.GetUserIdFromToken(token))
                .Returns(userId.ToString());

            _mockUserService.Setup(service => service.Delete(userId))
                .ReturnsAsync(isUserDeleted);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    Request =
            {
                Headers =
                {
                    ["Authorization"] = $"Bearer {token}"
                }
            }
                }
            };

            // Act
            var result = await _controller.Delete();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var responseData = Assert.IsType<ResponseData>(okResult.Value);

            Assert.True(responseData.success);
            Assert.Equal("User Deleted Successfully", responseData.message);
            Assert.Equal(200, responseData.code);
            Assert.True((bool)responseData.data); // Ensure that data indicates successful deletion
        }

        [Fact]
        public async Task DeleteTest_ReturnsNotFound_WhenUserDoesNotExist()
        {
            // Arrange
            var token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYW1laWQiOiIxIiwibmJmIjoxNzI1NDI5MjM5LCJleHAiOjE3MjU0MzAyMzksImlhdCI6MTcyNTQyOTIzOSwiaXNzIjoiSldUQXV0aGVudGljYXRpb25TZXJ2ZXIiLCJhdWQiOiJKV1RTZXJ2aWNlUG9zdG1hbkNsaWVudCJ9.vydxtrnIORhsaX5K2FJZuVreY6iXkOMmqCje_1zUE50";
            var userId = 1;
            var isUserDeleted = false;

            _mockTokenService.Setup(service => service.GetUserIdFromToken(token))
                .Returns(userId.ToString());

            _mockUserService.Setup(service => service.Delete(userId))
                .ReturnsAsync(isUserDeleted);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    Request =
            {
                Headers =
                {
                    ["Authorization"] = $"Bearer {token}"
                }
            }
                }
            };

            // Act
            var result = await _controller.Delete();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var responseData = Assert.IsType<ResponseData>(okResult.Value);

            Assert.False(responseData.success);
            Assert.Equal("User not found", responseData.message);
            Assert.Equal(404, responseData.code);
            Assert.False((bool)responseData.data); // Ensure that data indicates failure to delete
        }

        [Fact]
        public async Task DeleteTest_ReturnsServerError_WhenExceptionThrown()
        {
            // Arrange
            var token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYW1laWQiOiIxIiwibmJmIjoxNzI1NDMwOTY2LCJleHAiOjE3MjU0MzE5NjYsImlhdCI6MTcyNTQzMDk2NiwiaXNzIjoiSldUQXV0aGVudGljYXRpb25TZXJ2ZXIiLCJhdWQiOiJKV1RTZXJ2aWNlUG9zdG1hbkNsaWVudCJ9.fN5CdulPo-7gwTnXP5eViEqKaxcrDLl9Ytp-WigX2-Y";
            var userId = 1;

            _mockTokenService.Setup(service => service.GetUserIdFromToken(token))
                .Returns(userId.ToString());

            _mockUserService.Setup(service => service.Delete(userId))
                .ThrowsAsync(new System.Exception("Service exception"));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    Request =
                {
                    Headers =
                    {
                        ["Authorization"] = $"Bearer {token}"
                    }
                }
                }
            };

            // Act
            var result = await _controller.Delete();

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result.Result); // Correct the expected type to ObjectResult
            var responseData = Assert.IsType<ResponseData>(objectResult.Value);

            Assert.False(responseData.success);
            Assert.Equal("An error occurred: Service exception", responseData.message);
            Assert.Equal(500, responseData.code);
        }
        #endregion

        #region Update
        [Fact]
        public async Task UpdateTest_ReturnsOkResult_WhenUserIsUpdatedSuccessfully()
        {
            // Arrange
            var token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYW1laWQiOiIxIiwibmJmIjoxNzI1NDMwOTY2LCJleHAiOjE3MjU0MzE5NjYsImlhdCI6MTcyNTQzMDk2NiwiaXNzIjoiSldUQXV0aGVudGljYXRpb25TZXJ2ZXIiLCJhdWQiOiJKV1RTZXJ2aWNlUG9zdG1hbkNsaWVudCJ9.fN5CdulPo-7gwTnXP5eViEqKaxcrDLl9Ytp-WigX2-Y";
            var userId = 1;
            var updatedUser = new User
            {
                Id = userId,
                FirstName = "string",
                LastName = "string",
                Email = "ee@example.com",
                PhoneNumber = "1233211233",
                Password = "string"
            };

            _mockTokenService.Setup(service => service.GetUserIdFromToken(token))
                .Returns(userId.ToString());

            _mockUserService.Setup(service => service.Update(updatedUser))
                .ReturnsAsync(updatedUser);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    Request =
            {
                Headers =
                {
                    ["Authorization"] = $"Bearer {token}"
                }
            }
                }
            };

            // Act
            var result = await _controller.Update(updatedUser);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var responseData = Assert.IsType<ResponseData>(okResult.Value);

            Assert.True(responseData.success);
            Assert.Equal("User updated successfully", responseData.message);
            Assert.Equal(200, responseData.code);

            var user = Assert.IsType<User>(responseData.data);
            Assert.Equal(userId, user.Id);
            Assert.Equal("string", user.FirstName);
            Assert.Equal("string", user.LastName);
            Assert.Equal("ee@example.com", user.Email);
            Assert.Equal("1233211233", user.PhoneNumber);
            Assert.Equal("string", user.Password);
        }

        [Fact]
        public async Task UpdateTest_ReturnsBadRequest_WhenUserUpdateFails()
        {
            // Arrange
            var token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYW1laWQiOiIxIiwibmJmIjoxNzI1NDMwOTY2LCJleHAiOjE3MjU0MzE5NjYsImlhdCI6MTcyNTQzMDk2NiwiaXNzIjoiSldUQXV0aGVudGljYXRpb25TZXJ2ZXIiLCJhdWQiOiJKV1RTZXJ2aWNlUG9zdG1hbkNsaWVudCJ9.fN5CdulPo-7gwTnXP5eViEqKaxcrDLl9Ytp-WigX2-Y";
            var userId = 1;
            var updatedUser = new User
            {
                Id = userId,
                FirstName = "string",
                LastName = "string",
                Email = "ee@example.com",
                PhoneNumber = "1233211233",
                Password = "string"
            };

            _mockTokenService.Setup(service => service.GetUserIdFromToken(token))
                .Returns(userId.ToString());

            _mockUserService.Setup(service => service.Update(updatedUser))
                .ReturnsAsync((User)null);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    Request =
            {
                Headers =
                {
                    ["Authorization"] = $"Bearer {token}"
                }
            }
                }
            };

            // Act
            var result = await _controller.Update(updatedUser);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            var responseData = Assert.IsType<ResponseData>(badRequestResult.Value);

            Assert.False(responseData.success);
            Assert.Equal("User not updated successfully!", responseData.message);
            Assert.Equal(400, responseData.code);
            Assert.Null(responseData.data);
        }

        [Fact]
        public async Task UpdateTest_ReturnsServerError_WhenExceptionThrown()
        {
            // Arrange
            var token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYW1laWQiOiIxIiwibmJmIjoxNzI1NDMwOTY2LCJleHAiOjE3MjU0MzE5NjYsImlhdCI6MTcyNTQzMDk2NiwiaXNzIjoiSldUQXV0aGVudGljYXRpb25TZXJ2ZXIiLCJhdWQiOiJKV1RTZXJ2aWNlUG9zdG1hbkNsaWVudCJ9.fN5CdulPo-7gwTnXP5eViEqKaxcrDLl9Ytp-WigX2-Y";
            var userId = 1;
            var updatedUser = new User
            {
                Id = userId,
                FirstName = "string",
                LastName = "string",
                Email = "ee@example.com",
                PhoneNumber = "1233211233",
                Password = "string"
            };

            _mockTokenService.Setup(service => service.GetUserIdFromToken(token))
                .Returns(userId.ToString());

            _mockUserService.Setup(service => service.Update(updatedUser))
                .ThrowsAsync(new System.Exception("Service exception"));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    Request =
            {
                Headers =
                {
                    ["Authorization"] = $"Bearer {token}"
                }
            }
                }
            };

            // Act
            var result = await _controller.Update(updatedUser);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result.Result);
            var responseData = Assert.IsType<ResponseData>(objectResult.Value);

            Assert.False(responseData.success);
            Assert.Equal("Service exception", responseData.message);
            Assert.Equal(500, responseData.code);
        }
        #endregion
    }
}