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
    }
}