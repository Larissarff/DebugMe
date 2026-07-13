using DebugMeBackend.DTOs.User;
using DebugMeBackend.Entities;
using DebugMeBackend.Repositories.Interfaces;
using DebugMeBackend.Services;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;

namespace DebugMeBackend.Tests.Services
{
    public class UserServiceTests
    {
        private static IConfiguration CreateConfiguration()
        {
            Mock<IConfiguration> mock = new Mock<IConfiguration>();

            mock.Setup(c => c["Jwt:Secret"])
                .Returns("ThisIsASuperSecretKeyForTestingPurposesAtLeast32Chars!");
            mock.Setup(c => c["Jwt:Issuer"])
                .Returns("DebugMe");
            mock.Setup(c => c["Jwt:Audience"])
                .Returns("DebugMe");
            mock.Setup(c => c["Jwt:AccessTokenExpiryMinutes"])
                .Returns("60");
            mock.Setup(c => c["Jwt:RefreshTokenExpiryDays"])
                .Returns("7");

            return mock.Object;
        }

        private static UserService CreateUserService(
            Mock<IUserRepository>? userRepositoryMock = null,
            IConfiguration? configuration = null)
        {
            return new UserService(
                (userRepositoryMock ?? new Mock<IUserRepository>()).Object,
                configuration ?? CreateConfiguration());
        }

        // POST TESTS

        [Fact]
        public async Task CreateAsync_ShouldCreateUserSuccessfully_WhenDataIsValid()
        {
            CreateUserDto dto = new CreateUserDto
            {
                Name = "Larissa",
                Email = "larissa@email.com",
                Password = "123456"
            };

            Mock<IUserRepository> userRepositoryMock = new Mock<IUserRepository>();

            userRepositoryMock
                .Setup(repository => repository.GetByEmailAsync(dto.Email))
                .ReturnsAsync((User?)null);

            userRepositoryMock
                .Setup(repository => repository.AddAsync(It.IsAny<User>()))
                .Returns(Task.CompletedTask);

            UserService userService = CreateUserService(userRepositoryMock);

            TokenResponseDto result = await userService.CreateAsync(dto);

            result.Should().NotBeNull();
            result.Token.Should().NotBeNullOrEmpty();
            result.RefreshToken.Should().NotBeNullOrEmpty();
            result.User.Name.Should().Be("Larissa");
            result.User.Email.Should().Be("larissa@email.com");
            result.User.Id.Should().NotBeEmpty();
            result.User.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));

