using Moq;
using Xunit;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Crud_Operation.Controllers;
using Crud_Operation.Model;
using Crud_Operation.Services.Interface;
using Crud_Operation.Services.Token;
using Crud_Operation.Migrations;
using Crud_Operation.Services.OtpService;

namespace MyProject.Tests
{
    public class AuthControllerTests
    {
        private readonly AuthController _controller;
        private readonly Mock<IAuthService> _mockAuthService;
        private readonly Mock<ITokenService> _mockTokenService;
        private readonly Mock<IotpService> _mockOtpService;

        public AuthControllerTests()
        {
            _mockAuthService = new Mock<IAuthService>();
            _mockTokenService = new Mock<ITokenService>();
            _mockOtpService = new Mock<IotpService>();
            _controller = new AuthController(null, _mockAuthService.Object, _mockTokenService.Object ,_mockOtpService.Object);
        }
        #region Register

        // positive 
        [Fact]
        public async Task Register_ReturnsOkResult_WhenUserIsRegisteredSuccessfully()
        {
            // Arrange
            var user = new User
            {
                Id = 1,
                FirstName = "string",
                LastName = "string",
                Email = "ee@example.com",
                PhoneNumber = "1233211233",
                Password = "string"
            };
            _mockAuthService.Setup(service => service.Register(It.IsAny<User>())).ReturnsAsync(user);

            // Act
            var result = await _controller.Register(user);

            // Assert
            var objectResult = Assert.IsType<OkObjectResult>(result.Result);
            var responseData = Assert.IsType<ResponseData>(objectResult.Value);

            Assert.True(responseData.success);
            Assert.Equal("Register user successfully", responseData.message);
            Assert.Equal(200, responseData.code);
        }

        //negative
        [Fact]
        public async Task Register_ReturnsBadRequest_WhenRegistrationFails()
        {
            // Arrange
            _mockAuthService.Setup(service => service.Register(It.IsAny<User>())).ReturnsAsync((User)null);

            // Act
            var result = await _controller.Register(new User());

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            var responseData = Assert.IsType<ResponseData>(badRequestResult.Value);

            Assert.False(responseData.success);
            Assert.Equal("Problem while registering user!!", responseData.message);
            Assert.Equal(400, responseData.code);
        }

        //exception
        [Fact]
        public async Task Register_ReturnsBadRequest_WhenExceptionThrown()
        {
            // Arrange
            _mockAuthService.Setup(service => service.Register(It.IsAny<User>())).ThrowsAsync(new System.Exception("Service exception"));

            // Act
            var result = await _controller.Register(new User());

            // Assert
            var objectResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            var responseData = Assert.IsType<ResponseData>(objectResult.Value);

            Assert.False(responseData.success);
            Assert.Equal("Invalid data entered: Service exception", responseData.message);
            Assert.Equal(500, responseData.code);
        }
        #endregion

        #region Login

        // Positive 
        [Fact]
        public async Task Login_ReturnsOkResult_WithToken_WhenLoginIsSuccessful()
        {
            // Arrange
            var user = new LoginReponseView
            {
                Id = 1,
                Firstname = "string",
                Lastname = "string",
                Email = "ee@example.com",
                PhoneNumber = "1233211233",
                token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYW1laWQiOiIxIiwibmJmIjoxNzI1NDMwOTY2LCJleHAiOjE3MjU0MzE5NjYsImlhdCI6MTcyNTQzMDk2NiwiaXNzIjoiSldUQXV0aGVudGljYXRpb25TZXJ2ZXIiLCJhdWQiOiJKV1RTZXJ2aWNlUG9zdG1hbkNsaWVudCJ9.fN5CdulPo-7gwTnXP5eViEqKaxcrDLl9Ytp-WigX2-Y",
                refreshtoken = "31upKem0BsQ36hNskDlZbvkRqF8cppyslIOySoSoNEZaWJkfoDuvBzEczbIHd0R956TWnPJ25qnUp7+4t1uhzw=="
            };

            _mockAuthService.Setup(service => service.Login(It.IsAny<LoginViewModel>())).ReturnsAsync(user);
            _mockTokenService.Setup(service => service.GenerateAuthToken(It.IsAny<LoginReponseView>())).Returns(user.token);
            _mockTokenService.Setup(service => service.GenerateRefreshToken()).Returns(user.refreshtoken);

            // Act
            var result = await _controller.Login(new LoginViewModel
            {
                PhoneNumber = "1233211233",
                Password = "string"
            });

            // Assert
            var actionResult = Assert.IsType<ActionResult<ResponseData>>(result);
            var okResult = Assert.IsType<JsonResult>(actionResult.Result);
            var responseData = Assert.IsType<ResponseData>(okResult.Value);

            Assert.True(responseData.success);
            Assert.Equal("Login successfully", responseData.message);
            Assert.Equal(200, responseData.code);

            var data = responseData.data as dynamic;
            Assert.Equal(user.token, data.token);
            Assert.Equal(user.refreshtoken, data.refreshtoken);
        }

