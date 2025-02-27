namespace Common.Auth;

public interface ICurrentUserService
{
    Guid GetUserId();
}