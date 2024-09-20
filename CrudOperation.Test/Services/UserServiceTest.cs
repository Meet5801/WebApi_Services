using Crud_Operation.Model;
using Crud_Operation.Repository.Interface;
using Crud_Operation.Services;
using Crud_Operation.Services.Interface;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrudOperation.Test.Services
{
    public class UserServiceTest
    {
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly IUserService _userService;

        public UserServiceTest()
        {
            _mockUserRepository = new Mock<IUserRepository>();
            _userService = new UserService(_mockUserRepository.Object);
        }

        #region GetAll
        [Fact]
        public async Task GetAll_ReturnsUsers_WhenUsersExist()
        {
            // Arrange
            var users = new List<User>
            {
                new User {  Id = 1,
                            FirstName = "John",
                            LastName = "Doe",
                            Email = "john.doe@example.com",
                            PhoneNumber = "1234567890" 
                },
            };

            _mockUserRepository.Setup(repo => repo.GetAll()).ReturnsAsync(users);

            // Act
            var result = await _userService.GetAll();

            // Assert
            Assert.NotNull(result);
            Assert.Contains(result, u => u.Id == 1);
            Assert.Contains(result, u => u.FirstName == "John");
            Assert.Contains(result, u => u.LastName == "Doe");
            Assert.Contains(result, u => u.Email == "john.doe@example.com");
            Assert.Contains(result, u => u.PhoneNumber == "1234567890");
        }

        [Fact]
        public async Task GetAll_ReturnsEmptyList_WhenNoUsersExist()
        {
            // Arrange
            var users = new List<User>();

            _mockUserRepository.Setup(repo => repo.GetAll()).ReturnsAsync(users);

            // Act
            var result = await _userService.GetAll();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetAll_ThrowsException_WhenRepositoryFails()
        {
            // Arrange
            _mockUserRepository.Setup(repo => repo.GetAll()).ThrowsAsync(new Exception("Repository error"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _userService.GetAll());
            Assert.Equal("Repository error", exception.Message);
        }
        #endregion

        #region getById
        // positive
        [Fact]
        public async Task GetById_ReturnsUser_WhenUserExists()
        {
            // Arrange
            var userId = 1;
            var user = new User { Id = userId, FirstName = "John", LastName = "Doe", Email = "john.doe@example.com",PhoneNumber = "1234567890"
            };

            _mockUserRepository.Setup(repo => repo.GetById(userId)).ReturnsAsync(user);

            // Act
            var result = await _userService.GetById(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(userId, result.Id);
            Assert.Equal("John", result.FirstName);
            Assert.Equal("Doe", result.LastName);
            Assert.Equal("john.doe@example.com", result.Email);
            Assert.Equal("1234567890", result.PhoneNumber);
        }

        // Negative
        [Fact]
        public async Task GetById_ReturnsNull_WhenUserDoesNotExist()
        {
            // Arrange
            var userId = 1;

            _mockUserRepository.Setup(repo => repo.GetById(userId)).ReturnsAsync((User)null);

            // Act
            var result = await _userService.GetById(userId);

            // Assert
            Assert.Null(result);
        }

        // Exception
        [Fact]
        public async Task GetById_ThrowsException_WhenRepositoryFails()
        {
            // Arrange
            var userId = 1;

            _mockUserRepository.Setup(repo => repo.GetById(userId)).ThrowsAsync(new Exception("Repository error"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _userService.GetById(userId));
            Assert.Equal("Repository error", exception.Message);
        }
        #endregion

        #region Update
        // Postive
        [Fact]
        public async Task Update_ReturnsUpdatedUser_WhenUpdateIsSuccessful()
        {
            // Arrange
            var updatedUser = new User
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                PhoneNumber = "123-456-7890",
                Password = "newpassword"
            };

            _mockUserRepository.Setup(repo => repo.Update(It.IsAny<User>())).ReturnsAsync(updatedUser);

            var userToUpdate = new User
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                PhoneNumber = "123-456-7890",
                Password = "oldpassword"
            };

            // Act
            var result = await _userService.Update(userToUpdate);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(updatedUser.Id, result.Id);
            Assert.Equal(updatedUser.FirstName, result.FirstName);
            Assert.Equal(updatedUser.LastName, result.LastName);
            Assert.Equal(updatedUser.Email, result.Email);
            Assert.Equal(updatedUser.PhoneNumber, result.PhoneNumber);
            Assert.Equal(updatedUser.Password, result.Password);
        }

        // Negative
        [Fact]
        public async Task Update_ReturnsSameUser_WhenUpdateIsUnsuccessful()
        {
            // Arrange
            var existingUser = new User
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                PhoneNumber = "123-456-7890",
                Password = "oldpassword"
            };

            // Simulate that the repository returns the same user without any updates.
            _mockUserRepository.Setup(repo => repo.Update(It.IsAny<User>())).ReturnsAsync(existingUser);

            var userToUpdate = new User
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                PhoneNumber = "123-456-7890",
                Password = "newpassword" // Password is different to simulate no actual update.
            };

            // Act
            var result = await _userService.Update(userToUpdate);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(existingUser.Id, result.Id);
            Assert.Equal(existingUser.FirstName, result.FirstName);
            Assert.Equal(existingUser.LastName, result.LastName);
            Assert.Equal(existingUser.Email, result.Email);
            Assert.Equal(existingUser.PhoneNumber, result.PhoneNumber);
            Assert.Equal(existingUser.Password, result.Password);
        }

        // Exception
        [Fact]
        public async Task Update_ThrowsException_WhenRepositoryFails()
        {
            // Arrange
            var userToUpdate = new User
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                PhoneNumber = "123-456-7890",
                Password = "newpassword"
            };

            _mockUserRepository.Setup(repo => repo.Update(It.IsAny<User>())).ThrowsAsync(new Exception("Repository error"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _userService.Update(userToUpdate));
            Assert.Equal("Repository error", exception.Message);
        }
        #endregion

        #region Delete
        // positive
        [Fact]
        public async Task DeleteUser_WhenUserExists()
        {
            // Arrange
            var userId = 1;
            _mockUserRepository.Setup(repo => repo.Delete(userId)).ReturnsAsync(true);

            // Act
            var result = await _userService.Delete(userId);

            // Assert
            Assert.True(result);
           // _mockUserRepository.Verify(repo => repo.Delete(userId), Times.Once); 


        }

        // Negative
        [Fact]
        public async Task delete_ReturnsNull_WhenUserDoesNotExist()
        {
            // Arrange
            var userId = 1;

            _mockUserRepository.Setup(repo => repo.Delete(userId)).ReturnsAsync(false); 

            // Act
            var result = await _userService.Delete(userId);

            // Assert
            Assert.False(result);
            //_mockUserRepository.Verify(repo => repo.Delete(userId), Times.Once);
        }

        // Exception
        [Fact]
        public async Task Delete_ThrowsException_WhenRepositoryFails()
        {
            // Arrange
            var userId = 1;

            _mockUserRepository.Setup(repo => repo.Delete(userId)).ThrowsAsync(new Exception("Repository error"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _userService.Delete(userId));
            Assert.Equal("Repository error", exception.Message);
        }
        #endregion
    }
}
