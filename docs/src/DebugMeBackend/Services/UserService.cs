using System.Security.Cryptography;
using System.Text;
using DebugMeBackend.DTOs.User;
using DebugMeBackend.Entities;

namespace DebugMeBackend.Services
{
    public class UserService
    {
        private static readonly List<User> _users = new();

        public UserResponseDto Create(CreateUserDto dto)
        {
            var emailAlreadyExists = _users.Any(u => u.Email.ToLower() == dto.Email.ToLower());

            if (emailAlreadyExists)
            {
                throw new InvalidOperationException("Já existe um usuário com este e-mail.");
            }

            var user = new User
            {
                Name = dto.Name.Trim(),
                Email = dto.Email.Trim().ToLower(),
                PasswordHash = HashPassword(dto.Password)
            };

            _users.Add(user);

            return MapToResponse(user);
        }

        public UserResponseDto? GetById(Guid id)
        {
            var user = _users.FirstOrDefault(u => u.Id == id);

            if (user is null)
            {
                return null;
            }

            return MapToResponse(user);
        }

        public List<UserResponseDto> GetAll()
        {
            return _users.Select(MapToResponse).ToList();
        }

        public bool Login(LoginUserDto dto)
        {
            var user = _users.FirstOrDefault(u => u.Email.ToLower() == dto.Email.ToLower());

            if (user is null)
            {
                return false;
            }

            var passwordHash = HashPassword(dto.Password);

            return user.PasswordHash == passwordHash;
        }

        public UserResponseDto? Update(Guid id, UpdateUserDto dto)
        {
            var user = _users.FirstOrDefault(u => u.Id == id);

            if (user is null)
            {
                return null;
            }

            var emailAlreadyInUse = _users.Any(u =>
                u.Id != id &&
                u.Email.ToLower() == dto.Email.ToLower());

            if (emailAlreadyInUse)
            {
                throw new InvalidOperationException("Este e-mail já está em uso por outro usuário.");
            }

            user.Name = dto.Name.Trim();
            user.Email = dto.Email.Trim().ToLower();

            return MapToResponse(user);
        }

        private static UserResponseDto MapToResponse(User user)
        {
            return new UserResponseDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                CreatedAt = user.CreatedAt
            };
        }

        private static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        public bool Delete(Guid id)
        {
            User? user = _users.FirstOrDefault(u => u.Id == id);

            if (user is null)
            {
                return false;
            }

            _users.Remove(user);

            return true;
        }
    }
}