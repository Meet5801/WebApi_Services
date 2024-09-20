using Moq;
using Xunit;
using System;
using System.Threading.Tasks;
using Crud_Operation.Model;
using Crud_Operation.Repository.Interface;
using Crud_Operation.Services.Interface;
using Crud_Operation.Services;

public class AuthServiceTests
{
    private readonly Mock<IAuthRepository> _mockAuthRepository;
    private readonly IAuthService _authService;

    public AuthServiceTests()
    {
        _mockAuthRepository = new Mock<IAuthRepository>();
        _authService = new AuthService(_mockAuthRepository.Object);
    }

    #region Register

    // positive
    [Fact]
    public async Task Register_ShouldReturnUser_WhenValidModelIsGiven()
    {
        // Arrange
        var userModel = new User { Id = 1, FirstName = "John", LastName = "Doe", Email = "john.doe@example.com", PhoneNumber = "1234567890" };
        _mockAuthRepository.Setup(repo => repo.Register(It.IsAny<User>())).ReturnsAsync(userModel);

        // Act
        var result = await _authService.Register(userModel);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(userModel.Id, result.Id);
        Assert.Equal(userModel.FirstName, result.FirstName);
        Assert.Equal(userModel.LastName, result.LastName);
        Assert.Equal(userModel.Email, result.Email);
        Assert.Equal(userModel.PhoneNumber, result.PhoneNumber);
    }

    // negative

    [Fact]
    public async Task Register_ShouldReturnNull_WhenInvalidModelIsGiven()
    {
        // Arrange
        User invalidUserModel = null;
        _mockAuthRepository.Setup(repo => repo.Register(It.IsAny<User>())).ReturnsAsync((User)null);

        // Act
        var result = await _authService.Register(invalidUserModel);

        // Assert
        Assert.Null(result);
    }

    // Exception

    [Fact]
    public async Task Register_ShouldThrowException_WhenRepositoryThrowsException()
    {
        // Arrange
        var userModel = new User { Id = 1, FirstName = "John", LastName = "Doe", Email = "john.doe@example.com", PhoneNumber = "1234567890" };
        _mockAuthRepository.Setup(repo => repo.Register(It.IsAny<User>())).ThrowsAsync(new Exception("Repository exception"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _authService.Register(userModel));
    }
    #endregion

    #region Login

    //Positive 
    [Fact]
    public async Task Login_ShouldReturnUser_WhenValidCredentialsAreProvided()
    {
        // Arrange
        var loginViewModel = new LoginViewModel
        {
            PhoneNumber = "1234567890",
            Password = "password123"
        };

        var expectedUser = new LoginReponseView
        {
            Id = 1,
            Firstname = "John",
            Lastname = "Doe",
            Email = "john.doe@example.com",
            PhoneNumber = "1234567890"
        };

        _mockAuthRepository.Setup(repo => repo.Login(It.IsAny<LoginViewModel>()))
            .ReturnsAsync(expectedUser);

        var service = new AuthService(_mockAuthRepository.Object);

        // Act
        var result = await service.Login(loginViewModel);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedUser.Id, result.Id);
        Assert.Equal(expectedUser.Firstname, result.Firstname);
        Assert.Equal(expectedUser.Lastname, result.Lastname);
        Assert.Equal(expectedUser.Email, result.Email);
        Assert.Equal(expectedUser.PhoneNumber, result.PhoneNumber);
    }

    // negative
    [Fact]
    public async Task Login_InvalidUser_ThrowsInvalidOperationException()
    {
        // Arrange
        var loginModel = new LoginViewModel { PhoneNumber = "1234567890", Password = "WrongPassword" };

        _mockAuthRepository.Setup(repo => repo.Login(loginModel)).ReturnsAsync((LoginReponseView)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _authService.Login(loginModel));
        Assert.Equal("Invalid Phonenumber Or PassWord!!.", exception.Message);
    }

    // exception
    [Fact]
    public async Task Login_RepositoryThrowsException_ThrowsException()
    {
        // Arrange
        var loginModel = new LoginViewModel { PhoneNumber = "1234567890", Password = "Password" };

        _mockAuthRepository.Setup(repo => repo.Login(loginModel)).ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(() => _authService.Login(loginModel));
        Assert.Equal("Database error", exception.Message);
    }


    #endregion

    #region RefreshToken

    //positive
    [Fact]
    public async Task RefreshToken_ValidToken_ReturnsUser()
    {
        // Arrange
        var token = "valid_refresh_token";
        var user = new User
        {
            Id = 1,
            FirstName = "Jane",
            LastName = "Smith",
            Email = "jane.smith@example.com",
            PhoneNumber = "9876543210"
        };

        _mockAuthRepository.Setup(repo => repo.RefreshToken(token)).ReturnsAsync(user);

        // Act
        var result = await _authService.RefreshToken(token);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(user.Id, result.Id);
        Assert.Equal(user.FirstName, result.FirstName);
        Assert.Equal(user.LastName, result.LastName);
        Assert.Equal(user.Email, result.Email);
        Assert.Equal(user.PhoneNumber, result.PhoneNumber);
    }

    // Negative 
    [Fact]
    public async Task RefreshToken_InValidToken_ReturnsNull()
    {
        // Arrange
        var token = "valid_refresh_token";

        _mockAuthRepository.Setup(repo => repo.RefreshToken(token)).ReturnsAsync((User)null);

        // Act
        var result = await _authService.RefreshToken(token);

        // Assert
        Assert.Null(result);
    }

    // Exception
    [Fact]
    public async Task RefreshToken_RepositoryThrowsException_ThrowsException()
    {
        // Arrange
        var token = "any_token";

        _mockAuthRepository.Setup(repo => repo.RefreshToken(token)).ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(() => _authService.RefreshToken(token));
        Assert.Equal("Database error", exception.Message);
    }

    #endregion

    #region UpdateRefreshToken

    //positive
    [Fact]
    public async Task UpdateRefreshToken_ValidInputs_CallsRepositoryUpdate()
    {
        // Arrange
        var userId = 1;
        var refreshToken = "new_refresh_token";

        // Act
        await _authService.UpdateRefreshToken(userId, refreshToken);

        // Assert
        _mockAuthRepository.Verify(repo => repo.UpdateRefreshToken(userId, refreshToken), Times.Once);
    }


    // Negative 
    [Theory]
    [InlineData(-1, "valid_refresh_token", "User ID must be greater than zero. (Parameter 'userId')")]
    [InlineData(1, "", "Refresh token cannot be null or empty. (Parameter 'refreshToken')")]
    [InlineData(1, null, "Refresh token cannot be null or empty. (Parameter 'refreshToken')")]
    public async Task UpdateRefreshToken_InvalidInputs_ThrowsArgumentException(int userId, string refreshToken, string expectedMessage)
    {
        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
            _authService.UpdateRefreshToken(userId, refreshToken)
        );
        Assert.Equal(expectedMessage, exception.Message);
    }



    // Exception
    [Fact]
    public async Task UpdateRefreshToken_RepositoryThrowsException_ThrowsException()
    {
        // Arrange
        var userId = 1;
        var refreshToken = "valid_refresh_token";

        _mockAuthRepository.Setup(repo => repo.UpdateRefreshToken(userId, refreshToken)).ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(() => _authService.UpdateRefreshToken(userId, refreshToken));
        Assert.Equal("Database error", exception.Message);
    }

    #endregion
}
