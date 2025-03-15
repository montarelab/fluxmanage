namespace Auth.JWT;

public interface ITokenService
{
    string GenerateAccessToken(string username);
    string GenerateRefreshToken();
}