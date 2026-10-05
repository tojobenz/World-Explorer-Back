namespace AuthApi.Domain.Interfaces;

public interface IAuthService
{
    string HashPassword(string password);
    bool VerifyPassword(string password, string hash);
    string GenerateJwtToken(Guid userId, string email);
    string GenerateRefreshToken();
}