        // Negative
        [Fact]
        public async Task Login_ReturnsNotFound_WhenLoginFails()
        {
            // Arrange
            _mockAuthService.Setup(service => service.Login(It.IsAny<LoginViewModel>())).ReturnsAsync((LoginReponseView)null);

            // Act
            var result = await _controller.Login(new LoginViewModel
            {
                PhoneNumber = "nonexistent@user.com",
                Password = "wrongpassword"
            });

            // Assert
            var actionResult = Assert.IsType<ActionResult<ResponseData>>(result);
            var notFoundResult = Assert.IsType<JsonResult>(actionResult.Result);
            var responseData = Assert.IsType<ResponseData>(notFoundResult.Value);

            Assert.False(responseData.success);
            Assert.Equal("The provided mobile number does not match any user in our records!", responseData.message);
            Assert.Equal(404, responseData.code);
        }

        // Exception

        [Fact]
        public async Task Login_ReturnsInternalServerError_WhenExceptionOccurs()
        {
            // Arrange
            _mockAuthService.Setup(service => service.Login(It.IsAny<LoginViewModel>())).ThrowsAsync(new Exception("Some error occurred"));

            // Act
            var result = await _controller.Login(new LoginViewModel
            {
                PhoneNumber = "1233211233",
                Password = "string"
            });

            // Assert
            var actionResult = Assert.IsType<ActionResult<ResponseData>>(result);
            var jsonResult = Assert.IsType<JsonResult>(actionResult.Result);
            var responseData = Assert.IsType<ResponseData>(jsonResult.Value);

            Assert.False(responseData.success);
            Assert.Equal("Invalid data entered: Some error occurred", responseData.message);
            Assert.Equal(500, responseData.code);
        }

        #endregion

        #region RefreshToken

        // Positive 
        [Fact]
        public async Task RefreshToken_ReturnsOkResult_WithNewTokens_WhenRefreshIsSuccessful()
        {
            // Arrange
            var refreshToken = "a5m/1nW/zTyOs7DapNlSMJef6DtBaOMRVZ0eUG/Br5UE+lzUnYWuKSeaeIg1ryrsjDDwMy8KQ8QFZhTm3eaMNQ==";
            var newToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYW1laWQiOiIxIiwibmJmIjoxNzI1NDM3OTI2LCJleHAiOjE3MjU0Mzg5MjYsImlhdCI6MTcyNTQzNzkyNiwiaXNzIjoiSldUQXV0aGVudGljYXRpb25TZXJ2ZXIiLCJhdWQiOiJKV1RTZXJ2aWNlUG9zdG1hbkNsaWVudCJ9.I3bnSg-Kg8ujDWEKM4_lTpcAEFQeDNLpsMIZGsOjieI";
            var newRefreshToken = "dhN3eqx7DWLUn4Lh76IEf8jFjtlo57c+DgOD4R6nnxw2zKCZkuf6f3l9bx8LWeRg6Z/XshQcFv+99sfv1fovfQ==";
            var user = new User { Id = 1 };

            _mockAuthService.Setup(service => service.RefreshToken(refreshToken)).ReturnsAsync(user);
            _mockTokenService.Setup(service => service.GenerateAuthToken(It.IsAny<LoginReponseView>())).Returns(newToken);
            _mockTokenService.Setup(service => service.GenerateRefreshToken()).Returns(newRefreshToken);

            // Act
            var result = await _controller.RefreshToken(refreshToken);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result.Result);
            var responseData = Assert.IsType<ResponseData>(jsonResult.Value);

            Assert.True(responseData.success);
            Assert.Equal("Token refreshed successfully", responseData.message);
            Assert.Equal(200, responseData.code);

            var data = Assert.IsType<TokenResponse>(responseData.data);
            Assert.Equal(newToken, data.Token);
            Assert.Equal(newRefreshToken, data.RefreshToken);
        }

        // Negative 
        [Fact]
        public async Task RefreshToken_ReturnsBadRequest_WhenRefreshTokenIsInvalid()
        {
            // Arrange
            var invalidRefreshToken = "invalid-refresh-token";

            // Setup mocks
            _mockAuthService.Setup(service => service.RefreshToken(invalidRefreshToken)).ReturnsAsync((User)null);

            // Act
            var result = await _controller.RefreshToken(invalidRefreshToken);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result.Result);
            var responseData = Assert.IsType<ResponseData>(jsonResult.Value);

            Assert.False(responseData.success);
            Assert.Equal("Invalid refresh token", responseData.message);
            Assert.Equal(400, responseData.code);
        }

        // Exception
        [Fact]
        public async Task RefreshToken_ReturnsInternalServerError_WhenExceptionIsThrown()
        {
            // Arrange
            var refreshToken = "g4keC/F2x+Zxxy3vNrF2bV0o8pQ7hR5ghqT8bYau53xh7/Iw0gBxhke5bxVobibDxADPF6YQ3v0ZjW+CA3+4PA==";
            _mockAuthService.Setup(service => service.RefreshToken(refreshToken)).ThrowsAsync(new System.Exception("Some error"));

            // Act
            var result = await _controller.RefreshToken(refreshToken);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result.Result);
            var responseData = Assert.IsType<ResponseData>(jsonResult.Value);

            Assert.False(responseData.success);
            Assert.Equal("Error refreshing token: Some error", responseData.message);
            Assert.Equal(500, responseData.code);
        }


        #endregion
    }
}