            userRepositoryMock.Verify(repository => repository.GetByEmailAsync(dto.Email), Times.Once);
            userRepositoryMock.Verify(repository => repository.AddAsync(It.IsAny<User>()), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_ShouldHashPasswordWithBCrypt()
        {
            CreateUserDto dto = new CreateUserDto
            {
                Name = "Larissa",
                Email = "larissa@email.com",
                Password = "123456"
            };

            User? capturedUser = null;

            Mock<IUserRepository> userRepositoryMock = new Mock<IUserRepository>();

            userRepositoryMock
                .Setup(repository => repository.GetByEmailAsync(dto.Email))
                .ReturnsAsync((User?)null);

            userRepositoryMock
                .Setup(repository => repository.AddAsync(It.IsAny<User>()))
                .Callback<User>(user => capturedUser = user)
                .Returns(Task.CompletedTask);

            UserService userService = CreateUserService(userRepositoryMock);

            TokenResponseDto result = await userService.CreateAsync(dto);

            result.Should().NotBeNull();

            capturedUser.Should().NotBeNull();
            capturedUser!.PasswordHash.Should().StartWith("$2");

            BCrypt.Net.BCrypt.Verify(dto.Password, capturedUser.PasswordHash).Should().BeTrue();

            userRepositoryMock.Verify(repository => repository.GetByEmailAsync(dto.Email), Times.Once);
            userRepositoryMock.Verify(repository => repository.AddAsync(It.IsAny<User>()), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_ShouldThrowInvalidOperationException_WhenEmailAlreadyExists()
        {
            CreateUserDto dto = new CreateUserDto
            {
                Name = "Larissa",
                Email = "larissa@email.com",
                Password = "123456"
            };

            Mock<IUserRepository> userRepositoryMock = new Mock<IUserRepository>();

            userRepositoryMock
                .Setup(repository => repository.GetByEmailAsync(dto.Email))
                .ReturnsAsync(new User());

            UserService userService = CreateUserService(userRepositoryMock);

            Func<Task<TokenResponseDto>> act = () => userService.CreateAsync(dto);

            await act.Should().ThrowAsync<InvalidOperationException>();
        }

        [Fact]
        public async Task CreateAsync_ShouldTrimAndNormalizeEmail_WhenCreatingUser()
        {
            CreateUserDto dto = new CreateUserDto
            {
                Name = "Larissa",
                Email = "  Larissa@Email.com  ",
                Password = "123456"
            };

            Mock<IUserRepository> userRepositoryMock = new Mock<IUserRepository>();

            userRepositoryMock
                .Setup(repository => repository.GetByEmailAsync("larissa@email.com"))
                .ReturnsAsync((User?)null);

            userRepositoryMock
                .Setup(repository => repository.AddAsync(It.IsAny<User>()))
                .Returns(Task.CompletedTask);

            UserService userService = CreateUserService(userRepositoryMock);

            TokenResponseDto result = await userService.CreateAsync(dto);

            result.Should().NotBeNull();
            result.User.Name.Should().Be("Larissa");
            result.User.Email.Should().Be("larissa@email.com");
            result.User.Id.Should().NotBeEmpty();
            result.User.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));

            userRepositoryMock.Verify(repository => repository.GetByEmailAsync("larissa@email.com"), Times.Once);
            userRepositoryMock.Verify(repository => repository.AddAsync(It.IsAny<User>()), Times.Once);
        }

        // GET TESTS

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllUsers_WhenUsersExist()
        {
            List<User> users = new List<User>
            {
                new User { Id = Guid.NewGuid(), Name = "Larissa", Email = "larissa@email.com", CreatedAt = DateTime.UtcNow },
                new User { Id = Guid.NewGuid(), Name = "Jhonathan", Email = "jhonathan@email.com", CreatedAt = DateTime.UtcNow }
            };

            Mock<IUserRepository> userRepositoryMock = new Mock<IUserRepository>();

            userRepositoryMock
                .Setup(repository => repository.GetAllAsync())
                .ReturnsAsync(users);

            UserService userService = CreateUserService(userRepositoryMock);

            List<UserResponseDto> result = await userService.GetAllAsync();

            result.Should().NotBeNull();
            result.Count.Should().Be(2);
            result[0].Name.Should().Be("Larissa");
            result[1].Name.Should().Be("Jhonathan");

            userRepositoryMock.Verify(repository => repository.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnEmptyList_WhenNoUsersExist()
        {
            List<User> users = new List<User>();

            Mock<IUserRepository> userRepositoryMock = new Mock<IUserRepository>();

            userRepositoryMock
                .Setup(repository => repository.GetAllAsync())
                .ReturnsAsync(users);

            UserService userService = CreateUserService(userRepositoryMock);

            List<UserResponseDto> result = await userService.GetAllAsync();

            result.Should().NotBeNull();
            result.Count.Should().Be(0);

            userRepositoryMock.Verify(repository => repository.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnUser_WhenUserExists()
        {
            Guid userId = Guid.NewGuid();
            User user = new User { Id = userId, Name = "Larissa", Email = "larissa@email.com", CreatedAt = DateTime.UtcNow };

            Mock<IUserRepository> userRepositoryMock = new Mock<IUserRepository>();

            userRepositoryMock
                .Setup(repository => repository.GetByIdAsync(userId))
                .ReturnsAsync(user);

            UserService userService = CreateUserService(userRepositoryMock);

            UserResponseDto? result = await userService.GetByIdAsync(userId);

            result.Should().NotBeNull();
            result.Id.Should().Be(userId);
            result.Name.Should().Be("Larissa");
            result.Email.Should().Be("larissa@email.com");

            userRepositoryMock.Verify(repository => repository.GetByIdAsync(userId), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenUserDoesNotExist()
        {
            Guid userId = Guid.NewGuid();

            Mock<IUserRepository> userRepositoryMock = new Mock<IUserRepository>();

            userRepositoryMock
            .Setup(repository => repository.GetByIdAsync(userId))
            .ReturnsAsync((User?)null);

            UserService userService = CreateUserService(userRepositoryMock);

            UserResponseDto? result = await userService.GetByIdAsync(userId);

            result.Should().BeNull();

            userRepositoryMock.Verify(repository => repository.GetByIdAsync(userId), Times.Once);
        }

        // LOGIN TESTS

        [Fact]
        public async Task LoginAsync_ShouldReturnToken_WhenPasswordIsCorrect()
        {
            LoginUserDto dto = new LoginUserDto
            {
                Email = "larissa@email.com",
                Password = "123456"
            };

            string hashedPassword = BCrypt.Net.BCrypt.HashPassword("123456");
            Guid userId = Guid.NewGuid();

            User user = new User
            {
                Id = userId,
                Name = "Larissa",
                Email = "larissa@email.com",
                PasswordHash = hashedPassword,
                CreatedAt = DateTime.UtcNow
            };

            Mock<IUserRepository> userRepositoryMock = new Mock<IUserRepository>();

            userRepositoryMock
                .Setup(repository => repository.GetByEmailAsync(dto.Email))
                .ReturnsAsync(user);

            userRepositoryMock
                .Setup(repository => repository.UpdateAsync(It.IsAny<User>()))
                .Returns(Task.CompletedTask);

            UserService userService = CreateUserService(userRepositoryMock);

            TokenResponseDto? result = await userService.LoginAsync(dto);

            result.Should().NotBeNull();
            result!.Token.Should().NotBeNullOrEmpty();
            result.RefreshToken.Should().NotBeNullOrEmpty();
            result.ExpiresAt.Should().BeAfter(DateTime.UtcNow);
            result.User.Should().NotBeNull();
            result.User.Id.Should().Be(userId);
            result.User.Name.Should().Be("Larissa");
            result.User.Email.Should().Be("larissa@email.com");

            userRepositoryMock.Verify(repository => repository.GetByEmailAsync(dto.Email), Times.Once);
            userRepositoryMock.Verify(repository => repository.UpdateAsync(It.IsAny<User>()), Times.Once);
        }

        [Fact]
        public async Task LoginAsync_ShouldReturnNull_WhenPasswordIsIncorrect()
        {
            LoginUserDto dto = new LoginUserDto
            {
                Email = "larissa@email.com",
                Password = "wrongpassword"
            };

            string hashedPassword = BCrypt.Net.BCrypt.HashPassword("123456");

            User user = new User
            {
                Id = Guid.NewGuid(),
                Name = "Larissa",
                Email = "larissa@email.com",
                PasswordHash = hashedPassword,
                CreatedAt = DateTime.UtcNow
            };

            Mock<IUserRepository> userRepositoryMock = new Mock<IUserRepository>();

            userRepositoryMock
                .Setup(repository => repository.GetByEmailAsync(dto.Email))
                .ReturnsAsync(user);

            UserService userService = CreateUserService(userRepositoryMock);

            TokenResponseDto? result = await userService.LoginAsync(dto);

            result.Should().BeNull();

            userRepositoryMock.Verify(repository => repository.GetByEmailAsync(dto.Email), Times.Once);
        }

        [Fact]
        public async Task LoginAsync_ShouldReturnNull_WhenUserDoesNotExist()
        {
            LoginUserDto dto = new LoginUserDto
            {
                Email = "nonexistent@email.com",
                Password = "123456"
            };

            Mock<IUserRepository> userRepositoryMock = new Mock<IUserRepository>();

            userRepositoryMock
                .Setup(repository => repository.GetByEmailAsync(dto.Email))
                .ReturnsAsync((User?)null);

            UserService userService = CreateUserService(userRepositoryMock);

            TokenResponseDto? result = await userService.LoginAsync(dto);

            result.Should().BeNull();

            userRepositoryMock.Verify(repository => repository.GetByEmailAsync(dto.Email), Times.Once);
        }

        // REFRESH TOKEN TESTS

        [Fact]
        public async Task RefreshTokenAsync_ShouldReturnNewTokens_WhenRefreshTokenIsValid()
        {
            string oldRefreshToken = "valid-refresh-token";
            User user = new User
            {
                Id = Guid.NewGuid(),
                Name = "Larissa",
                Email = "larissa@email.com",
                CreatedAt = DateTime.UtcNow,
                RefreshToken = oldRefreshToken,
                RefreshTokenExpiry = DateTime.UtcNow.AddDays(1)
            };

            Mock<IUserRepository> userRepositoryMock = new Mock<IUserRepository>();

            userRepositoryMock
                .Setup(repository => repository.GetByRefreshTokenAsync(oldRefreshToken))
                .ReturnsAsync(user);

            userRepositoryMock
                .Setup(repository => repository.UpdateAsync(It.IsAny<User>()))
                .Returns(Task.CompletedTask);

            UserService userService = CreateUserService(userRepositoryMock);

            TokenResponseDto? result = await userService.RefreshTokenAsync(oldRefreshToken);

            result.Should().NotBeNull();
            result!.Token.Should().NotBeNullOrEmpty();
            result.RefreshToken.Should().NotBeNullOrEmpty();
            result.RefreshToken.Should().NotBe(oldRefreshToken);
            result.ExpiresAt.Should().BeAfter(DateTime.UtcNow);

            userRepositoryMock.Verify(repository => repository.GetByRefreshTokenAsync(oldRefreshToken), Times.Once);
            userRepositoryMock.Verify(repository => repository.UpdateAsync(It.IsAny<User>()), Times.Once);
        }

        [Fact]
        public async Task RefreshTokenAsync_ShouldReturnNull_WhenRefreshTokenNotFound()
        {
            Mock<IUserRepository> userRepositoryMock = new Mock<IUserRepository>();

            userRepositoryMock
                .Setup(repository => repository.GetByRefreshTokenAsync("invalid-token"))
                .ReturnsAsync((User?)null);

            UserService userService = CreateUserService(userRepositoryMock);

            TokenResponseDto? result = await userService.RefreshTokenAsync("invalid-token");

            result.Should().BeNull();

            userRepositoryMock.Verify(repository => repository.GetByRefreshTokenAsync("invalid-token"), Times.Once);
        }

        [Fact]
        public async Task RefreshTokenAsync_ShouldReturnNull_WhenRefreshTokenIsExpired()
        {
            string expiredToken = "expired-refresh-token";
            User user = new User
            {
                Id = Guid.NewGuid(),
                Name = "Larissa",
                Email = "larissa@email.com",
                CreatedAt = DateTime.UtcNow,
                RefreshToken = expiredToken,
                RefreshTokenExpiry = DateTime.UtcNow.AddDays(-1)
            };

            Mock<IUserRepository> userRepositoryMock = new Mock<IUserRepository>();

            userRepositoryMock
                .Setup(repository => repository.GetByRefreshTokenAsync(expiredToken))
                .ReturnsAsync(user);

            UserService userService = CreateUserService(userRepositoryMock);

            TokenResponseDto? result = await userService.RefreshTokenAsync(expiredToken);

            result.Should().BeNull();

            userRepositoryMock.Verify(repository => repository.GetByRefreshTokenAsync(expiredToken), Times.Once);
        }

        // JWT TOKEN TESTS

        [Fact]
        public async Task LoginAsync_GeneratedToken_ShouldContainUserClaims()
        {
            LoginUserDto dto = new LoginUserDto
            {
                Email = "larissa@email.com",
                Password = "123456"
            };

            string hashedPassword = BCrypt.Net.BCrypt.HashPassword("123456");
            Guid userId = Guid.NewGuid();

            User user = new User
            {
                Id = userId,
                Name = "Larissa",
                Email = "larissa@email.com",
                PasswordHash = hashedPassword,
                CreatedAt = DateTime.UtcNow
            };

            Mock<IUserRepository> userRepositoryMock = new Mock<IUserRepository>();

            userRepositoryMock
                .Setup(repository => repository.GetByEmailAsync(dto.Email))
                .ReturnsAsync(user);

            userRepositoryMock
                .Setup(repository => repository.UpdateAsync(It.IsAny<User>()))
                .Returns(Task.CompletedTask);

            UserService userService = CreateUserService(userRepositoryMock);

            TokenResponseDto? result = await userService.LoginAsync(dto);

            result.Should().NotBeNull();
            result!.Token.Should().NotBeNullOrEmpty();

            System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler handler =
                new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();

            System.IdentityModel.Tokens.Jwt.JwtSecurityToken jwt =
                handler.ReadJwtToken(result.Token);

            jwt.Subject.Should().Be(userId.ToString());
        }

        // PUT TESTS
        [Fact]
        public async Task UpdateAsync_ShouldUpdateUserSuccessfully_WhenDataIsValid()
        {
            Guid userId = Guid.NewGuid();
            UpdateUserDto dto = new UpdateUserDto
            {
                Name = "Larissa Updated",
                Email = "larissa.updated@email.com"
            };

            User existingUser = new User
            {
                Id = userId,
                Name = "Larissa",
                Email = "larissa@email.com",
                CreatedAt = DateTime.UtcNow
            };

            Mock<IUserRepository> userRepositoryMock = new Mock<IUserRepository>();
            userRepositoryMock
                .Setup(repository => repository.GetByIdAsync(userId))
                .ReturnsAsync(existingUser);
            userRepositoryMock
                .Setup(repository => repository.GetByEmailAsync(dto.Email))
                .ReturnsAsync((User?)null);

            UserService userService = CreateUserService(userRepositoryMock);

            UserResponseDto? result = await userService.UpdateAsync(userId, dto);

            result.Should().NotBeNull();
            result!.Id.Should().Be(userId);
            result.Name.Should().Be("Larissa Updated");
            result.Email.Should().Be("larissa.updated@email.com");

            userRepositoryMock.Verify(repository => repository.GetByIdAsync(userId), Times.Once);
            userRepositoryMock.Verify(repository => repository.GetByEmailAsync(dto.Email), Times.Once);
        }

        // DELETE TESTS

        [Fact]
        public async Task DeleteAsync_ShouldDeleteUserSuccessfully_WhenUserExists()
        {
            Guid userId = Guid.NewGuid();

            Mock<IUserRepository> userRepositoryMock = new Mock<IUserRepository>();

            userRepositoryMock
                .Setup(repository => repository.GetByIdAsync(userId))
                .ReturnsAsync(new User { Id = userId });

            userRepositoryMock
                .Setup(repository => repository.DeleteAsync(It.IsAny<User>()))
                .Returns(Task.CompletedTask);

            UserService userService = CreateUserService(userRepositoryMock);

            bool result = await userService.DeleteAsync(userId);

            result.Should().BeTrue();

            userRepositoryMock.Verify(repository => repository.GetByIdAsync(userId), Times.Once);
            userRepositoryMock.Verify(repository => repository.DeleteAsync(It.IsAny<User>()), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnFalse_WhenUserDoesNotExist()
        {
            Guid userId = Guid.NewGuid();

            Mock<IUserRepository> userRepositoryMock = new Mock<IUserRepository>();

            userRepositoryMock
                .Setup(repository => repository.GetByIdAsync(userId))
                .ReturnsAsync((User?)null);

            UserService userService = CreateUserService(userRepositoryMock);

            bool result = await userService.DeleteAsync(userId);

            result.Should().BeFalse();

            userRepositoryMock.Verify(repository => repository.GetByIdAsync(userId), Times.Once);
            userRepositoryMock.Verify(repository => repository.DeleteAsync(It.IsAny<User>()), Times.Never);
        }
    }
}
