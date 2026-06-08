using DebugMeBackend.DTOs.User;
using DebugMeBackend.Entities;
using DebugMeBackend.Repositories.Interfaces;

namespace DebugMeBackend.Services
{
    public class UserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<UserResponseDto> CreateAsync(CreateUserDto dto)
        {
            string normalizedEmail = dto.Email.Trim().ToLower();

            User? existingUser = await _userRepository.GetByEmailAsync(normalizedEmail);

            if (existingUser is not null)
            {
                throw new InvalidOperationException("Já existe um usuário com este e-mail.");
            }

            User user = new User
            {
                Name = dto.Name.Trim(),
                Email = normalizedEmail,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password)
            };

            await _userRepository.AddAsync(user);

            return MapToResponse(user);
        }

        public async Task<UserResponseDto?> GetByIdAsync(Guid id)
        {
            User? user = await _userRepository.GetByIdAsync(id);

            if (user is null)
            {
                return null;
            }

            return MapToResponse(user);
        }

        public async Task<List<UserResponseDto>> GetAllAsync()
        {
            List<User> users = await _userRepository.GetAllAsync();

            List<UserResponseDto> response = users
                .Select(MapToResponse)
                .ToList();

            return response;
        }

        public async Task<bool> LoginAsync(LoginUserDto dto)
        {
            string normalizedEmail = dto.Email.Trim().ToLower();

            User? user = await _userRepository.GetByEmailAsync(normalizedEmail);

            if (user is null)
            {
                return false;
            }

            return BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);
        }

        public async Task<UserResponseDto?> UpdateAsync(Guid id, UpdateUserDto dto)
        {
            User? user = await _userRepository.GetByIdAsync(id);

            if (user is null)
            {
                return null;
            }

            string normalizedEmail = dto.Email.Trim().ToLower();

            User? userWithSameEmail = await _userRepository.GetByEmailAsync(normalizedEmail);

            if (userWithSameEmail is not null && userWithSameEmail.Id != id)
            {
                throw new InvalidOperationException("Este e-mail já está em uso por outro usuário.");
            }

            user.Name = dto.Name.Trim();
            user.Email = normalizedEmail;

            await _userRepository.UpdateAsync(user);

            return MapToResponse(user);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            User? user = await _userRepository.GetByIdAsync(id);

            if (user is null)
            {
                return false;
            }

            await _userRepository.DeleteAsync(user);

            return true;
        }

        private static UserResponseDto MapToResponse(User user)
        {
            UserResponseDto response = new UserResponseDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                CreatedAt = user.CreatedAt
            };

            return response;
        }
    }
}