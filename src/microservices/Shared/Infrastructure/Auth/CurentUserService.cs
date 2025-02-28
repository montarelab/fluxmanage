using Common.Auth;

public class CurrentUserService : ICurrentUserService
{
    public Guid GetUserId()
    {
        return Guid.Empty;
    }
}