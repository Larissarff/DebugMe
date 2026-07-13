namespace DebugMeBackend.DTOs.User;

public class TokenResponseDto
{
    public string Token { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public UserResponseDto User { get; set; } = null!;
}
