using DebugMeBackend.DTOs.User;
using DebugMeBackend.Entities;
using DebugMeBackend.Repositories.Interfaces;
using DebugMeBackend.Services;
using FluentAssertions;
using Moq;

namespace DebugMeBackend.Tests.Services
{
    public class UserServiceTests
    {
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

            userRepositoryMock
                .Setup(repository => repository.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            UserService userService = new UserService(userRepositoryMock.Object);

            UserResponseDto result = await userService.CreateAsync(dto);

            result.Should().NotBeNull();
            result.Name.Should().Be("Larissa");
            result.Email.Should().Be("larissa@email.com");
            result.Id.Should().NotBeEmpty();
            result.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));

            userRepositoryMock.Verify(repository => repository.GetByEmailAsync(dto.Email), Times.Once);
            userRepositoryMock.Verify(repository => repository.AddAsync(It.IsAny<User>()), Times.Once);
            userRepositoryMock.Verify(repository => repository.SaveChangesAsync(), Times.Once);
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

            UserService userService = new UserService(userRepositoryMock.Object);

            Func<Task<UserResponseDto>> act = () => userService.CreateAsync(dto);

            await act.Should().ThrowAsync<InvalidOperationException>();
        }

        [Fact]
        public async Task CreateAsync_ShouldTrimAndNormalizeEmail_WhenCreatingUser()
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

            userRepositoryMock
                .Setup(repository => repository.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            UserService userService = new UserService(userRepositoryMock.Object);

            UserResponseDto result = await userService.CreateAsync(dto);

            result.Should().NotBeNull();
            result.Name.Should().Be("Larissa");
            result.Email.Should().Be("larissa@email.com");
            result.Id.Should().NotBeEmpty();
            result.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));

            userRepositoryMock.Verify(repository => repository.GetByEmailAsync(dto.Email), Times.Once);
            userRepositoryMock.Verify(repository => repository.AddAsync(It.IsAny<User>()), Times.Once);
            userRepositoryMock.Verify(repository => repository.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_ShouldThrowInvalidOperationException_WhenNameIsNullOrEmpty()
        {
            CreateUserDto dto = new CreateUserDto
            {
                Name = "",
                Email = "larissa@email.com",
                Password = "123456"
            };

            Mock<IUserRepository> userRepositoryMock = new Mock<IUserRepository>();

            UserService userService = new UserService(userRepositoryMock.Object);

            Func<Task<UserResponseDto>> act = () => userService.CreateAsync(dto);

            await act.Should().ThrowAsync<InvalidOperationException>();
        }

        [Fact]
        public async Task CreateAsync_ShouldThrowInvalidOperationException_WhenNameIsUnderTwoCharacters()
        {
            CreateUserDto dto = new CreateUserDto
            {
                Name = "L",
                Email = "larissa@email.com",
                Password = "123456"
            };

            Mock<IUserRepository> userRepositoryMock = new Mock<IUserRepository>();

            UserService userService = new UserService(userRepositoryMock.Object);

            Func<Task<UserResponseDto>> act = () => userService.CreateAsync(dto);

            await act.Should().ThrowAsync<InvalidOperationException>();
        }

        [Fact]
        public async Task CreateAsync_ShouldThrowInvalidOperationException_WhenNameIsMoreThan100Characters()
        {
            CreateUserDto dto = new CreateUserDto
            {
                Name = new string('L', 101),
                Email = "larissa@email.com",
                Password = "123456"
            };

            Mock<IUserRepository> userRepositoryMock = new Mock<IUserRepository>();

            UserService userService = new UserService(userRepositoryMock.Object);

            Func<Task<UserResponseDto>> act = () => userService.CreateAsync(dto);

            await act.Should().ThrowAsync<InvalidOperationException>();
        }

        [Fact]
        public async Task CreateAsync_ShouldThrowInvalidOperationException_WhenEmailIsNullOrEmpty()
        {
            CreateUserDto dto = new CreateUserDto
            {
                Name = "Larissa",
                Email = "",
                Password = "123456"
            };

            Mock<IUserRepository> userRepositoryMock = new Mock<IUserRepository>();

            UserService userService = new UserService(userRepositoryMock.Object);

            Func<Task<UserResponseDto>> act = () => userService.CreateAsync(dto);

            await act.Should().ThrowAsync<InvalidOperationException>();
        }

        [Fact]
        public async Task CreateAsync_ShouldThrowInvalidOperationException_WhenEmailIsMoreThan150Characters()
        {
            CreateUserDto dto = new CreateUserDto
            {
                Name = "Larissa",
                Email = new string('L', 151),
                Password = "123456"
            };

            Mock<IUserRepository> userRepositoryMock = new Mock<IUserRepository>();

            UserService userService = new UserService(userRepositoryMock.Object);

            Func<Task<UserResponseDto>> act = () => userService.CreateAsync(dto);

            await act.Should().ThrowAsync<InvalidOperationException>();
        }

        [Fact]
        public async Task CreateAsync_ShouldThrowInvalidOperationException_WhenPasswordIsNullOrEmpty()
        {
            CreateUserDto dto = new CreateUserDto
            {
                Name = "Larissa",
                Email = "larissa@email.com",
                Password = ""
            };

            Mock<IUserRepository> userRepositoryMock = new Mock<IUserRepository>();

            UserService userService = new UserService(userRepositoryMock.Object);

            Func<Task<UserResponseDto>> act = () => userService.CreateAsync(dto);

            await act.Should().ThrowAsync<InvalidOperationException>();
        }

        [Fact]
        public async Task CreateAsync_ShouldThrowInvalidOperationException_WhenPasswordIsUnderSixCharacters()
        {
            CreateUserDto dto = new CreateUserDto
            {
                Name = "Larissa",
                Email = "larissa@email.com",
                Password = new string('L', 5)
            };

            Mock<IUserRepository> userRepositoryMock = new Mock<IUserRepository>();

            UserService userService = new UserService(userRepositoryMock.Object);

            Func<Task<UserResponseDto>> act = () => userService.CreateAsync(dto);

            await act.Should().ThrowAsync<InvalidOperationException>();
        }

        [Fact]
        public async Task CreateAsync_ShouldThrowInvalidOperationException_WhenPasswordIsMoreThan100Characters()
        {
            CreateUserDto dto = new CreateUserDto
            {
                Name = "Larissa",
                Email = "larissa@email.com",
                Password = new string('L', 101)
            };

            Mock<IUserRepository> userRepositoryMock = new Mock<IUserRepository>();

            UserService userService = new UserService(userRepositoryMock.Object);

            Func<Task<UserResponseDto>> act = () => userService.CreateAsync(dto);

            await act.Should().ThrowAsync<InvalidOperationException>();
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

            UserService userService = new UserService(userRepositoryMock.Object);

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

            UserService userService = new UserService(userRepositoryMock.Object);

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

            UserService userService = new UserService(userRepositoryMock.Object);

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

            UserService userService = new UserService(userRepositoryMock.Object);

            UserResponseDto? result = await userService.GetByIdAsync(userId);

            result.Should().BeNull();

            userRepositoryMock.Verify(repository => repository.GetByIdAsync(userId), Times.Once);
        }

        // PUT TESTS
        [Fact]
        public async Task UpdateAsync_ShouldUpdateUserSuccessfully_WhenDataIsValid()
        {
            // Arrange
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
            userRepositoryMock
                .Setup(repository => repository.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            UserService userService = new UserService(userRepositoryMock.Object);

            // Act
            UserResponseDto? result = await userService.UpdateAsync(userId, dto);

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().Be(userId);
            result.Name.Should().Be("Larissa Updated");
            result.Email.Should().Be("larissa.updated@email.com");

            userRepositoryMock.Verify(repository => repository.GetByIdAsync(userId), Times.Once);
            userRepositoryMock.Verify(repository => repository.GetByEmailAsync(dto.Email), Times.Once);
            userRepositoryMock.Verify(repository => repository.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ShouldThrowInvalidOperationException_WhenEmailIsInvalid()
        {
            Guid userId = Guid.NewGuid();
            UpdateUserDto dto = new UpdateUserDto
            {
                Name = "Larissa Updated",
                Email = "l"
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

            UserService userService = new UserService(userRepositoryMock.Object);

            Func<Task<UserResponseDto?>> act = () => userService.UpdateAsync(userId, dto);

            await act.Should().ThrowAsync<InvalidOperationException>();
        }

        [Fact]
        public async Task UpdateAsync_ShouldThrowInvalidOperationException_WhenNameIsInvalid()
        {
            Guid userId = Guid.NewGuid();
            UpdateUserDto dto = new UpdateUserDto
            {
                Name = "L",
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

            UserService userService = new UserService(userRepositoryMock.Object);

            Func<Task<UserResponseDto?>> act = () => userService.UpdateAsync(userId, dto);

            await act.Should().ThrowAsync<InvalidOperationException>();
        }

        [Fact]
        public async Task UpdateAsync_ShouldThrowInvalidOperationException_WhenEmailIsUnderFiveCharacters()
        {
            UpdateUserDto dto = new UpdateUserDto
            {
                Name = "Larissa Updated",
                Email = "a@c"
            };

            Mock<IUserRepository> userRepositoryMock = new Mock<IUserRepository>();

            userRepositoryMock
                .Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new User
                {
                    Id = Guid.NewGuid(),
                    Name = "Larissa",
                    Email = "larissa@email.com",
                    PasswordHash = "hash"
                });

            UserService userService = new UserService(userRepositoryMock.Object);

            Func<Task<UserResponseDto?>> act = () => userService.UpdateAsync(Guid.NewGuid(), dto);

            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("O e-mail deve ter entre 5 e 150 caracteres.");
        }

        [Fact]
        public async Task UpdateAsync_ShouldThrowInvalidOperationException_WhenEmailIsMoreThan150Characters()
        {
            string longEmail = new string('a', 141) + "@teste.com"; 

            UpdateUserDto dto = new UpdateUserDto
            {
                Name = "Larissa Updated",
                Email = longEmail
            };

            Mock<IUserRepository> userRepositoryMock = new Mock<IUserRepository>();

            userRepositoryMock
                .Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new User
                {
                    Id = Guid.NewGuid(),
                    Name = "Larissa",
                    Email = "larissa@email.com",
                    PasswordHash = "hash"
                });

            UserService userService = new UserService(userRepositoryMock.Object);

            Func<Task<UserResponseDto?>> act = () => userService.UpdateAsync(Guid.NewGuid(), dto);

            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("O e-mail deve ter entre 5 e 150 caracteres.");
        }

                [Fact]
        public async Task UpdateAsync_ShouldThrowInvalidOperationException_WhenNameIsUnderTwoCharacters()
        {
            UpdateUserDto dto = new UpdateUserDto
            {
                Name = "L",
                Email = "larissa.updated@email.com"
            };

            Mock<IUserRepository> userRepositoryMock = new Mock<IUserRepository>();

            userRepositoryMock
                .Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new User
                {
                    Id = Guid.NewGuid(),
                    Name = "Larissa",
                    Email = "larissa@email.com",
                    PasswordHash = "hash"
                });

            UserService userService = new UserService(userRepositoryMock.Object);

            Func<Task<UserResponseDto?>> act = () => userService.UpdateAsync(Guid.NewGuid(), dto);

            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("O nome deve ter entre 2 e 100 caracteres.");
        }

        [Fact]
        public async Task UpdateAsync_ShouldThrowInvalidOperationException_WhenNameIsMoreThan150Characters()
        {
            string longName = new string('a', 141); 

            UpdateUserDto dto = new UpdateUserDto
            {
                Name = longName,
                Email = "larissa@email.com"
            };

            Mock<IUserRepository> userRepositoryMock = new Mock<IUserRepository>();

            userRepositoryMock
                .Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new User
                {
                    Id = Guid.NewGuid(),
                    Name = "Larissa",
                    Email = "larissa@email.com",
                    PasswordHash = "hash"
                });

            UserService userService = new UserService(userRepositoryMock.Object);

            Func<Task<UserResponseDto?>> act = () => userService.UpdateAsync(Guid.NewGuid(), dto);

            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("O nome deve ter entre 2 e 100 caracteres.");
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

            userRepositoryMock
                .Setup(repository => repository.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            UserService userService = new UserService(userRepositoryMock.Object);

            bool result = await userService.DeleteAsync(userId);

            result.Should().BeTrue();

            userRepositoryMock.Verify(repository => repository.GetByIdAsync(userId), Times.Once);
            userRepositoryMock.Verify(repository => repository.DeleteAsync(It.IsAny<User>()), Times.Once);
            userRepositoryMock.Verify(repository => repository.SaveChangesAsync(), Times.Once);
        }

                [Fact]

        public async Task DeleteAsync_ShouldReturnFalse_WhenUserDoesNotExist()
        {
            Guid userId = Guid.NewGuid();

            Mock<IUserRepository> userRepositoryMock = new Mock<IUserRepository>();

            userRepositoryMock
                .Setup(repository => repository.GetByIdAsync(userId))
                .ReturnsAsync((User?)null);

            userRepositoryMock
                .Setup(repository => repository.DeleteAsync(It.IsAny<User>()))
                .Returns(Task.CompletedTask);

            userRepositoryMock
                .Setup(repository => repository.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            UserService userService = new UserService(userRepositoryMock.Object);

            bool result = await userService.DeleteAsync(userId);

            result.Should().BeFalse();

            userRepositoryMock.Verify(repository => repository.GetByIdAsync(userId), Times.Once);
            userRepositoryMock.Verify(repository => repository.DeleteAsync(It.IsAny<User>()), Times.Never);
            userRepositoryMock.Verify(repository => repository.SaveChangesAsync(), Times.Never);
        }
    }   
}