namespace Auth.JWT;

public interface ITokenService
{
    string GenerateToken(string username);
}