using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using DebugMeBackend.DTOs.User;
using DebugMeBackend.Entities;
using DebugMeBackend.Repositories.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DebugMeBackend.Services
{
    public class UserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;

        public UserService(IUserRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration;
        }

        public async Task<TokenResponseDto> CreateAsync(CreateUserDto dto)
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

            string token = GenerateJwtToken(user);
            string refreshToken = GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(
                int.Parse(_configuration["Jwt:RefreshTokenExpiryDays"] ?? "7"));

            await _userRepository.AddAsync(user);

            return new TokenResponseDto
            {
                Token = token,
                RefreshToken = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddMinutes(
                    int.Parse(_configuration["Jwt:AccessTokenExpiryMinutes"] ?? "60")),
                User = MapToResponse(user)
            };
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

        public async Task<TokenResponseDto?> LoginAsync(LoginUserDto dto)
        {
            string normalizedEmail = dto.Email.Trim().ToLower();

            User? user = await _userRepository.GetByEmailAsync(normalizedEmail);

            if (user is null)
            {
                return null;
            }

            bool passwordValid = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);

            if (!passwordValid)
            {
                return null;
            }

            string token = GenerateJwtToken(user);
            string refreshToken = GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(
                int.Parse(_configuration["Jwt:RefreshTokenExpiryDays"] ?? "7"));
            await _userRepository.UpdateAsync(user);

            return new TokenResponseDto
            {
                Token = token,
                RefreshToken = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddMinutes(
                    int.Parse(_configuration["Jwt:AccessTokenExpiryMinutes"] ?? "60")),
                User = MapToResponse(user)
            };
        }

        public async Task<TokenResponseDto?> RefreshTokenAsync(string refreshToken)
        {
            User? user = await _userRepository.GetByRefreshTokenAsync(refreshToken);

            if (user is null || user.RefreshTokenExpiry is null || user.RefreshTokenExpiry < DateTime.UtcNow)
            {
                return null;
            }

            string newToken = GenerateJwtToken(user);
            string newRefreshToken = GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(
                int.Parse(_configuration["Jwt:RefreshTokenExpiryDays"] ?? "7"));
            await _userRepository.UpdateAsync(user);

            return new TokenResponseDto
            {
                Token = newToken,
                RefreshToken = newRefreshToken,
                ExpiresAt = DateTime.UtcNow.AddMinutes(
                    int.Parse(_configuration["Jwt:AccessTokenExpiryMinutes"] ?? "60")),
                User = MapToResponse(user)
            };
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

        private string GenerateJwtToken(User user)
        {
            string secret = _configuration["Jwt:Secret"]
                ?? throw new InvalidOperationException("JWT Secret is not configured.");

            SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            SigningCredentials credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            Claim[] claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            JwtSecurityToken token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(
                    int.Parse(_configuration["Jwt:AccessTokenExpiryMinutes"] ?? "60")),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private static string GenerateRefreshToken()
        {
            byte[] randomBytes = RandomNumberGenerator.GetBytes(64);
            return Convert.ToBase64String(randomBytes);
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