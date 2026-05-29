using Identity.API.Entities;

namespace Identity.API.Interfaces;

public interface IJwtService
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();
    (string Token, DateTime ExpiresAt) GenerateTokenPair(User user);
    Guid? ValidateToken(string token);
}
